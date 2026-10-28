using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Pokémon from the Recife Regional decks, names A to J. Registered by printing key.</summary>
    public static class RecifePokemonA
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEG-54", new AbraMEG());
            r.RegisterByKey("TWM-80", new AbraTWM());
            r.RegisterByKey("MEG-56", new AlakazamMEG());
            r.RegisterByKey("TWM-82", new AlakazamTWM());
            r.RegisterByKey("PBL-41", new AnnihilapePBL());
            r.RegisterByKey("SSP-100", new AnnihilapeSSP());
            r.RegisterByKey("TWM-126", new ApplinTWM126());
            r.RegisterByKey("TWM-17", new ApplinTWM17());
            r.RegisterByKey("DRI-23", new ArbolivaEx());
            r.RegisterByKey("PBL-12", new Armarouge());
            r.RegisterByKey("PBL-34", new Banette());
            r.RegisterByKey("MEG-9", new BayleefMEG());
            r.RegisterByKey("TEF-113", new BeldumTEF());
            r.RegisterByKey("JTG-24", new BlazikenEx());
            r.RegisterByKey("TWM-141", new BloodmoonUrsalunaEx());
            r.RegisterByKey("SCR-119", new BouffalantSCR());
            r.RegisterByKey("SSP-151", new BouffalantSSP());
            r.RegisterByKey("TEF-69", new Bronzong());
            r.RegisterByKey("TEF-68", new Bronzor());
            r.RegisterByKey("PFL-83", new Buneary());
            r.RegisterByKey("PFL-60", new Carvanha());
            r.RegisterByKey("MEG-12", new Celebi());
            r.RegisterByKey("SSP-36", new CeruledgeEx());
            r.RegisterByKey("PBL-59", new ChiYuPBL());
            r.RegisterByKey("TWM-39", new ChiYuTWM());
            r.RegisterByKey("SSP-56", new ChienPao());
            r.RegisterByKey("ASC-8", new ChikoritaASC());
            r.RegisterByKey("CRI-73", new CinccinoEx());
            r.RegisterByKey("SSP-83", new Cofagrigus());
            r.RegisterByKey("DRI-41", new Combusken());
            r.RegisterByKey("DRI-12", new Crustle());
            r.RegisterByKey("DRI-103", new CynthiasGabite());
            r.RegisterByKey("DRI-104", new CynthiasGarchompEx());
            r.RegisterByKey("DRI-102", new CynthiasGible());
            r.RegisterByKey("DRI-8", new CynthiasRoserade());
            r.RegisterByKey("DRI-129", new CynthiasSpiritomb());
            r.RegisterByKey("SSP-87", new Dedenne());
            r.RegisterByKey("CRI-32", new Deoxys());
            r.RegisterByKey("PBL-39", new Dhelmise());
            r.RegisterByKey("TWM-18", new Dipplin());
            r.RegisterByKey("DRI-22", new Dolliv());
            r.RegisterByKey("POR-52", new Drapion());
            r.RegisterByKey("PBL-46", new CallForFamilyMon());
            r.RegisterByKey("TEF-85", new DrilburTEF());
            r.RegisterByKey("TEF-128", new DunsparceTEF());
            r.RegisterByKey("PRE-36", new Dusclops());
            r.RegisterByKey("PRE-37", new Dusknoir());
            r.RegisterByKey("PRE-35", new Duskull());
            r.RegisterByKey("DRI-11", new Dwebble());
            r.RegisterByKey("BLK-40", new Elgyem());
            r.RegisterByKey("PFL-70", new EmpoleonEx());
            r.RegisterByKey("TWM-93", new Enamorus());
            r.RegisterByKey("30C-69", new Espeon());
            r.RegisterByKey("SCR-118", new FanRotom());
            r.RegisterByKey("TEF-78", new FlutterMane());
            r.RegisterByKey("WHT-44", new Frillish());
            r.RegisterByKey("SFA-40", new Genesect());
            r.RegisterByKey("BLK-67", new GenesectEx());
            r.RegisterByKey("TWM-44", new Goldeen());
            r.RegisterByKey("MEG-73", new Hariyama());
            r.RegisterByKey("TWM-123", new Heatran());
            r.RegisterByKey("SCR-114", new Hoothoot());
            r.RegisterByKey("JTG-138", new HopsCramorant());
            r.RegisterByKey("JTG-136", new HopsDubwool());
            r.RegisterByKey("ASC-95", new HopsPhantump());
            r.RegisterByKey("JTG-117", new HopsSnorlax());
            r.RegisterByKey("ASC-96", new HopsTrevenant());
            r.RegisterByKey("JTG-111", new HopsZacianEx());
            r.RegisterByKey("SCR-14", new HydrappleEx());
            r.RegisterByKey("TEF-81", new IronCrownEx());
            r.RegisterByKey("TEF-25", new IronLeavesEx());
            r.RegisterByKey("WHT-45", new JellicentEx());
            r.RegisterByKey("30C-102", new JirachiEx());
        }
    }

    // ------------------------------------------------------------------ Shared simple effects

    /// <summary>"This Pokémon also does N damage to itself" on one attack.</summary>
    public sealed class RecoilMon : Mon
    {
        private readonly string attack;
        private readonly int recoil;

        public RecoilMon(string attack, int recoil)
        {
            this.attack = attack;
            this.recoil = recoil;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(recoil);
            return true;
        }
    }

    /// <summary>Call for Family: put up to 2 Basic Pokémon from your deck onto your Bench.</summary>
    public sealed class CallForFamilyMon : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Call for Family") return false;
            if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => true, 2, "Call for Family: put up to 2 Basic Pokémon onto your Bench");
            return true;
        }
    }

    /// <summary>Attacks that search the deck for one kind of card and put it into the hand.</summary>
    public sealed class SearchAttackMon : Mon
    {
        private readonly string attack;
        private readonly Func<CardDef, bool> filter;
        private readonly int count;

        public SearchAttackMon(string attack, Func<CardDef, bool> filter, int count)
        {
            this.attack = attack;
            this.filter = filter;
            this.count = count;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(filter, count, atk.Name + ": search your deck");
            return true;
        }
    }

    /// <summary>"Flip N coins. This attack does X damage for each heads."</summary>
    public sealed class CoinsMon : Mon
    {
        private readonly string attack;
        private readonly int coins;

        public CoinsMon(string attack, int coins)
        {
            this.attack = attack;
            this.coins = coins;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage * ctx.FlipCoins(coins));
            return true;
        }
    }

    /// <summary>"During your next turn, this Pokémon can't attack" or "...can't use [this attack]".</summary>
    public sealed class LockAfterAttackMon : Mon
    {
        private readonly string attack;
        private readonly bool onlyThisAttack;

        public LockAfterAttackMon(string attack, bool onlyThisAttack)
        {
            this.attack = attack;
            this.onlyThisAttack = onlyThisAttack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            if (onlyThisAttack) ctx.SelfCantUseAttackNextTurn(atk.Name);
            else ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    /// <summary>"Flip a coin. If heads, during your opponent's next turn, prevent all damage from and effects of attacks done to this Pokémon."</summary>
    public sealed class DodgeMon : Mon
    {
        private readonly string attack;
        private readonly bool tailsDoesNothing;

        public DodgeMon(string attack, bool tailsDoesNothing)
        {
            this.attack = attack;
            this.tailsDoesNothing = tailsDoesNothing;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            bool heads = ctx.Flip();
            if (!heads && tailsDoesNothing) return Nothing(ctx);
            ctx.DamageDefending(atk.Damage);
            if (heads) ctx.ProtectSelfNextTurn();
            return true;
        }
    }

    /// <summary>"Switch this Pokémon with 1 of your Benched Pokémon" after the damage.</summary>
    public sealed class SwitchSelfMon : Mon
    {
        private readonly string attack;
        private readonly bool optional;

        public SwitchSelfMon(string attack, bool optional)
        {
            this.attack = attack;
            this.optional = optional;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Me.Bench.Count == 0) return true;
            if (!optional || ctx.AskYesNo(atk.Name + ": switch this Pokémon with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
            return true;
        }
    }

    /// <summary>"Draw N cards" after the damage.</summary>
    public sealed class DrawAttackMon : Mon
    {
        private readonly string attack;
        private readonly int cards;

        public DrawAttackMon(string attack, int cards)
        {
            this.attack = attack;
            this.cards = cards;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            ctx.Draw(cards);
            return true;
        }
    }

    /// <summary>"During your opponent's next turn, the Defending Pokémon can't retreat."</summary>
    public sealed class NoRetreatMon : Mon
    {
        private readonly string attack;

        public NoRetreatMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            ctx.DefendingCantRetreat(defending);
            return true;
        }
    }

    // ================================================================== A

    public sealed class AbraMEG : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SwitchSelfWithBench();
            return true;
        }
    }

    /// <summary>Teleporter: if Active, shuffle this Pokémon and everything attached into the deck.</summary>
    public sealed class AbraTWM : Mon
    {
        protected override string Activated { get { return "Teleporter"; } }
        // Without a Benched Pokémon this would lose the game on the spot, so it's only offered with one.
        protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Bench.Count > 0; }
        protected override void Ability(EffectContext ctx) { ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon); }
    }

    public sealed class AlakazamMEG : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            if (TriggerOnce(ctx, "Psychic Draw", ctx.Me.Deck.Count > 0, "Use Psychic Draw to draw 3 cards?")) ctx.Draw(3);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Powerful Hand") return false;
            ctx.PlaceCounters(ctx.Defending, 2 * ctx.Me.Hand.Count);
            return true;
        }
    }

    public sealed class AlakazamTWM : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Psychic")
            {
                ctx.DamageDefending(atk.Damage + 50 * ctx.EnergyCount(ctx.Defending));
                return true;
            }
            if (atk.Name != "Strange Hacking") return false;
            ctx.ApplyCondition(ctx.Defending, SpecialCondition.Confused);
            for (int guard = 0; guard < 30; guard++)
            {
                var from = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (from.Count == 0 || ctx.Opp.InPlay().Count() < 2) break;
                if (!ctx.AskYesNo("Strange Hacking: move damage counters between your opponent's Pokémon?")) break;
                var src = ctx.ChoosePokemon(from, "Move damage counters from which of your opponent's Pokémon?");
                int n = ctx.AskNumber(1, src.DamageCounters, "How many damage counters?");
                var dst = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != src), "Move them to which of your opponent's Pokémon?");
                ctx.Engine.MoveDamageCounters(src, dst, n, "Strange Hacking");
            }
            return true;
        }
    }

    public sealed class AnnihilapePBL : Mon
    {
        public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
        {
            return mon == src.Pokemon && src.Engine.FlipCoin(src.Player, "Durable Body");
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Ghostly Blow") return false;
            ctx.DamageDefending(atk.Damage);
            var bench = ctx.OpponentBench();
            if (bench.Count > 0) ctx.PlaceCounters(ctx.ChoosePokemon(bench, "Ghostly Blow: put 5 damage counters on which Benched Pokémon?"), 5);
            return true;
        }
    }

    public sealed class AnnihilapeSSP : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Tantrum")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
            if (atk.Name == "Destined Fight")
            {
                if (ctx.Defending != null) ctx.Engine.KnockOut(ctx.Defending, "Destined Fight");
                ctx.Engine.KnockOut(ctx.Pokemon, "Destined Fight");
                return true;
            }
            return false;
        }
    }

    public sealed class ApplinTWM126 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Find a Friend") return false;
            if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon, 1, "Find a Friend: choose a Pokémon");
            return true;
        }
    }

    public sealed class ApplinTWM17 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 20 : 0));
            return true;
        }
    }

    public sealed class ArbolivaEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Aroma Shot")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
            if (atk.Name != "Oil Salvo") return false;
            // Choose 6 times first, then each chosen Pokémon takes 20 for each time it was chosen.
            var hits = new Dictionary<PokemonInPlay, int>();
            var order = new List<PokemonInPlay>();
            for (int i = 0; i < 6; i++)
            {
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) break;
                var t = ctx.ChoosePokemon(all, "Oil Salvo: choose 1 of your opponent's Pokémon (" + (i + 1) + " of 6)");
                int n;
                hits.TryGetValue(t, out n);
                if (n == 0) order.Add(t);
                hits[t] = n + 1;
            }
            foreach (var t in order) ctx.DamageWith(t, 20 * hits[t], false, false, false);
            return true;
        }
    }

    public sealed class Armarouge : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int n = ctx.Me.Bench.Count(m => ctx.Engine.HasEnergyOfType(m, EnergyType.Fire));
            ctx.DamageDefending(atk.Damage + 40 * n);
            return true;
        }
    }

    // ================================================================== B

    public sealed class Banette : HideNSneakMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            // Compendium (Shinobi Blade): searching is optional, but if you search you must take a card.
            if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Puppet Pull: search your deck for a card?"))
                ctx.SearchToHand(d => true, 1, "Puppet Pull: choose a card to put into your hand", false, 1);
            return true;
        }
    }

    public sealed class BayleefMEG : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.OpponentSwitchesActive();
            return true;
        }
    }

    public sealed class BeldumTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Iron Tackle") return false;
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(10);
            return true;
        }
    }

    public sealed class BlazikenEx : Mon
    {
        protected override string Activated { get { return "Seething Spirit"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(ctx.BasicEnergyInDiscard(), 1, 1, "Seething Spirit: choose a Basic Energy from your discard pile")[0];
            var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Seething Spirit: attach " + energy.Def.Name + " to which Pokémon?");
            ctx.Me.Discard.Remove(energy);
            ctx.Engine.AttachEnergyCard(energy, target);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    public sealed class BloodmoonUrsalunaEx : Mon
    {
        public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
        {
            if (mon != src.Pokemon || attack.Name != "Blood Moon") return;
            for (int i = src.OpponentPrizesTaken; i > 0; i--) cost.Remove(EnergyType.Colorless);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    /// <summary>Curly Wall: with another Bouffalant in play, your Basic Colorless Pokémon take 60 less from attacks.</summary>
    public sealed class BouffalantSCR : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            var t = info.Target;
            if (!info.AgainstOpponent || t.Owner != src.Player || !t.Def.IsBasicPokemon || t.Def.PokemonType != EnergyType.Colorless) return damage;
            if (!src.MyPokemon().Any(m => m != src.Pokemon && m.Def.Name == "Bouffalant")) return damage;
            return info.ApplyOnce("Curly Wall") ? damage - 60 : damage;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    public sealed class BouffalantSSP : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Ready to Ram")
            {
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new RetaliateEffect("Ready to Ram: an attack that damages " + ctx.Pokemon.Name + " gets 6 damage counters back", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 6));
            }
            else if (atk.Name == "Smashing Headbutt")
            {
                ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Smashing Headbutt: discard 2 Energy from this Pokémon");
            }
            return true;
        }
    }

    public sealed class Bronzong : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Evolution Jammer") return false;
            ctx.DamageDefending(atk.Damage);
            ctx.Opp.EvolveLockTurn = ctx.Turn + 1;
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't evolve Pokémon from their hand during their next turn.");
            return true;
        }
    }

    public sealed class Bronzor : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            bool psychic = ctx.Defending != null && ctx.Engine.IsType(ctx.Defending, EnergyType.Psychic);
            ctx.DamageDefending(atk.Damage + (psychic ? 30 : 0));
            return true;
        }
    }

    public sealed class Buneary : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Run Around") return false;
            ctx.SwitchSelfWithBench();
            return true;
        }
    }

    // ================================================================== C

    public sealed class Carvanha : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(10);
            return true;
        }
    }

    public sealed class Celebi : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Traverse Time") return false;
            if (ctx.Me.Deck.Count > 0)
                ctx.SearchToHand(d => (d.IsPokemon && d.PokemonType == EnergyType.Grass) || d.IsStadiumCard, 3, "Traverse Time: choose up to 3 Grass Pokémon and/or Stadiums");
            return true;
        }
    }

    public sealed class CeruledgeEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Abyssal Flames")
            {
                ctx.DamageDefending(atk.Damage + 20 * ctx.Me.Discard.Count(c => c.Def.IsEnergy));
                return true;
            }
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Raging Amethyst") ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            return true;
        }
    }

    public sealed class ChiYuPBL : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int dmg = atk.Damage + (ctx.Pokemon.DamageCounters >= 2 ? 90 : 0);
            ctx.DamageWith(ctx.Defending, dmg, false, true, false);
            return true;
        }
    }

    public sealed class ChiYuTWM : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Allure")
            {
                ctx.Draw(2);
                return true;
            }
            if (atk.Name != "Ground Melter") return false;
            bool stadium = ctx.State.Stadium != null;
            ctx.DamageDefending(atk.Damage + (stadium ? 60 : 0));
            if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
            return true;
        }
    }

    public sealed class ChienPao : Mon
    {
        public override void OnPlayedFromHandToBench(EffectContext ctx)
        {
            if (!ctx.IsMyTurn || ctx.State.Stadium == null) return;
            if (!ctx.AskYesNo("Snow Sink: discard " + ctx.State.Stadium.Def.Name + "?")) return;
            Announce(ctx, "Snow Sink");
            ctx.Engine.DiscardStadium();
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.Energy.Count > 0)
            {
                var e = ctx.ChooseCards(ctx.Pokemon.Energy.ToList(), 1, 1, "Icicle Loop: put an Energy from this Pokémon into your hand")[0];
                ctx.Engine.ReturnEnergyToHand(ctx.Pokemon, e);
            }
            return true;
        }
    }

    public sealed class ChikoritaASC : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Growl") return false;
            var d = ctx.Defending;
            if (d != null)
                ctx.AddEffect(new WeakenAttackerEffect(d.Name + "'s attacks do 20 less damage during its next turn (Growl)", ctx.Player, d.Uid, ctx.Turn + 1, 20));
            return true;
        }
    }

    public sealed class CinccinoEx : Mon
    {
        public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
        {
            return info.Target == src.Pokemon && info.Attacker != null && src.Engine.FlipCoin(src.Player, "Smooth Coat");
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
            return true;
        }
    }

    public sealed class Cofagrigus : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Law of the Underworld") return false;
            var targets = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => ctx.Engine.HasAbility(m)).ToList();
            foreach (var t in targets) ctx.PlaceCounters(t, 6);
            return true;
        }
    }

    public sealed class Combusken : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Double Kick") return false;
            ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
            return true;
        }
    }

    /// <summary>
    /// Mysterious Rock Inn: no damage from the opponent's Pokémon ex. Superb Scissors: not affected by effects on the opponent's
    /// Active Pokémon, and (your ruling, v114) not by Resistance either; Weakness still applies.
    /// </summary>
    public sealed class Crustle : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            return info.Target == src.Pokemon && info.AgainstOpponent && info.Attacker.Def.IsEx ? 0 : damage;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
            return true;
        }
    }

    public sealed class CynthiasGabite : Mon
    {
        protected override string Activated { get { return "Champion's Call"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            ctx.SearchToHand(RecifeTrainers.IsCynthias, 1, "Champion's Call: choose a Cynthia's Pokémon");
        }
    }

    public sealed class CynthiasGarchompEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Corkscrew Dive")
            {
                if (ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Corkscrew Dive: draw until you have 6 cards?")) ctx.DrawUntil(6);
            }
            else if (atk.Name == "Draconic Buster")
            {
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            }
            return true;
        }
    }

    public sealed class CynthiasGible : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
            return true;
        }
    }

    public sealed class CynthiasRoserade : Mon
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            var a = info.Attacker;
            return a != null && a.Owner == src.Player && RecifeTrainers.IsCynthias(a.Def) && info.ToOpponentsActive ? damage + 30 : damage;
        }
    }

    public sealed class CynthiasSpiritomb : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int counters = ctx.Me.Bench.Where(m => RecifeTrainers.IsCynthias(m.Def)).Sum(m => m.DamageCounters);
            ctx.DamageWith(ctx.Defending, atk.Damage * counters, false, true, false);
            return true;
        }
    }

    // ================================================================== D

    public sealed class Dedenne : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Electromagnetic Sonar") return false;
            var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
            ctx.DiscardToHand(ctx.ChooseCards(trainers, 1, 1, "Electromagnetic Sonar: put a Trainer card into your hand"));
            return true;
        }
    }

    public sealed class Deoxys : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            bool extra = ExtraEnergy(ctx, atk, 2);
            ctx.DamageDefending(atk.Damage);
            if (extra) ctx.DamageOneBenched(120);
            return true;
        }
    }

    public sealed class Dhelmise : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + (HideNSneakInDiscard(ctx) >= 4 ? 140 : 0));
            return true;
        }
    }

    public sealed class Dipplin : FestivalLeadMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * ctx.Me.Bench.Count);
            return true;
        }
    }

    public sealed class Dolliv : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Nutrients") return false;
            var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
            if (hurt.Count > 0) ctx.Heal(ctx.ChoosePokemon(hurt, "Nutrients: heal 40 damage from which Pokémon?"), 40);
            return true;
        }
    }

    public sealed class Drapion : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Hazardous Tail") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(70);
            if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed | SpecialCondition.Poisoned);
            return true;
        }
    }

    public sealed class DrilburTEF : Mon
    {
        public override void OnPlayedFromHandToBench(EffectContext ctx)
        {
            if (!ctx.IsMyTurn || ctx.Me.Deck.Count == 0) return;
            if (!ctx.AskYesNo("Dig Dig Dig: discard up to 3 Basic Fighting Energy from your deck?")) return;
            Announce(ctx, "Dig Dig Dig");
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fighting, 3, "Dig Dig Dig: choose up to 3 Basic Fighting Energy to discard", false);
            ctx.Me.Discard.AddRange(found);
            if (found.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + found.Count + " Basic Fighting Energy from their deck.");
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class DunsparceTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Dig") return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Flip()) ctx.ProtectSelfNextTurn();
            return true;
        }
    }

    public sealed class Dusclops : CursedBlastMon
    {
        protected override int Counters { get { return 5; } }
    }

    public sealed class Dusknoir : CursedBlastMon
    {
        protected override int Counters { get { return 13; } }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Shadow Bind") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            ctx.DefendingCantRetreat(defending);
            return true;
        }
    }

    public sealed class Duskull : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Come and Get You") return false;
            int room = Math.Min(3, ctx.BenchSpace);
            var duskull = ctx.Me.Discard.Where(c => c.Def.Name == "Duskull").ToList();
            var picks = ctx.ChooseCards(duskull, 0, room, "Come and Get You: put up to 3 Duskull onto your Bench");
            foreach (var c in picks)
            {
                ctx.Me.Discard.Remove(c);
                ctx.Engine.PutOntoBench(ctx.Player, c, false);
            }
            return true;
        }
    }

    /// <summary>Ascension: evolve this Pokémon from the deck (an attack effect, so it ignores the usual timing limits).</summary>
    public sealed class Dwebble : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (ctx.Me.Deck.Count == 0) return true;
            string name = ctx.Pokemon.Name;
            var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Ascension: choose a card that evolves from " + name, false);
            if (found.Count > 0) ctx.Engine.EvolvePokemon(ctx.Pokemon, found[0]);
            ctx.ShuffleMyDeck();
            return true;
        }
    }

    // ================================================================== E

    public sealed class Elgyem : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Slight Shift") return false;
            var withEnergy = ctx.OpponentPokemon(m => m.Energy.Count > 0);
            if (withEnergy.Count == 0 || ctx.Opp.InPlay().Count() < 2) return true;
            var from = ctx.ChoosePokemon(withEnergy, "Slight Shift: move an Energy from which of your opponent's Pokémon?");
            var energy = ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Choose the Energy to move")[0];
            var to = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != from), "Move it to which of your opponent's Pokémon?");
            ctx.Engine.MoveEnergy(from, to, energy);
            return true;
        }
    }

    public sealed class EmpoleonEx : Mon
    {
        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return target == src.Pokemon && kind == EffectSourceKind.Attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfTakesLessNextTurn(60);
            return true;
        }
    }

    public sealed class Enamorus : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Love Resonance") return false;
            var theirs = new HashSet<EnergyType>(ctx.OpponentPokemon().Select(m => m.Def.PokemonType));
            bool match = ctx.MyPokemon().Any(m => theirs.Contains(m.Def.PokemonType));
            ctx.DamageDefending(atk.Damage + (match ? 120 : 0));
            return true;
        }
    }

    public sealed class Espeon : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Miraculous Shine") return false;
            foreach (var m in ctx.OpponentPokemon(x => x.Stack.Count > 1)) ctx.Engine.Devolve(m, 1);
            return true;
        }
    }

    // ================================================================== F

    public sealed class FanRotom : Mon
    {
        protected override string Activated { get { return "Fan Call"; } }
        protected override bool OnePerPlayer { get { return true; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.IsMyFirstTurn && ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            ctx.SearchToHand(d => d.IsPokemon && d.PokemonType == EnergyType.Colorless && d.Hp <= 100, 3, "Fan Call: choose up to 3 Colorless Pokémon with 100 HP or less");
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (ctx.State.Stadium == null) return Nothing(ctx);
            ctx.DamageDefending(atk.Damage);
            return true;
        }
    }

    /// <summary>Midnight Fluttering: while Active, the opponent's Active Pokémon has no Abilities (except Midnight Fluttering).</summary>
    public sealed class FlutterMane : Mon
    {
        public override bool IsAbilityBlocker { get { return true; } }

        public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
        {
            return src.PokemonIsActive && mon.Owner != src.Player && src.IsActive(mon) && abilityName != "Midnight Fluttering";
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.PlaceCountersAnyWay(2, () => ctx.OpponentBench());
            return true;
        }
    }

    public sealed class Frillish : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.Opp.ItemLockTurn = ctx.Turn + 1;
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn.");
            return true;
        }
    }

    // ================================================================== G

    public sealed class Genesect : Mon
    {
        public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
        {
            return player != src.Player && src.Pokemon.Tool != null && card.Def.IsAceSpec;
        }
    }

    public sealed class GenesectEx : Mon
    {
        protected override string Activated { get { return "Metallic Signal"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            ctx.SearchToHand(d => d.IsEvolution && d.PokemonType == EnergyType.Metal, 2, "Metallic Signal: choose up to 2 Evolution Metal Pokémon");
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfTakesLessNextTurn(30);
            return true;
        }
    }

    public sealed class Goldeen : FestivalLeadMon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Flip() && ctx.InPlay(defending) && defending.Energy.Count > 0)
            {
                var e = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Whirlpool: discard an Energy from the Defending Pokémon")[0];
                ctx.Engine.DiscardEnergy(defending, e);
            }
            return true;
        }
    }

    // ================================================================== H

    public sealed class Hariyama : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            if (TriggerOnce(ctx, "Heave-Ho Catcher", ctx.GustTargets().Count > 0, "Use Heave-Ho Catcher to switch in 1 of your opponent's Benched Pokémon?"))
                ctx.GustOpponent("Heave-Ho Catcher: choose 1 of your opponent's Benched Pokémon to switch in");
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(70);
            return true;
        }
    }

    public sealed class Heatran : Mon
    {
        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (holderWasActive && ctx.IsActive(attacker)) ctx.ApplyCondition(attacker, SpecialCondition.Burned);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var metal = ctx.Pokemon.Energy.Where(e => ctx.Engine.EnergyUnitsOf(ctx.Pokemon, e).Any(u => EnergyMask.Provides(u, EnergyType.Metal))).ToList();
            int n = 0;
            foreach (var e in metal) if (ctx.Engine.DiscardEnergy(ctx.Pokemon, e)) n++;
            ctx.DamageDefending(atk.Damage * n);
            return true;
        }
    }

    public sealed class Hoothoot : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
            return true;
        }
    }

    public sealed class HopsCramorant : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int left = ctx.Opp.Prizes.Count;
            if (left != 3 && left != 4) return Nothing(ctx);
            ctx.DamageDefending(atk.Damage);
            return true;
        }
    }

    public sealed class HopsDubwool : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            if (!ctx.IsMyTurn || ctx.GustTargets().Count == 0) return;
            if (!ctx.AskYesNo("Defiant Horn: switch in 1 of your opponent's Benched Pokémon?")) return;
            Announce(ctx, "Defiant Horn");
            ctx.GustOpponent("Defiant Horn: choose 1 of your opponent's Benched Pokémon to switch in");
        }
    }

    public sealed class HopsPhantump : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (ctx.Flip()) ctx.ProtectSelfNextTurn();
            return true;
        }
    }

    public sealed class HopsSnorlax : Mon
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            var a = info.Attacker;
            if (a == null || a.Owner != src.Player || !RecifeTrainers.IsHops(a.Def) || !info.ToOpponentsActive) return damage;
            return info.ApplyOnce("Extra Helpings") ? damage + 30 : damage;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.DamageSelf(80);
            return true;
        }
    }

    public sealed class HopsTrevenant : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Horrifying Revenge")
            {
                bool revenge = ctx.WasKnockedOutLastOpponentTurn(RecifeTrainers.IsHops, true);
                ctx.DamageDefending(atk.Damage + (revenge ? 100 : 0));
                return true;
            }
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            ctx.DefendingCantRetreat(defending);
            return true;
        }
    }

    public sealed class HopsZacianEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Insta-Strike") ctx.DamageOneBenched(30);
            else if (atk.Name == "Brave Slash") ctx.SelfCantUseAttackNextTurn("Brave Slash");
            return true;
        }
    }

    public sealed class HydrappleEx : Mon
    {
        protected override string Activated { get { return "Ripening Charge"; } }
        private static List<CardInstance> GrassInHand(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList(); }
        protected override bool AbilityReady(EffectContext ctx) { return GrassInHand(ctx).Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(GrassInHand(ctx), 1, 1, "Ripening Charge: choose a Basic Grass Energy from your hand")[0];
            var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Ripening Charge: attach " + energy.Def.Name + " to which Pokémon (then heal 30)?");
            ctx.Me.Hand.Remove(energy);
            ctx.Engine.AttachEnergyCard(energy, target, true);
            ctx.Heal(target, 30);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int grass = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Grass));
            ctx.DamageDefending(atk.Damage + 30 * grass);
            return true;
        }
    }

    // ================================================================== I, J

    public sealed class IronCrownEx : Mon
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            var a = info.Attacker;
            if (a == null || a.Owner != src.Player || !a.Def.IsFuture || a.Def.Name == "Iron Crown ex" || !info.ToOpponentsActive) return damage;
            return damage + 20;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            var all = ctx.OpponentPokemon();
            int n = Math.Min(2, all.Count);
            var targets = ctx.ChoosePokemonMany(all, n, 2, "Twin Shotels: choose 2 of your opponent's Pokémon");
            foreach (var t in targets) ctx.DamageWith(t, 50, false, false, true);
            return true;
        }
    }

    public sealed class IronLeavesEx : Mon
    {
        public override void OnPlayedFromHandToBench(EffectContext ctx)
        {
            if (!ctx.IsMyTurn || ctx.Me.Active == null) return;
            if (!ctx.AskYesNo("Rapid Vernier: switch Iron Leaves ex with your Active Pokémon?")) return;
            Announce(ctx, "Rapid Vernier");
            if (!ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon)) return;
            var others = ctx.MyPokemon(m => m != ctx.Pokemon && m.Energy.Count > 0);
            var energy = others.SelectMany(m => m.Energy).ToList();
            if (energy.Count == 0) return;
            var picks = ctx.ChooseCards(energy, 0, energy.Count, "Rapid Vernier: move any amount of Energy from your other Pokémon to Iron Leaves ex");
            foreach (var e in picks)
            {
                var from = others.FirstOrDefault(m => m.Energy.Contains(e));
                if (from != null) ctx.Engine.MoveEnergy(from, ctx.Pokemon, e);
            }
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantAttackNextTurn();
            return true;
        }
    }

    public sealed class JellicentEx : Mon
    {
        public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
        {
            return src.PokemonIsActive && player != src.Player && (card.Def.IsItem || card.Def.IsTool);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + (ExtraEnergy(ctx, atk, 2) ? 80 : 0));
            return true;
        }
    }

    public sealed class JirachiEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Wish Granter")
            {
                ctx.DrawUntil(7);
                return true;
            }
            ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
            return true;
        }
    }
}
