using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Deoxys Rare Pokémon, Pokémon-ex and Pokémon ★ (DX 1-30, 96-108; v165). Unlimited only. 1-on-1: "each Defending Pokémon" is the
    /// Defending Pokémon, and "your opponent chooses the Defending Pokémon to switch" has only one choice.
    /// </summary>
    public static class CardsDX
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DX-1", new CardsSS.SafeguardExMon(new CardsRS.TextDamageMon("Double Wing Attack", 20)));
            r.RegisterByKey("DX-2", new BeautiflyHunch());
            r.RegisterByKey("DX-3", new ComboMon(new LeafPoisonMon(), new HustlePunchMon()));
            r.RegisterByKey("DX-4", new ComboMon(new BackBurnerMon(), new SplitBombMon()));
            r.RegisterByKey("DX-5", new ClaydolPsychicTrace());
            r.RegisterByKey("DX-6", new CrawdauntDarkProtection());
            r.RegisterByKey("DX-7", new ComboMon(new PsychicRemovalMon(), new PowerfulHandMon()));
            r.RegisterByKey("DX-8", new ComboMon(new CardsSS.EachOpponentMon("Dragon Spark", 10),
                new CardsASR1.TimesMon("Full Retaliation", c => c.MyPokemon(m => m.Name == "Magikarp").Sum(m => m.DamageCounters)),
                CardsSS.ConditionBonus("Pulverize", 50, d => d.DamageCounters >= 2)));
            r.RegisterByKey("DX-9", new JirachiWishingStar());
            r.RegisterByKey("DX-10", new LudicoloSwingDance());
            r.RegisterByKey("DX-11", new MetagrossSuperConnectivity());
            r.RegisterByKey("DX-12", new ComboMon(new SeekOutOneMon(), new PullAwayMon()));
            r.RegisterByKey("DX-13", new NinjaskFastProtection());
            r.RegisterByKey("DX-14", new ShedinjaEmptyShell());
            r.RegisterByKey("DX-15", new SlakingLazyAura());
            r.RegisterByKey("DX-16", new FormChangeMon("Deoxys", new LinkBlastMon(20)));
            r.RegisterByKey("DX-17", new FormChangeMon("Deoxys", new CardsASR1.PlusMon("Energy Crush", 10, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("DX-18", new FormChangeMon("Deoxys", new ShieldAttackMon("Barrier Attack", 30)));
            r.RegisterByKey("DX-19", new LudicoloHappyDance());
            r.RegisterByKey("DX-20", new MagcargoSmoothOver());
            r.RegisterByKey("DX-21", new ComboMon(new DanceMon("Bay Dance"), new CardsFLF1.NoResistanceMon("Aqua Sonic")));
            r.RegisterByKey("DX-22", new RayquazaDragonAura());
            r.RegisterByKey("DX-23", new SableyeNightVision());
            r.RegisterByKey("DX-24", new ComboMon(new TripleBreachMon(), new CardsFST2.ConditionBonusMon("Rend", 20, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("DX-25", new ShiftryFanAction());
            r.RegisterByKey("DX-26", new TypeLiftMon("Metallic Lift", EnergyType.Metal, new ComboMon(new DrawAttackMon("Spearhead", 1), new HeavyMetalMon())));
            r.RegisterByKey("DX-27", new TropiusTropicalMotion());
            r.RegisterByKey("DX-28", new ComboMon(new RagingTrembleMon(), new MinusCountersMon("Dwindling Wave")));
            r.RegisterByKey("DX-29", new MirrorCoatMon(new ComboMon(new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused), new PsyloopMon())));
            r.RegisterByKey("DX-30", new ComboMon(new CardsSS.AlsoEachBenchMon("Spinning Horn", 10), new CardsFLF1.NoResistanceMon("Rock Hurl")));

            r.RegisterByKey("DX-96", new CrobatExDistortion());
            r.RegisterByKey("DX-97", new FormChangeMon("Deoxys ex", new EnergyBurstMon()));
            r.RegisterByKey("DX-98", new FormChangeMon("Deoxys ex", new PsyburstMon()));
            r.RegisterByKey("DX-99", new FormChangeMon("Deoxys ex", new ShieldFromMon("Psychic Shield", m => m.Def.IsOldEx, "Pokémon-ex")));
            r.RegisterByKey("DX-100", new HariyamaExCommandingAura());
            r.RegisterByKey("DX-101", new ComboMon(new DisconnectMon(), new MegaShotMon()));
            r.RegisterByKey("DX-102", new RayquazaExDragonBoost());
            r.RegisterByKey("DX-103", new SalamenceExDragonLift());
            r.RegisterByKey("DX-104", new ComboMon(new CardsTRR.DragOffMayMon(), new DarknessBlastMon()));
            r.RegisterByKey("DX-105", new ComboMon(new HealingLightMon(), new StarStrikeMon("Shooting Star", d => d.Def.IsOldEx)));
            r.RegisterByKey("DX-106", new ComboMon(new MiraculousLightMon(), new StarStrikeMon("Shining Star", d => d.Def.Stage == Stage.Stage2)));
            r.RegisterByKey("DX-107", new ComboMon(new SpiralRushMon(), new HolyStarMon()));
            r.RegisterByKey("DX-108", new RaikouExLightningBurst());
        }

        // ================================================================== Shared

        /// <summary>"[This Pokémon] can't be [condition]" (Self-control: Paralyzed; Carefree: Confused; Magma Armor: Asleep or Paralyzed, as flags).</summary>
        internal sealed class ConditionImmuneMon : Mon
        {
            private readonly SpecialCondition condition;
            private readonly Mon attacks;

            public ConditionImmuneMon(SpecialCondition condition, Mon attacks)
            {
                this.condition = condition;
                this.attacks = attacks;
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition c) { return target != null && target == src.Pokemon && (c & condition) != 0; }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"If this Pokémon has any [type] Energy attached, its Retreat Cost is 0" (Aqua Lift, Metallic Lift).</summary>
        internal sealed class TypeLiftMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            /// <param name="body">The Poké-Body's name (for reading the registration; the effect doesn't need it).</param>
            public TypeLiftMon(string body, EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon != me) return cost;
                var wanted = type;
                return me.Energy.Any(e => CardsHIF1.Provides(src, me, e, wanted)) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Mirror Coat (Poké-Body): if the opponent's attack makes this Pokémon Burned or Poisoned, the Attacking Pokémon gets the same.</summary>
        internal sealed class MirrorCoatMon : Mon
        {
            private readonly Mon attacks;

            public MirrorCoatMon(Mon attacks) { this.attacks = attacks; }

            public override void OnHolderConditionApplied(EffectContext src, SpecialCondition condition, EffectSourceKind kind, int sourceOwner, PokemonInPlay sourcePokemon)
            {
                var copy = condition & (SpecialCondition.Poisoned | SpecialCondition.Burned);
                if (copy == SpecialCondition.None || kind != EffectSourceKind.Attack || sourceOwner == src.Player) return;
                var attacker = sourcePokemon;
                if (attacker == null || !src.InPlay(attacker)) return;
                Announce(src, "Mirror Coat");
                if ((copy & SpecialCondition.Poisoned) != 0) src.ApplyCondition(attacker, SpecialCondition.Poisoned);
                if ((copy & SpecialCondition.Burned) != 0) src.ApplyCondition(attacker, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"During your opponent's next turn, prevent all effects of attacks, including damage, done to this Pokémon by [Pokémon]" (Psychic Shield, Advanced Armor).</summary>
        internal sealed class AttackerShieldEffect : ContinuousEffect
        {
            private readonly Func<PokemonInPlay, bool> from;

            public AttackerShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn, Func<PokemonInPlay, bool> from)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.from = from;
            }

            private bool Covers(PokemonInPlay attacker) { return attacker != null && attacker.Owner != Owner && from(attacker); }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && Covers(info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target != null && target.Uid == TargetUid && kind == EffectSourceKind.Attack && Covers(engine.CurrentSourcePokemon);
            }
        }

        internal sealed class ShieldFromMon : Mon
        {
            private readonly string attack;
            private readonly Func<PokemonInPlay, bool> from;
            private readonly string what;

            public ShieldFromMon(string attack, Func<PokemonInPlay, bool> from, string what)
            {
                this.attack = attack;
                this.from = from;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.AddEffect(new AttackerShieldEffect(me.Name + " is protected from the opponent's " + what + " during their next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1, from));
                return true;
            }
        }

        /// <summary>
        /// Form Change (Poké-Power): once during your turn, another card with this name from your deck takes this one's place with
        /// everything on it; this one goes on top of your deck, then the deck is shuffled. Only 1 Form Change each turn.
        /// </summary>
        internal sealed class FormChangeMon : Mon
        {
            private readonly string Power;
            private readonly string self;
            private readonly Mon attacks;

            public FormChangeMon(string self, Mon attacks) : this(self, attacks, "Form Change") { }

            /// <param name="power">The Poké-Power's name ("Form Change"; Unseen Forces' Unown: "Shuffle").</param>
            public FormChangeMon(string self, Mon attacks, string power)
            {
                this.self = self;
                this.attacks = attacks;
                Power = power;
            }

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                string name = self;
                return ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.Name == name);
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                string name = self;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == name, 1, Power + ": choose another " + name, true, 1);
                if (found.Count == 0 || me == null)
                {
                    ctx.Me.Deck.AddRange(found);
                    ctx.ShuffleMyDeck();
                    return;
                }
                var old = me.Top;
                me.Stack[me.Stack.Count - 1] = found[0];
                ctx.Me.Deck.Insert(0, old);
                ctx.ShuffleMyDeck();
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched " + name + " with another " + name + " from their deck (" + Power + ").", found[0].Id, me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Link Blast: the base damage is lower if this Pokémon and the Defending Pokémon have a different amount of Energy attached.</summary>
        internal sealed class LinkBlastMon : Mon
        {
            private readonly int different;

            public LinkBlastMon(int different) { this.different = different; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Link Blast") return false;
                var defending = ctx.Defending;
                bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                ctx.DamageDefending(same ? atk.Damage : different);
                return true;
            }
        }

        /// <summary>"[Attack]: the opponent chooses the Benched Pokémon": switch the Defending Pokémon with 1 of the opponent's Benched Pokémon they choose.</summary>
        internal static void OpponentChoosesSwitch(EffectContext ctx, string source)
        {
            var bench = ctx.Opp.Bench.Where(ctx.CanAffect).ToList();
            if (bench.Count == 0 || ctx.Opp.Active == null || !ctx.CanAffect(ctx.Opp.Active)) return;
            var pick = ctx.Engine.ChoosePokemon(ctx.Opponent, bench, source + ": choose the Benched Pokémon to switch with your Active Pokémon", ctx.Source);
            if (pick != null) ctx.Engine.SwitchActive(ctx.Opponent, pick);
        }

        // ================================================================== Rares

        /// <summary>Hunch (Poké-Body): with 40 HP or less left, Beautifly does 40 more to the Defending Pokémon (before W/R). Luring Antenna. Cutting Wind.</summary>
        private sealed class BeautiflyHunch : Mon
        {
            private static readonly Mon Antenna = new LuringAntennaMon();

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || src.Engine.RemainingHp(me) > 40) return damage;
                return damage + 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Antenna.HandleAttack(ctx, atk); }
        }

        /// <summary>Luring Antenna: before the damage you may switch in 1 of the opponent's Benched Pokémon; the damage goes to the new Defending Pokémon.</summary>
        private sealed class LuringAntennaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Luring Antenna") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Luring Antenna: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Luring Antenna: choose the new Defending Pokémon");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Leaf Poison: with any Grass Energy attached to Breloom, the Defending Pokémon is Poisoned.</summary>
        private sealed class LeafPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Poison") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool grass = me.Energy.Any(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Grass));
                ctx.DamageDefending(atk.Damage);
                if (grass && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Hustle Punch: 50 instead of 70 if Breloom used Hustle Punch during your last turn.</summary>
        private sealed class HustlePunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hustle Punch") return false;
                var me = ctx.Pokemon;
                bool tired = me.LastAttackName == "Hustle Punch" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(tired ? 50 : atk.Damage);
                return true;
            }
        }

        /// <summary>Back Burner: up to 2 basic Energy cards from your discard pile onto your Pokémon in any way you like.</summary>
        private sealed class BackBurnerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Back Burner") return false;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count > 0) ctx.AttachCardsAnyWay(basics, Math.Min(2, basics.Count), (c, m) => true, "Back Burner: attach basic Energy cards from your discard pile");
                return true;
            }
        }

        /// <summary>Split Bomb: 30 to each of 2 of the opponent's Pokémon (1 if there's only 1).</summary>
        internal sealed class SplitBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Split Bomb") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var picks = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, "Split Bomb: choose 2 of your opponent's Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Psychic Trace (Poké-Power): once during your turn, if Claydol is Active, shuffle your hand into your deck and draw as many cards as the opponent has in hand. Ancient Mantra.</summary>
        private sealed class ClaydolPsychicTrace : Mon
        {
            protected override string Activated { get { return "Psychic Trace"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int n = ctx.Opp.Hand.Count;
                if (n > 0) ctx.Draw(n);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ancient Mantra") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool psychic = me.Energy.Any(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Psychic));
                bool fighting = me.Energy.Any(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fighting));
                ctx.DamageDefending(atk.Damage + (fighting ? 20 : 0));
                if (psychic && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Dark Protection (Poké-Body): the opponent's attacks do 10 less per Darkness Energy on Crawdaunt (up to 20, after W/R). Bubblebeam. Cross-Cut.</summary>
        private sealed class CrawdauntDarkProtection : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ParalyzeFlipMon("Bubblebeam"), CardsSS.ConditionBonus("Cross-Cut", 20, CardsE2.IsEvolved));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                int dark = me.Energy.Count(e => CardsHIF1.Provides(src, me, e, EnergyType.Darkness));
                return damage - Math.Min(20, 10 * dark);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Removal: 2 coins; both heads, discard all Energy from the Defending Pokémon.</summary>
        private sealed class PsychicRemovalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Removal") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    foreach (var card in defending.Energy.ToList()) ctx.Engine.DiscardEnergy(defending, card);
                return true;
            }
        }

        /// <summary>Powerful Hand: a damage counter on the Defending Pokémon for each card in your hand (up to 7).</summary>
        private sealed class PowerfulHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Powerful Hand") return false;
                int n = Math.Min(7, ctx.Me.Hand.Count);
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Wishing Star (Poké-Power): once during your turn, if Jirachi is Active, a card from your top 5 into your hand, shuffle; Jirachi is now Asleep. Metallic Blow.</summary>
        private sealed class JirachiWishingStar : Mon
        {
            private static readonly Mon Blow = new CardsFST2.ConditionBonusMon("Metallic Blow", 30,
                c => c.Defending != null && c.Defending.Def.Abilities.Any(a => a.Kind == "Poké-Body"));

            protected override string Activated { get { return "Wishing Star"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var top = ctx.LookAtTop(5);
                ctx.Engine.NoteLookedAt(top, "The top cards of your deck", ctx.Player);
                var pick = ctx.ChooseCards(top, 1, 1, "Wishing Star: put which card into your hand?");
                foreach (var card in pick)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Hand.Add(card);
                ctx.ShuffleMyDeck();
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>Swing Dance (Poké-Power): once during your turn, draw a card; not with a Special Condition. Healing Steps. Circular Steps.</summary>
        private sealed class LudicoloSwingDance : Mon
        {
            protected override string Activated { get { return "Swing Dance"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Healing Steps")
                {
                    ctx.DamageDefending(atk.Damage);
                    var hand = ctx.Me.Hand.ToList();
                    if (hand.Count == 0 || !ctx.InPlay(me)) return true;
                    int n = 0;
                    foreach (var card in ctx.ChooseCards(hand, 0, hand.Count, "Healing Steps: discard any number of cards (1 damage counter off Ludicolo each)"))
                        if (ctx.Me.Hand.Contains(card))
                        {
                            ctx.Engine.DiscardFromHand(ctx.Player, card);
                            n++;
                        }
                    if (n > 0 && me.DamageCounters > 0) ctx.Heal(me, 10 * n);
                    return true;
                }
                if (atk.Name != "Circular Steps") return false;
                int count = ctx.MyPokemon(m => m != me).Count + ctx.OpponentPokemon().Count;
                ctx.DamageDefending(atk.Damage * count);
                return true;
            }
        }

        /// <summary>Super Connectivity (Poké-Power): once during your turn, a Psychic or Metal Energy card from your discard pile onto your Active Pokémon, then 1 damage counter on it. Link Blast.</summary>
        private sealed class MetagrossSuperConnectivity : Mon
        {
            private static readonly Mon Blast = new LinkBlastMon(40);

            private static List<CardInstance> Cards(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Psychic || d.EnergyProvides == EnergyType.Metal);
            }

            protected override string Activated { get { return "Super Connectivity"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Active != null && Cards(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var cards = Cards(ctx);
                var card = ctx.ChooseCards(cards, 1, 1, "Super Connectivity: attach which Energy card to " + active.Name + "?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                if (!ctx.Engine.AttachEnergyCard(card, active))
                {
                    ctx.Me.Discard.Add(card);
                    return;
                }
                if (ctx.InPlay(active)) ctx.PlaceCounters(active, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Seek Out: any 1 card from your deck into your hand.</summary>
        private sealed class SeekOutOneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seek Out") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Seek Out: choose a card", false);
                return true;
            }
        }

        /// <summary>Pull Away: if the opponent has 5 or more cards in hand, they discard down to 4.</summary>
        internal sealed class PullAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull Away") return false;
                ctx.DamageDefending(atk.Damage);
                int extra = ctx.Opp.Hand.Count - 4;
                if (ctx.Opp.Hand.Count < 5 || extra <= 0) return true;
                foreach (var card in ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), extra, extra, "Pull Away: discard " + extra + " card" + (extra == 1 ? "" : "s") + " (down to 4)", ctx.Source))
                    if (ctx.Opp.Hand.Contains(card)) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Fast Protection (Poké-Body): no damage or effects from the opponent's Basic Pokémon's attacks. Swords Dance: next turn Slash's base damage is 80.</summary>
        private sealed class NinjaskFastProtection : Mon
        {
            private static bool FromBasic(EffectContext src, PokemonInPlay attacker) { return attacker != null && attacker.Owner != src.Player && attacker.IsBasic; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return FromBasic(src, info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                return FromBasic(src, src.Engine.CurrentSourcePokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Swords Dance")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Slash") return false;
                bool danced = me.LastAttackName == "Swords Dance" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(danced ? 80 : atk.Damage);
                return true;
            }
        }

        /// <summary>Empty Shell (Poké-Body): when Shedinja is Knocked Out, the opponent takes no Prize cards. Extra Curse: 2 damage counters (4 on a Pokémon-ex).</summary>
        private sealed class ShedinjaEmptyShell : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return src.Pokemon != null && knockedOut == src.Pokemon ? 0 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra Curse") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.PlaceCounters(defending, defending.Def.IsOldEx ? 4 : 2);
                return true;
            }
        }

        /// <summary>Lazy Aura (Poké-Body): while Slaking is your Active Pokémon, the opponent's Pokémon-ex do 30 less damage with attacks (before W/R). Amnesia. Lazy Headbutt: Slaking is now Asleep.</summary>
        private sealed class SlakingLazyAura : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CardsRCL2.SelfConditionMon("Lazy Headbutt", SpecialCondition.Asleep));

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || !src.IsActive(me) || a == null || a.Owner == src.Player || !a.Def.IsOldEx) return damage;
                if (damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Math.Max(0, damage - 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Happy Dance (Poké-Power): once during your turn, 1 damage counter off each of your Pokémon; only 1 Happy Dance each turn. Water Punch.</summary>
        private sealed class LudicoloHappyDance : Mon
        {
            protected override string Activated { get { return "Happy Dance"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
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

        /// <summary>Smooth Over (Poké-Power): once during your turn, search your deck for a card, shuffle, then put it on top; not with a Special Condition. Knock Over. Combustion.</summary>
        private sealed class MagcargoSmoothOver : Mon
        {
            private static readonly Mon Knock = new CardsMAB.KnockOverMon();

            protected override string Activated { get { return "Smooth Over"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 1, "Smooth Over: choose a card to put on top of your deck (after shuffling)", false, 1);
                ctx.ShuffleMyDeck();
                if (found.Count == 0) return;
                ctx.Me.Deck.Insert(0, found[0]);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + ctx.Engine.Their(ctx.Player) + " deck and put a card on top of it (Smooth Over).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knock.HandleAttack(ctx, atk); }
        }

        /// <summary>Bay Dance, Dragon Dance: during your next turn, attacks by your current Active Pokémon do 30 more to the Defending Pokémon (before W/R).</summary>
        internal sealed class DanceMon : Mon
        {
            private readonly string attack;

            public DanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var active = ctx.Me.Active;
                if (active != null)
                    ctx.AddEffect(new BayDanceEffect(active.Name + " does 30 more damage during your next turn (" + attack + ")", ctx.Player, active.Uid, ctx.Turn));
                return true;
            }
        }

        private sealed class BayDanceEffect : ContinuousEffect
        {
            public BayDanceEffect(string name, int owner, int targetUid, int currentTurn) : base(name, owner, targetUid, currentTurn + 2, false)
            {
                ActiveFromTurn = currentTurn + 2;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                if (damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack || engine.State.TurnNumber < ActiveFromTurn) return damage;
                return damage + 30;
            }
        }

        /// <summary>Dragon Aura (Poké-Body): with basic Fire and basic Lightning Energy cards attached, no effects (other than damage) of the opponent's attacks. Tumbling Attack.</summary>
        private sealed class RayquazaDragonAura : Mon
        {
            private static readonly Mon Tumble = new FlipBonusMon("Tumbling Attack", 20);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                if (me == null || target != me || kind != EffectSourceKind.Attack) return false;
                if (!me.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Fire)) || !me.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Lightning))) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tumble.HandleAttack(ctx, atk); }
        }

        /// <summary>Night Vision (Poké-Power): once during your turn, if Sableye is Active, look at the opponent's hand. Limitation: no Supporter cards for the opponent next turn.</summary>
        private sealed class SableyeNightVision : Mon
        {
            protected override string Activated { get { return "Night Vision"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", "Night Vision: your opponent's hand", ctx.Source);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Limitation") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsPAR2.TrainerKindLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (Limitation)", ctx.Player, ctx.Opponent, ctx.Turn + 1, true));
                return true;
            }
        }

        /// <summary>Triple Breach: 10 to each of 3 of the opponent's Pokémon (all of them if fewer).</summary>
        private sealed class TripleBreachMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Breach") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var picks = all.Count <= 3 ? all : ctx.ChoosePokemonMany(all, 3, 3, "Triple Breach: choose 3 of your opponent's Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Fan Action (Poké-Power): once during your turn, switch the Defending Pokémon with a Benched Pokémon the opponent chooses; not with a Special Condition. Stadium Power.</summary>
        private sealed class ShiftryFanAction : Mon
        {
            private static readonly Mon Power = new CardsFST2.ConditionBonusMon("Stadium Power", 20, c => c.State.Stadium != null);

            protected override string Activated { get { return "Fan Action"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx) { OpponentChoosesSwitch(ctx, "Fan Action"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Power.HandleAttack(ctx, atk); }
        }

        /// <summary>Heavy Metal: a coin per Metal Energy attached, 20 more each heads.</summary>
        private sealed class HeavyMetalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Metal") return false;
                var me = ctx.Pokemon;
                int metal = me.Energy.Count(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Metal));
                int heads = metal > 0 ? ctx.FlipCoins(metal) : 0;
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>Tropical Motion (Poké-Body): while Tropius is your Active Pokémon, the opponent's Active Pokémon has no Resistance. Miracle Blow. Stomp.</summary>
        private sealed class TropiusTropicalMotion : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSVI1.MiraclePowderMon("Miracle Blow"), new FlipBonusMon("Stomp", 20));

            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && mon != null && mon.Owner != src.Player && src.IsActive(mon) ? 0 : amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Raging Tremble: 10 per damage counter on Whiscash to the Defending Pokémon (either coin result in 1-on-1).</summary>
        private sealed class RagingTrembleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Tremble") return false;
                ctx.Flip();
                ctx.DamageDefending(10 * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>"Does N damage minus 10 for each damage counter on this Pokémon" (Dwindling Wave).</summary>
        internal sealed class MinusCountersMon : Mon
        {
            private readonly string attack;

            public MinusCountersMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        /// <summary>Psyloop: 30 more for each Trainer card the opponent has in play (Tools and other Trainer cards attached to their Pokémon, and their Stadium).</summary>
        internal sealed class PsyloopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyloop") return false;
                int n = ctx.OpponentPokemon().Sum(m => m.Tools.Count + m.Attached.Count(c => c.Def.IsTrainer));
                if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent) n++;
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        // ================================================================== Pokémon-ex and Pokémon ★

        /// <summary>Distortion (Poké-Power): once during your turn, if Crobat ex is Active, 1 damage counter on 1 of the opponent's Pokémon. Cross Attack. Pester.</summary>
        private sealed class CrobatExDistortion : Mon
        {
            private static readonly Mon Pester = CardsSS.Pester(40);

            protected override string Activated { get { return "Distortion"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.OpponentPokemon().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var all = ctx.OpponentPokemon();
                var target = ctx.ChoosePokemon(all, "Distortion: put 1 damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross Attack") return Pester.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Energy Burst: 10 more for each Energy on Deoxys ex and the Defending Pokémon.</summary>
        private sealed class EnergyBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Burst") return false;
                int n = ctx.EnergyCount(ctx.Pokemon) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        /// <summary>Psyburst: you may discard 2 Energy from Deoxys ex; if you do, 20 more for each Energy on the Defending Pokémon.</summary>
        private sealed class PsyburstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyburst") return false;
                var me = ctx.Pokemon;
                bool more = false;
                if (!CardsBS1.CopyingWithMetronome && me.Energy.Count >= 2 && ctx.AskYesNo("Psyburst: discard 2 Energy for 20 more damage per Energy on the Defending Pokémon?"))
                    more = ctx.DiscardEnergyFrom(me, 2, "Psyburst: discard 2 Energy") == 2;
                int bonus = more && ctx.Defending != null ? 20 * ctx.EnergyCount(ctx.Defending) : 0;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Commanding Aura (Poké-Body): while Hariyama ex is Active, the opponent can't play Stadium cards. Knock Off. Pivot Throw: Hariyama ex takes 10 more next turn (before W/R).</summary>
        private sealed class HariyamaExCommandingAura : Mon
        {
            private static readonly Mon Knock = new CardsASC.RandomDiscardMon("Knock Off");

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && player != src.Player && card != null && card.Def.IsStadium;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pivot Throw") return Knock.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.AddEffect(new TakesMoreEffect(me.Name + " takes 10 more damage during the opponent's next turn (Pivot Throw)", ctx.Player, me.Uid, ctx.Turn + 1, 10));
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, any damage done to this Pokémon by attacks is increased by N (before W/R)."</summary>
        internal sealed class TakesMoreEffect : ContinuousEffect
        {
            private readonly int amount;

            public TakesMoreEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (target == null || target.Uid != TargetUid || attacker == null || damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + amount;
            }
        }

        /// <summary>Disconnect: the opponent can't play Trainer cards other than Supporters during their next turn.</summary>
        private sealed class DisconnectMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disconnect") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new NonSupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards other than Supporters during their next turn (Disconnect)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NonSupporterLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public NonSupporterLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card != null && card.Def.IsTrainer && !card.Def.IsSupporter;
            }
        }

        /// <summary>Mega Shot: discard all Lightning Energy from Manectric ex, then 80 to 1 of the opponent's Pokémon.</summary>
        private sealed class MegaShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Shot") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                    foreach (var card in me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 80));
                return true;
            }
        }

        /// <summary>Dragon Boost (Poké-Power): when benched from your hand, move any number of basic Energy cards from your Pokémon to Rayquaza ex. Spiral Blast: 20 per basic Energy card on it.</summary>
        private sealed class RayquazaExDragonBoost : Mon
        {
            private const string Power = "Dragon Boost";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    foreach (var e in mon.Energy.Where(c => c.Def.IsBasicEnergy)) holders[e] = mon;
                if (me == null || !TriggerOnce(ctx, Power, holders.Count > 0, Power + ": move basic Energy cards from your other Pokémon to Rayquaza ex?")) return;
                foreach (var card in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, Power + ": move any number of basic Energy cards to Rayquaza ex"))
                    ctx.Engine.MoveEnergy(holders[card], me, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Blast") return false;
                ctx.DamageDefending(atk.Damage * ctx.Pokemon.Energy.Count(e => e.Def.IsBasicEnergy));
                return true;
            }
        }

        /// <summary>Dragon Lift (Poké-Body): your Pokémon (not Pokémon-ex or Baby Pokémon) have no Retreat Cost. Flame Jet: 40 to 1 of the opponent's Pokémon (no W/R). Bright Flame: discard 2 Energy.</summary>
        private sealed class SalamenceExDragonLift : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && !mon.Def.IsOldEx && !mon.Def.IsBaby ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flame Jet")
                {
                    ctx.DamageAnyOpponent(40, true);
                    return true;
                }
                if (atk.Name != "Bright Flame") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Bright Flame: discard 2 Energy");
                return true;
            }
        }

        /// <summary>Darkness Blast: you may discard a Darkness Energy from Sharpedo ex; if you do, 20 more and discard an Energy card from the Defending Pokémon.</summary>
        private sealed class DarknessBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Blast") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                var dark = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Darkness)).ToList();
                bool more = false;
                if (!CardsBS1.CopyingWithMetronome && dark.Count > 0 && ctx.AskYesNo("Darkness Blast: discard a Darkness Energy for 20 more damage and to discard an Energy from the Defending Pokémon?"))
                {
                    var card = ctx.ChooseCards(dark, 1, 1, "Darkness Blast: discard which Energy?")[0];
                    more = ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                if (more && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Darkness Blast: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Healing Light: 1 damage counter off each of your Pokémon.</summary>
        private sealed class HealingLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Light") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>Miraculous Light: 1 damage counter and all Special Conditions off Latios ★.</summary>
        private sealed class MiraculousLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Light") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 10);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Shooting Star / Shining Star: against a matching Defending Pokémon, discard all Energy cards from this Pokémon and do 100 more.</summary>
        private sealed class StarStrikeMon : Mon
        {
            private readonly string attack;
            private readonly Func<PokemonInPlay, bool> target;

            public StarStrikeMon(string attack, Func<PokemonInPlay, bool> target)
            {
                this.attack = attack;
                this.target = target;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                bool hit = ctx.Defending != null && target(ctx.Defending);
                if (hit && !CardsBS1.CopyingWithMetronome) ctx.Engine.DiscardAllEnergy(me);
                ctx.DamageDefending(atk.Damage + (hit ? 100 : 0));
                return true;
            }
        }

        /// <summary>Spiral Rush: flip until tails, 30 per heads.</summary>
        private sealed class SpiralRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Rush") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>Holy Star: discard all Energy cards from Rayquaza ★; 100 to each of the opponent's Pokémon-ex.</summary>
        private sealed class HolyStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Holy Star") return false;
                if (!CardsBS1.CopyingWithMetronome) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                foreach (var mon in ctx.OpponentPokemon(m => m.Def.IsOldEx))
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 100);
                return true;
            }
        }

        /// <summary>
        /// Lightning Burst (Poké-Power): whenever you attach a Darkness Energy card from your hand to Rocket's Raikou ex, you may switch
        /// the Defending Pokémon with a Benched Pokémon the opponent chooses; not with a Special Condition. Thunderous Blow.
        /// </summary>
        private sealed class RaikouExLightningBurst : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !CardsRS.BasicOf(energy.Def, EnergyType.Darkness) || !CardsE1.NoCondition(me)) return;
                if (src.Opp.Active == null || src.Opp.Bench.Count == 0) return;
                if (!src.AskYesNo("Lightning Burst: switch the Defending Pokémon with 1 of your opponent's Benched Pokémon?")) return;
                Announce(src, "Lightning Burst");
                OpponentChoosesSwitch(src, "Lightning Burst");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderous Blow") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.Energy.Count(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)));
                return true;
            }
        }
    }
}
