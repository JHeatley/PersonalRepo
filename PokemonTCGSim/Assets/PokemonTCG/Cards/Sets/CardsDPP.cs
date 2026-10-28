using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// DP Black Star Promos (DPP DP01-DP56, 2007-2009; v136). Every card came out before Black &amp; White: Unlimited only.
    /// Prints: Tropical Wind 25 and 48 (of 5), Dialga LV.X 37 (of 17), Palkia LV.X 18 (30C CC12) and Garchomp C LV.X 46 (CEL CC18).
    /// Poké-Powers can't be used while the Pokémon has a Special Condition, except where the card doesn't say so (Baby
    /// Evolution, Stance). Your rulings: Porygon-Z's Learning levels up any of your Pokémon with no timing limits; Probopass G's
    /// Nose Unit counts Poké-Bodies printed on the Pokémon or the card under a LV.X; Arceus may be played in any number, in
    /// every format; a Pokémon with a LV.X on top still counts as the Pokémon under it for name checks in play; Mime Jr.'s Baby
    /// Evolution works on any turn. Hydro Impact is one hit to the chosen Pokémon (your Telekinesis ruling).
    /// New engine rules: Endless Darkness's Knock Out (PokemonInPlay.SleepKnocksOut), Time Skip's turn end
    /// (ContinuousEffect.EndsTurnAfterDraw), Heat Metal's Burned kept on evolving (CardScript.KeepsBurnOnEvolve), Heat Wave's
    /// record of Energy discarded by your own attack (PlayerState.OwnAttackDiscardedEnergy), the Enigma Berry held item
    /// (CardScript.ReactsToOwnDamageWithoutAbility) and Omniscient's attacks of other Arceus LV.X (ExtraAttacksMayShareName).
    /// Pokémon whose attacks only do their printed damage: Turtwig 1.
    /// </summary>
    public static class CardsDPP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DPP-1", new PlainMon());
            r.RegisterByKey("DPP-2", new FlipDiscardTypeMon("Ember", EnergyType.Fire));
            r.RegisterByKey("DPP-3", new FlipBonusMon("Water Splash", 10));
            r.RegisterByKey("DPP-4", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 1), new CardsFST1.FlipRecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("DPP-5", new TropicalWind());
            r.RegisterByKey("DPP-6", new Cards30CC.BenchHitMon("Jump Kick", 10));
            r.RegisterByKey("DPP-7", new Cards30CC.BenchHitMon("Steamroll", 20));
            r.RegisterByKey("DPP-8", new ComboMon(new CardsASC.SelfGuardMon("Hard Face", 20), new FlipBonusMon("Shield Attack", 20)));
            r.RegisterByKey("DPP-9", new TorterraLvX());
            r.RegisterByKey("DPP-10", new InfernapeLvX());
            r.RegisterByKey("DPP-11", new EmpoleonLvX());
            r.RegisterByKey("DPP-12", new LucarioLvX());
            r.RegisterByKey("DPP-13", new CardsFST2.ConditionBonusMon("Splash About", 10,
                c => c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending)));
            r.RegisterByKey("DPP-14", new ComboMon(new DrawAttackMon("Me First", 1), new CardsPAR2.FlipConditionMon("Tone-Deaf", SpecialCondition.Confused)));
            r.RegisterByKey("DPP-15", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("DPP-16", new PikachuDPP());
            r.RegisterByKey("DPP-17", new DialgaLvX());
            r.RegisterByKey("DPP-19", new DarkraiLvX());
            r.RegisterByKey("DPP-20", new ComboMon(new CardsMEG.SandAttackMon("Smoke Bomb"), new FlameDrumMon()));
            r.RegisterByKey("DPP-21", new RaichuDPP());
            r.RegisterByKey("DPP-22", new MimeJrDPP());
            r.RegisterByKey("DPP-23", new ComboMon(new CardsMEG.WeakenDefendingMon("Charm", 20), new TailsNothingMon("Pose")));
            r.RegisterByKey("DPP-24", new DarkraiEnigmaBerry());
            r.RegisterByKey("DPP-26", new ComboMon(new DrawAttackMon("Time Bellow", 1), new FlashCannonMon()));
            r.RegisterByKey("DPP-27", new ComboMon(new SpacialRendMon(), new TransbackMon()));
            r.RegisterByKey("DPP-28", new MewtwoLvX());
            r.RegisterByKey("DPP-29", new ComboMon(new HardCrushMon(), new UpthrowMon()));
            r.RegisterByKey("DPP-30", new RegigigasLvX());
            r.RegisterByKey("DPP-31", new HeatranLvX());
            r.RegisterByKey("DPP-32", new ComboMon(new CardsMEG.SandAttackMon("Mirror Shot"), new CardsSUM1.DiscardTypeFromSelfMon("Magnet Slash", EnergyType.Lightning, 99)));
            r.RegisterByKey("DPP-33", new DusknoirDPP());
            r.RegisterByKey("DPP-34", new ComboMon(new WindWaveMon(), new ExplosiveSmokeMon()));
            r.RegisterByKey("DPP-35", new PorygonZDPP());
            r.RegisterByKey("DPP-36", new GliscorDPP());
            r.RegisterByKey("DPP-38", new GiratinaLvX());
            r.RegisterByKey("DPP-39", new ShayminLvX());
            r.RegisterByKey("DPP-40", new ComboMon(new MayDragOffMon(), new LockAfterAttackMon("Giga Hammer", true)));
            r.RegisterByKey("DPP-41", new ToxicroakG());
            r.RegisterByKey("DPP-42", new ComboMon(new PowerWhipMon(), new GrassKnotMon()));
            r.RegisterByKey("DPP-43", new ComboMon(new NoseUnitMon(), new LoudSnortMon()));
            r.RegisterByKey("DPP-44", new ComboMon(new ChargeBeamMon(), new MetallicBoltMon()));
            r.RegisterByKey("DPP-45", new CharizardGLvX());
            r.RegisterByKey("DPP-47", new RayquazaCLvX());
            r.RegisterByKey("DPP-49", new ComboMon(new TimeCallMon(), new TimeWagerMon()));
            r.RegisterByKey("DPP-50", new JudgmentMon());
            r.RegisterByKey("DPP-51", new CresseliaDPP());
            r.RegisterByKey("DPP-52", new DarkraiDPP());
            r.RegisterByKey("DPP-53", new ArceusLvX(false));
            r.RegisterByKey("DPP-54", new BeginningDoor());
            r.RegisterByKey("DPP-55", new UltimateZone());
            r.RegisterByKey("DPP-56", new ArceusLvX(true));
        }

        // ================================================================== Shared

        /// <summary>Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        private static bool NoCondition(PokemonInPlay mon)
        {
            return mon != null && mon.Conditions == SpecialCondition.None;
        }

        /// <summary>The ability at this index is the named one (on the card it's printed on).</summary>
        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        /// <summary>
        /// The Pokémon counts as this name in play: its own name, or (your ruling) the name of the Pokémon under its LV.X
        /// (Arceus LV.X is Arceus, Darkrai LV.X is Darkrai).
        /// </summary>
        internal static bool Named(PokemonInPlay mon, string name)
        {
            if (mon == null) return false;
            if (mon.Name == name) return true;
            return mon.Def.IsLevelUp && mon.Stack.Count >= 2 && mon.Stack[mon.Stack.Count - 2].Def.Name == name;
        }

        /// <summary>"[Name] is anywhere under this Pokémon" (Pichu under Pikachu, Magby under Magmortar).</summary>
        private static bool Under(PokemonInPlay mon, string name)
        {
            if (mon == null) return false;
            for (int i = 0; i < mon.Stack.Count - 1; i++)
                if (mon.Stack[i].Def.Name == name) return true;
            return false;
        }

        private static bool BasicOf(CardDef d, EnergyType type)
        {
            return d.IsBasicEnergy && d.EnergyProvides == type;
        }

        /// <summary>Moves cards from the discard pile to the deck and shuffles it.</summary>
        private static void DiscardToDeck(EffectContext ctx, IEnumerable<CardInstance> cards)
        {
            foreach (var card in cards.ToList())
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Add(card);
            ctx.ShuffleMyDeck();
        }

        /// <summary>Attaches a card from your discard pile to a Pokémon.</summary>
        private static bool AttachFromDiscard(EffectContext ctx, CardInstance card, PokemonInPlay target)
        {
            if (card == null || target == null || !ctx.InPlay(target) || !ctx.Me.Discard.Remove(card)) return false;
            return ctx.Engine.AttachEnergyCard(card, target);
        }

        /// <summary>Damage that comes back off at the end of the turn (Enigma Berry).</summary>
        private sealed class HealAtEndOfTurnEffect : ContinuousEffect
        {
            private readonly int amount;

            public HealAtEndOfTurnEffect(string name, int owner, int targetUid, int turn, int amount) : base(name, owner, targetUid, turn, false)
            {
                this.amount = amount;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon != null && mon.DamageCounters > 0) engine.Heal(mon, amount);
            }
        }

        // ================================================================== Attacks used by several cards

        /// <summary>Ember: flip a coin; tails, discard an Energy of that type from this Pokémon.</summary>
        private sealed class FlipDiscardTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public FlipDiscardTypeMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() || !ctx.InPlay(me)) return true;
                var typed = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, type)).ToList();
                if (typed.Count == 0) return true;
                var card = ctx.ChooseCards(typed, 1, 1, attack + ": discard which " + type + " Energy?")[0];
                ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Spacial Rend: search your deck for a Stadium card and put it into your hand; then discard any Stadium in play.</summary>
        private sealed class SpacialRendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spacial Rend") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsStadiumCard, 1, "Spacial Rend: search your deck for a Stadium card");
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Tropical Wind: heads, remove 2 damage counters from each Active Pokémon; tails, each Active Pokémon is now Asleep.</summary>
        private sealed class TropicalWind : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool heads = ctx.Flip();
                for (int i = 0; i < 2; i++)
                {
                    var active = ctx.State.Players[i].Active;
                    if (active == null) continue;
                    if (heads) ctx.Heal(active, 20);
                    else ctx.ApplyCondition(active, SpecialCondition.Asleep);
                }
            }
        }

        /// <summary>Beginning Door: search your deck for a card named Arceus and put it into your hand.</summary>
        private sealed class BeginningDoor : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon && d.Name == "Arceus", 1, "Beginning Door: search your deck for Arceus");
            }
        }

        /// <summary>
        /// Ultimate Zone (Stadium): during each player's turn, that player may move an Energy card from 1 of their Benched Pokémon to
        /// their Active Arceus, as often as they like (the person moves several and presses Done).
        /// </summary>
        private sealed class UltimateZone : CardScript
        {
            private static bool Ready(EffectContext ctx)
            {
                return Named(ctx.Me.Active, "Arceus") && ctx.Me.Bench.Any(b => b.Energy.Count > 0);
            }

            public override bool CanUseStadium(EffectContext ctx) { return ctx.IsMyTurn && Ready(ctx); }

            public override bool UseStadium(EffectContext ctx)
            {
                RepeatableAbility.Repeat(ctx, () => Ready(ctx), offerDone =>
                {
                    var holders = ctx.Me.Bench.Where(b => b.Energy.Count > 0).ToList();
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, "Ultimate Zone: move an Energy card from which Benched Pokémon to your Active Arceus?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var card = ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Ultimate Zone: move which Energy card?")[0];
                    return ctx.Engine.MoveEnergy(from, ctx.Me.Active, card);
                });
                return false;
            }
        }

        // ================================================================== Pokémon LV.X

        /// <summary>
        /// Forest Murmurs (Poké-Power): with more Prize cards left than the opponent, switch 1 of their Benched Pokémon with the
        /// Defending Pokémon. Vigorous Dash: 30 to 1 of the opponent's Benched Pokémon and 30 to itself.
        /// </summary>
        private sealed class TorterraLvX : Mon
        {
            protected override string Activated { get { return "Forest Murmurs"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count && ctx.GustTargets().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.GustOpponent("Forest Murmurs: switch which of your opponent's Benched Pokémon with the Defending Pokémon?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vigorous Dash") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(30);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>
        /// Burning Head (Poké-Power): look at the top 3 cards of your deck, put 1 into your hand and discard the other 2. Flare Up:
        /// only with 8 Fire Energy cards in your discard pile; they're shuffled into your deck and it does 150.
        /// </summary>
        private sealed class InfernapeLvX : Mon
        {
            protected override string Activated { get { return "Burning Head"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return;
                var keep = ctx.ChooseCards(top, 1, 1, "Burning Head: put which card into your hand? The others are discarded.")[0];
                ctx.DeckToHand(new[] { keep });
                foreach (var card in top)
                {
                    if (card == keep || !ctx.Me.Deck.Remove(card)) continue;
                    ctx.Me.Discard.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card into their hand and discarded " + (top.Count - 1) + " (Burning Head).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Up") return false;
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count < 8) return Nothing(ctx);
                DiscardToDeck(ctx, fire.Take(8));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Supreme Command (Poké-Power): put up to 2 random cards from the opponent's hand face down next to the Defending Pokémon;
        /// they go back to their hand at the end of their next turn. Hydro Impact: 80 to 1 of the opponent's Pokémon; it can't attack
        /// during your next turn.
        /// </summary>
        private sealed class EmpoleonLvX : Mon
        {
            protected override string Activated { get { return "Supreme Command"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int max = Math.Min(2, ctx.Opp.Hand.Count);
                int n = max == 1 ? 1 : ctx.AskNumber(1, max, "Supreme Command: how many cards from your opponent's hand?");
                var taken = new List<int>();
                for (int i = 0; i < n && ctx.Opp.Hand.Count > 0; i++)
                {
                    var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                    ctx.Opp.Hand.Remove(card);
                    ctx.Opp.SetAsideHand.Add(card);
                    taken.Add(card.Id);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + taken.Count + " card" + (taken.Count == 1 ? "" : "s") + " from " + ctx.Engine.Their(ctx.Opponent) + " hand face down until the end of " + ctx.Engine.Their(ctx.Opponent) + " next turn (Supreme Command).");
                ctx.AddEffect(new SetAsideReturnEffect("Supreme Command: the cards come back at the end of the turn", ctx.Player, ctx.Opponent, ctx.Turn + 1, taken));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Impact") return false;
                ctx.DamageAnyOpponent(80);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Puts the set-aside cards (by id) back into their owner's hand at the end of the turn it expires.</summary>
        private sealed class SetAsideReturnEffect : ContinuousEffect
        {
            private readonly int victim;
            private readonly List<int> ids;

            public SetAsideReturnEffect(string name, int owner, int victim, int expiresAfterTurn, List<int> ids) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.victim = victim;
                this.ids = new List<int>(ids);
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var p = engine.State.Players[victim];
                var back = p.SetAsideHand.Where(c => ids.Contains(c.Id)).ToList();
                if (back.Count == 0) return;
                foreach (var card in back)
                {
                    p.SetAsideHand.Remove(card);
                    p.Hand.Add(card);
                }
                engine.Emit(GameEventType.Info, victim, engine.PlayerName(victim) + " put the " + back.Count + " card" + (back.Count == 1 ? "" : "s") + " set aside by Supreme Command back into " + engine.Their(victim) + " hand.");
            }
        }

        /// <summary>
        /// Stance (Poké-Power): when you level up your Active Lucario from your hand, you may prevent all effects of attacks on it
        /// during the opponent's next turn (ends if it stops being Active). Close Combat: during the opponent's next turn, damage done
        /// to it by attacks is 30 more (after Weakness and Resistance).
        /// </summary>
        private sealed class LucarioLvX : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Stance", ctx.IsActive(ctx.Pokemon), "Use Stance? Prevent all effects of attacks, damage included, done to " + ctx.Pokemon.Name + " during your opponent's next turn.")) return;
                ctx.ProtectSelfNextTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Close Combat") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new MoreDamageTakenEffect(me.Name + " takes 30 more damage from attacks during the opponent's next turn (Close Combat)", ctx.Player, me.Uid, ctx.Turn + 1, 30));
                return true;
            }
        }

        /// <summary>"Any damage done to this Pokémon by attacks is increased by N (after Weakness and Resistance)" while it lasts.</summary>
        private sealed class MoreDamageTakenEffect : ContinuousEffect
        {
            private readonly int amount;

            public MoreDamageTakenEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && damage > 0 ? damage + amount : damage;
            }
        }

        /// <summary>
        /// Time Skip (Poké-Power): the opponent flips 2 coins: both heads, your turn ends; both tails, their next turn ends right
        /// after they draw for it. Metal Flash: can't be used during your next turn.
        /// </summary>
        private sealed class DialgaLvX : Mon
        {
            private static readonly Mon MetalFlash = new LockAfterAttackMon("Metal Flash", true);

            protected override string Activated { get { return "Time Skip"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                int heads = ctx.Engine.FlipCoins(ctx.Opponent, 2, "Time Skip");
                if (heads == 2)
                {
                    ctx.Log("Time Skip: both heads, " + ctx.Engine.Possessive(ctx.Player) + " turn ends.");
                    ctx.Engine.RequestEndTurn();
                }
                else if (heads == 0)
                {
                    ctx.Log("Time Skip: both tails, " + ctx.Engine.Possessive(ctx.Opponent) + " next turn ends after they draw.");
                    ctx.AddEffect(new TimeSkipEffect("Time Skip", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MetalFlash.HandleAttack(ctx, atk); }
        }

        private sealed class TimeSkipEffect : ContinuousEffect
        {
            private readonly int victim;

            public TimeSkipEffect(string name, int owner, int victim, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.victim = victim;
            }

            public override bool EndsTurnAfterDraw(GameEngine engine, int player) { return player == victim; }
        }

        /// <summary>
        /// Dark Shadow (Poké-Body): each basic Darkness Energy card on the attacking Darkness Pokémon of yours adds 10 to its damage to
        /// the Active Pokémon (before Weakness and Resistance); only 1 Dark Shadow counts. Endless Darkness: the Defending Pokémon is
        /// Asleep; 2 coins between turns, any tails it stays Asleep, both tails it's Knocked Out.
        /// </summary>
        private sealed class DarkraiLvX : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!src.Engine.IsType(attacker, EnergyType.Darkness)) return damage;
                int dark = attacker.Energy.Count(en => BasicOf(en.Def, EnergyType.Darkness));
                if (dark == 0 || !info.ApplyOnce("Dark Shadow")) return damage;
                return damage + 10 * dark;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Endless Darkness") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Asleep))
                {
                    defending.SleepCoins = 2;
                    defending.SleepKnocksOut = true;
                }
                return true;
            }
        }

        /// <summary>Psybarrier (Poké-Body): prevent all effects of attacks, damage included, done to it by the opponent's unevolved Pokémon. Giga Burn: discard all its Energy.</summary>
        private sealed class MewtwoLvX : Mon
        {
            private static readonly Mon GigaBurn = new CardsTWM1.SelfDiscardMon("Giga Burn", 99);

            private static bool Unevolved(PokemonInPlay attacker) { return attacker != null && attacker.Stack.Count <= 1; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !Unevolved(a)) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && Unevolved(attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GigaBurn.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sacrifice (Poké-Power): 1 of your Pokémon is Knocked Out; then attach up to 2 basic Energy cards from your discard pile to
        /// this Pokémon and remove 8 damage counters from it. Giga Blaster: discard the top card of the opponent's deck and a random
        /// card from their hand; it can't be used during your next turn.
        /// </summary>
        private sealed class RegigigasLvX : Mon
        {
            protected override string Activated { get { return "Sacrifice"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var victim = ctx.ChoosePokemon(ctx.MyPokemon(), "Sacrifice: which of your Pokémon is Knocked Out?");
                if (victim == null) return;
                ctx.Engine.KnockOut(victim, "Sacrifice");
                ctx.Engine.ResolveKnockouts();
                if (ctx.State.IsOver || !ctx.InPlay(me)) return;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count > 0)
                {
                    var picks = basics.Count <= 2 && basics.Select(b => b.Def.Name).Distinct().Count() == 1 ? basics
                        : ctx.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Sacrifice: attach up to 2 basic Energy cards to " + me.Name);
                    foreach (var card in picks) AttachFromDiscard(ctx, card, me);
                }
                if (ctx.InPlay(me)) ctx.Heal(me, 80);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giga Blaster") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                TefPokemon.DiscardRandomFromOpponentHand(ctx);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Heat Metal (Poké-Body): the opponent's Burned Pokémon stay Burned when they evolve or devolve, and their Burned coin
        /// flips between turns count as tails. Heat Wave (Poké-Power): at the end of your turn, from the Bench, attach up to 2 of the
        /// basic Energy cards your Fire or Metal Active Pokémon discarded with its own attack this turn back to it.
        /// </summary>
        private sealed class HeatranLvX : Mon
        {
            public override bool KeepsBurnOnEvolve(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Owner != src.Player; }

            public override bool KeepsOpponentBurned(EffectContext src, int player) { return player != src.Player; }

            public override void OnEndOfTurn(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsMyTurn || !src.Me.Bench.Contains(me) || !NoCondition(me)) return;
                if (src.AbilityUsedThisTurn("Heat Wave")) return;
                var p = src.Me;
                if (p.OwnAttackDiscardedTurn != src.Turn || p.OwnAttackDiscardedEnergy.Count == 0) return;
                var target = src.State.FindPokemon(p.OwnAttackDiscardedFromUid);
                if (target == null || target.Owner != src.Player) return;
                if (!src.Engine.IsType(target, EnergyType.Fire) && !src.Engine.IsType(target, EnergyType.Metal)) return;
                var cards = p.OwnAttackDiscardedEnergy.Where(c => p.Discard.Contains(c)).ToList();
                if (cards.Count == 0) return;
                if (!src.AskYesNo("Use Heat Wave? Attach up to 2 of the discarded Energy cards back to " + target.Name + ".")) return;
                src.MarkAbilityUsed("Heat Wave");
                Announce(src, "Heat Wave");
                var picks = cards.Count <= 2 ? cards : src.ChooseCards(cards, 1, 2, "Heat Wave: attach which Energy cards to " + target.Name + "?");
                foreach (var card in picks) AttachFromDiscard(src, card, target);
            }
        }

        /// <summary>
        /// Invisible Tentacles (Poké-Body): whenever the opponent's Pokémon tries to attack, they discard a card from their hand; with
        /// no card to discard, their Pokémon can't attack. Only 1 Invisible Tentacles each turn. Darkness Lost: 30 to each of the
        /// opponent's Pokémon; any Knocked Out by it go to the Lost Zone.
        /// </summary>
        private sealed class GiratinaLvX : Mon
        {
            private const string Tentacles = "Invisible Tentacles";

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner != src.Player && src.Opp.Hand.Count == 0;
            }

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (attacker == null || attacker.Owner == src.Player || src.Opp.Hand.Count == 0) return;
                // "You can't use more than 1 Invisible Tentacles Poké-Body each turn": one discard, however many Giratina LV.X.
                var used = src.Me.NamedLimitUsedOnTurn;
                int turn;
                if (used.TryGetValue(Tentacles, out turn) && turn == src.Turn) return;
                used[Tentacles] = src.Turn;
                var card = src.Engine.ChooseCards(src.Opponent, src.Opp.Hand.ToList(), 1, 1, Tentacles + ": discard a card from your hand to attack", src.Source)[0];
                src.Engine.DiscardFromHand(src.Opponent, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Lost") return false;
                foreach (var mon in ctx.OpponentPokemon())
                {
                    mon.LostZoneKnockOutTurn = ctx.Turn;
                    ctx.DamageTarget(mon, 30);
                }
                return true;
            }
        }

        /// <summary>
        /// Revenge Seed (Poké-Body): if any of your Grass Pokémon were Knocked Out by damage from an opponent's attack during their
        /// last turn, its attacks do 60 more to the Active Pokémon. Energy Flare: move any number of Energy among your Pokémon.
        /// </summary>
        private sealed class ShayminLvX : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || !info.ToOpponentsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                int last = src.OpponentsLastTurn;
                bool revenge = src.Me.KnockOuts.Any(k => k.ByAttack && k.Turn == last && k.Def != null && (k.Def.PokemonType == EnergyType.Grass || k.Def.SecondType == EnergyType.Grass));
                return revenge ? damage + 60 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Flare") return false;
                ctx.DamageDefending(atk.Damage);
                // "You may move any number": the person moves as many as they like and presses Done; bots leave them.
                if (!RepeatableAbility.ForPerson(ctx)) return true;
                RepeatableAbility.Repeat(ctx, () => ctx.MyPokemon().Count > 1 && ctx.MyPokemon().Any(m => m.Energy.Count > 0), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m.Energy.Count > 0), "Energy Flare: move an Energy card from which Pokémon? Press Done when finished.", true);
                    if (from == null) return false;
                    var card = ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Energy Flare: move which Energy card?")[0];
                    var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), "Energy Flare: move it to which Pokémon?", true);
                    return to != null && ctx.Engine.MoveEnergy(from, to, card);
                });
                return true;
            }
        }

        /// <summary>Call for Power (Poké-Power): as often as you like, move an Energy from 1 of your other Pokémon to this one. Malevolent Fire: tails, discard all its Energy.</summary>
        private sealed class CharizardGLvX : Mon
        {
            private const string CallForPower = "Call for Power";

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.Energy.Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, CallForPower) && ctx.IsMyTurn && NoCondition(ctx.Pokemon) && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), CallForPower + ": move an Energy from which Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var card = ctx.ChooseCards(from.Energy.ToList(), 1, 1, CallForPower + ": move which Energy?")[0];
                    return ctx.Engine.MoveEnergy(from, me, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Malevolent Fire") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>
        /// Dragon Spirit (Poké-Body): when this Active Pokémon is damaged but not Knocked Out by an opponent's attack, you may attach
        /// an Energy card from your discard pile to it. Final Blowup: discard all its Energy, unless your hand is empty.
        /// </summary>
        private sealed class RayquazaCLvX : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || !holderWasActive || attacker == null || attacker.Owner == ctx.Player || damage <= 0) return;
                if (!ctx.InPlay(me) || ctx.RemainingHp(me) <= 0 || !ctx.IsActive(me)) return;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0 || !ctx.AskYesNo("Dragon Spirit: attach an Energy card from your discard pile to " + me.Name + "?")) return;
                Announce(ctx, "Dragon Spirit");
                var card = ctx.ChooseCards(energy, 1, 1, "Dragon Spirit: attach which Energy card?")[0];
                AttachFromDiscard(ctx, card, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Final Blowup") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>
        /// Multitype (Poké-Body): this Arceus LV.X is the type of the Arceus under it. Omniscient (Poké-Body, DP53): it can use the
        /// attacks of every Arceus you have in play. Meteor Blast (DP56): tails, 50 instead of 100.
        /// </summary>
        private sealed class ArceusLvX : Mon
        {
            private readonly bool meteor;

            public ArceusLvX(bool meteor) { this.meteor = meteor; }

            public override bool ExtraAttacksMayShareName { get { return !meteor; } }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon != src.Pokemon || mon.Stack.Count < 2) return;
                var below = mon.Stack[mon.Stack.Count - 2].Def;
                if (!below.IsPokemon) return;
                types.Clear();
                types.Add(below.PokemonType);
                if (below.SecondType.HasValue && below.SecondType.Value != below.PokemonType) types.Add(below.SecondType.Value);
            }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (meteor) return list;
                var seen = new HashSet<string>();
                foreach (var other in src.Me.InPlay())
                {
                    if (other == src.Pokemon || !Named(other, "Arceus")) continue;
                    var defs = new List<CardDef> { other.Def };
                    if (other.Def.IsLevelUp && other.Stack.Count >= 2) defs.Add(other.Stack[other.Stack.Count - 2].Def);
                    foreach (var def in defs)
                    {
                        if (!seen.Add(def.BaseKey)) continue;
                        for (int i = 0; i < def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(def, i));
                    }
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Meteor Blast") return false;
                ctx.DamageDefending(ctx.Flip() ? atk.Damage : 50);
                return true;
            }
        }

        // ================================================================== Other Pokémon

        /// <summary>Electro Recycle (Poké-Power): with Pichu under it, put a Lightning Energy card from your discard pile into your hand. BikaBika: heads, 10 more.</summary>
        private sealed class PikachuDPP : Mon
        {
            private static readonly Mon BikaBika = new FlipBonusMon("BikaBika", 10);

            private static List<CardInstance> Lightning(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning); }

            protected override string Activated { get { return "Electro Recycle"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && Under(ctx.Pokemon, "Pichu") && Lightning(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Lightning(ctx);
                if (cards.Count > 0) ctx.DiscardToHand(new List<CardInstance> { cards[0] });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BikaBika.HandleAttack(ctx, atk); }
        }

        /// <summary>Flame Drum: unless Magby is under this Pokémon, discard 2 Energy cards from your hand, or it does nothing.</summary>
        private sealed class FlameDrumMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Drum") return false;
                if (!Under(ctx.Pokemon, "Magby"))
                {
                    var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                    if (energy.Count < 2) return Nothing(ctx);
                    var picks = ctx.ChooseCards(energy, 2, 2, "Flame Drum: discard 2 Energy cards from your hand");
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Electromagnetic Induction: attach up to 2 Lightning Energy cards from your deck to 1 of your Pokémon. Explosive Thunder:
        /// discard all basic Lightning Energy cards from it; 30 for each.
        /// </summary>
        private sealed class RaichuDPP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Electromagnetic Induction")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Electromagnetic Induction: attach Lightning Energy to which of your Pokémon?");
                    var found = ctx.SearchMyDeck(d => BasicOf(d, EnergyType.Lightning), 2, "Electromagnetic Induction: choose up to 2 Lightning Energy cards");
                    foreach (var card in found)
                    {
                        if (target != null && ctx.InPlay(target)) ctx.Engine.AttachEnergyCard(card, target);
                        else ctx.Me.Deck.Add(card);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Explosive Thunder")
                {
                    var me = ctx.Pokemon;
                    int discarded = 0;
                    foreach (var en in me.Energy.Where(e => BasicOf(e.Def, EnergyType.Lightning)).ToList())
                        if (ctx.Engine.DiscardEnergy(me, en)) discarded++;
                    ctx.DamageDefending(atk.Damage * discarded);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Baby Evolution (Poké-Power): put Mr. Mime from your hand onto this Pokémon (evolving it; any turn, by your ruling) and
        /// remove all its damage counters. Mime: shuffle your hand into your deck and draw as many cards as the opponent has.
        /// </summary>
        private sealed class MimeJrDPP : Mon
        {
            private static List<CardInstance> MrMimes(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Name == "Mr. Mime").ToList();
            }

            protected override string Activated { get { return "Baby Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx) { return MrMimes(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var cards = MrMimes(ctx);
                if (cards.Count == 0) return;
                var card = cards.Select(c => c.Def.Key).Distinct().Count() == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Baby Evolution: put which Mr. Mime onto " + me.Name + "?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(me, card, true);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mime") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>
        /// Darkrai (DP24): Enigma Berry (a held item, not an Ability): damaged by an attack from the opponent's Fighting Pokémon, it
        /// loses 4 damage counters at the end of that turn. Spacial Rend; Roar of Time: 3 Pokémon from your discard pile on top of your
        /// deck, then shuffle; without 3, nothing.
        /// </summary>
        private sealed class DarkraiEnigmaBerry : Mon
        {
            private static readonly Mon SpacialRend = new SpacialRendMon();

            public override bool ReactsToOwnDamageWithoutAbility { get { return true; } }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || attacker == null || attacker.Owner == ctx.Player || damage <= 0) return;
                if (!ctx.Engine.IsType(attacker, EnergyType.Fighting)) return;
                ctx.AddEffect(new HealAtEndOfTurnEffect("Enigma Berry: " + me.Name + " loses 4 damage counters at the end of the turn", ctx.Player, me.Uid, ctx.Turn, 40));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roar of Time") return SpacialRend.HandleAttack(ctx, atk);
                var pokemon = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count < 3) return Nothing(ctx);
                var picks = pokemon.Count == 3 ? pokemon : ctx.ChooseCards(pokemon, 3, 3, "Roar of Time: put which 3 Pokémon on top of your deck?");
                DiscardToDeck(ctx, picks);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Flash Cannon: you may return all Energy cards from this Pokémon to your hand; if you do, the highest Stage Evolution card
        /// of the Defending Pokémon is shuffled into the opponent's deck.
        /// </summary>
        private sealed class FlashCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Cannon") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Flash Cannon: return all Energy cards attached to " + me.Name + " to your hand?")) return true;
                int returned = 0;
                foreach (var en in me.Energy.ToList())
                    if (ctx.Engine.ReturnEnergyToHand(me, en)) returned++;
                if (returned == 0) return true;
                if (TefPokemon.StillDefending(ctx, defending) && defending.Stack.Count > 1 && !defending.Def.IsMultiCard)
                    ctx.Engine.Devolve(defending, 1, true);
                return true;
            }
        }

        /// <summary>Transback: you may flip a coin; heads, discard all Energy from this Pokémon and shuffle the Defending Pokémon and its cards into the opponent's deck.</summary>
        private sealed class TransbackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Transback") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.AskYesNo("Transback: flip a coin?")) return true;
                if (!ctx.Flip()) return true;
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        /// <summary>Hard Crush: discard the top 5 cards of your deck; 50 for each Energy card discarded.</summary>
        private sealed class HardCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Crush") return false;
                var top = ctx.Me.Deck.Count > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, 5) : new List<CardInstance>();
                ctx.DamageDefending(atk.Damage * top.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Upthrow: shuffle all Fighting Energy cards from your discard pile into your deck.</summary>
        private sealed class UpthrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upthrow") return false;
                ctx.DamageDefending(atk.Damage);
                var fighting = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (fighting.Count > 0) DiscardToDeck(ctx, fighting);
                return true;
            }
        }

        /// <summary>
        /// Dark Hide (Poké-Power): flip a coin; heads, look at the opponent's hand and put a Pokémon from it on the bottom of their
        /// deck. Reaper Pulse: move up to 2 damage counters from this Pokémon to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class DusknoirDPP : Mon
        {
            protected override string Activated { get { return "Dark Hide"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", "Dark Hide: your opponent's hand", ctx.Source);
                var pokemon = ctx.Opp.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return;
                var card = ctx.ChooseCards(pokemon, 1, 1, "Dark Hide: put which Pokémon on the bottom of your opponent's deck?")[0];
                if (!ctx.Opp.Hand.Remove(card)) return;
                ctx.Opp.Deck.Add(card);
                ctx.Log(card.Def.Name + " was put on the bottom of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck (Dark Hide).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reaper Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.DamageCounters == 0) return true;
                var targets = ctx.OpponentBench().Where(ctx.CanAffect).ToList();
                if (targets.Count == 0) return true;
                int max = Math.Min(2, me.DamageCounters);
                int n = ctx.AskNumber(0, max, "Reaper Pulse: move how many damage counters from " + me.Name + "?");
                if (n <= 0) return true;
                var target = ctx.ChoosePokemon(targets, "Reaper Pulse: move them to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveDamageCounters(me, target, n, "Reaper Pulse");
                return true;
            }
        }

        /// <summary>Wind Wave: shuffle up to 5 Pokémon and Supporter cards from your discard pile into your deck.</summary>
        private sealed class WindWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wind Wave") return false;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsSupporter).ToList();
                if (cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, 0, Math.Min(5, cards.Count), "Wind Wave: shuffle up to 5 Pokémon and Supporter cards into your deck");
                if (picks.Count > 0) DiscardToDeck(ctx, picks);
                return true;
            }
        }

        /// <summary>Explosive Smoke: 10 to each Benched Pokémon, both players'.</summary>
        private sealed class ExplosiveSmokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Smoke") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 10);
                foreach (var benched in ctx.MyBench()) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>
        /// Install (Poké-Power): as often as you like, move a Technical Machine from 1 of your Pokémon to another. Learning: put a
        /// Pokémon LV.X from your deck onto the Pokémon of yours it levels up from (any of them, no timing limits: your ruling).
        /// Overload: 20 more for each Technical Machine on this Pokémon.
        /// </summary>
        private sealed class PorygonZDPP : Mon
        {
            private const string Install = "Install";

            private static bool IsTm(CardInstance tool) { return tool != null && tool.Def.Name.StartsWith("Technical Machine"); }

            private static List<PokemonInPlay> TmHolders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Tools.Any(IsTm) && ctx.MyPokemon(o => o != m && o.Tools.Count == 0).Count > 0);
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, Install) && ctx.IsMyTurn && NoCondition(ctx.Pokemon) && TmHolders(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => TmHolders(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, TmHolders(ctx), Install + ": move a Technical Machine from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var tms = from.Tools.Where(IsTm).ToList();
                    var tm = ctx.ChooseCards(tms, 1, 1, Install + ": move which Technical Machine?")[0];
                    var targets = ctx.MyPokemon(o => o != from && ctx.Engine.CanAttachToolTo(tm, o));
                    if (targets.Count == 0) return false;
                    var to = RepeatableAbility.ChoosePokemonOrDone(ctx, targets, Install + ": move " + tm.Def.Name + " to which Pokémon?", offerDone);
                    if (to == null || !from.Tools.Remove(tm)) return false;
                    ctx.Engine.AttachTool(tm, to);
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Learning")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var mine = ctx.MyPokemon();
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.IsLevelUp && mine.Any(m => m.Name == d.EvolvesFrom), 1, "Learning: choose a Pokémon LV.X that levels up from 1 of your Pokémon", false);
                    foreach (var lvx in found)
                    {
                        var targets = ctx.MyPokemon(m => m.Name == lvx.Def.EvolvesFrom);
                        var target = targets.Count == 0 ? null : ctx.ChoosePokemon(targets, "Learning: level up which Pokémon to " + lvx.Def.Name + "?");
                        if (target != null) ctx.Engine.EvolvePokemon(target, lvx);
                        else ctx.Me.Deck.Add(lvx);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Overload")
                {
                    ctx.DamageDefending(atk.Damage + 20 * ctx.Pokemon.Tools.Count(IsTm));
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Bind Eye (Poké-Body): while this Pokémon is your Active Pokémon, evolving or devolving doesn't remove the opponent's
        /// Pokémon's Special Conditions. Cutting Turn: heads, put damage counters on the Defending Pokémon until it has 10 HP left;
        /// if you do, shuffle this Pokémon and its cards into your deck. Friction Heat: the Defending Pokémon is now Burned.
        /// </summary>
        private sealed class GliscorDPP : Mon
        {
            private static readonly Mon FrictionHeat = new ConditionAttackMon("Friction Heat", SpecialCondition.Burned);

            public override bool KeepsConditionsOnEvolve(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner != src.Player && src.IsActive(src.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cutting Turn") return FrictionHeat.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                if (!ctx.Flip() || defending == null || !ctx.CanAffect(defending)) return true;
                int counters = (ctx.RemainingHp(defending) - 10) / 10;
                if (counters <= 0) return true;
                ctx.PlaceCounters(defending, counters);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Drag Off: before doing damage, you may switch 1 of the opponent's Benched Pokémon with the Defending Pokémon.</summary>
        private sealed class MayDragOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drag Off") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Drag Off: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Drag Off: choose 1 of your opponent's Benched Pokémon to switch in");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Leap Away (Poké-Power): from the Active Spot, flip a coin; heads, return this Pokémon and its cards to your hand (allowed
        /// even with no Bench, like Big Jump: you then have no Pokémon in play). Poison Revenge: if any of your Pokémon were Knocked
        /// Out by damage from an opponent's attack during their last turn, 40 more and the Defending Pokémon is Poisoned.
        /// </summary>
        private sealed class ToxicroakG : Mon
        {
            protected override string Activated { get { return "Leap Away"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Revenge") return false;
                var defending = ctx.Defending;
                int last = ctx.OpponentsLastTurn;
                bool revenge = ctx.Me.KnockOuts.Any(k => k.ByAttack && k.Turn == last);
                ctx.DamageDefending(atk.Damage + (revenge ? 40 : 0));
                if (revenge && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Power Whip: 10 for each Energy on this Pokémon, to 1 of the opponent's Pokémon.</summary>
        private sealed class PowerWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Whip") return false;
                int amount = 10 * ctx.EnergyCount(ctx.Pokemon);
                if (amount > 0) ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>Grass Knot: 10 more for each [C] in the Defending Pokémon's Retreat Cost.</summary>
        private sealed class GrassKnotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grass Knot") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null ? 10 * ctx.Engine.RetreatCostOf(defending) : 0));
                return true;
            }
        }

        /// <summary>Nose Unit: 20 to each of the opponent's Pokémon that has a Poké-Body printed on it or on the card under its LV.X (your ruling).</summary>
        private sealed class NoseUnitMon : Mon
        {
            internal static bool HasPokeBody(PokemonInPlay mon)
            {
                if (mon.Def.Abilities.Any(a => a.Kind == AbilityDef.PokeBody)) return true;
                return mon.Def.IsLevelUp && mon.Stack.Count >= 2 && mon.Stack[mon.Stack.Count - 2].Def.Abilities.Any(a => a.Kind == AbilityDef.PokeBody);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nose Unit") return false;
                foreach (var mon in ctx.OpponentPokemon(HasPokeBody)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Loud Snort: this Pokémon has no Retreat Cost until the end of your next turn.</summary>
        private sealed class LoudSnortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loud Snort") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new CardsGRI2.FreeRetreatEffect(me.Name + " has no Retreat Cost until the end of your next turn (Loud Snort)", ctx.Player, me.Uid, ctx.Turn + 2));
                return true;
            }
        }

        /// <summary>Charge Beam: attach an Energy card from your discard pile to this Pokémon.</summary>
        private sealed class ChargeBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge Beam") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (!ctx.InPlay(me) || energy.Count == 0) return true;
                var card = energy.Select(c => c.Def.Key).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Charge Beam: attach which Energy card to " + me.Name + "?")[0];
                AttachFromDiscard(ctx, card, me);
                return true;
            }
        }

        /// <summary>Metallic Bolt: you may discard a Lightning and a Metal Energy from this Pokémon for 120 instead of 60.</summary>
        private sealed class MetallicBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metallic Bolt") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning)).ToList();
                var metal = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Metal)).ToList();
                bool canBoost = lightning.Count > 0 && metal.Count > 0 && (lightning.Union(metal).Count() >= 2);
                int damage = atk.Damage;
                if (canBoost && ctx.AskYesNo("Metallic Bolt: discard a Lightning and a Metal Energy for 120 damage?"))
                {
                    var l = ctx.ChooseCards(lightning, 1, 1, "Metallic Bolt: discard which Lightning Energy?")[0];
                    var mLeft = metal.Where(x => x != l).ToList();
                    if (mLeft.Count > 0)
                    {
                        var m = ctx.ChooseCards(mLeft, 1, 1, "Metallic Bolt: discard which Metal Energy?")[0];
                        if (ctx.Engine.DiscardEnergy(me, l) && ctx.Engine.DiscardEnergy(me, m)) damage = 120;
                    }
                }
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Time Call: put a card that evolves from 1 of your Pokémon from your deck onto it.</summary>
        private sealed class TimeCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Call") return false;
                if (ctx.Me.Deck.Count > 0) CardsGEN2.EvolveOneFromDeck(ctx, ctx.MyPokemon(), null, "Time Call");
                return true;
            }
        }

        /// <summary>Time Wager: flip 2 coins; unless both are heads, 50 instead of 100.</summary>
        private sealed class TimeWagerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Wager") return false;
                ctx.DamageDefending(ctx.FlipCoins(2) == 2 ? atk.Damage : 50);
                return true;
            }
        }

        /// <summary>Judgment: discard all Energy cards from this Pokémon; 10 to each of the opponent's Benched Pokémon too.</summary>
        private sealed class JudgmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Judgment") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 10);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Lunar Aura (Poké-Body): with Darkrai in play, remove 1 damage counter from this Pokémon between turns. Lunar Flight: 80 minus 10 per damage counter on it.</summary>
        private sealed class CresseliaDPP : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.DamageCounters == 0 || !src.Me.InPlay().Any(m => Named(m, "Darkrai"))) return;
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lunar Flight") return false;
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        /// <summary>Darkness Aura (Poké-Body): with Cresselia in play, its attacks do 20 more to the Defending Pokémon. Hypnoblast: heads, the Defending Pokémon is Asleep.</summary>
        private sealed class DarkraiDPP : Mon
        {
            private static readonly Mon Hypnoblast = new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || !info.ToOpponentsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Me.InPlay().Any(m => Named(m, "Cresselia")) ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hypnoblast.HandleAttack(ctx, atk); }
        }
    }
}
