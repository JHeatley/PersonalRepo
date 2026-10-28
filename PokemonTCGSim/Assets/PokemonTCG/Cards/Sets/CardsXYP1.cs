using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// XY Black Star Promos (XYP, v107), part 1: XY04-XY70. Many promos are prints of set cards and use those cards' scripts.
    /// </summary>
    public static class CardsXYP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("XYP-4", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("XYP-9", new ComboMon(new CoinsMon("Dual Chop", 2), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            r.RegisterByKey("XYP-11", new RecoilMon("Take Down", 10));
            r.RegisterByKey("XYP-12", new BaseDamageNextTurnMon("Swords Dance", "Slash", 40));
            r.RegisterByKey("XYP-14", new ComboMon(new ConditionAttackMon("Eerie Wave", SpecialCondition.Confused), new FlipTailsRecoilMon("Wood Hammer", 20)));
            r.RegisterByKey("XYP-15", new ComboMon(new ShieldAttackMon("Cotton Guard", 30), new ConditionAttackMon("Sleepy Ball", SpecialCondition.Asleep)));
            r.RegisterByKey("XYP-16", new ComboMon(new CardsASC.PushDownMon("Push Down"), new FlipPerTypeBonusMon("Forest Press", EnergyType.Grass, 30)));
            r.RegisterByKey("XYP-17", new ComboMon(new SearchAttackMon("Mega Ascension", d => d.Name == "M Charizard-EX", 1), new RecoilMon("Brave Fire", 30)));
            r.RegisterByKey("XYP-18", new ComboMon(new CoinsMon("Pin Missile", 4), new RecoilMon("Wild Tackle", 20)));
            r.RegisterByKey("XYP-19", new ComboMon(new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused), new WonderFlareMon()));
            r.RegisterByKey("XYP-20", new ComboMon(new CardsMEG.SnipeMon("Sharpshooting", 30), new CardsASC.SelfDiscardTypeMon("Aqua Blast", EnergyType.Water, 1)));
            r.RegisterByKey("XYP-22", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new DeepWindMon()));
            r.RegisterByKey("XYP-23", new ComboMon(new WhiskAwayMon(), new CardsWHT.FlipMoreMon("Spirit Dance", 2, 30)));
            r.RegisterByKey("XYP-24", new GreninjaConcealment());
            r.RegisterByKey("XYP-25", new ComboMon(new CardsMEW1.CounterBonusMon("Second Bite", 10), new MegatonFangMon()));
            r.RegisterByKey("XYP-26", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"), new CardsTWM1.SelfDiscardMon("Royal Flare", 2)));
            r.RegisterByKey("XYP-31", new ComboMon(new AuroraGainMon(), new CardsFST2.ConditionBonusMon("Light of Life", 40, c => c.Opp.InPlay().Any(m => m.Name == "Yveltal" || m.Name == "Yveltal-EX"))));
            r.RegisterByKey("XYP-32", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Air Crash"), new CardsFST2.ConditionBonusMon("Wings of Destruction", 40, c => c.Opp.InPlay().Any(m => m.Name == "Xerneas" || m.Name == "Xerneas-EX"))));
            r.RegisterByKey("XYP-34", new ComboMon(new MagneticLaserMon(), new CoinsMon("Squared Attack", 4)));
            r.RegisterByKey("XYP-35", new CardsMEG.EnergyBonusMon("Gatling Slug", EnergyType.Metal, 10));
            r.RegisterByKey("XYP-40", new DittoGene());
            r.RegisterByKey("XYP-43", new ComboMon(new CardsMEG.WeakenDefendingMon("Moonblast", 20), new CardsWHT.FlipMoreMon("Luminous Swirl", 2, 40)));
            r.RegisterByKey("XYP-44", new DiamondForceMon());
            r.RegisterByKey("XYP-45", new ComboMon(new CardsFST2.ConditionBonusMon("Assault Sword", 40, c => c.Defending != null && c.Defending.Energy.Count == 0), new LockAfterAttackMon("Cross Slash", true)));
            r.RegisterByKey("XYP-47", new HelioliskSkin());
            r.RegisterByKey("XYP-48", new MeowsticEars());
            r.RegisterByKey("XYP-49", new ComboMon(new LandMakerMon(), new FlipBonusMon("Stone Edge", 40)));
            r.RegisterByKey("XYP-50", new PangoroBoss());
            r.RegisterByKey("XYP-53", new ComboMon(new DodgeMon("Agility", false), new LockAfterAttackMon("Strong Slash", true)));
            r.RegisterByKey("XYP-54", new ComboMon(new FistOfFocusMon(), new FlipPerTypeBonusMon("Flare Storm", EnergyType.Fire, 20)));
            r.RegisterByKey("XYP-55", new ComboMon(new MudFloodMon(), new RecoilMon("Hydro Tackle", 20)));
            r.RegisterByKey("XYP-56", new ComboMon(new CoinsMon("Double Slap", 2), new NursesEggMon()));
            r.RegisterByKey("XYP-57", new ComboMon(new CoinsMon("Repeating Drill", 5), new RecoilMon("Nosedive", 20)));
            r.RegisterByKey("XYP-58", new MalamarContrary());
            r.RegisterByKey("XYP-59", new ComboMon(new BenchSearchAttackMon("Call for Goons", 3), new CardsBRS1.MillSelfMon("Dragon Pulse", 1)));
            r.RegisterByKey("XYP-61", new FlygonVoice());
            r.RegisterByKey("XYP-62", new ComboMon(new DarkFangMon(), new CardsMEG.WeakenDefendingMon("Dark Edge", 20)));
            r.RegisterByKey("XYP-63", new DisasterWingMon());
            r.RegisterByKey("XYP-64", new RayquazaOzone());
            r.RegisterByKey("XYP-65", new TailsNothingMon("Supersonic Flight"));
            r.RegisterByKey("XYP-66", new ComboMon(new SearchAttackMon("Mega Ascension", d => d.Name == "M Rayquaza-EX", 1), new FlipTailsSelfDiscardManyMon("Aeroscream", 2)));
            r.RegisterByKey("XYP-67", new ComboMon(new StardustMon(), new CardsPHF1.DreamDanceMon()));
            r.RegisterByKey("XYP-70", new TyrantrumFang());
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin. If tails, this Pokémon does N damage to itself." (Wood Hammer).</summary>
        internal sealed class FlipTailsRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int recoil;

            public FlipTailsRecoilMon(string attack, int recoil)
            {
                this.attack = attack;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(recoil);
                return true;
            }
        }

        /// <summary>"Flip a coin for each [type] Energy attached to this Pokémon. This attack does N more damage for each heads." (Forest Press).</summary>
        internal sealed class FlipPerTypeBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int per;

            public FlipPerTypeBonusMon(string attack, EnergyType type, int per)
            {
                this.attack = attack;
                this.type = type;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int coins = ctx.Engine.EnergyCountOfType(ctx.Pokemon, type);
                ctx.DamageDefending(atk.Damage + per * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, discard N Energy attached to this Pokémon." (Aeroscream).</summary>
        internal sealed class FlipTailsSelfDiscardManyMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public FlipTailsSelfDiscardManyMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, count, atk.Name);
                return true;
            }
        }

        /// <summary>"During your next turn, the base damage of this Pokémon's [attack] is N." (Swords Dance).</summary>
        internal sealed class BaseDamageNextTurnMon : Mon
        {
            private readonly string setup;
            private readonly string boosted;
            private readonly int baseDamage;

            public BaseDamageNextTurnMon(string setup, string boosted, int baseDamage)
            {
                this.setup = setup;
                this.boosted = boosted;
                this.baseDamage = baseDamage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == setup)
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s " + boosted + " has a base damage of " + baseDamage + " during its next turn (" + setup + ")", ctx.Player, me.Uid, ctx.Turn + 2, boosted, baseDamage);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name != boosted) return false;
                int uid = me.Uid;
                bool set = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Any(b => b.TargetUid == uid && b.AttackName == boosted);
                ctx.DamageDefending(set ? Math.Max(atk.Damage, baseDamage) : atk.Damage);
                return true;
            }
        }

        /// <summary>"If this Pokémon is your Active Pokémon, you may have your opponent switch their Active Pokémon" Abilities (Voice of the Sands).</summary>
        internal abstract class ActiveSwitchAbilityMon : Mon
        {
            protected abstract string AbilityName { get; }
            protected override string Activated { get { return AbilityName; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsActive(ctx.Pokemon) && ctx.Opp.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        /// <summary>Effect: the Pokémon has no Weakness (Aurora Gain).</summary>
        internal sealed class NoWeaknessEffect : ContinuousEffect
        {
            public NoWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon != null && mon.Uid == TargetUid ? (EnergyType?)null : weakness;
            }
        }

        // ================================================================== Attacks

        /// <summary>Wonder Flare: the opponent reveals their hand; 40 more damage for each Energy card there.</summary>
        private sealed class WonderFlareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wonder Flare") return false;
                TefPokemon.RevealOpponentHand(ctx);
                int n = ctx.Opp.Hand.Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>Deep Wind: if the opponent's Active Pokémon is Asleep, 60 more damage and heal 30 from this Pokémon.</summary>
        private sealed class DeepWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Wind") return false;
                bool asleep = ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Asleep);
                ctx.DamageDefending(atk.Damage + (asleep ? 60 : 0));
                if (asleep && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        /// <summary>Whisk Away: the opponent reveals their hand; put a Pokémon there on the bottom of their deck; if you do, 30 more damage.</summary>
        private sealed class WhiskAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whisk Away") return false;
                TefPokemon.RevealOpponentHand(ctx);
                var mons = ctx.Opp.Hand.Where(c => c.Def.IsPokemon).ToList();
                bool done = false;
                if (mons.Count > 0)
                {
                    var pick = ctx.ChooseCards(mons, 1, 1, "Whisk Away: put which Pokémon from your opponent's hand on the bottom of their deck?");
                    if (pick.Count > 0 && ctx.Opp.Hand.Remove(pick[0]))
                    {
                        ctx.Opp.Deck.Add(pick[0]);
                        ctx.Log(pick[0].Def.Name + " was put on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Whisk Away).");
                        done = true;
                    }
                }
                ctx.DamageDefending(atk.Damage + (done ? 30 : 0));
                return true;
            }
        }

        /// <summary>Mist Concealment: when it evolves from your hand, you may prevent all effects of attacks, including damage, done to it next turn. Shadow Bullet.</summary>
        private sealed class GreninjaConcealment : Mon
        {
            private static readonly Mon Bullet = new Cards30CC.BenchHitMon("Shadow Bullet", 20);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Mist Concealment", true, "Use Mist Concealment? Prevent all effects of attacks, including damage, done to " + ctx.Pokemon.Name + " during your opponent's next turn.")) return;
                ctx.ProtectSelfNextTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bullet.HandleAttack(ctx, atk); }
        }

        /// <summary>Megaton Fang: discard a card from your hand; if you can't, this attack does nothing.</summary>
        private sealed class MegatonFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megaton Fang") return false;
                if (ctx.Me.Hand.Count == 0) return Nothing(ctx);
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Megaton Fang: discard a card from your hand");
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Aurora Gain: during the opponent's next turn, this Pokémon has no Weakness.</summary>
        private sealed class AuroraGainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aurora Gain") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new NoWeaknessEffect(me.Name + " has no Weakness during the opponent's next turn (Aurora Gain)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Magnetic Laser: you may move a Metal Energy from 1 of your Benched Pokémon to this Pokémon.</summary>
        private sealed class MagneticLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Laser") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var givers = ctx.MyBench().Where(m => m.Energy.Any(en => CardsDRI1.ProvidesType(ctx, m, en, EnergyType.Metal))).ToList();
                if (givers.Count == 0 || !ctx.AskYesNo("Magnetic Laser: move a Metal Energy from 1 of your Benched Pokémon to " + me.Name + "?")) return true;
                var giver = ctx.ChoosePokemon(givers, "Magnetic Laser: move a Metal Energy from which Benched Pokémon?");
                if (giver == null) return true;
                var metal = giver.Energy.Where(en => CardsDRI1.ProvidesType(ctx, giver, en, EnergyType.Metal)).ToList();
                var card = metal.Count == 1 ? metal[0] : ctx.ChooseCards(metal, 1, 1, "Magnetic Laser: move which Energy?")[0];
                ctx.Engine.MoveEnergy(giver, me, card);
                return true;
            }
        }

        /// <summary>Metamorphosis Gene: while Active, it can use the attacks of the opponent's Active Pokémon. Stick On: a basic Energy from the discard pile onto it.</summary>
        private sealed class DittoGene : Mon
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                var me = src.Pokemon;
                var other = src.Opp.Active;
                if (me == null || other == null || !src.IsActive(me)) return list;
                var defs = new List<CardDef> { other.Def };
                if ((other.Def.IsLevelUp || other.Def.IsBreak) && other.Stack.Count >= 2) defs.Add(other.Stack[other.Stack.Count - 2].Def);
                var seen = new HashSet<string>();
                foreach (var def in defs)
                {
                    if (!seen.Add(def.BaseKey)) continue;
                    for (int i = 0; i < def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(def, i));
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stick On") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard();
                if (!ctx.InPlay(me) || pile.Count == 0) return true;
                var pick = pile.Select(c => c.Def.Name).Distinct().Count() == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Stick On: attach which basic Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, me);
                return true;
            }
        }

        /// <summary>Effect: no damage to the owner's Pokémon from the opponent's Pokémon-EX attacks; ends when the source leaves the Active Spot (Diamond Force).</summary>
        private sealed class DiamondForceEffect : ContinuousEffect
        {
            public DiamondForceEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                var source = engine.State.FindPokemon(TargetUid);
                if (source == null || !engine.State.IsActive(source)) return false;
                return info.Target != null && info.Target.Owner == Owner && info.Attacker != null && info.Attacker.Owner != Owner && info.Attacker.Def.IsPokemonEX;
            }
        }

        /// <summary>Diamond Force: during the opponent's next turn, no damage to your Pokémon from attacks of the opponent's Pokémon-EX (while this one stays Active).</summary>
        private sealed class DiamondForceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Force") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.IsActive(me))
                    ctx.AddEffect(new DiamondForceEffect("No damage to " + ctx.Engine.PlayerName(ctx.Player) + "'s Pokémon from Pokémon-EX attacks during the next turn (Diamond Force)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Dry Skin: 30 less damage from the opponent's Water Pokémon's attacks (after Weakness and Resistance). Hyper Beam.</summary>
        private sealed class HelioliskSkin : Mon
        {
            private static readonly Mon Beam = new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || info.Target != me || attacker == null || attacker.Owner == me.Owner || !src.Engine.IsType(attacker, EnergyType.Water)) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beam.HandleAttack(ctx, atk); }
        }

        /// <summary>Mysterious Ears: damaged by the opponent's attack in the Active Spot, the Attacking Pokémon is now Confused. Psyblast.</summary>
        private sealed class MeowsticEars : Mon
        {
            private static readonly Mon Blast = new CardsDRI2.CrunchMon("Psyblast");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || !ctx.IsActive(attacker)) return;
                Announce(ctx, "Mysterious Ears");
                ctx.ApplyCondition(attacker, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Land Maker: put 2 Stadium cards from your discard pile into your hand.</summary>
        private sealed class LandMakerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Land Maker") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsStadiumCard).ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(2, pile.Count);
                ctx.DiscardToHand(n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Land Maker: put 2 Stadium cards into your hand"));
                return true;
            }
        }

        /// <summary>Benevolent Boss: while Active, +20 HP for each of your Benched Pokémon. Crazy Knuckle: 40 more with a Special Condition.</summary>
        private sealed class PangoroBoss : Mon
        {
            private static readonly Mon Knuckle = new CardsFST2.ConditionBonusMon("Crazy Knuckle", 40, c => c.Pokemon.Conditions != SpecialCondition.None);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon != src.Pokemon || !src.IsActive(mon)) return hp;
                return hp + 20 * src.Me.Bench.Count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knuckle.HandleAttack(ctx, atk); }
        }

        /// <summary>Fist of Focus: attach an Energy card from your discard pile to this Pokémon.</summary>
        private sealed class FistOfFocusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fist of Focus") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (!ctx.InPlay(me) || pile.Count == 0) return true;
                var pick = ctx.ChooseCards(pile, 1, 1, "Fist of Focus: attach which Energy card to " + me.Name + "?");
                if (pick.Count > 0 && ctx.Me.Discard.Remove(pick[0])) ctx.Engine.AttachEnergyCard(pick[0], me);
                return true;
            }
        }

        /// <summary>Mud Flood: reveal the top 4 cards of your deck; 40 more for each Water Energy card there; shuffle them back.</summary>
        private sealed class MudFloodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mud Flood") return false;
                var top = ctx.Me.Deck.Take(Math.Min(4, ctx.Me.Deck.Count)).ToList();
                if (top.Count > 0) ctx.Engine.Reveal(ctx.Player, top, "revealed the top cards of " + ctx.Engine.Their(ctx.Player) + " deck (Mud Flood)");
                int n = top.Count(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water));
                ctx.ShuffleMyDeck();
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>Nurse's Egg: heal 100 damage from each of your Benched Pokémon, then discard 2 Energy from this Pokémon.</summary>
        private sealed class NursesEggMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nurse's Egg") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) if (mon.DamageCounters > 0) ctx.Heal(mon, 100);
                if (ctx.InPlay(ctx.Pokemon)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Contrary (θ Stop): while this Pokémon is Active, the opponent's coin flips during their turn are all tails. Conform: Confused with equal hands.</summary>
        private sealed class MalamarContrary : Mon
        {
            public override bool ForcesTailsFor(EffectContext src, int player)
            {
                var me = src.Pokemon;
                return me != null && player != src.Player && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conform") return false;
                var defending = ctx.Defending;
                bool same = ctx.Me.Hand.Count == ctx.Opp.Hand.Count;
                ctx.DamageDefending(atk.Damage);
                if (same && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Voice of the Sands: once during your turn, if Active, the opponent switches their Active Pokémon. Spiral Buzz.</summary>
        private sealed class FlygonVoice : ActiveSwitchAbilityMon
        {
            private static readonly Mon Buzz = new CardsMEG.FlipUntilTailsBonusMon("Spiral Buzz", 30);

            protected override string AbilityName { get { return "Voice of the Sands"; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Buzz.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Fang: flip a coin; heads, discard a random card from the opponent's hand.</summary>
        private sealed class DarkFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Fang") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>Disaster Wing: discard the top card of the opponent's deck; if it's a Trainer card, 80 more damage.</summary>
        private sealed class DisasterWingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disaster Wing") return false;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                bool trainer = top.Any(c => c.Def.IsTrainer);
                ctx.DamageDefending(atk.Damage + (trainer ? 80 : 0));
                return true;
            }
        }

        /// <summary>Ozone Wall: while on your Bench, no damage from attacks (either player's). Dragon Pulse: discard the top 3 cards of your deck.</summary>
        private sealed class RayquazaOzone : Mon
        {
            private static readonly Mon Pulse = new CardsBRS1.MillSelfMon("Dragon Pulse", 3);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pulse.HandleAttack(ctx, atk); }
        }

        /// <summary>Stardust: discard a Special Energy from the opponent's Active Pokémon; if you do, prevent all effects of attacks done to this Pokémon next turn.</summary>
        private sealed class StardustMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stardust") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                if (!defending.Energy.Any(en => en.Def.IsSpecialEnergy)) return true;
                if (ctx.DiscardEnergyFrom(defending, 1, "Stardust: discard a Special Energy from " + defending.Name, en => en.Def.IsSpecialEnergy) > 0) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Despotic Fang: this Pokémon's attack damage isn't affected by effects on the opponent's Active Pokémon. Dragon Impact: discard 3 Energy.</summary>
        private sealed class TyrantrumFang : Mon
        {
            private static readonly Mon Impact = new CardsTWM1.SelfDiscardMon("Dragon Impact", 3);

            public override bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Attacker == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Impact.HandleAttack(ctx, atk); }
        }
    }
}
