using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Forbidden Light (FLI, v84), part 3: Fighting, Darkness, Metal, Fairy, Dragon and Colorless (FLI 57-101). Zygarde-GX,
    /// Rockruff, Buzzwole and Guzzlord (Hidden Fates Shiny Vault) and Ultra Necrozma-GX (Dragon Majesty 78) were already in the pool.
    /// </summary>
    public static class CardsFLI3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("FLI-57", new LockAfterAttackMon("Burdensome Bone", false));
            r.RegisterByKey("FLI-58", new ComboMon(new CardsDAA1.DrainMon("Giga Drain"), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20)));
            r.RegisterByKey("FLI-59", new InfernapeFLI());
            r.RegisterByKey("FLI-60", new CardsSIT1.AscensionMon());
            r.RegisterByKey("FLI-61", new CardsSIT1.AscensionMon());
            r.RegisterByKey("FLI-62", new ComboMon(new CardsMEG.SnipeMon("Quick Dive", 50), new CardsBRS1.SupporterBonusMon("Royal Blades", "Cynthia", 100)));
            r.RegisterByKey("FLI-63", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("FLI-64", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Exact Revenge", 70, c => c.WasKnockedOutLastOpponentTurn(d => d.PokemonType == EnergyType.Psychic || d.SecondType == EnergyType.Psychic, true))));
            r.RegisterByKey("FLI-65", new Cards30C.TypeEnergyBonusMon("Act Tough", EnergyType.Darkness, 30));
            r.RegisterByKey("FLI-66", new DrawAttackMon("Allotment", 1));
            r.RegisterByKey("FLI-67", new SevenShockMon());
            r.RegisterByKey("FLI-68", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("FLI-69", new TyrantrumFLI());
            r.RegisterByKey("FLI-70", new SkyDropMon());
            r.RegisterByKey("FLI-71", new ZygardeEarthenAura());
            r.RegisterByKey("FLI-72", new ComboMon(new ParalyzeFlipMon("Glare"), new CardsFST2.ConditionBonusMon("Calm Strike", 60, c => c.Me.GxUsed)));
            r.RegisterByKey("FLI-74", new DianciePrismStar());
            r.RegisterByKey("FLI-76", new CardsASR1.PlusMon("Dangerous Rogue", 20, c => c.Opp.Bench.Count));
            // Darkness
            r.RegisterByKey("FLI-78", new ComboMon(new UntamedPunchMon(), new CardsWHT.FlipMoreMon("Double Stomp", 2, 40)));
            r.RegisterByKey("FLI-79", new YveltalGX());
            // Metal
            r.RegisterByKey("FLI-81", new ComboMon(new CardsASR1.TimesMon("Total Command", c => c.Me.Bench.Count + c.Opp.Bench.Count), new CardsDRI2.CrunchMon("Whirlpool")));
            r.RegisterByKey("FLI-82", new ComboMon(new CardsBCR1.DrawUntilAttackMon("Overclock", 6), new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new TimelessMon()));
            // Fairy
            r.RegisterByKey("FLI-83", new CardsTEU3.EvolutionaryAdvantageMon());
            r.RegisterByKey("FLI-84", new SecretBlessingsMon());
            r.RegisterByKey("FLI-85", new SwirlingPetalsMon());
            r.RegisterByKey("FLI-86", new FlorgesFLI());
            r.RegisterByKey("FLI-87", new WinkWinkMon());
            r.RegisterByKey("FLI-88", new ComboMon(new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1),
                new CardsFST2.ConditionBonusMon("Electrichain", 30, c => c.Me.Bench.Any(m => c.Engine.IsType(m, EnergyType.Lightning)))));
            r.RegisterByKey("FLI-89", new ComboMon(new ConditionAttackMon("Metal Sound", SpecialCondition.Confused), new NoRetreatMon("Fairy Lock")));
            r.RegisterByKey("FLI-90", new ComboMon(new Cards30CC.BenchHitMon("Overrun", 20), new SanctuaryMon()));
            // Dragon
            r.RegisterByKey("FLI-91", new GoomyFLI());
            r.RegisterByKey("FLI-92", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("FLI-93", new CardsTWM1.HealSelfMon("Absorb", 30));
            r.RegisterByKey("FLI-94", new GoodraFLI());
            // Colorless
            r.RegisterByKey("FLI-96", new ArceusPrismStar());
            r.RegisterByKey("FLI-97", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("FLI-98", new ComboMon(new MountaintopMiningMon(), new CardsPFL.FlipUntilTailsTimesMon("Rock Cannon")));
            r.RegisterByKey("FLI-99", new CardsDAA1.MayDrawUntilMon("Return", 5));
            r.RegisterByKey("FLI-100", new DestructiveSoundMon());
            r.RegisterByKey("FLI-101", new ComboMon(new ConditionAttackMon("Supersonic", SpecialCondition.Confused),
                new CardsFST2.ConditionBonusMon("Resonance", 70, c => c.Defending != null && c.Defending.Has(SpecialCondition.Confused))));
        }

        // ================================================================== Fighting

        /// <summary>Flaming Fighter: the opponent's Burned Pokémon get 6 damage counters instead of 2 in Pokémon Checkup (doesn't stack: Compendium, 2018-02-01). Burst Punch: Burned.</summary>
        private sealed class InfernapeFLI : Mon
        {
            private static readonly Mon Punch = new ConditionAttackMon("Burst Punch", SpecialCondition.Burned);

            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return mon != null && mon.Owner != src.Player ? Math.Max(counters, 6) : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>Seven Shock: if you have exactly 7 cards in your hand, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class SevenShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seven Shock") return false;
                var defending = ctx.Defending;
                bool seven = ctx.Me.Hand.Count == 7;
                ctx.DamageDefending(atk.Damage);
                if (seven && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Tyrannical Heart: as long as you don't have more Pokémon in play than the opponent, this Pokémon's attacks do 60 more damage
        /// to the opponent's Active Pokémon and it takes 30 less damage from attacks. Crunch: discard an Energy from the Defending Pokémon.
        /// </summary>
        private sealed class TyrantrumFLI : Mon
        {
            private static readonly Mon Crunch = new CardsDRI2.CrunchMon("Crunch");

            private static bool Outnumbered(EffectContext src) { return src.Me.InPlay().Count() <= src.Opp.InPlay().Count(); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || !Outnumbered(src)) return damage;
                return damage + 60;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || !Outnumbered(src)) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Sky Drop: 20 less damage for each [C] in the opponent's Active Pokémon's Retreat Cost.</summary>
        private sealed class SkyDropMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sky Drop") return false;
                int cost = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                int dmg = Math.Max(0, atk.Damage - 20 * cost);
                if (dmg > 0) ctx.DamageDefending(dmg);
                else ctx.Log("Sky Drop did no damage.");
                return true;
            }
        }

        /// <summary>Earthen Aura: damage from this Pokémon's attacks isn't affected by Weakness or Resistance. Peace Maker: 30 more if the opponent has an Ultra Beast in play.</summary>
        private sealed class ZygardeEarthenAura : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peace Maker") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                bool beast = ctx.Opp.InPlay().Any(m => m.Def.IsUltraBeast);
                int dmg = atk.Damage + (beast ? 30 : 0);
                bool aura = !ctx.Engine.AbilityBlocked(ctx.Pokemon, "Earthen Aura");
                if (aura) ctx.DamageWith(defending, dmg, false, false, false);
                else ctx.DamageDefending(dmg);
                return true;
            }
        }

        /// <summary>Princess's Cheers: while this Pokémon is on your Bench, your Fighting Pokémon's attacks do 20 more damage to the opponent's Active Pokémon. Diamond Rain: heal 30 from each of your Benched Pokémon.</summary>
        private sealed class DianciePrismStar : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || src.IsActive(me) || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(a, EnergyType.Fighting) ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Rain") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) if (mon.DamageCounters > 0) ctx.Heal(mon, 30);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Untamed Punch: with any damage counters on this Pokémon, 50 more damage and both Active Pokémon are now Confused (both need the counters: Compendium, 2018-05-03).</summary>
        private sealed class UntamedPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Untamed Punch") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool hurt = me.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage + (hurt ? 50 : 0));
                if (!hurt) return true;
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Absorb Vitality: heal from this Pokémon the damage it did. Sonic Evil: not affected by Weakness or Resistance. Doom Count-GX:
        /// the opponent's Active Pokémon is Knocked Out if it has exactly 4 damage counters.
        /// </summary>
        private sealed class YveltalGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Absorb Vitality")
                {
                    int done = ctx.DamageDefending(atk.Damage);
                    if (done > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, done);
                    return true;
                }
                if (atk.Name == "Sonic Evil")
                {
                    if (defending != null) ctx.DamageWith(defending, atk.Damage, false, false, false);
                    return true;
                }
                if (atk.Name == "Doom Count-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && defending.DamageCounters == 4) ctx.Engine.KnockOut(defending, "Doom Count-GX");
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        /// <summary>Timeless-GX: take another turn after this one (skip the between-turns step).</summary>
        private sealed class TimelessMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Timeless-GX") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.State.ExtraTurnFor = ctx.Player;
                ctx.State.SkipCheckup = true;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " will take another turn after this one (Timeless-GX).");
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Secret Blessings: shuffle 3 cards in any combination of Pokémon and basic Energy cards from your discard pile into your deck.</summary>
        private sealed class SecretBlessingsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Secret Blessings") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(3, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Secret Blessings: shuffle which 3 Pokémon and basic Energy cards into your deck?");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Swirling Petals: switch 1 of the opponent's Benched Pokémon with their Active Pokémon; if you do, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class SwirlingPetalsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Petals") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Active == null || ctx.GustTargets().Count == 0) return true;
                var now = ctx.GustOpponent("Swirling Petals: switch which of your opponent's Benched Pokémon into the Active Spot?");
                if (now == null || ctx.Opp.Active != now) return true;
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Wondrous Gift: once during your turn, flip a coin; on heads, put an Item card from your discard pile on top of your deck. Mist Guard: no damage from Dragon Pokémon next turn.</summary>
        private sealed class FlorgesFLI : Mon
        {
            protected override string Activated { get { return "Wondrous Gift"; } }

            private static List<CardInstance> Items(EffectContext ctx) { return ctx.Me.Discard.Where(c => CardsLOT4.IsItemCard(c.Def)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return Items(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                if (ctx.Engine.TrainerDiscardToDeckBlocked(ctx.Player))
                {
                    ctx.Log("Trainer cards in " + ctx.Engine.Their(ctx.Player) + " discard pile can't be put into " + ctx.Engine.Their(ctx.Player) + " deck right now.");
                    return;
                }
                var items = Items(ctx);
                if (items.Count == 0) return;
                var pick = ctx.ChooseCards(items, 1, 1, "Wondrous Gift: put which Item card on top of your deck?");
                if (pick.Count == 0 || !ctx.Me.Discard.Remove(pick[0])) return;
                ctx.Me.Deck.Insert(0, pick[0]);
                ctx.Engine.Reveal(ctx.Player, pick, "put on top of their deck (Wondrous Gift)");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mist Guard") return false;
                ctx.DamageDefending(atk.Damage);
                CardsPBL.ShieldSelfFrom(ctx, atk.Name, 9999, m => ctx.Engine.IsType(m, EnergyType.Dragon), "Dragon Pokémon");
                return true;
            }
        }

        /// <summary>
        /// Wink Wink: the opponent reveals their hand; you may discard a Supporter card there and use its effect as this attack's effect.
        /// That isn't playing a Supporter, and the card's own conditions still apply (Compendium, 2018-05-03).
        /// </summary>
        private sealed class WinkWinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wink Wink") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in ctx.Opp.Hand)
                {
                    if (!card.Def.IsSupporter || !seen.Add(card.Def.Name)) continue;
                    var probe = new EffectContext(ctx.Engine, ctx.Player, card, null);
                    if (ctx.Engine.Scripts.For(card.Def).CanPlayTrainer(probe)) usable.Add(card);
                }
                if (usable.Count == 0) return true;
                var labels = usable.Select(x => "Discard " + x.Def.Name + " and use its effect").ToList();
                labels.Add("Don't discard a Supporter");
                int pick = ctx.ChooseOption("Wink Wink: discard a Supporter from your opponent's hand and use its effect?", labels.ToArray());
                if (pick < 0 || pick >= usable.Count) return true;
                var chosen = usable[pick];
                ctx.Engine.DiscardFromHand(ctx.Opponent, chosen);
                ctx.Log("Wink Wink: " + ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + ".");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
                return true;
            }
        }

        /// <summary>Sanctuary-GX: move all damage counters from each of your Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class SanctuaryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sanctuary-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0))
                    ctx.Engine.MoveDamageCounters(mon, defending, mon.DamageCounters, "Sanctuary-GX");
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Sticky Membrane: while this Pokémon is your Active Pokémon, the opponent's attacks cost [C] more (also free ones; not a cost set to nothing, Compendium 2018-05-03).</summary>
        private sealed class GoomyFLI : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !src.IsActive(me)) return;
                cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Hydration: whenever you attach a Water Energy card from your hand to this Pokémon, heal 20 damage from it. Soaking Horn: 80 more if it was healed this turn.</summary>
        private sealed class GoodraFLI : Mon
        {
            private static readonly Mon Horn = new CardsFST2.ConditionBonusMon("Soaking Horn", 80, c => c.Pokemon.HealedOnTurn == c.Turn);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !CardsSSP1.IsBasicOf(energy.Def, EnergyType.Water) || me.DamageCounters == 0) return;
                Announce(src, "Hydration");
                src.Heal(me, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Horn.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>
        /// First Law: prevent all effects of the opponent's attacks done to this Pokémon, except damage. Trinity Star: only with Grass,
        /// Water and Lightning Pokémon on your Bench; search your deck for up to 3 basic Energy cards and attach them any way you like.
        /// </summary>
        private sealed class ArceusPrismStar : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Trinity Star") return true;
                var bench = ctx.Me.Bench;
                return bench.Any(m => ctx.Engine.IsType(m, EnergyType.Grass)) && bench.Any(m => ctx.Engine.IsType(m, EnergyType.Water))
                       && bench.Any(m => ctx.Engine.IsType(m, EnergyType.Lightning));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trinity Star") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Trinity Star: search your deck for up to 3 basic Energy cards");
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (card, mon) => true, "Trinity Star", false);
                foreach (var c in found) if (!ctx.Me.Deck.Contains(c) && !ctx.MyPokemon().Any(m => m.Energy.Contains(c))) ctx.Me.Deck.Add(c);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Mountaintop Mining: you may do 40 more damage; if you do, discard the top 2 cards of your deck.</summary>
        private sealed class MountaintopMiningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountaintop Mining") return false;
                bool more = ctx.AskYesNo("Mountaintop Mining: do 40 more damage and discard the top 2 cards of your deck?");
                ctx.DamageDefending(atk.Damage + (more ? 40 : 0));
                if (more) ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                return true;
            }
        }

        /// <summary>Destructive Sound: the opponent reveals their hand; discard all Item cards there (not Pokémon Tools: your ruling, v90).</summary>
        internal sealed class DestructiveSoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Destructive Sound") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                foreach (var card in ctx.Opp.Hand.Where(c => CardsLOT4.IsItemCard(c.Def)).ToList())
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }
    }
}
