using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Rocket Returns Rare Pokémon, Pokémon-ex and Pokémon ★ (TRR 1-28, 96-109; v164). Unlimited only. "Dark" in a name means a
    /// name starting with "Dark " (your Rocket's Hideout ruling); "Rocket's" a name starting with "Rocket's ". 1-on-1: "each
    /// Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsTRR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TRR-1", new AzumarillFroth());
            r.RegisterByKey("TRR-2", new DarkAmpharosImpulse());
            r.RegisterByKey("TRR-3", new DarkCrobatBlackBeam());
            r.RegisterByKey("TRR-4", new DarkElectrodeNavigation());
            r.RegisterByKey("TRR-5", new ComboMon(new CardsASC.RandomDiscardMon("Shakedown"), new DarkFireMon()));
            r.RegisterByKey("TRR-6", new ComboMon(new CopyAttackMon("Dark Link", true, OwnDarkOthers, "1 of your Pokémon with Dark in its name"),
                new CardsASR1.TimesMon("Black Magic", c => c.Opp.Bench.Count)));
            r.RegisterByKey("TRR-7", new ComboMon(new BrickSmashMon(), new HardBoneMon()));
            r.RegisterByKey("TRR-8", new ComboMon(new BlackSuctionCupsMon(), new CardsRS.ExtraEnergyMon("Ink Blast", 2)));
            r.RegisterByKey("TRR-9", new DarkSlowkingCunning());
            r.RegisterByKey("TRR-10", new EnergyLinkMon("Energy Link"));
            r.RegisterByKey("TRR-11", new BufferMon(new CardsASR1.PlusMon("Energy Crush", 10, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("TRR-12", new KingdraDragonVeil());
            r.RegisterByKey("TRR-13", new ComboMon(new CardsDAA1.FlipCantAttackMon("Sheer Cold"), new TonnageMon(40)));
            r.RegisterByKey("TRR-14", new TogeticHolyShield());
            r.RegisterByKey("TRR-15", new DarkDragoniteTrance());
            r.RegisterByKey("TRR-16", new DarkMukStickyGoo());
            r.RegisterByKey("TRR-17", new ComboMon(new DarkSeedMon(), new SpreadPoisonMon()));
            r.RegisterByKey("TRR-18", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, new CardsMA.SwiftMon("Swift")));
            r.RegisterByKey("TRR-19", new ComboMon(new CardsASR1.PlusMon("Grind", 10, c => c.EnergyCount(c.Pokemon)),
                new CardsSS.EachOpponentMon("Spinning Tail", 20), CardsSS.ConditionBonus("Bite Off", 50, d => d.Def.IsOldEx)));
            r.RegisterByKey("TRR-20", new DarkTyranitarSandDamage());
            r.RegisterByKey("TRR-21", new DelibirdGiftExchange());
            r.RegisterByKey("TRR-22", new ComboMon(new QuickChangeMon(), new QuickTailSmashMon()));
            r.RegisterByKey("TRR-23", new LedianPowderProtection());
            r.RegisterByKey("TRR-24", new CardsSS.BabyEvolutionMon("Magmar", new DetourMon()));
            r.RegisterByKey("TRR-25", new MisdreavusDarkSpell());
            r.RegisterByKey("TRR-26", new SaturationMon(20, new CardsMA.ExtraBasicMon("Hyper Pump", 20, 3)));
            r.RegisterByKey("TRR-27", new QwilfishSpiny());
            r.RegisterByKey("TRR-28", new ComboMon(new CardsDR.DifferentEnergyMon("Quick Charge", 4), new CardsMA.SwiftMon("Swift")));

            r.RegisterByKey("TRR-96", new ArticunoDarknessVeil());
            r.RegisterByKey("TRR-97", new EnteiDarkCondition());
            r.RegisterByKey("TRR-98", new CardsASC.CounterattackMon("Strikes Back", 2, new CardsDR.AlsoOneBenchedMon("Mach Punch", 10)));
            r.RegisterByKey("TRR-99", new ComboMon(new DarknessSwitchMon(), new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("TRR-100", new CardsMAB.DarkLiftMon(new FireDanceMon()));
            r.RegisterByKey("TRR-101", new DualArmorMon(EnergyType.Metal, new RotatingClawsMon()));
            r.RegisterByKey("TRR-102", new DualArmorMon(EnergyType.Grass, new ComboMon(new SwitchSelfMon("Bounce", true), new LockAfterAttackMon("Slashing Strike", true))));
            r.RegisterByKey("TRR-103", new ComboMon(new DragOffMayMon(), new CardsASR1.PlusMon("Dark Ring", 10, c => c.MyPokemon(m => c.Engine.IsType(m, EnergyType.Darkness)).Count)));
            r.RegisterByKey("TRR-104", new SnorlaxDarkHealer());
            r.RegisterByKey("TRR-105", new SuicuneDarkAndClear());
            r.RegisterByKey("TRR-106", new ZapdosDarknessGuard());
            r.RegisterByKey("TRR-107", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool"), new SpringBackMon(SpecialCondition.Asleep)));
            r.RegisterByKey("TRR-108", new ComboMon(new FireworksMon(), new SpringBackMon(SpecialCondition.Confused)));
            r.RegisterByKey("TRR-109", new SpringBackMon(SpecialCondition.Poisoned));
        }

        // ================================================================== Shared

        internal static bool Dark(string name) { return name != null && name.StartsWith("Dark ", StringComparison.Ordinal); }

        internal static bool Rocket(string name) { return name != null && name.StartsWith("Rocket's ", StringComparison.Ordinal); }

        internal static bool DarkOrRocket(string name) { return Dark(name) || Rocket(name); }

        /// <summary>"Has any Darkness Energy attached" (R Energy and Dark Metal Energy count while they provide it).</summary>
        internal static bool HasDarkness(EffectContext src, PokemonInPlay mon)
        {
            return mon != null && mon.Energy.Any(e => CardsHIF1.Provides(src, mon, e, EnergyType.Darkness));
        }

        private static readonly string[] SecretMachines = { "Pokémon Retriever", "Pow! Hand Extension", "Surprise! Time Machine", "Swoop! Teleporter", "Venture Bomb" };

        /// <summary>A Rocket's Secret Machine card (Team Rocket Returns' Trainer cards of that kind; they play as Items).</summary>
        internal static bool IsSecretMachine(CardDef d) { return d != null && d.IsTrainer && SecretMachines.Contains(d.Name); }

        private static List<PokemonInPlay> OwnDarkOthers(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && Dark(m.Name)); }

        /// <summary>
        /// "Choose an attack on [a Pokémon] and copy it except for its Energy cost (you must still do anything else required)":
        /// Dark Link, Rainbow Moves, Mini-Metronome and (FireRed &amp; LeafGreen) Clefable ex's Metronome. Unlike Base Set Metronome,
        /// anything else the attack needs (discarding Energy) is still done. Optionally a coin flip first (tails: nothing).
        /// </summary>
        internal sealed class CopyAttackMon : Mon
        {
            private readonly string attack;
            private readonly bool flip;
            private readonly Func<EffectContext, List<PokemonInPlay>> from;
            private readonly string what;

            public CopyAttackMon(string attack, bool flip, Func<EffectContext, List<PokemonInPlay>> from, string what)
            {
                this.attack = attack;
                this.flip = flip;
                this.from = from;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (flip && !ctx.Flip()) return Nothing(ctx);
                var mons = from(ctx).Where(m => m.Def.Attacks.Count > 0).ToList();
                if (mons.Count == 0) return Nothing(ctx);
                var mon = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, attack + ": copy an attack of which Pokémon (" + what + ")?");
                CardDef source;
                int index;
                if (mon == null || !ChooseCopiedAttack(ctx, mon, attack + ": choose 1 of " + mon.Name + "'s attacks to copy", out source, out index)) return Nothing(ctx);
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Buffer (Poké-Body): if this Pokémon would be Knocked Out by an opponent's attack, flip a coin; heads, it stays in play with 10 HP left.</summary>
        internal sealed class BufferMon : Mon
        {
            private readonly Mon attacks;

            public BufferMon(Mon attacks) { this.attacks = attacks; }

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                if (attacker == null || attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return false;
                Announce(src, "Buffer");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Saturation (Poké-Body): attaching a Water Energy card from your hand to this Pokémon removes its Special Conditions and N damage.</summary>
        internal sealed class SaturationMon : Mon
        {
            private readonly int heal;
            private readonly Mon attacks;

            public SaturationMon(int heal, Mon attacks)
            {
                this.heal = heal;
                this.attacks = attacks;
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !CardsRS.BasicOf(energy.Def, EnergyType.Water) || !src.InPlay(me)) return;
                if (me.Conditions == SpecialCondition.None && me.DamageCounters == 0) return;
                Announce(src, "Saturation");
                src.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) src.Heal(me, heal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Darkness Shield: during the opponent's next turn, no attack damage or effects on your Active Pokémon (whichever it is).</summary>
        internal sealed class ActiveProtectEffect : ContinuousEffect
        {
            public ActiveProtectEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var t = info.Target;
                return t != null && t.Owner == Owner && engine.State.IsActive(t) && info.AgainstOpponent ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target == null || target.Owner != Owner || kind != EffectSourceKind.Attack || !engine.State.IsActive(target)) return false;
                var attacker = engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != Owner;
            }
        }

        /// <summary>Dark Seed: N damage counters on the Pokémon at the end of the opponent's next turn (if it's still there).</summary>
        internal sealed class DelayedCountersEffect : ContinuousEffect
        {
            private readonly int counters;
            private readonly bool noRetreat;
            private readonly string source;

            public DelayedCountersEffect(string name, int owner, int targetUid, int currentTurn, int counters) : this(name, owner, targetUid, currentTurn, counters, true, "Dark Seed") { }

            /// <param name="noRetreat">Dark Seed also stops retreating; Spiky Shell doesn't.</param>
            public DelayedCountersEffect(string name, int owner, int targetUid, int currentTurn, int counters, bool noRetreat, string source)
                : base(name, owner, targetUid, currentTurn + 1, true)
            {
                this.counters = counters;
                this.noRetreat = noRetreat;
                this.source = source;
            }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return noRetreat && mon != null && mon.Uid == TargetUid; }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon != null) engine.PlaceDamageCounters(mon, counters, source);
            }
        }

        /// <summary>Energy Link: an Energy card from your discard pile onto this Pokémon.</summary>
        internal sealed class EnergyLinkMon : Mon
        {
            private readonly string attack;

            public EnergyLinkMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, attack + ": attach which Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Tonnage: you may do N more; if you do, this Pokémon does 30 damage to itself.</summary>
        internal sealed class TonnageMon : Mon
        {
            private readonly int bonus;

            public TonnageMon(int bonus) { this.bonus = bonus; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tonnage") return false;
                bool more = ctx.AskYesNo("Tonnage: do " + bonus + " more damage and 30 damage to " + ctx.Pokemon.Name + "?");
                ctx.DamageDefending(atk.Damage + (more ? bonus : 0));
                if (more && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>
        /// "Search your discard pile for [cards]. Either put 1 into your hand, or shuffle a combination of 3 into your deck" (Dark Aid,
        /// Pokémon Retriever). Returns false if there was nothing to take.
        /// </summary>
        internal static bool RetrieveOneOrThree(EffectContext ctx, Func<CardDef, bool> filter, string what, string source)
        {
            var cards = ctx.Me.Discard.Where(c => filter(c.Def)).ToList();
            if (cards.Count == 0) return false;
            int mode = cards.Count < 3 ? 0 : ctx.ChooseOption(source + ": what do you want to do?", "Put 1 " + what + " into your hand", "Shuffle 3 " + what + "s into your deck");
            if (mode == 0)
            {
                var pick = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, source + ": put which card into your hand?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Me.Hand.Add(pick);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "put this card from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (" + source + ")");
                return true;
            }
            var picks = ctx.ChooseCards(cards, 3, 3, source + ": choose 3 cards to shuffle into your deck");
            foreach (var card in picks)
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Add(card);
            ctx.Engine.Reveal(ctx.Player, picks, "shuffled these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck (" + source + ")");
            ctx.ShuffleMyDeck();
            return true;
        }

        /// <summary>Spring Back: if the opponent has only 1 Prize card left, 50 more and the Defending Pokémon gets a Special Condition.</summary>
        internal sealed class SpringBackMon : Mon
        {
            private readonly SpecialCondition condition;

            public SpringBackMon(SpecialCondition condition) { this.condition = condition; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spring Back") return false;
                var defending = ctx.Defending;
                bool last = ctx.Opp.Prizes.Count == 1;
                ctx.DamageDefending(atk.Damage + (last ? 50 : 0));
                if (last && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>Dual Armor (Poké-Body): with any [type] Energy attached, this Pokémon is that type as well as Darkness.</summary>
        internal sealed class DualArmorMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            public DualArmorMon(EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || types.Contains(type)) return;
                var wanted = type;
                if (me.Energy.Any(e => CardsHIF1.Provides(src, me, e, wanted))) types.Add(type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Rares

        /// <summary>Froth (Poké-Power): when you evolve your Active Pokémon into Azumarill from your hand, the Defending Pokémon may be Paralyzed. Water Punch: a coin per Water Energy, 20 more each heads.</summary>
        private sealed class AzumarillFroth : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var theirs = ctx.Opp.Active;
                if (me == null || !TriggerOnce(ctx, "Froth", ctx.IsActive(me) && theirs != null, "Froth: make the Defending Pokémon Paralyzed?")) return;
                if (ctx.InPlay(theirs)) ctx.ApplyCondition(theirs, SpecialCondition.Paralyzed);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Punch") return false;
                var me = ctx.Pokemon;
                int water = me.Energy.Count(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Water));
                int heads = water > 0 ? ctx.FlipCoins(water) : 0;
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>Darkest Impulse (Poké-Body): each Pokémon the opponent evolves from their hand gets 2 damage counters; only 1 Darkest Impulse counts (your ruling: every evolution, no stacking). Shock Bolt: discard all Lightning Energy.</summary>
        private sealed class DarkAmpharosImpulse : Mon
        {
            public override void OnOpponentEvolvedFromHand(EffectContext src, PokemonInPlay evolved)
            {
                var first = src.MyPokemon(m => src.Engine.HasAbilityNamed(m, "Darkest Impulse")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != src.Pokemon || !src.InPlay(evolved)) return;
                Announce(src, "Darkest Impulse");
                src.PlaceCounters(evolved, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shock Bolt") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me)) return true;
                foreach (var card in me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Black Beam (Poké-Power): once during your turn, if Dark Crobat is Active, the Defending Pokémon is Poisoned; not with a Special Condition. Dark Drain. Skill Dive.</summary>
        private sealed class DarkCrobatBlackBeam : Mon
        {
            private static readonly Mon Dive = new CardsSSB.AnyOpponentTextMon("Skill Dive", 30);

            protected override string Activated { get { return "Black Beam"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx) { ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Poisoned); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Drain") return Dive.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 10);
                int n = ctx.OpponentPokemon().Count;
                if (n > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10 * n);
                return true;
            }
        }

        /// <summary>Darkness Navigation (Poké-Power): with no Energy on Dark Electrode, a Darkness or Dark Metal Energy from your deck onto it. Energy Bomb: you may move all its Energy to your Benched Pokémon.</summary>
        private sealed class DarkElectrodeNavigation : Mon
        {
            private const string Power = "Darkness Navigation";

            private static bool Fits(CardDef d) { return CardsRS.BasicOf(d, EnergyType.Darkness) || (d.IsEnergy && d.Name == "Dark Metal Energy"); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon.Energy.Count == 0 && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                foreach (var card in ctx.SearchMyDeck(Fits, 1, Power + ": choose a Darkness Energy or Dark Metal Energy card"))
                    if (!ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Bomb") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Energy Bomb: move all Energy cards from " + me.Name + " to your Benched Pokémon?")) return true;
                foreach (var card in me.Energy.ToList())
                {
                    var bench = ctx.Me.Bench.ToList();
                    var to = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Energy Bomb: move " + card.Def.Name + " to which Benched Pokémon?");
                    if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                }
                return true;
            }
        }

        /// <summary>Dark Fire: you may discard a Fire or Darkness Energy from Dark Houndoom for 20 more.</summary>
        private sealed class DarkFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Fire") return false;
                var me = ctx.Pokemon;
                var cards = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire) || CardsHIF1.Provides(ctx, me, e, EnergyType.Darkness)).ToList();
                bool more = false;
                if (!CardsBS1.CopyingWithMetronome && cards.Count > 0 && ctx.AskYesNo("Dark Fire: discard a Fire or Darkness Energy for 20 more damage?"))
                {
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Dark Fire: discard which Energy?")[0];
                    more = ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                return true;
            }
        }

        /// <summary>Brick Smash: not affected by Resistance or by effects on the Defending Pokémon (Weakness still applies).</summary>
        private sealed class BrickSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brick Smash") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }

        /// <summary>Hard Bone: discard a Basic Pokémon or Evolution card from your hand or the attack does nothing.</summary>
        private sealed class HardBoneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Bone") return false;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var cards = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                    if (cards.Count == 0) return Nothing(ctx);
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Hard Bone: discard a Basic Pokémon or Evolution card")[0];
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Black Suction Cups: 10 damage; heads, the Defending Pokémon is Paralyzed.</summary>
        private sealed class BlackSuctionCupsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Black Suction Cups") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 10);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Cunning (Poké-Power): once during your turn, look at the top card of the opponent's deck; you may shuffle it. Litter: discard up to 2 Tools / Rocket's Secret Machines from your hand, 30 more each.</summary>
        private sealed class DarkSlowkingCunning : Mon
        {
            private const string Power = "Cunning";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.ShowCards(ctx.Player, top, "Top card of your opponent's deck", Power + ": the top card of your opponent's deck", ctx.Source);
                if (ctx.AskYesNo(Power + ": shuffle your opponent's deck?"))
                {
                    ctx.Engine.ShuffleDeck(ctx.Opponent);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + "'s deck was shuffled (Cunning).");
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Litter") return false;
                int n = 0;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsTool || IsSecretMachine(c.Def)).ToList();
                if (!CardsBS1.CopyingWithMetronome && cards.Count > 0)
                    foreach (var card in ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Litter: discard up to 2 Pokémon Tool and Rocket's Secret Machine cards (30 more damage each)"))
                        if (ctx.Me.Hand.Contains(card))
                        {
                            ctx.Engine.DiscardFromHand(ctx.Player, card);
                            n++;
                        }
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        /// <summary>Dragon Veil (Poké-Body): while Kingdra is in play, your Active Pokémon has no Weakness. Hyper Whirlpool. Aqua Sonic.</summary>
        private sealed class KingdraDragonVeil : Mon
        {
            private static readonly Mon Sonic = new CardsFLF1.NoResistanceMon("Aqua Sonic");

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && src.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Whirlpool") return Sonic.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                int heads = ctx.FlipUntilTails();
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    if (ctx.DiscardEnergyFrom(defending, 1, "Hyper Whirlpool: discard an Energy card from " + defending.Name) == 0) break;
                }
                return true;
            }
        }

        /// <summary>Holy Shield (Poké-Body): no damage or effects of attacks from the opponent's Pokémon with Dark in its name. Dive. Rainbow Moves: copy an attack of the opponent's Benched Pokémon.</summary>
        private sealed class TogeticHolyShield : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new DodgeMon("Dive", false),
                new CopyAttackMon("Rainbow Moves", false, c => c.OpponentBench(), "1 of your opponent's Benched Pokémon"));

            private static bool FromDark(EffectContext src, PokemonInPlay attacker) { return attacker != null && attacker.Owner != src.Player && Dark(attacker.Name); }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return FromDark(src, info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                return FromDark(src, src.Engine.CurrentSourcePokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Trance (Poké-Power): as often as you like, move a Darkness Energy card from 1 of your Pokémon to another; not with a Special Condition. Double Wing Attack.</summary>
        private sealed class DarkDragoniteTrance : Mon
        {
            private const string Trance = "Dark Trance";
            private static readonly Mon Wing = new CardsRS.TextDamageMon("Double Wing Attack", 30);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Trance) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && CardsCEL.EnergyTransReady(ctx, EnergyType.Darkness);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { CardsCEL.UseEnergyTrans(ctx, Trance, EnergyType.Darkness); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wing.HandleAttack(ctx, atk); }
        }

        /// <summary>Sticky Goo (Poké-Body): while Dark Muk is your Active Pokémon, the opponent's Active Pokémon retreats for 2 more. Slimy Water. Acidic Poison.</summary>
        private sealed class DarkMukStickyGoo : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.TimesMon("Slimy Water", c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending)),
                new CardsMA.AcidicPoisonMon());

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || mon == null || mon.Owner == src.Player || !src.IsActive(mon)) return cost;
                return cost + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Seed: the Defending Pokémon can't retreat during the opponent's next turn, and gets 5 damage counters at its end.</summary>
        private sealed class DarkSeedMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Seed") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new DelayedCountersEffect(defending.Name + " can't retreat and gets 5 damage counters at the end of its next turn (Dark Seed)", ctx.Player, defending.Uid, ctx.Turn, 5));
                return true;
            }
        }

        /// <summary>Spread Poison: the Defending Pokémon is Poisoned; 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SpreadPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spread Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Sand Damage (Poké-Body): while Dark Tyranitar is your Active Pokémon, 1 damage counter on each of the opponent's Benched Basic Pokémon between turns; only 1 Sand Damage counts. Second Strike.</summary>
        private sealed class DarkTyranitarSandDamage : Mon
        {
            private static readonly Mon Strike = CardsSS.ConditionBonus("Second Strike", 20, d => d.DamageCounters >= 2);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var first = src.MyPokemon(m => src.IsActive(m) && src.Engine.HasAbilityNamed(m, "Sand Damage")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != me) return;
                var targets = src.Opp.Bench.Where(m => m.IsBasic).ToList();
                if (targets.Count == 0) return;
                Announce(src, "Sand Damage");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Sand Damage");
            }

            public override int CheckupAffects(EffectContext src) { return 1 - src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Strike.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Gift Exchange (Poké-Power): once during your turn, if Delibird is Active, shuffle a card from your hand into your deck and
        /// draw a card; not with a Special Condition. Souvenir: 3 coins (1 heads: 4 counters; 2: 1 counter off; 3: 10 counters; 0:
        /// all counters off the Defending Pokémon).
        /// </summary>
        private sealed class DelibirdGiftExchange : Mon
        {
            private const string Power = "Gift Exchange";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                var card = hand.Count == 1 ? hand[0] : ctx.ChooseCards(hand, 1, 1, Power + ": shuffle which card into your deck?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Souvenir") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads == 1) ctx.PlaceCounters(defending, 4);
                else if (heads == 3) ctx.PlaceCounters(defending, 10);
                else if (defending.DamageCounters > 0) ctx.Heal(defending, heads == 2 ? 10 : 10 * defending.DamageCounters);
                return true;
            }
        }

        /// <summary>Quick Change: put up to 3 cards from your hand on top of your deck, then search your deck for that many cards.</summary>
        private sealed class QuickChangeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Change") return false;
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, Math.Min(3, hand.Count), "Quick Change: put up to 3 cards from your hand on top of your deck");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card))
                    {
                        ctx.Me.Deck.Insert(0, card);
                        n++;
                    }
                if (n > 0) ctx.SearchToHand(d => true, n, "Quick Change: choose " + n + " card" + (n == 1 ? "" : "s"), false, n);
                return true;
            }
        }

        /// <summary>Quick Tail Smash: before the damage you may flip; heads 80 instead, tails nothing.</summary>
        private sealed class QuickTailSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Tail Smash") return false;
                if (ctx.AskYesNo("Quick Tail Smash: flip a coin (heads 80 damage, tails nothing)?"))
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(80);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Powder Protection (Poké-Body): attack damage to Ledian from Pokémon with an owner in their name is 40 less. Split Spiral Punch. Tackle.</summary>
        private sealed class LedianPowderProtection : Mon
        {
            private static readonly Mon Punch = new CardsPAR2.FlipConditionMon("Split Spiral Punch", SpecialCondition.Confused);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsHL.HasOwner(info.Attacker.Name) ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>Detour: with a Supporter card in play (played this turn), use its effect as this attack's effect.</summary>
        private sealed class DetourMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Detour") return false;
                var def = ctx.Me.SupporterThisTurn;
                if (def == null) return Nothing(ctx);
                var card = ctx.Me.Discard.LastOrDefault(c => c.Def == def);
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + def.Name + " (Detour).");
                ctx.Engine.Scripts.For(def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                return true;
            }
        }

        /// <summary>Dark Spell (Poké-Power): once during your turn, if Misdreavus is Active, flip; heads, 1 damage counter on 1 of the opponent's Pokémon. Hide in Shadows.</summary>
        private sealed class MisdreavusDarkSpell : Mon
        {
            private static readonly Mon Hide = new SwitchSelfMon("Hide in Shadows", false);

            protected override string Activated { get { return "Dark Spell"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.OpponentPokemon().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var all = ctx.OpponentPokemon();
                var target = all.Count == 1 ? all[0] : ctx.ChoosePokemon(all, "Dark Spell: put 1 damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hide.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiny (Poké-Body): if Qwilfish is Active and damaged by an opponent's attack, flip until tails; 1 damage counter on the Attacking Pokémon per heads. Stun Poison.</summary>
        private sealed class QwilfishSpiny : Mon
        {
            private static readonly Mon Stun = new CardsCES1.FlipConditionsMon("Stun Poison", SpecialCondition.Paralyzed, SpecialCondition.Poisoned);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Spiny");
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && ctx.InPlay(attacker)) ctx.Engine.PlaceDamageCounters(attacker, heads, "Spiny");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Stun.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon-ex

        /// <summary>Darkness Veil (Poké-Body): with any Darkness Energy attached, no effects (other than damage) of the opponent's attacks on it. Freeze Solid. Ice Wing.</summary>
        private sealed class ArticunoDarknessVeil : Mon
        {
            private static readonly Mon Freeze = new CardsOBF1.AttachFromDiscardToSelfMon("Freeze Solid", EnergyType.Water);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack || !HasDarkness(src, src.Pokemon)) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Freeze.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Condition (Poké-Body): with any Darkness Energy attached, no Weakness. Energy Link. Volcanic Ash: discard 2 Energy, 60 to 1 of the opponent's Pokémon.</summary>
        private sealed class EnteiDarkCondition : Mon
        {
            private static readonly Mon Link = new EnergyLinkMon("Energy Link");

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon && HasDarkness(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volcanic Ash") return Link.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome) ctx.DiscardEnergyFrom(me, 2, "Volcanic Ash: discard 2 Energy");
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 60));
                return true;
            }
        }

        /// <summary>Darkness Switch: discard an Energy card from Rocket's Mewtwo ex, then swap its damage counters with the Defending Pokémon's (prevented: nothing).</summary>
        private sealed class DarknessSwitchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Switch") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return Nothing(ctx);
                if (!CardsBS1.CopyingWithMetronome && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Darkness Switch: discard an Energy card");
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                int mine = me.DamageCounters;
                me.DamageCounters = defending.DamageCounters;
                defending.DamageCounters = mine;
                ctx.Log("Darkness Switch: " + me.Name + " and " + defending.Name + " switched their damage counters.");
                return true;
            }
        }

        /// <summary>Fire Dance: a Fire Energy card from your discard pile onto 1 of your Pokémon.</summary>
        internal sealed class FireDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Dance") return false;
                ctx.DamageDefending(atk.Damage);
                var cards = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                var mons = ctx.MyPokemon();
                if (cards.Count == 0 || mons.Count == 0) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Fire Dance: attach which Fire Energy card?")[0];
                var to = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "Fire Dance: attach it to which Pokémon?");
                if (to != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, to)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Rotating Claws: you may discard an Energy card from Rocket's Scizor ex; if you do, attach a different Energy card from your discard pile to it.</summary>
        private sealed class RotatingClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rotating Claws") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !ctx.Me.Discard.Any(c => c.Def.IsEnergy)) return true;
                if (!ctx.AskYesNo("Rotating Claws: discard an Energy card from " + me.Name + " and attach another from your discard pile?")) return true;
                var gone = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Rotating Claws: discard which Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(me, gone)) return true;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c != gone).ToList();
                if (cards.Count == 0) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Rotating Claws: attach which Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Drag Off: before the damage you may switch in 1 of the opponent's Benched Pokémon; the damage goes to the (new) Defending Pokémon.</summary>
        internal sealed class DragOffMayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drag Off") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Drag Off: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Drag Off: choose the new Defending Pokémon");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Dark Healer (Poké-Body): with any Darkness Energy attached, 1 damage counter comes off it between turns. Poison Claws. Collapse.</summary>
        private sealed class SnorlaxDarkHealer : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned),
                new CardsRCL2.SelfConditionMon("Collapse", SpecialCondition.Asleep));

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.DamageCounters == 0 || !HasDarkness(src, me)) return;
                Announce(src, "Dark Healer");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark and Clear (Poké-Body): with any Darkness Energy attached, no Special Conditions. Icy Wind. Hyper Splash: 40 more against a Stage 2 Pokémon.</summary>
        private sealed class SuicuneDarkAndClear : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep),
                CardsSS.ConditionBonus("Hyper Splash", 40, d => d.Def.Stage == Stage.Stage2));

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon && HasDarkness(src, target);
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Darkness Guard (Poké-Body): with any Darkness Energy attached, the opponent's attacks do 10 less to it (after W/R). Plasma. Raging Thunder: 30 to 1 of your Pokémon.</summary>
        private sealed class ZapdosDarknessGuard : Mon
        {
            private static readonly Mon Plasma = new CardsOBF1.AttachFromDiscardToSelfMon("Plasma", EnergyType.Lightning);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return HasDarkness(src, me) ? damage - 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Thunder") return Plasma.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Raging Thunder: do 30 damage to which of your Pokémon?");
                if (target == me) ctx.DamageSelf(30);
                else if (target != null) ctx.DamageWith(target, 30, false, false, false);
                return true;
            }
        }

        /// <summary>Fireworks: tails, discard a Fire Energy card from this Pokémon.</summary>
        private sealed class FireworksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireworks") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(me) && !CardsBS1.CopyingWithMetronome)
                    ctx.DiscardEnergyFrom(me, 1, "Fireworks: discard a Fire Energy card", e => CardsRS.BasicOf(e.Def, EnergyType.Fire));
                return true;
            }
        }
    }
}
