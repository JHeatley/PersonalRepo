using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Up (TEU, v80), part 2: Lightning and Psychic (TEU 34-70). Pikachu &amp; Zekrom-GX (TEU 33) is the card already in the
    /// pool as 30C CC25. Electropower (Lost Thunder) is found by name.
    /// </summary>
    public static class CardsTEU2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("TEU-34", new RecoilMon("Self-Destruct", 60));
            r.RegisterByKey("TEU-35", new ComboMon(new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 2), new TailsNothingMon("Smash Bomb")));
            r.RegisterByKey("TEU-37", new AlolanGolemTEU());
            r.RegisterByKey("TEU-39", new ElectrodeTEU());
            r.RegisterByKey("TEU-40", new ZapdosTEU());
            r.RegisterByKey("TEU-41", new DiscardAllTypeMon("Shock Bolt", EnergyType.Lightning));
            r.RegisterByKey("TEU-42", new DiscardAllTypeMon("Shock Bolt", EnergyType.Lightning));
            r.RegisterByKey("TEU-43", new AmpharosGX());
            r.RegisterByKey("TEU-44", new SearchAttackMon("Delivery Dash", d => d.Name == Electropower, 2));
            r.RegisterByKey("TEU-45", new CardsFST2.ConditionBonusMon("Raid", 90, EvolvedFromBlitzleThisTurn));
            r.RegisterByKey("TEU-46", new EmolgaTEU());
            r.RegisterByKey("TEU-47", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("TEU-48", new GalvantulaTEU());
            r.RegisterByKey("TEU-50", new ComboMon(new CardsMEG.SnipeMon("Random Spark", 30), new VoltSwitchMon("Volt Switch", EnergyType.Lightning)));
            r.RegisterByKey("TEU-51", new TapuKokoPrismStar());
            r.RegisterByKey("TEU-52", new RecoilMon("Wild Charge", 20));
            // Psychic
            r.RegisterByKey("TEU-53", new GengarMimikyuGX());
            r.RegisterByKey("TEU-54", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("TEU-55", new FamilyRescueMon());
            r.RegisterByKey("TEU-56", new NidoqueenTEU());
            r.RegisterByKey("TEU-59", new ComboMon(new CardsWHT.DragOffMon("Drag Off", 50), new CardsFST2.ConditionBonusMon("King's Drum", 100, ctx => ctx.MyBench().Any(m => m.Name == "Nidoqueen"))));
            r.RegisterByKey("TEU-60", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("TEU-61", new TentacruelTEU());
            r.RegisterByKey("TEU-63", new MukTEU());
            r.RegisterByKey("TEU-64", new AlolanMarowakTEU());
            r.RegisterByKey("TEU-65", new StarmieTEU());
            r.RegisterByKey("TEU-66", new MrMimeTEU());
            r.RegisterByKey("TEU-67", new MrMimeGX());
            r.RegisterByKey("TEU-68", new ComboMon(new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused), new MysteriousDanceMon()));
            r.RegisterByKey("TEU-69", new CardsDRM1.SubmergeMon());
            r.RegisterByKey("TEU-70", new CardsTWM1.HealSelfMon("Nap", 20));
        }

        internal const string Electropower = "Electropower";

        private static bool EvolvedFromBlitzleThisTurn(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            return me != null && me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Blitzle";
        }

        /// <summary>Discards every Energy attached to the Pokémon that provides the type. Returns how many were discarded.</summary>
        internal static int DiscardAllOfType(EffectContext ctx, PokemonInPlay mon, EnergyType type)
        {
            if (mon == null || !ctx.InPlay(mon)) return 0;
            int n = 0;
            foreach (var card in mon.Energy.Where(c => CardsDRI1.ProvidesType(ctx, mon, c, type)).ToList())
                if (ctx.Engine.DiscardEnergy(mon, card)) n++;
            return n;
        }

        /// <summary>"Discard all [T] Energy from this Pokémon" after the damage (Shock Bolt, Impact Bolt).</summary>
        internal sealed class DiscardAllTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public DiscardAllTypeMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                DiscardAllOfType(ctx, ctx.Pokemon, type);
                return true;
            }
        }

        /// <summary>"Switch this Pokémon with 1 of your Benched [T] Pokémon" after the damage (Volt Switch).</summary>
        internal sealed class VoltSwitchMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public VoltSwitchMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !ctx.IsActive(me)) return true;
                var bench = ctx.MyBench().Where(m => ctx.Engine.IsType(m, type)).ToList();
                if (bench.Count == 0) return true;
                var to = ctx.ChoosePokemon(bench, attack + ": switch " + me.Name + " with which Benched " + type + " Pokémon?");
                ctx.Engine.SwitchActive(ctx.Player, to);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Electromagnetic Bomb: move any number of Lightning Energy from your Benched Pokémon to this Pokémon; 20 damage for each
        /// Energy moved. Super Zap Cannon: discard 2 Energy from this Pokémon.
        /// </summary>
        private sealed class AlolanGolemTEU : Mon
        {
            private static readonly Mon Cannon = new CardsTWM1.SelfDiscardMon("Super Zap Cannon", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Cannon.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Electromagnetic Bomb") return false;
                var me = ctx.Pokemon;
                int moved = 0;
                for (int guard = 0; guard < 60; guard++)
                {
                    var sources = ctx.MyBench().Where(m => m.Energy.Any(c => CardsDRI1.ProvidesType(ctx, m, c, EnergyType.Lightning))).ToList();
                    if (sources.Count == 0) break;
                    if (!ctx.AskYesNo("Electromagnetic Bomb: move a Lightning Energy from your Bench to " + me.Name + "? (" + moved + " moved so far)")) break;
                    var from = ctx.ChoosePokemon(sources, "Electromagnetic Bomb: move a Lightning Energy from which Benched Pokémon?");
                    var energy = from.Energy.Where(c => CardsDRI1.ProvidesType(ctx, from, c, EnergyType.Lightning)).ToList();
                    var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Electromagnetic Bomb: choose the Energy to move")[0];
                    if (ctx.Engine.MoveEnergy(from, me, card)) moved++;
                }
                ctx.DamageDefending(atk.Damage * moved);
                return true;
            }
        }

        /// <summary>
        /// Electro Shaker: when you play this Pokémon from your hand to evolve 1 of your Pokémon during your turn, you may move any
        /// number of Lightning Energy from your Pokémon to your other Pokémon in any way you like.
        /// </summary>
        private sealed class ElectrodeTEU : Mon
        {
            private static bool HasLightning(EffectContext ctx, PokemonInPlay m)
            {
                return m.Energy.Any(c => CardsDRI1.ProvidesType(ctx, m, c, EnergyType.Lightning));
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.MyPokemon().Count >= 2 && ctx.MyPokemon().Any(m => HasLightning(ctx, m));
                if (!TriggerOnce(ctx, "Electro Shaker", ready, "Electro Shaker: move Lightning Energy between your Pokémon?")) return;
                for (int guard = 0; guard < 60; guard++)
                {
                    var sources = ctx.MyPokemon(m => HasLightning(ctx, m));
                    if (sources.Count == 0) break;
                    if (guard > 0 && !ctx.AskYesNo("Electro Shaker: move another Lightning Energy?")) break;
                    var from = ctx.ChoosePokemon(sources, "Electro Shaker: move a Lightning Energy from which Pokémon?");
                    var energy = from.Energy.Where(c => CardsDRI1.ProvidesType(ctx, from, c, EnergyType.Lightning)).ToList();
                    var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Electro Shaker: choose the Energy to move")[0];
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Electro Shaker: move " + card.Def.Name + " to which Pokémon?");
                    ctx.Engine.MoveEnergy(from, to, card);
                }
            }
        }

        /// <summary>
        /// Thunderous Assault: 70 more damage if this Pokémon moved from your Bench to the Active Spot this turn. Its damage isn't
        /// affected by Weakness.
        /// </summary>
        private sealed class ZapdosTEU : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderous Assault") return false;
                var me = ctx.Pokemon;
                int damage = atk.Damage + (me.MovedToActiveOnTurn == ctx.Turn ? 70 : 0);
                ctx.DamageWith(ctx.Defending, damage, false, true, false);
                return true;
            }
        }

        /// <summary>
        /// Power Recharge: put all Electropower cards from your discard pile into your hand. Impact Bolt: discard all Lightning
        /// Energy from this Pokémon. Electrical-GX: search your deck for up to 7 Pokémon and put them into your hand.
        /// </summary>
        private sealed class AmpharosGX : Mon
        {
            private static readonly Mon Impact = new DiscardAllTypeMon("Impact Bolt", EnergyType.Lightning);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Impact.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Power Recharge")
                {
                    ctx.DamageDefending(atk.Damage);
                    var cards = ctx.Me.Discard.Where(c => c.Def.Name == Electropower).ToList();
                    if (cards.Count > 0) ctx.DiscardToHand(cards);
                    return true;
                }
                if (atk.Name != "Electrical-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon, 7, "Electrical-GX: search your deck for up to 7 Pokémon");
                return true;
            }
        }

        /// <summary>Nuzzly Gathering: once during your turn, search your deck for a Pokémon that has the Nuzzle attack. Nuzzle: flip, Paralyzed.</summary>
        private sealed class EmolgaTEU : Mon
        {
            private static readonly Mon Nuzzle = new ParalyzeFlipMon("Nuzzle");

            protected override string Activated { get { return "Nuzzly Gathering"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.Attacks.Any(a => a.Name == "Nuzzle"), 1, "Nuzzly Gathering: search your deck for a Pokémon with the Nuzzle attack");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Nuzzle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Unnerve: the effects of the opponent's Item and Supporter cards don't affect this Pokémon. Spider Thread: put a card from
        /// your discard pile into your hand.
        /// </summary>
        private sealed class GalvantulaTEU : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && trainer != null && (trainer.IsItem || trainer.IsSupporter);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Thread") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Spider Thread: put a card from your discard pile into your hand");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>
        /// Dance of the Ancients: once during your turn, if this Pokémon is on your Bench, choose 2 of your Benched Pokémon and
        /// attach a Lightning Energy card from your discard pile to each; if you do, discard all cards attached to this Pokémon and
        /// put it in the Lost Zone.
        /// </summary>
        private sealed class TapuKokoPrismStar : Mon
        {
            protected override string Activated { get { return "Dance of the Ancients"; } }

            private static List<CardInstance> LightningInDiscard(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Lightning)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !ctx.PokemonIsActive && ctx.Me.Bench.Count >= 2 && LightningInDiscard(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                // Any 2 Benched Pokémon, this one included (it's listed last: its Energy would be discarded with it).
                var bench = ctx.MyBench().Where(m => m != me).ToList();
                bench.Add(me);
                var targets = ctx.ChoosePokemonMany(bench, 2, 2, "Dance of the Ancients: choose 2 of your Benched Pokémon");
                int attached = 0;
                foreach (var target in targets)
                {
                    var energy = LightningInDiscard(ctx);
                    if (energy.Count == 0) break;
                    var card = energy[0];
                    ctx.Me.Discard.Remove(card);
                    if (ctx.Engine.AttachEnergyCard(card, target)) attached++;
                }
                if (attached == 0 || !ctx.InPlay(me)) return;
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.LostZone.AddRange(me.Stack);
                ctx.Log(me.Name + " was put in the Lost Zone and its attached cards were discarded (Dance of the Ancients).");
                ctx.Engine.DiscardFromPlay(me.Energy.ToList());
                ctx.Engine.DiscardFromPlay(me.Tools.ToList());
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Poltergeist: your opponent reveals their hand; 50 damage for each Trainer card there. Horror House-GX: your opponent can't
        /// play any cards from their hand during their next turn; with 1 extra Psychic Energy, each player draws until they have 7.
        /// </summary>
        private sealed class GengarMimikyuGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poltergeist")
                {
                    var hand = ctx.Opp.Hand.ToList();
                    if (hand.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Opponent, hand, "revealed their hand (Poltergeist)");
                        ctx.Engine.ShowCards(ctx.Player, hand, "Poltergeist", ctx.Engine.PlayerName(ctx.Opponent) + "'s hand", ctx.Source);
                    }
                    else ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " has no cards in their hand.");
                    ctx.DamageDefending(atk.Damage * hand.Count(c => c.Def.IsTrainer));
                    return true;
                }
                if (atk.Name != "Horror House-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Psychic);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new HandLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play any cards from their hand during their next turn (Horror House-GX)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                if (extra)
                {
                    ctx.DrawUntil(7);
                    var opp = ctx.Opp;
                    if (opp.Hand.Count < 7) ctx.Engine.DrawCards(ctx.Opponent, 7 - opp.Hand.Count);
                }
                return true;
            }
        }

        /// <summary>"Your opponent can't play any cards from their hand during their next turn" (Horror House-GX): Trainers, Energy and Pokémon.</summary>
        internal sealed class HandLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public HandLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected; }
            public override bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card) { return player == affected; }
            public override bool BlocksPlayingPokemon(GameEngine engine, int player, CardDef card) { return player == affected; }
        }

        /// <summary>Family Rescue: shuffle 5 Psychic Pokémon from your discard pile into your deck.</summary>
        private sealed class FamilyRescueMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Family Rescue") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var psychic = ctx.Me.Discard.Where(c => c.Def.IsPokemon && (c.Def.PokemonType == EnergyType.Psychic || c.Def.SecondType == EnergyType.Psychic)).ToList();
                if (psychic.Count == 0) return true;
                int n = Math.Min(5, psychic.Count);
                var picks = n == psychic.Count ? psychic : ctx.ChooseCards(psychic, n, n, "Family Rescue: shuffle 5 Psychic Pokémon from your discard pile into your deck");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Queen's Call: once during your turn, search your deck for a Pokémon that isn't a Pokémon-GX or Pokémon-EX. Power Lariat:
        /// 50 more damage for each Evolution Pokémon on your Bench.
        /// </summary>
        private sealed class NidoqueenTEU : Mon
        {
            private static readonly Mon Lariat = new CardsASR1.PlusMon("Power Lariat", 50, ctx => ctx.MyBench().Count(m => m.Def.IsEvolution));

            protected override string Activated { get { return "Queen's Call"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && !d.IsGXorEX, 1, "Queen's Call: search your deck for a Pokémon that isn't a Pokémon-GX or Pokémon-EX");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lariat.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Void Tentacles: Confused and Poisoned. Paranormal: during your opponent's next turn, prevent all damage done to this
        /// Pokémon by attacks from Ultra Beasts.
        /// </summary>
        private sealed class TentacruelTEU : Mon
        {
            private static readonly Mon Tentacles = new CardsCES1.ConditionsMon("Void Tentacles", SpecialCondition.Confused, SpecialCondition.Poisoned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Tentacles.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Paranormal") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new UltraBeastShieldEffect(me.Name + ": no damage from Ultra Beasts' attacks during the opponent's next turn (Paranormal)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Prevents all damage done to the Pokémon by attacks from Ultra Beasts.</summary>
        internal sealed class UltraBeastShieldEffect : ContinuousEffect
        {
            public UltraBeastShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Def.IsUltraBeast;
            }
        }

        /// <summary>
        /// Poison Sacs: the opponent's Poisoned Pokémon stay Poisoned when they evolve or devolve. Toxic Secretion: Poisoned, 2
        /// damage counters between turns.
        /// </summary>
        private sealed class MukTEU : Mon
        {
            private static readonly Mon Toxic = new CardsCEC1.ToxicCECMon("Toxic Secretion");

            public override bool KeepsPoisonOnEvolve(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Toxic.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Limbo Limbo: search your deck for up to 2 basic Energy cards and attach them to your Pokémon in any way you like. Alolan
        /// Club: 20 damage for each of your Pokémon in play with "Alolan" in its name.
        /// </summary>
        internal sealed class AlolanMarowakTEU : Mon
        {
            private static readonly Mon Club = new CardsASR1.TimesMon("Alolan Club", ctx => ctx.MyPokemon(m => m.Name.Contains("Alolan")).Count);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Club.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Limbo Limbo") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Limbo Limbo: search your deck for up to 2 basic Energy cards");
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (c, m) => true, "Limbo Limbo", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Strange Wave: search your deck for up to 3 Water and Psychic Energy cards in any combination and attach them to 1 of your Benched Pokémon.</summary>
        private sealed class StarmieTEU : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strange Wave") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsTEU1.BasicOf(d, EnergyType.Water) || CardsTEU1.BasicOf(d, EnergyType.Psychic), 3,
                    "Strange Wave: search your deck for up to 3 Water and Psychic Energy cards");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Strange Wave: attach them to which Benched Pokémon?");
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Scoop-Up Block: the opponent's Pokémon that have damage counters on them, and the cards attached to them, can't be put
        /// into the opponent's hand. Psy Bolt: flip, Paralyzed.
        /// </summary>
        private sealed class MrMimeTEU : Mon
        {
            private static readonly Mon Bolt = new ParalyzeFlipMon("Psy Bolt");

            public override bool KeepsOutOfHand(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && mon.DamageCounters > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bolt.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Magic Odds: prevent all damage done to this Pokémon by the opponent's attacks if it's 10, 30, 50 ... 250. Breakdown: 1
        /// damage counter on the opponent's Active Pokémon for each card in their hand. Life Trick-GX: heal all damage from it.
        /// </summary>
        private sealed class MrMimeGX : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || !info.AgainstOpponent) return false;
                return damage > 0 && damage <= 250 && damage % 20 == 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Breakdown")
                {
                    var defending = ctx.Defending;
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    int counters = ctx.Opp.Hand.Count;
                    if (counters > 0 && defending != null && ctx.InPlay(defending)) ctx.PlaceCounters(defending, counters);
                    return true;
                }
                if (atk.Name != "Life Trick-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsTEU1.HealAll(ctx, ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Mysterious Dance: 1 damage counter on the opponent's Pokémon for each of their Benched Pokémon, placed in any way you like.</summary>
        private sealed class MysteriousDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mysterious Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int counters = ctx.Opp.Bench.Count;
                if (counters > 0) ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                return true;
            }
        }

    }
}
