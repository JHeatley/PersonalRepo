"""A small GPT in the Llama layout (RMSNorm, rotary positions, SwiGLU, no biases), written out in plain PyTorch.

Every layer has the same name and shape as in Hugging Face's LlamaForCausalLM, so export.py can save it in that format
and llama.cpp's converter turns it into a .gguf file for Ollama, like any Llama model.
"""
import math
from dataclasses import dataclass, asdict

import torch
import torch.nn as nn
import torch.nn.functional as F


@dataclass
class Config:
    vocab_size: int = 8000
    dim: int = 384           # width of every token's vector
    layers: int = 8          # transformer blocks
    heads: int = 6           # attention heads (dim must divide by it)
    hidden: int = 1024       # width inside each block's feed-forward part
    context: int = 512       # longest text it sees at once, in tokens
    rope_theta: float = 10000.0
    norm_eps: float = 1e-5

    def to_dict(self):
        return asdict(self)


# Model sizes. The parameter counts include the 8,000-token vocabulary.
SIZES = {
    "test": Config(dim=64, layers=2, heads=4, hidden=192, context=128),       # 0.6M: for checking the code runs
    "tiny": Config(dim=384, layers=8, heads=6, hidden=1024, context=512),     # 17M: 1-3 hours on an RTX 2060
    "small": Config(dim=512, layers=12, heads=8, hidden=1408, context=512),   # 43M: 6-12 hours on an RTX 2060
    "medium": Config(dim=768, layers=12, heads=12, hidden=2048, context=1024),  # 91M: 1-2 days; or hours on a rented GPU
}


class RMSNorm(nn.Module):
    def __init__(self, dim, eps):
        super().__init__()
        self.eps = eps
        self.weight = nn.Parameter(torch.ones(dim))

    def forward(self, x):
        h = x.float()
        h = h * torch.rsqrt(h.pow(2).mean(-1, keepdim=True) + self.eps)
        return self.weight * h.to(x.dtype)


def rope_tables(cfg, length, device):
    """cos and sin for rotary positions (Hugging Face's layout: the two halves of each head are rotated together)."""
    head = cfg.dim // cfg.heads
    inv = 1.0 / (cfg.rope_theta ** (torch.arange(0, head, 2, device=device).float() / head))
    t = torch.arange(length, device=device).float()
    freqs = torch.outer(t, inv)
    emb = torch.cat([freqs, freqs], dim=-1)
    return emb.cos(), emb.sin()


def rotate_half(x):
    a, b = x.chunk(2, dim=-1)
    return torch.cat([-b, a], dim=-1)


class Attention(nn.Module):
    def __init__(self, cfg):
        super().__init__()
        self.heads = cfg.heads
        self.q_proj = nn.Linear(cfg.dim, cfg.dim, bias=False)
        self.k_proj = nn.Linear(cfg.dim, cfg.dim, bias=False)
        self.v_proj = nn.Linear(cfg.dim, cfg.dim, bias=False)
        self.o_proj = nn.Linear(cfg.dim, cfg.dim, bias=False)

    def forward(self, x, cos, sin):
        b, t, d = x.shape
        q = self.q_proj(x).view(b, t, self.heads, -1).transpose(1, 2)
        k = self.k_proj(x).view(b, t, self.heads, -1).transpose(1, 2)
        v = self.v_proj(x).view(b, t, self.heads, -1).transpose(1, 2)
        cos, sin = cos[:t].to(q.dtype), sin[:t].to(q.dtype)
        q = q * cos + rotate_half(q) * sin
        k = k * cos + rotate_half(k) * sin
        y = F.scaled_dot_product_attention(q, k, v, is_causal=True)  # each token only sees the ones before it
        return self.o_proj(y.transpose(1, 2).reshape(b, t, d))


class MLP(nn.Module):
    def __init__(self, cfg):
        super().__init__()
        self.gate_proj = nn.Linear(cfg.dim, cfg.hidden, bias=False)
        self.up_proj = nn.Linear(cfg.dim, cfg.hidden, bias=False)
        self.down_proj = nn.Linear(cfg.hidden, cfg.dim, bias=False)

    def forward(self, x):
        return self.down_proj(F.silu(self.gate_proj(x)) * self.up_proj(x))  # SwiGLU


class Block(nn.Module):
    def __init__(self, cfg):
        super().__init__()
        self.input_layernorm = RMSNorm(cfg.dim, cfg.norm_eps)
        self.self_attn = Attention(cfg)
        self.post_attention_layernorm = RMSNorm(cfg.dim, cfg.norm_eps)
        self.mlp = MLP(cfg)

    def forward(self, x, cos, sin):
        x = x + self.self_attn(self.input_layernorm(x), cos, sin)
        return x + self.mlp(self.post_attention_layernorm(x))


class Inner(nn.Module):
    """The part Hugging Face calls "model" (everything but the output layer), so the weights' names match."""

    def __init__(self, cfg):
        super().__init__()
        self.embed_tokens = nn.Embedding(cfg.vocab_size, cfg.dim)
        self.layers = nn.ModuleList(Block(cfg) for _ in range(cfg.layers))
        self.norm = RMSNorm(cfg.dim, cfg.norm_eps)


class TinyLLM(nn.Module):
    def __init__(self, cfg: Config):
        super().__init__()
        self.cfg = cfg
        self.model = Inner(cfg)
        # The output layer shares its weights with the input embedding (tied): saves vocab x dim parameters.
        self.lm_head = nn.Linear(cfg.dim, cfg.vocab_size, bias=False)
        self.lm_head.weight = self.model.embed_tokens.weight
        self.apply(self._init)
        # GPT-2's trick: the layers that add into the residual stream start smaller, so a deep stack starts stable.
        for name, p in self.named_parameters():
            if name.endswith("o_proj.weight") or name.endswith("down_proj.weight"):
                nn.init.normal_(p, mean=0.0, std=0.02 / math.sqrt(2 * cfg.layers))
        self._rope = None

    @staticmethod
    def _init(m):
        if isinstance(m, (nn.Linear, nn.Embedding)):
            nn.init.normal_(m.weight, mean=0.0, std=0.02)

    def params(self):
        return sum(p.numel() for p in self.parameters())

    def rope(self, device):
        if self._rope is None or self._rope[0].device != torch.device(device):
            self._rope = rope_tables(self.cfg, self.cfg.context, device)
        return self._rope

    def forward(self, idx, targets=None, mask=None):
        """Logits for every position; with targets, also the loss (mask: 1 where a target counts, e.g. only replies)."""
        cos, sin = self.rope(idx.device)
        x = self.model.embed_tokens(idx)
        for layer in self.model.layers:
            x = layer(x, cos, sin)
        logits = self.lm_head(self.model.norm(x))
        if targets is None:
            return logits, None
        loss = F.cross_entropy(logits.float().view(-1, logits.size(-1)), targets.view(-1), reduction="none")
        if mask is None:
            return logits, loss.mean()
        m = mask.view(-1).float()
        return logits, (loss * m).sum() / m.sum().clamp(min=1)

    @torch.no_grad()
    def generate(self, ids, max_new=200, temperature=0.8, top_k=40, stop=()):
        """Continues the token list ids, one token at a time (temperature 0: always the most likely)."""
        self.eval()
        device = next(self.parameters()).device
        out = list(ids)
        for _ in range(max_new):
            ctx = torch.tensor([out[-self.cfg.context:]], device=device)
            logits, _ = self(ctx)
            logits = logits[0, -1].float()
            if temperature <= 0:
                nxt = int(logits.argmax())
            else:
                logits = logits / temperature
                if top_k:
                    kth = torch.topk(logits, min(top_k, logits.numel())).values[-1]
                    logits[logits < kth] = -float("inf")
                nxt = int(torch.multinomial(F.softmax(logits, dim=-1), 1))
            if nxt in stop:
                break
            out.append(nxt)
        return out[len(ids):]


def save(path, model, extra=None):
    import os
    os.makedirs(os.path.dirname(path), exist_ok=True)
    state = {"config": model.cfg.to_dict(), "model": model.state_dict()}
    state.update(extra or {})
    torch.save(state, path + ".tmp")
    os.replace(path + ".tmp", path)  # a crash mid-save can't destroy the last good checkpoint


def load(path, device="cpu"):
    state = torch.load(path, map_location=device, weights_only=False)
    model = TinyLLM(Config(**state["config"])).to(device)
    model.load_state_dict(state["model"])
    return model, state
