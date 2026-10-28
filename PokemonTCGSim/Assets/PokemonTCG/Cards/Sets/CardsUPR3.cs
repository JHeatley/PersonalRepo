using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ultra Prism (UPR, v85), part 3: Fighting, Darkness, Metal, Fairy and Dragon (UPR 64-101). Riolu, Lucario, Magnemite (80),
    /// Magneton (82), Magnezone, Gible (96), Gabite and Garchomp were already in the pool (Hidden Fates Shiny Vault) and keep their
    /// scripts. Heatran, Dialga-GX and Palkia-GX have the same attacks as their Forbidden Light prints and reuse those scripts.
    /// </summary>
    public static class CardsUPR3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("UPR-65", new ComboMon(new CardsPAL1.EvolutionBonusMon("Clean Hit", 60), new CardsFST1.OppressingSandstormMon("Wild Crash")));
            r.RegisterByKey("UPR-68", new CardsFST2.ConditionBonusMon("Eleventh Hour Tackle", 130, c => c.Me.Deck.Count <= 3));
            r.RegisterByKey("UPR-69", new ComboMon(new NoRetreatMon("Sand Tomb"), new CardsOBF1.RetreatCostDamageMon("Dust Cannon", 10, false)));
            r.RegisterByKey("UPR-70", new PassimianUPR());
            // Darkness
            r.RegisterByKey("UPR-71", new NoRetreatMon("Mean Look"));
            r.RegisterByKey("UPR-72", new RipAndRunMon());
            r.RegisterByKey("UPR-73", new SneaselUPR());
            r.RegisterByKey("UPR-74", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep),
                new CardsASR1.TimesMon("Evil Admonition", c => c.OpponentPokemon(m => c.Engine.HasAbility(m)).Count)));
            r.RegisterByKey("UPR-75", new GasBondMon());
            r.RegisterByKey("UPR-76", new SmokescreenMon("Sticky Smokescreen", 2));
            r.RegisterByKey("UPR-77", new CardsUPR2.StarAttachMon("Nightmare Star", EnergyType.Darkness, new AbyssalSleepMon("Abyssal Sleep")));
            // Metal
            r.RegisterByKey("UPR-78", new CardsASR1.TimesMon("Iron Head", c => c.FlipUntilTails()));
            r.RegisterByKey("UPR-79", new GoldRushMon());
            r.RegisterByKey("UPR-81", new MagnemiteUPR());
            r.RegisterByKey("UPR-84", new RecoilMon("Take Down", 10));
            r.RegisterByKey("UPR-85", new BastiodonUPR());
            r.RegisterByKey("UPR-86", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("UPR-87", new ComboMon(new ParalyzeFlipMon("Psy Bolt"),
                new CardsFST2.ConditionBonusMon("Psychic Resonance", 60, c => c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Psychic)).Count > 0)));
            CardsUPR1.Same(r, "UPR-88", "FLI-13");
            r.RegisterByKey("UPR-89", new ComboMon(new CardsUPR2.StarEnergyMon("Radiant Star", EnergyType.Metal), new LockAfterAttackMon("Corona Impact", false)));
            r.RegisterByKey("UPR-90", new DuskManeNecrozmaGX());
            r.RegisterByKey("UPR-91", new MagearnaUPR());
            // Fairy
            r.RegisterByKey("UPR-92", new ConditionAttackMon("Flickering Spores", SpecialCondition.Asleep));
            r.RegisterByKey("UPR-93", new ShiinoticUPR());
            r.RegisterByKey("UPR-94", new ComboMon(new CardsASR1.TimesMon("Psywave", CardsLOT2.DefendingEnergy), new MagicalSwapMon()));
            // Dragon
            r.RegisterByKey("UPR-95", new AlolanExeggutorUPR());
            r.RegisterByKey("UPR-97", new GibleUPR());
            CardsUPR1.Same(r, "UPR-100", "FLI-82");
            CardsUPR1.Same(r, "UPR-101", "FLI-20");
        }

        // ================================================================== Shared helpers

        /// <summary>"If the Defending Pokémon tries to attack during your opponent's next turn, your opponent flips N coins. If any of them is tails, that attack does nothing" (Weak Kneed, Sticky Smokescreen).</summary>
        internal sealed class SmokescreenMon : Mon
        {
            private readonly string attack;
            private readonly int coins;

            public SmokescreenMon(string attack, int coins)
            {
                this.attack = attack;
                this.coins = coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsCRI.AttackCoinsEffect(defending.Name + " must flip " + coins + (coins == 1 ? " coin" : " coins") + " to attack during its next turn (" + attack + ")",
                    ctx.Player, defending.Uid, ctx.Turn + 1, coins));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Power Huddle: while this Pokémon is on your Bench, your Passimian's attacks do 30 more damage to the opponent's Active
        /// Evolution Pokémon (before Weakness and Resistance); each Benched Passimian adds 30. Rock Hurl: not affected by Resistance.
        /// </summary>
        private sealed class PassimianUPR : Mon
        {
            private static readonly Mon Hurl = new CardsFLF1.NoResistanceMon("Rock Hurl");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || src.PokemonIsActive || attacker == null || attacker.Owner != src.Player || attacker.Def.Name != "Passimian") return damage;
                if (!info.ToOpponentsActive || !info.Target.Def.IsEvolution || damage <= 0) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hurl.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Rip and Run: discard a random card from the opponent's hand, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class RipAndRunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rip and Run") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Sneaky Smash: only if you went second, on your first turn; discard an Energy from 1 of the opponent's Pokémon. Ambush: flip for 20 more.</summary>
        private sealed class SneaselUPR : Mon
        {
            private static readonly Mon Ambush = new FlipBonusMon("Ambush", 20);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Sneaky Smash" || CardsUNB1.SecondPlayersFirstTurn(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Smash") return Ambush.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Sneaky Smash: discard an Energy from which of your opponent's Pokémon?");
                if (target == null) return true;
                var pick = target.Energy.Count == 1 ? target.Energy.ToList() : ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Sneaky Smash: discard which Energy?");
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(target, pick[0]);
                return true;
            }
        }

        /// <summary>Gas Bond: both Active Pokémon are now Confused.</summary>
        private sealed class GasBondMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gas Bond") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Abyssal Sleep: the opponent's Active Pokémon is now Asleep, and while that Asleep lasts its owner flips 2 coins between turns instead of 1; it wakes up only if both are heads (as Altaria's Soothing Lullaby, your ruling). Used for Abyssal Sleep, Soothing Lullaby (the attack name is passed in).</summary>
        internal sealed class AbyssalSleepMon : Mon
        {
            private readonly string attack;

            public AbyssalSleepMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Asleep)) defending.SleepCoins = 2;
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Gold Rush: discard any number of Metal Energy cards from your hand; 30 damage for each card discarded.</summary>
        private sealed class GoldRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gold Rush") return false;
                var metal = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Metal)).ToList();
                int n = 0;
                if (metal.Count > 0)
                    foreach (var card in ctx.ChooseCards(metal, 0, metal.Count, "Gold Rush: discard any number of Metal Energy cards from your hand (30 damage each)"))
                    {
                        if (!ctx.Me.Hand.Contains(card)) continue;
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Gold Rush did no damage.");
                return true;
            }
        }

        /// <summary>Solid Unit: as long as this Pokémon is on your Bench, prevent all damage done to it by attacks (yours and the opponent's).</summary>
        private sealed class MagnemiteUPR : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.State.IsActive(me);
            }
        }

        /// <summary>
        /// Earthen Shield: prevent all damage done to your Metal Pokémon by attacks from the opponent's Pokémon that have any Special
        /// Energy attached. Push Down: you may have the opponent switch their Active Pokémon with 1 of their Benched Pokémon.
        /// </summary>
        private sealed class BastiodonUPR : Mon
        {
            private static readonly Mon Push = new CardsASC.PushDownMon("Push Down");

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                var attacker = info.Attacker;
                if (src.Pokemon == null || target == null || attacker == null || target.Owner != src.Player || attacker.Owner == src.Player) return false;
                return src.Engine.IsType(target, EnergyType.Metal) && attacker.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Push.HandleAttack(ctx, atk); }
        }

        /// <summary>Meteor Tempest: discard 3 Energy from this Pokémon. Sun's Eclipse-GX: only with more Prize cards left than the opponent.</summary>
        private sealed class DuskManeNecrozmaGX : Mon
        {
            private static readonly Mon Tempest = new CardsTWM1.SelfDiscardMon("Meteor Tempest", 3);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Sun's Eclipse-GX" || CardsUPR2.BehindOnPrizes(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tempest.HandleAttack(ctx, atk); }
        }

        /// <summary>Change Clothes: once during your turn, you may put a Pokémon Tool attached to 1 of your Pokémon into your hand.</summary>
        private sealed class MagearnaUPR : Mon
        {
            protected override string Activated { get { return "Change Clothes"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon(m => m.Tools.Count > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holder = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Tools.Count > 0), "Change Clothes: put a Pokémon Tool from which of your Pokémon into your hand?");
                if (holder == null || holder.Tools.Count == 0) return;
                var tool = holder.Tools.Count == 1 ? holder.Tools[0] : ctx.ChooseCards(holder.Tools.ToList(), 1, 1, "Change Clothes: which Pokémon Tool?")[0];
                if (!holder.Tools.Remove(tool)) return;
                ctx.Me.Hand.Add(tool);
                ctx.Log(tool.Def.Name + " went from " + holder.Name + " into " + ctx.Engine.Their(ctx.Player) + " hand (Change Clothes).");
            }
        }

        // ================================================================== Fairy

        /// <summary>Illuminate: once during your turn, you may search your deck for a Fairy Pokémon and put it into your hand. Flickering Spores: Asleep.</summary>
        private sealed class ShiinoticUPR : Mon
        {
            private static readonly Mon Spores = new ConditionAttackMon("Flickering Spores", SpecialCondition.Asleep);

            protected override string Activated { get { return "Illuminate"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Fairy), 1, "Illuminate: choose a Fairy Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spores.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Magical Swap: move any number of damage counters on the opponent's Pokémon to their other Pokémon in any way you like.
        /// Generations' Meowstic (Ear Influence, v95) uses it under its own name.
        /// </summary>
        internal sealed class MagicalSwapMon : Mon
        {
            private readonly string attack;

            public MagicalSwapMon() : this("Magical Swap") { }

            public MagicalSwapMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int round = 0; round < 12; round++)
                {
                    var damaged = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                    if (damaged.Count == 0 || ctx.OpponentPokemon().Count < 2) break;
                    if (!ctx.AskYesNo(round == 0 ? attack + ": move damage counters between your opponent's Pokémon?" : attack + ": move more damage counters?")) break;
                    var source = ctx.ChoosePokemon(damaged, attack + ": move damage counters from which of your opponent's Pokémon?");
                    if (source == null) break;
                    var dest = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != source), attack + ": move them to which of your opponent's Pokémon?");
                    if (dest == null) break;
                    int n = ctx.AskNumber(1, source.DamageCounters, attack + ": move how many damage counters?");
                    ctx.Engine.MoveDamageCounters(source, dest, n, attack);
                }
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Exeggutor's Paradise: for each of your Benched Exeggcute, search your deck for an Alolan Exeggutor or Alolan Exeggutor-GX
        /// and evolve that Exeggcute. Draco Meteor Barrage: flip a coin for each Grass Energy on this Pokémon; 80 damage for each heads.
        /// </summary>
        private sealed class AlolanExeggutorUPR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Exeggutor's Paradise")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var eggs = ctx.MyBench().Where(m => m.Name == "Exeggcute" && !CardsEVS1.EvolveBlocked(ctx, m)).ToList();
                    if (eggs.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                    foreach (var egg in eggs)
                    {
                        if (ctx.Me.Deck.Count == 0) break;
                        var basic = egg;
                        var found = ctx.SearchMyDeck(d => (d.Name == "Alolan Exeggutor" || d.Name == "Alolan Exeggutor-GX") && CardsEVS1.EvolvesOnto(ctx, basic, d), 1,
                            "Exeggutor's Paradise: choose an Alolan Exeggutor or Alolan Exeggutor-GX for " + basic.Name, false);
                        if (found.Count > 0) ctx.Engine.EvolvePokemon(basic, found[0]);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name != "Draco Meteor Barrage") return false;
                int coins = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                else ctx.Log(ctx.Pokemon.Name + "'s Draco Meteor Barrage did no damage.");
                return true;
            }
        }

        /// <summary>Rock Hiding: if this Pokémon has any Fighting Energy attached, it has no Retreat Cost.</summary>
        private sealed class GibleUPR : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return mon.Energy.Any(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Fighting)) ? 0 : cost;
            }
        }
    }
}
