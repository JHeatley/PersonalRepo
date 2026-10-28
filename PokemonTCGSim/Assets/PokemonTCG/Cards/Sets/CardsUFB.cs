using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unseen Forces Uncommon and Common Pokémon (UF 34-79) and the 28 Unown (UF A-Z, !, ?; v168). Unlimited only. Every Unown has
    /// Shuffle (Form Change for Unown) and its own Hidden Power.
    /// </summary>
    public static class CardsUFB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UF-34", new AipomSnappyMove());
            r.RegisterByKey("UF-35", new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep));
            r.RegisterByKey("UF-36", new ComboMon(new ConditionAttackMon("Shining Fingers", SpecialCondition.Asleep), new ExtraCometPunchMon()));
            r.RegisterByKey("UF-37", new ComboMon(new CryForHelpMon(), new CardsFST1.TwoBenchedMon("Double Attack", 10)));
            r.RegisterByKey("UF-38", new CardsFLF1.NoResistanceMon("Aqua Sonic"));
            r.RegisterByKey("UF-39", new GranbullIntimidatingFang());
            r.RegisterByKey("UF-40", new ComboMon(new CardsRGB.EnergyStreamMon(), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("UF-41", new CardsTRR.DualArmorMon(EnergyType.Fighting, new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"),
                CardsSS.ConditionBonus("Extra Flame", 30, d => d.Def.IsOldEx))));
            r.RegisterByKey("UF-42", new MiltankThickFat());
            r.RegisterByKey("UF-43", new ComboMon(new DrawAttackMon("Spearhead", 3), new CardsSS.SurpriseMon()));
            r.RegisterByKey("UF-44", new QuagsireDense());
            r.RegisterByKey("UF-45", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("UF-46", new ComboMon(new CardsSS.BenchNamedMon("Twin Play", new[] { "Scyther", "Scyther ex" }, false, 2), new DodgeMon("Agility", false)));
            r.RegisterByKey("UF-47", new ShuckleExtraTight());
            r.RegisterByKey("UF-48", new SmeargleMakeover());
            r.RegisterByKey("UF-49", new ComboMon(new TelekinesisMon(30), new ParalyzeFlipMon("Psyshock")));
            r.RegisterByKey("UF-50", new ComboMon(new GrandLoopMon(), new FlipSpinningTailMon()));
            r.RegisterByKey("UF-51", new PlainMon());
            r.RegisterByKey("UF-52", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("UF-53", new ConditionAttackMon("Lullaby", SpecialCondition.Asleep));
            r.RegisterByKey("UF-54", new ComboMon(new ParalyzeFlipMon("Paralyzing Gaze"), new CardsE2.FireworksMon()));
            r.RegisterByKey("UF-55", new EeveeEnergyEvolution());
            r.RegisterByKey("UF-56", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new RecoilMon("Thunder", 10)));
            r.RegisterByKey("UF-57", new CardsRG.EnergyRetreatMon(false, new ConditionAttackMon("Toxic Grip", SpecialCondition.Poisoned)));
            r.RegisterByKey("UF-58", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("UF-59", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("UF-60", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("UF-61", new CardsASR1.TimesMon("Retaliate", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("UF-62", new PlainMon());
            r.RegisterByKey("UF-63", new TelekinesisMon(10));
            r.RegisterByKey("UF-64", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("UF-65", new DigDeepMon());
            r.RegisterByKey("UF-66", new PlainMon());
            r.RegisterByKey("UF-67", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("UF-68", new CardsRS.ExtraEnergyMon("Energy Ball", 2));
            r.RegisterByKey("UF-69", new CoinsMon("3-D Attack", 3));
            r.RegisterByKey("UF-70", new PlainMon());
            r.RegisterByKey("UF-71", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("UF-72", new ComboMon(new FishingTailMon(), new FlipBonusMon("Trip Over", 10)));
            r.RegisterByKey("UF-73", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("UF-74", new ParalyzeFlipMon("Lick"));
            r.RegisterByKey("UF-75", new CardsCES1.FlipConditionsMon("Stun Poison", SpecialCondition.Paralyzed, SpecialCondition.Poisoned));
            r.RegisterByKey("UF-76", new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 1));
            r.RegisterByKey("UF-77", new ShieldAttackMon("Teary Eyes", 20));
            r.RegisterByKey("UF-78", new TelekinesisMon(10, "Splatter"));
            r.RegisterByKey("UF-79", new PlainMon());

            r.RegisterByKey("UF-A", Unown('A'));
            r.RegisterByKey("UF-B", Unown('B'));
            r.RegisterByKey("UF-C", Unown('C'));
            r.RegisterByKey("UF-D", Unown('D'));
            r.RegisterByKey("UF-E", Unown('E'));
            r.RegisterByKey("UF-F", Unown('F'));
            r.RegisterByKey("UF-G", Unown('G'));
            r.RegisterByKey("UF-H", Unown('H'));
            r.RegisterByKey("UF-I", Unown('I'));
            r.RegisterByKey("UF-J", Unown('J'));
            r.RegisterByKey("UF-K", Unown('K'));
            r.RegisterByKey("UF-L", Unown('L'));
            r.RegisterByKey("UF-M", Unown('M'));
            r.RegisterByKey("UF-N", Unown('N'));
            r.RegisterByKey("UF-O", Unown('O'));
            r.RegisterByKey("UF-P", Unown('P'));
            r.RegisterByKey("UF-Q", Unown('Q'));
            r.RegisterByKey("UF-R", Unown('R'));
            r.RegisterByKey("UF-S", Unown('S'));
            r.RegisterByKey("UF-T", Unown('T'));
            r.RegisterByKey("UF-U", Unown('U'));
            r.RegisterByKey("UF-V", Unown('V'));
            r.RegisterByKey("UF-W", Unown('W'));
            r.RegisterByKey("UF-X", Unown('X'));
            r.RegisterByKey("UF-Y", Unown('Y'));
            r.RegisterByKey("UF-Z", Unown('Z'));
            r.RegisterByKey("UF-!", Unown('!'));
            r.RegisterByKey("UF-?", Unown('?'));
        }

        /// <summary>An Unown: Shuffle (Form Change for Unown) and the letter's Hidden Power.</summary>
        private static Mon Unown(char letter) { return new CardsDX.FormChangeMon("Unown", new HiddenPowerMon(letter), "Shuffle"); }

        /// <summary>Snappy Move (Poké-Power): once during your turn, if Aipom is on your Bench, draw a card, then discard everything attached to Aipom and put it on the bottom of your deck; only 1 Snappy Move each turn.</summary>
        private sealed class AipomSnappyMove : Mon
        {
            private static readonly Mon Tail = new CardsSSB.AnyOpponentTextMon("Snap Tail", 10);

            protected override string Activated { get { return "Snappy Move"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                ctx.Draw(1);
                if (ctx.InPlay(me)) ctx.Engine.PutPokemonOnDeckBottom(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tail.HandleAttack(ctx, atk); }
        }

        /// <summary>Extra Comet Punch: 30 more if Clefable used Extra Comet Punch during your last turn.</summary>
        private sealed class ExtraCometPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra Comet Punch") return false;
                var me = ctx.Pokemon;
                bool again = me.LastAttackName == "Extra Comet Punch" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(atk.Damage + (again ? 30 : 0));
                return true;
            }
        }

        /// <summary>Cry for Help: a Water or Fighting Pokémon (not a Pokémon-ex) from your deck into your hand.</summary>
        private sealed class CryForHelpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cry for Help") return false;
                if (ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(d => d.IsPokemon && !d.IsOldEx && (d.HasType(EnergyType.Water) || d.HasType(EnergyType.Fighting)), 1, "Cry for Help: choose a Water or Fighting Pokémon (not a Pokémon-ex)");
                return true;
            }
        }

        /// <summary>Intimidating Fang (Poké-Body, Granbull): while Granbull is your Active Pokémon, the opponent's attacks do 10 less damage to your Pokémon (after W/R). Crushing Blow. Double Lariat.</summary>
        private sealed class GranbullIntimidatingFang : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow"), new CoinsMon("Double Lariat", 2));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Thick Fat (Poké-Body): attack damage from Fire and Water Pokémon is 30 less (after W/R). Healing Milk. Body Slam.</summary>
        private sealed class MiltankThickFat : Mon
        {
            private static readonly Mon Slam = new ParalyzeFlipMon("Body Slam");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(a, EnergyType.Fire) || src.Engine.IsType(a, EnergyType.Water) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Milk") return Slam.HandleAttack(ctx, atk);
                int heads = ctx.FlipCoins(2);
                for (int i = 0; i < heads; i++)
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count == 0) break;
                    var target = ctx.ChoosePokemon(hurt, "Healing Milk: remove 2 damage counters from which Pokémon?");
                    if (target != null) ctx.Heal(target, 20);
                }
                return true;
            }
        }

        /// <summary>Dense (Poké-Body, Quagsire): attack damage from the opponent's Evolved Pokémon is 20 less (after W/R). Mud Shot. Rock Hurl.</summary>
        private sealed class QuagsireDense : Mon
        {
            private static readonly Mon Hurl = new CardsFLF1.NoResistanceMon("Rock Hurl");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsE2.IsEvolved(a) ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hurl.HandleAttack(ctx, atk); }
        }

        /// <summary>Extra-tight (Poké-Body): no attack damage to Shuckle from the opponent's Pokémon-ex. Toxic.</summary>
        private sealed class ShuckleExtraTight : Mon
        {
            private static readonly Mon Toxic = new CardsBS1.ToxicMon("Toxic", 2);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Def.IsOldEx ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Toxic.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Makeover (Poké-Power): once during your turn, discard a basic Energy card from 1 of your Pokémon (not a Pokémon-ex); if you
        /// do, attach a different basic Energy card from your discard pile to it. Split Spiral Punch.
        /// </summary>
        private sealed class SmeargleMakeover : Mon
        {
            private const string Power = "Makeover";
            private static readonly Mon Punch = new CardsPAR2.FlipConditionMon("Split Spiral Punch", SpecialCondition.Confused);

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => !m.Def.IsOldEx && m.Energy.Any(e => e.Def.IsBasicEnergy)); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && Targets(ctx).Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var mon = ctx.ChoosePokemon(targets, Power + ": discard a basic Energy card from which Pokémon?");
                if (mon == null) return;
                var basics = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var gone = ctx.ChooseCards(basics, 1, 1, Power + ": discard which Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(mon, gone)) return;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && c != gone).ToList();
                if (pile.Count == 0 || !ctx.InPlay(mon)) return;
                var card = ctx.ChooseCards(pile, 1, 1, Power + ": attach which basic Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, mon)) ctx.Me.Discard.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>Telekinesis / Splatter: N to 1 of the opponent's Pokémon, not affected by Weakness or Resistance.</summary>
        private sealed class TelekinesisMon : Mon
        {
            private readonly int amount;
            private readonly string attack;

            public TelekinesisMon(int amount) : this(amount, "Telekinesis") { }

            public TelekinesisMon(int amount, string attack)
            {
                this.amount = amount;
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageAnyOpponent(amount, true);
                return true;
            }
        }

        /// <summary>Grand Loop: draw 2 cards; you may switch Yanma with 1 of your Benched Pokémon.</summary>
        private sealed class GrandLoopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Loop") return false;
                ctx.Draw(2);
                if (ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Grand Loop: switch Yanma with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Spinning Tail (Yanma): heads, 20 to each of the opponent's Pokémon.</summary>
        private sealed class FlipSpinningTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Tail") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(20);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>
        /// Energy Evolution (Poké-Power): whenever you attach an Energy card from your hand to Eevee, you may evolve it into a card from
        /// your deck that evolves from Eevee and is that Energy's type. Tail Whap.
        /// </summary>
        private sealed class EeveeEnergyEvolution : Mon
        {
            public override bool EvolvesOnOwnerAttachFromHand { get { return true; } }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var mon = src.Pokemon;
                if (mon == null || target != mon || energy == null || !src.IsMyTurn || !src.InPlay(mon)) return;
                if (src.Engine.CurrentSourceKind == EffectSourceKind.Attack) return;
                var type = energy.Def.EnergyProvides;
                if (src.Me.Deck.Count == 0 || CardsEVS1.EvolveBlocked(src, mon)) return;
                if (!src.AskYesNo("Energy Evolution: search your deck for a " + type + " card that evolves from " + mon.Name + " and evolve it?")) return;
                Announce(src, "Energy Evolution");
                var found = src.SearchMyDeck(d => d.HasType(type) && CardsEVS1.EvolvesOnto(src, mon, d), 1, "Energy Evolution: choose a " + type + " card that evolves from " + mon.Name, false);
                if (found.Count > 0) src.Engine.EvolvePokemon(mon, found[0]);
                src.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>Dig Deep: an Energy card from your discard pile into your hand.</summary>
        private sealed class DigDeepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Deep") return false;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Dig Deep: put which Energy card into your hand?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Hand.Add(card);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "put this card from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (Dig Deep)");
                return true;
            }
        }

        /// <summary>Fishing Tail: a Basic Pokémon, Evolution card or basic Energy card from your discard pile into your hand.</summary>
        private sealed class FishingTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fishing Tail") return false;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Fishing Tail: put which card into your hand?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Hand.Add(card);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "put this card from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (Fishing Tail)");
                return true;
            }
        }

        // ================================================================== Unown

        /// <summary>Each Unown's Hidden Power (the letter picks the effect).</summary>
        private sealed class HiddenPowerMon : Mon
        {
            private readonly char letter;

            public HiddenPowerMon(char letter) { this.letter = letter; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                switch (letter)
                {
                    case 'A':
                        if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsSupporter, 2, "Hidden Power: choose up to 2 Supporter cards");
                        return true;
                    case 'B':
                        if (defending != null && ctx.Opp.Hand.Count > 0) ctx.PlaceCounters(defending, ctx.Opp.Hand.Count);
                        return true;
                    case 'C':
                        return OpponentsSupporter(ctx);
                    case 'D':
                        {
                            if (!ctx.Flip()) return true;
                            var evolved = ctx.MyPokemon(CardsE2.IsEvolved).Concat(ctx.OpponentPokemon(m => CardsE2.IsEvolved(m) && ctx.CanAffect(m))).ToList();
                            if (evolved.Count == 0) return true;
                            var target = ctx.ChoosePokemon(evolved, "Hidden Power: devolve which Evolved Pokémon?");
                            if (target != null) ctx.Engine.Devolve(target, 1);
                            return true;
                        }
                    case 'E':
                        {
                            if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                            var hand = ctx.Opp.Hand.ToList();
                            ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Hidden Power: your opponent's hand", ctx.Source);
                            var basics = hand.Where(c => c.Def.IsBasicPokemon).ToList();
                            if (basics.Count == 0) return true;
                            var card = ctx.ChooseCards(basics, 1, 1, "Hidden Power: put which Basic Pokémon onto your opponent's Bench?")[0];
                            if (!ctx.Opp.Hand.Remove(card)) return true;
                            var benched = ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
                            if (benched != null && ctx.Opp.Active != null && ctx.CanAffect(ctx.Opp.Active)) ctx.Engine.SwitchActive(ctx.Opponent, benched);
                            return true;
                        }
                    case 'F':
                        {
                            int heads = ctx.FlipCoins(3);
                            ctx.DamageDefending(atk.Damage);
                            if (heads == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                            ctx.ApplyCondition(defending, heads == 1 ? SpecialCondition.Asleep : heads == 2 ? SpecialCondition.Confused : SpecialCondition.Paralyzed);
                            return true;
                        }
                    case 'G':
                        {
                            int n = ctx.MyPokemon(m => m.DamageCounters > 0).Count;
                            if (n > 0 && defending != null) ctx.PlaceCounters(defending, n);
                            return true;
                        }
                    case 'H':
                        return HoardTools(ctx);
                    case 'I':
                        {
                            if (ctx.GustTargets().Count == 0) return true;
                            var target = ctx.GustOpponent("Hidden Power: choose 1 of your opponent's Benched Pokémon to switch in");
                            if (target != null && ctx.Opp.Active == target)
                            {
                                ctx.ApplyCondition(target, SpecialCondition.Burned);
                                ctx.ApplyCondition(target, SpecialCondition.Confused);
                            }
                            return true;
                        }
                    case 'J':
                        {
                            if (!ctx.Flip() || defending == null) return true;
                            if (!ctx.CanAffect(defending)) return Nothing(ctx);
                            int mine = me.DamageCounters;
                            me.DamageCounters = defending.DamageCounters;
                            defending.DamageCounters = mine;
                            ctx.Log("Hidden Power: " + me.Name + " and " + defending.Name + " switched their damage counters.");
                            return true;
                        }
                    case 'K':
                        ctx.DamageDefending(atk.Damage);
                        if (ctx.State.Stadium != null && ctx.AskYesNo("Hidden Power: discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                        return true;
                    case 'L':
                        {
                            if (!ctx.Flip() || defending == null) return true;
                            int n = (ctx.RemainingHp(defending) - 10) / 10;
                            if (n > 0) ctx.PlaceCounters(defending, n);
                            return true;
                        }
                    case 'M':
                        {
                            if (ctx.Me.Deck.Count == 0) return true;
                            var mine = ctx.MyPokemon();
                            var found = ctx.SearchMyDeck(d => d.IsPokemon && mine.Any(m => d.EvolvesFrom == m.Name), 1, "Hidden Power: choose a card that evolves from 1 of your Pokémon", false);
                            bool evolved = false;
                            if (found.Count > 0)
                            {
                                var card = found[0];
                                var targets = ctx.MyPokemon(m => card.Def.EvolvesFrom == m.Name);
                                var target = ctx.ChoosePokemon(targets, "Hidden Power: evolve which Pokémon?");
                                if (target != null)
                                {
                                    ctx.Engine.EvolvePokemon(target, card);
                                    evolved = true;
                                }
                                else ctx.Me.Deck.Add(card);
                            }
                            ctx.ShuffleMyDeck();
                            if (evolved && ctx.InPlay(me)) ctx.PlaceCounters(me, 1);
                            return true;
                        }
                    case 'N':
                        if (me.DamageCounters > 0) ctx.Heal(me, 50);
                        return true;
                    case 'O':
                        ctx.DamageDefending(atk.Damage);
                        if (ctx.InPlay(me)) ctx.Engine.ReturnPokemonToHand(me);
                        return true;
                    case 'P':
                        foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0))
                            if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 1);
                        return true;
                    case 'Q':
                        return Quick.HandleAttack(ctx, atk);
                    case 'R':
                        {
                            if (defending == null || ctx.Opp.Bench.Count == 0) return Nothing(ctx);
                            if (ctx.Flip() && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                            return true;
                        }
                    case 'S':
                        return SteelPull(ctx);
                    case 'T':
                        ctx.DamageAnyOpponent(20, true);
                        return true;
                    case 'U':
                        return Amnesia.HandleAttack(ctx, atk);
                    case 'V':
                        ctx.DamageDefending(atk.Damage + Math.Min(60, 10 * ctx.Me.Discard.Count(c => c.Def.IsPokemon)));
                        return true;
                    case 'W':
                        {
                            ctx.DamageDefending(atk.Damage);
                            if (!ctx.Flip() || ctx.Me.Discard.Count == 0) return true;
                            var pile = ctx.Me.Discard.ToList();
                            var card = ctx.ChooseCards(pile, 1, 1, "Hidden Power: put which card on top of your deck?")[0];
                            if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Insert(0, card);
                            ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "put this card from " + ctx.Engine.Their(ctx.Player) + " discard pile on top of " + ctx.Engine.Their(ctx.Player) + " deck (Hidden Power)");
                            return true;
                        }
                    case 'X':
                        ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                        return true;
                    case 'Y':
                        ctx.DamageDefending(atk.Damage);
                        if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                        return true;
                    case 'Z':
                        for (int side = 0; side < 2; side++)
                            foreach (var mon in ctx.State.Players[side].InPlay().ToList())
                                if (ctx.InPlay(mon) && mon.Def.Abilities.Any(a => a.IsPokePowerOrBody)) ctx.DamageWith(mon, 20, false, false, false);
                        return true;
                    case '!':
                        {
                            bool heads = ctx.Flip();
                            var all = heads ? ctx.OpponentPokemon() : ctx.MyPokemon();
                            if (all.Count == 0) return true;
                            var target = ctx.ChoosePokemon(all, "Hidden Power: put 2 damage counters on which Pokémon?");
                            if (target != null) ctx.PlaceCounters(target, 2);
                            return true;
                        }
                    case '?':
                        return GuessingGame(ctx);
                }
                return false;
            }

            private static readonly Mon Amnesia = new CardsTEU1.AmnesiaMon("Hidden Power");
            private static readonly Mon Quick = new CardsDR.DifferentEnergyMon("Hidden Power", 3);

            /// <summary>Unown C: use the effect of a Supporter card in the opponent's discard pile (it stays there).</summary>
            private static bool OpponentsSupporter(EffectContext ctx)
            {
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in ctx.Opp.Discard)
                    if (card.Def.IsSupporter && seen.Add(card.Def.Name) && MrMimeTEF.WouldDoSomething(ctx, card)) usable.Add(card);
                if (usable.Count == 0) return true;
                var labels = usable.Select(x => "Use " + x.Def.Name + "'s effect").ToArray();
                int pick = usable.Count == 1 ? 0 : ctx.ChooseOption("Hidden Power: use the effect of which Supporter from your opponent's discard pile?", labels);
                if (pick < 0 || pick >= usable.Count) return true;
                var chosen = usable[pick];
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + ".");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
                return true;
            }

            /// <summary>Unown H: up to 2 Pokémon Tools from your deck onto your Pokémon that have none.</summary>
            private static bool HoardTools(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var tool in ctx.SearchMyDeck(d => d.IsTool, 2, "Hidden Power: choose up to 2 Pokémon Tool cards"))
                {
                    var spots = ctx.MyPokemon(m => m.Tools.Count == 0 && ctx.Engine.CanAttachToolTo(tool, m));
                    var target = spots.Count == 0 ? null : ctx.ChoosePokemon(spots, "Hidden Power: attach " + tool.Def.Name + " to which Pokémon?");
                    if (target != null) ctx.Engine.AttachTool(tool, target);
                    else ctx.Me.Deck.Add(tool);
                }
                ctx.ShuffleMyDeck();
                return true;
            }

            /// <summary>Unown S: if the Defending Pokémon has a Poké-Power or Poké-Body, up to 2 basic Energy cards from 1 of the opponent's Pokémon go onto it.</summary>
            private static bool SteelPull(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (defending == null || !defending.Def.Abilities.Any(a => a.IsPokePowerOrBody)) return true;
                var donors = ctx.OpponentPokemon(m => m != defending && m.Energy.Any(e => e.Def.IsBasicEnergy));
                if (donors.Count == 0) return true;
                var from = ctx.ChoosePokemon(donors, "Hidden Power: move basic Energy cards from which of your opponent's Pokémon?");
                if (from == null || !ctx.CanAffect(from)) return true;
                var basics = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                foreach (var card in ctx.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Hidden Power: move up to 2 basic Energy cards to " + defending.Name))
                    ctx.Engine.MoveEnergy(from, defending, card);
                return true;
            }

            /// <summary>Unown ?: a card from your hand face down; the opponent guesses Pokémon, Trainer or Energy; a wrong guess means you draw 2.</summary>
            private static bool GuessingGame(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var card = ctx.ChooseCards(hand, 1, 1, "Hidden Power: choose a card from your hand to put face down")[0];
                var options = new List<DecisionOption>
                {
                    new DecisionOption { Label = "Pokémon", Index = 0 },
                    new DecisionOption { Label = "Trainer", Index = 1 },
                    new DecisionOption { Label = "Energy", Index = 2 }
                };
                int guess = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Hidden Power: is the face-down card a Pokémon, Trainer or Energy card?", options, ctx.Source, false);
                int actual = card.Def.IsPokemon ? 0 : card.Def.IsTrainer ? 1 : 2;
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "revealed the face-down card (Hidden Power)");
                if (guess != actual) ctx.Draw(2);
                return true;
            }
        }
    }
}
