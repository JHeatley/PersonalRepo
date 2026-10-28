using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Pokémon from the Recife Regional decks, names K to Z. Registered by printing key.</summary>
    public static class RecifePokemonB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEG-55", new Kadabra());
            r.RegisterByKey("ASC-121", new KoraidonEx());
            r.RegisterByKey("SFA-47", new Kyurem());
            r.RegisterByKey("SSP-76", new LatiasEx());
            r.RegisterByKey("JTG-56", new LilliesClefairyEx());
            r.RegisterByKey("30C-83", new Lucario30C());
            r.RegisterByKey("MEG-74", new Lunatone());
            r.RegisterByKey("PBL-65", new MegaExcadrillEx());
            r.RegisterByKey("MEG-104", new MegaKangaskhanEx());
            r.RegisterByKey("PFL-84", new MegaLopunnyEx());
            r.RegisterByKey("MEG-77", new MegaLucarioEx());
            r.RegisterByKey("PFL-61", new MegaSharpedoEx());
            r.RegisterByKey("POR-55", new MegaSkarmoryEx());
            r.RegisterByKey("MEG-10", new Meganium());
            r.RegisterByKey("CRI-61", new MetagrossCRI());
            r.RegisterByKey("TEF-114", new MetangTEF());
            r.RegisterByKey("30C-66", new MewEx());
            r.RegisterByKey("CRI-72", new RecoilMon("Take Down", 10));
            r.RegisterByKey("PFL-14", new Moltres());
            r.RegisterByKey("JTG-27", new NsDarmanitan());
            r.RegisterByKey("JTG-96", new NsPurrloin());
            r.RegisterByKey("SCR-115", new Noctowl());
            r.RegisterByKey("SSP-18", new PaldeanTauros());
            r.RegisterByKey("SSP-111", new Passimian());
            r.RegisterByKey("CRI-70", new Patrat());
            r.RegisterByKey("PFL-34", new Pawmot());
            r.RegisterByKey("SVP-149", new Pecharunt());
            r.RegisterByKey("PBL-66", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("PFL-27", new SearchAttackMon("Call for Support", d => d.IsSupporter, 1));
            r.RegisterByKey("PBL-5", new Poltchageist());
            r.RegisterByKey("ASC-39", new Psyduck());
            r.RegisterByKey("TEF-24", new Rabsca());
            r.RegisterByKey("SCR-111", new ThunderburstStormMon("Thunderburst Storm", 30));
            r.RegisterByKey("TEF-123", new RagingBoltEx());
            r.RegisterByKey("TEF-23", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("30C-14", new Reshiram30C());
            r.RegisterByKey("MEG-76", new LockAfterAttackMon("Accelerating Stab", true));
            r.RegisterByKey("PRE-21", new Seaking());
            r.RegisterByKey("30C-84", new Seismitoad());
            r.RegisterByKey("DRI-10", new Shaymin());
            r.RegisterByKey("PBL-33", new HideNSneakMon());
            r.RegisterByKey("PBL-6", new Sinistcha());
            r.RegisterByKey("SCR-58", new Slowking());
            r.RegisterByKey("SCR-57", new Slowpoke());
            r.RegisterByKey("SSP-75", new Smoochum());
            r.RegisterByKey("MEG-75", new Solrock());
            r.RegisterByKey("ASC-98", new Spectrier());
            r.RegisterByKey("DRI-145", new StevensMetagrossEx());
            r.RegisterByKey("DRI-144", new LockAfterAttackMon("Metal Slash", false));
            r.RegisterByKey("ASC-62", new Stunfisk());
            r.RegisterByKey("SFA-6", new RecoilMon("Wood Hammer", 30));
            r.RegisterByKey("TWM-131", new Tatsugiri());
            r.RegisterByKey("TWM-25", new TealMaskOgerponEx());
            r.RegisterByKey("DRI-51", new TeamRocketsArticuno());
            r.RegisterByKey("ASC-127", new TeamRocketsHonchkrow());
            r.RegisterByKey("ASC-162", new TeamRocketsKangaskhanEx());
            r.RegisterByKey("DRI-81", new TeamRocketsMewtwoEx());
            r.RegisterByKey("DRI-87", new TeamRocketsMimikyu());
            r.RegisterByKey("DRI-127", new TeamRocketsMurkrow());
            r.RegisterByKey("DRI-153", new TeamRocketsPorygon());
            r.RegisterByKey("DRI-154", new TeamRocketsPorygon2());
            r.RegisterByKey("DRI-128", new TeamRocketsSneasel());
            r.RegisterByKey("DRI-20", new TeamRocketsSpidops());
            r.RegisterByKey("DRI-19", new RecoilMon("Take Down", 10));
            r.RegisterByKey("TWM-15", new Thwackey());
            r.RegisterByKey("DRI-40", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("PBL-68", new Toucannon());
            r.RegisterByKey("PFL-67", new CallForFamilyMon());
            r.RegisterByKey("PFL-68", new Toxtricity());
            r.RegisterByKey("PBL-67", new DodgeMon("Fly", true));
            r.RegisterByKey("30C-72", new Unown());
            r.RegisterByKey("TWM-64", new WellspringMaskOgerponEx());
            r.RegisterByKey("30C-100", new Yveltal30C());
            r.RegisterByKey("DRI-78", new Zeraora());
        }
    }

    // ================================================================== K, L

    public sealed class Kadabra : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            if (TriggerOnce(ctx, "Psychic Draw", ctx.Me.Deck.Count > 0, "Use Psychic Draw to draw 2 cards?")) ctx.Draw(2);
        }
    }

    public sealed class KoraidonEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Orichalcum Fang")
            {
                ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutByAttackLastOpponentTurn() ? 120 : 0));
                return true;
            }
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantUseAttackNextTurn(atk.Name);
            return true;
        }
    }

    public sealed class Kyurem : Mon
    {
        public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
        {
            if (mon != src.Pokemon || attack.Name != "Trifrost") return;
            if (!src.Opp.Discard.Any(c => c.Def.Name.Contains("Colress"))) return;
            cost.Clear();
            cost.Add(EnergyType.Colorless);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            var all = ctx.OpponentPokemon();
            int n = Math.Min(3, all.Count);
            foreach (var t in ctx.ChoosePokemonMany(all, n, 3, "Trifrost: choose 3 of your opponent's Pokémon")) ctx.DamageTarget(t, atk.Damage);
            return true;
        }
    }

    public sealed class LatiasEx : Mon
    {
        public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
        {
            return mon.Owner == src.Player && mon.Def.IsBasicPokemon ? 0 : cost;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    public sealed class LilliesClefairyEx : Mon
    {
        public override EnergyType? ModifyWeakness(EffectContext src, PokemonInPlay mon, EnergyType? weakness)
        {
            return mon.Owner != src.Player && mon.Def.PokemonType == EnergyType.Dragon ? EnergyType.Psychic : weakness;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int benched = ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
            ctx.DamageDefending(atk.Damage + 20 * benched);
            return true;
        }
    }

    public sealed class Lucario30C : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.DamageOneBenched(60);
            return true;
        }
    }

    public sealed class Lunatone : Mon
    {
        protected override string Activated { get { return "Lunar Cycle"; } }
        protected override bool OnePerPlayer { get { return true; } }
        private static List<CardInstance> FightingInHand(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fighting).ToList(); }

        protected override bool AbilityReady(EffectContext ctx)
        {
            return ctx.MyPokemon().Any(m => m.Def.Name == "Solrock") && FightingInHand(ctx).Count > 0 && ctx.Me.Deck.Count > 0;
        }

        protected override void Ability(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(FightingInHand(ctx), 1, 1, "Lunar Cycle: choose a Basic Fighting Energy to discard")[0];
            ctx.Engine.DiscardFromHand(ctx.Player, energy);
            ctx.Draw(3);
        }
    }

    // ================================================================== M

    public sealed class MegaExcadrillEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Undermine")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                return true;
            }
            ctx.DamageDefending(atk.Damage + (ExtraEnergy(ctx, atk, 2) ? 130 : 0));
            return true;
        }
    }

    public sealed class MegaKangaskhanEx : Mon
    {
        protected override string Activated { get { return "Run Errand"; } }
        protected override bool OnePerPlayer { get { return true; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }
        protected override void Ability(EffectContext ctx) { ctx.Draw(2); }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + 50 * ctx.FlipUntilTails());
            return true;
        }
    }

    public sealed class MegaLopunnyEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Gale Thrust")
            {
                bool moved = ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn;
                ctx.DamageDefending(atk.Damage + (moved ? 170 : 0));
                return true;
            }
            ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
            return true;
        }
    }

    public sealed class MegaLucarioEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Aura Jab")
            {
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (energy.Count > 0 && ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(energy, 3, (c, m) => !ctx.IsActive(m), "Aura Jab: attach Basic Fighting Energy to your Benched Pokémon");
            }
            else if (atk.Name == "Mega Brave")
            {
                ctx.SelfCantUseAttackNextTurn("Mega Brave");
            }
            return true;
        }
    }

    public sealed class MegaSharpedoEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Greedy Fang")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.Draw(2);
                return true;
            }
            ctx.DamageDefending(atk.Damage + (ctx.Pokemon.DamageCounters > 0 ? 150 : 0));
            return true;
        }
    }

    public sealed class MegaSkarmoryEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.Engine.ShuffleEnergyIntoDeck(ctx.Pokemon, ctx.Pokemon.Energy.ToList());
            ctx.DamageAnyOpponent(220);
            return true;
        }
    }

    /// <summary>Wild Growth: each Basic Grass Energy on your Pokémon provides 2 Grass Energy (doesn't stack).</summary>
    public sealed class Meganium : Mon
    {
        public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
        {
            if (holder.Owner != src.Player || !energy.Def.IsBasicEnergy || energy.Def.EnergyProvides != EnergyType.Grass) return;
            units.Clear();
            units.Add(EnergyMask.Of(EnergyType.Grass));
            units.Add(EnergyMask.Of(EnergyType.Grass));
        }
    }

    public sealed class MetagrossCRI : Mon
    {
        private static List<CardInstance> MetalEnergy(EffectContext ctx)
        {
            return ctx.Pokemon.Energy.Where(e => ctx.Engine.EnergyUnitsOf(ctx.Pokemon, e).Any(u => EnergyMask.Provides(u, EnergyType.Metal))).ToList();
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Bounce Back")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.OpponentSwitchesActive();
                return true;
            }
            if (atk.Name != "Metallic Hammer") return false;
            int bonus = 0;
            var metal = MetalEnergy(ctx);
            if (metal.Count >= 3 && ctx.AskYesNo("Metallic Hammer: discard 3 Metal Energy for 150 more damage?"))
            {
                var picks = ctx.ChooseCards(metal, 3, 3, "Choose 3 Metal Energy to discard");
                int n = 0;
                foreach (var e in picks) if (ctx.Engine.DiscardEnergy(ctx.Pokemon, e)) n++;
                if (n == 3) bonus = 150;
            }
            ctx.DamageDefending(atk.Damage + bonus);
            return true;
        }
    }

    /// <summary>Metal Maker: look at the top 4, attach any Basic Metal Energy there, the rest go to the bottom (shuffled).</summary>
    public sealed class MetangTEF : Mon
    {
        protected override string Activated { get { return "Metal Maker"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var top = ctx.LookAtTop(4);
            foreach (var c in top) ctx.Me.Deck.Remove(c);
            var metal = top.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Metal).ToList();
            var attached = new List<CardInstance>();
            for (int i = 0; i < metal.Count; i++)
            {
                var left = metal.Where(c => !attached.Contains(c)).ToList();
                var pick = ctx.ChooseCards(left, 0, 1, "Metal Maker: attach a Basic Metal Energy (" + (i + 1) + " of up to " + metal.Count + ")");
                if (pick.Count == 0) break;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Metal Maker: attach " + pick[0].Def.Name + " to which Pokémon?");
                ctx.Engine.AttachEnergyCard(pick[0], target);
                attached.Add(pick[0]);
            }
            var rest = top.Where(c => !attached.Contains(c)).ToList();
            ctx.State.Rng.Shuffle(rest);
            ctx.Me.Deck.AddRange(rest);
            if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + rest.Count + " cards on the bottom of their deck.");
        }
    }

    /// <summary>Memory Helix: can use the attacks of your Benched Pokémon.</summary>
    public sealed class MewEx : Mon
    {
        public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
        {
            foreach (var b in src.Me.Bench)
                for (int i = 0; i < b.Def.Attacks.Count; i++)
                    yield return new KeyValuePair<CardDef, int>(b.Def, i);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (ctx.Me.Bench.Count > 0 && ctx.PokemonIsActive && ctx.AskYesNo("Teleportation Burst: switch this Pokémon with 1 of your Benched Pokémon?"))
                ctx.SwitchSelfWithBench();
            return true;
        }
    }

    public sealed class Moltres : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            bool ex = ctx.Defending != null && ctx.Defending.Def.IsEx;
            ctx.DamageDefending(atk.Damage + (ex ? 90 : 0));
            return true;
        }
    }

    // ================================================================== N, O, P

    public sealed class NsDarmanitan : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Back Draft")
            {
                ctx.DamageDefending(atk.Damage * ctx.Opp.Discard.Count(c => c.Def.IsBasicEnergy));
                return true;
            }
            ctx.DamageDefending(atk.Damage);
            ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            ctx.DamageOneBenched(90);
            return true;
        }
    }

    public sealed class NsPurrloin : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            var hand = ctx.Opp.Hand.ToList();
            if (hand.Count == 0) return true;
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed their hand: " + string.Join(", ", hand.Select(c => c.Def.Name).ToArray()) + ".");
            var pick = ctx.ChooseCards(hand, 1, 1, "Thieving Swipe: put a card from your opponent's hand on the bottom of their deck")[0];
            ctx.Opp.Hand.Remove(pick);
            ctx.Opp.Deck.Add(pick);
            ctx.Log(pick.Def.Name + " was put on the bottom of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck.");
            return true;
        }
    }

    public sealed class Noctowl : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            bool ready = ctx.Engine.HasTeraInPlay(ctx.Player) && ctx.Me.Deck.Count > 0;
            if (TriggerOnce(ctx, "Jewel Seeker", ready, "Use Jewel Seeker to search your deck for up to 2 Trainer cards?"))
                ctx.SearchToHand(d => d.IsTrainer, 2, "Jewel Seeker: choose up to 2 Trainer cards");
        }
    }

    public sealed class PaldeanTauros : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Spirited Tackle") return false;
            bool stage1 = ctx.Defending != null && ctx.Defending.Def.Stage == Stage.Stage1;
            ctx.DamageDefending(atk.Damage + (stage1 ? 90 : 0));
            return true;
        }
    }

    public sealed class Passimian : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.Def.IsBasicPokemon).Count);
            return true;
        }
    }

    public sealed class Patrat : Mon
    {
        public override bool BlocksCounterMovement(EffectContext src) { return true; }
    }

    public sealed class Pawmot : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(defending) && ctx.AskYesNo("Voltaic Fist: do 60 damage to this Pokémon to Paralyze the opponent's Active Pokémon?"))
            {
                ctx.DamageSelf(60);
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
            }
            return true;
        }
    }

    public sealed class Pecharunt : Mon
    {
        public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters)
        {
            return src.PokemonIsActive && mon.Owner != src.Player ? counters + 5 : counters;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!ctx.InPlay(defending)) return true;
            ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            ctx.DefendingCantRetreat(defending);
            return true;
        }
    }

    public sealed class Poltchageist : HideNSneakMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.PlaceCounters(ctx.Defending, 1);
            return true;
        }
    }

    /// <summary>Damp: Abilities that require the Pokémon using them to Knock Out itself don't exist.</summary>
    public sealed class Psyduck : Mon
    {
        public override bool IsAbilityBlocker { get { return true; } }
        public override bool LockDependsOnSpot { get { return false; } }

        public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
        {
            return src.Engine.Scripts.For(mon.Def).AbilityKnocksOutSelf(abilityName);
        }
    }

    // ================================================================== R

    public sealed class Rabsca : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            return info.AgainstOpponent && info.Target.Owner == src.Player && !info.TargetIsActive ? 0 : damage;
        }

        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return kind == EffectSourceKind.Attack && target.Owner == src.Player && !src.IsActive(target);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + 30 * ctx.EnergyCount(ctx.Defending));
            return true;
        }
    }

    /// <summary>Used for Thunderburst Storm, Power Whip (the attack name and the numbers are passed in).</summary>
    public sealed class ThunderburstStormMon : Mon
    {
        private readonly string attack;
        private readonly int each;

        public ThunderburstStormMon(string attack, int each)
        {
            this.attack = attack;
            this.each = each;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            int dmg = each * ctx.EnergyCount(ctx.Pokemon);
            if (dmg > 0) ctx.DamageAnyOpponent(dmg);
            return true;
        }
    }

    public sealed class RagingBoltEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Burst Roar")
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(6);
                return true;
            }
            var basics = ctx.MyPokemon().SelectMany(m => m.Energy.Where(e => e.Def.IsBasicEnergy)).ToList();
            var picks = ctx.ChooseCards(basics, 0, basics.Count, "Bellowing Thunder: discard any amount of Basic Energy from your Pokémon (70 damage each)");
            int n = 0;
            foreach (var e in picks)
            {
                var holder = ctx.MyPokemon().FirstOrDefault(m => m.Energy.Contains(e));
                if (holder != null && ctx.Engine.DiscardEnergy(holder, e)) n++;
            }
            ctx.DamageDefending(atk.Damage * n);
            return true;
        }
    }

    public sealed class Reshiram30C : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Laser Flame") return false;
            ctx.DamageDefending(atk.Damage + (ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Lightning) ? 80 : 0));
            return true;
        }
    }

    // ================================================================== S

    public sealed class Seaking : FestivalLeadMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.Draw(2);
            return true;
        }
    }

    public sealed class Seismitoad : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Quaking Fist") return false;
            ctx.DamageDefending(atk.Damage);
            ctx.AddEffect(new TrainerFlipEffect("Quaking Fist: " + ctx.Engine.PlayerName(ctx.Opponent) + " flips a coin before each Trainer card next turn", ctx.Player, ctx.Opponent, ctx.Turn + 1));
            return true;
        }
    }

    public sealed class Shaymin : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            var t = info.Target;
            return info.AgainstOpponent && t.Owner == src.Player && !info.TargetIsActive && !t.Def.HasRuleBox ? 0 : damage;
        }
    }

    public sealed class Sinistcha : HideNSneakMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (HideNSneakInDiscard(ctx) < 6) return Nothing(ctx);
            foreach (var m in ctx.OpponentPokemon()) ctx.PlaceCounters(m, 4);
            return true;
        }
    }

    /// <summary>Seek Inspiration: discard the top card; if it's a Pokémon without a Rule Box, use 1 of its attacks.</summary>
    public sealed class Slowking : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Seek Inspiration") return false;
            var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
            if (top.Count == 0) return true;
            var d = top[0].Def;
            if (!d.IsPokemon || d.HasRuleBox || d.Attacks.Count == 0) return true;
            int index = ChooseAttackOf(ctx, d, "Seek Inspiration: choose 1 of " + d.Name + "'s attacks to use");
            if (index >= 0) UseCopiedAttack(ctx, d, index);
            return true;
        }
    }

    public sealed class Slowpoke : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Dangle Tail") return false;
            ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList(), 1, 1, "Dangle Tail: put a Pokémon into your hand"));
            return true;
        }
    }

    public sealed class Smoochum : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
            var target = ctx.ChoosePokemon(ctx.MyBench(), "Delightful Kiss: attach Basic Psychic Energy to which Benched Pokémon?");
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Psychic, 2, "Delightful Kiss: choose up to 2 Basic Psychic Energy");
            foreach (var e in found) ctx.Engine.AttachEnergyCard(e, target);
            ctx.ShuffleMyDeck();
            return true;
        }
    }

    public sealed class Solrock : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (!ctx.Me.Bench.Any(m => m.Def.Name == "Lunatone")) return Nothing(ctx);
            ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
            return true;
        }
    }

    public sealed class Spectrier : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Phantasmal Barrage") return false;
            ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            var all = ctx.OpponentPokemon();
            if (all.Count > 0) ctx.PlaceCounters(ctx.ChoosePokemon(all, "Phantasmal Barrage: put 12 damage counters on which Pokémon?"), 12);
            return true;
        }
    }

    /// <summary>X-Boot: search for a Basic Psychic Energy, a Basic Metal Energy or both; attach to your Psychic / Metal Pokémon.</summary>
    public sealed class StevensMetagrossEx : Mon
    {
        protected override string Activated { get { return "X-Boot"; } }

        protected override bool AbilityReady(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Psychic) || ctx.Engine.IsType(m, EnergyType.Metal)).Count > 0;
        }

        protected override void Ability(EffectContext ctx)
        {
            AttachOne(ctx, EnergyType.Psychic);
            AttachOne(ctx, EnergyType.Metal);
            ctx.ShuffleMyDeck();
        }

        private static void AttachOne(EffectContext ctx, EnergyType type)
        {
            var targets = ctx.MyPokemon(m => ctx.Engine.IsType(m, type));
            if (targets.Count == 0) return;
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == type, 1, "X-Boot: you may choose a Basic " + type + " Energy");
            if (found.Count == 0) return;
            var target = ctx.ChoosePokemon(targets, "X-Boot: attach " + found[0].Def.Name + " to which " + type + " Pokémon?");
            ctx.Engine.AttachEnergyCard(found[0], target);
        }
    }

    /// <summary>Pouncing Trap: can't retreat next turn; during your next turn it takes 100 more damage from attacks.</summary>
    public sealed class Stunfisk : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!ctx.InPlay(defending)) return true;
            ctx.DefendingCantRetreat(defending);
            var trap = new DamageTakenEffect(defending.Name + " takes 100 more damage from attacks during " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Pouncing Trap)",
                ctx.Player, defending.Uid, ctx.Turn + 2, 100);
            trap.ActiveFromTurn = ctx.Turn + 2;
            ctx.AddEffect(trap);
            return true;
        }
    }

    // ================================================================== T

    public sealed class Tatsugiri : Mon
    {
        protected override string Activated { get { return "Attract Customers"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var top = ctx.LookAtTop(6);
            var picks = ctx.ChooseCards(top.Where(c => c.Def.IsSupporter).ToList(), 0, 1, "Attract Customers: put a Supporter into your hand");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class TealMaskOgerponEx : Mon
    {
        protected override string Activated { get { return "Teal Dance"; } }
        private static List<CardInstance> GrassInHand(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList(); }
        protected override bool AbilityReady(EffectContext ctx) { return GrassInHand(ctx).Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(GrassInHand(ctx), 1, 1, "Teal Dance: choose a Basic Grass Energy from your hand to attach to " + ctx.Pokemon.Name)[0];
            ctx.Me.Hand.Remove(energy);
            ctx.Engine.AttachEnergyCard(energy, ctx.Pokemon, true);
            ctx.Draw(1);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int energy = ctx.EnergyCount(ctx.Pokemon) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
            ctx.DamageDefending(atk.Damage + 30 * energy);
            return true;
        }
    }

    public sealed class TeamRocketsArticuno : Mon
    {
        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return kind == EffectSourceKind.Attack && target.Owner == src.Player && target.Def.IsBasicPokemon && target.Def.IsTeamRockets;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            bool trEnergy = ctx.Pokemon.Energy.Any(e => e.Def.Name == "Team Rocket's Energy");
            ctx.DamageDefending(atk.Damage + (trEnergy ? 60 : 0));
            return true;
        }
    }

    public sealed class TeamRocketsHonchkrow : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Rocket Feathers") return false;
            var tr = ctx.Me.Hand.Where(c => RecifeTrainers.IsTeamRocketSupporter(c.Def)).ToList();
            var picks = ctx.ChooseCards(tr, 0, tr.Count, "Rocket Feathers: discard any number of Team Rocket Supporters (60 damage each)");
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
            ctx.DamageDefending(atk.Damage * picks.Count);
            return true;
        }
    }

    public sealed class TeamRocketsKangaskhanEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Comet Punch")
            {
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(4));
                return true;
            }
            var played = ctx.Me.SupporterNameThisTurn;
            bool tr = played != null && played.Contains("Team Rocket");
            ctx.DamageDefending(atk.Damage + (tr ? 100 : 0));
            return true;
        }
    }

    public sealed class TeamRocketsMewtwoEx : Mon
    {
        public override bool CanUseAttack(EffectContext ctx, int attackIndex)
        {
            // Power Saver only limits this Pokémon (not a Pokémon copying its attack).
            if (ctx.Pokemon.Def.Name != "Team Rocket's Mewtwo ex" || ctx.Engine.AbilityBlocked(ctx.Pokemon, "Power Saver")) return true;
            return TeamRocketsInPlay(ctx) >= 4;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int n = 0;
            for (int k = 0; k < 2; k++)
            {
                var energy = ctx.Me.Bench.SelectMany(m => m.Energy).ToList();
                if (energy.Count == 0) break;
                var pick = ctx.ChooseCards(energy, 0, 1, "Erasure Ball: discard an Energy from your Benched Pokémon for 60 more damage (" + (k + 1) + " of up to 2)");
                if (pick.Count == 0) break;
                var holder = ctx.Me.Bench.First(m => m.Energy.Contains(pick[0]));
                if (ctx.Engine.DiscardEnergy(holder, pick[0])) n++;
            }
            ctx.DamageDefending(atk.Damage + 60 * n);
            return true;
        }
    }

    public sealed class TeamRocketsMimikyu : Mon
    {
        public override bool CanUseAttack(EffectContext ctx, int attackIndex)
        {
            var d = ctx.Opp.Active;
            return d != null && d.Def.IsTera && d.Def.Attacks.Count > 0;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var d = ctx.Defending;
            if (d == null || !d.Def.IsTera) return Nothing(ctx);
            CardDef copied;
            int index;
            if (ChooseCopiedAttack(ctx, d, "Gemstone Mimicry: choose 1 of " + d.Name + "'s attacks to use", out copied, out index))
                UseCopiedAttack(ctx, copied, index);
            return true;
        }
    }

    public sealed class TeamRocketsMurkrow : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Deceit")
            {
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsSupporter, 1, "Deceit: choose a Supporter");
                return true;
            }
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!ctx.InPlay(defending)) return true;
            int index = ChooseAttackOf(ctx, defending.Def, "Torment: choose 1 of the Defending Pokémon's attacks");
            if (index < 0) return true;
            string name = defending.Def.Attacks[index].Name;
            ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (Torment)", ctx.Player, defending.Uid, ctx.Turn + 1, name));
            return true;
        }
    }

    public sealed class TeamRocketsPorygon : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (ctx.Me.Hand.Count == 0) return true;
            var mine = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Hacking: discard a card from your hand")[0];
            ctx.Engine.DiscardFromHand(ctx.Player, mine);
            if (ctx.Opp.Hand.Count == 0) return true;
            var theirs = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Hacking: discard a card from your hand", ctx.Source)[0];
            ctx.Engine.DiscardFromHand(ctx.Opponent, theirs);
            return true;
        }
    }

    public sealed class TeamRocketsPorygon2 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => RecifeTrainers.IsTeamRocketSupporter(c.Def)));
            return true;
        }
    }

    public sealed class TeamRocketsSneasel : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Strike the Sleeper") return false;
            var bench = ctx.OpponentBench();
            if (bench.Count == 0) return true;
            var t = ctx.ChoosePokemon(bench, "Strike the Sleeper: choose 1 of your opponent's Benched Pokémon");
            ctx.DamageTarget(t, 20 * t.DamageCounters);
            return true;
        }
    }

    public sealed class TeamRocketsSpidops : Mon
    {
        protected override string Activated { get { return "Charging Up"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var e = ctx.ChooseCards(ctx.BasicEnergyInDiscard(), 1, 1, "Charging Up: attach a Basic Energy from your discard pile to this Pokémon")[0];
            ctx.Me.Discard.Remove(e);
            ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * TeamRocketsInPlay(ctx));
            return true;
        }
    }

    public sealed class Thwackey : Mon
    {
        protected override string Activated { get { return "Boom Boom Groove"; } }

        protected override bool AbilityReady(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0 && ctx.Engine.HasAbilityNamed(ctx.Me.Active, "Festival Lead");
        }

        protected override void Ability(EffectContext ctx)
        {
            ctx.SearchToHand(d => true, 1, "Boom Boom Groove: choose any card", false, 1);
        }
    }

    public sealed class Toucannon : Mon
    {
        protected override string Activated { get { return "Aerial Draw"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
        protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + 20 * (ctx.Me.Bench.Count + ctx.Opp.Bench.Count));
            return true;
        }
    }

    public sealed class Toxtricity : Mon
    {
        protected override string Activated { get { return "Sinister Surge"; } }

        protected override bool AbilityReady(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0 && ctx.Me.Bench.Any(m => ctx.Engine.IsType(m, EnergyType.Darkness));
        }

        protected override void Ability(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Darkness, 1, "Sinister Surge: choose a Basic Darkness Energy");
            if (found.Count > 0)
            {
                var target = ctx.ChoosePokemon(ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Darkness)).ToList(), "Sinister Surge: attach it to which Benched Darkness Pokémon?");
                ctx.Engine.AttachEnergyCard(found[0], target);
                ctx.PlaceCounters(target, 2);
            }
            ctx.ShuffleMyDeck();
        }
    }

    // ================================================================== U - Z

    public sealed class Unown : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int owner = ctx.Player;
            ctx.AddEffect(new ExtraPrizeEffect("Mysterious Signal: take 1 more Prize card for a Knock Out by this attack", owner, ctx.Turn,
                (e, ko) => ko.HitByAttack && ko.HitByPlayer == owner && !ko.ForcedKnockOut));
            ctx.DamageDefending(atk.Damage);
            return true;
        }
    }

    public sealed class WellspringMaskOgerponEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Sob")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
            ctx.DamageDefending(atk.Damage);
            // Compendium (Torrential Pump with Double Turbo Energy): with fewer than 3 Energy you shuffle in as many as you can,
            // and as long as some go in, the 120 to the Bench still happens.
            if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.Energy.Count > 0 && ctx.Opp.Bench.Count > 0
                && ctx.AskYesNo("Torrential Pump: shuffle 3 Energy from this Pokémon into your deck to do 120 damage to a Benched Pokémon?"))
            {
                int n = Math.Min(3, ctx.Pokemon.Energy.Count);
                var picks = ctx.ChooseCards(ctx.Pokemon.Energy.ToList(), n, n, "Choose " + n + " Energy to shuffle into your deck");
                if (ctx.Engine.ShuffleEnergyIntoDeck(ctx.Pokemon, picks) > 0) ctx.DamageOneBenched(120);
            }
            return true;
        }
    }

    public sealed class Yveltal30C : Mon
    {
        public override bool BlocksHealing(EffectContext src, PokemonInPlay mon)
        {
            return mon.Owner != src.Player && src.IsActive(mon);
        }
    }

    public sealed class Zeraora : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Thunder Raid") return false;
            ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            var exes = ctx.Opp.Bench.Where(m => m.Def.IsEx).ToList();
            if (exes.Count > 0) ctx.DamageTarget(ctx.ChoosePokemon(exes, "Thunder Raid: choose 1 of your opponent's Benched Pokémon ex"), 210);
            return true;
        }
    }
}
