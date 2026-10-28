using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legendary Treasures (LTR, v108): Bianca, Cedric Juniper (your rulings: the opponent guesses the exact height; it needs a
    /// Pokémon with a printed height in your hand) and the Radiant Collection (RC1-RC25). Crushing Hammer, Energy Switch and Double
    /// Colorless Energy were already in the pool.
    /// </summary>
    public static class CardsLTR3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Bianca", new Bianca());
            r.RegisterByName("Cedric Juniper", new CedricJuniper());
            r.RegisterByName("Elesa", new Elesa());

            r.RegisterByKey("LTR-RC1", new CardsCES1.AttachFromHandToSelfMon("Growth", EnergyType.Grass, 1));
            r.RegisterByKey("LTR-RC2", new FlipBonusMon("Leaf Blade", 30));
            r.RegisterByKey("LTR-RC3", new SerperiorGarden());
            r.RegisterByKey("LTR-RC4", new ComboMon(new RestAllMon(), new TailsNothingMon("Lunge")));
            r.RegisterByKey("LTR-RC6", new CardsMEG.WeakenDefendingMon("Charm", 20));
            r.RegisterByKey("LTR-RC7", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("LTR-RC8", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("LTR-RC9", new ComboMon(new TiptoeStepMon(), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("LTR-RC10", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new EternalRadianceMon()));
            r.RegisterByKey("LTR-RC11", new ComboMon(new BrilliantVoiceMon("Brilliant Voice"), new CardsBLK.RoundMon()));
            r.RegisterByKey("LTR-RC12", new CardsMEG.SandAttackMon("Attract"));
            r.RegisterByKey("LTR-RC13", new CaptivateFlipMon());
            r.RegisterByKey("LTR-RC14", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("LTR-RC15", new CardsTWM1.HealSelfMon("Honey Snack", 20));
            r.RegisterByKey("LTR-RC16", new ComboMon(new PicnicWeatherMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("LTR-RC17", new CardsPFL.FlipUntilTailsTimesMon("Relaxed Roll"));
            r.RegisterByKey("LTR-RC18", new CoinsMon("Tail Slap", 2));
            r.RegisterByKey("LTR-RC21", new ComboMon(new CardsVIV1.SynthesisMon(), new CardsASR1.PlusMon("Revenge Blast", 30, c => c.OpponentPrizesTaken)));
        }

        // ================================================================== Trainers

        /// <summary>Bianca: draw cards until you have 6 cards in your hand.</summary>
        private sealed class Bianca : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < 6;
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(6); }
        }

        /// <summary>Elesa: search your deck for up to 3 Pokémon Tool cards, reveal them and put them into your hand.</summary>
        private sealed class Elesa : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTool, 3, "Elesa: choose up to 3 Pokémon Tool cards");
            }
        }

        /// <summary>
        /// Cedric Juniper: put a Pokémon with a printed height from your hand face down and name it; the opponent guesses its exact height
        /// from the list of every height (a person chooses, a bot guesses at random). Right: they draw 3; wrong: you draw 3. The Pokémon
        /// stays in your hand. Your rulings (v108): exact height from the full list; not playable without such a Pokémon.
        /// </summary>
        internal sealed class CedricJuniper : CardScript
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && PokedexHeights.HasPrintedHeight(c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Candidates(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var candidates = Candidates(ctx);
                if (candidates.Count == 0) return;
                var pick = ctx.ChooseCards(candidates, 1, 1, "Cedric Juniper: put which Pokémon face down? (Your opponent guesses its height.)")[0];
                int height = PokedexHeights.HeightOf(pick.Def);
                ctx.Log("Cedric Juniper: " + ctx.Engine.PlayerName(ctx.Player) + " put a Pokémon face down and named it " + pick.Def.Name + ".");
                int guess = Guess(ctx, pick.Def.Name);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "revealed the Pokémon (Cedric Juniper)");
                bool right = guess == height;
                ctx.Log("Cedric Juniper: " + ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + PokedexHeights.Label(guess) + "; " + pick.Def.Name + " is "
                        + PokedexHeights.Label(height) + (right ? ". Right!" : ". Wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 3);
                else ctx.Draw(3);
            }

            /// <summary>The opponent's guess in decimetres: a person picks from every height, a bot guesses at random.</summary>
            internal static int Guess(EffectContext ctx, string name)
            {
                var heights = PokedexHeights.AllHeights();
                if (!ctx.Engine.IsInteractive(ctx.Opponent)) return heights[ctx.State.Rng.Next(heights.Count)];
                var opts = new List<DecisionOption>();
                for (int i = 0; i < heights.Count; i++) opts.Add(new DecisionOption { Label = PokedexHeights.Label(heights[i]), Index = i });
                int at = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Cedric Juniper: how tall is " + name + "?", opts, ctx.Source, false);
                return heights[opts[at].Index];
            }
        }

        // ================================================================== Radiant Collection

        /// <summary>Royal Garden: if this Pokémon has any Grass Energy attached, it has no Retreat Cost.</summary>
        private sealed class SerperiorGarden : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                return me != null && mon == me && src.Engine.HasEnergyOfType(me, EnergyType.Grass) ? 0 : cost;
            }
        }

        /// <summary>Rest: heal all damage from this Pokémon; it is now Asleep.</summary>
        internal sealed class RestAllMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Tiptoe Step: draw a card for each Psychic Energy attached to this Pokémon.</summary>
        private sealed class TiptoeStepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tiptoe Step") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Psychic);
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Eternal Radiance: move all damage counters from this Pokémon to the Defending Pokémon; it can't use Eternal Radiance next turn.</summary>
        private sealed class EternalRadianceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eternal Radiance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (defending != null && me.DamageCounters > 0 && ctx.CanAffect(defending))
                    ctx.Engine.MoveDamageCounters(me, defending, me.DamageCounters, atk.Name);
                if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Brilliant Voice: flip a coin; heads, the Defending Pokémon is now Asleep; tails, Confused. Used for Brilliant Voice, Distortion Beam (the attack name is passed in).</summary>
        internal sealed class BrilliantVoiceMon : Mon
        {
            private readonly string attack;

            public BrilliantVoiceMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Asleep : SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Captivate: flip a coin; heads, switch 1 of your opponent's Benched Pokémon with the Defending Pokémon.</summary>
        internal sealed class CaptivateFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Captivate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.GustOpponent("Captivate: choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>Picnic Weather: put a Teddiursa from your discard pile onto your Bench, then attach an Energy card from your discard pile to it.</summary>
        private sealed class PicnicWeatherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Picnic Weather") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bears = ctx.Me.Discard.Where(c => c.Def.Name == "Teddiursa" && c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                if (bears.Count == 0 || ctx.BenchSpace <= 0) return true;
                var bear = ctx.ChooseCards(bears, 1, 1, "Picnic Weather: put which Teddiursa onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(bear)) return true;
                var benched = ctx.Engine.PutOntoBench(ctx.Player, bear, false);
                if (benched == null) return true;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Picnic Weather: attach which Energy card to Teddiursa?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, benched);
                return true;
            }
        }
    }
}
