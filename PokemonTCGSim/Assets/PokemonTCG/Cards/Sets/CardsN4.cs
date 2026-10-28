using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Destiny Pokémon (N4 1-15, 17-91, 107-113; v142). Unlimited only. Shining Celebi 106 is the 30th Celebration Classic
    /// Collection card. Unown [G], [H], [W], [X], [C], [P], [Q], [Z], [L], [S], [T] and [V] follow the Unown deck rule; the Shining
    /// Pokémon are 1 per deck (CardDef.OnePerDeck). Your rulings: Explosive Evolution and Sunbathe search only on heads; Reflect
    /// Shield's 20 damage gets Weakness and Resistance; Beatdown counts Dark Pokémon (names starting with "Dark ") and Darkness
    /// Pokémon; Dark Fire takes basic or Special Darkness Energy and always does 60 when one is discarded; Call Back is stopped only
    /// by the opponent's full Bench; Hot Plate hits any Pokémon going from a hand onto its owner's Bench, played or put there by an
    /// effect (GameEngine.PutOntoBench). Pokémon whose attacks only do their printed damage: Growlithe 68, Machop 73, Mareep 75,
    /// Pineco 77, Porygon 78, Slugma 82.
    /// </summary>
    public static class CardsN4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N4-1", new DarkAmpharosN4());
            r.RegisterByKey("N4-2", new DarkCrobatN4());
            r.RegisterByKey("N4-3", new TuskTossMon());
            r.RegisterByKey("N4-4", new ComboMon(new FlipBonusMon("Gouge", 10), new PsysplashMon()));
            r.RegisterByKey("N4-5", new DarkFeraligatrN4());
            r.RegisterByKey("N4-6", new DarkGengarN4());
            r.RegisterByKey("N4-7", new DarkHoundoomN4());
            r.RegisterByKey("N4-8", new DarkPorygon2N4());
            r.RegisterByKey("N4-9", new ThreatenMon());
            r.RegisterByKey("N4-10", new RushingFlamesMon());
            r.RegisterByKey("N4-11", new DarkTyranitarN4());
            r.RegisterByKey("N4-12", new LightArcanineN4());
            r.RegisterByKey("N4-13", new ComboMon(new ParalyzeFlipMon("Bubble"), new BubbleJumpMon()));
            r.RegisterByKey("N4-14", new LightDragoniteN4());
            r.RegisterByKey("N4-15", new LightTogeticN4());
            r.RegisterByKey("N4-17", new ComboMon(new EntangleMon(), new PoisonBindMon()));
            r.RegisterByKey("N4-18", new DarkMagcargoN4());
            r.RegisterByKey("N4-19", new ComboMon(new PrehistoricWaterMon(), new DarkTentacleMon()));
            r.RegisterByKey("N4-20", new DarkSlowkingN4());
            r.RegisterByKey("N4-21", new ComboMon(new ProvokeMon(), new BattleFrenzyMon()));
            r.RegisterByKey("N4-22", new ComboMon(new HealingLightMon(), new DodgeMon("Protective Wave", false)));
            r.RegisterByKey("N4-23", new ComboMon(new SearchlightMon(), new SparkMon()));
            r.RegisterByKey("N4-24", new ComboMon(new FlashTouchMon(), new CoinsMon("Comet Punch", 4)));
            r.RegisterByKey("N4-25", new LightMachampN4());
            r.RegisterByKey("N4-26", new LightPiloswineN4());
            r.RegisterByKey("N4-27", new GiveUnown());
            r.RegisterByKey("N4-28", new HelpUnown());
            r.RegisterByKey("N4-29", new WantUnown());
            r.RegisterByKey("N4-30", new HiddenPowerUnown());
            r.RegisterByKey("N4-31", new ComboMon(new CoinsMon("Doubleslap", 2), new EggTossMon()));
            r.RegisterByKey("N4-32", new ClampingJawMon());
            r.RegisterByKey("N4-33", new ComboMon(new CoinsMon("Triple Headbutt", 3), new MaxBurstMon()));
            r.RegisterByKey("N4-34", new ComboMon(new HighVoltageMon(), new StunWaveMon()));
            r.RegisterByKey("N4-35", new DarkForretressN4());
            r.RegisterByKey("N4-36", new DarkHaunterN4());
            r.RegisterByKey("N4-37", new WaterCannonMon());
            r.RegisterByKey("N4-38", new DarkPupitarN4());
            r.RegisterByKey("N4-39", new ComboMon(new IncinerateMon(), new RushingMagmaMon()));
            r.RegisterByKey("N4-40", new ComboMon(new CardsSI.HeadsTailsConditionMon("Dark Song", SpecialCondition.Asleep, SpecialCondition.Confused), new SlapAwakeMon()));
            r.RegisterByKey("N4-41", new FlipBonusMon("Charging Horn", 30));
            r.RegisterByKey("N4-42", new ComboMon(new CoinsMon("One-Two Kick", 2), new TailsNothingMon("Heel Drop")));
            r.RegisterByKey("N4-43", new ComboMon(new CornerMon(), new TailsNothingMon("Lunge")));
            r.RegisterByKey("N4-44", new ExpandMon());
            r.RegisterByKey("N4-45", new ComboMon(new CardsSI.HeadsTailsConditionMon("Freezing Breath", SpecialCondition.Paralyzed, SpecialCondition.Asleep), new IcePillarMon()));
            r.RegisterByKey("N4-46", new ComboMon(new WarmUpMon(), new BurningFlameMon()));
            r.RegisterByKey("N4-47", new ComboMon(new FlipperStrokeMon(), new CoreBlastMon()));
            r.RegisterByKey("N4-48", new ComboMon(new PulseGuardMon(), new ThunderNeedleMon()));
            r.RegisterByKey("N4-49", new ComboMon(new ReturnHomeMon(), new GentleKickMon()));
            r.RegisterByKey("N4-50", new CardsBS1.DiscardCostMon("Fire Blast", EnergyType.Fire, 1, null, new GuidingFlameMon()));
            r.RegisterByKey("N4-51", new ComboMon(new FishOutMon(), new SplashAboutMon()));
            r.RegisterByKey("N4-52", new ComboMon(new WashAwayMon(), new HypnoshowerMon()));
            r.RegisterByKey("N4-53", new ComboMon(new MysteriousWingMon(), new SynchronizeMon()));
            r.RegisterByKey("N4-54", new ComboMon(new EvolutionSongMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("N4-55", new DodgeMon("Agility", false));
            r.RegisterByKey("N4-56", new ComboMon(new CharmMon(), new RecoilMon("Spike Ball Tackle", 10)));
            r.RegisterByKey("N4-57", new ChaseUnown());
            r.RegisterByKey("N4-58", new PerformUnown());
            r.RegisterByKey("N4-59", new QuickenUnown());
            r.RegisterByKey("N4-60", new ZoomUnown());
            r.RegisterByKey("N4-61", new CardsUPR3.SmokescreenMon("Smokescreen", 1));
            r.RegisterByKey("N4-62", new ComboMon(new InkBlastMon(), new TentacleWrapMon()));
            r.RegisterByKey("N4-63", new ComboMon(new ShedMon(), new CoinsMon("Fury Attack", 2)));
            r.RegisterByKey("N4-64", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new ParalyzeFlipMon("Psyshock")));
            r.RegisterByKey("N4-65", new ConditionAttackMon("Nightmare", SpecialCondition.Asleep));
            r.RegisterByKey("N4-66", new ComboMon(new FlipBonusMon("Tail Bite", 20), new CardsN2.PsychicPlusMon()));
            r.RegisterByKey("N4-67", new StunPoisonMon());
            r.RegisterByKey("N4-68", new PlainMon());
            r.RegisterByKey("N4-69", new FlipDodgeMon());
            r.RegisterByKey("N4-70", new LeerMon());
            r.RegisterByKey("N4-71", new SwiftMon());
            r.RegisterByKey("N4-72", new ReflectedSunlightMon());
            r.RegisterByKey("N4-73", new PlainMon());
            r.RegisterByKey("N4-74", new LockAfterAttackMon("Giant Wave", false));
            r.RegisterByKey("N4-75", new PlainMon());
            r.RegisterByKey("N4-76", new FlipBonusMon("Crushing Step", 10));
            r.RegisterByKey("N4-77", new PlainMon());
            r.RegisterByKey("N4-78", new PlainMon());
            r.RegisterByKey("N4-79", new MigraineMon());
            r.RegisterByKey("N4-80", new CoinsMon("Fury Strikes", 3));
            r.RegisterByKey("N4-81", new RecoilMon("Take Down", 10));
            r.RegisterByKey("N4-82", new PlainMon());
            r.RegisterByKey("N4-83", new SunbatheMon());
            r.RegisterByKey("N4-84", new ParalyzeFlipMon("Generate Cold"));
            r.RegisterByKey("N4-85", new CardsBS1.ExtraWaterMon("Water Gun"));
            r.RegisterByKey("N4-86", new LaughUnown());
            r.RegisterByKey("N4-87", new SearchUnown());
            r.RegisterByKey("N4-88", new TellUnown());
            r.RegisterByKey("N4-89", new VanishUnown());
            r.RegisterByKey("N4-90", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("N4-91", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("N4-107", new ShiningCharizardN4());
            r.RegisterByKey("N4-108", new ShiningKabutopsN4());
            r.RegisterByKey("N4-109", new ShiningMewtwoN4());
            r.RegisterByKey("N4-110", new FlashingEyesMon());
            r.RegisterByKey("N4-111", new ThundersquallMon());
            r.RegisterByKey("N4-112", new CrushingEarthMon());
            r.RegisterByKey("N4-113", new ShiningTyranitarN4());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        /// <summary>The Energy card provides this type right now (basic, or a Special Energy like Miracle Energy that counts as it).</summary>
        private static bool Provides(EffectContext ctx, PokemonInPlay mon, CardInstance card, EnergyType type)
        {
            return CardsDRI1.ProvidesType(ctx, mon, card, type);
        }

        private static List<CardInstance> EnergyCardsOf(EffectContext ctx, PokemonInPlay mon, EnergyType type)
        {
            return mon == null ? new List<CardInstance>() : mon.Energy.Where(e => Provides(ctx, mon, e, type)).ToList();
        }

        /// <summary>"Discard N [type] Energy cards attached to this Pokémon or this attack does nothing." False when it can't be paid.</summary>
        private static bool PayDiscard(EffectContext ctx, EnergyType type, int count, string attack)
        {
            if (CardsBS1.CopyingWithMetronome || count <= 0) return true;
            var me = ctx.Pokemon;
            var cards = EnergyCardsOf(ctx, me, type);
            if (cards.Count < count) return false;
            var picks = cards.Count == count ? cards : ctx.ChooseCards(cards, count, count, attack + ": discard " + count + " " + type + " Energy card" + (count == 1 ? "" : "s"));
            int paid = 0;
            foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) paid++;
            return paid == count;
        }

        /// <summary>A context for the opponent's own choices during this effect ("your opponent may ...; either way, you may do the same").</summary>
        private static EffectContext Them(EffectContext ctx) { return new EffectContext(ctx.Engine, ctx.Opponent, ctx.Card, ctx.Pokemon); }

        private static bool IsBabyOrBasic(CardDef d) { return d != null && d.IsPokemon && (d.IsBasicPokemon || d.IsBaby); }

        private static bool IsUnown(PokemonInPlay mon) { return mon != null && mon.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0; }

        private static bool IsDarkPokemon(EffectContext ctx, PokemonInPlay mon)
        {
            return mon != null && (mon.Name.StartsWith("Dark ", StringComparison.Ordinal) || ctx.Engine.IsType(mon, EnergyType.Darkness));
        }

        // ================================================================== Holo and rare Pokémon

        /// <summary>
        /// Conductivity (Pokémon Power): whenever your opponent attaches an Energy card from their hand to a Pokémon, 10 damage to
        /// that Pokémon (no Weakness or Resistance); off with more than 1 Dark Ampharos in play, or Asleep, Confused or Paralyzed.
        /// Shock Bolt: discard all Lightning Energy cards attached to this Pokémon or the attack does nothing.
        /// </summary>
        private sealed class DarkAmpharosN4 : Mon
        {
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target == null || !PowerWorks(me) || src.MyPokemon(m => m.Name == "Dark Ampharos").Count > 1) return;
                if (src.State.FindPokemon(target.Uid) != target) return;
                Announce(src, "Conductivity");
                src.Engine.DealDamage(new DamageInfo { Target = target, ApplyWeakness = false, ApplyResistance = false }, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shock Bolt") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var lightning = EnergyCardsOf(ctx, me, EnergyType.Lightning);
                    if (lightning.Count == 0) return Nothing(ctx);
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Surprise Bite (Pokémon Power): when you play this Pokémon from your hand, you may do 20 damage to 1 of your opponent's
        /// Pokémon (no Weakness or Resistance). Dark Drain: a coin for each of the opponent's Pokémon, 10 to each with heads (no W/R);
        /// remove as many damage counters from this Pokémon as damage was done.
        /// </summary>
        private sealed class DarkCrobatN4 : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = ctx.OpponentPokemon();
                if (ctx.Pokemon == null || targets.Count == 0 || !ctx.AskYesNo("Surprise Bite: do 20 damage to 1 of your opponent's Pokémon?")) return;
                Announce(ctx, "Surprise Bite");
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Surprise Bite: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.Engine.DealDamage(new DamageInfo { Target = target, ApplyWeakness = false, ApplyResistance = false }, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Drain") return false;
                var hits = new List<PokemonInPlay>();
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.Engine.FlipCoin(ctx.Player, "Dark Drain (" + mon.Name + ")")) hits.Add(mon);
                int done = 0;
                foreach (var mon in hits)
                    if (ctx.InPlay(mon)) done += ctx.DamageWith(mon, 10, false, false, false);
                var me = ctx.Pokemon;
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, done);
                return true;
            }
        }

        /// <summary>Tusk Toss: with any Benched Pokémon on the other side, flip; heads, the Defending Pokémon and its cards go back to the hand; tails, they switch it.</summary>
        private sealed class TuskTossMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tusk Toss") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (ctx.Opp.Bench.Count == 0 || defending == null) return true;
                if (ctx.Flip())
                {
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                }
                else ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Psysplash: 10 damage to each of the opponent's Pokémon for each Energy card attached to it (no Weakness or Resistance).</summary>
        private sealed class PsysplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psysplash") return false;
                foreach (var mon in ctx.OpponentPokemon())
                {
                    if (!ctx.InPlay(mon) || mon.Energy.Count == 0) continue;
                    ctx.DamageWith(mon, 10 * mon.Energy.Count, false, false, false);
                }
                return true;
            }
        }

        /// <summary>
        /// Scare (Pokémon Power): while this Pokémon is your Active Pokémon, the opponent's Baby Pokémon's Pokémon Powers stop working
        /// and their Baby Pokémon can't attack (not while Asleep, Confused or Paralyzed). Crushing Blow: with Energy on the Defending
        /// Pokémon, flip; heads, discard 1 of them.
        /// </summary>
        private sealed class DarkFeraligatrN4 : Mon
        {
            private static bool Scares(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (mon == null || me == null || mon.Owner == src.Player || !mon.Def.IsBaby) return false;
                return src.IsActive(me) && PowerWorks(me) && !src.Engine.AbilityBlocked(me, "Scare");
            }

            public override bool StopsPokemonPowersOf(EffectContext src, PokemonInPlay mon) { return Scares(src, mon); }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon) { return Scares(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Blow") return false;
                var defending = ctx.Defending;
                bool hasEnergy = defending != null && defending.Energy.Count > 0;
                ctx.DamageDefending(atk.Damage);
                if (!hasEnergy || !ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                ctx.DiscardEnergyFrom(defending, 1, "Crushing Blow: discard which Energy card from " + defending.Name + "?");
                return true;
            }
        }

        /// <summary>
        /// Deep Sleep (Pokémon Power): while it works, each player flips 2 coins for each of their Asleep Pokémon between turns; it
        /// stays Asleep if either is tails. Pull In: you may switch in 1 of the opponent's Benched Pokémon first; the Defending
        /// Pokémon is now Asleep.
        /// </summary>
        private sealed class DarkGengarN4 : Mon
        {
            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins)
            {
                var me = src.Pokemon;
                return me != null && PowerWorks(me) ? Math.Max(coins, 2) : coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull In") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Pull In: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent("Pull In: choose 1 of your opponent's Benched Pokémon to switch in");
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Eerie Howl: if the opponent's Bench isn't full, look at their hand; you may put a Baby or Basic Pokémon from it onto their
        /// Bench, then switch it with the Defending Pokémon. Dark Fire: discard a Darkness Energy card (basic or Special) for 60
        /// damage, otherwise 30 (your ruling).
        /// </summary>
        private sealed class DarkHoundoomN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Eerie Howl")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                    var hand = ctx.Opp.Hand.ToList();
                    ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Eerie Howl: your opponent's hand", ctx.Source);
                    var basics = hand.Where(c => IsBabyOrBasic(c.Def)).ToList();
                    if (basics.Count == 0) return true;
                    var pick = ctx.ChooseCards(basics, 1, 1, "Eerie Howl: put which Pokémon onto your opponent's Bench?");
                    if (pick.Count == 0 || !ctx.Opp.Hand.Remove(pick[0])) return true;
                    var benched = ctx.Engine.PutOntoBench(ctx.Opponent, pick[0], false, true);
                    if (benched != null && ctx.Opp.Bench.Contains(benched) && ctx.Opp.Active != null) ctx.Engine.SwitchActive(ctx.Opponent, benched);
                    return true;
                }
                if (atk.Name == "Dark Fire")
                {
                    var me = ctx.Pokemon;
                    var dark = me == null ? new List<CardInstance>() : me.Energy.Where(e => e.Def.IsEnergy && e.Def.Name == "Darkness Energy").ToList();
                    if (dark.Count == 0 && !CardsBS1.CopyingWithMetronome)
                    {
                        ctx.DamageDefending(atk.Damage);
                        return true;
                    }
                    if (dark.Count > 0)
                    {
                        var card = dark.Count == 1 ? dark[0] : ctx.ChooseCards(dark, 1, 1, "Dark Fire: discard which Darkness Energy card?")[0];
                        ctx.Engine.DiscardEnergy(me, card);
                    }
                    ctx.DamageDefending(atk.Damage + 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Spatial Distortion (Pokémon Power): once during your turn, flip; heads, put a Stadium card from your discard pile into play
        /// (the one in play is discarded). Curve Attack: heads, prevent all damage done to this Pokémon by attacks next turn.
        /// </summary>
        private sealed class DarkPorygon2N4 : Mon
        {
            private const string Power = "Spatial Distortion";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Discard.Any(c => c.Def.IsStadiumAlone); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var stadiums = ctx.Me.Discard.Where(c => c.Def.IsStadiumAlone).ToList();
                if (stadiums.Count == 0) return;
                var pick = stadiums.Count == 1 ? stadiums[0] : ctx.ChooseCards(stadiums, 1, 1, Power + ": put which Stadium card into play?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                ctx.Engine.PutStadiumIntoPlay(ctx.Player, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curve Attack") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + ": attacks do no damage to it during the opponent's next turn (Curve Attack)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Threaten: heads, look at the opponent's hand; they shuffle a Trainer card you choose from it into their deck.</summary>
        private sealed class ThreatenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Threaten") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Opp.Hand.Count == 0) return true;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Threaten: your opponent's hand", ctx.Source);
                var trainers = hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Threaten: your opponent shuffles which Trainer card into their deck?");
                if (pick.Count == 0 || !ctx.Opp.Hand.Remove(pick[0])) return true;
                ctx.Opp.Deck.Add(pick[0]);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + pick[0].Def.Name + " into their deck (Threaten).");
                return true;
            }
        }

        /// <summary>Rushing Flames: discard any number of Fire Energy cards from your Pokémon; a coin for each, 40 for each heads.</summary>
        private sealed class RushingFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rushing Flames") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var card in EnergyCardsOf(ctx, mon, EnergyType.Fire)) holders[card] = mon;
                int discarded = 0;
                if (holders.Count > 0)
                {
                    var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Rushing Flames: discard any number of Fire Energy cards attached to your Pokémon");
                    foreach (var card in picks)
                        if (ctx.Engine.DiscardEnergy(holders[card], card)) discarded++;
                }
                int heads = discarded > 0 ? ctx.FlipCoins(discarded) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>
        /// Mountain Smasher: a coin for each Fighting Energy card attached, 20 per heads, and the top card of the opponent's deck is
        /// discarded for each heads. Fling Away: with a Benched Pokémon on the other side, 30 to the Defending Pokémon and 30 to 1 of them.
        /// </summary>
        private sealed class DarkTyranitarN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mountain Smasher")
                {
                    int coins = EnergyCardsOf(ctx, ctx.Pokemon, EnergyType.Fighting).Count;
                    int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                    ctx.DamageDefending(atk.Damage * heads);
                    if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                    return true;
                }
                if (atk.Name == "Fling Away")
                {
                    if (ctx.Opp.Bench.Count == 0)
                    {
                        ctx.DamageDefending(atk.Damage);
                        return true;
                    }
                    ctx.DamageDefending(30);
                    ctx.DamageOneBenched(30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Drive Off (Pokémon Power): while Active, once during your turn, if the opponent has Benched Pokémon they switch 1 of them
        /// with the Defending Pokémon (they choose). Gentle Flames: 10 to a Baby Pokémon, 30 to a Basic Pokémon, otherwise 50.
        /// </summary>
        private sealed class LightArcanineN4 : Mon
        {
            protected override string Activated { get { return "Drive Off"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && PowerWorks(ctx.Pokemon) && ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null;
            }

            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gentle Flames") return false;
                var defending = ctx.Defending;
                int damage = atk.Damage;
                if (defending != null && defending.Def.IsBaby) damage = 10;
                else if (defending != null && defending.Def.IsBasicPokemon) damage = 30;
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Bubble Jump: with Benched Pokémon, flip; heads, 2 Energy cards from this Pokémon go to 1 of them, then this Pokémon and its cards return to your hand.</summary>
        private sealed class BubbleJumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Jump") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0 || !ctx.Flip()) return true;
                if (me.Energy.Count > 0)
                {
                    var bench = ctx.MyBench();
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Bubble Jump: move 2 Energy cards to which Benched Pokémon?");
                    var energy = me.Energy.ToList();
                    var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Bubble Jump: move which 2 Energy cards?");
                    if (target != null) foreach (var card in picks) ctx.Engine.MoveEnergy(me, target, card);
                }
                if (ctx.InPlay(me)) ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>
        /// Miraculous Wind (Pokémon Power): while this Pokémon is your Active Pokémon, every Special Energy card provides Colorless
        /// Energy and has no other effect (not while Asleep, Confused or Paralyzed). Light Wave: prevent all effects of attacks,
        /// other than damage, done to this Pokémon during the opponent's next turn.
        /// </summary>
        private sealed class LightDragoniteN4 : Mon
        {
            public override bool DisablesSpecialEnergy(EffectContext src)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && PowerWorks(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light Wave") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new NoAttackEffectsEffect(me.Name + ": no effects of attacks other than damage during the opponent's next turn (Light Wave)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"Prevent all effects of attacks, except damage, done to this Pokémon" (Light Wave).</summary>
        private sealed class NoAttackEffectsEffect : ContinuousEffect
        {
            public NoAttackEffectsEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target == null || target.Uid != TargetUid || kind != EffectSourceKind.Attack) return false;
                var attacker = engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != Owner;
            }
        }

        /// <summary>
        /// Gift (Pokémon Power): when you play this Pokémon from your hand, your opponent may search their deck for a Pokémon Tool
        /// card, show it and put it into their hand; either way you may do the same. Sweet Kiss: your opponent may draw a card.
        /// </summary>
        private sealed class LightTogeticN4 : Mon
        {
            private static bool SearchTool(EffectContext who)
            {
                if (who.Me.Deck.Count == 0 || !who.AskYesNo("Gift: search your deck for a Pokémon Tool card?")) return false;
                var found = who.SearchMyDeck(d => d.IsTool, 1, "Gift: choose a Pokémon Tool card to put into your hand");
                who.Me.Hand.AddRange(found);
                if (found.Count > 0) who.Engine.ShowCards(who.Opponent, found, "Gift", "Gift: " + who.Engine.PlayerName(who.Player) + " took this card", who.Source);
                return true;
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null) return;
                Announce(ctx, "Gift");
                bool theySearched = SearchTool(Them(ctx));
                bool iSearched = SearchTool(ctx);
                if (theySearched) ctx.Engine.ShuffleDeck(ctx.Opponent);
                if (iSearched) ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Kiss") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Sweet Kiss: draw a card?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 1);
                return true;
            }
        }

        // ================================================================== Rare and uncommon Pokémon

        /// <summary>Entangle: with Benched Pokémon on the other side, switch 1 of them in (you choose), then flip; heads, it's Paralyzed.</summary>
        private sealed class EntangleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entangle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count == 0) return true;
                ctx.GustOpponent("Entangle: choose 1 of your opponent's Benched Pokémon to switch in");
                var defending = ctx.Defending;
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Poison Bind: the Defending Pokémon is now Poisoned; heads, until the end of the opponent's next turn it can't retreat and
        /// effects of attacks, Pokémon Powers and Trainer cards can't change that player's Active Pokémon.
        /// </summary>
        private sealed class PoisonBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Bind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new BindEffect(defending.Name + " can't retreat or be switched out until the end of its next turn (Poison Bind)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class BindEffect : ContinuousEffect
        {
            public BindEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }

            public override bool LocksActiveSpot(GameEngine engine, int player)
            {
                var active = engine.State.Players[player].Active;
                return player != Owner && active != null && active.Uid == TargetUid;
            }
        }

        /// <summary>
        /// Hot Plate (Pokémon Power): while this Pokémon is your Active Pokémon, whenever a player puts a Pokémon onto their Bench from
        /// their hand (played, or by an effect: your ruling), 10 damage to it (no W/R). Ball of Flame: you may discard a Fire Energy
        /// card for 20 damage to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class DarkMagcargoN4 : Mon
        {
            public override void OnPokemonBenchedFromHand(EffectContext src, PokemonInPlay benched)
            {
                var me = src.Pokemon;
                if (me == null || benched == null || !src.IsActive(me) || !PowerWorks(me) || !IsBabyOrBasic(benched.Def)) return;
                Announce(src, "Hot Plate");
                src.Engine.DealDamage(new DamageInfo { Target = benched, ApplyWeakness = false, ApplyResistance = false }, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ball of Flame") return false;
                var me = ctx.Pokemon;
                bool snipe = false;
                var fire = EnergyCardsOf(ctx, me, EnergyType.Fire);
                if (fire.Count > 0 && ctx.Opp.Bench.Count > 0 && ctx.AskYesNo("Ball of Flame: discard a Fire Energy card to do 20 damage to 1 of your opponent's Benched Pokémon?"))
                {
                    var card = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Ball of Flame: discard which Fire Energy card?")[0];
                    snipe = ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage);
                if (snipe) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Prehistoric Water: choose 1 of the opponent's evolved Pokémon and flip; heads, they shuffle its highest Stage Evolution card into their deck.</summary>
        private sealed class PrehistoricWaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Prehistoric Water") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var evolved = ctx.OpponentPokemon(m => m.Stack.Count >= 2 && m.Def.IsEvolution);
                if (evolved.Count == 0) return true;
                var target = evolved.Count == 1 ? evolved[0] : ctx.ChoosePokemon(evolved, "Prehistoric Water: choose 1 of your opponent's evolved Pokémon");
                if (target == null || !ctx.Flip() || !ctx.InPlay(target) || !ctx.CanAffect(target)) return true;
                ctx.Engine.Devolve(target, 1, true);
                return true;
            }
        }

        /// <summary>Dark Tentacle: the Defending Pokémon can't evolve from the hand during the opponent's next turn (benching it ends this).</summary>
        private sealed class DarkTentacleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Tentacle") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new NoEvolveEffect(defending.Name + " can't evolve from the hand during its next turn (Dark Tentacle)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoEvolveEffect : ContinuousEffect
        {
            public NoEvolveEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>
        /// Cunning (Pokémon Power): once during your turn, flip; heads, look at the top card of the opponent's deck, then you may
        /// have them shuffle it. Mind Shock: no Weakness or Resistance.
        /// </summary>
        private sealed class DarkSlowkingN4 : Mon
        {
            private const string Power = "Cunning";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip() || ctx.Opp.Deck.Count == 0) return;
                var top = ctx.Opp.Deck.Take(1).ToList();
                ctx.Engine.ShowCards(ctx.Player, top, "Top of your opponent's deck", Power + ": the top card of your opponent's deck", ctx.Source);
                if (ctx.AskYesNo(Power + ": have your opponent shuffle their deck?")) ctx.Engine.ShuffleDeck(ctx.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Shock") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Provoke: look at the opponent's hand; put any number of Baby and Basic Pokémon from it onto their Bench; then they look at
        /// your hand and may do the same to you.
        /// </summary>
        private sealed class ProvokeMon : Mon
        {
            private static void Bench(EffectContext chooser, int owner)
            {
                var engine = chooser.Engine;
                var p = engine.State.Players[owner];
                var hand = p.Hand.ToList();
                engine.ShowCards(chooser.Player, hand, "Provoke", "Provoke: " + engine.PlayerName(owner) + "'s hand", chooser.Source);
                var basics = hand.Where(c => IsBabyOrBasic(c.Def)).ToList();
                int room = engine.BenchSpace(owner);
                if (basics.Count == 0 || room <= 0) return;
                var picks = engine.ChooseCards(chooser.Player, basics, 0, Math.Min(room, basics.Count), "Provoke: put any of these onto " + engine.PlayerName(owner) + "'s Bench", chooser.Source);
                foreach (var card in picks)
                {
                    if (engine.BenchSpace(owner) <= 0 || !p.Hand.Remove(card)) continue;
                    engine.PutOntoBench(owner, card, false, true);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Provoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Bench(ctx, ctx.Opponent);
                if (!ctx.State.IsOver) Bench(Them(ctx), ctx.Player);
                return true;
            }
        }

        /// <summary>Battle Frenzy: a coin for each Pokémon in play (both sides); 20 to each one with heads (no Weakness or Resistance).</summary>
        private sealed class BattleFrenzyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Battle Frenzy") return false;
                var hits = new List<PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                    if (ctx.Engine.FlipCoin(ctx.Player, "Battle Frenzy (" + mon.Name + ")")) hits.Add(mon);
                foreach (var mon in hits)
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Healing Light: remove 1 damage counter from each of your Pokémon that has any.</summary>
        private sealed class HealingLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Light") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>Searchlight: heads, each player may put a card from their discard pile into their hand (your opponent first).</summary>
        private sealed class SearchlightMon : Mon
        {
            private static void Take(EffectContext who)
            {
                var pile = who.Me.Discard.ToList();
                if (pile.Count == 0) return;
                var pick = who.ChooseCards(pile, 0, 1, "Searchlight: you may put a card from your discard pile into your hand");
                if (pick.Count > 0) who.DiscardToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searchlight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                Take(Them(ctx));
                Take(ctx);
                return true;
            }
        }

        /// <summary>Spark: 10 damage to 1 of the opponent's Benched Pokémon too.</summary>
        private sealed class SparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spark") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>
        /// Flash Touch: switch 1 of your Benched Pokémon with this Pokémon; as long as that Pokémon is your Active Pokémon, it can't
        /// become Asleep, Confused, Paralyzed or Poisoned.
        /// </summary>
        private sealed class FlashTouchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Touch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || !ctx.SwitchSelfWithBench()) return true;
                var active = ctx.Me.Active;
                if (active != null)
                    ctx.AddEffect(new FlashTouchEffect(active.Name + " can't become Asleep, Confused, Paralyzed or Poisoned while it's your Active Pokémon (Flash Touch)", ctx.Player, active.Uid));
                return true;
            }
        }

        private sealed class FlashTouchEffect : ContinuousEffect
        {
            public FlashTouchEffect(string name, int owner, int targetUid) : base(name, owner, targetUid, int.MaxValue, true) { }

            public override bool PreventsCondition(GameEngine engine, PokemonInPlay mon, SpecialCondition condition)
            {
                if (mon == null || mon.Uid != TargetUid || !engine.State.IsActive(mon)) return false;
                return condition == SpecialCondition.Asleep || condition == SpecialCondition.Confused || condition == SpecialCondition.Paralyzed || condition == SpecialCondition.Poisoned;
            }
        }

        /// <summary>
        /// Tag Team (Pokémon Power): when you play this Pokémon from your hand, if it's on your Bench, remove 3 damage counters from
        /// your Active Pokémon, then switch this Pokémon with it. Beatdown: against a Dark or Darkness Pokémon, heads 100 instead of 50.
        /// </summary>
        private sealed class LightMachampN4 : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var active = ctx.Me.Active;
                if (me == null || active == null || !ctx.Me.Bench.Contains(me)) return;
                Announce(ctx, "Tag Team");
                if (active.DamageCounters > 0) ctx.Heal(active, 30);
                if (ctx.Me.Bench.Contains(me)) ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Beatdown") return false;
                bool big = IsDarkPokemon(ctx, ctx.Defending) && ctx.Flip();
                ctx.DamageDefending(big ? 100 : atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Fluffy Wool (Pokémon Power): during the opponent's turn, if this Pokémon is your Active Pokémon and is damaged by their
        /// attack, flip; heads, the attacking Pokémon is now Asleep. It doesn't work if this Pokémon was already Asleep, Confused or
        /// Paralyzed when the attack started. Knock Over: you may discard the Stadium card in play.
        /// </summary>
        private sealed class LightPiloswineN4 : Mon
        {
            private const string Marker = "Fluffy Wool off";

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                var me = src.Pokemon;
                if (me == null) return;
                if (PowerWorks(me)) me.Markers.Remove(Marker);
                else me.Markers[Marker] = src.Turn;
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || attacker == null || !holderWasActive || damage <= 0 || attacker.Owner == ctx.Player || ctx.IsMyTurn) return;
                int turn;
                if (me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn) return;
                Announce(ctx, "Fluffy Wool");
                if (ctx.Flip() && ctx.InPlay(attacker)) ctx.Engine.ApplyCondition(attacker, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Knock Over: discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Chansey's Egg Toss: 2 coins; any tails, nothing.</summary>
        private sealed class EggTossMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Egg Toss") return false;
                if (ctx.FlipCoins(2) < 2) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Clamping Jaw: during the opponent's next turn the Defending Pokémon can't retreat, and it must flip a coin to attack (tails,
        /// nothing). Benching either Pokémon ends this.
        /// </summary>
        private sealed class ClampingJawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clamping Jaw") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new ClampingJawEffect(defending.Name + " can't retreat and flips to attack during its next turn while " + me.Name + " is Active (Clamping Jaw)",
                    ctx.Player, defending.Uid, ctx.Turn + 1, me.Uid));
                return true;
            }
        }

        private sealed class ClampingJawEffect : ContinuousEffect
        {
            private readonly int anchorUid;

            public ClampingJawEffect(string name, int owner, int targetUid, int expiresAfterTurn, int anchorUid) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.anchorUid = anchorUid;
            }

            private bool Holds(GameEngine engine)
            {
                var active = engine.State.Players[Owner].Active;
                return active != null && active.Uid == anchorUid;
            }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid && Holds(engine); }

            public override bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return attacker != null && attacker.Uid == TargetUid && Holds(engine); }
        }

        /// <summary>MAX Burst: a coin for each Energy card attached to the opponent's Pokémon; 20 for each heads.</summary>
        private sealed class MaxBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "MAX Burst") return false;
                int coins = ctx.OpponentPokemon().Sum(m => m.Energy.Count);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>High Voltage: heads, the opponent can't play Trainer cards during their next turn.</summary>
        private sealed class HighVoltageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Voltage") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new NoTrainersEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (High Voltage)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoTrainersEffect : ContinuousEffect
        {
            private readonly int affected;

            public NoTrainersEffect(string name, int owner, int affected, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.affected = affected;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && engine.State.CurrentPlayer == affected && card != null && card.Def.IsTrainer;
            }
        }

        /// <summary>Stun Wave: if the Defending Pokémon has a Pokémon Power, it stops working until the end of your next turn.</summary>
        private sealed class StunWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stun Wave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || !GameEngine.HasOldPokemonPower(defending.Def)) return true;
                ctx.AddEffect(new StunWaveEffect(defending.Name + "'s Pokémon Power stops working until the end of your next turn (Stun Wave)", ctx.Player, defending.Uid, ctx.Turn + 2));
                return true;
            }
        }

        private sealed class StunWaveEffect : ContinuousEffect
        {
            public StunWaveEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>
        /// Armor Up: until the end of your next turn, if this Pokémon would be Knocked Out by damage from an attack, flip; heads, it
        /// isn't and its remaining HP is 10 (its own Explosion included). Explosion: 60 damage to itself.
        /// </summary>
        private sealed class DarkForretressN4 : Mon
        {
            private static bool ArmoredUp(EffectContext ctx, PokemonInPlay me)
            {
                return me != null && ctx.State.Effects.Any(e => e is ArmorUpEffect && e.TargetUid == me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Armor Up")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me))
                        ctx.AddEffect(new ArmorUpEffect(me.Name + ": if damage from an attack would Knock it Out, flip; heads, it keeps 10 HP (Armor Up)", ctx.Player, me.Uid, ctx.Turn + 2));
                    return true;
                }
                if (atk.Name == "Explosion")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(60);
                    // The engine's own check covers only the opponent's attacks; Armor Up says "an attack", so this one too.
                    if (ctx.InPlay(me) && ctx.Engine.IsKnockedOut(me) && !me.ForcedKnockOut && ArmoredUp(ctx, me) && ctx.Engine.FlipCoin(ctx.Player, "Armor Up"))
                    {
                        me.DamageCounters = Math.Max(0, (ctx.Engine.MaxHp(me) - 10) / 10);
                        ctx.Log(me.Name + " wasn't Knocked Out (Armor Up).");
                    }
                    return true;
                }
                return false;
            }
        }

        private sealed class ArmorUpEffect : ContinuousEffect
        {
            public ArmorUpEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon)
            {
                return mon != null && mon.Uid == TargetUid && engine.FlipCoin(Owner, "Armor Up");
            }
        }

        /// <summary>
        /// Call Back: put a Baby or Basic Pokémon card from the opponent's discard pile onto their Bench with 1 damage counter (can't be
        /// used while their Bench is full: your ruling). Surround: heads, Asleep; tails, it can't retreat.
        /// </summary>
        private sealed class DarkHaunterN4 : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Call Back" || ctx.Engine.BenchSpace(ctx.Opponent) > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Call Back")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var cards = ctx.Opp.Discard.Where(c => IsBabyOrBasic(c.Def)).ToList();
                    if (cards.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                    var pick = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Call Back: put which Pokémon onto your opponent's Bench?")[0];
                    if (!ctx.Opp.Discard.Remove(pick)) return true;
                    var mon = ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
                    if (mon != null) ctx.PlaceCounters(mon, 1);
                    return true;
                }
                if (atk.Name == "Surround")
                {
                    var defending = ctx.Defending;
                    bool heads = ctx.Flip();
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    if (heads) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    else if (ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Water Cannon: 10 damage to 1 of the opponent's Pokémon for each Water Energy card on this Pokémon, 30 at most (no W/R).</summary>
        private sealed class WaterCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Cannon") return false;
                int damage = Math.Min(30, 10 * EnergyCardsOf(ctx, ctx.Pokemon, EnergyType.Water).Count);
                if (damage > 0) ctx.DamageAnyOpponent(damage, true);
                else ctx.Log("Water Cannon does no damage.");
                return true;
            }
        }

        /// <summary>
        /// Rock Tackle: 10 damage to itself; it can't use Rock Tackle during your next turn. Explosive Evolution: heads, 10 damage to
        /// each of the opponent's Pokémon (no W/R), then evolve this Pokémon into Dark Tyranitar from your deck (tails: nothing).
        /// </summary>
        private sealed class DarkPupitarN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Rock Tackle")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(10);
                    ctx.SelfCantUseAttackNextTurn(atk.Name);
                    return true;
                }
                if (atk.Name == "Explosive Evolution")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    foreach (var mon in ctx.OpponentPokemon())
                        if (ctx.InPlay(mon)) ctx.DamageWith(mon, 10, false, false, false);
                    if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                    if (CardsEVS1.EvolveBlocked(ctx, me))
                    {
                        ctx.Log(me.Name + " can't evolve right now (Explosive Evolution).");
                        return true;
                    }
                    var found = ctx.SearchMyDeck(d => d.Name == "Dark Tyranitar" && CardsEVS1.EvolvesOnto(ctx, me, d), 1, "Explosive Evolution: choose a Dark Tyranitar", false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Incinerate: show the top card of the opponent's deck; a Trainer card is discarded.</summary>
        private sealed class IncinerateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Incinerate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                var top = ctx.Opp.Deck.Take(1).ToList();
                ctx.Engine.ShowCards(ctx.Player, top, "Incinerate", "Incinerate: the top card of your opponent's deck", ctx.Source);
                ctx.Engine.ShowCards(ctx.Opponent, top, "Incinerate", "Incinerate: the top card of your deck", ctx.Source);
                if (top[0].Def.IsTrainer) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                else ctx.Log(top[0].Def.Name + " isn't a Trainer card (Incinerate).");
                return true;
            }
        }

        /// <summary>Rushing Magma: discard the top 5 cards of your deck; 20 damage for each Fire Energy card among them.</summary>
        private sealed class RushingMagmaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rushing Magma") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int fire = discarded.Count(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire);
                ctx.DamageDefending(atk.Damage * fire);
                return true;
            }
        }

        /// <summary>Slap Awake: 40 if the Defending Pokémon is Asleep or Confused, otherwise 20; then it's no longer Asleep or Confused.</summary>
        private sealed class SlapAwakeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slap Awake") return false;
                var defending = ctx.Defending;
                bool more = defending != null && (defending.Has(SpecialCondition.Asleep) || defending.Has(SpecialCondition.Confused));
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                if (defending.Has(SpecialCondition.Asleep)) ctx.Engine.RemoveCondition(defending, SpecialCondition.Asleep);
                if (defending.Has(SpecialCondition.Confused)) ctx.Engine.RemoveCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Common Pokémon

        /// <summary>Corner: the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class CornerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Corner") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Expand: during the opponent's next turn, damage done to this Pokémon is reduced by 10 (after W/R).</summary>
        private sealed class ExpandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Expand") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(10);
                return true;
            }
        }

        /// <summary>Ice Pillar: until the end of the opponent's next turn, while this Pokémon is your Active Pokémon, attacks do no damage to your Benched Pokémon.</summary>
        private sealed class IcePillarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Pillar") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new IcePillarEffect("Attacks do no damage to " + ctx.Engine.Possessive(ctx.Player) + " Benched Pokémon while " + me.Name + " is Active (Ice Pillar)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class IcePillarEffect : ContinuousEffect
        {
            public IcePillarEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Attacker == null || info.Target.Owner != Owner || engine.State.IsActive(info.Target)) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var active = engine.State.Players[Owner].Active;
                return active != null && active.Uid == TargetUid ? 0 : damage;
            }
        }

        /// <summary>Warm Up: with Benched Pokémon, attach a Fire Energy card from your deck to 1 of them.</summary>
        private sealed class WarmUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Warm Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fire, 1, "Warm Up: choose a Fire Energy card");
                foreach (var card in found)
                {
                    var bench = ctx.MyBench();
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Warm Up: attach " + card.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Burning Flame: 2 coins; discard a Fire Energy card for each heads or the attack does nothing; 30 + 20 per heads.</summary>
        private sealed class BurningFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Flame") return false;
                int heads = ctx.FlipCoins(2);
                if (!PayDiscard(ctx, EnergyType.Fire, heads, atk.Name)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>
        /// Flipper Stroke: your opponent looks at the top 3 cards of their deck and may put the basic Energy cards among them into
        /// their hand (shown to you); then you do the same; each player shuffles.
        /// </summary>
        private sealed class FlipperStrokeMon : Mon
        {
            private static void Look(EffectContext who)
            {
                var deck = who.Me.Deck;
                if (deck.Count == 0) return;
                var top = deck.Take(3).ToList();
                who.Engine.ShowCards(who.Player, top, "Flipper Stroke", "Flipper Stroke: the top cards of your deck", who.Source);
                var basics = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0)
                {
                    var picks = who.ChooseCards(basics, 0, basics.Count, "Flipper Stroke: put any of these basic Energy cards into your hand");
                    foreach (var card in picks)
                        if (deck.Remove(card)) who.Me.Hand.Add(card);
                    if (picks.Count > 0) who.Engine.ShowCards(who.Opponent, picks, "Flipper Stroke", "Flipper Stroke: " + who.Engine.PlayerName(who.Player) + " took these cards", who.Source);
                }
                who.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flipper Stroke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Look(Them(ctx));
                Look(ctx);
                return true;
            }
        }

        /// <summary>Core Blast: 30 + 20 for each Special Energy card attached to the Defending Pokémon.</summary>
        private sealed class CoreBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Blast") return false;
                var defending = ctx.Defending;
                int special = defending == null ? 0 : defending.Energy.Count(e => e.Def.IsSpecialEnergy);
                ctx.DamageDefending(atk.Damage + 20 * special);
                return true;
            }
        }

        /// <summary>Pulse Guard: during the opponent's next turn, damage of 30 or more done to this Pokémon (after W/R) is prevented.</summary>
        private sealed class PulseGuardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pulse Guard") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new PulseGuardEffect(me.Name + ": damage of 30 or more is prevented during the opponent's next turn (Pulse Guard)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class PulseGuardEffect : ContinuousEffect
        {
            public PulseGuardEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.AgainstOpponent && damage >= 30;
            }
        }

        /// <summary>Thunder Needle: 3 coins, 20 per heads; 2 or more heads, Paralyzed.</summary>
        private sealed class ThunderNeedleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Needle") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Return Home: shuffle 1 of your Benched Pokémon and its cards into your deck.</summary>
        private sealed class ReturnHomeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return Home") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Return Home: shuffle which Benched Pokémon into your deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Gentle Kick: 40, or 20 if the Defending Pokémon has damage counters.</summary>
        private sealed class GentleKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gentle Kick") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(defending != null && defending.DamageCounters > 0 ? 20 : atk.Damage);
                return true;
            }
        }

        /// <summary>Guiding Flame: put a Baby or Basic Pokémon card from your discard pile onto your Bench (not with a full Bench).</summary>
        private sealed class GuidingFlameMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Guiding Flame" || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Guiding Flame") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Discard.Where(c => IsBabyOrBasic(c.Def)).ToList();
                if (cards.Count == 0 || ctx.BenchSpace <= 0) return true;
                var pick = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Guiding Flame: put which Pokémon onto your Bench?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                return true;
            }
        }

        /// <summary>Fish Out: your opponent may shuffle up to 3 Pokémon cards from their discard pile into their deck; then you may do the same.</summary>
        private sealed class FishOutMon : Mon
        {
            private static void Fish(EffectContext who)
            {
                var cards = who.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (cards.Count == 0) return;
                var picks = who.ChooseCards(cards, 0, Math.Min(3, cards.Count), "Fish Out: shuffle up to 3 Pokémon cards from your discard pile into your deck");
                if (picks.Count == 0) return;
                who.DiscardToDeck(picks);
                who.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fish Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Fish(Them(ctx));
                Fish(ctx);
                return true;
            }
        }

        /// <summary>Splash About: 40 if the Defending Pokémon has more Energy attached than this Pokémon, otherwise 20.</summary>
        private sealed class SplashAboutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splash About") return false;
                var defending = ctx.Defending;
                bool more = defending != null && ctx.Pokemon != null && ctx.EnergyCount(defending) > ctx.EnergyCount(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                return true;
            }
        }

        /// <summary>Wash Away: with Benched Pokémon, flip; heads, remove all damage counters from 1 of them and discard all its Energy cards.</summary>
        private sealed class WashAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wash Away") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || !ctx.Flip()) return true;
                var bench = ctx.MyBench();
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Wash Away: choose 1 of your Benched Pokémon");
                if (target == null) return true;
                if (target.DamageCounters > 0) ctx.Heal(target, target.DamageCounters * 10);
                ctx.Engine.DiscardAllEnergy(target);
                return true;
            }
        }

        /// <summary>Hypnoshower: the Defending Pokémon is now Asleep; 1 damage counter off each Benched Pokémon (both sides) that has any.</summary>
        private sealed class HypnoshowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnoshower") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).Where(m => m.DamageCounters > 0).ToList())
                    if (mon.Owner == ctx.Player || ctx.CanAffect(mon)) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>Mysterious Wing: your opponent may put a Pokémon card from their discard pile into their hand; then you may do the same.</summary>
        private sealed class MysteriousWingMon : Mon
        {
            private static void Take(EffectContext who)
            {
                var cards = who.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (cards.Count == 0) return;
                var pick = who.ChooseCards(cards, 0, 1, "Mysterious Wing: you may put a Pokémon card from your discard pile into your hand");
                if (pick.Count > 0) who.DiscardToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mysterious Wing") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Take(Them(ctx));
                Take(ctx);
                return true;
            }
        }

        /// <summary>Synchronize: does nothing if this Pokémon and the Defending Pokémon have different numbers of Energy cards attached.</summary>
        private sealed class SynchronizeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchronize") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (defending == null || me == null || defending.Energy.Count != me.Energy.Count) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Evolution Song: your opponent may evolve 1 of their Pokémon with a card from their deck that evolves from it; then you may
        /// do the same; each player who searched shuffles.
        /// </summary>
        private sealed class EvolutionSongMon : Mon
        {
            private static bool Evolve(EffectContext who)
            {
                if (who.Me.Deck.Count == 0) return false;
                var candidates = who.MyPokemon(m => !CardsEVS1.EvolveBlocked(who, m));
                if (candidates.Count == 0 || !who.AskYesNo("Evolution Song: evolve 1 of your Pokémon from your deck?")) return false;
                var target = candidates.Count == 1 ? candidates[0] : who.ChoosePokemon(candidates, "Evolution Song: choose 1 of your Pokémon");
                if (target == null) return false;
                var found = who.SearchMyDeck(d => CardsEVS1.EvolvesOnto(who, target, d), 1, "Evolution Song: choose a card that evolves from " + target.Name, false);
                if (found.Count > 0) who.Engine.EvolvePokemon(target, found[0]);
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evolution Song") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool theySearched = Evolve(Them(ctx));
                bool iSearched = Evolve(ctx);
                if (theySearched) ctx.Engine.ShuffleDeck(ctx.Opponent);
                if (iSearched) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Charm: if the Defending Pokémon attacks during the opponent's next turn, its damage is reduced by 10 (before W/R).</summary>
        private sealed class CharmMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charm") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 10 less damage during its next turn (Charm)", ctx.Player, defending.Uid, ctx.Turn + 1, 10));
                return true;
            }
        }

        /// <summary>Ink Blast: 20 + 10 for each Energy attached to this Pokémon but not used to pay for this attack (20 more at most).</summary>
        private sealed class InkBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ink Blast") return false;
                int extra = ctx.Pokemon == null ? 0 : Math.Max(0, Math.Min(2, ctx.EnergyCount(ctx.Pokemon) - atk.Cost.Length));
                ctx.DamageDefending(atk.Damage + 10 * extra);
                return true;
            }
        }

        /// <summary>Tentacle Wrap: heads, Paralyzed; tails, the opponent pays [C] more to retreat the Defending Pokémon during their next turn.</summary>
        private sealed class TentacleWrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacle Wrap") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                else if (ctx.CanAffect(defending))
                    ctx.AddEffect(new ExtraRetreatEffect(defending.Name + "'s Retreat Cost is [C] more during its next turn (Tentacle Wrap)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class ExtraRetreatEffect : ContinuousEffect
        {
            public ExtraRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost) { return mon != null && mon.Uid == TargetUid ? cost + 1 : cost; }
        }

        /// <summary>Shed: remove 1 damage counter from this Pokémon.</summary>
        private sealed class ShedMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shed") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Stun Poison: heads, Paralyzed and Poisoned.</summary>
        private sealed class StunPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stun Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Dodge: during the opponent's next turn, flip whenever an attack would damage this Pokémon; heads, that damage is prevented.</summary>
        private sealed class FlipDodgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dodge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new CardsSUM4.FlipToPreventEffect(me.Name + ": flip to prevent attack damage during the opponent's next turn (Dodge)", ctx.Player, me.Uid, ctx.Turn + 1, "Dodge"));
                return true;
            }
        }

        /// <summary>Leer: heads, the Defending Pokémon can't attack during the opponent's next turn (benching or evolving either Pokémon ends this).</summary>
        private sealed class LeerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leer") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!heads || !ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new LeerEffect(defending.Name + " can't attack during its next turn (Leer)", ctx.Player, defending.Uid, ctx.Turn + 1, me.Uid, me.Stack.Count));
                return true;
            }
        }

        private sealed class LeerEffect : ContinuousEffect
        {
            private readonly int anchorUid;
            private readonly int anchorStack;

            public LeerEffect(string name, int owner, int targetUid, int expiresAfterTurn, int anchorUid, int anchorStack) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.anchorUid = anchorUid;
                this.anchorStack = anchorStack;
            }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                if (mon == null || mon.Uid != TargetUid) return false;
                var anchor = engine.State.Players[Owner].Active;
                return anchor != null && anchor.Uid == anchorUid && anchor.Stack.Count == anchorStack;
            }
        }

        /// <summary>Swift: the damage isn't affected by Weakness, Resistance, Pokémon Powers or any other effects on the Defending Pokémon.</summary>
        internal sealed class SwiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>Reflected Sunlight: attach up to 2 Grass Energy cards from your hand to 1 of your Grass Pokémon.</summary>
        private sealed class ReflectedSunlightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflected Sunlight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var grass = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList();
                var targets = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass));
                if (grass.Count == 0 || targets.Count == 0) return true;
                var picks = ctx.ChooseCards(grass, 0, Math.Min(2, grass.Count), "Reflected Sunlight: attach up to 2 Grass Energy cards from your hand");
                if (picks.Count == 0) return true;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Reflected Sunlight: attach them to which of your Grass Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
                return true;
            }
        }

        /// <summary>Migraine: heads, the Defending Pokémon is now Confused; tails, this Pokémon is.</summary>
        private sealed class MigraineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Migraine") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                {
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Sunbathe: heads, remove all damage counters from this Pokémon, then evolve it with a card from your deck (tails: nothing, your ruling).</summary>
        private sealed class SunbatheMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sunbathe") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                CardsEVS1.EvolveFromDeck(ctx, me, "Sunbathe");
                return true;
            }
        }

        // ================================================================== Unown

        /// <summary>Unown [X]: [XXXXX] works through Hidden Power (CardsN2.XxxxxBonus).</summary>
        private sealed class HiddenPowerUnown : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Give]: once during your turn, with Unown G, I, V and E on your Bench, flip; heads, attach a basic Energy card from your deck to 1 of your Pokémon.</summary>
        private sealed class GiveUnown : Mon
        {
            protected override string Activated { get { return "[Give]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && CardsN2.UnownOnBench(ctx, "G", "I", "V", "E"); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "[Give]: choose a basic Energy card");
                foreach (var card in found)
                {
                    var mons = ctx.MyPokemon();
                    var target = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "[Give]: attach " + card.Def.Name + " to which Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Help]: once during your turn, with Unown H, E, L and P on your Bench, shuffle your hand into your deck and draw that many cards.</summary>
        private sealed class HelpUnown : Mon
        {
            protected override string Activated { get { return "[Help]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsN2.UnownOnBench(ctx, "H", "E", "L", "P"); }

            protected override void Ability(EffectContext ctx)
            {
                int count = ctx.Me.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                if (count > 0) ctx.Draw(count);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Want]: once during your turn, with Unown W, A, N and T on your Bench, flip; heads, a Trainer card from your discard pile into your hand.</summary>
        private sealed class WantUnown : Mon
        {
            protected override string Activated { get { return "[Want]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsTrainer) && CardsN2.UnownOnBench(ctx, "W", "A", "N", "T"); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return;
                var pick = trainers.Count == 1 ? trainers : ctx.ChooseCards(trainers, 1, 1, "[Want]: put which Trainer card into your hand?");
                ctx.DiscardToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>
        /// [Chase]: while this Pokémon is your Active Pokémon, whenever the opponent's Active Pokémon tries to retreat, flip; heads,
        /// 1 damage counter on it, with Weakness and Resistance applied (no Asleep, Confused or Paralyzed clause).
        /// </summary>
        private sealed class ChaseUnown : Mon
        {
            public override void OnActiveRetreatTried(EffectContext src, PokemonInPlay retreating)
            {
                var me = src.Pokemon;
                if (me == null || retreating == null || retreating.Owner == src.Player || !src.IsActive(me)) return;
                Announce(src, "[Chase]");
                if (!src.Engine.FlipCoin(src.Player, "[Chase]")) return;
                int damage = CardsTR.WithWeaknessAndResistance(src.Engine, me, retreating, 10);
                if (damage >= 10) src.Engine.PlaceDamageCounters(retreating, damage / 10, "[Chase]");
                else src.Log("[Chase]: no damage counter after Resistance.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>
        /// [Perform]: if an attack damaged this Pokémon during the opponent's last turn while it was your Active Pokémon, its Hidden
        /// Power does that much more damage.
        /// </summary>
        private sealed class PerformUnown : Mon
        {
            private const string TurnMarker = "Perform turn";
            private const string DamageMarker = "Perform damage";

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || attacker == null || attacker.Owner == ctx.Player || !holderWasActive || damage <= 0) return;
                int turn;
                if (!me.Markers.TryGetValue(TurnMarker, out turn) || turn != ctx.Turn)
                {
                    me.Markers[TurnMarker] = ctx.Turn;
                    me.Markers[DamageMarker] = 0;
                }
                int sum;
                me.Markers.TryGetValue(DamageMarker, out sum);
                me.Markers[DamageMarker] = sum + damage;
            }

            private static int Bonus(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                int turn, damage;
                if (me == null || !me.Markers.TryGetValue(TurnMarker, out turn) || turn != ctx.OpponentsLastTurn) return 0;
                if (!me.Markers.TryGetValue(DamageMarker, out damage) || damage <= 0 || ctx.Engine.AbilityBlocked(me, "[Perform]")) return 0;
                ctx.Log("[Perform]: " + damage + " more damage.");
                return damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                return CardsN2.HiddenPower(ctx, atk, Bonus(ctx));
            }
        }

        /// <summary>
        /// [Quicken]: once during your turn (1 per player with more than 1 Unown [Q]), flip; heads, prevent all effects of attacks,
        /// damage included, done to your Pokémon with Unown in their names during the opponent's next turn. It works even while
        /// Asleep, Confused or Paralyzed.
        /// </summary>
        private sealed class QuickenUnown : Mon
        {
            protected override string Activated { get { return "[Quicken]"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                    ctx.AddEffect(new QuickenEffect(ctx.Engine.Possessive(ctx.Player) + " Pokémon with Unown in their names are protected from attacks during the opponent's next turn ([Quicken])", ctx.Player, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        private sealed class QuickenEffect : ContinuousEffect
        {
            public QuickenEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            private bool Covers(PokemonInPlay mon) { return mon != null && mon.Owner == Owner && IsUnown(mon); }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return Covers(info.Target) && info.AgainstOpponent && engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (!Covers(target) || kind != EffectSourceKind.Attack) return false;
                var attacker = engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != Owner;
            }
        }

        /// <summary>[Zoom]: while this Pokémon is on your Bench, your Pokémon with Unown in their names have no Retreat Cost.</summary>
        private sealed class ZoomUnown : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner != src.Player || !src.Me.Bench.Contains(me) || !IsUnown(mon)) return cost;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Laugh]: once during your turn, flip; heads, each player shuffles their deck.</summary>
        private sealed class LaughUnown : Mon
        {
            protected override string Activated { get { return "[Laugh]"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.ShuffleMyDeck();
                ctx.Engine.ShuffleDeck(ctx.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Search]: once during your turn, look at 1 of your Prize cards (it stays face down).</summary>
        private sealed class SearchUnown : Mon
        {
            protected override string Activated { get { return "[Search]"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Prizes.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int count = ctx.Me.Prizes.Count;
                if (count == 0) return;
                int pick = count == 1 ? 1 : ctx.AskNumber(1, count, "[Search]: look at which Prize card (1-" + count + ")?");
                pick = Math.Max(1, Math.Min(count, pick));
                var card = ctx.Me.Prizes[pick - 1];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { card }, "Prize card", "[Search]: Prize card " + pick, ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at 1 of their Prize cards ([Search]).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>[Tell]: once during your turn, flip; heads, look at your opponent's hand and show them yours.</summary>
        private sealed class TellUnown : Mon
        {
            protected override string Activated { get { return "[Tell]"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", "[Tell]: your opponent's hand", ctx.Source);
                ctx.Engine.ShowCards(ctx.Opponent, ctx.Me.Hand.ToList(), "Your opponent's hand", "[Tell]: your opponent's hand", ctx.Source);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>
        /// [Vanish]: when you play this Pokémon from your hand, you may flip; heads, return 1 of your other Pokémon with Unown in its
        /// name to your hand and discard the cards attached to it.
        /// </summary>
        private sealed class VanishUnown : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null) return;
                var others = ctx.MyPokemon(m => m != me && IsUnown(m));
                if (others.Count == 0 || !ctx.AskYesNo("[Vanish]: flip a coin to return 1 of your other Unown to your hand?")) return;
                Announce(ctx, "[Vanish]");
                if (!ctx.Flip()) return;
                others = ctx.MyPokemon(m => m != me && IsUnown(m));
                if (others.Count == 0) return;
                var target = others.Count == 1 ? others[0] : ctx.ChoosePokemon(others, "[Vanish]: return which Pokémon to your hand?");
                if (target == null || !ctx.Engine.CanPutIntoHand(target)) return;
                ctx.Engine.RemoveFromPlay(target);
                ctx.Me.Hand.AddRange(target.Stack);
                ctx.Engine.DiscardFromPlay(target.Energy.ToList());
                ctx.Engine.DiscardFromPlay(target.Tools.ToList());
                ctx.Log(target.Name + " was returned to " + ctx.Engine.Possessive(ctx.Player) + " hand and its attached cards were discarded ([Vanish]).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        // ================================================================== Shining Pokémon

        /// <summary>White-hot Flame: discard a Fire and a Lightning Energy card or nothing; tails, 30 damage to itself.</summary>
        private sealed class ShiningCharizardN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "White-hot Flame") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var fire = EnergyCardsOf(ctx, me, EnergyType.Fire);
                    var lightning = EnergyCardsOf(ctx, me, EnergyType.Lightning);
                    if (fire.Count == 0 || lightning.Count == 0 || (fire.Count == 1 && lightning.Count == 1 && fire[0] == lightning[0])) return Nothing(ctx);
                    var f = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "White-hot Flame: discard which Fire Energy card?")[0];
                    lightning.Remove(f);
                    if (lightning.Count == 0) return Nothing(ctx);
                    var l = lightning.Count == 1 ? lightning[0] : ctx.ChooseCards(lightning, 1, 1, "White-hot Flame: discard which Lightning Energy card?")[0];
                    ctx.Engine.DiscardEnergy(me, f);
                    ctx.Engine.DiscardEnergy(me, l);
                }
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>
        /// Lightning Cut: heads, 40 and 10 to each of the opponent's Benched Pokémon; tails, 30 and 10 to itself. Water Slash: 50 + 10
        /// for each Water Energy not used to pay for it; Resistance doesn't apply.
        /// </summary>
        private sealed class ShiningKabutopsN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Lightning Cut")
                {
                    if (ctx.Flip())
                    {
                        ctx.DamageDefending(atk.Damage + 10);
                        foreach (var benched in ctx.OpponentBench())
                            if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                    }
                    else
                    {
                        ctx.DamageDefending(atk.Damage);
                        if (ctx.InPlay(me)) ctx.DamageSelf(10);
                    }
                    return true;
                }
                if (atk.Name == "Water Slash")
                {
                    int water = me == null ? 0 : ctx.Engine.EnergyCountOfType(me, EnergyType.Water);
                    int total = me == null ? 0 : ctx.EnergyCount(me);
                    int w = atk.Cost.Count(t => t == EnergyType.Water);
                    int c = atk.Cost.Count(t => t == EnergyType.Colorless);
                    int extra = Math.Max(0, water - w - Math.Max(0, c - (total - water)));
                    ctx.DamageWith(ctx.Defending, atk.Damage + 10 * extra, true, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Reflect Shield: during the opponent's next turn, if an attack does damage to this Pokémon, flip; heads, that damage is
        /// prevented and the attacking Pokémon takes 20 damage (Weakness and Resistance apply: your ruling). Psyburst: discard a Fire
        /// Energy card or nothing; 40 + 10 for each Energy attached to the Defending Pokémon.
        /// </summary>
        private sealed class ShiningMewtwoN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Reflect Shield")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me))
                        ctx.AddEffect(new ReflectShieldEffect(me.Name + ": flip to reflect attack damage during the opponent's next turn (Reflect Shield)", ctx.Player, me.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Psyburst")
                {
                    if (!PayDiscard(ctx, EnergyType.Fire, 1, atk.Name)) return Nothing(ctx);
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage + 10 * (defending == null ? 0 : ctx.EnergyCount(defending)));
                    return true;
                }
                return false;
            }
        }

        private sealed class ReflectShieldEffect : ContinuousEffect
        {
            public ReflectShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return false;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack || !engine.FlipCoin(Owner, "Reflect Shield")) return false;
                var attacker = info.Attacker;
                if (engine.State.FindPokemon(attacker.Uid) == attacker)
                {
                    int back = CardsTR.WithWeaknessAndResistance(engine, info.Target, attacker, 20);
                    engine.Emit(GameEventType.Info, Owner, "Reflect Shield: " + info.Target.Name + " does " + back + " damage to " + attacker.Name + ".", info.Target.Top.Id, info.Target.Uid);
                    if (back >= 10) engine.PlaceDamageCounters(attacker, back / 10, "Reflect Shield");
                }
                return true;
            }
        }

        /// <summary>Flashing Eyes: 3 coins; exactly 1 heads Asleep, exactly 2 Confused, 3 Paralyzed.</summary>
        private sealed class FlashingEyesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flashing Eyes") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage);
                if (heads == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, heads == 1 ? SpecialCondition.Asleep : heads == 2 ? SpecialCondition.Confused : SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Thundersquall: 10 damage to 1 of the opponent's Benched Pokémon for each Water Energy attached to this Pokémon.</summary>
        private sealed class ThundersquallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thundersquall") return false;
                int water = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                ctx.DamageDefending(atk.Damage);
                if (water > 0) ctx.DamageOneBenched(10 * water);
                return true;
            }
        }

        /// <summary>Crushing Earth: heads, 80 and 10 to each Benched Pokémon (both sides); tails, nothing; either way it can't attack next turn.</summary>
        private sealed class CrushingEarthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Earth") return false;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var benched in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                        if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                }
                else Nothing(ctx);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Mountain Crush: flip until tails; the opponent discards the top card of their deck for each heads. Destructive Fire: a coin
        /// for each Fire Energy card attached; discard a Fire Energy card for each heads, then discard an Energy card from the
        /// Defending Pokémon for each heads.
        /// </summary>
        private sealed class ShiningTyranitarN4 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Mountain Crush")
                {
                    ctx.DamageDefending(atk.Damage);
                    int heads = ctx.FlipUntilTails();
                    if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                    return true;
                }
                if (atk.Name == "Destructive Fire")
                {
                    int coins = EnergyCardsOf(ctx, me, EnergyType.Fire).Count;
                    int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                    if (!PayDiscard(ctx, EnergyType.Fire, heads, atk.Name)) return Nothing(ctx);
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                        ctx.DiscardEnergyFrom(defending, heads, "Destructive Fire: discard " + heads + " Energy card" + (heads == 1 ? "" : "s") + " from " + defending.Name);
                    return true;
                }
                return false;
            }
        }
    }
}
