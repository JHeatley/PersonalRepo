using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Genesis Pokémon (N1 1-82, 2000; v138). Unlimited only. Pokémon Powers can't be used (and passive ones stop) while their
    /// Pokémon is Asleep, Confused or Paralyzed, and they aren't Abilities (rule 144). Pichu, Cleffa, Elekid and Magby are Baby
    /// Pokémon (the Baby Rule, rule 173). Cleffa 20 and Sneasel 25 are the Classic Collection cards already in the pool.
    /// Your rulings: Zzzap and Sputter touch only Pokémon Powers; Slowking's Mind Games works only while Slowking is the Active
    /// Pokémon (the Japanese text) and a Supporter it stops isn't the opponent's Supporter for the turn.
    /// Pokémon whose attacks only do their printed damage: Magmar 40.
    /// </summary>
    public static class CardsN1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N1-1", new FlipParalyzeSpreadMon("Gigaspark"));
            r.RegisterByKey("N1-2", new FlipParalyzeSpreadMon("Bubble Shower"));
            r.RegisterByKey("N1-3", new ComboMon(new SweetNectarMon(), new CardsASR1.TimesMon("Flower Dance", BellossomCount)));
            r.RegisterByKey("N1-4", new BerserkMon());
            r.RegisterByKey("N1-5", new DownpourMon());
            r.RegisterByKey("N1-6", new FinalBlowMon());
            r.RegisterByKey("N1-7", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new CardsBS1.LeechSeedMon()));
            r.RegisterByKey("N1-8", new ComboMon(new DodgeMon("Agility", false), new CardsG2.DragonTornadoMon()));
            r.RegisterByKey("N1-9", new ElementalBlastMon());
            r.RegisterByKey("N1-10", new HerbalScentMon());
            r.RegisterByKey("N1-11", new WildGrowthMon());
            r.RegisterByKey("N1-12", new ZzzapMon());
            r.RegisterByKey("N1-13", new ComboMon(new TailsNothingMon("Claw"), new FlipGuardMon("Steel Wing", 20)));
            r.RegisterByKey("N1-14", new MindGamesMon());
            r.RegisterByKey("N1-15", new FlipBonusMon("Tail Crush", 20));
            r.RegisterByKey("N1-16", new ComboMon(new SuperMetronomeMon(), new DodgeMon("Fly", true)));
            r.RegisterByKey("N1-17", new FireRechargeMon());
            r.RegisterByKey("N1-18", new FireBoostMon());
            r.RegisterByKey("N1-21", new ComboMon(new CardsASR1.TimesMon("Flail", SelfCounters), new CardsWP.HitmontopWP()));
            r.RegisterByKey("N1-22", new PlayfulPunchMon());
            r.RegisterByKey("N1-23", new SputterMon());
            r.RegisterByKey("N1-24", new ComboMon(new MeanLookMon(), new CardsCES1.FeintAttackMon("Feint Attack", 20)));
            r.RegisterByKey("N1-26", new ComboMon(new PilferMon(), new CoinsMon("Tail Rap", 2)));
            r.RegisterByKey("N1-27", new ComboMon(new SpiderWebMon(), new PoisonBiteMon()));
            r.RegisterByKey("N1-28", new ComboMon(new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned), new PollenShieldMon()));
            r.RegisterByKey("N1-29", new ComboMon(new SweetScentFlipMon(), new CoinsMon("Double Razor Leaf", 2)));
            r.RegisterByKey("N1-30", new ComboMon(new CoinsMon("Doubleslap", 2), new SquaredanceMon()));
            r.RegisterByKey("N1-31", new ComboMon(new CardsCES2.ScreechMon("Screech", 20), new ClampMon("Jaw Clamp", false, 1)));
            r.RegisterByKey("N1-32", new CardsBRS1.MillSelfMon("Sweep Away", 3));
            r.RegisterByKey("N1-33", new CardsCRZ1.PiercingMon("Swift"));
            r.RegisterByKey("N1-34", new CardsG1.CheckedCombo(new CardsG1.DischargeMon(), new CardsG1.ElectricCurrentMon()));
            r.RegisterByKey("N1-35", new ComboMon(new FlipBonusMon("Quick Attack", 10), new CoinsMon("Slam", 2)));
            r.RegisterByKey("N1-36", new ComboMon(new CardsG2.StrangePowderMon(), new ClampMon("Sticky Nectar", true, 2)));
            r.RegisterByKey("N1-37", new RagingChargeMon());
            r.RegisterByKey("N1-38", new LanturnN1());
            r.RegisterByKey("N1-39", new BatonPassMon());
            r.RegisterByKey("N1-40", new PlainMon());
            r.RegisterByKey("N1-41", new ComboMon(new MilkDrinkMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("N1-42", new GlaringGazeMon());
            r.RegisterByKey("N1-43", new CardsCES2.EndureMon("Endure"));
            r.RegisterByKey("N1-44", new ComboMon(new FreezeMon(), new CardsFO.BlizzardMon()));
            r.RegisterByKey("N1-45", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10));
            r.RegisterByKey("N1-46", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, new FireWindMon()));
            r.RegisterByKey("N1-47", new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"), new CharMon()));
            r.RegisterByKey("N1-48", new ComboMon(new ParalyzeFlipMon("Bubble"), new CardsG1.ChooseBenchedMon("Mud Splash", 1, 10, true)));
            r.RegisterByKey("N1-49", new ComboMon(new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned), new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("N1-50", new CardsBLW.PetalDanceMon("Petal Dance"));
            r.RegisterByKey("N1-51", new ConditionAttackMon("Poison Barb", SpecialCondition.Poisoned));
            r.RegisterByKey("N1-52", new ComboMon(new CardsFO.ProphecyMon(), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("N1-53", new CardsG1.DeflectorMon());
            r.RegisterByKey("N1-54", new CardsJU.ReduceFromDefendingMon("Growl", 10));
            r.RegisterByKey("N1-55", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsASR1.TimesMon("Flail", SelfCounters)));
            r.RegisterByKey("N1-56", new ComboMon(new CardsJU.CantAttackThisMon("Leer"), new CardsCRZ1.PiercingMon("Swift")));
            r.RegisterByKey("N1-57", new ComboMon(new CardsFLF1.FlipTailsSelfDiscardMon("Fireworks"), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("N1-58", new ComboMon(new DodgeMon("Agility", false), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("N1-59", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("N1-60", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("N1-61", new CardsJU.FamilyMon("Sprout", d => d.Name == "Hoppip", "a Basic Pokémon named Hoppip"));
            r.RegisterByKey("N1-62", new FinSlapMon());
            r.RegisterByKey("N1-63", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CoinsMon("Comet Punch", 4)));
            r.RegisterByKey("N1-64", new DodgeMon("Undulate", false));
            r.RegisterByKey("N1-65", new ComboMon(new StaticElectricityMon(), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("N1-66", new ComboMon(new CardsMEW1.WithdrawMon("Defense Curl"), new BubbleBombMon()));
            r.RegisterByKey("N1-67", new TelekinesisMon());
            r.RegisterByKey("N1-68", new ComboMon(new DodgeMon("Hide", false), new CardsJU.HalfDrainMon("Absorb")));
            r.RegisterByKey("N1-69", new ComboMon(new CardsCES2.ScreechMon("Screech", 20), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("N1-70", new ComboMon(new FlipBonusMon("Quick Attack", 20), new DodgeMon("Agility", false)));
            r.RegisterByKey("N1-71", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("N1-72", new ComboMon(new CardsMEW1.WithdrawMon("Withdraw"), new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("N1-73", new ComboMon(new ParalyzeFlipMon("Psyshock"), new CardsBS1.ExtraWaterMon("Water Gun")));
            r.RegisterByKey("N1-74", new ComboMon(new RoarFlipMon(), new ParalyzeFlipMon("Lick")));
            r.RegisterByKey("N1-75", new ComboMon(new ScaryFaceMon(), new ParalyzeFlipMon("String Shot")));
            r.RegisterByKey("N1-76", new ComboMon(new FlipBonusMon("Stomp", 10), new CardsPAR2.FlipConditionMon("Mystifying Horns", SpecialCondition.Confused)));
            r.RegisterByKey("N1-77", new CardsASR1.TimesMon("Flail", SelfCounters));
            r.RegisterByKey("N1-78", new ComboMon(new CardsG2.GrowthMon(), new CardsJU.HalfDrainMon("Mega Drain")));
            r.RegisterByKey("N1-79", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("N1-80", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("N1-81", new ComboMon(new CardsJU.CantAttackThisMon("Leer"), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("N1-82", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CoinsMon("Slam", 2)));
        }

        // ================================================================== Shared

        /// <summary>"This power can't be used if this Pokémon is Asleep, Confused, or Paralyzed."</summary>
        internal static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        private static int SelfCounters(EffectContext ctx) { return ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters; }

        private static int BellossomCount(EffectContext ctx) { return ctx.MyPokemon(m => m.Name.Contains("Bellossom")).Count; }

        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        /// <summary>A Pokémon Power of this Pokémon works now: not Asleep, Confused or Paralyzed, and nothing stops Pokémon Powers.</summary>
        private static bool PowerUsable(EffectContext ctx, PokemonInPlay mon, string power)
        {
            return mon != null && ctx.InPlay(mon) && PowerWorks(mon) && !ctx.Engine.AbilityBlocked(mon, power);
        }

        private static bool BasicOf(CardDef d, EnergyType type) { return d.IsEnergy && d.IsBasicEnergy && d.EnergyProvides == type; }

        /// <summary>
        /// "As long as [this Pokémon] is your Active Pokémon" (Murkrow's Mean Look, Croconaw's Jaw Clamp, Gloom's Sticky Nectar):
        /// the attacker carries this effect, which ends when it's Benched, evolves or leaves play.
        /// </summary>
        private sealed class ActiveAnchorEffect : ContinuousEffect
        {
            public ActiveAnchorEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }
        }

        private static bool AnchorHolds(GameEngine engine, string name, int owner, int uid)
        {
            foreach (var e in engine.State.Effects)
                if (e is ActiveAnchorEffect && e.Name == name && e.Owner == owner && e.TargetUid == uid) return true;
            return false;
        }

        // ================================================================== Attacks

        /// <summary>Gigaspark / Bubble Shower: heads, the Defending Pokémon is now Paralyzed and 10 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class FlipParalyzeSpreadMon : Mon
        {
            private readonly string attack;

            public FlipParalyzeSpreadMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>Sweet Nectar: heads, remove all damage counters from 1 of your Pokémon.</summary>
        private sealed class SweetNectarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Nectar") return false;
                if (!ctx.Flip()) return true;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Sweet Nectar: remove all damage counters from which of your Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Berserk (Pokémon Power): when you play this Pokémon from your hand, flip a coin; heads, the top 5 cards of the opponent's
        /// deck are discarded, tails the top 5 of yours. Chomp: a coin for each damage counter on it, 50 + 10 for each heads.
        /// </summary>
        private sealed class BerserkMon : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !PowerWorks(me)) return;
                Announce(ctx, "Berserk");
                int player = ctx.Flip() ? ctx.Opponent : ctx.Player;
                if (ctx.State.Players[player].Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(player, 5);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chomp") return false;
                int coins = ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters;
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>
        /// Downpour (Pokémon Power): as often as you like during your turn, discard a Water Energy card from your hand. Riptide: 10 + 10
        /// for each Water Energy card in your discard pile, then shuffle them all into your deck.
        /// </summary>
        private sealed class DownpourMon : Mon
        {
            private const string Downpour = "Downpour";

            private static List<CardInstance> Water(EffectContext ctx) { return ctx.Me.Hand.Where(c => BasicOf(c.Def, EnergyType.Water)).ToList(); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, Downpour) && ctx.IsMyTurn && PowerWorks(ctx.Pokemon) && Water(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var water = Water(ctx);
                if (water.Count == 0) return;
                var card = ctx.ChooseCards(water, 1, 1, Downpour + ": discard which Water Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Riptide") return false;
                var water = ctx.Me.Discard.Where(c => BasicOf(c.Def, EnergyType.Water)).ToList();
                ctx.DamageDefending(atk.Damage + 10 * water.Count);
                water = ctx.Me.Discard.Where(c => BasicOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                ctx.DiscardToDeck(water);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Megahorn: tails, nothing. Final Blow (Pokémon Power): with 20 HP or less left, you may make its base damage 120.</summary>
        private sealed class FinalBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megahorn") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var me = ctx.Pokemon;
                int damage = atk.Damage;
                if (PowerUsable(ctx, me, "Final Blow") && ctx.Engine.RemainingHp(me) <= 20
                    && ctx.AskYesNo("Final Blow: make Megahorn's base damage 120 instead of " + atk.Damage + "?"))
                {
                    Announce(ctx, "Final Blow");
                    damage = 120;
                }
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>
        /// Elemental Blast: discard a Fire, a Water and a Lightning Energy card attached to this Pokémon in order to use it (three
        /// different cards; it can't be used without them).
        /// </summary>
        private sealed class ElementalBlastMon : Mon
        {
            private static readonly EnergyType[] Types = { EnergyType.Fire, EnergyType.Water, EnergyType.Lightning };

            /// <summary>Three different Energy cards, one for each type, or null.</summary>
            private static List<CardInstance> Payment(EffectContext ctx, PokemonInPlay me)
            {
                if (me == null) return null;
                var cards = me.Energy.ToList();
                foreach (var a in cards)
                    foreach (var b in cards)
                        foreach (var c in cards)
                        {
                            if (a == b || a == c || b == c) continue;
                            if (CardsDRI1.ProvidesType(ctx, me, a, Types[0]) && CardsDRI1.ProvidesType(ctx, me, b, Types[1]) && CardsDRI1.ProvidesType(ctx, me, c, Types[2]))
                                return new List<CardInstance> { a, b, c };
                        }
                return null;
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Elemental Blast" || Payment(ctx, ctx.Pokemon) != null;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Elemental Blast") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var pay = Payment(ctx, me);
                    if (pay == null) return Nothing(ctx);
                    foreach (var card in pay) ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Herbal Scent (Pokémon Power): when you play this Pokémon from your hand, you may flip a coin; heads, remove all damage
        /// counters from all Grass Pokémon in play (both players'). Body Slam: heads, Paralyzed.
        /// </summary>
        private sealed class HerbalScentMon : Mon
        {
            private static readonly Mon BodySlam = new ParalyzeFlipMon("Body Slam");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !PowerWorks(me) || !ctx.AskYesNo("Herbal Scent: flip a coin to heal all Grass Pokémon in play?")) return;
                Announce(ctx, "Herbal Scent");
                if (!ctx.Flip()) return;
                for (int i = 0; i < 2; i++)
                    foreach (var mon in ctx.State.Players[i].InPlay().ToList())
                        if (mon.DamageCounters > 0 && ctx.Engine.IsType(mon, EnergyType.Grass)) ctx.Heal(mon, mon.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BodySlam.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wild Growth (Pokémon Power): each Grass Energy card attached to your Grass Pokémon provides 2 Grass Energy. It doesn't
        /// stack, and stops while this Pokémon is Asleep, Confused or Paralyzed. Soothing Scent: Asleep.
        /// </summary>
        private sealed class WildGrowthMon : Mon
        {
            private static readonly Mon Soothing = new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder == null || holder.Owner != src.Player || units.Count != 1 || !PowerWorks(src.Pokemon)) return;
                if (!BasicOf(energy.Def, EnergyType.Grass) || !src.Engine.IsType(holder, EnergyType.Grass)) return;
                units.Add(units[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Soothing.HandleAttack(ctx, atk); }
        }

        /// <summary>Zzzap: 20 damage to each Pokémon in play that has a Pokémon Power (yours too), no Weakness or Resistance (your ruling: only Pokémon Powers).</summary>
        private sealed class ZzzapMon : Mon
        {
            private static bool HasPokemonPower(PokemonInPlay mon)
            {
                foreach (var a in mon.Def.Abilities) if (a.Kind == AbilityDef.PokemonPower) return true;
                return false;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zzzap") return false;
                var targets = ctx.OpponentPokemon(HasPokemonPower);
                targets.AddRange(ctx.MyPokemon(HasPokemonPower));
                foreach (var mon in targets)
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, all damage done by attacks to this Pokémon during your opponent's next turn is reduced by N" (Steel Wing).</summary>
        private sealed class FlipGuardMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FlipGuardMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(amount);
                return true;
            }
        }

        /// <summary>
        /// Mind Games (Pokémon Power, only while this Pokémon is your Active Pokémon: your ruling): whenever your opponent plays a
        /// Trainer card, you may flip a coin; heads, it does nothing and goes on top of their deck. Mind Blast: heads, 10 more and Confused.
        /// </summary>
        private sealed class MindGamesMon : Mon
        {
            private static readonly Mon MindBlast = new CardsPLS1.FlipBonusConditionMon("Mind Blast", 10, SpecialCondition.Confused);

            public override bool StopsOpponentsTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                if (me == null || player == src.Player || !src.IsActive(me) || !PowerWorks(me)) return false;
                if (!src.AskYesNo("Mind Games: flip a coin to stop " + card.Def.Name + "?")) return false;
                Announce(src, "Mind Games");
                if (!src.Flip()) return false;
                var owner = src.State.Players[card.Owner];
                owner.Deck.Insert(0, card);
                src.Log(card.Def.Name + " does nothing and goes on top of " + src.Engine.Their(card.Owner) + " deck (Mind Games).");
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MindBlast.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Super Metronome: flip a coin; heads, choose an attack of 1 of your opponent's Pokémon and use it as this attack without its
        /// Energy cost (anything else needed to use it still applies). This Pokémon stays Colorless.
        /// </summary>
        private sealed class SuperMetronomeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Metronome") return false;
                if (!ctx.Flip()) return true;
                var withAttacks = ctx.OpponentPokemon(m => m.Def.Attacks.Count > 0 || (m.Stack.Count >= 2 && (m.Def.IsLevelUp || m.Def.IsBreak)));
                if (withAttacks.Count == 0) return true;
                var mon = ctx.ChoosePokemon(withAttacks, "Super Metronome: copy an attack of which of your opponent's Pokémon?");
                CardDef source;
                int index;
                if (mon == null || !ChooseCopiedAttack(ctx, mon, "Super Metronome: choose the attack to copy", out source, out index)) return true;
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>
        /// Fire Recharge (Pokémon Power): once during your turn, flip a coin; heads, attach a Fire Energy card from your discard pile
        /// to 1 of your Fire Pokémon. Flame Burst: heads, 20 more damage and 20 damage to itself.
        /// </summary>
        private sealed class FireRechargeMon : Mon
        {
            private const string FireRecharge = "Fire Recharge";

            protected override string Activated { get { return FireRecharge; } }

            private static List<CardInstance> Fire(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire); }

            private static List<PokemonInPlay> FirePokemon(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire)); }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && Fire(ctx).Count > 0 && FirePokemon(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var fire = Fire(ctx);
                var targets = FirePokemon(ctx);
                if (fire.Count == 0 || targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, FireRecharge + ": attach a Fire Energy card to which of your Fire Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(fire[0])) ctx.Engine.AttachEnergyCard(fire[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Burst") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                return true;
            }
        }

        /// <summary>
        /// Fire Boost (Pokémon Power): when you play this Pokémon from your hand, you may flip a coin; heads, attach up to 4 Fire Energy
        /// cards from your deck to it. Flame Wheel: discard 3 Fire Energy cards from it to use it; 20 to each Benched Pokémon (both sides).
        /// </summary>
        private sealed class FireBoostMon : Mon
        {
            private static readonly Mon FlameWheel = new CardsBS1.DiscardCostMon("Flame Wheel", EnergyType.Fire, 3, FlameWheelEffect, null);

            private static void FlameWheelEffect(EffectContext ctx, AttackDef atk)
            {
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 20);
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex) { return FlameWheel.CanUseAttack(ctx, attackIndex); }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !PowerWorks(me) || ctx.Me.Deck.Count == 0 || !ctx.AskYesNo("Fire Boost: flip a coin to attach Fire Energy from your deck?")) return;
                Announce(ctx, "Fire Boost");
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => BasicOf(d, EnergyType.Fire), 4, "Fire Boost: choose up to 4 Fire Energy cards to attach to " + me.Name);
                foreach (var card in found)
                {
                    if (ctx.InPlay(me)) ctx.Engine.AttachEnergyCard(card, me);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FlameWheel.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Playful Punch (Pokémon Power): once during your turn, flip a coin; heads, 20 damage to the opponent's Active Pokémon (Weakness
        /// and Resistance apply). Either way your turn ends.
        /// </summary>
        private sealed class PlayfulPunchMon : Mon
        {
            private const string PlayfulPunch = "Playful Punch";

            protected override string Activated { get { return PlayfulPunch; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target != null && ctx.Flip())
                {
                    int damage = CardsTR.WithWeaknessAndResistance(ctx.Engine, ctx.Pokemon, target, 20);
                    ctx.Log(PlayfulPunch + " does " + damage + " damage to " + target.Name + ".");
                    if (damage >= 10) ctx.Engine.PlaceDamageCounters(target, damage / 10, PlayfulPunch);
                }
                ctx.Log(ctx.Engine.Possessive(ctx.Player) + " turn ends (" + PlayfulPunch + ").");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Sputter: the damage, then all Pokémon Powers stop working until the end of your next turn (only Pokémon Powers: your ruling).</summary>
        private sealed class SputterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sputter") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsTR.GoopGasEffect("All Pokémon Powers stop working until the end of " + ctx.Engine.Their(ctx.Player) + " next turn (Sputter)", ctx.Player, ctx.Turn + 2));
                ctx.Log("All Pokémon Powers stop working until the end of " + ctx.Engine.Their(ctx.Player) + " next turn (Sputter).");
                return true;
            }
        }

        /// <summary>Mean Look: the Defending Pokémon can't retreat as long as this Pokémon stays your Active Pokémon (Benching or evolving either ends it).</summary>
        private sealed class MeanLookMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mean Look") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (me == null || !ctx.InPlay(me) || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                string anchor = "Mean Look on " + defending.Uid;
                ctx.AddEffect(new ActiveAnchorEffect(anchor, ctx.Player, me.Uid, int.MaxValue));
                ctx.AddEffect(new MeanLookEffect(defending.Name + " can't retreat while " + me.Name + " is the Active Pokémon (Mean Look)", ctx.Player, defending.Uid, anchor, me.Uid));
                return true;
            }
        }

        private sealed class MeanLookEffect : ContinuousEffect
        {
            private readonly string anchor;
            private readonly int anchorUid;

            public MeanLookEffect(string name, int owner, int targetUid, string anchor, int anchorUid) : base(name, owner, targetUid, int.MaxValue, true)
            {
                this.anchor = anchor;
                this.anchorUid = anchorUid;
            }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon)
            {
                return mon != null && mon.Uid == TargetUid && AnchorHolds(engine, anchor, Owner, anchorUid);
            }
        }

        /// <summary>
        /// Jaw Clamp (until the end of the opponent's next turn) / Sticky Nectar (heads: 10 more damage, until the end of your next
        /// turn): as long as this Pokémon is your Active Pokémon, the Defending Pokémon can't retreat, and no attack, Pokémon Power
        /// or Trainer card effect can change the opponent's Active Pokémon.
        /// </summary>
        private sealed class ClampMon : Mon
        {
            private readonly string attack;
            private readonly bool flip;
            private readonly int turns;

            public ClampMon(string attack, bool flip, int turns)
            {
                this.attack = attack;
                this.flip = flip;
                this.turns = turns;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                bool heads = !flip || ctx.Flip();
                ctx.DamageDefending(atk.Damage + (flip && heads ? 10 : 0));
                if (!heads || me == null || !ctx.InPlay(me) || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                string anchor = attack + " on " + defending.Uid + " (turn " + ctx.Turn + ")";
                int expires = ctx.Turn + turns;
                ctx.AddEffect(new ActiveAnchorEffect(anchor, ctx.Player, me.Uid, expires));
                ctx.AddEffect(new ClampEffect(defending.Name + " can't retreat or be switched out while " + me.Name + " is the Active Pokémon (" + attack + ")",
                    ctx.Player, defending.Uid, expires, anchor, me.Uid));
                return true;
            }
        }

        private sealed class ClampEffect : ContinuousEffect
        {
            private readonly string anchor;
            private readonly int anchorUid;

            public ClampEffect(string name, int owner, int targetUid, int expiresAfterTurn, string anchor, int anchorUid) : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                this.anchor = anchor;
                this.anchorUid = anchorUid;
            }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon)
            {
                return mon != null && mon.Uid == TargetUid && AnchorHolds(engine, anchor, Owner, anchorUid);
            }

            public override bool LocksActiveSpot(GameEngine engine, int player)
            {
                return player != Owner && AnchorHolds(engine, anchor, Owner, anchorUid);
            }
        }

        /// <summary>Pilfer: shuffle this Pokémon and all cards attached to it into your deck; then flip a coin, heads, shuffle a card from your discard pile into your deck.</summary>
        private sealed class PilferMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pilfer") return false;
                var me = ctx.Pokemon;
                if (me != null && ctx.InPlay(me)) ctx.Engine.ShufflePokemonIntoDeck(me);
                if (!ctx.Flip() || ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Pilfer: shuffle which card from your discard pile into your deck?");
                if (pick.Count == 0) return true;
                ctx.DiscardToDeck(pick);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Spider Web: heads, the Defending Pokémon can't retreat (until it's Benched or evolves).</summary>
        private sealed class SpiderWebMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Web") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantRetreatEffect(defending.Name + " can't retreat (Spider Web)", ctx.Player, defending.Uid, int.MaxValue));
                return true;
            }
        }

        /// <summary>Poison Bite: if it damages the Defending Pokémon, that Pokémon is now Poisoned and this Pokémon heals half that damage (rounded up to 10).</summary>
        internal sealed class PoisonBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Bite") return false;
                var defending = ctx.Defending;
                int done = ctx.DamageDefending(atk.Damage);
                if (done <= 0) return true;
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                var me = ctx.Pokemon;
                int heal = (done / 2 + 9) / 10 * 10;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, heal);
                return true;
            }
        }

        /// <summary>Pollen Shield: during the opponent's next turn, this Pokémon can't become Asleep, Confused, Paralyzed or Poisoned.</summary>
        private sealed class PollenShieldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pollen Shield") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fx = new PollenShieldEffect(me.Name + " can't become Asleep, Confused, Paralyzed or Poisoned during the opponent's next turn (Pollen Shield)", ctx.Player, me.Uid, ctx.Turn + 1);
                fx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(fx);
                return true;
            }
        }

        private sealed class PollenShieldEffect : ContinuousEffect
        {
            private const SpecialCondition Shielded = SpecialCondition.Asleep | SpecialCondition.Confused | SpecialCondition.Paralyzed | SpecialCondition.Poisoned;

            public PollenShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsCondition(GameEngine engine, PokemonInPlay mon, SpecialCondition condition)
            {
                return mon != null && mon.Uid == TargetUid && (condition & Shielded) != 0;
            }
        }

        /// <summary>Sweet Scent: heads, remove 2 damage counters from 1 of your Pokémon; tails, from 1 of your opponent's Pokémon (you choose).</summary>
        private sealed class SweetScentFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Scent") return false;
                bool heads = ctx.Flip();
                var hurt = heads ? ctx.MyPokemon(m => m.DamageCounters > 0) : ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Sweet Scent: remove 2 damage counters from which " + (heads ? "of your" : "of your opponent's") + " Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Squaredance: a coin for each Pokémon in play (both sides); for each heads, you may put a basic Energy card from your deck into your hand.</summary>
        private sealed class SquaredanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Squaredance") return false;
                int coins = ctx.MyPokemon().Count + ctx.OpponentPokemon().Count;
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads == 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => d.IsEnergy && d.IsBasicEnergy, heads, "Squaredance: put up to " + heads + " basic Energy cards from your deck into your hand");
                return true;
            }
        }

        /// <summary>Raging Charge: 10 + 10 for each damage counter on this Pokémon, then 20 damage to itself.</summary>
        private sealed class RagingChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Charge") return false;
                int counters = ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters;
                ctx.DamageDefending(atk.Damage + 10 * counters);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                return true;
            }
        }

        /// <summary>
        /// Floodlight: heads, Paralyzed. Hydroelectric Power (Pokémon Power): you may make it do 10 more damage for each Water Energy
        /// attached to this Pokémon that isn't needed for Floodlight's cost.
        /// </summary>
        private sealed class LanturnN1 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Floodlight") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int damage = atk.Damage;
                if (me != null && PowerUsable(ctx, me, "Hydroelectric Power"))
                {
                    int water = ctx.Engine.EnergyCountOfType(me, EnergyType.Water);
                    int spare = Math.Max(0, Math.Min(water, ctx.EnergyCount(me) - atk.Cost.Length));
                    if (spare > 0 && ctx.AskYesNo("Hydroelectric Power: Floodlight does " + 10 * spare + " more damage?"))
                    {
                        Announce(ctx, "Hydroelectric Power");
                        damage += 10 * spare;
                    }
                }
                ctx.DamageDefending(damage);
                if (ctx.Flip() && defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Baton Pass: if you have a Grass Pokémon on your Bench, move all Grass Energy cards from this Pokémon to 1 of them, then switch with it.</summary>
        private sealed class BatonPassMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Baton Pass") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !ctx.IsActive(me)) return true;
                var grass = ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Grass)).ToList();
                if (grass.Count == 0) return true;
                var to = ctx.ChoosePokemon(grass, "Baton Pass: move the Grass Energy to which of your Benched Grass Pokémon?");
                if (to == null) return true;
                foreach (var card in me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Grass)).ToList())
                    ctx.Engine.MoveEnergy(me, to, card);
                if (ctx.InPlay(to) && ctx.Me.Bench.Contains(to)) ctx.Engine.SwitchActive(ctx.Player, to);
                return true;
            }
        }

        /// <summary>Milk Drink: flip 2 coins; remove 2 damage counters from this Pokémon for each heads.</summary>
        private sealed class MilkDrinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Milk Drink") return false;
                int heads = ctx.FlipCoins(2);
                var me = ctx.Pokemon;
                if (heads > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20 * heads);
                return true;
            }
        }

        /// <summary>
        /// Glaring Gaze (Pokémon Power): once during your turn, flip a coin; heads, look at your opponent's hand and choose a Trainer
        /// card there; they shuffle it into their deck.
        /// </summary>
        private sealed class GlaringGazeMon : Mon
        {
            private const string GlaringGaze = "Glaring Gaze";

            protected override string Activated { get { return GlaringGaze; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", GlaringGaze + ": your opponent's hand", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand (" + GlaringGaze + ").");
                var trainers = hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return;
                var pick = ctx.ChooseCards(trainers, 1, 1, GlaringGaze + ": your opponent shuffles which Trainer card into their deck?");
                if (pick.Count == 0 || !ctx.Opp.Hand.Remove(pick[0])) return;
                ctx.Opp.Deck.Add(pick[0]);
                ctx.Engine.Reveal(ctx.Opponent, pick, "shuffled into the deck (Glaring Gaze)");
                ctx.Engine.ShuffleDeck(ctx.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>Freeze: heads, the Defending Pokémon can't attack (until it's Benched or evolves).</summary>
        private sealed class FreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Freeze") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack (Freeze)", ctx.Player, defending.Uid, int.MaxValue));
                return true;
            }
        }

        /// <summary>Fire Wind: choose 1 of the opponent's Benched Pokémon; flip 2 coins, 10 damage to it for each heads (no W/R).</summary>
        private sealed class FireWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Wind") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Fire Wind: choose 1 of your opponent's Benched Pokémon");
                int heads = ctx.FlipCoins(2);
                if (target != null && heads > 0 && ctx.InPlay(target)) ctx.DamageWith(target, 10 * heads, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Char: if the Defending Pokémon has no Char counter, flip a coin; heads, put one on it. After every turn its owner flips a
        /// coin; tails, 2 damage counters on it (GameEngine, Pokémon Checkup). It stays while the Pokémon is in play.
        /// </summary>
        private sealed class CharMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Char") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !TefPokemon.StillDefending(ctx, defending) || defending.Markers.ContainsKey(PokemonInPlay.CharMarker)) return true;
                if (!ctx.Flip() || !ctx.CanAffect(defending)) return true;
                defending.Markers[PokemonInPlay.CharMarker] = 1;
                ctx.Log(defending.Name + " has a Char counter.");
                return true;
            }
        }

        /// <summary>Fin Slap: 10 more damage if an attack damaged this Pokémon during your opponent's last turn.</summary>
        private sealed class FinSlapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fin Slap") return false;
                var me = ctx.Pokemon;
                bool hit = me != null && me.AttackDamageTurn == ctx.Turn - 1;
                ctx.DamageDefending(atk.Damage + (hit ? 10 : 0));
                return true;
            }
        }

        /// <summary>Static Electricity: for each Mareep you have in play, you may attach a Lightning Energy card from your deck to that Mareep.</summary>
        private sealed class StaticElectricityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Static Electricity") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var mareep in ctx.MyPokemon(m => m.Name == "Mareep"))
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var found = ctx.SearchMyDeck(d => BasicOf(d, EnergyType.Lightning), 1, "Static Electricity: choose a Lightning Energy card for " + mareep.Name);
                    foreach (var card in found)
                    {
                        if (ctx.InPlay(mareep)) ctx.Engine.AttachEnergyCard(card, mareep);
                        else ctx.Me.Deck.Add(card);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Bubble Bomb: heads, the Defending Pokémon is now Paralyzed; tails, this Pokémon does 10 damage to itself.</summary>
        private sealed class BubbleBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Bomb") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                {
                    if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Telekinesis: 20 damage to 1 of the opponent's Pokémon, no Weakness or Resistance.</summary>
        private sealed class TelekinesisMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Telekinesis") return false;
                ctx.DamageAnyOpponent(atk.Damage, true);
                return true;
            }
        }

        /// <summary>Roar: flip a coin; heads, the opponent switches the Defending Pokémon with 1 of their Benched Pokémon (they choose).</summary>
        private sealed class RoarFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Scary Face: heads, until the end of the opponent's next turn the Defending Pokémon can't attack or retreat.</summary>
        private sealed class ScaryFaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scary Face") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                ctx.AddEffect(new CantRetreatEffect(defending.Name + " can't retreat during its next turn (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }
    }
}
