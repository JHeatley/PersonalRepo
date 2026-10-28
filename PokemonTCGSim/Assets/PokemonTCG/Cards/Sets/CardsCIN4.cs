using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crimson Invasion (CIN, v86), part 4: Colorless (CIN 78-90), Trainers and Special Energy (CIN 91-100), and Olivia, Wishful
    /// Baton and Warp Energy, which are only secret rares in this set. Type: Null and Silvally-GX (Hidden Fates Shiny Vault),
    /// Counter Catcher (PAR), Lusamine and Peeking Red Card (UPR) were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsCIN4
    {
        public static void Register(CardScriptRegistry r)
        {
            // Colorless
            r.RegisterByKey("CIN-78", new MiltankCIN());
            r.RegisterByKey("CIN-80", new ComboMon(new DracoMelodyMon(), new ShieldAttackMon("Cotton Guard", 30)));
            r.RegisterByKey("CIN-81", new Cards30C.PeerAtMon("Bug Search"));
            r.RegisterByKey("CIN-83", new ComboMon(new NoRetreatMon("Clutch"), new SkyHuntingMon()));
            r.RegisterByKey("CIN-84", new RegigigasCIN());
            r.RegisterByKey("CIN-85", new ParalyzeFlipMon("Tickle"));
            r.RegisterByKey("CIN-86", new AmazingPleaMon());
            r.RegisterByKey("CIN-88", new ComboMon(new DodgeMon("Dig", false), new CardsSSP2.StompOffMon("Hammer Arm", 1)));
            // Trainers
            r.RegisterByName("Dashing Pouch", new DashingPouch());
            r.RegisterByName("Devoured Field", new DevouredField());
            r.RegisterByName("Fighting Memory", new CardsUPR5.SilvallyMemory(EnergyType.Fighting));
            r.RegisterByName("Gladion", new Gladion());
            r.RegisterByName("Psychic Memory", new CardsUPR5.SilvallyMemory(EnergyType.Psychic));
            r.RegisterByName("Sea of Nothingness", new SeaOfNothingness());
            r.RegisterByName("Olivia", new Olivia());
            r.RegisterByName("Wishful Baton", new WishfulBaton());
            // Special Energy
            r.RegisterByName("Counter Energy", new CounterEnergy());
            r.RegisterByName("Warp Energy", new WarpEnergy());
        }

        // ================================================================== Colorless

        /// <summary>Moomoo Malt: while this Pokémon is your Active Pokémon, whenever you attach an Energy card from your hand to 1 of your Pokémon, heal 90 damage from it. Sitdown Splash: flip for 30 more.</summary>
        private sealed class MiltankCIN : Mon
        {
            private static readonly Mon Splash = new FlipBonusMon("Sitdown Splash", 30);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || target == null || target.Owner != src.Player || target.DamageCounters == 0) return;
                Announce(src, "Moomoo Malt");
                src.Heal(target, 90);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Draco Melody: flip a coin; on heads, search your deck for a Dragon Pokémon and put it onto your Bench. Stage 1 and Stage 2
        /// Dragon Pokémon can go straight onto the Bench (Compendium, 2017-11-02).
        /// </summary>
        private sealed class DracoMelodyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draco Melody") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0 || ctx.BenchSpace <= 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.HasType(EnergyType.Dragon) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1,
                    "Draco Melody: choose a Dragon Pokémon (any Stage) to put onto your Bench");
                CardsDRM2.BenchCards(ctx, found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Sky Hunting: if the opponent's Pokémon is Knocked Out by damage from this attack, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class SkyHuntingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sky Hunting") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && ctx.Engine.IsKnockedOut(defending) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Seal of Antiquity: this Pokémon can't attack unless Regirock, Regice and Registeel are all on your Bench. Giant Stomp: discard any Stadium card in play.</summary>
        private sealed class RegigigasCIN : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, "Seal of Antiquity")) return true;
                return new[] { "Regirock", "Regice", "Registeel" }.All(n => ctx.Me.Bench.Any(m => m.Def.Name == n));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giant Stomp") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Amazing Plea: choose 2 cards from your discard pile and ask the opponent; if they agree, put them into your hand, otherwise 80 damage to their Active Pokémon.</summary>
        private sealed class AmazingPleaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amazing Plea") return false;
                var pile = ctx.Me.Discard.ToList();
                int n = Math.Min(2, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Amazing Plea: choose 2 cards from your discard pile");
                string what = picks.Count == 0 ? "nothing" : string.Join(" and ", picks.Select(c => c.Def.Name).ToArray());
                if (ctx.Engine.AskYesNo(ctx.Opponent, "Amazing Plea: may your opponent put " + what + " from their discard pile into their hand? (If not, your Active Pokémon takes 80 damage.)", ctx.Source))
                {
                    if (picks.Count > 0) ctx.DiscardToHand(picks);
                    return true;
                }
                if (ctx.Defending != null) ctx.DamageDefending(80);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Dashing Pouch: Energy the holder discards to pay its Retreat Cost goes into your hand instead (any Energy, not only basic).</summary>
        private sealed class DashingPouch : CardScript
        {
            public override bool RetreatAnyEnergyToHand(EffectContext src, PokemonInPlay retreating)
            {
                return src.Pokemon != null && retreating == src.Pokemon;
            }
        }

        /// <summary>Devoured Field: the attacks of Darkness and Dragon Pokémon (both players') do 10 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class DevouredField : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(attacker, EnergyType.Darkness) || src.Engine.IsType(attacker, EnergyType.Dragon) ? damage + 10 : damage;
            }
        }

        /// <summary>
        /// Gladion: look at your face-down Prize cards and put 1 of them into your hand; then shuffle this Gladion into your remaining
        /// Prize cards, all face down (trading isn't taking a Prize card: Compendium).
        /// </summary>
        private sealed class Gladion : CardScript
        {
            private static List<CardInstance> FaceDown(EffectContext ctx)
            {
                var p = ctx.Me;
                return p.Prizes.Where(c => !p.FaceUpPrizeIds.Contains(c.Id)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return FaceDown(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var p = ctx.Me;
                var faceDown = FaceDown(ctx);
                if (faceDown.Count == 0) return;
                var pick = ctx.ChooseCards(faceDown, 1, 1, "Gladion: put which of your face-down Prize cards into your hand?");
                if (pick.Count == 0 || !p.Prizes.Remove(pick[0])) return;
                p.Hand.Add(pick[0]);
                p.Prizes.Add(ctx.Card);
                ctx.CardPlacedElsewhere = true;
                for (int i = p.Prizes.Count - 1; i > 0; i--)
                {
                    int j = ctx.State.Rng.Next(i + 1);
                    var tmp = p.Prizes[i];
                    p.Prizes[i] = p.Prizes[j];
                    p.Prizes[j] = tmp;
                }
                p.FaceUpPrizeIds.Clear();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " took a Prize card into their hand and shuffled Gladion into their Prize cards (Gladion).");
            }
        }

        /// <summary>Sea of Nothingness: Special Conditions aren't removed when Pokémon (both players') evolve or devolve.</summary>
        private sealed class SeaOfNothingness : CardScript
        {
            public override bool KeepsConditionsOnEvolve(EffectContext src, PokemonInPlay mon) { return true; }
        }

        /// <summary>Olivia: search your deck for up to 2 Pokémon-GX.</summary>
        private sealed class Olivia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.IsGX, 2, "Olivia: choose up to 2 Pokémon-GX");
            }
        }

        /// <summary>Wishful Baton: if the holder is your Active Pokémon and is Knocked Out by damage from an opponent's attack, move up to 3 basic Energy cards from it to 1 of your Benched Pokémon.</summary>
        private sealed class WishfulBaton : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !wasActive) return;
                var bench = src.Me.Bench.Where(m => m != mon && !src.Engine.IsKnockedOut(m)).ToList();
                var energy = mon.Energy.Where(en => en.Def.IsBasicEnergy).ToList();
                if (bench.Count == 0 || energy.Count == 0) return;
                var picks = src.ChooseCards(energy, 0, Math.Min(3, energy.Count), "Wishful Baton: move up to 3 basic Energy cards from " + mon.Name + " to 1 of your Benched Pokémon");
                if (picks.Count == 0) return;
                var to = src.ChoosePokemon(bench, "Wishful Baton: move them to which Benched Pokémon?");
                if (to == null) return;
                foreach (var e in picks) src.Engine.MoveEnergy(mon, to, e);
            }
        }

        // ================================================================== Special Energy

        /// <summary>
        /// Counter Energy: [C]. With more Prize cards left than the opponent, on a Pokémon that isn't a Pokémon-GX or Pokémon-EX it
        /// provides every type, 2 Energy at a time.
        /// </summary>
        private sealed class CounterEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                bool on = holder != null && !holder.Def.IsGXorEX && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
                return on ? RecifeEnergy.Units(EnergyMask.AnyType, 2) : RecifeEnergy.Units(EnergyMask.Colorless, 1);
            }
        }

        /// <summary>Warp Energy: [C]. When you attach it from your hand to your Active Pokémon, switch that Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class WarpEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.PokemonIsActive || ctx.Me.Bench.Count == 0) return;
                ctx.SwitchMyActive("Warp Energy: switch " + ctx.Pokemon.Name + " with which of your Benched Pokémon?");
            }
        }
    }
}
