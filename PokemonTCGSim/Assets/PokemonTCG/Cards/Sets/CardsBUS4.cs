using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Burning Shadows (BUS, v88), part 4: Fairy, Dragon and Colorless (BUS 91-111), and Lycanroc-GX (136, printed here only as a
    /// full art and a rainbow rare). Ralts, Kirlia, Gardevoir-GX, Diancie, Noibat and Noivern-GX (Hidden Fates Shiny Vault) were
    /// already in the pool and keep their scripts.
    /// </summary>
    public static class CardsBUS4
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fairy
            r.RegisterByKey("BUS-96", new RibombeeBUS());
            r.RegisterByKey("BUS-98", new ComboMon(new StrengthSapMon(), new ConditionAttackMon("Sleep Pulse", SpecialCondition.Asleep)));
            // Dragon
            r.RegisterByKey("BUS-100", new CoreEnforcerMon());
            // Colorless
            r.RegisterByKey("BUS-101", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("BUS-102", new CardsCES2.ScreechMon("Screech", 60));
            r.RegisterByKey("BUS-103", new CodeCheckMon());
            r.RegisterByKey("BUS-104", new CardsBST1.RadarMon("Calculate", 6));
            r.RegisterByKey("BUS-105", new PorygonZBUS());
            r.RegisterByKey("BUS-106", new SeeThroughMon());
            r.RegisterByKey("BUS-107", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new NightRaidMon()));
            r.RegisterByKey("BUS-108", new BouffalantBUS());
            r.RegisterByKey("BUS-110", new NoRetreatMon("Baby-Doll Eyes"));
            r.RegisterByKey("BUS-111", new BewearBUS());
            // Fighting (full art and rainbow rare only)
            r.RegisterByKey("BUS-136", new ComboMon(new CardsDRI2.CrunchMon("Crunch"), new CardsTWM1.SelfDiscardMon("Lycanfang-GX", 2)));
        }

        // ================================================================== Fairy

        /// <summary>Honey Gather: once during your turn, search your deck for up to 2 basic Energy cards and put them into your hand.</summary>
        private sealed class RibombeeBUS : Mon
        {
            protected override string Activated { get { return "Honey Gather"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Honey Gather: search your deck for up to 2 basic Energy cards");
            }
        }

        /// <summary>Strength Sap: heal 30 damage from this Pokémon for each Energy attached to the opponent's Active Pokémon.</summary>
        private sealed class StrengthSapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strength Sap") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int energy = defending == null ? 0 : ctx.EnergyCount(defending);
                ctx.DamageDefending(atk.Damage);
                if (energy > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30 * energy);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Core Enforcer: discard a Darkness Energy and a Fairy Energy from this Pokémon. As much as possible; an Energy that provides
        /// both types (Rainbow Energy) goes for Fairy when the Darkness can be paid by another (Compendium, 2017-09-21).
        /// </summary>
        private sealed class CoreEnforcerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Enforcer") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                var dark = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Darkness)).ToList();
                var plainDark = dark.Where(en => !CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fairy)).ToList();
                var forDark = plainDark.Count > 0 ? plainDark : dark;
                if (forDark.Count > 0)
                {
                    var pick = forDark.Count == 1 ? forDark[0] : ctx.ChooseCards(forDark, 1, 1, "Core Enforcer: discard which Darkness Energy?")[0];
                    ctx.Engine.DiscardEnergy(me, pick);
                }
                if (!ctx.InPlay(me)) return true;
                var fairy = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fairy)).ToList();
                if (fairy.Count > 0)
                {
                    var pick = fairy.Count == 1 ? fairy[0] : ctx.ChooseCards(fairy, 1, 1, "Core Enforcer: discard which Fairy Energy?")[0];
                    ctx.Engine.DiscardEnergy(me, pick);
                }
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Code Check: look at 1 of the opponent's face-down Prize cards.</summary>
        private sealed class CodeCheckMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Code Check") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var opp = ctx.Opp;
                var slots = opp.PrizeSlotList();
                var opts = new List<DecisionOption>();
                for (int k = 0; k < opp.Prizes.Count; k++)
                {
                    if (opp.FaceUpPrizeIds.Contains(opp.Prizes[k].Id)) continue;
                    opts.Add(new DecisionOption { Label = "Prize card " + (slots[k] + 1), Index = k });
                }
                if (opts.Count == 0) return true;
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Code Check: look at which of your opponent's face-down Prize cards?", opts, ctx.Source);
                var card = opp.Prizes[opts[pick].Index];
                var shown = new List<CardInstance> { card };
                ctx.Engine.NoteLookedAt(shown, "1 of your opponent's Prize cards", ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, shown, "Code Check", "1 of your opponent's Prize cards", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at 1 of " + ctx.Engine.Their(ctx.Opponent) + " Prize cards (Code Check).");
                return true;
            }
        }

        /// <summary>
        /// Initialize: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may devolve each of the opponent's
        /// evolved Pokémon by putting its highest Stage Evolution card into their hand. A Pokémon put into play unevolved (Eternal
        /// Flame-GX) isn't affected (Compendium, 2017-08-03). Zap Cannon: can't be used next turn.
        /// </summary>
        private sealed class PorygonZBUS : Mon
        {
            private static readonly Mon Zap = new LockAfterAttackMon("Zap Cannon", true);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.OpponentPokemon().Any(m => m.Stack.Count >= 2);
                if (!TriggerOnce(ctx, "Initialize", ready, "Use Initialize? Devolve each of your opponent's evolved Pokémon.")) return;
                foreach (var mon in ctx.OpponentPokemon(m => m.Stack.Count >= 2))
                    if (ctx.InPlay(mon)) ctx.Engine.Devolve(mon, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Zap.HandleAttack(ctx, atk); }
        }

        /// <summary>See Through: the opponent reveals their hand.</summary>
        private sealed class SeeThroughMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "See Through") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                RevealHand(ctx, "See Through");
                return true;
            }
        }

        /// <summary>Shows the opponent's hand to the attacking player and puts it in the log.</summary>
        internal static void RevealHand(EffectContext ctx, string source)
        {
            var hand = ctx.Opp.Hand.ToList();
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed " + ctx.Engine.Their(ctx.Opponent) + " hand (" + source + "): "
                + (hand.Count == 0 ? "no cards." : string.Join(", ", hand.Select(c => c.Def.Name).ToArray()) + "."));
            if (hand.Count > 0) ctx.Engine.NoteLookedAt(hand, "Your opponent's hand", ctx.Player);
        }

        /// <summary>Night Raid: the opponent reveals their hand; discard a Pokémon you find there.</summary>
        private sealed class NightRaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Raid") return false;
                ctx.DamageDefending(atk.Damage);
                RevealHand(ctx, "Night Raid");
                var pokemon = ctx.Opp.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return true;
                var pick = ctx.ChooseCards(pokemon, 1, 1, "Night Raid: discard which Pokémon from your opponent's hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                return true;
            }
        }

        /// <summary>Bouffant Head: takes 30 less damage next turn. Knock Over: you may discard any Stadium card in play.</summary>
        private sealed class BouffalantBUS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bouffant Head")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(30);
                    return true;
                }
                if (atk.Name != "Knock Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Knock Over: discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Mix-Up: flip a coin; heads, discard the top 3 cards of the opponent's deck. Tantrum: this Pokémon is now Confused.</summary>
        private sealed class BewearBUS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mix-Up")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                    return true;
                }
                if (atk.Name != "Tantrum") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
