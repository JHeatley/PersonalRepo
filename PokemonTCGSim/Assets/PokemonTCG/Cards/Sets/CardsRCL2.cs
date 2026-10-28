using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Rebel Clash (RCL, 2020), Pokémon 94-153 (Fighting, Darkness, Metal and Colorless), the Trainers and the Special Energy.
    /// Regulation mark D: Expanded only.
    /// Already in the pool: Galarian Farfetch'd, Galarian Sirfetch'd, Galarian Yamask, Galarian Runerigus, Rolycoly, Carkol,
    /// Coalossal, Falinks, Falinks V, Stonjourner, Galarian Weezing, Trubbish, Impidimp, Morgrem, Grimmsnarl, Galarian Meowth,
    /// Duraludon, Skwovet, Greedent, Dubwool V, Boss's Orders, Full Heal, Milo, Poké Ball, Skyla, Sonia, Tool Scrapper, Training
    /// Court, Turffield Stadium and Capture Energy; the secret rare Galarian Perrserker (Steely Spirit) too.
    /// Pokémon whose attacks only do their printed damage need no script (Nosepass, Barboach, Koffing, Impidimp, Bronzor,
    /// Honedge, Tranquill).
    /// </summary>
    public static class CardsRCL2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("RCL-97", new ParalyzeFlipMon("Yoga Shock"));
            r.RegisterByKey("RCL-98", new ComboMon(new CardsFST1.NoWeaknessResistanceMon("Yoga Kick"), new CardsMEG.DefendingEnergyBonusMon("Psychic", 20)));
            r.RegisterByKey("RCL-100", new WhiscashRCL());
            r.RegisterByKey("RCL-103", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("RCL-104", new ComboMon(new CardsDAA2.RockHurlMon(), new CardsASR1.WideWingMon("Hand Press")));
            r.RegisterByKey("RCL-108", new ComboMon(new CardsRCL1.AttachFromDiscardMon("Sand Eater", EnergyType.Fighting, true), new CardsTWM1.SelfDiscardMon("Sand Breath", 2)));
            // Darkness
            r.RegisterByKey("RCL-114", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("RCL-115", new PoisonRingMon());
            r.RegisterByKey("RCL-116", new ComboMon(new SplittingSpiteMon(), new DrippingGrudgeMon()));
            r.RegisterByKey("RCL-118", new GarbodorRCL());
            r.RegisterByKey("RCL-119", new CardsBLK.PeckOffMon("Pluck"));
            r.RegisterByKey("RCL-120", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Bone Rush"), new BlindsideMon()));
            r.RegisterByKey("RCL-121", new ComboMon(new CardsWHT.DragOffMon("Drag Off", 30), new ConditionAttackMon("Brain Shake", SpecialCondition.Confused)));
            r.RegisterByKey("RCL-122", new HandToDeckBottomMon("Max Jammer"));
            // Metal
            r.RegisterByKey("RCL-127", new CoinsMon("Claw Dagger", 3));
            r.RegisterByKey("RCL-128", new ComboMon(new CardsRCL1.EvolvedFromBonusMon("Raid", "Scyther", 90), new ShieldAttackMon("Guard Claw", 30)));
            r.RegisterByKey("RCL-130", new CardsFST2.RapidSpinMon("Gyro Ball"));
            r.RegisterByKey("RCL-131", new CardsOBF1.RetreatCostDamageMon("Gravitational Drop", 30, false));
            r.RegisterByKey("RCL-132", new GatheringFoodMon());
            r.RegisterByKey("RCL-134", new CoinsMon("Double Slash", 2));
            r.RegisterByKey("RCL-135", new AegislashRCL());
            r.RegisterByKey("RCL-136", new ShieldAttackMon("Adamantine Press", 30));
            r.RegisterByKey("RCL-137", new CardsCRZ1.BasicTargetBonusMon("Dangerous Nose", 100));
            r.RegisterByKey("RCL-139", new ComboMon(new CardsRCL1.AttachFromDiscardMon("Energy Stream", EnergyType.Metal, true), new SmashingEdgeMon()));
            r.RegisterByKey("RCL-140", new ComboMon(new ShieldAttackMon("Guard Press", 20), new CardsSVI1.FlipTailsLockMon("Power Rush")));
            // Colorless
            r.RegisterByKey("RCL-141", new ComboMon(new DrawAttackMon("Collect", 2), new SelfConditionMon("Collapse", SpecialCondition.Asleep)));
            r.RegisterByKey("RCL-142", new ChatotRCL());
            r.RegisterByKey("RCL-143", new SearchAttackMon("Chirp", FightingResistant, 2));
            r.RegisterByKey("RCL-145", new ComboMon(new CardsMEG.WeakenDefendingMon("Daunt", 50), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("RCL-146", new CardsSSP2.StompOffMon("Burrow", 1));
            r.RegisterByKey("RCL-147", new MiningRushMon());
            r.RegisterByKey("RCL-148", new CardsDAA1.DeepSeaSwirlMon("Windfall", 5));
            r.RegisterByKey("RCL-149", new FlipBonusMon("Jump On", 40));
            r.RegisterByKey("RCL-150", new ComboMon(new CardsSSP2.StompOffMon("Hammer Arm", 1), new BigThrowMon()));

            // Trainers
            r.RegisterByName("Big Charm", new BigCharm());
            r.RegisterByName("Burning Scarf", new BurningScarf());
            r.RegisterByName("Capacious Bucket", new CapaciousBucket());
            r.RegisterByName("Cursed Shovel", new CursedShovel());
            r.RegisterByName("Dan", new Dan());
            r.RegisterByName("Galar Mine", new GalarMine());
            r.RegisterByName("Nugget", new Nugget());
            r.RegisterByName("Oleana", new Oleana());
            r.RegisterByName("Scoop Up Net", new ScoopUpNet());
            // Special Energy
            r.RegisterByName("Horror Psychic Energy", new HorrorPsychicEnergy());
            r.RegisterByName("Speed Lightning Energy", new SpeedLightningEnergy());
            r.RegisterByName("Twin Energy", new TwinEnergy());
        }

        private static bool FightingResistant(CardDef d) { return d.IsPokemon && d.Resistance == EnergyType.Fighting; }
        private static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }

        // ================================================================== Fighting

        /// <summary>Submerge: on the Bench, prevent all damage done to this Pokémon by attacks (yours and your opponent's). Earthquake: 20 to each of your Benched Pokémon.</summary>
        private sealed class WhiscashRCL : Mon
        {
            private static readonly Mon Earthquake = new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && !src.IsActive(src.Pokemon) ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Earthquake.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Poison Ring: the opponent's Active Pokémon is now Poisoned and can't retreat during their next turn.</summary>
        private sealed class PoisonRingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Ring") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Splitting Spite: discard the top card of each player's deck.</summary>
        private sealed class SplittingSpiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splitting Spite") return false;
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>Dripping Grudge: 1 damage counter on the opponent's Active Pokémon for each Pokémon in your discard pile.</summary>
        private sealed class DrippingGrudgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dripping Grudge") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon);
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Poisonous Puddle: once during your turn, if a Stadium is in play, the opponent's Active Pokémon is now Poisoned.</summary>
        private sealed class GarbodorRCL : Mon
        {
            protected override string Activated { get { return "Poisonous Puddle"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.State.Stadium != null && ctx.Opp.Active != null; }
            protected override void Ability(EffectContext ctx) { ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Poisoned); }
        }

        /// <summary>Blindside: 100 damage to 1 of the opponent's Pokémon that has any damage counters on it.</summary>
        private sealed class BlindsideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blindside") return false;
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0)
                {
                    ctx.Log("Blindside did nothing: none of the opponent's Pokémon has damage counters on it.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Blindside: 100 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, 100);
                return true;
            }
        }

        /// <summary>Max Jammer: the opponent reveals their hand; you put a card from it on the bottom of their deck.</summary>
        private sealed class HandToDeckBottomMon : Mon
        {
            private readonly string attack;

            public HandToDeckBottomMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var pick = ctx.ChooseCards(hand.ToList(), 1, 1, attack + ": choose a card from your opponent's hand to put on the bottom of their deck")[0];
                if (hand.Remove(pick)) ctx.Opp.Deck.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + pick.Def.Name + " from their hand on the bottom of their deck (" + attack + ").");
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Gathering Food: for each Energy attached to this Pokémon, search your deck for a Trainer card.</summary>
        private sealed class GatheringFoodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gathering Food") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                if (n > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(IsTrainerCard, n, "Gathering Food: choose up to " + n + (n == 1 ? " Trainer card" : " Trainer cards"));
                return true;
            }
        }

        /// <summary>Big Shield: your Pokémon take 30 less damage from the opponent's attacks; only 1 Big Shield applies at a time.</summary>
        private sealed class AegislashRCL : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target == null || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                foreach (var mon in src.Me.InPlay())
                {
                    if (!mon.Def.Abilities.Any(a => a.Name == "Big Shield") || !src.Engine.HasWorkingAbilityOrPower(mon)) continue;
                    return mon == me ? Math.Max(0, damage - 30) : damage;
                }
                return damage;
            }
        }

        /// <summary>Smashing Edge: flip a coin; tails, discard 2 Energy from this Pokémon.</summary>
        private sealed class SmashingEdgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smashing Edge") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Smashing Edge: discard 2 Energy from " + me.Name);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>"This Pokémon is now [condition]" after the damage (Collapse).</summary>
        internal sealed class SelfConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public SelfConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, condition);
                return true;
            }
        }

        /// <summary>Lucky Match: when you play this Pokémon from your hand onto your Bench, you may flip a coin; heads, put a Supporter card from your discard pile into your hand.</summary>
        private sealed class ChatotRCL : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Me.Discard.Any(c => c.Def.IsSupporter);
                if (!TriggerOnce(ctx, "Lucky Match", ready, "Use Lucky Match: flip a coin, and on heads put a Supporter card from your discard pile into your hand?")) return;
                if (!ctx.Flip()) return;
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return;
                var pick = ctx.ChooseCards(supporters, 1, 1, "Lucky Match: put which Supporter card into your hand?");
                ctx.DiscardToHand(pick);
            }
        }

        /// <summary>Mining Rush: discard up to 6 cards from the top of your deck, one at a time, stopping whenever you like (Compendium); 30 damage for each.</summary>
        private sealed class MiningRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mining Rush") return false;
                int n = 0;
                while (n < 6 && ctx.Me.Deck.Count > 0)
                {
                    if (!ctx.AskYesNo("Mining Rush: discard the top card of your deck? (" + n + " discarded so far: " + (atk.Damage * n) + " damage)")) break;
                    if (ctx.Engine.DiscardTopOfDeck(ctx.Player, 1).Count == 0) break;
                    n++;
                }
                if (n == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Big Throw: flip a coin; heads, discard the opponent's Active Pokémon and all attached cards (not a Knock Out: no Prize cards).</summary>
        private sealed class BigThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Throw") return false;
                var defending = ctx.Defending;
                if (defending == null || !ctx.Flip()) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Big Throw on " + defending.Name + " was prevented.");
                    return true;
                }
                ctx.Engine.DiscardPokemonFromPlay(defending);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Big Charm: the Pokémon this card is attached to gets +30 HP.</summary>
        private sealed class BigCharm : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon ? hp + 30 : hp;
            }
        }

        /// <summary>Burning Scarf: if its Fire Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), the Attacking Pokémon is now Burned.</summary>
        private sealed class BurningScarf : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || !holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (!ctx.Engine.IsType(holder, EnergyType.Fire)) return;
                ctx.Log("Burning Scarf: " + attacker.Name + " is now Burned.");
                ctx.ApplyCondition(attacker, SpecialCondition.Burned);
            }
        }

        /// <summary>Capacious Bucket: search your deck for up to 2 basic Water Energy cards.</summary>
        private sealed class CapaciousBucket : CardScript
        {
            private static bool WaterCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Water); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(WaterCard, 2, "Capacious Bucket: choose up to 2 Water Energy cards");
            }
        }

        /// <summary>Cursed Shovel: if its Pokémon is Knocked Out by damage from an opponent's attack, discard the top 2 cards of the opponent's deck.</summary>
        private sealed class CursedShovel : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Opp.Deck.Count == 0) return;
                // A Pokémon Tool, so Bibarel's Reassuring Dam doesn't stop it (Compendium, 2024-03-21).
                src.Log("Cursed Shovel: the top 2 cards of " + src.Engine.PlayerName(src.Opponent) + "'s deck are discarded.");
                src.Engine.DiscardTopOfDeck(src.Opponent, 2);
            }
        }

        /// <summary>
        /// Dan: draw 2 cards; then you and your opponent play Rock-Paper-Scissors until someone wins, and if you win, draw 2 more.
        /// Your ruling (v65): both players choose (the bots at random); ties are played again.
        /// </summary>
        private sealed class Dan : CardScript
        {
            private static readonly string[] Hands = { "Rock", "Paper", "Scissors" };
            private const int MaxRounds = 20;

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                for (int round = 1; round <= MaxRounds; round++)
                {
                    string title = "Dan: Rock-Paper-Scissors" + (round > 1 ? " (round " + round + ")" : "") + ". Choose your hand.";
                    int mine = ctx.ChooseOption(title, Hands);
                    var opts = new List<DecisionOption>();
                    for (int i = 0; i < Hands.Length; i++) opts.Add(new DecisionOption { Label = Hands[i], Index = i });
                    int theirs = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, title, opts, ctx.Source, false);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " chose " + Hands[mine] + "; " + ctx.Engine.PlayerName(ctx.Opponent) + " chose " + Hands[theirs] + ".");
                    if (mine == theirs) continue;
                    Finish(ctx, (mine - theirs + 3) % 3 == 1);
                    return;
                }
                // A safety stop for players who keep tying: the engine decides at random.
                Finish(ctx, ctx.State.Rng.Next(2) == 0);
            }

            private static void Finish(EffectContext ctx, bool won)
            {
                ctx.Log(ctx.Engine.PlayerName(won ? ctx.Player : ctx.Opponent) + " won the Rock-Paper-Scissors (Dan).");
                if (won) ctx.Draw(2);
            }
        }

        /// <summary>Galar Mine: the Retreat Cost of both Active Pokémon is [C][C] more.</summary>
        private sealed class GalarMine : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && src.State.IsActive(mon) ? cost + 2 : cost;
            }
        }

        /// <summary>
        /// Nugget: playable only right after you draw it at the beginning of your turn, before it goes into your hand. Draw 3
        /// cards. The engine asks when it's drawn; Item lock still stops it.
        /// </summary>
        private sealed class Nugget : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return false; }

            public override void OnDrawnForTurn(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0 || !ctx.Engine.CanPlayItemFromOutsideHand(ctx.Player, ctx.Card)) return;
                if (!ctx.AskYesNo("Nugget: you drew it at the beginning of your turn. Play it now to draw 3 cards?")) return;
                if (!ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.Engine.PlayItemFromOutsideHand(ctx.Player, ctx.Card, "right after drawing it");
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Oleana: discard 2 other cards from your hand to play it; your opponent reveals their hand and you put a Trainer card from it on the bottom of their deck.</summary>
        private sealed class Oleana : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                var discards = others.Count <= 2 ? others : ctx.ChooseCards(others, 2, 2, "Oleana: discard 2 other cards from your hand");
                foreach (var card in discards) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                var trainers = ctx.Opp.Hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return;
                var pick = trainers.Count == 1 ? trainers[0]
                    : ctx.ChooseCards(trainers, 1, 1, "Oleana: put which Trainer card on the bottom of your opponent's deck?")[0];
                if (ctx.Opp.Hand.Remove(pick)) ctx.Opp.Deck.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + pick.Def.Name + " from their hand on the bottom of their deck (Oleana).");
            }
        }

        /// <summary>
        /// Scoop Up Net: put 1 of your Pokémon that isn't a Pokémon V or a Pokémon-GX into your hand (every Pokémon card in it:
        /// Compendium, 2020-04-30) and discard all attached cards.
        /// </summary>
        private sealed class ScoopUpNet : CardScript
        {
            private static bool Eligible(EffectContext ctx, PokemonInPlay mon)
            {
                return !mon.Def.IsPokemonV && !mon.Def.IsGX && ctx.Engine.CanPutIntoHand(mon);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => Eligible(ctx, m)).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.MyPokemon(m => Eligible(ctx, m));
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Scoop Up Net: put which of your Pokémon into your hand?");
                if (mon == null) return;
                var pokemonCards = mon.Stack.ToList();
                var attached = mon.Energy.Concat(mon.Tools).ToList();
                ctx.Engine.RemoveFromPlay(mon);
                ctx.Me.Hand.AddRange(pokemonCards);
                ctx.Me.Discard.AddRange(attached);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, mon.Name + " was put into " + ctx.Engine.PlayerName(ctx.Player) + "'s hand"
                    + (attached.Count > 0 ? " and its attached cards were discarded" : "") + " (Scoop Up Net).", mon.Top.Id, mon.Uid);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Horror Psychic Energy: provides Psychic; if its Psychic Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), 2 damage counters on the Attacking Pokémon.</summary>
        private sealed class HorrorPsychicEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Psychic), 1); }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || !holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (!ctx.Engine.IsType(holder, EnergyType.Psychic)) return;
                ctx.Log("Horror Psychic Energy: 2 damage counters on " + attacker.Name + ".");
                ctx.PlaceCounters(attacker, 2);
            }
        }

        /// <summary>Speed Lightning Energy: provides Lightning; when you attach it from your hand to a Lightning Pokémon, draw 2 cards.</summary>
        private sealed class SpeedLightningEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Lightning), 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Lightning)) return;
                ctx.Draw(2);
            }
        }

        /// <summary>Twin Energy: provides [C][C] on a Pokémon that isn't a Pokémon V or a Pokémon-GX, [C] on one that is.</summary>
        private sealed class TwinEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                bool single = holder != null && (holder.Def.IsPokemonV || holder.Def.IsGX);
                return RecifeEnergy.Units(EnergyMask.Colorless, single ? 1 : 2);
            }
        }
    }
}
