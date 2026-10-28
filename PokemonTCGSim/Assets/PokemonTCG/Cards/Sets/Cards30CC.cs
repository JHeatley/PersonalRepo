using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// 30th Celebration's Classic Collection (30C CC1-CC30): reprints of classic cards. CC1-CC17 first came out before
    /// Black & White, so they're Unlimited only; CC18-CC30 (Black & White to Paldea Evolved) are Expanded.
    ///
    /// Older mechanics are handled by the engine: Pokémon Powers, Poké-Powers and Poké-Bodies work as Abilities, LV.X
    /// only goes onto the Active Pokémon and keeps the attacks of the Pokémon under it (BREAK too), LEGEND halves are
    /// played together, GX attacks and VSTAR Powers are once per game, the Lost Zone, Mega Evolution Pokémon-EX end the
    /// turn, and Prize counts (Pokémon-ex, EX, GX, V, VSTAR, LEGEND: 2; VMAX and TAG TEAM: 3).
    /// </summary>
    public static class Cards30CC
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("30C-CC1", new PikachuCC());
            r.RegisterByKey("30C-CC2", new CharizardCC());
            r.RegisterByName("Misty", new MistyCC());
            r.RegisterByKey("30C-CC4", new ErikasJigglypuffCC());
            r.RegisterByKey("30C-CC5", new SneaselCC());
            r.RegisterByKey("30C-CC6", new ShiningCelebiCC());
            r.RegisterByKey("30C-CC7", new LugiaCC());
            r.RegisterByKey("30C-CC8", new DelcattyCC());
            r.RegisterByKey("30C-CC9", new DarkTyranitarCC());
            r.RegisterByKey("30C-CC10", new ScizorExCC());
            r.RegisterByKey("30C-CC11", new MetagrossCC());
            r.RegisterByKey("30C-CC12", new PalkiaLvXCC());
            r.RegisterByKey("30C-CC13", new UxieCC());
            r.RegisterByKey("30C-CC14", new CrobatGCC());
            r.RegisterByKey("30C-CC15", new GengarCC());
            r.RegisterByKey("30C-CC16", new DarkraiCresseliaLegendCC());
            r.RegisterByKey("30C-CC17", new DarkraiCresseliaLegendCC());
            r.RegisterByName("N", new NCC());
            r.RegisterByKey("30C-CC19", new RayquazaExCC());
            r.RegisterByKey("30C-CC20", new GenesectExCC());
            r.RegisterByKey("30C-CC21", new MGardevoirExCC());
            r.RegisterByKey("30C-CC22", new GreninjaBreakCC());
            r.RegisterByKey("30C-CC23", new SolgaleoGxCC());
            r.RegisterByKey("30C-CC24", new BuzzwoleGxCC());
            r.RegisterByKey("30C-CC25", new PikachuZekromGxCC());
            r.RegisterByKey("30C-CC26", new ZacianVCC());
            r.RegisterByKey("30C-CC27", new BenchHitMon("Amazing Shot", 120));
            r.RegisterByKey("30C-CC28", new MewVmaxCC());
            // 30C-CC29 Arceus VSTAR is a printing of Crown Zenith GG70 / Brilliant Stars 123 and uses its script (v69).
            r.RegisterByKey("30C-CC30", new CardsPRC1.KnockoutNeedleMon("Expert Splasher", 20));
        }

        /// <summary>Old Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        private static bool NoCondition(PokemonInPlay mon)
        {
            return mon != null && mon.Conditions == SpecialCondition.None;
        }

        // ================================================================== Unlimited (before Black & White)

        /// <summary>Thunder Jolt: flip a coin; if tails, 10 damage to itself.</summary>
        private sealed class PikachuCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Jolt") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Energy Burn (Pokémon Power): all its Energy is Fire for the turn. Fire Spin: discard 2 Energy to use it.</summary>
        private sealed class CharizardCC : Mon
        {
            protected override string Activated { get { return "Energy Burn"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me.Has(SpecialCondition.Asleep) || me.Has(SpecialCondition.Confused) || me.Has(SpecialCondition.Paralyzed)) return false;
                return me.Energy.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Log("Energy Burn: every Energy attached to " + ctx.Pokemon.Name + " is Fire Energy for the rest of the turn.");
            }

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder != src.Pokemon || !src.AbilityUsedThisTurn("Energy Burn")) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Spin") return false;
                ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Fire Spin: discard 2 Energy cards attached to Charizard");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Misty (Gym Heroes): discard 2 other cards; this turn, "Misty" attackers do 20 more damage when they do damage.</summary>
        private sealed class MistyCC : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count < 2) return;
                var picks = ctx.ChooseCards(others, 2, 2, "Misty: discard 2 other cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.AddEffect(new MistyBoostEffect(ctx.Player, ctx.Turn));
                ctx.Log("This turn, attacks by Pokémon with \"Misty\" in their name do 20 more damage.");
            }
        }

        private sealed class MistyBoostEffect : ContinuousEffect
        {
            public MistyBoostEffect(int owner, int turn) : base("Misty", owner, -1, turn, false)
            {
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (damage <= 0 || info.Attacker == null || info.Attacker.Owner != Owner || !info.ToOpponentsActive) return damage;
                return info.Attacker.Name.Contains("Misty") ? damage + 20 : damage;
            }
        }

        /// <summary>Group Therapy: each player heals 1 counter from each of their damaged Pokémon. Pulled Punch: 40, or 10 against a damaged Pokémon.</summary>
        private sealed class ErikasJigglypuffCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Group Therapy")
                {
                    foreach (var m in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                        if (m.DamageCounters > 0) ctx.Heal(m, 10);
                    return true;
                }
                if (atk.Name == "Pulled Punch")
                {
                    var d = ctx.Defending;
                    ctx.DamageDefending(d != null && d.DamageCounters > 0 ? 10 : atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fury Swipes: 3 coins, 10 each. Beat Up: a coin per Pokémon you have in play, 20 each.</summary>
        private sealed class SneaselCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fury Swipes")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                    return true;
                }
                if (atk.Name == "Beat Up")
                {
                    int coins = ctx.MyPokemon().Count;
                    ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Healing Water: heal a Benched Pokémon 1 counter per Water Energy. Miracle Leaf: coins per Defending Energy; any heads = your choice of condition.</summary>
        private sealed class ShiningCelebiCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Healing Water")
                {
                    int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                    var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                    if (n <= 0 || hurt.Count == 0) return true;
                    var target = ctx.ChoosePokemon(hurt, "Healing Water: remove damage counters from which Benched Pokémon?");
                    ctx.Heal(target, 10 * Math.Min(n, target.DamageCounters));
                    return true;
                }
                if (atk.Name == "Miracle Leaf")
                {
                    var defending = ctx.Defending;
                    int coins = defending == null ? 0 : defending.Energy.Count;
                    int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                    ctx.DamageDefending(atk.Damage);
                    if (heads > 0 && TefPokemon.StillDefending(ctx, defending))
                    {
                        int pick = ctx.ChooseOption("Miracle Leaf: the Defending Pokémon becomes...", "Asleep", "Confused", "Poisoned");
                        var condition = pick == 0 ? SpecialCondition.Asleep : pick == 1 ? SpecialCondition.Confused : SpecialCondition.Poisoned;
                        ctx.ApplyCondition(defending, condition);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>Crystal Type (Poké-Body): attaching a basic Fire, Water or Psychic Energy from the hand makes it that type for the turn.</summary>
        private sealed class LugiaCC : Mon
        {
            private const string Mark = "Crystal Type";
            private const string TypeMark = "Crystal Type (type)";

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || !energy.Def.IsBasicEnergy) return;
                var t = energy.Def.EnergyProvides;
                if (t != EnergyType.Fire && t != EnergyType.Water && t != EnergyType.Psychic) return;
                src.Pokemon.AbilityUsedOnTurn[Mark] = src.Turn;
                src.Pokemon.AbilityUsedOnTurn[TypeMark] = (int)t;
                src.Log("Crystal Type: " + src.Pokemon.Name + " is " + t + " type until the end of the turn.");
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon != src.Pokemon) return;
                int turn, type;
                if (!mon.AbilityUsedOnTurn.TryGetValue(Mark, out turn) || turn != src.Turn) return;
                if (!mon.AbilityUsedOnTurn.TryGetValue(TypeMark, out type)) return;
                types.Clear();
                types.Add((EnergyType)type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Psychic")
                {
                    var d = ctx.Defending;
                    ctx.DamageDefending(atk.Damage * (d == null ? 0 : d.Energy.Count));
                    return true;
                }
                if (atk.Name == "Steam Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Steam Blast: discard an Energy card attached to Lugia");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Energy Draw (Poké-Power): discard an Energy card from the hand, draw up to 3. Max Energy Source: 10 per Energy on your Active.</summary>
        private sealed class DelcattyCC : Mon
        {
            protected override string Activated { get { return "Energy Draw"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                // Old "discard, then draw up to 3" wording: the discard is the first step, not a cost, and drawing 0 is
                // allowed, so it works with an empty deck (Compendium: Swampert Power Draw / Zoroark-GX Trade, 2019-03-21).
                // Modern "you must discard ... in order to use this Ability" cards need a card in the deck (Liepard Trade, 2022).
                return NoCondition(ctx.Pokemon) && ctx.Me.Hand.Any(c => c.Def.IsEnergy);
            }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                var pick = ctx.ChooseCards(energy, 1, 1, "Energy Draw: discard an Energy card from your hand");
                foreach (var c in pick) ctx.Engine.DiscardFromHand(ctx.Player, c);
                int max = Math.Min(3, ctx.Me.Deck.Count);
                if (max > 0) ctx.Draw(ctx.AskNumber(0, max, "Energy Draw: draw how many cards (up to 3)?"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Energy Source") return false;
                var active = ctx.Me.Active;
                ctx.DamageDefending(atk.Damage * (active == null ? 0 : ctx.EnergyCount(active)));
                return true;
            }
        }

        /// <summary>Grind: +10 per Energy on it. Spinning Tail: 20 to each of the opponent's Pokémon. Bite Off: +50 against a Pokémon-ex.</summary>
        private sealed class DarkTyranitarCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                switch (atk.Name)
                {
                    case "Grind":
                        ctx.DamageDefending(atk.Damage + 10 * ctx.EnergyCount(ctx.Pokemon));
                        return true;
                    case "Spinning Tail":
                        ctx.DamageDefending(atk.Damage);
                        foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, atk.Damage);
                        return true;
                    case "Bite Off":
                    {
                        var d = ctx.Defending;
                        ctx.DamageDefending(atk.Damage + (d != null && d.Def.IsOldEx ? 50 : 0));
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>Danger Perception (Poké-Body): at 60 HP or less, +40 damage. Steel Wing: takes 20 less next turn. Cross-Cut: +30 against an Evolved Pokémon.</summary>
        private sealed class ScizorExCC : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.RemainingHp(src.Pokemon) <= 60 ? damage + 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Steel Wing")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(20);
                    return true;
                }
                if (atk.Name == "Cross-Cut")
                {
                    var d = ctx.Defending;
                    bool evolved = d != null && d.Stack.Count > 1 && !d.Def.IsFossil && !d.Def.IsMultiCard;
                    ctx.DamageDefending(atk.Damage + (evolved ? 30 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Delta Control (Poké-Power): top 4, 1 into the hand, the rest to the bottom. Crush and Burn: +20 per Energy discarded from your Pokémon.</summary>
        private sealed class MetagrossCC : Mon
        {
            protected override string Activated { get { return "Delta Control"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return;
                var pick = ctx.ChooseCards(top, 1, 1, "Delta Control: put 1 of these cards into your hand");
                RecifeTrainers.TakeIntoHand(ctx, pick);
                foreach (var c in top)
                {
                    if (pick.Contains(c) || !ctx.Me.Deck.Remove(c)) continue;
                    ctx.Me.Deck.Add(c);
                }
                if (top.Count > pick.Count) ctx.Log("Delta Control: " + (top.Count - pick.Count) + " cards went to the bottom of the deck.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush and Burn") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var m in ctx.MyPokemon())
                    foreach (var e in m.Energy) holders[e] = m;
                int discarded = 0;
                if (holders.Count > 0)
                {
                    var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Crush and Burn: discard any Energy cards attached to your Pokémon (20 more damage each)");
                    foreach (var e in picks)
                        if (ctx.Engine.DiscardEnergy(holders[e], e)) discarded++;
                }
                ctx.DamageDefending(atk.Damage + 20 * discarded);
                return true;
            }
        }

        /// <summary>Restructure (Poké-Power): the opponent switches in 1 of your Benched Pokémon, then you switch in 1 of theirs. Hydro Reflect: may move all its Energy to the Bench.</summary>
        private sealed class PalkiaLvXCC : Mon
        {
            protected override string Activated { get { return "Restructure"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && (ctx.GustTargets().Count > 0 || ctx.Me.Bench.Count > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Bench.Count > 0 && ctx.Me.Active != null)
                {
                    var mine = ctx.Engine.ChoosePokemon(ctx.Opponent, ctx.Me.Bench.ToList(), "Restructure: choose which of your opponent's Benched Pokémon becomes their Active Pokémon", ctx.Source);
                    ctx.Engine.SwitchActive(ctx.Player, mine);
                }
                ctx.GustOpponent("Restructure: choose 1 of your opponent's Benched Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Reflect") return false;
                ctx.DamageDefending(atk.Damage);
                var self = ctx.Pokemon;
                if (!ctx.InPlay(self) || self.Energy.Count == 0 || ctx.Me.Bench.Count(b => b != self) == 0) return true;
                if (!ctx.AskYesNo("Hydro Reflect: move all Energy attached to " + self.Name + " to your Benched Pokémon?")) return true;
                foreach (var e in self.Energy.ToList())
                {
                    var targets = ctx.MyBench().Where(b => b != self).ToList();
                    if (targets.Count == 0) break;
                    var to = ctx.ChoosePokemon(targets, "Hydro Reflect: move " + e.Def.Name + " to which Benched Pokémon?");
                    ctx.Engine.MoveEnergy(self, to, e);
                }
                return true;
            }
        }

        /// <summary>Set Up (Poké-Power): benched from the hand, draw until 7. Psychic Restore: may put itself and its cards on the bottom of the deck.</summary>
        private sealed class UxieCC : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Set Up", ctx.Me.Hand.Count < 7 && ctx.Me.Deck.Count > 0, "Set Up: draw cards until you have 7 cards in your hand?"))
                    ctx.DrawUntil(7);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Restore") return false;
                ctx.DamageDefending(atk.Damage);
                var self = ctx.Pokemon;
                if (!ctx.InPlay(self) || !ctx.AskYesNo("Psychic Restore: put " + self.Name + " and all cards attached to it on the bottom of your deck?")) return true;
                var cards = self.AllCards();
                ctx.Engine.RemoveFromPlay(self);
                ctx.Me.Deck.AddRange(cards);
                ctx.Log(self.Name + " and " + (cards.Count - 1) + " attached cards went to the bottom of the deck.");
                return true;
            }
        }

        /// <summary>Flash Bite (Poké-Power): benched from the hand, 1 damage counter on an opponent's Pokémon. Toxic Fang: Poisoned, 2 counters a checkup.</summary>
        private sealed class CrobatGCC : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Flash Bite", ctx.OpponentPokemon().Count > 0, "Flash Bite: put 1 damage counter on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Flash Bite: put 1 damage counter on which Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toxic Fang") return false;
                var d = ctx.Defending;
                if (d != null && ctx.ApplyCondition(d, SpecialCondition.Poisoned)) d.PoisonCounterBase = 2;
                return true;
            }
        }

        /// <summary>Catastrophe (Poké-Body): while Active, the opponent's Knocked Out Pokémon go to the Lost Zone. Hurl into Darkness, Cursed Drop.</summary>
        private sealed class GengarCC : Mon
        {
            public override int LostZoneOnKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon.Owner == src.Player || !src.State.IsActive(src.Pokemon)) return 0;
                return 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hurl into Darkness")
                {
                    int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Psychic);
                    ctx.Engine.Reveal(ctx.Opponent, ctx.Opp.Hand, "showed their hand to Hurl into Darkness");
                    var pokemon = ctx.Opp.Hand.Where(c => c.Def.IsPokemon).ToList();
                    if (n <= 0 || pokemon.Count == 0) return true;
                    var picks = ctx.ChooseCards(pokemon, 0, Math.Min(n, pokemon.Count), "Hurl into Darkness: put up to " + n + " Pokémon from your opponent's hand into the Lost Zone");
                    foreach (var c in picks)
                    {
                        if (!ctx.Opp.Hand.Remove(c)) continue;
                        ctx.Opp.LostZone.Add(c);
                    }
                    if (picks.Count > 0) ctx.Log("Hurl into Darkness put " + string.Join(", ", picks.Select(c => c.Def.Name).ToArray()) + " in the Lost Zone.");
                    return true;
                }
                if (atk.Name == "Cursed Drop")
                {
                    ctx.PlaceCountersAnyWay(4, () => ctx.OpponentPokemon());
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Darkrai & Cresselia LEGEND (both halves). Lost Crisis: 2 of its Energy to the Lost Zone; a Pokémon Knocked Out by
        /// the damage goes to the Lost Zone with everything attached. Moon's Invite: move the opponent's damage counters around.
        /// </summary>
        private sealed class DarkraiCresseliaLegendCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lost Crisis")
                {
                    var self = ctx.Pokemon;
                    var energy = self.Energy.ToList();
                    var lost = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Lost Crisis: put 2 Energy attached to this Pokémon in the Lost Zone");
                    foreach (var e in lost)
                    {
                        if (!self.Energy.Remove(e)) continue;
                        ctx.Me.LostZone.Add(e);
                    }
                    if (lost.Count > 0) ctx.Log("Lost Crisis put " + string.Join(", ", lost.Select(c => c.Def.Name).ToArray()) + " in the Lost Zone.");
                    var d = ctx.Defending;
                    if (d != null) d.LostZoneKnockOutTurn = ctx.Turn;
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name == "Moon's Invite")
                {
                    for (int round = 0; round < 6; round++)
                    {
                        var damaged = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                        if (damaged.Count == 0 || ctx.OpponentPokemon().Count < 2) break;
                        if (round > 0 && !ctx.AskYesNo("Moon's Invite: move more damage counters?")) break;
                        var source = ctx.ChoosePokemon(damaged, "Moon's Invite: move damage counters from which of your opponent's Pokémon?");
                        var to = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != source), "Moon's Invite: move them to which of your opponent's Pokémon?");
                        if (source == null || to == null) break;
                        int n = ctx.AskNumber(1, source.DamageCounters, "Moon's Invite: move how many damage counters?");
                        ctx.Engine.MoveDamageCounters(source, to, n, "Moon's Invite");
                    }
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Expanded (Black & White to G)

        /// <summary>N: each player shuffles their hand into their deck and draws a card per remaining Prize card.</summary>
        private sealed class NCC : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent }) ctx.Engine.ShuffleHandIntoDeck(player);
                foreach (int player in new[] { ctx.Player, ctx.Opponent }) ctx.Engine.DrawCards(player, ctx.State.Players[player].Prizes.Count);
            }
        }

        /// <summary>Celestial Roar: discard the top 3, attach the Energy among them. Dragon Burst: discard all basic Fire or all basic Lightning, 60 each.</summary>
        private sealed class RayquazaExCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Celestial Roar")
                {
                    var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                    foreach (var e in discarded.Where(c => c.Def.IsEnergy).ToList())
                    {
                        if (!ctx.InPlay(ctx.Pokemon) || !ctx.Me.Discard.Remove(e)) continue;
                        ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                    }
                    return true;
                }
                if (atk.Name == "Dragon Burst")
                {
                    var self = ctx.Pokemon;
                    var fire = self.Energy.Where(e => e.Def.IsBasicEnergy && e.Def.EnergyProvides == EnergyType.Fire).ToList();
                    var lightning = self.Energy.Where(e => e.Def.IsBasicEnergy && e.Def.EnergyProvides == EnergyType.Lightning).ToList();
                    List<CardInstance> chosen;
                    if (fire.Count == 0) chosen = lightning;
                    else if (lightning.Count == 0) chosen = fire;
                    else chosen = ctx.ChooseOption("Dragon Burst: discard which Energy?", "All basic Fire Energy (" + fire.Count + ")", "All basic Lightning Energy (" + lightning.Count + ")") == 0 ? fire : lightning;
                    int n = 0;
                    foreach (var e in chosen) if (ctx.Engine.DiscardEnergy(self, e)) n++;
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Red Signal: attaching a Plasma Energy from the hand lets you switch in an opponent's Benched Pokémon. Megalo Cannon: 20 to a Benched Pokémon.</summary>
        private sealed class GenesectExCC : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || energy.Def.Name != "Plasma Energy" || src.GustTargets().Count == 0) return;
                if (src.AskYesNo("Red Signal: switch 1 of your opponent's Benched Pokémon with their Active Pokémon?")) src.GustOpponent("Red Signal: choose the Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megalo Cannon") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Brilliant Arrow: 30 per Fairy Energy on all your Pokémon. (Mega Evolving ends the turn: engine.)</summary>
        private sealed class MGardevoirExCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brilliant Arrow") return false;
                int n = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Fairy));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Giant Water Shuriken: while Active, discard a Water Energy card from the hand for 6 damage counters on an opponent's Pokémon.</summary>
        private sealed class GreninjaBreakCC : Mon
        {
            protected override string Activated { get { return "Giant Water Shuriken"; } }

            private static bool WaterCard(CardInstance c)
            {
                return c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Water;
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Me.Hand.Any(WaterCard) && ctx.OpponentPokemon().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var water = ctx.Me.Hand.Where(WaterCard).ToList();
                if (water.Count == 0) return;
                var pick = ctx.ChooseCards(water, 1, 1, "Giant Water Shuriken: discard a Water Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Giant Water Shuriken: put 6 damage counters on which Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 6);
            }
        }

        /// <summary>Ultra Road: switch your Active with a Benched Pokémon. Sunsteel Strike: discard all its Energy. Sol Burst-GX: up to 5 Energy from the deck, attached any way.</summary>
        private sealed class SolgaleoGxCC : Mon
        {
            protected override string Activated { get { return "Ultra Road"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SwitchMyActive("Ultra Road: choose a Benched Pokémon to switch with your Active Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sunsteel Strike")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                    return true;
                }
                if (atk.Name == "Sol Burst-GX")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsEnergy, 5, "Sol Burst-GX: choose up to 5 Energy cards to attach");
                    if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (c, m) => true, "Sol Burst-GX", false);
                    foreach (var c in found)
                        if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c)) && !ctx.Me.Deck.Contains(c)) ctx.Me.Deck.Add(c);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Jet Punch: 30 to a Benched Pokémon too. Knuckle Impact: can't attack next turn. Absorption-GX: 40 per remaining Prize card.</summary>
        private sealed class BuzzwoleGxCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                switch (atk.Name)
                {
                    case "Jet Punch":
                        ctx.DamageDefending(atk.Damage);
                        ctx.DamageOneBenched(30);
                        return true;
                    case "Knuckle Impact":
                        ctx.DamageDefending(atk.Damage);
                        ctx.SelfCantAttackNextTurn();
                        return true;
                    case "Absorption-GX":
                        ctx.DamageDefending(atk.Damage * ctx.Me.Prizes.Count);
                        return true;
                }
                return false;
            }
        }

        /// <summary>Full Blitz: up to 3 Lightning Energy from the deck onto 1 Pokémon. Tag Bolt-GX: with 3 extra Lightning Energy, 170 to a Benched Pokémon.</summary>
        private sealed class PikachuZekromGxCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Full Blitz")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning, 3, "Full Blitz: choose up to 3 Lightning Energy cards");
                    var target = found.Count > 0 ? ctx.ChoosePokemon(ctx.MyPokemon(), "Full Blitz: attach them to which Pokémon?") : null;
                    foreach (var e in found)
                    {
                        if (target != null) ctx.Engine.AttachEnergyCard(e, target);
                        else ctx.Me.Deck.Add(e);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Tag Bolt-GX")
                {
                    // At least 3 extra Lightning Energy beyond the attack's own 3: 6 Lightning in all.
                    bool extra = ctx.Engine.CanPayCost(ctx.Pokemon, Enumerable.Repeat(EnergyType.Lightning, 6).ToList());
                    ctx.DamageDefending(atk.Damage);
                    if (extra) ctx.DamageOneBenched(170);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Intrepid Sword: top 3, attach the Metal Energy, the rest into the hand, and the turn ends. Brave Blade: can't attack next turn.</summary>
        private sealed class ZacianVCC : Mon
        {
            protected override string Activated { get { return "Intrepid Sword"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                var metal = top.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Metal).ToList();
                var attach = metal.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(metal, 0, metal.Count, "Intrepid Sword: attach any Metal Energy cards to " + ctx.Pokemon.Name);
                foreach (var e in attach)
                {
                    if (!ctx.Me.Deck.Remove(e)) continue;
                    ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                }
                var rest = top.Where(c => !attach.Contains(c)).ToList();
                RecifeTrainers.TakeIntoHand(ctx, rest);
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brave Blade") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Amazing Shot: 120 to a Benched Pokémon too. Used for Amazing Shot, Overrun, Fire Soul (the attack name and the numbers are passed in).</summary>
        internal sealed class BenchHitMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchHitMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>Cross Fusion Strike: use a Benched Fusion Strike Pokémon's attack. Max Miracle: ignores effects on the Defending Pokémon.</summary>
        private sealed class MewVmaxCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cross Fusion Strike")
                {
                    // v55: Fusion Strike's Pokémon are in the pool. Any attack of a Benched Fusion Strike Pokémon, without paying
                    // its cost (Compendium, 2021-11-11). Another Cross Fusion Strike isn't offered: it would only choose again.
                    var opts = new List<DecisionOption>();
                    foreach (var benched in ctx.MyBench())
                    {
                        if (!benched.Def.IsFusionStrike) continue;
                        for (int i = 0; i < benched.Def.Attacks.Count; i++)
                        {
                            var a = benched.Def.Attacks[i];
                            if (a.Name == "Cross Fusion Strike") continue;
                            if (a.VstarPower && ctx.Me.VstarUsed) continue;
                            if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                            if (ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                            opts.Add(new DecisionOption { Label = benched.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = benched.Def, Index = i });
                        }
                    }
                    if (opts.Count == 0)
                    {
                        ctx.Log("Cross Fusion Strike: you have no Benched Fusion Strike Pokémon with an attack to use.");
                        return true;
                    }
                    int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Cross Fusion Strike: choose an attack of 1 of your Benched Fusion Strike Pokémon", opts, ctx.Source);
                    UseCopiedAttack(ctx, opts[pick].Card, opts[pick].Index);
                    return true;
                }
                if (atk.Name == "Max Miracle")
                {
                    ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                return false;
            }
        }

    }
}
