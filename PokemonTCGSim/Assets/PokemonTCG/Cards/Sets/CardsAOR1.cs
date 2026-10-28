using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ancient Origins (AOR, v98), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-36). Magikarp and Gyarados (19-20)
    /// are the same cards as Generations' and keep those scripts. The Ancient Traits θ Stop, θ Double and θ Max are applied by the
    /// engine from the card data (CardDef.AncientTrait).
    /// </summary>
    public static class CardsAOR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("AOR-1", new FlipBonusMon("Trip Over", 10));
            r.RegisterByKey("AOR-3", new VileplumeAOR());
            r.RegisterByKey("AOR-4", new ComboMon(new SwitchSelfMon("Windmill", false), new FlowerTornadoMon()));
            r.RegisterByKey("AOR-5", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("AOR-6", new AriadosAOR());
            r.RegisterByKey("AOR-7", new ComboMon(new CardsCES1.FlipConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Unseen Claw", 70, c => c.Defending != null && c.Defending.Conditions != SpecialCondition.None)));
            r.RegisterByKey("AOR-8", new JaggedSaberMon());
            r.RegisterByKey("AOR-10", new ComboMon(new CardsMEG.DrawUntilSixMon("Intelligence Gathering"),
                new CardsASR1.PlusMon("Bee Revenge", 10, c => c.Me.Discard.Count(x => x.Def.IsPokemon))));
            r.RegisterByKey("AOR-11", new ComboMon(new BeeDrainMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("AOR-12", new ComboMon(new BailOutMon(), new CardsPAR1.PrizeCountMon(80)));
            r.RegisterByKey("AOR-13", new EeveelutionEffectMon("Flare Effect", EnergyType.Fire, new FlipBonusMon("Heat Breath", 20)));
            r.RegisterByKey("AOR-14", new ComboMon(new BurningRoarMon(), new CardsASR1.PlusMon("Combat Blaze", 20, c => c.Opp.Bench.Count)));
            r.RegisterByKey("AOR-15", new ComboMon(new CardsASC.SelfGuardMon("Flame Screen", 30), new CardsGRI2.FlipBonusOrRecoilMon("Heat Tackle", 0, 30)));
            r.RegisterByKey("AOR-17", new ComboMon(new SolarBirthMon(), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("AOR-18", new ComboMon(new CardsWHT.FlipMoreMon("Burning Scales", 2, 20), new CardsDRI1.WhirlwindMon("Wind Wheel")));
            r.RegisterByKey("AOR-21", new ComboMon(new CardsASR1.PlusMon("Full Retaliation", 30, c => c.Me.Bench.Where(m => m.Name == "Magikarp").Sum(m => m.DamageCounters)),
                new CardsGRI2.FlipBonusOrRecoilMon("Thrash", 30, 30)));
            r.RegisterByKey("AOR-22", new EeveelutionEffectMon("Aqua Effect", EnergyType.Water, null));
            r.RegisterByKey("AOR-23", new ComboMon(new SearchAttackMon("Deep Sea Search", d => d.IsTool, 2), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("AOR-24", new ComboMon(new ParalyzeFlipMon("Ice Beam"), new ResistanceBlizzardMon()));
            r.RegisterByKey("AOR-25", new ComboMon(new GlaciateMon(), new IcecaliburMon()));
            r.RegisterByKey("AOR-26", new EeveelutionEffectMon("Electric Effect", EnergyType.Lightning, new CardsTWM1.SelfDiscardMon("Thunder Blast", 1)));
            r.RegisterByKey("AOR-27", new ComboMon(new ThunderRodMon(), new CardsCRZ1.PiercingMon("Sparkling Tail")));
            r.RegisterByKey("AOR-28", new ExavoltMon());
            r.RegisterByKey("AOR-29", new ElectricMischiefMon());
            r.RegisterByKey("AOR-30", new UnownAOR());
            r.RegisterByKey("AOR-32", new FutureSpinMon());
            r.RegisterByKey("AOR-33", new ComboMon(new RewindMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("AOR-34", new TailsNothingMon("Smash Punch"));
            r.RegisterByKey("AOR-35", new GolurkAOR());
            r.RegisterByKey("AOR-36", new HoopaEX());
        }

        // ================================================================== Shared helpers

        /// <summary>A Stage 1 Pokémon (not a BREAK, M Pokémon-EX or LV.X card, which the data also files under Stage 1).</summary>
        internal static bool IsStage1(CardDef d)
        {
            return d.IsPokemon && d.Stage == Stage.Stage1 && !d.IsBreak && !d.IsOldMega && !d.IsLevelUp;
        }

        /// <summary>Flare / Aqua / Electric Effect: each of your Stage 1 Pokémon in play is also this type (2015-08-13: they add up with Double Type).</summary>
        private sealed class EeveelutionEffectMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            public EeveelutionEffectMon(string ability, EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !IsStage1(mon.Def) || types.Contains(type)) return;
                types.Add(type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>Irritating Pollen: no player can play Item cards from their hand (Pokémon Tools aren't Item cards: your ruling, v90).</summary>
        private sealed class VileplumeAOR : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && card != null && card.Def.IsItem;
            }
        }

        /// <summary>Flower Tornado: move any number of [G] Energy between your Pokémon (each Grass Energy can be moved once).</summary>
        private sealed class FlowerTornadoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flower Tornado") return false;
                ctx.DamageDefending(atk.Damage);
                var moved = new HashSet<int>();
                for (int guard = 0; guard < 30; guard++)
                {
                    var sources = ctx.MyPokemon(m => m.Energy.Any(en => !moved.Contains(en.Id) && CardsDRI1.ProvidesType(ctx, m, en, EnergyType.Grass)));
                    if (sources.Count == 0 || ctx.MyPokemon().Count < 2) break;
                    if (!ctx.AskYesNo("Flower Tornado: move a Grass Energy to another of your Pokémon?")) break;
                    var giver = sources.Count == 1 ? sources[0] : ctx.ChoosePokemon(sources, "Flower Tornado: move a Grass Energy from which Pokémon?");
                    var grass = giver.Energy.Where(en => !moved.Contains(en.Id) && CardsDRI1.ProvidesType(ctx, giver, en, EnergyType.Grass)).ToList();
                    var card = grass.Count == 1 ? grass[0] : ctx.ChooseCards(grass, 1, 1, "Flower Tornado: move which Energy?")[0];
                    var targets = ctx.MyPokemon(m => m != giver);
                    var taker = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Flower Tornado: move " + card.Def.Name + " to which Pokémon?");
                    moved.Add(card.Id);
                    ctx.Engine.MoveEnergy(giver, taker, card);
                }
                return true;
            }
        }

        /// <summary>
        /// Poisonous Nest: once during your turn, both Active Pokémon that aren't Grass Pokémon are now Poisoned (not usable when both
        /// are Grass: 2015-08-13). Impound: the Defending Pokémon can't retreat.
        /// </summary>
        private sealed class AriadosAOR : Mon
        {
            private static readonly Mon Impound = new NoRetreatMon("Impound");

            protected override string Activated { get { return "Poisonous Nest"; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var list = new List<PokemonInPlay>();
                foreach (var a in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (a != null && !ctx.Engine.IsType(a, EnergyType.Grass)) list.Add(a);
                return list;
            }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in Targets(ctx)) ctx.ApplyCondition(mon, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Impound.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Jagged Saber: you may attach up to 2 [G] Energy cards from your hand to your Benched Pokémon in any way, and heal all damage
        /// from each Pokémon that got one. The attachment is from the hand (Hyper Hypnosis reacts to it: 2015-10-29).
        /// </summary>
        private sealed class JaggedSaberMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jagged Saber") return false;
                ctx.DamageDefending(atk.Damage);
                var got = new List<PokemonInPlay>();
                for (int i = 0; i < 2; i++)
                {
                    var grass = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
                    var bench = ctx.MyBench();
                    if (grass.Count == 0 || bench.Count == 0) break;
                    var picks = ctx.ChooseCards(grass, 0, 1, "Jagged Saber: attach a Grass Energy card from your hand to 1 of your Benched Pokémon (" + (i + 1) + " of up to 2)");
                    if (picks.Count == 0) break;
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Jagged Saber: attach " + picks[0].Def.Name + " to which Benched Pokémon?");
                    if (target == null || !ctx.Me.Hand.Remove(picks[0])) break;
                    if (ctx.Engine.AttachEnergyCard(picks[0], target, true) && !got.Contains(target)) got.Add(target);
                }
                foreach (var mon in got)
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Bee Drain: heal from this Pokémon as much damage as the attack did to the opponent's Active Pokémon.</summary>
        private sealed class BeeDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bee Drain") return false;
                int done = ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, done);
                return true;
            }
        }

        /// <summary>Bail Out: put 2 Pokémon from your discard pile into your hand (at least 1 if you have any: 2021-11-11).</summary>
        private sealed class BailOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bail Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(2, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Bail Out: put 2 Pokémon from your discard pile into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Burning Roar: discard the top 4 cards of your deck; attach the Fire Energy cards among them to your Pokémon in any way.</summary>
        private sealed class BurningRoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Roar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 4);
                var fire = top.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire) && ctx.Me.Discard.Contains(c)).ToList();
                if (fire.Count > 0) ctx.AttachCardsAnyWay(fire, fire.Count, (card, mon) => true, "Burning Roar: attach a Fire Energy card to 1 of your Pokémon");
                return true;
            }
        }

        /// <summary>Solar Birth: bench a Basic Pokémon from your deck, then attach up to 2 basic Energy cards from your deck to it.</summary>
        private sealed class SolarBirthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Birth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, "Solar Birth: choose a Basic Pokémon to put onto your Bench");
                if (found.Count == 0)
                {
                    ctx.ShuffleMyDeck();
                    return true;
                }
                var mon = ctx.Engine.PutOntoBench(ctx.Player, found[0], false);
                var energy = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Solar Birth: choose up to 2 basic Energy cards to attach to " + found[0].Def.Name);
                foreach (var card in energy)
                {
                    if (mon != null && ctx.InPlay(mon)) ctx.Engine.AttachEnergyCard(card, mon);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>"During your opponent's next turn, prevent all effects of attacks, including damage, done to this Pokémon by Pokémon-EX."</summary>
        internal sealed class ExBarrierEffect : ContinuousEffect
        {
            public ExBarrierEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.Attacker != null && info.AgainstOpponent && info.Attacker.Def.IsPokemonEX;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                var attacker = engine.CurrentSourcePokemon;
                return target.Uid == TargetUid && kind == EffectSourceKind.Attack && attacker != null && attacker.Owner != target.Owner && attacker.Def.IsPokemonEX;
            }
        }

        /// <summary>Resistance Blizzard: no damage or effects from Pokémon-EX's attacks to this Pokémon during the opponent's next turn.</summary>
        private sealed class ResistanceBlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Resistance Blizzard") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new ExBarrierEffect(me.Name + " is protected from Pokémon-EX during the opponent's next turn (Resistance Blizzard)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Glaciate: 30 damage to each of the opponent's Pokémon (W/R only for the Active).</summary>
        private sealed class GlaciateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glaciate") return false;
                var bench = ctx.Opp.Bench.ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in bench)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Icecalibur: discard an Energy from this Pokémon; the Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class IcecaliburMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Icecalibur") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Icecalibur: discard an Energy from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Icecalibur)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Thunder Rod: look at the top 4 cards of your deck, attach any Lightning Energy cards there to this Pokémon, shuffle the rest back.</summary>
        private sealed class ThunderRodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Rod") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var top = ctx.LookAtTop(4);
                var lightning = top.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
                var picks = lightning.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(lightning, 0, lightning.Count, "Thunder Rod: attach any number of Lightning Energy cards to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Deck.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Exavolt: you may do 50 more damage; if you do, the opponent's Active Pokémon is Paralyzed and this Pokémon does 30 damage to itself.</summary>
        private sealed class ExavoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Exavolt") return false;
                var defending = ctx.Defending;
                bool more = ctx.AskYesNo("Exavolt: do 50 more damage and Paralyze your opponent's Active Pokémon (this Pokémon does 30 damage to itself)?");
                ctx.DamageDefending(atk.Damage + (more ? 50 : 0));
                if (!more) return true;
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Electric Mischief: flip 3 coins; for each heads, a random card from the opponent's hand is shuffled into their deck.</summary>
        private sealed class ElectricMischiefMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Mischief") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && ctx.Opp.Hand.Count > 0) CardsTWM1.ShuffleRandomFromOpponentHand(ctx, heads);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Farewell Letter: once during your turn, if it's on your Bench, draw a card, then discard it and everything attached (not a Knock Out; not with an empty deck: 2019-03-28).</summary>
        private sealed class UnownAOR : Mon
        {
            protected override string Activated { get { return "Farewell Letter"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                ctx.Draw(1);
                if (ctx.InPlay(me)) ctx.Engine.DiscardPokemonFromPlay(me);
            }
        }

        /// <summary>Future Spin: look at the top 3 cards of either player's deck and put them back in any order.</summary>
        private sealed class FutureSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Future Spin") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool mine = ctx.Me.Deck.Count > 0;
                bool theirs = ctx.Opp.Deck.Count > 0;
                if (!mine && !theirs) return true;
                bool own = mine && (!theirs || ctx.ChooseOption("Future Spin: look at the top 3 cards of which deck?", "Your deck", "Your opponent's deck") == 0);
                if (own) CardsOBF2.ReorderOwnTop(ctx, 3, "Future Spin");
                else CardsDRI2.ReorderOpponentTop(ctx, 3, "Future Spin");
                return true;
            }
        }

        /// <summary>Rewind: devolve each of the opponent's evolved Pokémon, the top Evolution card going into their hand.</summary>
        private sealed class RewindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rewind") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Stack.Count >= 2 && !m.Def.IsMultiCard))
                    if (ctx.InPlay(mon)) ctx.Engine.Devolve(mon, 1);
                return true;
            }
        }

        /// <summary>Double Type: this Pokémon is Psychic and Fighting. Superpower: you may do 40 more and take 20 yourself.</summary>
        private sealed class GolurkAOR : Mon
        {
            private static readonly Mon Superpower = new CardsSUM5.OptionalBoostRecoilMon("Superpower", 40, 20);

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Psychic)) types.Add(EnergyType.Psychic);
                if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Superpower.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Scoundrel Ring: played from your hand onto your Bench, you may search your deck for up to 3 Pokémon-EX other than Hoopa-EX.
        /// Hyperspace Fury: discard 2 Energy from this Pokémon, then 100 damage to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class HoopaEX : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Scoundrel Ring", ctx.Me.Deck.Count > 0, "Scoundrel Ring: search your deck for up to 3 Pokémon-EX (not Hoopa-EX)?")) return;
                ctx.SearchToHand(d => d.IsPokemon && d.IsPokemonEX && d.Name != "Hoopa-EX", 3, "Scoundrel Ring: choose up to 3 Pokémon-EX other than Hoopa-EX");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyperspace Fury") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Hyperspace Fury: discard 2 Energy from " + me.Name);
                ctx.DamageAnyOpponent(atk.Damage);
                return true;
            }
        }
    }
}
