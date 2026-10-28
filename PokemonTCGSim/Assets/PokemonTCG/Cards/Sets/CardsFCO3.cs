using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Fates Collide (FCO, v94), part 3: the Fairy, Dragon and Colorless Pokémon (65-89), the Trainers and Strong Energy. The
    /// three Spirit Links (Alakazam, Altaria, Audino) use Evolutions' (GameEngine.SpiritLinkFor).
    /// </summary>
    public static class CardsFCO3
    {
        internal static readonly string[] Fossils = { "Helix Fossil Omanyte", "Dome Fossil Kabuto", "Old Amber Aerodactyl" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FCO-66", new ComboMon(new CardsASC.SelfGuardMon("Expand", 30), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("FCO-67", new ComboMon(new CardsUNB3.MimicMon(), new CoinsMon("Juggling", 4)));
            r.RegisterByKey("FCO-68", new TailsNothingMon("Ruckus"));
            r.RegisterByKey("FCO-69", new MistPurgeMon());
            r.RegisterByKey("FCO-71", new CardsFST1.MoveEnergyToBenchMon("Light Steps"));
            r.RegisterByKey("FCO-72", new DiancieEX());
            r.RegisterByKey("FCO-73", new ComboMon(new CardsRCL1.DiscardStadiumAttackMon("Big Storm"), new DragonTrailMon()));
            r.RegisterByKey("FCO-74", new TurmoilStrikeMon());
            r.RegisterByKey("FCO-76", new CardsASR1.DiscardDefendingSpecialMon("Jet Draft", false));
            r.RegisterByKey("FCO-77", new SnorlaxFCO());
            r.RegisterByKey("FCO-78", new LugiaFCO());
            r.RegisterByKey("FCO-79", new CardsTWM1.SelfDiscardMon("Flash of Destruction", 2));
            r.RegisterByKey("FCO-80", new CardsBLK.RoundMon());
            r.RegisterByKey("FCO-81", new FlipBonusMon("Stomp", 40));
            r.RegisterByKey("FCO-82", new ComboMon(new FlipBonusMon("Ambush", 40), new CacophonyMon()));
            r.RegisterByKey("FCO-83", new ComboMon(new PowerfulGainMon(), new ShiningWindMon()));
            r.RegisterByKey("FCO-84", new ComboMon(new CardsTWM1.HealSelfMon("Drain Slap", 20), new DoTheWaveMon("Do the Wave", 10)));
            r.RegisterByKey("FCO-85", new MagicalSymphonyMon());
            r.RegisterByKey("FCO-86", new CardsUNM3.CleaningUpMon());
            r.RegisterByKey("FCO-88", new ComboMon(new CovetMon(), new TailsNothingMon("Last Resort")));
            r.RegisterByKey("FCO-89", new ComboMon(new SweepingCureMon(), new FlipBonusMon("Knock Away", 30)));

            foreach (var link in new[] { "Alakazam Spirit Link", "Altaria Spirit Link", "Audino Spirit Link" })
                r.RegisterByName(link, new CardsEVO3.SpiritLink());
            r.RegisterByName("Bent Spoon", new BentSpoon());
            r.RegisterByName("Chaos Tower", new ChaosTower());
            r.RegisterByName("Dome Fossil Kabuto", new CardsSTS3.FossilItem("Kabuto"));
            r.RegisterByName("Helix Fossil Omanyte", new CardsSTS3.FossilItem("Omanyte"));
            r.RegisterByName("Old Amber Aerodactyl", new CardsSTS3.FossilItem("Aerodactyl"));
            r.RegisterByName("Energy Pouch", new EnergyPouch());
            r.RegisterByName("Energy Reset", new EnergyReset());
            r.RegisterByName("Fairy Drop", new FairyDrop());
            r.RegisterByName("Fairy Garden", new FairyGarden());
            r.RegisterByName("Fossil Excavation Kit", new FossilExcavationKit());
            r.RegisterByName("Lass's Special", new LassesSpecial());
            r.RegisterByName("Mega Catcher", new MegaCatcher());
            r.RegisterByName("Power Memory", new PowerMemory());
            r.RegisterByName("Random Receiver", new RandomReceiver());
            r.RegisterByName("Scorched Earth", new ScorchedEarth());
            r.RegisterByName("Team Rocket's Handiwork", new TeamRocketsHandiwork());
            r.RegisterByName("Strong Energy", new StrongEnergy());
        }

        // ================================================================== Fairy / Dragon

        /// <summary>Mist Purge: if this Pokémon has any Special Energy attached, 30 more damage and heal 30 damage from each of your Pokémon.</summary>
        private sealed class MistPurgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mist Purge") return false;
                bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? 30 : 0));
                if (!special) return true;
                foreach (var mon in ctx.MyPokemon())
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, 30);
                return true;
            }
        }

        /// <summary>
        /// Sparkle Veil: while this Pokémon is your Active Pokémon, damage done to your Pokémon by the opponent's attacks is 30 less
        /// (after Weakness and Resistance). Wonder Stage: 50 more if any Stadium is in play.
        /// </summary>
        private sealed class DiancieEX : Mon
        {
            private static readonly Mon WonderStage = new CardsFST2.ConditionBonusMon("Wonder Stage", 50, CardsFCO2.StadiumInPlay);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                var a = info.Attacker;
                if (src.Pokemon == null || !src.PokemonIsActive || t == null || t.Owner != src.Player || a == null || a.Owner == src.Player) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WonderStage.HandleAttack(ctx, atk); }
        }

        /// <summary>Dragon Trail: 30 more for each basic Lightning Energy attached to this Pokémon.</summary>
        private sealed class DragonTrailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Trail") return false;
                int basicLightning = ctx.Pokemon.Energy.Count(e => e.Def.IsBasicEnergy && e.Def.EnergyProvides == EnergyType.Lightning);
                ctx.DamageDefending(atk.Damage + 30 * basicLightning);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Turmoil Strike: 50 damage to 1 of the opponent's Pokémon that has any damage counters on it (no W/R for Benched Pokémon).</summary>
        private sealed class TurmoilStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turmoil Strike") return false;
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Turmoil Strike: do 50 damage to which of your opponent's damaged Pokémon?");
                if (target != null) ctx.DamageTarget(target, 50);
                return true;
            }
        }

        /// <summary>
        /// Toss and Turn: usable even while this Pokémon is Asleep, and 90 more if it is (only this attack: the other one still
        /// can't be used while Asleep). Swallow: heal as much as the damage done.
        /// </summary>
        private sealed class SnorlaxFCO : Mon
        {
            private static readonly Mon Swallow = new CardsDAA1.DrainMon("Swallow");

            // "This attack can be used even if this Pokémon is Asleep" (not while Paralyzed): the engine's hook for that (Dream Waltz).
            public override bool UsableWhileAsleep(AttackDef attack) { return attack != null && attack.Name == "Toss and Turn"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toss and Turn") return Swallow.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Has(SpecialCondition.Asleep) ? 90 : 0));
                return true;
            }
        }

        /// <summary>
        /// Pressure: while this Pokémon is your Active Pokémon, the opponent's Active Pokémon's attacks do 20 less damage to your
        /// Pokémon (before Weakness and Resistance; Benched ones too: Compendium, 2018-07-26). Intensifying Burn: 60 more against a
        /// Pokémon-EX.
        /// </summary>
        private sealed class LugiaFCO : Mon
        {
            private static readonly Mon Burn = new CardsFST2.ConditionBonusMon("Intensifying Burn", 60, c => c.Defending != null && c.Defending.Def.IsPokemonEX);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (info.Target == null || info.Target.Owner != src.Player || !src.State.IsActive(info.Attacker)) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burn.HandleAttack(ctx, atk); }
        }

        /// <summary>Cacophony: at the end of the opponent's next turn, the Defending Pokémon and all cards attached to it are discarded (no Prize card).</summary>
        private sealed class CacophonyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cacophony") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsPBL.DiscardDefendingLaterEffect(defending.Name + " will be discarded at the end of " + ctx.Engine.Possessive(ctx.Opponent) + " next turn (Cacophony)",
                    ctx.Player, defending.Uid, ctx.Turn, "Cacophony"));
                return true;
            }
        }

        /// <summary>Powerful Gain: if this Pokémon was healed during this turn, 60 more damage and heal 30 damage from it.</summary>
        private sealed class PowerfulGainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Powerful Gain") return false;
                var me = ctx.Pokemon;
                bool healed = me.HealedOnTurn == ctx.Turn;
                ctx.DamageDefending(atk.Damage + (healed ? 60 : 0));
                if (healed && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30);
                return true;
            }
        }

        /// <summary>Shining Wind: during the opponent's next turn, this Pokémon has no Weakness (an effect of the attack: Pokémon Ranger removes it).</summary>
        internal sealed class ShiningWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shining Wind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new CardsSSP3.NoWeaknessEffect(ctx.Pokemon.Name + " has no Weakness during the opponent's next turn (Shining Wind)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Do the Wave: 10 more for each of your Benched Pokémon. Used for Do the Wave, Line Force, Linked Lightning, Adventuring Together (the attack name and the numbers are passed in).</summary>
        internal sealed class DoTheWaveMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public DoTheWaveMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + bonus * ctx.Me.Bench.Count);
                return true;
            }
        }

        /// <summary>Magical Symphony: if you played a Supporter card from your hand this turn, also 50 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class MagicalSymphonyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Symphony") return false;
                bool played = ctx.Me.SupporterThisTurn != null;
                ctx.DamageDefending(atk.Damage);
                if (played) ctx.DamageOneBenched(50);
                return true;
            }
        }

        /// <summary>Covet: the opponent reveals their hand; put a card from it on the bottom of their deck.</summary>
        private sealed class CovetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Covet") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsTWM1.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                var card = ctx.ChooseCards(hand, 1, 1, "Covet: put which card from your opponent's hand on the bottom of their deck?")[0];
                if (ctx.Opp.Hand.Remove(card))
                {
                    ctx.Opp.Deck.Add(card);
                    ctx.Log(card.Def.Name + " from " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand went on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Covet).");
                }
                return true;
            }
        }

        /// <summary>Sweeping Cure: heal 90 damage from 1 of your Benched Pokémon.</summary>
        private sealed class SweepingCureMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweeping Cure") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Sweeping Cure: heal 90 damage from which of your Benched Pokémon?");
                if (target != null) ctx.Heal(target, 90);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Bent Spoon: prevent all effects of the opponent's attacks, except damage, done to the holder (existing effects stay).</summary>
        private sealed class BentSpoon : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>The way Chaos Tower faces: Owner is the player the "can't be Confused or Poisoned" half points at.</summary>
        internal sealed class ChaosTowerFacing : StadiumFacingEffect
        {
            // The printed card's upright half is the "can't be Asleep or Paralyzed" one (your card image, v97), so the card is
            // turned towards the other player.
            public ChaosTowerFacing(int confusedPoisonedSide, int stadiumId) : base("Chaos Tower", confusedPoisonedSide, stadiumId, 1 - confusedPoisonedSide) { }
        }

        /// <summary>
        /// Chaos Tower: the player who plays it chooses which way it faces. The Pokémon of the player one half points at can't be
        /// Confused or Poisoned; the other player's can't be Asleep or Paralyzed (those conditions already there are removed).
        /// </summary>
        private sealed class ChaosTower : CardScript
        {
            public override bool CuresConditionsItPrevents { get { return true; } }

            public override void OnStadiumPlayed(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ChaosTowerFacing);
                int pick = ctx.ChooseOption("Chaos Tower: which way does it face?",
                    "Your Pokémon can't be Confused or Poisoned; your opponent's can't be Asleep or Paralyzed",
                    "Your Pokémon can't be Asleep or Paralyzed; your opponent's can't be Confused or Poisoned");
                int side = pick == 0 ? ctx.Player : ctx.Opponent;
                ctx.State.Effects.Add(new ChaosTowerFacing(side, ctx.Card.Id));
                ctx.Log("Chaos Tower: " + ctx.Engine.PlayerName(side) + "'s Pokémon can't be Confused or Poisoned, and "
                    + ctx.Engine.PlayerName(1 - side) + "'s can't be Asleep or Paralyzed.");
            }

            public override void OnLeavePlay(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ChaosTowerFacing);
            }

            /// <summary>The player facing the Confused / Poisoned half (the one who played it, if it was put in play without choosing).</summary>
            private static int ConfusedPoisonedSide(EffectContext src)
            {
                foreach (var e in src.State.Effects)
                {
                    var facing = e as ChaosTowerFacing;
                    if (facing != null && src.Card != null && facing.StadiumId == src.Card.Id) return facing.Owner;
                }
                return src.Player;
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                if (target == null) return false;
                bool cpSide = target.Owner == ConfusedPoisonedSide(src);
                if (condition == SpecialCondition.Confused || condition == SpecialCondition.Poisoned) return cpSide;
                if (condition == SpecialCondition.Asleep || condition == SpecialCondition.Paralyzed) return !cpSide;
                return false;
            }
        }

        /// <summary>
        /// Energy Pouch: if the holder is Knocked Out by damage from the opponent's attack, its basic Energy goes into your hand. With
        /// an Exp. Share on another of your Pokémon, you choose which goes first (Compendium, 2017-04-20).
        /// </summary>
        private sealed class EnergyPouch : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var holder = src.Pokemon;
                if (!byOpponentsAttack || holder == null) return;
                var basic = holder.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basic.Count == 0) return;
                var sharers = !holder.HitWhileActive ? new List<PokemonInPlay>()
                    : src.MyPokemon(m => m != holder && !src.Engine.IsKnockedOut(m) && m.Tools.Any(t => t.Def.Name == "Exp. Share" && src.Engine.ToolWorks(m, t)));
                if (sharers.Count > 0 && src.ChooseOption("Energy Pouch and Exp. Share: which goes first?",
                        "Exp. Share first (1 basic Energy moves to the Pokémon with Exp. Share)", "Energy Pouch first (all basic Energy to your hand)") == 0)
                {
                    var sharer = src.ChoosePokemon(sharers, "Exp. Share: move a basic Energy to which Pokémon?");
                    var pick = src.ChooseCards(basic, 1, 1, "Exp. Share: move which Energy?")[0];
                    if (sharer != null && src.Engine.MoveEnergy(holder, sharer, pick)) basic.Remove(pick);
                    if (basic.Count == 0) return;
                }
                foreach (var card in basic)
                    if (holder.Energy.Remove(card)) src.Me.Hand.Add(card);
                src.Log("Energy Pouch: " + basic.Count + " basic Energy from " + holder.Name + " went into " + src.Engine.Their(src.Player) + " hand.");
            }
        }

        /// <summary>Energy Reset: put any number of Energy attached to your Pokémon into your hand.</summary>
        private sealed class EnergyReset : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.Energy.Count > 0); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var attached = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon()) attached.AddRange(mon.Energy);
                if (attached.Count == 0) return;
                var picks = ctx.ChooseCards(attached, 1, attached.Count, "Energy Reset: put which Energy attached to your Pokémon into your hand?");
                foreach (var card in picks)
                {
                    var holder = ctx.MyPokemon().FirstOrDefault(m => m.Energy.Contains(card));
                    if (holder != null) ctx.Engine.ReturnEnergyToHand(holder, card);
                }
            }
        }

        /// <summary>Fairy Drop: heal 50 damage from 1 of your Pokémon that has any Fairy Energy attached.</summary>
        private sealed class FairyDrop : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.HasEnergyOfType(m, EnergyType.Fairy));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Fairy Drop: heal 50 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 50);
            }
        }

        /// <summary>Fairy Garden: each Pokémon (both players') with any Fairy Energy attached has no Retreat Cost.</summary>
        private sealed class FairyGarden : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && src.Engine.HasEnergyOfType(mon, EnergyType.Fairy) ? 0 : cost;
            }
        }

        /// <summary>Fossil Excavation Kit: put 2 cards in any combination of Helix Fossil Omanyte, Dome Fossil Kabuto and Old Amber Aerodactyl from your discard pile into your hand.</summary>
        private sealed class FossilExcavationKit : CardScript
        {
            private static List<CardInstance> InDiscard(EffectContext ctx) { return ctx.Me.Discard.Where(c => Fossils.Contains(c.Def.Name)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return InDiscard(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = InDiscard(ctx);
                if (pile.Count == 0) return;
                int n = Math.Min(2, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Fossil Excavation Kit: put 2 Fossil cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Lass's Special: draw a card for each of your opponent's Benched Basic Pokémon.</summary>
        private sealed class LassesSpecial : CardScript
        {
            private static int Count(EffectContext ctx) { return ctx.Opp.Bench.Count(m => m.Def.IsBasicPokemon); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Count(ctx) > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Count(ctx);
                if (n > 0) ctx.Draw(n);
            }
        }

        /// <summary>Mega Catcher: switch 1 of your opponent's Benched Mega Evolution Pokémon (XY-era M Pokémon-EX) with their Active Pokémon.</summary>
        private sealed class MegaCatcher : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentBench().Where(m => m.Def.IsXYMegaEvolution && ctx.CanAffect(m)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Mega Catcher: switch which of your opponent's Benched Mega Evolution Pokémon into the Active Spot?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Power Memory: the Zygarde-EX it's attached to can also use All Cells Burn (200; discard 3 Energy from this Pokémon). It still needs the Energy.</summary>
        private sealed class PowerMemory : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Def.Name != "Zygarde-EX" || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return;
                int n = Math.Min(3, me.Energy.Count);
                var picks = n == me.Energy.Count ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), n, n, "All Cells Burn: discard 3 Energy from " + me.Name);
                foreach (var card in picks) ctx.Engine.DiscardEnergy(me, card);
            }
        }

        /// <summary>Random Receiver: reveal cards from the top of your deck until you reveal a Supporter; put it into your hand and shuffle the rest back.</summary>
        private sealed class RandomReceiver : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                var revealed = new List<CardInstance>();
                CardInstance supporter = null;
                foreach (var card in deck)
                {
                    revealed.Add(card);
                    if (card.Def.IsSupporter) { supporter = card; break; }
                }
                ctx.Engine.Reveal(ctx.Player, revealed, "revealed from the top of " + ctx.Engine.Their(ctx.Player) + " deck (Random Receiver)");
                if (supporter != null && deck.Remove(supporter)) ctx.Me.Hand.Add(supporter);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Scorched Earth: once during each player's turn, that player may discard a Fire or Fighting Energy card from their hand and draw 2 cards.</summary>
        private sealed class ScorchedEarth : StadiumAction
        {
            private static List<CardInstance> Discardable(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && (c.Def.EnergyProvides == EnergyType.Fire || c.Def.EnergyProvides == EnergyType.Fighting)).ToList();
            }

            protected override bool Ready(EffectContext ctx) { return Discardable(ctx).Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var cards = Discardable(ctx);
                if (cards.Count == 0) return false;
                var card = ctx.ChooseCards(cards, 1, 1, "Scorched Earth: discard which Fire or Fighting Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Team Rocket's Handiwork: flip 2 coins; for each heads, discard the top 2 cards of the opponent's deck.</summary>
        private sealed class TeamRocketsHandiwork : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2 * heads);
            }
        }

        // ================================================================== Energy

        /// <summary>
        /// Strong Energy: only on a Fighting Pokémon (discarded from anything else, including after an evolution changes the type:
        /// Compendium, 2014-08-28); provides Fighting Energy there; its Pokémon's attacks do 20 more damage to the opponent's
        /// Active Pokémon (before Weakness and Resistance).
        /// </summary>
        private sealed class StrongEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fighting)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Fighting) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Fighting); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fighting); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(src.Pokemon, EnergyType.Fighting) ? damage + 20 : damage;
            }
        }
    }
}
