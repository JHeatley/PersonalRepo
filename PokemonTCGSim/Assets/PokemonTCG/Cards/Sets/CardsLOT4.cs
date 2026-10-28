using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Thunder (LOT, v81), part 4: Fighting, Darkness, Metal, Fairy and Colorless (LOT 109-166). Your rulings used (v81):
    /// Smeargle's Stunning Likeness can use any Supporter's effect, even one whose play condition isn't met; Kecleon's Unit Color
    /// makes it only the 3 listed types. Pokémon Tools are not Item cards, so "Item cards" never includes them (your ruling, v90).
    /// </summary>
    public static class CardsLOT4
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("LOT-110", new CardsFST2.ConditionBonusMon("Reply Strongly", 80, c => c.Pokemon != null && c.Pokemon.ActiveAttackDamageTurn == c.OpponentsLastTurn));
            r.RegisterByKey("LOT-111", new TailsNothingMon("Last Resort"));
            r.RegisterByKey("LOT-112", new DonphanLOT());
            r.RegisterByKey("LOT-113", new ComboMon(new RapidSpinMon(), new CoinsMon("Triple Kick", 3)));
            r.RegisterByKey("LOT-114", new CardsDRM1.SubmergeMon());
            r.RegisterByKey("LOT-115", new CardsFST2.ConditionBonusMon("Second Strike", 70, c => c.Defending != null && c.Defending.DamageCounters >= 3));
            r.RegisterByKey("LOT-116", new PupitarLOT());
            r.RegisterByKey("LOT-117", new ComboMon(new DiamondGateMon(), new ShieldAttackMon("Guard Press", 20)));
            // Darkness
            r.RegisterByKey("LOT-118", new CardsFST2.ConditionBonusMon("Spoil the Fun", 60, c => CardsUNB1.SecondPlayersFirstTurn(c)));
            r.RegisterByKey("LOT-119", new EmptyThreatMon());
            r.RegisterByKey("LOT-120", new Cards30C.RetaliateBonusMon("Retaliate", 90));
            r.RegisterByKey("LOT-121", new TyranitarGX());
            // Metal
            r.RegisterByKey("LOT-122", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("LOT-123", new CardsLOT1.PlainDamageMon("Digging Dash", false));
            r.RegisterByKey("LOT-124", new ForretressLOT());
            r.RegisterByKey("LOT-125", new RecoilMon("Iron Tackle", 50));
            r.RegisterByKey("LOT-126", new ScizorLOT());
            r.RegisterByKey("LOT-127", new ComboMon(new TurnBackTimeMon(), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            r.RegisterByKey("LOT-128", new ComboMon(new KnockOverMon(), new MountainMunchMon()));
            r.RegisterByKey("LOT-129", new ComboMon(new ShieldAttackMon("Guard Press", 30), new CardsFST2.ConditionBonusMon("Metal Arms", 40, c => c.Pokemon != null && c.Pokemon.Tools.Count > 0)));
            r.RegisterByKey("LOT-130", new GenesectGX());
            r.RegisterByKey("LOT-131", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2), new CardsASR1.PlusMon("Energy Press", 20, CardsLOT2.DefendingEnergy)));
            // Fairy
            r.RegisterByKey("LOT-132", new AlolanNinetalesGX());
            r.RegisterByKey("LOT-133", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("LOT-134", new ComboMon(new ShieldAttackMon("Expand", 30), new CardsFST2.ConditionBonusMon("Charmed Slap", 70, c => CardsLOT1.HasFairyCharm(c.Pokemon))));
            r.RegisterByKey("LOT-136", new ComboMon(new PolkaDotSearchMon(), new FlipBonusMon("Play Rough", 30)));
            r.RegisterByKey("LOT-137", new CardsFST1.DiscardFromHandTimesMon("Make a Mess", d => d.IsTrainer, 2, "Trainer cards"));
            r.RegisterByKey("LOT-138", new CardsFST2.ConditionBonusMon("All Out", 130, c => c.Me.Hand.Count == 0));
            r.RegisterByKey("LOT-139", new BeckonMon());
            r.RegisterByKey("LOT-140", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("LOT-141", new ComboMon(new BrilliantSearchMon(), new CardsFST2.ConditionBonusMon("Sensitive Ray", 90, CardsLOT1.SupporterPlayed)));
            r.RegisterByKey("LOT-142", new ZzzapTouchMon());
            r.RegisterByKey("LOT-143", new WonderRayMon());
            r.RegisterByKey("LOT-144", new XerneasPrismStar());
            r.RegisterByKey("LOT-145", new CardsPOR.SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("LOT-146", new RibombeeLOT());
            r.RegisterByKey("LOT-147", new ConditionAttackMon("Perplex", SpecialCondition.Confused));
            r.RegisterByKey("LOT-148", new CardsCRI.CounterConditionMon("Effect Spore", SpecialCondition.Asleep, new DreamsTouchMon()));
            r.RegisterByKey("LOT-149", new MimikyuGX());
            r.RegisterByKey("LOT-150", new TapuLeleLOT());
            r.RegisterByKey("LOT-151", new ComboMon(new DreamAwayMon(), new ConditionAttackMon("Wonder Shine", SpecialCondition.Confused)));
            // Colorless
            r.RegisterByKey("LOT-152", new ComboMon(new HealingPirouetteMon(), new SympatheticSlapMon()));
            r.RegisterByKey("LOT-153", new BlisseyLOT());
            r.RegisterByKey("LOT-154", new DittoPrismStar());
            r.RegisterByKey("LOT-156", new ComboMon(new ConditionAttackMon("Mystifying Horns", SpecialCondition.Confused), new CardsFST2.ConditionBonusMon("Enhanced Horns", 60, c => c.Pokemon != null && c.Pokemon.Tools.Count > 0)));
            r.RegisterByKey("LOT-157", new SmeargleLOT());
            r.RegisterByKey("LOT-158", new MilkCannonMon());
            r.RegisterByKey("LOT-159", new LugiaGX());
            r.RegisterByKey("LOT-160", new CardsASR1.PlusMon("Rainbow Burn", 30, c => c.Pokemon == null ? 0 : c.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count()));
            r.RegisterByKey("LOT-161", new KecleonLOT("Unit Energy GRW", EnergyType.Grass, EnergyType.Fire, EnergyType.Water));
            r.RegisterByKey("LOT-162", new KecleonLOT("Unit Energy FDY", EnergyType.Fighting, EnergyType.Darkness, EnergyType.Fairy));
            r.RegisterByKey("LOT-163", new PeckOffMon("Peck Off"));
            r.RegisterByKey("LOT-164", new CardsDRI1.WhirlwindMon("Send Back"));
            r.RegisterByKey("LOT-165", new TrumbeakLOT());
            r.RegisterByKey("LOT-166", new ComboMon(new ConditionAttackMon("Heat Beak", SpecialCondition.Burned), new CardsLOT1.DoesNothingUnlessMon("Giganticannon", c => c.Pokemon != null && c.Pokemon.TurnEvolved != c.Turn)));
        }

        /// <summary>"Item cards": Pokémon Tools are not Item cards (your ruling, v90).</summary>
        internal static bool IsItemCard(CardDef d) { return d.IsItem; }

        // ================================================================== Fighting

        /// <summary>Sturdy: at full HP, it survives a Knock Out from attack damage with 10 HP. Rolling Spin: 70 more during your next turn.</summary>
        private sealed class DonphanLOT : Mon
        {
            private static readonly Mon Spin = new CardsASC.NextTurnBoostMon("Rolling Spin", 70);

            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spin.HandleAttack(ctx, atk); }
        }

        /// <summary>Rapid Spin: switch this Pokémon with 1 of your Benched Pokémon; if you do, the opponent switches their Active Pokémon with 1 of their Benched Pokémon.</summary>
        internal sealed class RapidSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || !ctx.SwitchSelfWithBench()) return true;
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>
        /// Hard Shell Evolution: when played from your hand to evolve during your turn, you may prevent all damage done to this
        /// Pokémon by the opponent's attacks until the end of their next turn.
        /// </summary>
        private sealed class PupitarLOT : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Hard Shell Evolution", true, "Hard Shell Evolution: prevent all damage done to " + ctx.Pokemon.Name + " by your opponent's attacks until the end of their next turn?")) return;
                ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks until the end of the opponent's next turn (Hard Shell Evolution)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
            }
        }

        /// <summary>Diamond Gate: search your deck for a Supporter card and a Stadium card, reveal them and put them into your hand.</summary>
        private sealed class DiamondGateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Gate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsSupporter, 1, "Diamond Gate: choose a Supporter card");
                found.AddRange(ctx.SearchMyDeck(d => d.IsStadiumCard, 1, "Diamond Gate: choose a Stadium card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Empty Threat: 30 less damage for each Energy attached to the opponent's Active Pokémon.</summary>
        private sealed class EmptyThreatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Empty Threat") return false;
                int damage = Math.Max(0, atk.Damage - 30 * CardsLOT2.DefendingEnergy(ctx));
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log("Empty Threat did no damage.");
                return true;
            }
        }

        /// <summary>
        /// Lost Out: the opponent's Pokémon Knocked Out by damage from this Pokémon's attacks go to the Lost Zone with all attached cards.
        /// Dusty Ruckus: 30 damage to each of the opponent's Benched Basic Pokémon. Lay the Smackdown-GX: not affected by effects on the
        /// opponent's Active Pokémon.
        /// </summary>
        private sealed class TyranitarGX : Mon
        {
            private static readonly Mon Smackdown = new CardsPAF2.IgnoreDefenderEffectsMon("Lay the Smackdown-GX");

            public override int LostZoneOnKnockOut(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || src.State.InCheckup) return 0;
                return mon.HitByAttack && !mon.ForcedKnockOut && mon.HitByAttackerUid == me.Uid ? 2 : 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Smackdown.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Dusty Ruckus") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench())
                    if (mon.IsBasic) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>
        /// Thorny Eruption: flip 3 coins; 10 damage for each heads to each of the opponent's Pokémon. Lost Blast: put this Pokémon and
        /// all attached cards in the Lost Zone.
        /// </summary>
        private sealed class ForretressLOT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thorny Eruption")
                {
                    int heads = ctx.FlipCoins(3);
                    if (heads == 0)
                    {
                        ctx.Log("Thorny Eruption did no damage.");
                        return true;
                    }
                    foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 10 * heads);
                    return true;
                }
                if (atk.Name == "Lost Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) CardsLOR1.PokemonToLostZone(ctx, ctx.Pokemon, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Exoskeleton: this Pokémon takes 30 less damage from attacks (after Weakness and Resistance). Special Blow: 70 more against a Special Energy.</summary>
        private sealed class ScizorLOT : Mon
        {
            private static readonly Mon Blow = new CardsFST2.ConditionBonusMon("Special Blow", 70, c => c.Defending != null && c.Defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>Turn Back Time: if the opponent's Active Pokémon is evolved, devolve it; its highest Stage Evolution card goes to their hand.</summary>
        private sealed class TurnBackTimeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turn Back Time") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending) && defending.Stack.Count > 1 && !defending.Def.IsMultiCard)
                    ctx.Engine.Devolve(defending, 1);
                return true;
            }
        }

        /// <summary>Knock Over: you may discard a Stadium card in play.</summary>
        internal sealed class KnockOverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Knock Over: discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Mountain Munch: discard the top 2 cards of your opponent's deck.</summary>
        private sealed class MountainMunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Munch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                if (ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Log("The top cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck can't be discarded right now.");
                else ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                return true;
            }
        }

        /// <summary>Double Drive: this Pokémon can have up to 2 Pokémon Tools attached. Break Buster-GX: not affected by Resistance.</summary>
        private sealed class GenesectGX : Mon
        {
            private static readonly Mon Buster = new CardsLOT1.PlainDamageMon("Break Buster-GX", true);

            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(max, 2) : max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Buster.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fairy

        /// <summary>
        /// Mysterious Guidance: when played from your hand to evolve during your turn, you may search your deck for up to 2 Item cards
        /// and put them into your hand. Snowy Wind: 30 to 1 Benched Pokémon. Sublimation-GX: an Ultra Beast Active Pokémon is Knocked Out.
        /// </summary>
        private sealed class AlolanNinetalesGX : Mon
        {
            private static readonly Mon Wind = new Cards30CC.BenchHitMon("Snowy Wind", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Mysterious Guidance", ctx.Me.Deck.Count > 0, "Mysterious Guidance: search your deck for up to 2 Item cards?")) return;
                ctx.SearchToHand(IsItemCard, 2, "Mysterious Guidance: choose up to 2 Item cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Wind.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Sublimation-GX") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending) && defending.Def.IsUltraBeast && ctx.CanAffect(defending))
                    ctx.Engine.KnockOut(defending, atk.Name);
                else ctx.Log("Sublimation-GX did nothing: the opponent's Active Pokémon isn't an Ultra Beast.");
                return true;
            }
        }

        /// <summary>
        /// Polka-Dot Search: look at the top 8 cards of your deck and attach any number of Energy cards there to your Pokémon in any
        /// way you like; shuffle the other cards back into your deck.
        /// </summary>
        private sealed class PolkaDotSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Polka-Dot Search") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.LookAtTop(8);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) ctx.Engine.ShowCards(ctx.Player, top, "Polka-Dot Search", "The top " + top.Count + " cards of your deck (no Energy)", ctx.Source);
                else ctx.AttachCardsAnyWay(energy, energy.Count, (c, m) => true, "Polka-Dot Search: attach Energy cards to your Pokémon");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Beckon: put a Supporter card from your discard pile into your hand. Also Ferry (HSP Lapras, v135: the attack name is passed in).</summary>
        internal sealed class BeckonMon : Mon
        {
            private readonly string attack;

            public BeckonMon() : this("Beckon") { }

            public BeckonMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                var pick = supporters.Count == 1 ? supporters[0] : ctx.ChooseCards(supporters, 1, 1, attack + ": put which Supporter card into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
                return true;
            }
        }

        /// <summary>Brilliant Search: search your deck for up to 3 cards and put them into your hand.</summary>
        internal sealed class BrilliantSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brilliant Search") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 3, "Brilliant Search: choose up to 3 cards", false);
                return true;
            }
        }

        /// <summary>Zzzap Touch: flip a coin; heads: Confused, tails: Paralyzed.</summary>
        private sealed class ZzzapTouchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zzzap Touch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Confused : SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Wonder Ray: during the opponent's next turn, prevent all effects of attacks, damage included, done to this Pokémon by Pokémon with an Ability.</summary>
        private sealed class WonderRayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wonder Ray") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new WonderRayEffect(ctx.Pokemon.Name + " is protected from attacks by Pokémon with an Ability (Wonder Ray)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class WonderRayEffect : ContinuousEffect
        {
            public WonderRayEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && engine.HasAbility(info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target == null || target.Uid != TargetUid || kind != EffectSourceKind.Attack) return false;
                var attacker = engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != target.Owner && engine.HasAbility(attacker);
            }
        }

        /// <summary>
        /// Path of Life: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may move any number of
        /// Energy from your other Pokémon to it. Bright Horns: can't use Bright Horns next turn.
        /// </summary>
        private sealed class XerneasPrismStar : Mon
        {
            private static readonly Mon Horns = new LockAfterAttackMon("Bright Horns", true);

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null) return;
                var others = ctx.MyPokemon(m => m != me && m.Energy.Count > 0);
                if (!TriggerOnce(ctx, "Path of Life", others.Count > 0, "Path of Life: move Energy from your other Pokémon to " + me.Name + "?")) return;
                var holders = new Dictionary<int, PokemonInPlay>();
                var energy = new List<CardInstance>();
                foreach (var mon in others)
                    foreach (var card in mon.Energy)
                    {
                        energy.Add(card);
                        holders[card.Id] = mon;
                    }
                var picks = ctx.ChooseCards(energy, 0, energy.Count, "Path of Life: choose any number of Energy to move to " + me.Name);
                foreach (var card in picks)
                {
                    PokemonInPlay holder;
                    if (holders.TryGetValue(card.Id, out holder) && holder.Energy.Contains(card) && ctx.InPlay(me)) ctx.Engine.MoveEnergy(holder, me, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Horns.HandleAttack(ctx, atk); }
        }

        /// <summary>Mysterious Buzz: while this Pokémon is on your Bench, the opponent's Supporter cards don't affect your Fairy Pokémon in play.</summary>
        private sealed class RibombeeLOT : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                var me = src.Pokemon;
                if (me == null || src.IsActive(me) || trainer == null || !trainer.IsSupporter) return false;
                if (target == null || target.Owner != src.Player) return false;
                return src.Engine.IsType(target, EnergyType.Fairy);
            }
        }

        /// <summary>Dream's Touch: if the opponent's Active Pokémon is Asleep, they shuffle all Energy attached to it into their deck.</summary>
        private sealed class DreamsTouchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream's Touch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !defending.Has(SpecialCondition.Asleep) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                ctx.Engine.ShuffleEnergyIntoDeck(defending, defending.Energy.ToList());
                return true;
            }
        }

        /// <summary>
        /// Perplex: Confused. Let's Snuggle & Fall: 30 more for each damage counter on the opponent's Active Pokémon. Dream Fear-GX:
        /// the opponent shuffles 1 of their Benched Pokémon (you choose) and its cards into their deck.
        /// </summary>
        private sealed class MimikyuGX : Mon
        {
            private static readonly Mon Perplex = new ConditionAttackMon("Perplex", SpecialCondition.Confused);
            private static readonly Mon Snuggle = new CardsASR1.PlusMon("Let's Snuggle & Fall", 30, c => c.Defending == null ? 0 : c.Defending.DamageCounters);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Perplex.HandleAttack(ctx, atk) || Snuggle.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Dream Fear-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.GustTargets();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Dream Fear-GX: choose 1 of your opponent's Benched Pokémon to shuffle into their deck");
                if (target != null && ctx.InPlay(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Charmed Charm: when you attach a Pokémon Tool with "Fairy Charm" in its name from your hand to this Pokémon during your turn, you may make the opponent's Active Pokémon Confused.</summary>
        private sealed class TapuLeleLOT : Mon
        {
            public override void OnToolAttachedFromHand(EffectContext src, CardInstance tool, PokemonInPlay holder)
            {
                if (src.Pokemon == null || holder != src.Pokemon || tool == null || !src.IsMyTurn) return;
                if (tool.Def.Name.IndexOf("Fairy Charm", StringComparison.Ordinal) < 0 || src.Opp.Active == null) return;
                if (!src.AskYesNo("Charmed Charm: make your opponent's Active Pokémon Confused?")) return;
                Announce(src, "Charmed Charm");
                src.ApplyCondition(src.Opp.Active, SpecialCondition.Confused);
            }
        }

        /// <summary>Dream Away: flip a coin; on heads, the opponent shuffles their Active Pokémon and all attached cards into their deck.</summary>
        private sealed class DreamAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Away") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect on " + defending.Name + " was prevented.");
                    return true;
                }
                ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Healing Pirouette: heal 20 damage from each of your Pokémon.</summary>
        private sealed class HealingPirouetteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Pirouette") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 20);
                return true;
            }
        }

        /// <summary>Sympathetic Slap: if the opponent's Active Pokémon already has damage counters on it, this attack does nothing.</summary>
        private sealed class SympatheticSlapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sympathetic Slap") return false;
                if (ctx.Defending != null && ctx.Defending.DamageCounters > 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Happiness Supplement: once during your turn, remove a Special Condition from your Active Pokémon. Powerful Slap: flip per Energy, 80 each heads.</summary>
        private sealed class BlisseyLOT : Mon
        {
            private static readonly Mon Slap = new CardsASC.FlipPerEnergyMon("Powerful Slap");

            private static readonly SpecialCondition[] All =
            {
                SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned
            };

            protected override string Activated { get { return "Happiness Supplement"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && active.Conditions != SpecialCondition.None;
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var present = All.Where(c => active.Has(c)).ToList();
                if (present.Count == 0) return;
                var pick = present.Count == 1 ? present[0] : present[ctx.ChooseOption("Happiness Supplement: remove which Special Condition from " + active.Name + "?", present.Select(c => c.ToString()).ToArray())];
                ctx.Engine.RemoveCondition(active, pick);
                ctx.Log(active.Name + " is no longer " + pick + ".");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slap.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Almighty Evolution: once during your turn, you may put any Stage 1 card from your hand onto this Pokémon to evolve it. Not
        /// during your first turn or the turn this Pokémon was put into play.
        /// </summary>
        private sealed class DittoPrismStar : Mon
        {
            protected override string Activated { get { return "Almighty Evolution"; } }

            private static List<CardInstance> Stage1(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Stage == Stage.Stage1 && !c.Def.IsLevelUp && !c.Def.IsBreak
                                              && !ctx.Engine.Scripts.For(c.Def).PutIntoPlayOnlyByEffect && !ctx.Engine.PokemonPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.State.IsPlayersFirstTurn || me.TurnEnteredPlay >= ctx.Turn) return false;
                if (me.CantEvolveOnTurn == ctx.Turn || ctx.Engine.EvolveLocked(ctx.Player) || ctx.Engine.EvolutionFromHandBlocked(me)) return false;
                return Stage1(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Stage1(ctx);
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, "Almighty Evolution: choose a Stage 1 card to put onto " + ctx.Pokemon.Name)[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(ctx.Pokemon, card, true);
            }
        }

        /// <summary>
        /// Stunning Likeness: the opponent reveals their hand; you may use the effect of a Supporter card there as this attack's effect
        /// (any Supporter, even one whose play condition isn't met: your ruling, v81). Tail Smash: tails, nothing.
        /// </summary>
        private sealed class SmeargleLOT : Mon
        {
            private static readonly Mon Smash = new TailsNothingMon("Tail Smash");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Smash.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Stunning Likeness") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in ctx.Opp.Hand)
                    if (card.Def.IsSupporter && seen.Add(card.Def.Name)) usable.Add(card);
                if (usable.Count == 0) return true;
                var labels = usable.Select(x => "Use " + x.Def.Name + "'s effect").ToList();
                labels.Add("Don't use a Supporter's effect");
                int pick = ctx.ChooseOption("Stunning Likeness: use the effect of a Supporter from your opponent's hand?", labels.ToArray());
                if (pick < 0 || pick >= usable.Count) return true;
                var chosen = usable[pick];
                ctx.Log("Stunning Likeness: " + ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + ".");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
                return true;
            }
        }

        /// <summary>Milk Cannon: reveal any number of Moomoo Milk cards from your hand; 60 damage for each.</summary>
        private sealed class MilkCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Milk Cannon") return false;
                var milk = ctx.Me.Hand.Where(c => c.Def.Name == "Moomoo Milk").ToList();
                int n = 0;
                if (milk.Count > 0)
                {
                    var picks = ctx.ChooseCards(milk, 0, milk.Count, "Milk Cannon: reveal any number of Moomoo Milk cards from your hand (" + atk.Damage + " damage each)");
                    n = picks.Count;
                    if (n > 0) ctx.Engine.Reveal(ctx.Player, picks, "revealed " + n + " Moomoo Milk (Milk Cannon)");
                }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Milk Cannon did no damage.");
                return true;
            }
        }

        /// <summary>
        /// Psychic: 30 more for each Energy attached to the opponent's Active Pokémon. Pelagic Blade: can't use Pelagic Blade next
        /// turn. Lost Purge-GX: the opponent's Active Pokémon and all attached cards go to the Lost Zone.
        /// </summary>
        private sealed class LugiaGX : Mon
        {
            private static readonly Mon Psychic = new CardsASR1.PlusMon("Psychic", 30, CardsLOT2.DefendingEnergy);
            private static readonly Mon Blade = new LockAfterAttackMon("Pelagic Blade", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Psychic.HandleAttack(ctx, atk) || Blade.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Lost Purge-GX") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect on " + defending.Name + " was prevented.");
                    return true;
                }
                CardsLOR1.PokemonToLostZone(ctx, defending, true);
                return true;
            }
        }

        /// <summary>Unit Color: with the named Unit Energy attached, Kecleon is only the 3 listed types (your ruling, v81). Tongue Smack: 50 more against an Evolution Pokémon.</summary>
        private sealed class KecleonLOT : Mon
        {
            private static readonly Mon Smack = new CardsPAL1.EvolutionBonusMon("Tongue Smack", 50);
            private readonly string unit;
            private readonly EnergyType[] colors;

            public KecleonLOT(string unit, params EnergyType[] colors)
            {
                this.unit = unit;
                this.colors = colors;
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!mon.Energy.Any(e => e.Def.Name == unit)) return;
                types.Clear();
                types.AddRange(colors);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smack.HandleAttack(ctx, atk); }
        }

        /// <summary>Peck Off: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon. Used for Peck Off, Obsidian Fang (the attack name is passed in).</summary>
        internal sealed class PeckOffMon : Mon
        {
            private readonly string attack;

            public PeckOffMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending)) ctx.Engine.DiscardAllTools(defending);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Mountain Pass (from the hand): once during your turn, show this card; look at the top card of the opponent's deck and put
        /// this Pokémon in the Lost Zone; if that card is a Supporter, you may put it in the Lost Zone. Not if their deck is empty.
        /// </summary>
        private sealed class TrumbeakLOT : Mon
        {
            private const string Pass = "Mountain Pass";

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && ctx.Opp.Deck.Count > 0 && !ctx.NamedLimitUsedThisTurn(Pass + "#" + ctx.Card.Id);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Contains(ctx.Card) || ctx.Opp.Deck.Count == 0) return;
                ctx.MarkNamedLimit(Pass + "#" + ctx.Card.Id);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { ctx.Card }, "revealed with " + Pass);
                var top = ctx.Opp.Deck[0];
                ctx.Engine.NoteLookedAt(new List<CardInstance> { top }, "Top card of your opponent's deck", ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { top }, Pass, "The top card of your opponent's deck", ctx.Card.Def);
                ctx.Me.Hand.Remove(ctx.Card);
                CardsCRZ1.PutInLostZone(ctx, ctx.Card);
                if (!top.Def.IsSupporter || ctx.Opp.Deck.Count == 0 || ctx.Opp.Deck[0] != top) return;
                if (!ctx.AskYesNo(Pass + ": put " + top.Def.Name + " from the top of your opponent's deck in the Lost Zone?")) return;
                ctx.Opp.Deck.RemoveAt(0);
                CardsCRZ1.PutInLostZone(ctx, top);
            }
        }
    }
}
