using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Blast (PLB, v109): Grass to Psychic Pokémon (1-46). Lileep, Tirtouga and Archen are Restored Pokémon (Root Fossil Lileep,
    /// Cover Fossil, Plume Fossil). Genesect-EX is Celebration's Classic Collection card (30C CC20).
    /// </summary>
    public static class CardsPLB1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLB-1", new CardsPOR.SweetScentMon("Sweet Scent", 20));
            r.RegisterByKey("PLB-2", new MasquerainToolReversal());
            r.RegisterByKey("PLB-3", new PrehistoricCallMon(new CardsTWM1.HealSelfMon("Spiral Drain", 10)));
            r.RegisterByKey("PLB-4", new ComboMon(new LifesplosionMon(), new CardsTWM1.HealSelfMon("Spiral Drain", 20)));
            r.RegisterByKey("PLB-5", new ComboMon(new DrawUntilAttackMon("Return", 6), new CardsMEG.DefendingEnergyBonusMon("Energy Press", 20)));
            r.RegisterByKey("PLB-7", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("PLB-8", new ComboMon(new RetributionMon(), new ConditionAttackMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("PLB-9", new VirizionVerdantWind());
            r.RegisterByKey("PLB-10", new ComboMon(new GrassCallForFamilyMon(), new FlipBonusMon("Jet Impact", 20)));
            r.RegisterByKey("PLB-12", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("PLB-13", new ComboMon(new SolarTransporterMon(), new CardsDAA1.DrainMon("Leech Life")));
            r.RegisterByKey("PLB-14", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("PLB-15", new ComboMon(new ParalyzeFlipMon("Bubble"), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("PLB-16", new AttachAsOftenMon("Deluge", EnergyType.Water, new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10)));
            r.RegisterByKey("PLB-17", new HealEachBenchedMon("Cleanse Away", 30));
            r.RegisterByKey("PLB-18", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("PLB-19", new ComboMon(new CardsMEG.SnipeMon("Sharpshooting", 30), new ParalyzeFlipMon("Bubble Beam")));
            r.RegisterByKey("PLB-20", new CardsLTR2.SafeguardMon(null));
            r.RegisterByKey("PLB-21", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("PLB-22", new ComboMon(new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep), new ReflectEnergyMon()));
            r.RegisterByKey("PLB-23", new FroslassCursedGlare());
            r.RegisterByKey("PLB-24", new FossilHuntMon());
            r.RegisterByKey("PLB-26", new BangHeadsMon());
            r.RegisterByKey("PLB-27", new PrehistoricCallMon(new CoinsMon("Slam", 2)));
            r.RegisterByKey("PLB-28", new FossilClutchMon());
            r.RegisterByKey("PLB-29", new CardsMEG.SnipeMon("Splatter", 20));
            r.RegisterByKey("PLB-30", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new CardsASC.SelfDiscardTypeMon("Giga Frost", EnergyType.Water, 2)));
            r.RegisterByKey("PLB-32", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("PLB-33", new ComboMon(new CrushAndBurnMon(), new CoinsMon("Thunder Tempest", 4)));
            r.RegisterByKey("PLB-34", new ComboMon(new CardsPAR2.FlipConditionMon("Creepy Wind", SpecialCondition.Confused), new Cards30CC.BenchHitMon("Wind Blast", 40)));
            r.RegisterByKey("PLB-35", new DrifblimBalloon());
            r.RegisterByKey("PLB-36", new CardsLOR1.CountersAnyWayMon("Psypower", 3));
            r.RegisterByKey("PLB-37", new MespritShroud());
            r.RegisterByKey("PLB-38", new ComboMon(new SwitchSelfMon("Trading Places", false), new PsyjammingMon()));
            r.RegisterByKey("PLB-39", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("PLB-40", new ComboMon(new CardsXYP2.PrecognitiveDreamMon(), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("PLB-41", new SigilyphToolbox());
            r.RegisterByKey("PLB-42", new DodgeMon("Hide", false));
            r.RegisterByKey("PLB-43", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("PLB-44", new ComboMon(new BarrierAttackMon(), new SwitchSelfMon("Telekinesis of Nobility", false)));
            r.RegisterByKey("PLB-46", new ComboMon(new IronFistOfJusticeMon(), new CardsFLF1.NoResistanceMon("Shadow Punch")));
        }

        // ================================================================== Shared

        /// <summary>A Team Plasma Pokémon in play (its card, or a Team Plasma Badge).</summary>
        internal static bool IsPlasma(EffectContext ctx, PokemonInPlay mon) { return mon != null && ctx.Engine.IsTeamPlasma(mon); }

        /// <summary>How many Plasma Energy are attached to the Pokémon.</summary>
        internal static int PlasmaEnergy(PokemonInPlay mon) { return mon == null ? 0 : mon.Energy.Count(e => e.Def.Name == "Plasma Energy"); }

        /// <summary>Prehistoric Call: during your turn, if this Pokémon is in your discard pile, you may put it on the bottom of your deck (each time it is back there: Propagation ruling, 2013-05-16).</summary>
        internal sealed class PrehistoricCallMon : Mon
        {
            private readonly Mon attacks;

            public PrehistoricCallMon(Mon attacks) { this.attacks = attacks; }

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.Me.Deck.Add(ctx.Card);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s " + ctx.Card.Def.Name + " used its Ability Prehistoric Call: it went to the bottom of the deck.", ctx.Card.Id, -1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Draw cards until you have N cards in your hand" after the damage (Tropius's Return).</summary>
        internal sealed class DrawUntilAttackMon : Mon
        {
            private readonly string attack;
            private readonly int hand;

            public DrawUntilAttackMon(string attack, int hand)
            {
                this.attack = attack;
                this.hand = hand;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.DrawUntil(hand);
                return true;
            }
        }

        /// <summary>"Heal N damage from each of your Benched Pokémon" (Lapras's Cleanse Away).</summary>
        internal sealed class HealEachBenchedMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealEachBenchedMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) if (mon.DamageCounters > 0) ctx.Heal(mon, amount);
                return true;
            }
        }

        /// <summary>
        /// "As often as you like during your turn (before your attack), you may attach a [type] Energy card from your hand to 1 of your
        /// Pokémon" (Blastoise's Deluge, like Emboar's Inferno Fandango): a person attaches as many as they like, a bot one per use.
        /// </summary>
        internal sealed class AttachAsOftenMon : Mon
        {
            private readonly string ability;
            private readonly EnergyType type;
            private readonly Mon attacks;

            public AttachAsOftenMon(string ability, EnergyType type, Mon attacks)
            {
                this.ability = ability;
                this.type = type;
                this.attacks = attacks;
            }

            private bool Matches(CardDef d) { return CardsSSP1.IsBasicOf(d, type); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != ability || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(c => Matches(c.Def)) && ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                if (RepeatableAbility.ForPerson(ctx))
                {
                    RepeatableAbility.AttachFromHand(ctx, Matches, () => ctx.MyPokemon(), ability, "a " + type + " Energy card");
                    return;
                }
                var card = ctx.Me.Hand.FirstOrDefault(c => Matches(c.Def));
                if (card == null) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)), ability + ": attach a " + type + " Energy card to which Pokémon?");
                if (target == null) return;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>Tool Reversal: as often as you like during your turn, put a Pokémon Tool attached to 1 of your Pokémon into your hand.</summary>
        private sealed class MasquerainToolReversal : Mon
        {
            private const string Reversal = "Tool Reversal";

            private static List<KeyValuePair<PokemonInPlay, CardInstance>> Tools(EffectContext ctx)
            {
                var list = new List<KeyValuePair<PokemonInPlay, CardInstance>>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var tool in mon.Tools)
                        if (tool.Owner == ctx.Player) list.Add(new KeyValuePair<PokemonInPlay, CardInstance>(mon, tool));
                return list;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Reversal && ctx.IsMyTurn && Tools(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => Tools(ctx).Count > 0, offerDone =>
                {
                    var all = Tools(ctx);
                    var holders = all.Select(kv => kv.Key).Distinct().ToList();
                    var holder = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, Reversal + ": put a Pokémon Tool from which Pokémon into your hand?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (holder == null) return false;
                    var tools = all.Where(kv => kv.Key == holder).Select(kv => kv.Value).ToList();
                    var tool = ctx.ChooseCards(tools, 1, 1, Reversal + ": put which Pokémon Tool into your hand?")[0];
                    if (!holder.Tools.Remove(tool)) return false;
                    ctx.Me.Hand.Add(tool);
                    ctx.Log(tool.Def.Name + " was put into " + ctx.Engine.PlayerName(ctx.Player) + "'s hand from " + holder.Name + " (Tool Reversal).");
                    return true;
                });
            }
        }

        /// <summary>Lifesplosion: for each Energy attached to this Pokémon, search your deck for a Stage 2 Pokémon and put it onto your Bench.</summary>
        private sealed class LifesplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lifesplosion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(ctx.Pokemon.Energy.Count, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage2 && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Lifesplosion: choose up to " + n + " Stage 2 Pokémon to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Retribution: if an Escavalier of yours was Knocked Out by damage from an opponent's attack during their last turn, all Energy on the Defending Pokémon goes into the opponent's hand.</summary>
        private sealed class RetributionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Retribution") return false;
                var defending = ctx.Defending;
                bool avenged = ctx.WasKnockedOutLastOpponentTurn(d => d.Name == "Escavalier", true);
                ctx.DamageDefending(atk.Damage);
                if (!avenged || !TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var energy in defending.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(defending, energy);
                return true;
            }
        }

        /// <summary>
        /// Verdant Wind: your Pokémon with any Grass Energy attached can't be affected by Special Conditions (and lose any they have).
        /// Emerald Slash: you may search your deck for 2 Grass Energy cards and attach them to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class VirizionVerdantWind : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && src.Engine.HasEnergyOfType(target, EnergyType.Grass);
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Emerald Slash") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 2, "Emerald Slash: you may choose up to 2 Grass Energy cards for 1 of your Benched Pokémon");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Emerald Slash: attach them to which Benched Pokémon?");
                    foreach (var card in found)
                        if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Call for Family: search your deck for up to 2 Grass Basic Pokémon and put them onto your Bench.</summary>
        private sealed class GrassCallForFamilyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.PokemonType == EnergyType.Grass || d.SecondType == EnergyType.Grass, 2, "Call for Family: put up to 2 Grass Basic Pokémon onto your Bench");
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Solar Transporter: reveal the top 5 cards of your deck; Team Plasma cards go into your hand, the others are discarded.</summary>
        private sealed class SolarTransporterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Transporter") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(5);
                if (top.Count == 0) return true;
                ctx.Engine.Reveal(ctx.Player, top, "revealed the top cards of the deck (Solar Transporter)");
                var plasma = top.Where(c => c.Def.IsTeamPlasma).ToList();
                ctx.DeckToHand(plasma);
                var rest = top.Where(c => !c.Def.IsTeamPlasma && ctx.Me.Deck.Contains(c)).ToList();
                foreach (var card in rest)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Reflect Energy: move a Water Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class ReflectEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflect Energy") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                var card = water.Select(e => e.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Reflect Energy: move which Water Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Reflect Energy: move " + card.Def.Name + " to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Cursed Glare: while Active, the opponent can't attach Special Energy cards from their hand. Blizzard: 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class FroslassCursedGlare : Mon
        {
            private static readonly Mon Blizzard = new CardsMEW1.SpreadBenchMon("Blizzard", 10);

            public override bool BlocksEnergyCard(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && player != src.Player && card != null && card.Def.IsSpecialEnergy && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blizzard.HandleAttack(ctx, atk); }
        }

        /// <summary>Fossil Hunt: put 2 Item cards with "Fossil" in their names from your discard pile into your hand.</summary>
        private sealed class FossilHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fossils = ctx.Me.Discard.Where(c => c.Def.IsItem && c.Def.Name.Contains("Fossil")).ToList();
                if (fossils.Count == 0) return true;
                var picks = fossils.Count <= 2 ? fossils : ctx.ChooseCards(fossils, 2, 2, "Fossil Hunt: put which 2 Fossil cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Bang Heads: both this Pokémon and the Defending Pokémon are now Confused.</summary>
        private sealed class BangHeadsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bang Heads") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Fossil Clutch: you may discard an Item card with "Fossil" in its name from your hand; if you do, 50 more damage.</summary>
        private sealed class FossilClutchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Clutch") return false;
                var fossils = ctx.Me.Hand.Where(c => c.Def.IsItem && c.Def.Name.Contains("Fossil")).ToList();
                bool more = false;
                if (fossils.Count > 0 && ctx.AskYesNo("Fossil Clutch: discard a Fossil card from your hand for 50 more damage?"))
                {
                    var pick = ctx.ChooseCards(fossils, 1, 1, "Fossil Clutch: discard which Fossil card?")[0];
                    ctx.Engine.DiscardFromHand(ctx.Player, pick);
                    more = true;
                }
                ctx.DamageDefending(atk.Damage + (more ? 50 : 0));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Crush and Burn: discard as many Energy attached to your Pokémon as you like; 30 damage for each one discarded.</summary>
        private sealed class CrushAndBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush and Burn") return false;
                var all = new List<CardInstance>();
                var holderOf = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var energy in mon.Energy)
                    {
                        all.Add(energy);
                        holderOf[energy] = mon;
                    }
                int discarded = 0;
                if (all.Count > 0)
                {
                    var picks = ctx.ChooseCards(all, 0, all.Count, "Crush and Burn: discard any number of Energy from your Pokémon (30 damage each)");
                    foreach (var card in picks)
                        if (ctx.Engine.DiscardEnergy(holderOf[card], card)) discarded++;
                }
                ctx.DamageDefending(atk.Damage * discarded);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Drifting Balloon: this Pokémon's attacks cost Colorless less for each of the opponent's Team Plasma Pokémon in play. Derail.</summary>
        private sealed class DrifblimBalloon : Mon
        {
            private static readonly Mon Derail = new CardsASR1.DiscardDefendingSpecialMon("Derail", false);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                int n = src.OpponentPokemon(m => IsPlasma(src, m)).Count;
                for (int i = 0; i < n; i++)
                {
                    int at = cost.IndexOf(EnergyType.Colorless);
                    if (at < 0) break;
                    cost.RemoveAt(at);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Derail.HandleAttack(ctx, atk); }
        }

        /// <summary>Mental Shroud: if you have Uxie and Azelf in play, each of your Pokémon has no Weakness.</summary>
        private sealed class MespritShroud : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return false;
                return src.MyPokemon(m => m.Name == "Uxie").Count > 0 && src.MyPokemon(m => m.Name == "Azelf").Count > 0;
            }
        }

        /// <summary>Psyjamming: move as many Special Energy on the opponent's Pokémon to their other Pokémon as you like.</summary>
        private sealed class PsyjammingMon : Mon
        {
            private static List<PokemonInPlay> Givers(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsSpecialEnergy) && ctx.CanAffect(m)); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyjamming") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                RepeatableAbility.Repeat(ctx, () => ctx.OpponentPokemon().Count >= 2 && Givers(ctx).Count > 0, offerDone =>
                {
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Psyjamming: move a Special Energy from which of your opponent's Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var specials = giver.Energy.Where(e => e.Def.IsSpecialEnergy).ToList();
                    var card = ctx.ChooseCards(specials, 1, 1, "Psyjamming: move which Special Energy?")[0];
                    var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.OpponentPokemon(m => m != giver && ctx.CanAffect(m)), "Psyjamming: move " + card.Def.Name + " to which Pokémon?", offerDone);
                    return to != null && ctx.Engine.MoveEnergy(giver, to, card);
                });
                return true;
            }
        }

        /// <summary>Toolbox: this Pokémon can have up to 4 Pokémon Tools attached (the engine trims them back to 1 if it loses the Ability).</summary>
        private sealed class SigilyphToolbox : Mon
        {
            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(max, 4) : max;
            }
        }

        /// <summary>Barrier Attack: 30, and during the opponent's next turn this Pokémon takes 30 less damage from attacks (after W/R).</summary>
        private sealed class BarrierAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Barrier Attack") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(30);
                return true;
            }
        }

        /// <summary>Iron Fist of Justice: does nothing if you have any Team Plasma Pokémon in play.</summary>
        private sealed class IronFistOfJusticeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Fist of Justice") return false;
                if (ctx.MyPokemon(m => IsPlasma(ctx, m)).Count > 0)
                {
                    ctx.Log("Iron Fist of Justice: you have a Team Plasma Pokémon in play, so the attack does nothing.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }
    }
}
