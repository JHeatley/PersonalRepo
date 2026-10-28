using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paldea Evolved (PAL) 100-193: the Pokémon, Trainers and Special Energy that weren't already in the pool (all G-mark,
    /// Expanded only). Tinkaton ex and Annihilape ex are reprints of promos already in the pool. Pokémon whose attacks only do
    /// their printed damage need no script: Mankey, Pupitar, Makuhita, Rockruff, Nacli, Glimmet, Paldean Wooper, Maschiff,
    /// Cufant, Tandemaus.
    /// </summary>
    public static class CardsPAL2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAL-100", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("PAL-102", new FlipBonusMon("Brutal Swing", 20));
            r.RegisterByKey("PAL-103", new LockAfterAttackMon("Boundless Power", false));
            r.RegisterByKey("PAL-107", new FlipBonusMon("Pummel", 60));
            r.RegisterByKey("PAL-109", new DodgeMon("Hit and Hide", false));
            r.RegisterByKey("PAL-110", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("PAL-113", new ArmThrustPracticeMon());
            r.RegisterByKey("PAL-114", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("PAL-115", new ConditionAttackMon("Knuckle Claws", SpecialCondition.Poisoned));
            r.RegisterByKey("PAL-117", new ScaryFangsMon());
            r.RegisterByKey("PAL-118", new MakeTheAssistMon());
            r.RegisterByKey("PAL-119", new FalinksPAL());
            r.RegisterByKey("PAL-126", new GlimmoraPAL());
            r.RegisterByKey("PAL-132", new DirtyThrowMon());
            r.RegisterByKey("PAL-135", new TyranitarPAL());
            r.RegisterByKey("PAL-137", new CardsPAL1.EvolutionBonusMon("Cross-Cut", 50));
            r.RegisterByKey("PAL-138", new FlipBonusMon("Ambush", 20));
            r.RegisterByKey("PAL-139", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 2));
            r.RegisterByKey("PAL-140", new HydreigonPAL());
            r.RegisterByKey("PAL-143", new ComeuppanceMon());
            r.RegisterByKey("PAL-145", new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("PAL-147", new ComboMon(new BenchSearchAttackMon("Delivery Pocket", 1), new CardsTWM1.SelfDiscardMon("Dark Edge", 1)));
            r.RegisterByKey("PAL-148", new ComboMon(new AccelerateMon(), new CardsTWM1.SelfDiscardMon("Spinning Bird", 2)));
            r.RegisterByKey("PAL-150", new CopperajahExPAL());
            r.RegisterByKey("PAL-151", new OrthwormPAL());
            r.RegisterByKey("PAL-154", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PAL-155", new EitherFaceMon());
            r.RegisterByKey("PAL-156", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("PAL-157", new DigAwayFlashMon());
            r.RegisterByKey("PAL-160", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("PAL-161", new FlipBonusMon("Sharp Claws", 60));
            r.RegisterByKey("PAL-162", new SlakingPAL());
            r.RegisterByKey("PAL-163", new RecoilMon("Nosedive", 10));
            r.RegisterByKey("PAL-164", new CardsASC.PushDownMon("Send Back"));
            r.RegisterByKey("PAL-165", new NoRetreatMon("Clutch"));
            r.RegisterByKey("PAL-168", new GnawRelentlesslyMon());

            r.RegisterByName("Bravery Charm", new BraveryCharm());
            r.RegisterByName("Calamitous Snowy Mountain", new CalamitousSnowyMountain());
            r.RegisterByName("Calamitous Wasteland", new CalamitousWasteland());
            r.RegisterByName("Choice Belt", new ChoiceBelt());
            r.RegisterByName("Clavell", new Clavell());
            r.RegisterByName("Delivery Drone", new DeliveryDrone());
            r.RegisterByName("Dendra", new Dendra());
            r.RegisterByName("Falkner", new Falkner());
            r.RegisterByName("Fighting Au Lait", new FightingAuLait());
            r.RegisterByName("Giacomo", new Giacomo());
            r.RegisterByName("Great Ball", new GreatBall());
            r.RegisterByName("Grusha", new Grusha());
            r.RegisterByName("Practice Studio", new PracticeStudio());
            r.RegisterByName("Saguaro", new Saguaro());
            r.RegisterByName("Super Rod", new SuperRod());
            r.RegisterByName("Superior Energy Retrieval", new SuperiorEnergyRetrieval());
            r.RegisterByName("Jet Energy", new JetEnergy());
            r.RegisterByName("Luminous Energy", new LuminousEnergy());
            r.RegisterByName("Therapeutic Energy", new TherapeuticEnergy());
        }

        // ================================================================== Fighting

        /// <summary>Arm Thrust Practice, Protective Bell: all of your Pokémon take 10 less damage from the opponent's attacks. Each copy in play counts.</summary>
        internal sealed class ArmThrustPracticeMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target.Owner == src.Player && info.AgainstOpponent ? damage - 10 : damage;
            }
        }

        /// <summary>Scary Fangs: during the opponent's next turn, if this Pokémon is damaged by an attack, 10 damage counters on the Attacking Pokémon.</summary>
        private sealed class ScaryFangsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scary Fangs") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new RetaliateEffect(me.Name + " puts 10 damage counters on an attacker that damages it during the opponent's next turn (Scary Fangs)",
                        ctx.Player, me.Uid, ctx.Turn + 1, 10));
                return true;
            }
        }

        /// <summary>Make the Assist: move an Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class MakeTheAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Make the Assist") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var bench = ctx.Me.Bench.Where(m => m != me).ToList();
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || bench.Count == 0) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Make the Assist: move which Energy?")[0];
                var target = ctx.ChoosePokemon(bench, "Make the Assist: move " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Reckless Charge Together: 90 more damage if a Falinks is on your Bench (the exact name: your ruling).</summary>
        private sealed class FalinksPAL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reckless Charge Together") return false;
                var me = ctx.Pokemon;
                bool partner = ctx.Me.Bench.Any(m => m != me && m.Def.Name == "Falinks");
                ctx.DamageDefending(atk.Damage + (partner ? 90 : 0));
                return true;
            }
        }

        /// <summary>
        /// Shattering Crystal: when this Pokémon is Knocked Out, flip a coin; on heads the opponent takes no Prize cards for it.
        /// Poison Petals: Poisoned, with 6 damage counters in Pokémon Checkup instead of 1.
        /// </summary>
        private sealed class GlimmoraPAL : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var me = src.Pokemon;
                if (me == null || knockedOut != me || taker == src.Player || prizes <= 0) return prizes;
                Announce(src, "Shattering Crystal");
                if (!src.Engine.FlipCoin(src.Player, "Shattering Crystal")) return prizes;
                src.Log("Shattering Crystal: " + src.Engine.PlayerName(taker) + " takes no Prize cards for " + me.Name + ".");
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Petals") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 6;
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Dirty Throw: discard a card from your hand; if you can't, the attack does nothing.</summary>
        private sealed class DirtyThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Throw") return false;
                if (ctx.Me.Hand.Count == 0) return Nothing(ctx);
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Dirty Throw: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Rout: 30 more for each of the opponent's Benched Pokémon. Dread Mountain: discard the top 4 cards of your deck.</summary>
        private sealed class TyranitarPAL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rout")
                {
                    ctx.DamageDefending(atk.Damage + 30 * ctx.Opp.Bench.Count);
                    return true;
                }
                if (atk.Name == "Dread Mountain")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 4);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Tri Howl: once during your turn, look at the top 3 cards of your deck, attach any number of Energy cards there to your
        /// Pokémon in any way you like, and discard the other cards.
        /// </summary>
        private sealed class HydreigonPAL : Mon
        {
            protected override string Activated { get { return "Tri Howl"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, energy.Count, (c, m) => true, "Tri Howl: attach an Energy card to your Pokémon");
                var rest = new List<CardInstance>();
                foreach (var card in top)
                    if (ctx.Me.Deck.Remove(card)) rest.Add(card);
                if (rest.Count == 0) return;
                ctx.Me.Discard.AddRange(rest);
                ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " +
                    string.Join(", ", rest.Select(c => c.Def.Name).ToArray()) + " (Tri Howl).", rest);
            }
        }

        /// <summary>Comeuppance: during the opponent's next turn, damage done to this Pokémon by an attack is put back on the Attacking Pokémon as damage counters.</summary>
        private sealed class ComeuppanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Comeuppance") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new CardsSVP.MirrorDamageEffect(me.Name + " returns attack damage as damage counters during the opponent's next turn (Comeuppance)",
                        ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Accelerate: if the opponent's Pokémon is Knocked Out by this attack's damage, prevent all damage from and effects of attacks done to this Pokémon next turn.</summary>
        private sealed class AccelerateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Accelerate") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && ctx.InPlay(defending) && ctx.Engine.IsKnockedOut(defending)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Bronze Body: takes 30 less damage from attacks. Nosequake: 30 damage to each of your own Benched Pokémon too.</summary>
        private sealed class CopperajahExPAL : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nosequake") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) if (mon != me) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Nutritional Iron: +100 HP with 3 or more [M] Energy attached. Shoot Through: 30 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class OrthwormPAL : Mon
        {
            private static readonly Mon ShootThrough = new Cards30CC.BenchHitMon("Shoot Through", 30);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Energy.Count < 3) return hp;
                return src.Engine.EnergyCountOfType(mon, EnergyType.Metal) >= 3 ? hp + 100 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ShootThrough.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Either Face: choose a player; they shuffle their hand into their deck and draw 4 cards.</summary>
        private sealed class EitherFaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Either Face") return false;
                int who = ctx.ChooseOption("Either Face: which player shuffles their hand into their deck and draws 4 cards?", "You", "Your opponent") == 0 ? ctx.Player : ctx.Opponent;
                ctx.Engine.ShuffleHandIntoDeck(who);
                ctx.Engine.DrawCards(who, 4);
                return true;
            }
        }

        /// <summary>Dig Away Flash: the opponent's Active Pokémon is Paralyzed, then this Pokémon and its cards are shuffled into your deck.</summary>
        private sealed class DigAwayFlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Away Flash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Stir and Snooze: while Asleep, it flips 2 coins in Pokémon Checkup and wakes up only on 2 heads. Slacker's Headstrike: this Pokémon is now Asleep.</summary>
        private sealed class SlakingPAL : Mon
        {
            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(coins, 2) : coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slacker's Headstrike") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Gnaw Relentlessly: 1 damage counter on each of the opponent's Pokémon for each of your Maushold in play (exact name: your ruling).</summary>
        private sealed class GnawRelentlesslyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gnaw Relentlessly") return false;
                int n = ctx.MyPokemon(m => m.Def.Name == "Maushold").Count;
                if (n == 0) return true;
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, n);
                return true;
            }
        }

        // ================================================================== Tools and Stadiums

        /// <summary>Bravery Charm: the Basic Pokémon this card is attached to gets +50 HP.</summary>
        private sealed class BraveryCharm : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.IsBasic ? hp + 50 : hp;
            }
        }

        /// <summary>Choice Belt: the holder's attacks do 30 more damage to the opponent's Active Pokémon V.</summary>
        private sealed class ChoiceBelt : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.Target.Def.IsPokemonV ? damage + 30 : damage;
            }
        }

        /// <summary>Calamitous Snowy Mountain: when any player attaches an Energy card from their hand to their Basic non-[W] Pokémon, 2 damage counters on it.</summary>
        private sealed class CalamitousSnowyMountain : CardScript
        {
            private static void Hit(EffectContext src, PokemonInPlay target)
            {
                if (target == null || !target.IsBasic || src.Engine.IsType(target, EnergyType.Water)) return;
                src.Engine.PlaceDamageCounters(target, 2, "Calamitous Snowy Mountain");
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Hit(src, target); }
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Hit(src, target); }
        }

        /// <summary>Calamitous Wasteland: the Retreat Cost of each Basic non-[F] Pokémon in play is [C] more.</summary>
        private sealed class CalamitousWasteland : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon.IsBasic && !src.Engine.IsType(mon, EnergyType.Fighting) ? cost + 1 : cost;
            }
        }

        /// <summary>Practice Studio: attacks of Stage 1 Pokémon (both players') do 10 more damage to the opponent's Active Pokémon.</summary>
        private sealed class PracticeStudio : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.Attacker.Def.Stage == Stage.Stage1 ? damage + 10 : damage;
            }
        }

        // ================================================================== Items and Supporters

        /// <summary>Clavell: search your deck for up to 3 Basic Pokémon with 120 HP or less.</summary>
        private sealed class Clavell : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicPokemon && d.Hp <= 120, 3, "Clavell: search your deck for up to 3 Basic Pokémon with 120 HP or less");
            }
        }

        /// <summary>Delivery Drone: flip 2 coins; if both are heads, search your deck for a card.</summary>
        private sealed class DeliveryDrone : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.FlipCoins(2) < 2) return;
                ctx.SearchToHand(d => true, 1, "Delivery Drone: put a card from your deck into your hand", false, 1);
            }
        }

        /// <summary>Dendra: put a card from your hand on the bottom of your deck, then draw until you have 5 cards. Needs another card in your hand.</summary>
        private sealed class Dendra : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Dendra: put a card from your hand on the bottom of your deck")[0];
                ctx.Me.Hand.Remove(pick);
                ctx.Me.Deck.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card on the bottom of their deck (Dendra).");
                ctx.DrawUntil(5);
            }
        }

        /// <summary>Falkner: draw 2 cards; 2 more if you have a Stadium in play.</summary>
        private sealed class Falkner : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(CardsPAL1.HaveStadium(ctx) ? 4 : 2);
            }
        }

        /// <summary>Fighting Au Lait: only with more Prize cards left than the opponent; heal 60 damage from 1 of your Pokémon.</summary>
        private sealed class FightingAuLait : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count && CardsMEG.AnyHurt(ctx);
            }

            public override void ResolveTrainer(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 60, "Fighting Au Lait"); }
        }

        /// <summary>Giacomo: discard a Special Energy from each of the opponent's Pokémon.</summary>
        private sealed class Giacomo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.OpponentPokemon().Any(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.OpponentPokemon())
                {
                    if (!mon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy)) continue;
                    if (!ctx.CanAffect(mon))
                    {
                        ctx.Log("The effect of Giacomo on " + mon.Name + " was prevented.");
                        continue;
                    }
                    ctx.DiscardEnergyFrom(mon, 1, "Giacomo: discard which Special Energy from " + mon.Name + "?", e => e.Def.IsAttachedSpecialEnergy);
                }
            }
        }

        /// <summary>Great Ball: look at the top 7 cards of your deck; you may put a Pokémon you find there into your hand; shuffle the rest back.</summary>
        private sealed class GreatBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var mons = top.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count > 0)
                {
                    var picks = ctx.ChooseCards(mons, 0, 1, "Great Ball: you may put a Pokémon into your hand");
                    if (picks.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Player, picks, "put this Pokémon into " + ctx.Engine.Their(ctx.Player) + " hand");
                        ctx.DeckToHand(picks);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Grusha: draw until you have 5 cards, or 7 if none of your Pokémon have any Energy attached.</summary>
        private sealed class Grusha : CardScript
        {
            private static int Target(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.Energy.Count > 0) ? 5 : 7; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < Target(ctx);
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(Target(ctx)); }
        }

        /// <summary>Saguaro: heal 50 damage from each of up to 2 of your Pokémon.</summary>
        private sealed class Saguaro : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var picks = ctx.ChoosePokemonMany(hurt, 1, Math.Min(2, hurt.Count), "Saguaro: heal 50 damage from up to 2 of your Pokémon");
                foreach (var mon in picks) ctx.Heal(mon, 50);
            }
        }

        /// <summary>Super Rod: shuffle up to 3 in any combination of Pokémon and basic Energy cards from your discard pile into your deck.</summary>
        private sealed class SuperRod : CardScript
        {
            private static List<CardInstance> Cards(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Cards(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(3, cards.Count), "Super Rod: shuffle up to 3 Pokémon and basic Energy cards into your deck");
                var moved = ctx.DiscardToDeck(picks);
                if (moved.Count > 0) ctx.Engine.Reveal(ctx.Player, moved, "shuffled these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Superior Energy Retrieval: discard 2 other cards from your hand, then put up to 4 basic Energy cards from your discard
        /// pile into your hand (not the ones just discarded). You don't need 4 there and needn't take all 4 (your ruling).
        /// </summary>
        private sealed class SuperiorEnergyRetrieval : CardScript
        {
            // A card that would do nothing can't be played: it needs a basic Energy card in the discard pile already (v69, your bug report).
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Discard.Any(x => x.Def.IsBasicEnergy) && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count < 2) return;
                var discarded = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Superior Energy Retrieval: discard 2 cards from your hand");
                foreach (var card in discarded) ctx.Engine.DiscardFromHand(ctx.Player, card);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && !discarded.Contains(c)).ToList();
                if (energy.Count == 0) return;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(4, energy.Count), "Superior Energy Retrieval: put up to 4 basic Energy cards into your hand");
                if (picks.Count > 0) ctx.DiscardToHand(picks);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Jet Energy: provides [C]; when attached from your hand to a Benched Pokémon, switch it with your Active Pokémon.</summary>
        private sealed class JetEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null || !ctx.Me.Bench.Contains(mon) || ctx.Me.Active == null) return;
                ctx.Engine.SwitchActive(ctx.Player, mon);
            }
        }

        /// <summary>
        /// Luminous Energy: every type, 1 Energy at a time; [C] instead while its Pokémon has any other Special Energy attached.
        /// It isn't Basic Energy (Compendium: Alcremie, 2024-09-12) and counts as every type (Tinkaton, 2023-11-09).
        /// </summary>
        private sealed class LuminousEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var holder = ctx.Pokemon;
                bool otherSpecial = holder != null && holder.Energy.Any(e => e != ctx.Card && e.Def.IsAttachedSpecialEnergy);
                return RecifeEnergy.Units(otherSpecial ? EnergyMask.Colorless : EnergyMask.AnyType, 1);
            }
        }

        /// <summary>Therapeutic Energy: provides [C]; its Pokémon recovers from and can't be affected by Asleep, Confused or Paralyzed.</summary>
        private sealed class TherapeuticEnergy : CardScript
        {
            private const SpecialCondition Covered = SpecialCondition.Asleep | SpecialCondition.Confused | SpecialCondition.Paralyzed;

            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & Covered) != 0;
            }

            private static void Recover(EffectContext src)
            {
                var m = src.Pokemon;
                if (m == null || (m.Conditions & Covered) == 0) return;
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed })
                    if (m.Has(c)) src.Engine.RemoveCondition(m, c);
            }

            public override void OnAttachedFromHand(EffectContext ctx) { Recover(ctx); }
            public override void OnPokemonCheckup(EffectContext src) { Recover(src); }
            public override void OnEndOfTurn(EffectContext src) { Recover(src); }
        }
    }
}
