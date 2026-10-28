using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Destined Rivals (DRI) 1-70: the cards that weren't already in the pool. Pokémon whose attacks only do their
    /// printed damage need no script: Pineco, Shroomish, Fomantis, Growlithe, Ethan's Slugma, Team Rocket's Houndour,
    /// Buizel, Snover, Cetoddle, Electabuzz.
    /// </summary>
    public static class CardsDRI1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DRI-1", new EthansPinsirDRI());
            r.RegisterByKey("DRI-2", new WhirlwindMon("Whirlwind"));
            r.RegisterByKey("DRI-3", new YanmegaExDRI());
            r.RegisterByKey("DRI-6", new BreloomDRI());
            r.RegisterByKey("DRI-9", new RotomDRI());
            r.RegisterByKey("DRI-14", new LurantisDRI());
            r.RegisterByKey("DRI-15", new TeamRocketsBlipbugDRI());
            r.RegisterByKey("DRI-16", new CardsTWM1.HealSelfMon("Mini Drain", 10));
            r.RegisterByKey("DRI-17", new EnergyLoopMon("Energy Loop"));
            r.RegisterByKey("DRI-18", new HydrappleDRI());
            r.RegisterByKey("DRI-25", new RabscaExDRI());
            r.RegisterByKey("DRI-26", new OgerponDRI("Grass Kagura", EnergyType.Grass));
            r.RegisterByKey("DRI-28", new ArcanineDRI());
            r.RegisterByKey("DRI-29", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("DRI-30", new RapidashDRI());
            r.RegisterByKey("DRI-31", new TeamRocketsMoltresExDRI());
            r.RegisterByKey("DRI-32", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("DRI-33", new EthansQuilavaDRI());
            r.RegisterByKey("DRI-34", new EthansTyphlosionDRI());
            r.RegisterByKey("DRI-36", new EthansMagcargoDRI());
            r.RegisterByKey("DRI-38", new TeamRocketsHoundoomDRI());
            r.RegisterByKey("DRI-39", new EthansHoOhExDRI());
            r.RegisterByKey("DRI-42", new BlazikenDRI());
            r.RegisterByKey("DRI-43", new RotomDRI());
            r.RegisterByKey("DRI-44", new OgerponDRI("Fire Kagura", EnergyType.Fire));
            r.RegisterByKey("DRI-45", new MistysPsyduckDRI());
            r.RegisterByKey("DRI-46", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("DRI-47", new MistysStarmieDRI());
            r.RegisterByKey("DRI-48", new MistysMagikarpDRI());
            r.RegisterByKey("DRI-49", new MistysGyaradosDRI());
            r.RegisterByKey("DRI-50", new SearchAttackMon("Swim Together", d => d.IsOwnersPokemon("Misty"), 3));
            r.RegisterByKey("DRI-52", new DodgeMon("Undulate", false));
            r.RegisterByKey("DRI-53", new CynthiasMiloticDRI());
            r.RegisterByKey("DRI-54", new ShieldAttackMon("Shell Press", 10));
            r.RegisterByKey("DRI-55", new HuntailDRI());
            r.RegisterByKey("DRI-56", new GorebyssDRI());
            r.RegisterByKey("DRI-58", new FloatzelDRI());
            r.RegisterByKey("DRI-60", new AbomasnowDRI());
            r.RegisterByKey("DRI-61", new RotomDRI());
            r.RegisterByKey("DRI-62", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("DRI-63", new DodgeMon("Dive", false));
            r.RegisterByKey("DRI-65", new CetitanExDRI());
            r.RegisterByKey("DRI-66", new DondozoExDRI());
            r.RegisterByKey("DRI-67", new OgerponDRI("Water Kagura", EnergyType.Water));
            r.RegisterByKey("DRI-69", new ElectivireExDRI());
            r.RegisterByKey("DRI-70", new TeamRocketsZapdosDRI());
        }

        // ================================================================== Shared helpers (also used by CardsDRI2)

        /// <summary>The Energy card provides that type while attached to the Pokémon ("[R] Energy attached").</summary>
        public static bool ProvidesType(EffectContext ctx, PokemonInPlay mon, CardInstance energy, EnergyType type)
        {
            return ctx.Engine.EnergyUnitsOf(mon, energy).Any(u => EnergyMask.Provides(u, type));
        }

        /// <summary>Gadget Show: Pokémon Tools attached to all of your Pokémon.</summary>
        public static int ToolsInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => m.Tools.Count); }

        /// <summary>"Discard N [type] Energy cards from your hand" as an attack cost. Returns false (and discards nothing) if you can't.</summary>
        public static bool DiscardEnergyFromHand(EffectContext ctx, EnergyType type, int count, string attack)
        {
            var cards = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
            if (cards.Count < count) return false;
            var picks = cards.Count == count ? cards : ctx.ChooseCards(cards, count, count, attack + ": discard " + count + " Basic " + type + " Energy cards from your hand");
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
            return true;
        }

        /// <summary>Moves up to count Energy from this Pokémon to 1 of your Benched Pokémon (all of it if it has fewer).</summary>
        public static void MoveEnergyToBench(EffectContext ctx, int count, string attack)
        {
            var me = ctx.Pokemon;
            if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return;
            int n = Math.Min(count, me.Energy.Count);
            var picks = n >= me.Energy.Count ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), n, n, attack + ": choose " + n + " Energy to move");
            var target = ctx.ChoosePokemon(ctx.MyBench(), attack + ": move the Energy to which Benched Pokémon?");
            if (target == null) return;
            foreach (var e in picks) ctx.Engine.MoveEnergy(me, target, e);
        }

        /// <summary>"This attack does N damage to 2 of your opponent's Pokémon" (no Weakness or Resistance for Benched Pokémon).</summary>
        public static void DamageTwoOpponents(EffectContext ctx, int amount, string attack)
        {
            var all = ctx.OpponentPokemon();
            if (all.Count == 0) return;
            int n = Math.Min(2, all.Count);
            foreach (var t in ctx.ChoosePokemonMany(all, n, n, attack + ": choose " + n + " of your opponent's Pokémon to damage"))
                ctx.DamageTarget(t, amount);
        }

        /// <summary>"Look at 1 of your opponent's face-down Prize cards" (Team Rocket's Blipbug).</summary>
        public static void LookAtOpponentPrize(EffectContext ctx, string source)
        {
            var opp = ctx.Opp;
            var faceDown = opp.Prizes.Where(c => !opp.FaceUpPrizeIds.Contains(c.Id)).ToList();
            if (faceDown.Count == 0) return;
            var labels = new string[faceDown.Count];
            for (int i = 0; i < faceDown.Count; i++) labels[i] = "Face-down Prize card " + (i + 1);
            var card = faceDown[ctx.ChooseOption(source + ": look at which of your opponent's Prize cards?", labels)];
            ctx.Engine.NoteLookedAt(new List<CardInstance> { card }, "1 of your opponent's Prize cards", ctx.Player);
            ctx.ChooseOption(source + ": that Prize card is " + card.Def.Name + ".", "OK");
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at 1 of the other player's Prize cards.");
        }

        /// <summary>Whirlwind / Push Down: switch out the opponent's Active Pokémon (they choose the new one).</summary>
        public sealed class WhirlwindMon : Mon
        {
            private readonly string attack;
            private readonly bool optional;

            public WhirlwindMon(string attack) : this(attack, false) { }

            public WhirlwindMon(string attack, bool optional)
            {
                this.attack = attack;
                this.optional = optional;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || ctx.Opp.Bench.Count == 0) return true;
                if (optional && !ctx.AskYesNo(attack + ": switch out your opponent's Active Pokémon to the Bench?")) return true;
                ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>
        /// The masked Ogerpon: [type] Kagura searches for a Basic Energy of its type and attaches it to 1 of your Pokémon.
        /// The second attacks: Ogre's Hammer (can't use it next turn), Searing Flame (Burned), Bubble Drain (heal 30),
        /// Mountain Ramming (discard the top card of the opponent's deck).
        /// </summary>
        public sealed class OgerponDRI : Mon
        {
            private readonly string kagura;
            private readonly EnergyType type;

            public OgerponDRI(string kagura, EnergyType type)
            {
                this.kagura = kagura;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == kagura)
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, type), 1, kagura + ": search your deck for a Basic " + type + " Energy card");
                    if (found.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(), kagura + ": attach " + found[0].Def.Name + " to which of your Pokémon?");
                        if (target != null) ctx.Engine.AttachEnergyCard(found[0], target);
                        else ctx.Me.Deck.Add(found[0]);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                var defending = ctx.Defending;
                switch (atk.Name)
                {
                    case "Ogre's Hammer":
                        ctx.DamageDefending(atk.Damage);
                        ctx.SelfCantUseAttackNextTurn(atk.Name);
                        return true;
                    case "Searing Flame":
                        ctx.DamageDefending(atk.Damage);
                        if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                        return true;
                    case "Bubble Drain":
                        ctx.DamageDefending(atk.Damage);
                        if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                        return true;
                    case "Mountain Ramming":
                        ctx.DamageDefending(atk.Damage);
                        ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                        return true;
                }
                return false;
            }
        }

        // ================================================================== Grass

        /// <summary>Rallying Horn: +100 if any of your Ethan's Pokémon were Knocked Out by attack damage during the opponent's last turn.</summary>
        private sealed class EthansPinsirDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rallying Horn") return false;
                bool avenge = ctx.WasKnockedOutLastOpponentTurn(d => d.IsOwnersPokemon("Ethan"), true);
                ctx.DamageDefending(atk.Damage + (avenge ? 100 : 0));
                return true;
            }
        }

        /// <summary>Buzzing Boost: when it moves from the Bench to the Active Spot during your turn, you may attach up to 3 Basic Grass Energy from your deck to it. Jet Cyclone: move 3 Energy to 1 of your Benched Pokémon.</summary>
        private sealed class YanmegaExDRI : Mon
        {
            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Buzzing Boost", ctx.Me.Deck.Count > 0, "Buzzing Boost: search your deck for up to 3 Basic Grass Energy and attach them to " + ctx.Pokemon.Name + "?"))
                    return;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 3, "Buzzing Boost: choose up to 3 Basic Grass Energy cards");
                foreach (var e in found) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                MoveEnergyToBench(ctx, 3, atk.Name);
                return true;
            }
        }

        /// <summary>Damage Rush: flip until tails, +50 per heads. Mega Drain: heal 30.</summary>
        private sealed class BreloomDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Damage Rush")
                {
                    ctx.DamageDefending(atk.Damage + 50 * ctx.FlipUntilTails());
                    return true;
                }
                if (atk.Name == "Mega Drain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// The DRI Rotom forms. Gadget Show: 30 for each Pokémon Tool attached to your Pokémon. Trimming Mower: discard a
        /// Stadium in play. Singe: Burned. Manual Wash: heal 10 from each of your Pokémon. Astonish: a random card from the
        /// opponent's hand is shuffled into their deck.
        /// </summary>
        public sealed class RotomDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                switch (atk.Name)
                {
                    case "Gadget Show":
                        ctx.DamageDefending(atk.Damage * ToolsInPlay(ctx));
                        return true;
                    case "Trimming Mower":
                        ctx.DamageDefending(atk.Damage);
                        CardsTWM1.DiscardStadiumInPlay(ctx);
                        return true;
                    case "Singe":
                        if (defending != null) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                        return true;
                    case "Manual Wash":
                        ctx.DamageDefending(atk.Damage);
                        foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 10);
                        return true;
                    case "Astonish":
                        ctx.DamageDefending(atk.Damage);
                        CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 1);
                        return true;
                }
                return false;
            }
        }

        /// <summary>Petal Blade Dance: discard 2 Basic Grass Energy from your hand, or the attack does nothing.</summary>
        private sealed class LurantisDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Blade Dance") return false;
                if (!DiscardEnergyFromHand(ctx, EnergyType.Grass, 2, atk.Name)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Searching Eyes: look at 1 of your opponent's face-down Prize cards.</summary>
        private sealed class TeamRocketsBlipbugDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searching Eyes") return false;
                LookAtOpponentPrize(ctx, atk.Name);
                return true;
            }
        }

        /// <summary>Energy Loop: put an Energy attached to this Pokémon into your hand. Used for Energy Loop, Rolling Fireball (the attack name is passed in).</summary>
        internal sealed class EnergyLoopMon : Mon
        {
            private readonly string attack;

            public EnergyLoopMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                var pick = ctx.ChooseCards(me.Energy.ToList(), 1, 1, attack + ": put which Energy into your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(me, pick);
                return true;
            }
        }

        /// <summary>Hydra Breath: discard 6 Basic Grass Energy from your hand to Knock Out the opponent's Active Pokémon.</summary>
        private sealed class HydrappleDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydra Breath") return false;
                var defending = ctx.Defending;
                if (!DiscardEnergyFromHand(ctx, EnergyType.Grass, 6, atk.Name)) return Nothing(ctx);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, atk.Name);
                return true;
            }
        }

        /// <summary>Upside-Down Draw: draw 3 cards from the bottom of your deck. Psychic: +90 per Energy on the opponent's Active Pokémon.</summary>
        private sealed class RabscaExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Upside-Down Draw")
                {
                    var deck = ctx.Me.Deck;
                    int n = Math.Min(3, deck.Count);
                    var drawn = new List<CardInstance>();
                    for (int i = 0; i < n; i++)
                    {
                        var c = deck[deck.Count - 1];
                        deck.RemoveAt(deck.Count - 1);
                        drawn.Add(c);
                    }
                    ctx.Me.Hand.AddRange(drawn);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew " + n + (n == 1 ? " card" : " cards") + " from the bottom of the deck.");
                    return true;
                }
                if (atk.Name == "Psychic")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : 90 * ctx.EnergyCount(ctx.Defending)));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Punishing Fang: +100 against a Darkness Pokémon.</summary>
        private sealed class ArcanineDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Punishing Fang") return false;
                bool dark = ctx.Defending != null && ctx.Engine.IsType(ctx.Defending, EnergyType.Darkness);
                ctx.DamageDefending(atk.Damage + (dark ? 100 : 0));
                return true;
            }
        }

        /// <summary>Hurried Gait: once during your turn, draw a card.</summary>
        private sealed class RapidashDRI : Mon
        {
            protected override string Activated { get { return "Hurried Gait"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }
        }

        /// <summary>Flame Screen: takes 50 less next turn. Evil Incineration: discard a Team Rocket's Energy from it to discard the opponent's Active Pokémon.</summary>
        private sealed class TeamRocketsMoltresExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flame Screen")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(50);
                    return true;
                }
                if (atk.Name == "Evil Incineration")
                {
                    var defending = ctx.Defending;
                    var me = ctx.Pokemon;
                    var rocket = me.Energy.Where(e => e.Def.Name == "Team Rocket's Energy").ToList();
                    if (rocket.Count == 0) return Nothing(ctx);
                    if (!ctx.Engine.DiscardEnergy(me, rocket[0])) return true;
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.Engine.DiscardPokemonFromPlay(defending);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Bonded by the Journey: once during your turn, search your deck for an Ethan's Adventure.</summary>
        private sealed class EthansQuilavaDRI : Mon
        {
            protected override string Activated { get { return "Bonded by the Journey"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.Name == "Ethan's Adventure", 1, "Bonded by the Journey: search your deck for an Ethan's Adventure card");
            }
        }

        /// <summary>Buddy Blast: +60 for each Ethan's Adventure in your discard pile.</summary>
        private sealed class EthansTyphlosionDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Buddy Blast") return false;
                ctx.DamageDefending(atk.Damage + 60 * ctx.Me.Discard.Count(c => c.Def.Name == "Ethan's Adventure"));
                return true;
            }
        }

        /// <summary>Melt Away: no Retreat Cost without Energy attached. Lava Burst: discard up to 5 Fire Energy, 70 for each.</summary>
        private sealed class EthansMagcargoDRI : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Burst") return false;
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(e => ProvidesType(ctx, me, e, EnergyType.Fire)).ToList();
                int discarded = 0;
                if (fire.Count > 0)
                {
                    var picks = ctx.ChooseCards(fire, 0, Math.Min(5, fire.Count), "Lava Burst: discard up to 5 Fire Energy (70 damage for each)");
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(me, e)) discarded++;
                }
                ctx.DamageDefending(atk.Damage * discarded);
                return true;
            }
        }

        /// <summary>Cruel Coal: Burned and Confused. Scorching Fire: discard an Energy from this Pokémon.</summary>
        private sealed class TeamRocketsHoundoomDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cruel Coal")
                {
                    var defending = ctx.Defending;
                    if (defending == null) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Scorching Fire")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Golden Flame: once during your turn, attach up to 2 Basic Fire Energy from your hand to 1 of your Benched Ethan's Pokémon. Shining Feathers: heal 50 from each of your Pokémon.</summary>
        private sealed class EthansHoOhExDRI : Mon
        {
            protected override string Activated { get { return "Golden Flame"; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.Me.Bench.Where(m => m.Def.IsOwnersPokemon("Ethan") && ctx.Engine.CanAttachFromHand(m)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Targets(ctx), "Golden Flame: attach Fire Energy to which Benched Ethan's Pokémon?");
                if (target == null) return;
                var fire = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                var picks = ctx.ChooseCards(fire, 1, Math.Min(2, fire.Count), "Golden Flame: choose up to 2 Basic Fire Energy to attach to " + target.Name);
                foreach (var e in picks)
                {
                    if (!ctx.Me.Hand.Remove(e)) continue;
                    ctx.Engine.AttachEnergyCard(e, target, true);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shining Feathers") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 50);
                return true;
            }
        }

        /// <summary>Inferno Kick Flurry: also 120 to 1 of the opponent's Benched Pokémon; discard 2 Energy from this Pokémon.</summary>
        private sealed class BlazikenDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inferno Kick Flurry") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(120);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Flustered Leap: once during your turn, from the Bench, discard the bottom card of your deck to put this Pokémon on top of your deck (its attached cards are discarded).</summary>
        private sealed class MistysPsyduckDRI : Mon
        {
            protected override string Activated { get { return "Flustered Leap"; } }
            protected override bool AbilityReady(EffectContext ctx) { return !ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var deck = ctx.Me.Deck;
                var bottom = deck[deck.Count - 1];
                deck.RemoveAt(deck.Count - 1);
                ctx.Me.Discard.Add(bottom);
                ctx.Engine.Emit(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + bottom.Def.Name + " from the bottom of the deck.", bottom.Id);
                foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                ctx.Engine.DiscardAllTools(me);
                ctx.Engine.RemoveFromPlay(me);
                // Whatever is left attached goes to the discard pile; the Pokémon card(s) go on top of the deck.
                foreach (var e in me.Energy) ctx.Me.Discard.Add(e);
                me.Energy.Clear();
                var cards = me.Stack.ToList();
                cards.Reverse();
                deck.InsertRange(0, cards);
                ctx.Log(me.Name + " was put on top of " + ctx.Engine.Their(ctx.Player) + " deck.");
            }
        }

        /// <summary>Abrupt Flash: +80 if this Pokémon evolved from Misty's Staryu during this turn.</summary>
        private sealed class MistysStarmieDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Abrupt Flash") return false;
                var me = ctx.Pokemon;
                bool justEvolved = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Misty's Staryu";
                ctx.DamageDefending(atk.Damage + (justEvolved ? 80 : 0));
                return true;
            }
        }

        /// <summary>So Submerged: on the Bench, prevent all damage from and effects of the opponent's attacks done to it.</summary>
        private sealed class MistysMagikarpDRI : Mon
        {
            private static bool Submerged(EffectContext src, PokemonInPlay target)
            {
                return target != null && target == src.Pokemon && !src.IsActive(target);
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.AgainstOpponent && Submerged(src, info.Target) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Submerged(src, target);
            }
        }

        /// <summary>Splashing Panic: discard the top 7 cards of your deck, 70 for each Misty's Pokémon discarded.</summary>
        private sealed class MistysGyaradosDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splashing Panic") return false;
                var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 7);
                ctx.DamageDefending(atk.Damage * gone.Count(c => c.Def.IsOwnersPokemon("Misty")));
                return true;
            }
        }

        /// <summary>Aqua Split: also 30 to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class CynthiasMiloticDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Split") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var t in ctx.ChoosePokemonMany(bench, n, n, "Aqua Split: choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(t, 30);
                return true;
            }
        }

        /// <summary>Diver's Catch: when 1 of your Water Pokémon is Knocked Out by an opponent's attack, you may put its Basic Water Energy into your hand.</summary>
        private sealed class HuntailDRI : Mon
        {
            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                if (!byOpponentsAttack || !src.Engine.IsType(mon, EnergyType.Water)) return;
                var water = mon.Energy.Where(e => CardsSSP1.IsBasicOf(e.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return;
                if (!src.AskYesNo("Diver's Catch: put the " + water.Count + " Basic Water Energy attached to " + mon.Name + " into your hand?")) return;
                Announce(src, "Diver's Catch");
                foreach (var e in water) src.Engine.ReturnEnergyToHand(mon, e);
            }
        }

        /// <summary>Crescendo Wave: first you may attach any number of Basic Water Energy from your hand to it; 30 for each Water Energy attached.</summary>
        private sealed class GorebyssDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crescendo Wave") return false;
                var me = ctx.Pokemon;
                var water = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count > 0 && ctx.Engine.CanAttachFromHand(me))
                {
                    var picks = ctx.ChooseCards(water, 0, water.Count, "Crescendo Wave: attach any number of Basic Water Energy from your hand to " + me.Name);
                    foreach (var e in picks)
                        if (ctx.Me.Hand.Remove(e)) ctx.Engine.AttachEnergyCard(e, me, true);
                }
                ctx.DamageDefending(atk.Damage * ctx.Engine.EnergyCountOfType(me, EnergyType.Water));
                return true;
            }
        }

        /// <summary>Whirlpool: discard an Energy from the opponent's Active Pokémon. Aqua Slash: can't attack next turn.</summary>
        private sealed class FloatzelDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Whirlpool")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.DiscardEnergyFrom(defending, 1, "Whirlpool: discard an Energy from " + defending.Name);
                    return true;
                }
                if (atk.Name == "Aqua Slash")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Frozen Wood: +120 with 2 or more Grass Energy attached.</summary>
        private sealed class AbomasnowDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frozen Wood") return false;
                bool grass = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass) >= 2;
                ctx.DamageDefending(atk.Damage + (grass ? 120 : 0));
                return true;
            }
        }

        /// <summary>Snow Camouflage: Items and Supporters the opponent plays don't affect it. Crushing Press: you may discard a Stadium in play for 140 more.</summary>
        private sealed class CetitanExDRI : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && (trainer.IsItem || trainer.IsSupporter);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Press") return false;
                bool crush = ctx.State.Stadium != null && ctx.AskYesNo("Crushing Press: discard " + ctx.State.Stadium.Def.Name + " for 140 more damage?")
                             && CardsTWM1.DiscardStadiumInPlay(ctx);
                ctx.DamageDefending(atk.Damage + (crush ? 140 : 0));
                return true;
            }
        }

        /// <summary>Avenging Billow: +10 per damage counter on it. Dynamic Dive: you may do 120 more and 50 to itself.</summary>
        private sealed class DondozoExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Avenging Billow")
                {
                    ctx.DamageDefending(atk.Damage + 10 * ctx.Pokemon.DamageCounters);
                    return true;
                }
                if (atk.Name == "Dynamic Dive")
                {
                    bool dive = ctx.AskYesNo("Dynamic Dive: do 120 more damage (and 50 damage to Dondozo ex itself)?");
                    ctx.DamageDefending(atk.Damage + (dive ? 120 : 0));
                    if (dive) ctx.DamageSelf(50);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning

        /// <summary>Dual Bolt: 50 to 2 of the opponent's Pokémon. High-Voltage Press: +100 with 2 extra Energy.</summary>
        private sealed class ElectivireExDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dual Bolt")
                {
                    DamageTwoOpponents(ctx, 50, atk.Name);
                    return true;
                }
                if (atk.Name == "High-Voltage Press")
                {
                    ctx.DamageDefending(atk.Damage + (ExtraEnergy(ctx, atk, 2) ? 100 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Jamming Wing: you may move an Energy from the opponent's Active to 1 of their Benched Pokémon. Wicked Thunder: +60 with Team Rocket's Energy attached.</summary>
        private sealed class TeamRocketsZapdosDRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Jamming Wing")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                    if (!ctx.AskYesNo("Jamming Wing: move an Energy from " + defending.Name + " to 1 of your opponent's Benched Pokémon?")) return true;
                    var energy = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Jamming Wing: move which Energy?")[0];
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Jamming Wing: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(defending, target, energy);
                    return true;
                }
                if (atk.Name == "Wicked Thunder")
                {
                    bool rocket = ctx.Pokemon.Energy.Any(e => e.Def.Name == "Team Rocket's Energy");
                    ctx.DamageDefending(atk.Damage + (rocket ? 60 : 0));
                    return true;
                }
                return false;
            }
        }
    }
}
