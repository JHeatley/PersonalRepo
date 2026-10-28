using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>SM Black Star Promos (SMP, v91), part 4: SM206-SM246 (the promos new to the pool) and Sabrina &amp; Brycen.</summary>
    public static class CardsSMP4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SMP-206", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("SMP-207", new TerritorialStrikeMon());
            r.RegisterByKey("SMP-216", new PorygonZGX());
            r.RegisterByKey("SMP-217", new ComboMon(new CardsDAA1.RandomHandToDeckMon("Night Watch", 2), new PaleMoonMon()));
            r.RegisterByKey("SMP-222", new ComboMon(new CardsASR1.TimesMon("Psywave", CardsLOT2.DefendingEnergy), new CardsEVS1.AstonishMon("Astonish")));
            r.RegisterByKey("SMP-224", new ComboMon(new CallForGreatnessMon(), new ParalyzeFlipMon("Psy Bolt")));
            r.RegisterByKey("SMP-227", new ComboMon(new FlipBonusMon("Quick Attack", 10), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("SMP-228", new LockAfterAttackMon("Psychic Raid", false));
            r.RegisterByKey("SMP-232", new ComboMon(new DodgeMon("Agility", false), new RecoilMon("Volt Tackle", 30), new ConditionAttackMon("Tail Break-GX", SpecialCondition.Paralyzed)));
            r.RegisterByKey("SMP-234", new CardsSMP1.FlipSearchMon("Find a Friend", d => d.IsPokemon, "a Pokémon"));
            r.RegisterByKey("SMP-235", new CardsPFL.FlipUntilTailsTimesMon("Running Charge"));
            r.RegisterByKey("SMP-236", new CardsASC.CounterattackMon("Spiky Shield", 3, new SpikyStormMon()));
            r.RegisterByKey("SMP-237", new ComboMon(new AromaxMon(), new FlipBonusMon("Leaf Blade", 40)));
            r.RegisterByKey("SMP-238", new ComboMon(new DodgeMon("Snow Cloak", false), new HypnoticBlizzardMon()));
            r.RegisterByKey("SMP-239", new CarracostaGX());
            r.RegisterByKey("SMP-243", new MillOpponentMon("Regiblast", 1));
            r.RegisterByKey("SMP-244", new ComboMon(new YankOutMon(), new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("SMP-245", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new NohahexMon()));
            r.RegisterByName("Sabrina & Brycen", new SabrinaAndBrycen());
        }

        // ================================================================== Shared helpers

        /// <summary>"Discard the top N cards of your opponent's deck" (Regiblast, Rumbling Wires).</summary>
        internal sealed class MillOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public MillOpponentMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, count);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Territorial Strike: does nothing unless you have a Stadium card in play (one you played).</summary>
        private sealed class TerritorialStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Territorial Strike") return false;
                if (ctx.State.Stadium == null || ctx.State.StadiumOwner != ctx.Player) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Troubleshooting: once during your turn, you may discard a Special Energy from this Pokémon; if you do, heal 80 damage from
        /// it. Abnormal Overheating: this Pokémon is now Burned. Critical Error-GX: search your deck for up to 10 cards and discard
        /// them.
        /// </summary>
        private sealed class PorygonZGX : Mon
        {
            protected override string Activated { get { return "Troubleshooting"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Pokemon.Energy.Any(en => en.Def.IsAttachedSpecialEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var special = me.Energy.Where(en => en.Def.IsAttachedSpecialEnergy).ToList();
                if (special.Count == 0) return;
                var card = ctx.ChooseCards(special, 1, 1, "Troubleshooting: discard which Special Energy?")[0];
                if (ctx.Engine.DiscardEnergy(me, card) && me.DamageCounters > 0) ctx.Heal(me, 80);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Abnormal Overheating")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Burned);
                    return true;
                }
                if (atk.Name != "Critical Error-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => true, 10, "Critical Error-GX: search your deck for up to 10 cards to discard", false);
                ctx.Me.Discard.AddRange(found);
                if (found.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + found.Count + " cards from " + ctx.Engine.Their(ctx.Player) + " deck (Critical Error-GX).");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Pale Moon-GX: the Defending Pokémon is Knocked Out at the end of the opponent's next turn; with at least 1 extra Psychic
        /// Energy on this Pokémon, also discard all Energy from the opponent's Active Pokémon.
        /// </summary>
        private sealed class PaleMoonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pale Moon-GX") return false;
                var defending = ctx.Defending;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Psychic);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of its next turn (Pale Moon-GX)", ctx.Player, defending.Uid, ctx.Turn, "Pale Moon-GX", 0));
                if (extra) ctx.Engine.DiscardAllEnergy(defending);
                return true;
            }
        }

        /// <summary>Call for Greatness: search your deck for up to 3 Pokémon-GX with different names and put them into your hand.</summary>
        private sealed class CallForGreatnessMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Greatness") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.IsGX, 3, "Call for Greatness: search your deck for up to 3 Pokémon-GX with different names");
                var keep = new List<CardInstance>();
                foreach (var card in found)
                {
                    if (keep.Any(k => k.Def.Name == card.Def.Name)) ctx.Me.Deck.Add(card);
                    else keep.Add(card);
                }
                ctx.Me.Hand.AddRange(keep);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Spiky Storm-GX: 100 damage to each of the opponent's Pokémon that has any damage counters on it.</summary>
        private sealed class SpikyStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiky Storm-GX") return false;
                var hurt = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return Nothing(ctx);
                foreach (var mon in hurt) ctx.DamageTarget(mon, 100);
                return true;
            }
        }

        /// <summary>Aromax: heal all damage from 1 of your Benched Pokémon.</summary>
        private sealed class AromaxMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aromax") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Aromax: heal all damage from which of your Benched Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Hypnotic Blizzard: the opponent's Active Pokémon is now Asleep; 20 damage to each of their Benched Pokémon.</summary>
        private sealed class HypnoticBlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnotic Blizzard") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>
        /// High Density Armor: with full HP, this Pokémon takes 90 less damage from the opponent's attacks (after Weakness and
        /// Resistance). Ground Crush: no retreat. Stone Age-GX: put any number of Pokémon that evolve from Unidentified Fossil from your
        /// discard pile onto your Bench.
        /// </summary>
        private sealed class CarracostaGX : Mon
        {
            private static readonly Mon Crush = new NoRetreatMon("Ground Crush");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !info.AgainstOpponent || me.DamageCounters > 0 || damage <= 0) return damage;
                return Math.Max(0, damage - 90);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stone Age-GX") return Crush.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fossils = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.EvolvesFrom == "Unidentified Fossil" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                int room = ctx.BenchSpace;
                if (fossils.Count == 0 || room <= 0) return true;
                var picks = ctx.ChooseCards(fossils, 0, Math.Min(room, fossils.Count), "Stone Age-GX: put any number of these Pokémon onto your Bench");
                foreach (var card in picks)
                    if (ctx.BenchSpace > 0 && ctx.Me.Discard.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                return true;
            }
        }

        /// <summary>Yank Out: discard random cards from the opponent's hand until they have 5 cards.</summary>
        private sealed class YankOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Yank Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int guard = 0;
                while (ctx.Opp.Hand.Count > 5 && guard++ < 60)
                {
                    var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                    int before = ctx.Opp.Hand.Count;
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                    if (ctx.Opp.Hand.Count == before) break;
                }
                return true;
            }
        }

        /// <summary>Nohahex: if the opponent's Active Pokémon has exactly 9 damage counters on it, it's Knocked Out.</summary>
        private sealed class NohahexMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nohahex") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (defending == null || defending.DamageCounters != 9 || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return Nothing(ctx);
                ctx.Engine.KnockOut(defending, "Nohahex");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Sabrina &amp; Brycen (TAG TEAM Supporter): search your deck for up to 2 basic Energy cards; you may discard 5 other cards from
        /// your hand to also search for up to 3 Pokémon of different types.
        /// </summary>
        private sealed class SabrinaAndBrycen : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool bonus = CardsCEC1.TagTeamBonus(ctx, 5, "Sabrina & Brycen: discard 5 other cards from your hand to also search for up to 3 Pokémon of different types?");
                if (ctx.Me.Deck.Count == 0) return;
                var energy = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Sabrina & Brycen: search your deck for up to 2 basic Energy cards");
                ctx.Me.Hand.AddRange(energy);
                if (bonus && ctx.Me.Deck.Count > 0)
                {
                    var found = ctx.SearchMyDeck(d => d.IsPokemon, 3, "Sabrina & Brycen: search your deck for up to 3 Pokémon of different types");
                    var keep = new List<CardInstance>();
                    foreach (var card in found)
                    {
                        if (keep.Any(k => k.Def.PokemonType == card.Def.PokemonType)) ctx.Me.Deck.Add(card);
                        else keep.Add(card);
                    }
                    ctx.Me.Hand.AddRange(keep);
                }
                ctx.ShuffleMyDeck();
            }
        }
    }
}
