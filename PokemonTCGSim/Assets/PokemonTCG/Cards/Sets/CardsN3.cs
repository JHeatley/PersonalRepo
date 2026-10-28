using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Revelation Pokémon (N3 1-59, 65; v141). Unlimited only. Shining Magikarp 66 is Celebrations' Classic Collection card.
    /// Smoochum is a Baby Pokémon; Unown [B], [Y] and [K] follow the Unown deck rule; Shining Gyarados is 1 per deck.
    /// Your rulings: Dark Flame takes basic or Special Darkness Energy; Time Travel only fails for a condition Celebi had before
    /// the attack; Legendary Body stops every player's non-Stadium Trainer cards on it (switching included); Allergic Pollen
    /// covers attacks and old Pokémon Powers only (GameEngine.DiscardShield). Pokémon whose attacks only do their printed
    /// damage: Geodude 44.
    /// </summary>
    public static class CardsN3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N3-1", new AmpharosN3());
            r.RegisterByKey("N3-2", new SoftboiledMon());
            r.RegisterByKey("N3-3", new TimeTravelMon());
            r.RegisterByKey("N3-4", new CrobatN3());
            r.RegisterByKey("N3-5", new PresentMon());
            r.RegisterByKey("N3-6", new HowlMon());
            r.RegisterByKey("N3-7", new ComboMon(new StokeMon(), new CardsSI.SnipeMon("Sacred Fire", 40, true), new TailsNothingMon("Dive Bomb")));
            r.RegisterByKey("N3-8", new HoundoomN3());
            r.RegisterByKey("N3-9", new EvolutionarySporeMon());
            r.RegisterByKey("N3-10", new ElectromagneticPowerMon());
            r.RegisterByKey("N3-11", new MisdreavusN3());
            r.RegisterByKey("N3-12", new EnergyConverterMon());
            r.RegisterByKey("N3-13", new LightningBurstMon());
            r.RegisterByKey("N3-14", new CrystalBodyMon());
            r.RegisterByKey("N3-15", new PrehistoricMemoryMon());
            r.RegisterByKey("N3-16", new PsychicLeafMon());
            r.RegisterByKey("N3-17", new LegendaryBodyMon(new MegaFlameMon()));
            r.RegisterByKey("N3-18", new RainbowBurnMon());
            r.RegisterByKey("N3-19", new ComboMon(new GeneticMemoryMon(), new CardsNP.TwisterMon()));
            r.RegisterByKey("N3-20", new AerowingMon());
            r.RegisterByKey("N3-21", new ComboMon(new ParalyzeFlipMon("Thundershock"), new LightningStrikeMon()));
            r.RegisterByKey("N3-22", new LegendaryBodyMon(new LightningSparkMon()));
            r.RegisterByKey("N3-23", new ComboMon(new CoinsMon("Fury Attack", 2), new DodgeMon("Agility", false)));
            r.RegisterByKey("N3-24", new ComboMon(new SwipeMon(), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("N3-25", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new CoreStreamMon()));
            r.RegisterByKey("N3-26", new MimicMon());
            r.RegisterByKey("N3-27", new LegendaryBodyMon(new CrystalWaveMon()));
            r.RegisterByKey("N3-28", new TailShockMon());
            r.RegisterByKey("N3-29", new ComboMon(new CardsCES2.ScreechMon("Screech", 20), new CardsN1.PoisonBiteMon()));
            r.RegisterByKey("N3-30", new ComboMon(new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10), new NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("N3-31", new ComboMon(new ConditionAttackMon("Freezing Kiss Goodnight", SpecialCondition.Asleep),
                new CardsPLS1.FlipBonusConditionMon("Strange Dance", 20, SpecialCondition.Confused)));
            r.RegisterByKey("N3-32", new SubmergeMon());
            r.RegisterByKey("N3-33", new MagmaPoolMon());
            r.RegisterByKey("N3-34", new ComboMon(new ParalyzeFlipMon("Constrict"), new OctazookaMon()));
            r.RegisterByKey("N3-35", new AllergicPollenMon());
            r.RegisterByKey("N3-36", new PiloswineN3());
            r.RegisterByKey("N3-37", new ComboMon(new FlipBonusMon("Rising Lunge", 10), new HornSwipeMon()));
            r.RegisterByKey("N3-38", new ComboMon(new TerrorizeMon(), new OverheadTossMon()));
            r.RegisterByKey("N3-39", new BearUnown());
            r.RegisterByKey("N3-40", new YieldUnown());
            r.RegisterByKey("N3-41", new GrabMon());
            r.RegisterByKey("N3-42", new ComboMon(new FlipBonusMon("Positive Ion", 10), new NegativeIonMon()));
            r.RegisterByKey("N3-43", new LockAfterAttackMon("Leek Jab", true));
            r.RegisterByKey("N3-44", new PlainMon());
            r.RegisterByKey("N3-45", new CoinsMon("Fin Smack", 2));
            r.RegisterByKey("N3-46", new ComboMon(new CardsJU.FamilyMon("Call for Family", d => d.Name == "Murkrow", "a Basic Pokémon named Murkrow"), new FlockAttackMon()));
            r.RegisterByKey("N3-47", new CardsPAR2.FlipConditionMon("Sleep Spore", SpecialCondition.Asleep));
            r.RegisterByKey("N3-48", new CardsDRI2.CrunchMon("Whirlpool"));
            r.RegisterByKey("N3-49", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned), new NeedleStormMon()));
            r.RegisterByKey("N3-50", new CardsSI.SnipeMon("Sharpshooting", 20, true));
            r.RegisterByKey("N3-51", new HardShellMon());
            r.RegisterByKey("N3-52", new CardsPAR2.FlipConditionMon("Mysterious Powder", SpecialCondition.Confused));
            r.RegisterByKey("N3-53", new MagmaRingMon());
            r.RegisterByKey("N3-54", new PsykissMon());
            r.RegisterByKey("N3-55", new RagingHeadbuttMon());
            r.RegisterByKey("N3-56", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("N3-57", new RecoilMon("Take Down", 10));
            r.RegisterByKey("N3-58", new KeepUnown());
            r.RegisterByKey("N3-59", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
            r.RegisterByKey("N3-65", new ShiningGyaradosN3());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        private static bool EnergyOf(CardDef d, EnergyType type) { return d != null && d.IsEnergy && d.EnergyProvides == type; }

        private static readonly EnergyType[] EnergyTypes =
        {
            EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning,
            EnergyType.Psychic, EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal
        };

        /// <summary>"Discard N [type] Energy cards attached to this Pokémon or this attack does nothing." False when it can't be paid.</summary>
        private static bool PayDiscard(EffectContext ctx, EnergyType type, int count, string attack)
        {
            if (CardsBS1.CopyingWithMetronome) return true;
            var me = ctx.Pokemon;
            var cards = me == null ? new List<CardInstance>() : me.Energy.Where(e => EnergyOf(e.Def, type)).ToList();
            if (cards.Count < count) return false;
            var picks = cards.Count == count ? cards : ctx.ChooseCards(cards, count, count, attack + ": discard " + count + " " + type + " Energy card" + (count == 1 ? "" : "s"));
            int paid = 0;
            foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) paid++;
            return paid == count;
        }

        /// <summary>The Pokémon cards under this Pokémon's top card: its Basic Pokémon card and lower Evolution cards (not Baby Pokémon or Trainers).</summary>
        private static List<CardDef> LowerPokemonCards(PokemonInPlay mon)
        {
            var list = new List<CardDef>();
            if (mon == null) return list;
            for (int i = 0; i < mon.Stack.Count - 1; i++)
            {
                var d = mon.Stack[i].Def;
                if (d.IsPokemon && !d.IsBaby && !list.Contains(d)) list.Add(d);
            }
            return list;
        }

        // ================================================================== Holo Pokémon

        /// <summary>Attract Current: 3 coins; for each heads you may attach a Lightning Energy card from your deck to 1 of your Lightning Pokémon. Gigavolt: heads 20 more, tails Paralyzed.</summary>
        private sealed class AmpharosN3 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Attract Current")
                {
                    ctx.DamageDefending(atk.Damage);
                    int heads = ctx.FlipCoins(3);
                    if (heads == 0 || ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => EnergyOf(d, EnergyType.Lightning), heads, "Attract Current: choose up to " + heads + " Lightning Energy cards");
                    foreach (var card in found)
                    {
                        var targets = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Lightning));
                        if (targets.Count == 0) { ctx.Me.Deck.Add(card); continue; }
                        var target = ctx.ChoosePokemon(targets, "Attract Current: attach " + card.Def.Name + " to which of your Lightning Pokémon?");
                        if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                        else ctx.Me.Deck.Add(card);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Gigavolt")
                {
                    var defending = ctx.Defending;
                    bool heads = ctx.Flip();
                    ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                    if (!heads && defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Softboiled (Pokémon Power): when you play this Pokémon from your hand, you may flip a coin; heads 8 damage counters off it, tails 4.</summary>
        private sealed class SoftboiledMon : Mon
        {
            private static readonly Mon BodySlam = new ParalyzeFlipMon("Body Slam");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || me.DamageCounters == 0 || !ctx.AskYesNo("Softboiled: flip a coin to remove damage counters from " + me.Name + "?")) return;
                Announce(ctx, "Softboiled");
                ctx.Heal(me, ctx.Flip() ? 80 : 40);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BodySlam.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Time Travel (Pokémon Power): if an opponent's attack would Knock Out this Pokémon, flip a coin; heads, it's shuffled with its
        /// cards into your deck instead. It doesn't work if it was Asleep, Confused or Paralyzed before that attack (your ruling).
        /// Psychic Damage: 3 coins, 1 damage counter on the Defending Pokémon for each heads.
        /// </summary>
        private sealed class TimeTravelMon : Mon
        {
            private const string Marker = "Time Travel off";

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                var me = src.Pokemon;
                if (me == null) return;
                if (PowerWorks(me)) me.Markers.Remove(Marker);
                else me.Markers[Marker] = src.Turn;
            }

            public override bool EscapesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon || !mon.HitByAttack || mon.ForcedKnockOut || mon.HitByPlayer != 1 - src.Player) return false;
                int turn;
                if (mon.Markers.TryGetValue(Marker, out turn) && turn == src.Turn) return false;
                Announce(src, "Time Travel");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Damage") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.PlaceCounters(defending, heads);
                return true;
            }
        }

        /// <summary>
        /// Triggered Poison: if your opponent attaches an Energy card to the Defending Pokémon during their next turn, it's Poisoned.
        /// Cross Attack: 4 coins, 20 per heads; 2 or more heads, Confused.
        /// </summary>
        private sealed class CrobatN3 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Triggered Poison")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (defending != null && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.AddEffect(new TriggeredPoisonEffect(defending.Name + " is Poisoned if an Energy card is attached to it during its next turn (Triggered Poison)",
                            ctx.Player, defending.Uid, ctx.Turn + 1) { ActiveFromTurn = ctx.Turn + 1 });
                    return true;
                }
                if (atk.Name == "Cross Attack")
                {
                    int heads = ctx.FlipCoins(4);
                    ctx.DamageDefending(atk.Damage * heads);
                    if (heads >= 2 && defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        private sealed class TriggeredPoisonEffect : ContinuousEffect
        {
            public TriggeredPoisonEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnEnergyAttached(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target.Uid != TargetUid || engine.State.CurrentPlayer != target.Owner) return;
                engine.Emit(GameEventType.Info, Owner, "Triggered Poison: " + target.Name + " is now Poisoned.", target.Top.Id, target.Uid);
                engine.ApplyCondition(target, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Present: 3 coins. 1 heads, 40 damage; 2 heads, 3 damage counters off the Defending Pokémon; 3 heads, 60 damage; 0 heads, all its counters off.</summary>
        private sealed class PresentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Present") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                if (heads == 1) ctx.DamageDefending(40);
                else if (heads == 3) ctx.DamageDefending(60);
                else if (defending != null && defending.DamageCounters > 0 && ctx.CanAffect(defending))
                    ctx.Heal(defending, heads == 2 ? 30 : defending.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Howl (Pokémon Power): when you play this Pokémon from your hand, you may discard the top 5 cards of your deck and attach the
        /// Fire Energy cards among them to your Fire Pokémon as you like; your turn ends. Searing Flames: discard 2 Fire Energy cards or nothing.
        /// </summary>
        private sealed class HowlMon : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Pokemon == null || ctx.Me.Deck.Count == 0 || !ctx.AskYesNo("Howl: discard the top 5 cards of your deck? (Your turn ends.)")) return;
                Announce(ctx, "Howl");
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                var fire = discarded.Where(c => EnergyOf(c.Def, EnergyType.Fire) && ctx.Me.Discard.Contains(c)).ToList();
                if (fire.Count > 0)
                    ctx.AttachCardsAnyWay(fire, fire.Count, (card, mon) => ctx.Engine.IsType(mon, EnergyType.Fire), "Howl", false);
                ctx.Log(ctx.Engine.Possessive(ctx.Player) + " turn ends (Howl).");
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searing Flames") return false;
                if (!PayDiscard(ctx, EnergyType.Fire, 2, atk.Name)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Stoke: you may attach a Fire Energy card from your deck to this Pokémon.</summary>
        private sealed class StokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => EnergyOf(d, EnergyType.Fire), 1, "Stoke: you may choose a Fire Energy card to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Dark Flame: discard a Fire Energy card or nothing; then attach a Darkness Energy card (basic or Neo Genesis's Special: your
        /// ruling) from your discard pile to this Pokémon. Black Fang: a coin per Darkness Energy attached, 30 + 10 per heads.
        /// </summary>
        private sealed class HoundoomN3 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Dark Flame")
                {
                    if (!PayDiscard(ctx, EnergyType.Fire, 1, atk.Name)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var dark = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.Name == "Darkness Energy").ToList();
                    if (dark.Count == 0) return true;
                    var card = ctx.ChooseCards(dark, 1, 1, "Dark Flame: attach which Darkness Energy card to " + me.Name + "?")[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                    return true;
                }
                if (atk.Name == "Black Fang")
                {
                    int coins = me == null ? 0 : ctx.Engine.EnergyCountOfType(me, EnergyType.Darkness);
                    int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                    ctx.DamageDefending(atk.Damage + 10 * heads);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Evolutionary Spore: choose any of your Hoppip and Skiploom; each may evolve with a card from your deck that evolves from it.</summary>
        private sealed class EvolutionarySporeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evolutionary Spore") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var candidates = ctx.MyPokemon(m => m.Name == "Hoppip" || m.Name == "Skiploom");
                if (candidates.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var chosen = ctx.ChoosePokemonMany(candidates, 0, candidates.Count, "Evolutionary Spore: choose any of your Hoppip and Skiploom");
                foreach (var mon in chosen)
                {
                    if (!ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) continue;
                    if (CardsEVS1.EvolveBlocked(ctx, mon))
                    {
                        ctx.Log(mon.Name + " can't evolve right now (Evolutionary Spore).");
                        continue;
                    }
                    var found = ctx.SearchMyDeck(d => CardsEVS1.EvolvesOnto(ctx, mon, d), 1, "Evolutionary Spore: you may choose a card that evolves from " + mon.Name, false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(mon, found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Electromagnetic Power (Pokémon Power): as often as you like during your turn, move an Energy card between your Magnemite,
        /// Magneton and Dark Magneton. Plasma: attach a Lightning Energy card from your discard pile to this Pokémon.
        /// </summary>
        private sealed class ElectromagneticPowerMon : Mon
        {
            private const string Power = "Electromagnetic Power";
            private static readonly Mon Plasma = new CardsOBF1.AttachFromDiscardToSelfMon("Plasma", EnergyType.Lightning);

            private static List<PokemonInPlay> Magnets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Name == "Magnemite" || m.Name == "Magneton" || m.Name == "Dark Magneton");
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (!IsAbility(ctx, abilityIndex, Power) || !ctx.IsMyTurn || !PowerWorks(ctx.Pokemon)) return false;
                var magnets = Magnets(ctx);
                return magnets.Count >= 2 && magnets.Any(m => m.Energy.Count > 0);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var magnets = Magnets(ctx);
                var from = magnets.Where(m => m.Energy.Count > 0).ToList();
                if (from.Count == 0 || magnets.Count < 2) return;
                var source = ctx.ChoosePokemon(from, Power + ": move an Energy card from which Pokémon?");
                if (source == null) return;
                var card = ctx.ChooseCards(source.Energy.ToList(), 1, 1, Power + ": move which Energy card?")[0];
                var to = magnets.Where(m => m != source).ToList();
                var target = ctx.ChoosePokemon(to, Power + ": attach " + card.Def.Name + " to which Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(source, target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Plasma.HandleAttack(ctx, atk); }
        }

        /// <summary>Night Eyes: the Defending Pokémon is now Asleep. Perish Song: if it's Asleep and was hit by Night Eyes during your last turn, it's Knocked Out.</summary>
        private sealed class MisdreavusN3 : Mon
        {
            private static bool MarkedByNightEyes(EffectContext ctx, PokemonInPlay mon)
            {
                foreach (var e in ctx.State.Effects)
                {
                    var mark = e as NightEyesMark;
                    if (mark != null && mark.TargetUid == mon.Uid && mark.Owner == ctx.Player && mark.Turn == ctx.MyLastTurn) return true;
                }
                return false;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Night Eyes")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    ctx.State.Effects.Add(new NightEyesMark("Night Eyes on " + defending.Name, ctx.Player, defending.Uid, ctx.Turn + 2, ctx.Turn));
                    return true;
                }
                if (atk.Name == "Perish Song")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                    if (defending.Has(SpecialCondition.Asleep) && MarkedByNightEyes(ctx, defending)) ctx.Engine.KnockOut(defending, "Perish Song");
                    else ctx.Log("Perish Song did nothing.");
                    return true;
                }
                return false;
            }
        }

        /// <summary>A note that Night Eyes hit this Pokémon on a turn (Perish Song checks it). Benching or evolving it ends it.</summary>
        private sealed class NightEyesMark : ContinuousEffect
        {
            public readonly int Turn;

            public NightEyesMark(string name, int owner, int targetUid, int expiresAfterTurn, int turn) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                Turn = turn;
            }
        }

        /// <summary>
        /// Energy Converter (Pokémon Power): once during your turn, a basic Energy card on 1 of your Pokémon counts as the type you
        /// choose until the end of the turn; it goes back if this Pokémon becomes Asleep, Confused or Paralyzed. Delta Beam: heads,
        /// Asleep, Confused or Paralyzed (your choice).
        /// </summary>
        private sealed class EnergyConverterMon : Mon
        {
            private const string Power = "Energy Converter";

            protected override string Activated { get { return Power; } }

            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy)); }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && Holders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holders = Holders(ctx);
                if (holders.Count == 0) return;
                var holder = ctx.ChoosePokemon(holders, Power + ": choose a basic Energy card on which Pokémon?");
                if (holder == null) return;
                var cards = holder.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": which basic Energy card?")[0];
                int pick = ctx.ChooseOption(Power + ": treat " + card.Def.Name + " as which type?", EnergyTypes.Select(t => t.ToString()).ToArray());
                if (pick < 0 || pick >= EnergyTypes.Length) return;
                var type = EnergyTypes[pick];
                ctx.State.Effects.RemoveAll(e => e is ConverterEffect && ((ConverterEffect)e).EnergyId == card.Id);
                ctx.AddEffect(new ConverterEffect(card.Def.Name + " on " + holder.Name + " is " + type + " Energy this turn (Energy Converter)", ctx.Player, ctx.Turn, card.Id, ctx.Pokemon.Uid, type));
            }

            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || PowerWorks(me)) return;
                src.State.Effects.RemoveAll(e => e is ConverterEffect && ((ConverterEffect)e).PorygonUid == me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                int pick = ctx.ChooseOption("Delta Beam: the Defending Pokémon is now...", "Asleep", "Confused", "Paralyzed");
                var condition = pick == 1 ? SpecialCondition.Confused : pick == 2 ? SpecialCondition.Paralyzed : SpecialCondition.Asleep;
                ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        private sealed class ConverterEffect : ContinuousEffect
        {
            public readonly int EnergyId;
            public readonly int PorygonUid;
            private readonly EnergyType type;

            public ConverterEffect(string name, int owner, int expiresAfterTurn, int energyId, int porygonUid, EnergyType type) : base(name, owner, -1, expiresAfterTurn, false)
            {
                EnergyId = energyId;
                PorygonUid = porygonUid;
                this.type = type;
            }

            public override void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy == null || energy.Id != EnergyId || units.Count == 0) return;
                var porygon = engine.State.FindPokemon(PorygonUid);
                if (porygon == null || !PowerWorks(porygon) || engine.AbilityBlocked(porygon, "Energy Converter")) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(type);
            }
        }

        /// <summary>
        /// Lightning Burst (Pokémon Power): whenever you attach a Lightning Energy card from your hand to this Pokémon, your opponent
        /// switches their Active Pokémon with 1 of their Benched Pokémon (they choose). Lightning Tackle: tails, 20 to itself.
        /// </summary>
        private sealed class LightningBurstMon : Mon
        {
            private static readonly Mon Tackle = new CardsFST1.FlipRecoilMon("Lightning Tackle", 20);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !EnergyOf(energy.Def, EnergyType.Lightning) || !PowerWorks(me) || src.Opp.Bench.Count == 0) return;
                Announce(src, "Lightning Burst");
                src.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Crystal Body (Pokémon Power): prevent all effects of the opponent's attacks, except damage, done to this Pokémon. Aurora Wave:
        /// 2 heads Paralyzed, 1 heads Asleep.
        /// </summary>
        private sealed class CrystalBodyMon : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack || !PowerWorks(src.Pokemon)) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aurora Wave") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                if (heads == 0 || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, heads == 2 ? SpecialCondition.Paralyzed : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Prehistoric Memory (Pokémon Power): any Evolved Pokémon (both players') can use the attacks of the Pokémon cards under it,
        /// paying their costs. Fly: heads, protected; tails, nothing.
        /// </summary>
        private sealed class PrehistoricMemoryMon : Mon
        {
            private static readonly Mon Fly = new DodgeMon("Fly", true);

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon == null || mon.Stack.Count < 2 || !PowerWorks(src.Pokemon)) yield break;
                foreach (var d in LowerPokemonCards(mon))
                    for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fly.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Leaf: a coin for each Energy on the Defending Pokémon, 10 + 10 per heads; heal this Pokémon by the damage done.</summary>
        private sealed class PsychicLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Leaf") return false;
                var defending = ctx.Defending;
                int coins = defending == null ? 0 : ctx.EnergyCount(defending);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                int done = ctx.DamageDefending(atk.Damage + 10 * heads);
                var me = ctx.Pokemon;
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, done);
                return true;
            }
        }

        // ================================================================== Legendary Body

        /// <summary>
        /// Legendary Body (Pokémon Power; works while Asleep, Confused or Paralyzed): while this Pokémon is the Active Pokémon, no
        /// Trainer card other than a Stadium affects it or its Energy, whoever plays it (your ruling), and its Tools are discarded.
        /// </summary>
        private sealed class LegendaryBodyMon : Mon
        {
            private readonly Mon attacks;

            public LegendaryBodyMon(Mon attacks) { this.attacks = attacks; }

            private static bool Active(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon); }

            public override bool PreventsAllTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return target == src.Pokemon && Active(src) && trainer != null && !trainer.IsStadium;
            }

            public override bool ProtectsEnergyFromTrainerDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy) { return holder == src.Pokemon && Active(src); }

            public override bool ProtectsEnergyFromTrainerRemoval(EffectContext src, PokemonInPlay holder, CardInstance energy) { return holder == src.Pokemon && Active(src); }

            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.Tools.Count == 0 || !Active(src) || src.Engine.AbilityBlocked(me, "Legendary Body")) return;
                Announce(src, "Legendary Body");
                src.Engine.DiscardAllTools(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Mega Flame: the damage, then 2 coins; discard a Fire Energy card from this Pokémon for each tails.</summary>
        private sealed class MegaFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Flame") return false;
                ctx.DamageDefending(atk.Damage);
                int tails = 2 - ctx.FlipCoins(2);
                var me = ctx.Pokemon;
                if (tails > 0 && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, tails, "Mega Flame: discard " + tails + " Fire Energy card" + (tails == 1 ? "" : "s"), e => EnergyOf(e.Def, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Lightning Spark: if the opponent has Benched Pokémon, flip; heads, 20 to 1 of them.</summary>
        private sealed class LightningSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Spark") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Flip()) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Crystal Wave: heads, 10 more; tails, the opponent switches the Defending Pokémon with 1 of their Benched Pokémon after the damage.</summary>
        private sealed class CrystalWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crystal Wave") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (!heads) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        // ================================================================== Other rares

        /// <summary>Rainbow Burn: 30 + 10 for each type of basic Energy card attached to this Pokémon.</summary>
        private sealed class RainbowBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Burn") return false;
                var me = ctx.Pokemon;
                int types = me == null ? 0 : me.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage + 10 * types);
                return true;
            }
        }

        /// <summary>Genetic Memory: use an attack of this Pokémon's Basic Pokémon card or Evolution card, without its Energy cost.</summary>
        internal sealed class GeneticMemoryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Genetic Memory") return false;
                var opts = new List<DecisionOption>();
                foreach (var d in LowerPokemonCards(ctx.Pokemon))
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                if (opts.Count == 0) return Nothing(ctx);
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Genetic Memory: choose an attack to use", opts, ctx.Source);
                if (pick < 0 || pick >= opts.Count) pick = 0;
                UseCopiedAttack(ctx, opts[pick].Card, opts[pick].Index);
                return true;
            }
        }

        /// <summary>Aerowing: you may flip a coin; heads 80, tails nothing. Without flipping, 40.</summary>
        private sealed class AerowingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aerowing") return false;
                if (!ctx.AskYesNo("Aerowing: flip a coin (heads 80 damage, tails nothing)?"))
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(80);
                return true;
            }
        }

        /// <summary>Lightning Strike: you may discard all Lightning Energy cards attached to this Pokémon to make the damage 80.</summary>
        private sealed class LightningStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                var lightning = me == null ? new List<CardInstance>() : me.Energy.Where(e => EnergyOf(e.Def, EnergyType.Lightning)).ToList();
                int damage = atk.Damage;
                if (lightning.Count > 0 && ctx.AskYesNo("Lightning Strike: discard all Lightning Energy cards for 80 damage?"))
                {
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                    damage = 80;
                }
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Swipe: heads, discard all Trainer cards attached to the opponent's Pokémon.</summary>
        private sealed class SwipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swipe") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0))
                    if (ctx.CanAffect(mon)) ctx.Engine.DiscardAllTools(mon);
                return true;
            }
        }

        /// <summary>Core Stream: choose a type other than Colorless; 20 to each of the opponent's Pokémon with an Energy card of that type attached (no W/R).</summary>
        private sealed class CoreStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Stream") return false;
                int pick = ctx.ChooseOption("Core Stream: choose an Energy type", EnergyTypes.Select(t => t.ToString()).ToArray());
                if (pick < 0 || pick >= EnergyTypes.Length) pick = 0;
                var type = EnergyTypes[pick];
                ctx.Log("Core Stream: " + type + ".");
                foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, type))))
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Mimic (Pokémon Power): while this Pokémon is your Active Pokémon, it has the Defending Pokémon's attacks (paying their costs).</summary>
        private sealed class MimicMon : Mon
        {
            private static readonly Mon Slam = new CoinsMon("Slam", 2);

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var me = src.Pokemon;
                var defending = src.Opp.Active;
                if (me == null || defending == null || !src.IsActive(me) || !PowerWorks(me)) yield break;
                for (int i = 0; i < defending.Def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(defending.Def, i);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slam.HandleAttack(ctx, atk); }
        }

        // ================================================================== Uncommons

        /// <summary>Tail Shock: heads, 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class TailShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Shock") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                foreach (var benched in ctx.OpponentBench())
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>"Don't apply Resistance." (Rock Tumble.)</summary>
        private sealed class NoResistanceMon : Mon
        {
            private readonly string attack;

            public NoResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }

        /// <summary>
        /// Submerge (Pokémon Power): once during your turn, this Pokémon's type is Water until the end of the turn; it's Lightning again
        /// once it becomes Asleep, Confused or Paralyzed. Blinding Light: heads, Confused.
        /// </summary>
        private sealed class SubmergeMon : Mon
        {
            private const string Power = "Submerge";
            private static readonly Mon BlindingLight = new CardsPAR2.FlipConditionMon("Blinding Light", SpecialCondition.Confused);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Pokemon.Markers[Power] = ctx.Turn;
                ctx.Log(ctx.Pokemon.Name + " is a Water Pokémon until the end of the turn (Submerge).");
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                int turn;
                if (mon == null || mon != src.Pokemon || !mon.Markers.TryGetValue(Power, out turn) || turn != src.Turn || !PowerWorks(mon)) return;
                types.Clear();
                types.Add(EnergyType.Water);
            }

            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me != null && me.Markers.ContainsKey(Power) && !PowerWorks(me)) me.Markers.Remove(Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BlindingLight.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Magma Pool (Pokémon Power): when this Active Pokémon moves to the Bench, 1 Fire Energy card attached to it moves to the new
        /// Active Pokémon. Lava Flow: discard any number of Fire Energy cards from it, 20 more for each.
        /// </summary>
        private sealed class MagmaPoolMon : Mon
        {
            public override void OnLeftActiveSpot(EffectContext ctx, PokemonInPlay newActive)
            {
                var me = ctx.Pokemon;
                if (me == null || newActive == null || !ctx.InPlay(newActive)) return;
                var fire = me.Energy.Where(e => EnergyOf(e.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return;
                Announce(ctx, "Magma Pool");
                var card = ctx.ChooseCards(fire, 1, 1, "Magma Pool: move which Fire Energy card to " + newActive.Name + "?")[0];
                if (me.Energy.Remove(card)) ctx.Engine.AttachEnergyCard(card, newActive);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Flow") return false;
                var me = ctx.Pokemon;
                var fire = me == null ? new List<CardInstance>() : me.Energy.Where(e => EnergyOf(e.Def, EnergyType.Fire)).ToList();
                int discarded = 0;
                if (fire.Count > 0)
                {
                    int n = ctx.AskNumber(0, fire.Count, "Lava Flow: discard how many Fire Energy cards (20 more damage each)?");
                    if (n > 0)
                    {
                        var picks = n >= fire.Count ? fire : ctx.ChooseCards(fire, n, n, "Lava Flow: discard which Fire Energy cards?");
                        foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 20 * discarded);
                return true;
            }
        }

        /// <summary>Octazooka: heads, whenever the Defending Pokémon attacks, its owner flips a coin; tails, that attack does nothing (until it's Benched or evolves).</summary>
        private sealed class OctazookaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Octazooka") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new OctazookaEffect(defending.Name + " has to flip a coin to attack (Octazooka)", ctx.Player, defending.Uid));
                return true;
            }
        }

        private sealed class OctazookaEffect : ContinuousEffect
        {
            public OctazookaEffect(string name, int owner, int targetUid) : base(name, owner, targetUid, int.MaxValue, true) { }

            public override bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return attacker != null && attacker.Uid == TargetUid; }
        }

        /// <summary>Allergic Pollen (Pokémon Power): cards in discard piles aren't affected by attacks or Pokémon Powers (GameEngine.DiscardShield). Sleep Pinchers: heads, Asleep.</summary>
        private sealed class AllergicPollenMon : Mon
        {
            private static readonly Mon Pinchers = new CardsPAR2.FlipConditionMon("Sleep Pinchers", SpecialCondition.Asleep);

            public override bool ShieldsDiscardPiles(EffectContext src) { return src.Pokemon != null && PowerWorks(src.Pokemon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pinchers.HandleAttack(ctx, atk); }
        }

        /// <summary>Nap: 3 damage counters off this Pokémon. High-Speed Charge: 30 to itself; it can't be used during your next turn.</summary>
        private sealed class PiloswineN3 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Nap")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30);
                    return true;
                }
                if (atk.Name == "High-Speed Charge")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.DamageSelf(30);
                    if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Horn Swipe: 2 coins; both heads, 40 more.</summary>
        private sealed class HornSwipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horn Swipe") return false;
                ctx.DamageDefending(atk.Damage + (ctx.FlipCoins(2) == 2 ? 40 : 0));
                return true;
            }
        }

        /// <summary>Terrorize: if the Defending Pokémon is a Basic Pokémon, choose 1 of its attacks; it can't use it during the opponent's next turn.</summary>
        private sealed class TerrorizeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Terrorize") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null || !defending.Def.IsBasicPokemon || defending.Def.Attacks.Count == 0 || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var names = defending.Def.Attacks.Select(a => a.Name).ToArray();
                int pick = names.Length == 1 ? 0 : ctx.ChooseOption("Terrorize: which attack can't " + defending.Name + " use?", names);
                if (pick < 0 || pick >= names.Length) pick = 0;
                ctx.AddEffect(new TerrorizeEffect(defending.Name + " can't use " + names[pick] + " during its next turn (Terrorize)", ctx.Player, defending.Uid, ctx.Turn + 1, names[pick]) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class TerrorizeEffect : ContinuousEffect
        {
            private readonly string attackName;

            public TerrorizeEffect(string name, int owner, int targetUid, int expiresAfterTurn, string attackName) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.attackName = attackName;
            }

            public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string name)
            {
                return mon != null && mon.Uid == TargetUid && name == attackName;
            }
        }

        /// <summary>Overhead Toss: if you have Benched Pokémon, flip; tails, 10 to 1 of them (your choice).</summary>
        private sealed class OverheadTossMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overhead Toss") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0 || ctx.Flip()) return true;
                var target = ctx.ChoosePokemon(bench, "Overhead Toss: 10 damage to which of your Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 10);
                return true;
            }
        }

        // ================================================================== Unown

        /// <summary>[Bear]: once during your turn, move 1 damage counter from another of your Unown to this one (not with 10 HP left). Works while Asleep, Confused or Paralyzed.</summary>
        private sealed class BearUnown : Mon
        {
            protected override string Activated { get { return "[Bear]"; } }

            private static List<PokemonInPlay> Donors(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m != ctx.Pokemon && m.DamageCounters > 0 && m.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0);
            }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.RemainingHp(ctx.Pokemon) > 10 && Donors(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var donors = Donors(ctx);
                if (donors.Count == 0) return;
                var from = ctx.ChoosePokemon(donors, "[Bear]: move a damage counter from which Unown?");
                if (from != null) ctx.Engine.MoveDamageCounters(from, ctx.Pokemon, 1, "[Bear]");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Yield]: once during your turn, flip; heads, attach a Psychic Energy card from your deck to 1 of your Unown. Works while Asleep, Confused or Paralyzed.</summary>
        private sealed class YieldUnown : Mon
        {
            protected override string Activated { get { return "[Yield]"; } }

            private static List<PokemonInPlay> Unown(EffectContext ctx) { return ctx.MyPokemon(m => m.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Unown(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => EnergyOf(d, EnergyType.Psychic), 1, "[Yield]: choose a Psychic Energy card");
                foreach (var card in found)
                {
                    var unown = Unown(ctx);
                    var target = ctx.ChoosePokemon(unown, "[Yield]: attach " + card.Def.Name + " to which Unown?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Keep]: the opponent's attacks, Pokémon Powers and Trainer cards can't discard Energy cards from your Unown. No Asleep, Confused or Paralyzed clause.</summary>
        private sealed class KeepUnown : Mon
        {
            public override bool ProtectsEnergyFromOpponentsDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy)
            {
                return holder != null && holder.Owner == src.Player && holder.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        // ================================================================== Commons

        /// <summary>Grab: choose a Trainer card attached to 1 of the opponent's Pokémon; they shuffle it into their deck.</summary>
        private sealed class GrabMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grab") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var tools = new List<CardInstance>();
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m))) tools.AddRange(mon.Tools);
                if (tools.Count == 0) return true;
                var tool = ctx.ChooseCards(tools, 1, 1, "Grab: your opponent shuffles which Trainer card into their deck?")[0];
                var holder = ctx.Engine.ToolHolder(tool);
                if (holder == null || !holder.Tools.Remove(tool)) return true;
                ctx.State.Players[tool.Owner].Deck.Add(tool);
                ctx.Log(tool.Def.Name + " was shuffled into " + ctx.Engine.PlayerName(tool.Owner) + "'s deck (Grab).");
                ctx.Engine.ShuffleDeck(tool.Owner);
                return true;
            }
        }

        /// <summary>
        /// Negative Ion: during the opponent's next turn, damage the Defending Pokémon's attacks do to this Pokémon is 10 less, before
        /// Weakness and Resistance (benching or evolving either Pokémon ends it).
        /// </summary>
        private sealed class NegativeIonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Negative Ion") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                string name = "Negative Ion: " + defending.Name + "'s attacks do 10 less damage to " + me.Name + " during its next turn";
                if (!ctx.AddEffect(new NegativeIonAnchor(name, ctx.Player, defending.Uid, ctx.Turn + 1))) return true;
                ctx.AddEffect(new NegativeIonEffect(name, ctx.Player, me.Uid, ctx.Turn + 1, defending.Uid) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class NegativeIonAnchor : ContinuousEffect
        {
            public NegativeIonAnchor(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }
        }

        private sealed class NegativeIonEffect : ContinuousEffect
        {
            private readonly int attackerUid;

            public NegativeIonEffect(string name, int owner, int targetUid, int expiresAfterTurn, int attackerUid) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.attackerUid = attackerUid;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || target == null || attacker.Uid != attackerUid || target.Uid != TargetUid || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                foreach (var e in engine.State.Effects)
                    if (e is NegativeIonAnchor && e.Name == Name && e.TargetUid == attackerUid) return Math.Max(0, damage - 10);
                return damage;
            }
        }

        /// <summary>Flock Attack: a coin for each Murkrow on your Bench, 10 + 10 per heads.</summary>
        private sealed class FlockAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flock Attack") return false;
                int coins = ctx.Me.Bench.Count(m => m.Name == "Murkrow");
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Needle Storm: flip until tails, 20 per heads.</summary>
        private sealed class NeedleStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Needle Storm") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>
        /// Hard Shell (Pokémon Power): an attack (yours too) that does 40 or less damage to this Pokémon, after Weakness and Resistance,
        /// does 10 instead. Toxic Saliva: heads, Poisoned with 2 damage counters between turns (even if already Poisoned).
        /// </summary>
        private sealed class HardShellMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || damage <= 10 || damage > 40 || !PowerWorks(me)) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !info.ApplyOnce("Hard Shell")) return damage;
                src.Log("Hard Shell: " + me.Name + " takes 10 damage.");
                return 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toxic Saliva") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned) || defending.Has(SpecialCondition.Poisoned)) defending.PoisonCounterBase = Math.Max(defending.PoisonCounterBase, 2);
                return true;
            }
        }

        /// <summary>Magma Ring: the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class MagmaRingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Ring") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Psykiss: heads, choose a Special Energy card attached to 1 of the opponent's Pokémon; they shuffle it into their deck.</summary>
        private sealed class PsykissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psykiss") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var special = new List<CardInstance>();
                foreach (var mon in ctx.OpponentPokemon(m => ctx.CanAffect(m))) special.AddRange(mon.Energy.Where(e => e.Def.IsSpecialEnergy));
                if (special.Count == 0) return true;
                var card = ctx.ChooseCards(special, 1, 1, "Psykiss: your opponent shuffles which Special Energy card into their deck?")[0];
                var holder = ctx.OpponentPokemon(m => m.Energy.Contains(card)).FirstOrDefault();
                if (holder != null) ctx.Engine.ShuffleEnergyIntoDeck(holder, new List<CardInstance> { card });
                return true;
            }
        }

        /// <summary>Raging Headbutt: heads, 10 + 10 for each damage counter on this Pokémon; tails, 10.</summary>
        private sealed class RagingHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Headbutt") return false;
                int counters = ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters;
                ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 10 * counters : 0));
                return true;
            }
        }

        // ================================================================== Shining Gyarados

        /// <summary>
        /// Outrage: heads, 30 + 10 per damage counter on it; tails, nothing. Devastate: discard 2 Fire Energy cards or nothing; 10 to each
        /// of the opponent's Benched Pokémon; then heads, discard 1 Energy card from each of the opponent's Pokémon that has any.
        /// </summary>
        private sealed class ShiningGyaradosN3 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Outrage")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage + 10 * (me == null ? 0 : me.DamageCounters));
                    return true;
                }
                if (atk.Name == "Devastate")
                {
                    if (!PayDiscard(ctx, EnergyType.Fire, 2, atk.Name)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    foreach (var benched in ctx.OpponentBench())
                        if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                    if (!ctx.Flip()) return true;
                    foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0))
                        if (ctx.InPlay(mon) && ctx.CanAffect(mon))
                            ctx.DiscardEnergyFrom(mon, 1, "Devastate: discard which Energy card from " + mon.Name + "?");
                    return true;
                }
                return false;
            }
        }
    }
}
