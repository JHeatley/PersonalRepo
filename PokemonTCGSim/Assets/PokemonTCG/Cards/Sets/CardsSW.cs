using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Secret Wonders (SW, 2007; v172): the cards not already in the pool (the others are prints). Unlimited only. Rares and LV.X
    /// here; Uncommons, Commons and Trainers in CardsSWB.cs.
    /// </summary>
    public static partial class CardsSW
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SW-1", new AmpharosJamming());
            r.RegisterByKey("SW-2", new BlastoiseWaterlog());
            r.RegisterByKey("SW-3", new CharizardFuryBlaze());
            r.RegisterByKey("SW-4", new EnteiBurningCoat());
            r.RegisterByKey("SW-5", new FlygonIrritatingBuzz());
            r.RegisterByKey("SW-6", new ComboMon(new SonicBladeMon(), new PsychicCutMon()));
            r.RegisterByKey("SW-7", new GardevoirTelepass());
            r.RegisterByKey("SW-8", new GastrodonOsmoticPressure());
            r.RegisterByKey("SW-9", new ComboMon(new RagingFloodMon(), new WildWavesMon()));
            r.RegisterByKey("SW-10", new HoOhPhoenixTurn());
            r.RegisterByKey("SW-11", new JumpluffCottonSpore());
            r.RegisterByKey("SW-12", new ComboMon(new TongueReelMon(), new LockAfterAttackMon("Boundless Power", false)));
            r.RegisterByKey("SW-13", new CardsSS.RainDishMon(new NaturePowerMon()));
            r.RegisterByKey("SW-14", new ComboMon(new SilverWingMon(), new PsychicDestructionMon()));
            r.RegisterByKey("SW-15", new ComboMon(new PsychicBalanceMon(), new ReCreationMon()));
            r.RegisterByKey("SW-16", new RaikouThunderRumble());
            r.RegisterByKey("SW-17", new ComboMon(new CardsCES1.ConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned), new CardsXYP2.MagicalLeafMon(30, 30)));
            r.RegisterByKey("SW-18", new ComboMon(new CardsMA.SwiftMon("Direct Hit"), new DragonFinishMon()));
            r.RegisterByKey("SW-19", new SuicuneAquaRecover());
            r.RegisterByKey("SW-20", new VenusaurMiracleAroma());
            r.RegisterByKey("SW-131", new GardevoirLvXTeleportation());
            r.RegisterByKey("SW-132", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 40), new DarknessWingMon()));
            RegisterUncommonsAndTrainers(r);
        }

        /// <summary>
        /// Uses the effect of a Supporter card from a pile as the effect of a power or attack (Telepass, Psychic Research). The card
        /// stays where it is. Only Supporters whose effect would do something can be chosen. Returns false if there was none.
        /// </summary>
        internal static bool UseSupporterFrom(EffectContext ctx, List<CardInstance> pile, string what)
        {
            var usable = new List<CardInstance>();
            var seen = new HashSet<string>();
            foreach (var card in pile)
                if (card.Def.IsSupporter && seen.Add(card.Def.Name) && MrMimeTEF.WouldDoSomething(ctx, card)) usable.Add(card);
            if (usable.Count == 0) return false;
            var chosen = ctx.ChooseCards(usable, 1, 1, what + ": use the effect of which Supporter card?")[0];
            ctx.Log(ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + " (" + what + ").");
            var home = pile == ctx.Opp.Discard ? ctx.Opp.Discard : ctx.Me.Discard;
            ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
            ctx.Me.Hand.Remove(chosen);
            ctx.Me.Deck.Remove(chosen);
            if (home != ctx.Me.Discard) ctx.Me.Discard.Remove(chosen);
            if (!home.Contains(chosen)) home.Add(chosen);
            return true;
        }

        /// <summary>Jamming (Poké-Body): after the opponent plays a Supporter from their hand, 1 damage counter on each of their Pokémon (1 Jamming a turn). Cluster Bolt.</summary>
        private sealed class AmpharosJamming : Mon
        {
            public override void OnOpponentPlayedSupporter(EffectContext src, int player, CardDef supporter)
            {
                if (src.Pokemon == null || src.NamedLimitUsedThisTurn("Jamming")) return;
                src.MarkNamedLimit("Jamming");
                Announce(src, "Jamming");
                foreach (var m in src.OpponentPokemon()) src.Engine.PlaceDamageCounters(m, 1, "Jamming");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cluster Bolt") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                bool discard = lightning.Count > 0 && ctx.AskYesNo("Cluster Bolt: discard all Lightning Energy from " + me.Name + " for 20 to each Benched Pokémon with Energy?");
                ctx.DamageDefending(atk.Damage);
                if (!discard) return true;
                foreach (var card in lightning) if (me.Energy.Contains(card)) ctx.Engine.DiscardEnergy(me, card);
                foreach (var m in ctx.OpponentBench().Where(b => b.Energy.Count > 0).ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>
        /// Waterlog (Poké-Power): once during your turn, attach any number of basic Water Energy cards from your hand to your Pokémon
        /// in any way you like; if you do, your turn ends. Not with a Special Condition. Hydro Pump: 20 more for each Water Energy not
        /// used for the cost, no more than 40.
        /// </summary>
        private sealed class BlastoiseWaterlog : Mon
        {
            private static readonly Mon Pump = new CardsSS.WaterExtraMon("Hydro Pump", 20);

            private static List<CardInstance> Water(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList(); }

            protected override string Activated { get { return "Waterlog"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Water(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int attached = 0;
                for (int i = 0; i < 60; i++)
                {
                    var water = Water(ctx);
                    if (water.Count == 0) break;
                    var picks = ctx.ChooseCards(water, attached == 0 ? 1 : 0, 1, "Waterlog: attach which basic Water Energy card? (none to stop)");
                    if (picks.Count == 0) break;
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Waterlog: attach " + picks[0].Def.Name + " to which Pokémon?");
                    if (target == null) break;
                    ctx.Me.Hand.Remove(picks[0]);
                    if (ctx.Engine.AttachEnergyCard(picks[0], target, true)) attached++;
                }
                if (attached > 0)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (Waterlog).");
                    ctx.Engine.RequestEndTurn();
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Fury Blaze (Poké-Body): with the opponent at 3 or fewer Prize cards, Charizard's attacks do 50 more to the Active Pokémon (before W/R). Blast Burn: heads, discard 2 Energy cards; tails, 4 (can't: nothing).</summary>
        private sealed class CharizardFuryBlaze : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Opp.Prizes.Count > 3 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 50;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blast Burn") return false;
                var me = ctx.Pokemon;
                int need = ctx.Flip() ? 2 : 4;
                if (me.Energy.Count < need) return Nothing(ctx);
                foreach (var card in ctx.ChooseCards(me.Energy.ToList(), need, need, "Blast Burn: discard " + need + " Energy cards from " + me.Name)) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Burning Coat (Poké-Body): each time an opponent's Pokémon is Knocked Out by damage from Entei's attack, discard the top 3 cards of their deck. Blaze Roar.</summary>
        private sealed class EnteiBurningCoat : Mon
        {
            public override void OnOpponentPokemonKnockedOutByAttack(EffectContext src, PokemonInPlay knockedOut)
            {
                var me = src.Pokemon;
                if (me == null || knockedOut == null || knockedOut.HitByAttackerUid != me.Uid || src.Opp.Deck.Count == 0) return;
                Announce(src, "Burning Coat");
                src.Engine.DiscardTopOfDeck(src.Opponent, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Roar") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Blaze Roar: do 20 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 20);
                }
                if (!ctx.Flip() && ctx.InPlay(me))
                {
                    var fire = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire)).ToList();
                    int n = Math.Min(2, fire.Count);
                    if (n > 0) foreach (var card in ctx.ChooseCards(fire, n, n, "Blaze Roar: discard 2 Fire Energy cards")) ctx.Engine.DiscardEnergy(me, card);
                }
                return true;
            }
        }

        /// <summary>Irritating Buzz (Poké-Body): while Flygon is Active, 1 damage counter on the opponent's non-Fighting Active Pokémon between turns. Sand Sonic: 20 more if you attached a Fighting Energy card from your hand to it this turn.</summary>
        private sealed class FlygonIrritatingBuzz : Mon
        {
            private const string Marker = "Sand Sonic";

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                var target = src.Opp.Active;
                if (me == null || !src.IsActive(me) || target == null || src.Engine.IsType(target, EnergyType.Fighting)) return;
                Announce(src, "Irritating Buzz");
                src.Engine.PlaceDamageCounters(target, 1, "Irritating Buzz");
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != null && target == src.Pokemon && energy != null && energy.Def.EnergyProvides == EnergyType.Fighting) target.Markers[Marker] = src.Turn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Marker) return false;
                int turn;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 20 : 0));
                return true;
            }
        }

        /// <summary>Sonic Blade: damage counters on the Defending Pokémon until it has 50 HP left; if you put any, the opponent switches it with a Benched Pokémon.</summary>
        private sealed class SonicBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Blade") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int counters = Math.Max(0, (ctx.RemainingHp(defending) - 50) / 10);
                if (counters == 0 || !ctx.CanAffect(defending)) return true;
                ctx.PlaceCounters(defending, counters);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Psychic Cut: turn any number of your face-down Prize cards face up (for the rest of the game); 20 more for each.</summary>
        private sealed class PsychicCutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Cut") return false;
                var faceDown = ctx.Me.Prizes.Where(c => !ctx.Me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                int n = faceDown.Count == 0 ? 0 : ctx.AskNumber(0, faceDown.Count, "Psychic Cut: turn how many face-down Prize cards face up? (20 more damage each)");
                if (n > 0)
                {
                    var flipped = faceDown.Take(n).ToList();
                    foreach (var card in flipped) ctx.Me.FaceUpPrizeIds.Add(card.Id);
                    ctx.Engine.ShowCards(ctx.Opponent, flipped, "Psychic Cut", "Psychic Cut: Prize cards turned face up", ctx.Source);
                }
                ctx.DamageDefending(atk.Damage + 20 * n);
                return true;
            }
        }

        /// <summary>
        /// Telepass (Poké-Power): once during your turn, use the effect of a Supporter card in the opponent's discard pile (it stays
        /// there); 1 Telepass a turn. Not with a Special Condition. Psychic Lock: the opponent can't use Poké-Powers next turn (on the player).
        /// </summary>
        private sealed class GardevoirTelepass : Mon
        {
            protected override string Activated { get { return "Telepass"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Discard.Any(c => c.Def.IsSupporter && MrMimeTEF.WouldDoSomething(ctx, c)); }

            protected override void Ability(EffectContext ctx) { UseSupporterFrom(ctx, ctx.Opp.Discard, "Telepass"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Lock") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new PokePowerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't use Poké-Powers during their next turn (Psychic Lock)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class PokePowerLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public PokePowerLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName)
            {
                return mon != null && mon.Owner == affected && engine.State.CurrentPlayer == affected;
            }
        }

        /// <summary>Osmotic Pressure (Poké-Power): once during your turn, move up to 3 damage counters from Gastrodon East Sea to 1 of your Gastrodon West Sea. Dwindling Wave.</summary>
        private sealed class GastrodonOsmoticPressure : Mon
        {
            private static readonly Mon Wave = new CardsUPR1.LessPerSelfCounterMon("Dwindling Wave", 10);

            private static List<PokemonInPlay> West(EffectContext ctx) { return ctx.MyPokemon(m => m.Name == "Gastrodon West Sea"); }

            protected override string Activated { get { return "Osmotic Pressure"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon.DamageCounters > 0 && West(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var target = ctx.ChoosePokemon(West(ctx), "Osmotic Pressure: move damage counters to which Gastrodon West Sea?");
                if (target == null) return;
                int max = Math.Min(3, me.DamageCounters);
                int n = max == 1 ? 1 : ctx.AskNumber(1, max, "Osmotic Pressure: move how many damage counters?");
                ctx.Engine.MoveDamageCounters(me, target, n, "Osmotic Pressure");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wave.HandleAttack(ctx, atk); }
        }

        /// <summary>Raging Flood: 10 more for each damage counter on this Pokémon; then remove 2 damage counters from it.</summary>
        private sealed class RagingFloodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Flood") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>Wild Waves: 1 damage counter on each Benched Pokémon (both players').</summary>
        private sealed class WildWavesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wild Waves") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (m.Owner == ctx.Player || ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
                return true;
            }
        }

        /// <summary>
        /// Phoenix Turn (Poké-Power): once during the opponent's turn, if Ho-Oh would be Knocked Out by attack damage, flip: heads, it
        /// stays in play with all damage counters, Special Conditions and effects removed and its non-Energy attached cards discarded;
        /// it still counts as Knocked Out and the opponent takes a Prize card (Secret Wonders FAQ). Rainbow Wing.
        /// </summary>
        private sealed class HoOhPhoenixTurn : Mon
        {
            private const string Power = "Phoenix Turn";

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon || src.IsMyTurn || src.AbilityUsedThisTurn(Power) || !CardsDS.PowerReady(src)) return false;
                if (!src.AskYesNo(Power + ": flip a coin to keep " + mon.Name + " in play?")) return false;
                src.MarkAbilityUsed(Power);
                Announce(src, Power);
                return src.Flip();
            }

            public override void OnSavedFromKnockOut(EffectContext src, PokemonInPlay mon)
            {
                mon.DamageCounters = 0;
                src.Engine.RemoveAllConditions(mon);
                foreach (var tool in mon.Tools.ToList()) src.Engine.DiscardTool(mon, tool);
                src.Log(mon.Name + " rises again (Phoenix Turn); " + src.Engine.PlayerName(src.Opponent) + " takes a Prize card.");
                src.Engine.TakePrizeCards(src.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Wing") return false;
                var types = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage * types);
                return true;
            }
        }

        /// <summary>Cotton Spore (Poké-Body): when damaged by the opponent's attack, flip: heads prevents that damage. Cottonweed Punch: 2 coins, 30 per heads to 1 of the opponent's Pokémon.</summary>
        private sealed class JumpluffCottonSpore : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || damage <= 0 || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return false;
                if (!info.ApplyOnce("Cotton Spore#" + me.Uid)) return false;
                Announce(src, "Cotton Spore");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cottonweed Punch") return false;
                int heads = ctx.FlipCoins(2);
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Cottonweed Punch: choose 1 of your opponent's Pokémon");
                if (target != null && heads > 0) ctx.DamageTarget(target, 30 * heads);
                return true;
            }
        }

        /// <summary>Tongue Reel: choose 1 of the opponent's Pokémon; a Benched one is switched in first; 20 to it; heads, it's Paralyzed.</summary>
        private sealed class TongueReelMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tongue Reel") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Tongue Reel: choose 1 of your opponent's Pokémon");
                if (target == null) return true;
                if (target != ctx.Opp.Active && ctx.CanAffect(target)) ctx.Engine.SwitchActive(ctx.Opponent, target);
                ctx.DamageTarget(target, 20);
                if (ctx.Flip() && ctx.InPlay(target) && ctx.Opp.Active == target) ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Nature Power: 20 more with your Stadium in play; with the opponent's Stadium in play, the Defending Pokémon is Confused.</summary>
        private sealed class NaturePowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nature Power") return false;
                var defending = ctx.Defending;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                ctx.DamageDefending(atk.Damage + (mine ? 20 : 0));
                if (theirs && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Silver Wing: heads, an Energy card on the Defending Pokémon goes back to the opponent's hand.</summary>
        private sealed class SilverWingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silver Wing") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Silver Wing: return which Energy card to your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, card);
                return true;
            }
        }

        /// <summary>Psychic Destruction: base 40 if the Defending Pokémon has any Energy attached.</summary>
        private sealed class PsychicDestructionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Destruction") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.Defending.Energy.Count > 0 ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Psychic Balance: with fewer cards in hand than the opponent, draw until you have as many (else nothing).</summary>
        internal sealed class PsychicBalanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Balance") return false;
                if (ctx.Me.Hand.Count >= ctx.Opp.Hand.Count) return Nothing(ctx);
                ctx.DrawUntil(ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Re-creation: copy an attack of a Pokémon in the opponent's discard pile (no Energy cost) and use it.</summary>
        private sealed class ReCreationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Re-creation") return false;
                var pokemon = ctx.Opp.Discard.Where(c => c.Def.IsPokemon && c.Def.Attacks.Count > 0).GroupBy(c => c.Def.Key).Select(g => g.First()).ToList();
                if (pokemon.Count == 0) return Nothing(ctx);
                var card = ctx.ChooseCards(pokemon, 1, 1, "Re-creation: copy an attack of which Pokémon in your opponent's discard pile?")[0];
                int index = ChooseAttackOf(ctx, card.Def, "Re-creation: copy which attack?");
                if (index < 0) return Nothing(ctx);
                UseCopiedAttack(ctx, card.Def, index);
                return true;
            }
        }

        /// <summary>
        /// Thunder Rumble (Poké-Power): once during your turn, when you attach a Lightning Energy card from your hand to Raikou, you may
        /// put 1 damage counter on 1 of the opponent's Benched Pokémon. Thunder Climb: discard the top 3 cards of your deck; 10 more
        /// for each Lightning Energy card discarded; then attach them to 1 of your Pokémon.
        /// </summary>
        private sealed class RaikouThunderRumble : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || energy.Def.EnergyProvides != EnergyType.Lightning || !energy.Def.IsEnergy) return;
                if (src.OpponentBench().Count == 0 || !CardsDS.PowerReady(src)) return;
                if (!TriggerOnce(src, "Thunder Rumble", true, "Thunder Rumble: put 1 damage counter on 1 of your opponent's Benched Pokémon?")) return;
                var bench = src.OpponentBench();
                var pick = src.ChoosePokemon(bench, "Thunder Rumble: put 1 damage counter on which Benched Pokémon?");
                if (pick != null && src.CanAffect(pick)) src.PlaceCounters(pick, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Climb") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                var lightning = milled.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
                ctx.DamageDefending(atk.Damage + 10 * lightning.Count);
                if (lightning.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Thunder Climb: attach the Lightning Energy cards to which of your Pokémon?");
                if (target == null) return true;
                foreach (var card in lightning)
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Dragon Finish: discard 2 basic Fire Energy cards (100 to the Defending Pokémon) or 2 basic Water Energy cards (100 to 1 Benched Pokémon); can't: nothing.</summary>
        private sealed class DragonFinishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Finish") return false;
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(e => e.Def.IsBasicEnergy && e.Def.EnergyProvides == EnergyType.Fire).ToList();
                var water = me.Energy.Where(e => e.Def.IsBasicEnergy && e.Def.EnergyProvides == EnergyType.Water).ToList();
                bool canFire = fire.Count >= 2, canWater = water.Count >= 2 && ctx.OpponentBench().Count > 0;
                if (!canFire && !canWater) return Nothing(ctx);
                bool useFire = canFire && (!canWater || ctx.ChooseOption("Dragon Finish: discard which Energy?", "2 Fire Energy (100 to the Defending Pokémon)", "2 Water Energy (100 to a Benched Pokémon)") == 0);
                foreach (var card in (useFire ? fire : water).Take(2).ToList()) ctx.Engine.DiscardEnergy(me, card);
                if (useFire) ctx.DamageDefending(100);
                else
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Dragon Finish: do 100 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 100);
                }
                return true;
            }
        }

        /// <summary>Aqua Recover (Poké-Power): when you put Suicune from your hand onto your Bench, up to 3 Water Pokémon from your discard pile into your hand. Cure Stream.</summary>
        private sealed class SuicuneAquaRecover : Mon
        {
            private static readonly Mon Stream = new CardsMEG.WeakenDefendingMon("Cure Stream", 20);

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Water)).ToList(); }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Engine.DiscardToHandBlocked(ctx.Player) || !TriggerOnce(ctx, "Aqua Recover", Pile(ctx).Count > 0, "Aqua Recover: put up to 3 Water Pokémon from your discard pile into your hand?")) return;
                var pile = Pile(ctx);
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, Math.Min(3, pile.Count), "Aqua Recover: put which Water Pokémon into your hand?"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Stream.HandleAttack(ctx, atk); }
        }

        /// <summary>Miracle Aroma (Poké-Power): once during your turn, heads makes the Defending Pokémon Asleep, Burned or Poisoned (your choice). Not with a Special Condition. Giant Bloom.</summary>
        private sealed class VenusaurMiracleAroma : Mon
        {
            private static readonly Mon Bloom = new CardsBCR1.FlipHealSelfMon("Giant Bloom", 40);

            protected override string Activated { get { return "Miracle Aroma"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var target = ctx.Opp.Active;
                if (target == null || !ctx.CanAffect(target)) return;
                var conditions = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Poisoned };
                int pick = ctx.ChooseOneOption("Miracle Aroma: the Defending Pokémon is now...", "Asleep", "Burned", "Poisoned");
                ctx.ApplyCondition(target, conditions[Math.Max(0, Math.Min(2, pick))]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bloom.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Teleportation (Poké-Power): once during your turn, switch Gardevoir with your Active Pokémon or 1 of your Benched Pokémon.
        /// Not with a Special Condition. Bring Down: the Pokémon in play (either player's, not Gardevoir) with the fewest HP left is
        /// Knocked Out; you choose among ties, and must choose 1 even if it's yours.
        /// </summary>
        private sealed class GardevoirLvXTeleportation : Mon
        {
            protected override string Activated { get { return "Teleportation"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Bench.Count > 0 && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.IsActive(me))
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Teleportation: switch Gardevoir with which Benched Pokémon?");
                    if (target != null) ctx.Engine.SwitchActive(ctx.Player, target);
                }
                else ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bring Down") return false;
                var me = ctx.Pokemon;
                var others = ctx.MyPokemon(m => m != me).Concat(ctx.OpponentPokemon()).ToList();
                if (others.Count == 0) return true;
                int low = others.Min(m => ctx.RemainingHp(m));
                var lowest = others.Where(m => ctx.RemainingHp(m) == low).ToList();
                var target = ctx.ChoosePokemon(lowest, "Bring Down: Knock Out which Pokémon?");
                if (target != null && (target.Owner == ctx.Player || ctx.CanAffect(target))) ctx.Engine.KnockOut(target, "Bring Down");
                return true;
            }
        }

        /// <summary>Darkness Wing: if the Defending Pokémon would be Knocked Out by this damage, you may put any 1 card from your discard pile into your hand.</summary>
        private sealed class DarknessWingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Wing") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool knocked = defending != null && (!ctx.InPlay(defending) || ctx.Engine.IsKnockedOut(defending) || defending.SavedTo10HpTurn == ctx.Turn);
                if (!knocked || ctx.Me.Discard.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                if (!ctx.AskYesNo("Darkness Wing: put a card from your discard pile into your hand?")) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Darkness Wing: put which card into your hand?"));
                return true;
            }
        }
    }
}
