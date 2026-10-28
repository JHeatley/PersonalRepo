using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BREAKthrough (BKT, v97), part 3: the Dragon and Colorless Pokémon (109-132), the Trainers and Burning Energy (133-151).
    /// Meowth (114), Doduo (116), Dodrio (117) and Swablu (124) are the same cards as Generations'; Fisherman, Judge, Skyla, Super Rod
    /// and Rainbow Energy were already in the pool.
    /// </summary>
    public static class CardsBKT3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Dragon
            r.RegisterByKey("BKT-109", new RecoilMon("Extra Chop", 10));
            r.RegisterByKey("BKT-110", new CardsMEW1.DefendingTypeBonusMon("Dragon Slayer", EnergyType.Dragon, 40));
            r.RegisterByKey("BKT-111", new ComboMon(new DragonDanceMon(), new CardsBRS1.MillSelfMon("Dragon Pulse", 3)));
            r.RegisterByKey("BKT-112", new ComboMon(new CardsEVS1.SynchrodrawMon("Tuning"), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("BKT-113", new CardsFST2.ConditionBonusMon("Synchro Woofer", 80, c => c.Me.Hand.Count == c.Opp.Hand.Count));
            // Colorless
            r.RegisterByKey("BKT-115", new ComboMon(new SimultaneousPeckMon(), new DrawAttackMon("Doduo Delivery", 2)));
            r.RegisterByKey("BKT-118", new SnorlaxBKT());
            r.RegisterByKey("BKT-119", new ProclaimTheNightMon());
            r.RegisterByKey("BKT-120", new HighFlightMon());
            r.RegisterByKey("BKT-122", new ComboMon(new CardsWHT.DragOffMon("Drag Off", 50), new CardsWHT.FlipMoreMon("Swing Around", 2, 40)));
            r.RegisterByKey("BKT-123", new SmeargleBKT());
            r.RegisterByKey("BKT-125", new CallForPalsMon());
            r.RegisterByKey("BKT-126", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("BKT-127", new ComboMon(new CycloneSlashMon(), new RecoilMon("Brave Bird", 20)));
            r.RegisterByKey("BKT-128", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new FlipBonusMon("Aerial Ace", 30)));
            r.RegisterByKey("BKT-130", new CoinsMon("Dual Cut", 2));
            r.RegisterByKey("BKT-131", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("BKT-132", new CardsASC.FlipDiscardDefendingEnergyMon("Mysterious Beam"));
            // Trainers
            r.RegisterByName("Assault Vest", new AssaultVest());
            r.RegisterByName("Brigette", new Brigette());
            r.RegisterByName("Buddy-Buddy Rescue", new BuddyBuddyRescue());
            r.RegisterByName("Float Stone", new FloatStone());
            r.RegisterByName("Giovanni's Scheme", new GiovannisScheme());
            r.RegisterByName("Glalie Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Heavy Ball", new HeavyBall());
            r.RegisterByName("Heavy Boots", new HeavyBoots());
            r.RegisterByName("Houndoom Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Mewtwo Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Parallel City", new ParallelCity());
            r.RegisterByName("Professor's Letter", new ProfessorsLetter());
            r.RegisterByName("Reserved Ticket", new ReservedTicket());
            r.RegisterByName("Town Map", new TownMap());
            r.RegisterByName("Burning Energy", new BurningEnergy());
        }

        // ================================================================== Dragon

        /// <summary>Dragon Dance: while this Haxorus stays your Active Pokémon, its attacks do 100 more damage to the opponent's Active Pokémon (before W/R); never more than 100.</summary>
        internal sealed class DragonDanceEffect : ContinuousEffect
        {
            public DragonDanceEffect(string name, int owner, int targetUid)
                : base(name, owner, targetUid, int.MaxValue, true) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || target == null || attacker.Uid != TargetUid || target.Owner == attacker.Owner || damage <= 0) return damage;
                if (!engine.State.IsActive(attacker) || !engine.State.IsActive(target)) return damage;
                return damage + 100;
            }
        }

        /// <summary>
        /// Dragon Dance: only a Haxorus gets the boost (Zoroark's Foul Play copying it does nothing: Compendium, 2015-10-29), and using
        /// it again adds nothing ("You can't add more than 100 damage in this way").
        /// </summary>
        private sealed class DragonDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Name != "Haxorus")
                {
                    ctx.Log("Dragon Dance: only Haxorus can use this effect, so nothing happens.");
                    return true;
                }
                int uid = me.Uid;
                if (ctx.Engine.ActiveEffects().OfType<DragonDanceEffect>().Any(fx => fx.TargetUid == uid))
                {
                    ctx.Log("Dragon Dance: " + me.Name + " already does 100 more damage, and that can't go higher.");
                    return true;
                }
                ctx.AddEffect(new DragonDanceEffect(me.Name + "'s attacks do 100 more damage while it stays your Active Pokémon (Dragon Dance)", ctx.Player, uid));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Simultaneous Peck: flip 2 coins; if either is tails, the attack does nothing.</summary>
        private sealed class SimultaneousPeckMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Simultaneous Peck") return false;
                if (ctx.FlipCoins(2) == 2) ctx.DamageDefending(atk.Damage);
                else ctx.Log("Simultaneous Peck: a coin came up tails, so the attack does nothing.");
                return true;
            }
        }

        /// <summary>Plump Body: 30 less damage from attacks (after W/R). Knock Away: flip a coin; heads, 30 more damage.</summary>
        private sealed class SnorlaxBKT : Mon
        {
            private static readonly Mon Knock = new FlipBonusMon("Knock Away", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || damage <= 0) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knock.HandleAttack(ctx, atk); }
        }

        /// <summary>Proclaim the Night: the opponent can't play Item cards from their hand during their next turn.</summary>
        private sealed class ProclaimTheNightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Proclaim the Night") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during " + ctx.Engine.Their(ctx.Opponent) + " next turn (Proclaim the Night).");
                return true;
            }
        }

        /// <summary>High Flight: each player reveals their hand; 20 damage for each Item card revealed (not Pokémon Tools: your ruling, v90).</summary>
        private sealed class HighFlightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Flight") return false;
                if (ctx.Me.Hand.Count > 0) ctx.Engine.Reveal(ctx.Player, ctx.Me.Hand.ToList(), "revealed " + ctx.Engine.Their(ctx.Player) + " hand (High Flight)");
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.Reveal(ctx.Opponent, ctx.Opp.Hand.ToList(), "revealed " + ctx.Engine.Their(ctx.Opponent) + " hand (High Flight)");
                int items = ctx.Me.Hand.Count(c => c.Def.IsItem) + ctx.Opp.Hand.Count(c => c.Def.IsItem);
                ctx.DamageDefending(atk.Damage * items);
                return true;
            }
        }

        /// <summary>Second Coat: once during your turn, switch a basic Energy card on your Active Pokémon with a basic Energy card of a different type from your discard pile.</summary>
        private sealed class SmeargleBKT : Mon
        {
            protected override string Activated { get { return "Second Coat"; } }

            private static List<CardInstance> Swappable(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return new List<CardInstance>();
                return active.Energy.Where(en => en.Def.IsBasicEnergy && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides != en.Def.EnergyProvides)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return Swappable(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var mine = Swappable(ctx);
                if (active == null || mine.Count == 0) return;
                var old = mine.Count == 1 ? mine[0] : ctx.ChooseCards(mine, 1, 1, "Second Coat: which basic Energy on " + active.Name + " do you switch?")[0];
                var options = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides != old.Def.EnergyProvides).ToList();
                if (options.Count == 0) return;
                var fresh = options.Count == 1 ? options[0] : ctx.ChooseCards(options, 1, 1, "Second Coat: attach which basic Energy from your discard pile instead of " + old.Def.Name + "?")[0];
                if (!ctx.Engine.DiscardEnergy(active, old)) return;
                if (ctx.Me.Discard.Remove(fresh)) ctx.Engine.AttachEnergyCard(fresh, active);
            }
        }

        /// <summary>Call for Pals: put any number of Starly from your deck onto your Bench.</summary>
        private sealed class CallForPalsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Pals") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.SearchToBench(d => d.Name == "Starly", ctx.BenchSpace, "Call for Pals: put any number of Starly onto your Bench");
                return true;
            }
        }

        /// <summary>Cyclone Slash: before the damage, the opponent switches their Active Pokémon; the damage happens even with nothing to switch (2015-11-19).</summary>
        private sealed class CycloneSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cyclone Slash") return false;
                ctx.OpponentSwitchesActive();
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Assault Vest: 40 less damage from attacks of the opponent's Pokémon that have any Special Energy attached (after W/R).</summary>
        private sealed class AssaultVest : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == me.Owner || damage <= 0) return damage;
                return a.Energy.Any(en => en.Def.IsAttachedSpecialEnergy) ? damage - 40 : damage;
            }
        }

        /// <summary>Brigette: put 1 Basic Pokémon-EX, or up to 3 Basic Pokémon that aren't Pokémon-EX, from your deck onto your Bench.</summary>
        private sealed class Brigette : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool anyEx = ctx.Me.Deck.Any(c => c.Def.IsBasicPokemon && c.Def.IsPokemonEX);
                bool anyOther = ctx.Me.Deck.Any(c => c.Def.IsBasicPokemon && !c.Def.IsPokemonEX);
                bool ex = anyEx && (!anyOther || ctx.ChooseOption("Brigette: what do you look for?",
                    "Up to 3 Basic Pokémon (not Pokémon-EX)", "1 Basic Pokémon-EX") == 1);
                if (ex) ctx.SearchToBench(d => d.IsPokemonEX, 1, "Brigette: choose 1 Basic Pokémon-EX to put onto your Bench");
                else ctx.SearchToBench(d => !d.IsPokemonEX, 3, "Brigette: choose up to 3 Basic Pokémon (not Pokémon-EX) to put onto your Bench");
            }
        }

        /// <summary>Buddy-Buddy Rescue: each player puts a Pokémon from their discard pile into their hand; the opponent chooses first.</summary>
        private sealed class BuddyBuddyRescue : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var theirs = ctx.Opp.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (theirs.Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Opponent))
                {
                    var pick = ctx.Engine.ChooseCards(ctx.Opponent, theirs, 1, 1, "Buddy-Buddy Rescue: put a Pokémon from your discard pile into your hand", ctx.Card.Def);
                    if (pick.Count > 0 && ctx.Opp.Discard.Remove(pick[0]))
                    {
                        ctx.Opp.Hand.Add(pick[0]);
                        ctx.Engine.Reveal(ctx.Opponent, pick, "put this Pokémon from " + ctx.Engine.Their(ctx.Opponent) + " discard pile into " + ctx.Engine.Their(ctx.Opponent) + " hand (Buddy-Buddy Rescue)");
                    }
                }
                var mine = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (mine.Count == 0) return;
                var own = mine.Count == 1 ? mine : ctx.ChooseCards(mine, 1, 1, "Buddy-Buddy Rescue: put a Pokémon from your discard pile into your hand");
                ctx.DiscardToHand(own);
            }
        }

        /// <summary>Float Stone: the Pokémon this card is attached to has no Retreat Cost.</summary>
        private sealed class FloatStone : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon ? 0 : cost;
            }
        }

        /// <summary>Giovanni's Scheme: draw until you have 5 cards, or your attacks do 20 more to the opponent's Active Pokémon this turn (the choice is locked: 2015-11-19).</summary>
        private sealed class GiovannisScheme : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool canDraw = ctx.Me.Hand.Count < 5 && ctx.Me.Deck.Count > 0;
                int pick = ctx.ChooseOption("Giovanni's Scheme: choose 1",
                    "Your Pokémon's attacks do 20 more damage to your opponent's Active Pokémon this turn",
                    "Draw cards until you have 5 cards in your hand" + (canDraw ? "" : " (you can't draw any)"));
                if (pick == 1)
                {
                    ctx.DrawUntil(5);
                    return;
                }
                ctx.AddEffect(new DamageBonusEffect("Giovanni's Scheme: your attacks do 20 more damage to the opponent's Active Pokémon this turn", ctx.Player, ctx.Turn, 20, false));
            }
        }

        /// <summary>Heavy Ball: search your deck for a Pokémon with a Retreat Cost of 3 or more (a BREAK card has none: 2015-11-19).</summary>
        private sealed class HeavyBall : CardScript
        {
            internal static bool Heavy(CardDef d) { return d.IsPokemon && !d.IsBreak && !d.IsVUnion && d.RetreatCost >= 3; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(Heavy, 1, "Heavy Ball: choose a Pokémon with a Retreat Cost of 3 or more");
            }
        }

        /// <summary>Heavy Boots: if its Pokémon's Retreat Cost is 3 or more (as it is now), +20 HP and it can't be Confused (and stops being Confused).</summary>
        private sealed class HeavyBoots : CardScript
        {
            [ThreadStatic] private static bool checking;

            private static bool Applies(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || checking) return false;
                checking = true;
                try { return src.Engine.RetreatCostOf(mon) >= 3; }
                finally { checking = false; }
            }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return Applies(src, mon) ? hp + 20 : hp; }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return condition == SpecialCondition.Confused && Applies(src, target);
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        /// <summary>The way Parallel City faces: Owner is the player whose Bench is limited to 3.</summary>
        internal sealed class ParallelCityFacing : StadiumFacingEffect
        {
            public ParallelCityFacing(int limitedSide, int stadiumId) : base("Parallel City", limitedSide, stadiumId, limitedSide) { }
        }

        /// <summary>
        /// Parallel City: the player who plays it chooses which way it faces. The player one half faces can't have more than 3 Benched
        /// Pokémon (they discard down to 3 when it comes into play); the other player's Grass, Fire and Water Pokémon's attacks do 20 less
        /// damage (before W/R). Another Parallel City can't replace it (same name: 2015-11-19).
        /// </summary>
        private sealed class ParallelCity : CardScript
        {
            public override void OnStadiumPlayed(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ParallelCityFacing);
                int pick = ctx.ChooseOption("Parallel City: which way does it face?",
                    "Your opponent can't have more than 3 Benched Pokémon; your Grass, Fire and Water Pokémon do 20 less damage",
                    "You can't have more than 3 Benched Pokémon; your opponent's Grass, Fire and Water Pokémon do 20 less damage");
                int side = pick == 0 ? ctx.Opponent : ctx.Player;
                ctx.State.Effects.Add(new ParallelCityFacing(side, ctx.Card.Id));
                ctx.Log("Parallel City: " + ctx.Engine.PlayerName(side) + " can't have more than 3 Benched Pokémon, and "
                    + ctx.Engine.PlayerName(1 - side) + "'s Grass, Fire and Water Pokémon do 20 less damage.");
                ctx.Engine.EnforceBenchLimits(side);
            }

            public override void OnLeavePlay(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ParallelCityFacing);
            }

            /// <summary>The player whose Bench is limited (the opponent of the one who played it, if it was put into play without choosing).</summary>
            private static int LimitedSide(EffectContext src)
            {
                foreach (var e in src.State.Effects)
                {
                    var facing = e as ParallelCityFacing;
                    if (facing != null && src.Card != null && facing.StadiumId == src.Card.Id) return facing.Owner;
                }
                return 1 - src.Player;
            }

            public override int CapMaxBench(EffectContext src, int player) { return player == LimitedSide(src) ? 3 : int.MaxValue; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || !info.AgainstOpponent || damage <= 0 || a.Owner == LimitedSide(src)) return damage;
                bool weakened = src.Engine.IsType(a, EnergyType.Grass) || src.Engine.IsType(a, EnergyType.Fire) || src.Engine.IsType(a, EnergyType.Water);
                return weakened ? damage - 20 : damage;
            }
        }

        /// <summary>Professor's Letter: search your deck for up to 2 basic Energy cards and put them into your hand.</summary>
        private sealed class ProfessorsLetter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Professor's Letter: choose up to 2 basic Energy cards");
            }
        }

        /// <summary>Reserved Ticket: flip a coin; heads, search your deck for a card, shuffle, and put that card on top.</summary>
        private sealed class ReservedTicket : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => true, 1, "Reserved Ticket: choose a card to put on top of your deck", false);
                ctx.ShuffleMyDeck();
                if (found.Count == 0) return;
                ctx.Me.Deck.Insert(0, found[0]);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card on top of " + ctx.Engine.Their(ctx.Player) + " deck (Reserved Ticket).");
            }
        }

        /// <summary>Town Map: turn all of your Prize cards face up for the rest of the game.</summary>
        private sealed class TownMap : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var p = ctx.Me;
                foreach (var prize in p.Prizes)
                    if (!p.FaceUpPrizeIds.Contains(prize.Id)) p.FaceUpPrizeIds.Add(prize.Id);
                if (p.Prizes.Count > 0) ctx.Engine.Reveal(ctx.Player, p.Prizes.ToList(), "turned " + ctx.Engine.Their(ctx.Player) + " Prize cards face up for the rest of the game (Town Map)");
            }
        }

        /// <summary>
        /// Burning Energy: only on a Fire Pokémon (discarded from anything else); provides Fire there. Discarded by an attack of that Fire
        /// Pokémon, it's attached to it again after the attack.
        /// </summary>
        private sealed class BurningEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fire)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Fire) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Fire); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fire); }

            public override bool ReturnsAfterAttackDiscard(EffectContext ctx)
            {
                return ctx.Pokemon != null && ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fire);
            }
        }
    }
}
