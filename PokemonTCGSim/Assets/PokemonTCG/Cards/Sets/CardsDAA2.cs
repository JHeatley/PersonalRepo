using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Darkness Ablaze (DAA, 2020), Pokémon 84-156 (Fighting, Darkness, Metal and Colorless), the Trainers and the Special
    /// Energy. Regulation mark D: Expanded only.
    /// Already in the pool: Trapinch, Galarian Sirfetch'd, Spinarak, Ariados, Crobat V, Purrloin, Liepard, Hoopa, Grimmsnarl V /
    /// VMAX, Eternatus V / VMAX, Cufant, Copperajah, Ducklett, Swanna, Bunnelby, Bird Keeper, Cape of Toughness, Kabu, Piers
    /// and Rose; the secret rare Rillaboom and Coalossal too. Rare Fossil's play rules are in the engine (CardDef.IsFossil with
    /// FossilGivesPrize).
    /// Pokémon whose attacks only do their printed damage need no script (Diglett, Larvitar, Diggersby, Deino, Zweilous,
    /// Aron, Meltan, Tauros, Fletchling).
    /// </summary>
    public static class CardsDAA2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("DAA-85", new DodgeMon("Dig", false));
            r.RegisterByKey("DAA-87", new CardsEVS1.EvolveFromDeckAttackMon("Rocket Evolution"));
            r.RegisterByKey("DAA-88", new ComboMon(new CardsPAF1.DiscardStadiumMon("Bedrock Breaker"), new CardsBRS1.MillSelfMon("Mountain Swing", 5)));
            r.RegisterByKey("DAA-90", new CardsMEW1.SpreadBenchMon("Sand Pulse", 10));
            r.RegisterByKey("DAA-91", new FlygonDAA());
            r.RegisterByKey("DAA-92", new SolrockDAA());
            r.RegisterByKey("DAA-93", new CardsWHT.FlipMoreMon("Double Stomp", 2, 30));
            r.RegisterByKey("DAA-94", new CoinsMon("Triple Smash", 3));
            r.RegisterByKey("DAA-95", new ComboMon(new CardsDRI2.CrunchMon("Drill Run"), new LockAfterAttackMon("Heavy Rock Artillery", true)));
            r.RegisterByKey("DAA-97", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new RockHurlMon()));
            // Darkness
            r.RegisterByKey("DAA-99", new GalarianSlowbroV());
            r.RegisterByKey("DAA-100", new CardsSSP2.StompOffMon("Stomp Off", 1));
            r.RegisterByKey("DAA-101", new TriplePoisonMon("Triple Poison", 3));
            r.RegisterByKey("DAA-105", new DarkraiDAA());
            r.RegisterByKey("DAA-110", new HydreigonDAA());
            r.RegisterByKey("DAA-112", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("DAA-113", new SearchAttackMon("Nasty Plot", AnyCard, 2));
            // Metal
            r.RegisterByKey("DAA-118", new HackOffMon());
            r.RegisterByKey("DAA-119", new ShieldAttackMon("Hard Scissors", 30));
            r.RegisterByKey("DAA-120", new MetalArmsMon());
            r.RegisterByKey("DAA-122", new FlipBonusMon("Knock Away", 30));
            r.RegisterByKey("DAA-123", new AggronDAA());
            r.RegisterByKey("DAA-124", new FuryCutterMon());
            r.RegisterByKey("DAA-125", new SearchAttackMon("Call for Backup", IsMetalPokemon, 1));
            r.RegisterByKey("DAA-126", new SearchAttackMon("Call for Backup", IsMetalPokemon, 1));
            r.RegisterByKey("DAA-127", new ClockworkMon());
            r.RegisterByKey("DAA-128", new GalarianStunfiskV());
            r.RegisterByKey("DAA-130", new EnergyLinkMon());
            // Colorless
            r.RegisterByKey("DAA-133", new Cards30C.RetaliateBonusMon("Rally Back", 90));
            r.RegisterByKey("DAA-135", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("DAA-136", new ComboMon(new DrawAttackMon("Feelin' Fine", 3), new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("DAA-137", new DunsparceDAA());
            r.RegisterByKey("DAA-138", new NoRetreatMon("Baby-Doll Eyes"));
            r.RegisterByKey("DAA-139", new CardsSSP2.StompOffMon("Hammer Arm", 1));
            r.RegisterByKey("DAA-140", new CardsVIV1.AnyEnergyToHandMon("Energy Loop", 1));
            r.RegisterByKey("DAA-141", new SearchAttackMon("Drawup Power", IsEnergyCard, 1));
            r.RegisterByKey("DAA-142", new CardsDAA1.GustAttackMon("Captivating Tail", true));
            r.RegisterByKey("DAA-143", new CardsCEC3.SpreadAmountMon("Swoop Across", 30));
            r.RegisterByKey("DAA-144", new ComboMon(new SonicDoubleMon(), new LockAfterAttackMon("Max Wings", true)));
            r.RegisterByKey("DAA-145", new CardsSHF1.SkyCircusMon(new SearchAttackMon("Keen Eye", AnyCard, 2)));
            r.RegisterByKey("DAA-146", new FlipBonusMon("Ambush", 20));
            r.RegisterByKey("DAA-147", new ComboMon(new HurricaneBlenderMon(), new RecoilMon("Brave Bird", 30)));
            r.RegisterByKey("DAA-152", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("DAA-153", new ComboMon(new CardsBLK.PeckOffMon("Scrape Off"), new SmackAndRunMon()));
            r.RegisterByKey("DAA-154", new CardsBLK.PeckOffMon("Pluck"));
            r.RegisterByKey("DAA-155", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("DAA-156", new CorviknightDAA());

            // Trainers
            r.RegisterByName("Big Parasol", new BigParasol());
            r.RegisterByName("Billowing Smoke", new BillowingSmoke());
            r.RegisterByName("Familiar Bell", new FamiliarBell());
            r.RegisterByName("Glimwood Tangle", new GlimwoodTangle());
            r.RegisterByName("Old PC", new OldPC());
            r.RegisterByName("Pokémon Breeder's Nurturing", new BreedersNurturing());
            // Rare Fossil's play rules are in the engine, from its card data (CardDef.IsFossil): 70 HP, no Special Conditions, a
            // Prize card when Knocked Out, like Unidentified Fossil's.
            r.RegisterByName("Rare Fossil", new CardScript());
            r.RegisterByName("Rose Tower", new RoseTower());
            r.RegisterByName("Spikemuth", new Spikemuth());
            r.RegisterByName("Struggle Gloves", new StruggleGloves());
            r.RegisterByName("Turbo Patch", new TurboPatch());
            r.RegisterByName("Yell Horn", new YellHorn());
            // Special Energy
            r.RegisterByName("Heat Fire Energy", new HeatFireEnergy());
            r.RegisterByName("Hiding Darkness Energy", new HidingDarknessEnergy());
            r.RegisterByName("Powerful Colorless Energy", new PowerfulColorlessEnergy());
            r.RegisterByName("Capture Energy", new CaptureEnergy());
        }

        private static bool AnyCard(CardDef d) { return true; }
        private static bool IsEnergyCard(CardDef d) { return d.IsEnergy; }
        private static bool IsMetalPokemon(CardDef d) { return d.IsPokemon && d.HasType(EnergyType.Metal); }

        // ================================================================== Fighting

        /// <summary>
        /// Labyrinth of Sand: while this Pokémon is Active, the opponent's Active Pokémon can't retreat. Desert Geyser: if your
        /// opponent has a Stadium in play, discard it; if you did, prevent all damage and effects of attacks done to this Pokémon
        /// during their next turn.
        /// </summary>
        private sealed class FlygonDAA : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                var self = src.Pokemon;
                return self != null && mon != null && mon.Owner != src.Player && src.State.IsActive(self) && src.State.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Desert Geyser") return false;
                ctx.DamageDefending(atk.Damage);
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                if (theirs && CardsTWM1.DiscardStadiumInPlay(ctx) && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Resistance Shade: if you have Lunatone in play, your opponent's Pokémon have no Resistance.</summary>
        private sealed class SolrockDAA : Mon
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Target != null && info.Target.Owner != src.Player
                    && src.Me.InPlay().Any(m => m.Name == "Lunatone");
            }
        }

        /// <summary>Rock Hurl: this attack's damage isn't affected by Resistance.</summary>
        internal sealed class RockHurlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Hurl") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Rapid-Fire Poison: once during your turn, if this Pokémon is Active, the opponent's Active Pokémon is now Poisoned. Tripping Shot: can't retreat.</summary>
        private sealed class GalarianSlowbroV : Mon
        {
            private static readonly Mon TrippingShot = new NoRetreatMon("Tripping Shot");

            protected override string Activated { get { return "Rapid-Fire Poison"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Opp.Active != null && !ctx.Opp.Active.Has(SpecialCondition.Poisoned);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TrippingShot.HandleAttack(ctx, atk); }
        }

        /// <summary>Triple Poison: the opponent's Active Pokémon is now Poisoned, with 3 damage counters in Pokémon Checkup instead of 1. Used for Triple Poison, Tormenting Poison, Severe Poison (the attack name and the numbers are passed in).</summary>
        internal sealed class TriplePoisonMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public TriplePoisonMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = counters;
                return true;
            }
        }

        /// <summary>Darkness Guard: with any Darkness Energy attached, this Pokémon takes 20 less damage from attacks. Vortex of Darkness: 20 more per Darkness Energy.</summary>
        private sealed class DarkraiDAA : Mon
        {
            private static readonly Mon Vortex = new CardsMEG.EnergyBonusMon("Vortex of Darkness", EnergyType.Darkness, 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.Engine.EnergyCountOfType(me, EnergyType.Darkness) > 0 ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Vortex.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Squall: as often as you like during your turn, attach a Darkness Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class HydreigonDAA : Mon
        {
            private const string Squall = "Dark Squall";

            private static bool DarknessCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Darkness); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Squall || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(c => DarknessCard(c.Def)) && ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, DarknessCard, () => ctx.MyPokemon(), Squall, "a Darkness Energy card");
            }
        }

        // ================================================================== Metal

        /// <summary>Hack Off: discard a Pokémon Tool and a Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class HackOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hack Off") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                if (defending.Tools.Count > 0)
                {
                    var tool = defending.Tools.Count == 1 ? defending.Tools[0]
                        : ctx.ChooseCards(defending.Tools.ToList(), 1, 1, "Hack Off: discard which Pokémon Tool?")[0];
                    ctx.Engine.DiscardTool(defending, tool);
                }
                var special = defending.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                if (special.Count > 0)
                {
                    var pick = ctx.ChooseCards(special, 1, 1, "Hack Off: discard which Special Energy?")[0];
                    ctx.Engine.DiscardEnergy(defending, pick);
                }
                return true;
            }
        }

        /// <summary>Metal Arms: 40 more damage if this Pokémon has a Pokémon Tool attached.</summary>
        private sealed class MetalArmsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Arms") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Tools.Count > 0 ? 40 : 0));
                return true;
            }
        }

        /// <summary>Sturdy: with full HP, a Knock Out by attack damage leaves it at 10 HP instead.</summary>
        private sealed class AggronDAA : Mon
        {
            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }
        }

        /// <summary>Fury Cutter: flip 3 coins; 1 heads 20 more, 2 heads 70 more, 3 heads 140 more.</summary>
        private sealed class FuryCutterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                int more = heads == 1 ? 20 : heads == 2 ? 70 : heads >= 3 ? 140 : 0;
                ctx.DamageDefending(atk.Damage + more);
                return true;
            }
        }

        /// <summary>Clockwork: does nothing unless you have Klink and Klang on your Bench.</summary>
        private sealed class ClockworkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clockwork") return false;
                bool klink = ctx.Me.Bench.Any(b => b.Name == "Klink");
                bool klang = ctx.Me.Bench.Any(b => b.Name == "Klang");
                if (!klink || !klang) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Metal Skin: +20 HP for each Metal Energy attached. Trapping Bite: during the opponent's next turn, if this Pokémon is
        /// damaged by an attack (even if it's Knocked Out), put 12 damage counters on the Attacking Pokémon.
        /// </summary>
        private sealed class GalarianStunfiskV : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return hp;
                return hp + 20 * src.Engine.EnergyCountOfType(mon, EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trapping Bite") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new TrappingBiteEffect(ctx.Pokemon.Name + " puts 12 damage counters on the Pokémon that damages it (Trapping Bite)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class TrappingBiteEffect : ContinuousEffect
        {
            public TrappingBiteEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage < 10) return;
                if (engine.State.FindPokemon(info.Attacker.Uid) != info.Attacker) return;
                engine.PlaceDamageCounters(info.Attacker, 12, "Trapping Bite");
            }
        }

        /// <summary>Energy Link: attach an Energy card from your discard pile to this Pokémon.</summary>
        internal sealed class EnergyLinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Link") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Energy Link: attach which Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Final Dig: if this Pokémon is Knocked Out by damage from the opponent's attack, discard the top 2 cards of their deck.</summary>
        private sealed class DunsparceDAA : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Opp.Deck.Count == 0) return;
                Announce(src, "Final Dig");
                src.Engine.DiscardTopOfDeck(src.Opponent, 2);
            }
        }

        /// <summary>Sonic Double: 40 damage to 2 of your opponent's Pokémon (Weakness and Resistance only for the Active one).</summary>
        private sealed class SonicDoubleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Double") return false;
                var all = ctx.OpponentPokemon();
                int n = Math.Min(2, all.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(all, n, n, "Sonic Double: choose " + n + " of your opponent's Pokémon"))
                    ctx.DamageTarget(target, 40);
                return true;
            }
        }

        /// <summary>Hurricane Blender: then move any amount of Energy from your Pokémon to your other Pokémon in any way you like.</summary>
        private sealed class HurricaneBlenderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hurricane Blender") return false;
                ctx.DamageDefending(atk.Damage);
                CardsSIT1.MoveEnergyAnyWay(ctx, "Hurricane Blender");
                return true;
            }
        }

        /// <summary>Smack and Run: put this Pokémon and all attached cards into your hand.</summary>
        private sealed class SmackAndRunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smack and Run") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Flying Taxi: when you play this Pokémon from your hand to evolve, you may put 1 of your Pokémon, except any Corviknight, and all attached cards into your hand.</summary>
        private sealed class CorviknightDAA : Mon
        {
            private static List<PokemonInPlay> Riders(EffectContext ctx) { return ctx.MyPokemon(m => m.Name != "Corviknight"); }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Flying Taxi", Riders(ctx).Count > 0, "Flying Taxi: put 1 of your Pokémon (not a Corviknight) and all attached cards into your hand?")) return;
                var rider = ctx.ChoosePokemon(Riders(ctx), "Flying Taxi: put which Pokémon into your hand?");
                if (rider != null) ctx.Engine.ReturnPokemonToHand(rider);
            }
        }

        // ================================================================== Trainers

        /// <summary>Big Parasol: while its holder is Active, prevent all effects of the opponent's attacks done to all of your Pokémon (not damage).</summary>
        private sealed class BigParasol : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && src.State.IsActive(src.Pokemon) && target != null && target.Owner == src.Player && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Billowing Smoke: if its holder is Knocked Out by damage from the opponent's attack, they discard the Prize cards they would take for it.</summary>
        private sealed class BillowingSmoke : CardScript
        {
            public override bool DiscardsPrizesForHolder(EffectContext src) { return src.Pokemon != null; }
        }

        /// <summary>Familiar Bell: search your deck for a Pokémon with the same name as a Pokémon in your discard pile.</summary>
        private sealed class FamiliarBell : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var names = new HashSet<string>(ctx.Me.Discard.Where(c => c.Def.IsPokemon).Select(c => c.Def.Name));
                ctx.SearchToHand(d => d.IsPokemon && names.Contains(d.Name), 1, "Familiar Bell: choose a Pokémon with the same name as one in your discard pile");
            }
        }

        /// <summary>Glimwood Tangle: once during each player's turn, after flipping coins for an attack they may flip them again (the engine asks).</summary>
        private sealed class GlimwoodTangle : CardScript
        {
            public override bool StadiumOffersAttackReflip(EffectContext src, PokemonInPlay attacker) { return attacker != null; }
        }

        /// <summary>Old PC: flip 2 coins; if both are heads, put a card from your discard pile into your hand.</summary>
        private sealed class OldPC : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.FlipCoins(2) < 2 || ctx.Me.Discard.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Old PC: put which card from your discard pile into your hand?");
                ctx.DiscardToHand(pick);
            }
        }

        /// <summary>
        /// Pokémon Breeder's Nurturing: choose up to 2 of your Pokémon; evolve each with a card from your deck. Not during your first
        /// turn, nor on a Pokémon put into play this turn (your ruling: one that already evolved this turn can evolve again).
        /// Evolving from the deck isn't from the hand, so Dracovish's Primal Law doesn't stop it (Compendium, 2020-08-13).
        /// </summary>
        private sealed class BreedersNurturing : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.TurnEnteredPlay != ctx.Turn && !CardsEVS1.EvolveBlocked(ctx, m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return !ctx.IsMyFirstTurn && ctx.Me.Deck.Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var picks = ctx.ChoosePokemonMany(targets, 1, Math.Min(2, targets.Count), "Pokémon Breeder's Nurturing: choose up to 2 of your Pokémon to evolve");
                foreach (var mon in picks)
                {
                    if (!ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) continue;
                    var target = mon;
                    var found = ctx.SearchMyDeck(d => CardsEVS1.EvolvesOnto(ctx, target, d), 1, "Pokémon Breeder's Nurturing: choose a card that evolves from " + target.Name, false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(target, found[0]);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Rose Tower: once during each player's turn, that player may draw cards until they have 3 cards in their hand.</summary>
        private sealed class RoseTower : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count < 3 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.DrawUntil(3);
                return false;
            }
        }

        /// <summary>Spikemuth: whenever a player's Active Pokémon moves to the Bench during their turn, put 2 damage counters on it.</summary>
        private sealed class Spikemuth : CardScript
        {
            public override void OnActiveMovedToBenchUnderStadium(EffectContext src, int player, PokemonInPlay movedToBench)
            {
                if (movedToBench == null || src.State.FindPokemon(movedToBench.Uid) != movedToBench) return;
                src.Engine.PlaceDamageCounters(movedToBench, 2, "Spikemuth");
            }
        }

        /// <summary>Struggle Gloves: if the holder is weak to the opponent's Active Pokémon's type, its attacks do 30 more damage to that Pokémon (before Weakness and Resistance).</summary>
        private sealed class StruggleGloves : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                var weakness = src.Engine.WeaknessOf(me);
                return weakness.HasValue && src.Engine.IsType(info.Target, weakness.Value) ? damage + 30 : damage;
            }
        }

        /// <summary>Turbo Patch: flip a coin; heads, attach a basic Energy card from your discard pile to 1 of your Basic Pokémon that isn't a Pokémon-GX.</summary>
        private sealed class TurboPatch : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsBasicPokemon && !m.Def.IsGX); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var pile = ctx.BasicEnergyInDiscard();
                var targets = Targets(ctx);
                if (pile.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(pile, 1, 1, "Turbo Patch: attach which basic Energy card?")[0];
                var target = ctx.ChoosePokemon(targets, "Turbo Patch: attach " + card.Def.Name + " to which of your Basic Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Yell Horn: both Active Pokémon are now Confused.</summary>
        private sealed class YellHorn : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null || ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Confused);
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Confused);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Heat Fire Energy: provides Fire; its Fire Pokémon gets +20 HP.</summary>
        private sealed class HeatFireEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Fire), 1); }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Fire) ? hp + 20 : hp;
            }
        }

        /// <summary>Hiding Darkness Energy: provides Darkness; its Darkness Pokémon has no Retreat Cost.</summary>
        private sealed class HidingDarknessEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Darkness), 1); }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Darkness) ? cost - 100 : cost;
            }
        }

        /// <summary>Powerful Colorless Energy: provides Colorless; its Colorless Pokémon's attacks do 20 more damage to the opponent's Active Pokémon.</summary>
        private sealed class PowerfulColorlessEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Colorless), 1); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(me, EnergyType.Colorless) ? damage + 20 : damage;
            }
        }

        /// <summary>Capture Energy: provides Colorless; attached from your hand, search your deck for a Basic Pokémon and put it onto your Bench.</summary>
        private sealed class CaptureEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Colorless), 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0 || ctx.Me.Deck.Count == 0) return;
                ctx.SearchToBench(AnyCard, 1, "Capture Energy: choose a Basic Pokémon to put onto your Bench");
            }
        }
    }
}
