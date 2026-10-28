using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Cosmic Eclipse (CEC) part 2: Lightning, Psychic and Fighting (CEC 66-127). Rulings used: Light of the Protector-GX protects
    /// every one of your Pokémon during the opponent's turn right after the attack (your ruling, v68); Mimikyu's Impersonation uses the
    /// Supporter's whole effect, "when you play this card" parts included, even one you couldn't play (Compendium, 2019); Recall needs
    /// only its own cost (2019-10-31); Golurk's Double Type adds to its types rather than replacing them (2015-08-13).
    /// </summary>
    public static class CardsCEC2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("CEC-66", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new RecoilMon("Volt Tackle", 10)));
            r.RegisterByKey("CEC-67", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new CardsASR1.TimesMon("Powerful Spark", LightningEnergyInPlay)));
            r.RegisterByKey("CEC-68", new CardsMEG.SandAttackMon("Mirror Shot"));
            r.RegisterByKey("CEC-69", new MagnetonCEC());
            r.RegisterByKey("CEC-70", new JolteonCEC());
            r.RegisterByKey("CEC-71", new Cards30C.PeerAtMon("Scout"));
            r.RegisterByKey("CEC-72", new LanturnCEC());
            r.RegisterByKey("CEC-73", new ComboMon(new CallForFamilyMon(), new CardsMEG.SnipeMon("Random Spark", 20)));
            r.RegisterByKey("CEC-74", new ParalyzeFlipMon("Thunder Shock"));
            // Psychic
            r.RegisterByKey("CEC-75", new SolgaleoLunalaGX());
            r.RegisterByKey("CEC-76", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("CEC-77", new BalloonBurstMon());
            r.RegisterByKey("CEC-78", new CardsVIV1.FutureSightMon());
            r.RegisterByKey("CEC-79", new ComboMon(new ConditionAttackMon("Creepy Wind", SpecialCondition.Confused), new LifeDrainMon()));
            r.RegisterByKey("CEC-80", new SwitchSelfMon("Teleport", false));
            r.RegisterByKey("CEC-81", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("CEC-82", new GalladeCEC());
            r.RegisterByKey("CEC-83", new DuskullCEC());
            r.RegisterByKey("CEC-84", new DisableMon("Disable"));
            r.RegisterByKey("CEC-85", new DusknoirCEC());
            r.RegisterByKey("CEC-86", new ComboMon(new CardsCRZ1.CycleDrawMon("Cycle Draw", 2), new EnergyAssistMon()));
            r.RegisterByKey("CEC-87", new ComboMon(new NoRetreatMon("Nasal Suction"), new TailsNothingMon("Air Cutter")));
            r.RegisterByKey("CEC-88", new ComboMon(new ConditionAttackMon("Supersonic", SpecialCondition.Confused), new CharmingStampMon()));
            r.RegisterByKey("CEC-89", new CardsDAA1.MayDrawUntilMon("Return", 5));
            r.RegisterByKey("CEC-90", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new FistOfAntiquityMon()));
            r.RegisterByKey("CEC-91", new ConditionAttackMon("Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("CEC-92", new PoisonCultivationMon());
            r.RegisterByKey("CEC-94", new ComboMon(new CardsDRI1.WhirlwindMon("Perplexing Forest", true), new ShadowImpactMon()));
            r.RegisterByKey("CEC-95", new OricorioGX());
            r.RegisterByKey("CEC-96", new MimikyuImpersonation());
            r.RegisterByKey("CEC-97", new MimikyuShadowBox());
            r.RegisterByKey("CEC-98", new ComboMon(new SeaweedGrabMon(), new CardsFLF1.NoResistanceMon("Buster Swing")));
            r.RegisterByKey("CEC-99", new CardsSIT1.AscensionMon());
            r.RegisterByKey("CEC-100", new CosmogUnaware());
            r.RegisterByKey("CEC-101", new ShieldAttackMon("Stiffen", 40));
            r.RegisterByKey("CEC-102", new LunalaCEC());
            r.RegisterByKey("CEC-103", new MarshadowCEC());
            r.RegisterByKey("CEC-104", new FireworksBombMon());
            // Fighting
            r.RegisterByKey("CEC-105", new ComboMon(new DigDeepMon(), new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("CEC-106", new ComboMon(new CardsDAA1.GustAttackMon("Draw Toward", false), new LockAfterAttackMon("Zap Cannon", true)));
            r.RegisterByKey("CEC-107", new SearchAttackMon("Nest Building", IsStadiumCard, 1));
            r.RegisterByKey("CEC-108", new DodgeMon("Dig", false));
            r.RegisterByKey("CEC-109", new VibravaCEC());
            r.RegisterByKey("CEC-110", new FlygonGX());
            r.RegisterByKey("CEC-112", new ComboMon(new CardsASR1.PlusMon("Ancient Blast", 50, FossilsInDiscard), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("CEC-113", new ComboMon(new DroughtMon(), new LockAfterAttackMon("Trembling Ground", true)));
            r.RegisterByKey("CEC-114", new RototillerMon());
            r.RegisterByKey("CEC-115", new ComboMon(new CardsFST2.ConditionBonusMon("Eleventh Hour Tackle", 150, DeckNearlyEmpty), new CardsBRS1.MillSelfMon("Drill Bazooka", 4)));
            r.RegisterByKey("CEC-116", new CardsBUS1.OwnBenchQuakeMon("Mini Earthquake", 10));
            r.RegisterByKey("CEC-117", new SeismitoadCEC());
            r.RegisterByKey("CEC-118", new CardsSVP.BenchHurtBonusMon("Reverse Shoulder Throw", 90));
            r.RegisterByKey("CEC-122", new CardsFST1.TakeLessMon(30, new CardsFST2.ConditionBonusMon("Freezing Punch", 80, HasWaterEnergy)));
            r.RegisterByKey("CEC-123", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("CEC-124", new LycanrocCEC());
            r.RegisterByKey("CEC-125", new DrawAttackMon("Spike Draw", 2));
            r.RegisterByKey("CEC-126", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("CEC-127", new ComboMon(new ShieldAttackMon("Guard Press", 20), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20)));
        }

        private static int LightningEnergyInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Lightning)); }
        private static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }
        private static bool IsStadiumCard(CardDef d) { return d.IsStadiumCard; }
        private static int FossilsInDiscard(EffectContext ctx) { return ctx.Me.Discard.Count(c => c.Def.Name == "Unidentified Fossil"); }
        private static bool DeckNearlyEmpty(EffectContext ctx) { return ctx.Me.Deck.Count <= 3; }
        private static bool HasWaterEnergy(EffectContext ctx) { return ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water) > 0; }

        /// <summary>
        /// "Prevent all effects of attacks, including damage, done to each of your Pokémon during your opponent's next turn" (Light of
        /// the Protector-GX): every Pokémon of the player's in play during that turn (your ruling, v68).
        /// </summary>
        internal sealed class TeamProtectEffect : ContinuousEffect
        {
            public TeamProtectEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage) { return info.Target.Owner == Owner && info.AgainstOpponent; }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind) { return target.Owner == Owner && kind == EffectSourceKind.Attack; }
        }

        // ================================================================== Lightning

        /// <summary>Call Signal: once during your turn, search for up to 3 Supporter cards; if you searched, this Pokémon is Knocked Out.</summary>
        private sealed class MagnetonCEC : Mon
        {
            protected override string Activated { get { return "Call Signal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Call Signal"; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(IsSupporterCard, 3, "Call Signal: search your deck for up to 3 Supporter cards");
                ctx.Engine.KnockOut(ctx.Pokemon, "Call Signal");
            }
        }

        /// <summary>Speed Cheer: attacks of your Pokémon-GX that evolve from Eevee cost [C] less (only 1 Speed Cheer applies).</summary>
        private sealed class JolteonCEC : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !CardsCEC1.EeveeGX(mon)) return;
                if (!CardsCRZ1.IsLeadSource(src, "Speed Cheer")) return;
                int i = cost.IndexOf(EnergyType.Colorless);
                if (i >= 0) cost.RemoveAt(i);
            }
        }

        /// <summary>Blinking Lights: as often as you like during your turn, look at the top card of the opponent's deck. Swirling Flow: you may have the opponent shuffle their deck.</summary>
        private sealed class LanturnCEC : Mon
        {
            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != "Blinking Lights") return false;
                return ctx.IsMyTurn && ctx.Opp.Deck.Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck (Blinking Lights).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Flow") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 1 && ctx.AskYesNo("Swirling Flow: have your opponent shuffle their deck?")) ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Cosmic Burn: can't be used during your next turn. Light of the Protector-GX: if you played Lillie's Full Force from your hand this
        /// turn, prevent all effects of attacks, damage included, done to each of your Pokémon during the opponent's next turn.
        /// </summary>
        private sealed class SolgaleoLunalaGX : Mon
        {
            private static readonly Mon CosmicBurn = new LockAfterAttackMon("Cosmic Burn", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light of the Protector-GX") return CosmicBurn.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.SupporterThisTurn != null && ctx.Me.SupporterThisTurn.Name == "Lillie's Full Force")
                    ctx.AddEffect(new TeamProtectEffect("Effects of attacks, damage included, are prevented on " + ctx.Engine.PlayerName(ctx.Player) + "'s Pokémon during the opponent's next turn (Light of the Protector-GX)", ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Balloon Burst: after the damage, discard this Pokémon and all cards attached to it (not a Knock Out).</summary>
        private sealed class BalloonBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Balloon Burst") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Life Drain: on heads, put damage counters on the opponent's Active Pokémon until its remaining HP is 10.</summary>
        private sealed class LifeDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Life Drain") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip() || defending == null) return true;
                int counters = (ctx.Engine.RemainingHp(defending) - 10) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Double Type: while in play it's Psychic and Fighting (types other effects give it stay). Power Cyclone: move an Energy to a Benched Pokémon.</summary>
        private sealed class GalladeCEC : Mon
        {
            private static readonly Mon PowerCyclone = new CardsFST1.MoveEnergyToBenchMon("Power Cyclone");

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Psychic)) types.Add(EnergyType.Psychic);
                if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PowerCyclone.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiritborne Evolution: once during your turn, discard 3 cards from your hand to evolve it from your deck. Ominous Eyes: 2 counters on 1 of the opponent's Pokémon.</summary>
        private sealed class DuskullCEC : Mon
        {
            private static readonly Mon OminousEyes = new CardsRCL1.CountersOnOneMon("Ominous Eyes", 2);

            protected override string Activated { get { return "Spiritborne Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count >= 3 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 3, 3, "Spiritborne Evolution: discard 3 cards from your hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                var me = ctx.Pokemon;
                string name = me.Name;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Spiritborne Evolution: choose a card that evolves from " + name, false);
                if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return OminousEyes.HandleAttack(ctx, atk); }
        }

        /// <summary>Disable: choose 1 of the Defending Pokémon's attacks; it can't use that attack during the opponent's next turn. Used for Disable, Psyjack (the attack name is passed in).</summary>
        internal sealed class DisableMon : Mon
        {
            private readonly string attack;

            public DisableMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                int index = ChooseAttackOf(ctx, defending.Def, attack + ": choose 1 of " + defending.Name + "'s attacks it can't use during its next turn");
                if (index < 0) return true;
                string name = defending.Def.Attacks[index].Name;
                ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1, name));
                return true;
            }
        }

        /// <summary>Grim Marking: if it's Knocked Out by damage from an opponent's attack while Active, put 4 damage counters on their Pokémon in any way you like. Psych Up: 60 more next turn.</summary>
        private sealed class DusknoirCEC : Mon
        {
            private static readonly Mon PsychUp = new CardsASC.NextTurnBoostMon("Psych Up", 60);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.OpponentPokemon().Count == 0) return;
                src.Log("Grim Marking: " + src.Engine.PlayerName(src.Player) + " puts 4 damage counters on the opponent's Pokémon.");
                src.PlaceCountersAnyWay(4, () => src.OpponentPokemon());
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PsychUp.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Assist: attach 2 basic Energy cards from your discard pile to your Benched Pokémon in any way you like.</summary>
        private sealed class EnergyAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, Math.Min(2, energy.Count), (card, mon) => !ctx.IsActive(mon), "Energy Assist: attach a basic Energy card to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>Charming Stamp: your opponent chooses 1 of their own Pokémon; 90 damage to it.</summary>
        private sealed class CharmingStampMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charming Stamp") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.Engine.ChoosePokemon(ctx.Opponent, all, "Charming Stamp: choose 1 of your Pokémon to take 90 damage", ctx.Source);
                if (target != null) ctx.DamageTarget(target, 90);
                return true;
            }
        }

        /// <summary>Fist of Antiquity: does nothing if you have any Supporter cards in your discard pile.</summary>
        private sealed class FistOfAntiquityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fist of Antiquity") return false;
                if (ctx.Me.Discard.Any(c => c.Def.IsSupporter)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Poison Cultivation: if the opponent's Active Pokémon is Poisoned, Poison puts 10 damage counters on it instead of 1.</summary>
        private sealed class PoisonCultivationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Cultivation") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Poisoned) || !ctx.CanAffect(defending)) return true;
                defending.PoisonCounterBase = 10;
                ctx.Log("Poison Cultivation: Poison puts 10 damage counters on " + defending.Name + " between turns.");
                return true;
            }
        }

        /// <summary>Shadow Impact: put 4 damage counters on 1 of your Pokémon.</summary>
        private sealed class ShadowImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Shadow Impact: put 4 damage counters on which of your Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 4);
                return true;
            }
        }

        /// <summary>Dance of Tribute: once during your turn (1 per player), if any of your Pokémon were Knocked Out during the opponent's last turn, draw 3. Strafe-GX: switch with a Benched Pokémon.</summary>
        private sealed class OricorioGX : Mon
        {
            private static readonly Mon Strafe = new SwitchSelfMon("Strafe-GX", false);

            protected override string Activated { get { return "Dance of Tribute"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.LastKnockedOutTurn == ctx.OpponentsLastTurn && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(3); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Strafe.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Impersonation: discard a Supporter card from your hand and use its effect as this attack's effect (its whole effect, even if you
        /// couldn't play it: Compendium, 2019). Mischievous Hands: 2 damage counters on each of 2 of the opponent's Pokémon.
        /// </summary>
        private sealed class MimikyuImpersonation : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mischievous Hands")
                {
                    var all = ctx.OpponentPokemon();
                    if (all.Count == 0) return true;
                    var targets = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, "Mischievous Hands: choose 2 of your opponent's Pokémon (2 damage counters each)");
                    foreach (var t in targets) ctx.PlaceCounters(t, 2);
                    return true;
                }
                if (atk.Name != "Impersonation") return false;
                var supporters = ctx.Me.Hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0)
                {
                    ctx.Log("Impersonation did nothing: there's no Supporter card in the hand.");
                    return true;
                }
                var card = ctx.ChooseCards(supporters, 1, 1, "Impersonation: discard which Supporter card and use its effect?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (!ctx.Me.Discard.Contains(card)) return true;
                ctx.Log("Impersonation: " + ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + ".");
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                return true;
            }
        }

        /// <summary>Shadow Box: Pokémon-GX with damage counters on them (both players') have no Abilities. Tail Trickery: Confused on heads.</summary>
        private sealed class MimikyuShadowBox : Mon
        {
            private static readonly Mon TailTrickery = new CardsPAR2.FlipConditionMon("Tail Trickery", SpecialCondition.Confused);

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return mon != null && mon.Def.IsGX && mon.DamageCounters > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TailTrickery.HandleAttack(ctx, atk); }
        }

        /// <summary>Seaweed Grab: put a Trainer card from your discard pile into your hand.</summary>
        private sealed class SeaweedGrabMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seaweed Grab") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Seaweed Grab: put which Trainer card into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
                return true;
            }
        }

        /// <summary>Unaware: prevent all effects of the opponent's attacks done to it, except damage. Surprise Attack: nothing on tails.</summary>
        private sealed class CosmogUnaware : Mon
        {
            private static readonly Mon SurpriseAttack = new TailsNothingMon("Surprise Attack");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SurpriseAttack.HandleAttack(ctx, atk); }
        }

        /// <summary>Blessing of the Moone: once during your turn, if you have Solgaleo in play, attach up to 2 Energy cards from your deck to your Solgaleo or Lunala.</summary>
        private sealed class LunalaCEC : Mon
        {
            private static bool SolgaleoOrLunala(CardInstance card, PokemonInPlay mon) { return mon.Name == "Solgaleo" || mon.Name == "Lunala"; }

            protected override string Activated { get { return "Blessing of the Moone"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.MyPokemon().Any(m => m.Name == "Solgaleo"); }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 2, "Blessing of the Moone: search your deck for up to 2 Energy cards");
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, SolgaleoOrLunala, "Blessing of the Moone: attach an Energy card to your Solgaleo or Lunala", false);
                foreach (var left in found.Where(c => !ctx.MyPokemon().Any(m => m.Energy.Contains(c))).ToList()) ctx.Me.Deck.Add(left);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Shadow Imitation: choose 1 of the opponent's Active Pokémon's non-GX attacks and use it as this attack.</summary>
        private sealed class MarshadowCEC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Imitation") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                var defs = new List<CardDef> { defending.Def };
                if (defending.Def.IsLevelUp && defending.Stack.Count >= 2) defs.Add(defending.Stack[defending.Stack.Count - 2].Def);
                var opts = new List<DecisionOption>();
                foreach (var d in defs)
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (GameEngine.IsGxAttack(a) || (a.VstarPower && ctx.Me.VstarUsed)) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                if (opts.Count == 0)
                {
                    ctx.Log("Shadow Imitation did nothing: " + defending.Name + " has no non-GX attacks.");
                    return true;
                }
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Shadow Imitation: choose 1 of " + defending.Name + "'s attacks", opts, ctx.Source);
                UseCopiedAttack(ctx, opts[pick].Card, opts[pick].Index);
                return true;
            }
        }

        /// <summary>Fireworks Bomb: put 4 damage counters on the opponent's Pokémon in any way you like; 12 if they have exactly 3 Prize cards left.</summary>
        private sealed class FireworksBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireworks Bomb") return false;
                int counters = ctx.Opp.Prizes.Count == 3 ? 12 : 4;
                ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Dig Deep: put an Energy card from your discard pile into your hand.</summary>
        private sealed class DigDeepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Deep") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var pick = ctx.ChooseCards(energy, 1, 1, "Dig Deep: put which Energy card into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
                return true;
            }
        }

        /// <summary>Obnoxious Whirring: prevent all effects of the opponent's Supporter cards done to it.</summary>
        private sealed class VibravaCEC : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && trainer != null && trainer.IsSupporter;
            }
        }

        /// <summary>
        /// Dusty Defense: while it's your Active Pokémon, all of your Pokémon take 30 less damage from the opponent's attacks. Desert
        /// Hurricane: 120 more if a Stadium is in play, then discard it. Sonic Edge-GX: ignores effects on the Defending Pokémon.
        /// </summary>
        private sealed class FlygonGX : Mon
        {
            private static readonly Mon SonicEdge = new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge-GX");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Desert Hurricane") return SonicEdge.HandleAttack(ctx, atk);
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? 120 : 0));
                if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Drought: attach up to 2 basic Fighting Energy cards from your hand to 1 of your Pokémon.</summary>
        private sealed class DroughtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drought") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fighting = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fighting)).ToList();
                if (fighting.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Drought: attach Fighting Energy from your hand to which of your Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(fighting, 0, Math.Min(2, fighting.Count), "Drought: attach up to 2 Fighting Energy cards to " + target.Name);
                foreach (var e in picks)
                    if (ctx.Me.Hand.Remove(e)) ctx.Engine.AttachEnergyCard(e, target, true);
                return true;
            }
        }

        /// <summary>Rototiller: shuffle a card from your discard pile into your deck.</summary>
        internal sealed class RototillerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rototiller") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Rototiller: shuffle which card from your discard pile into your deck?")[0];
                ctx.DiscardToDeck(new List<CardInstance> { pick });
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Bulldoze: once during your turn, search your deck for a card, shuffle, then put it on top. Tremulous Fist: 30 more for each damaged Benched Pokémon of yours.</summary>
        private sealed class SeismitoadCEC : Mon
        {
            private static readonly Mon TremulousFist = new CardsASR1.PlusMon("Tremulous Fist", 30, DamagedBenched);

            private static int DamagedBenched(EffectContext ctx) { return ctx.Me.Bench.Count(m => m.DamageCounters > 0); }

            protected override string Activated { get { return "Bulldoze"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(CardsCEC1.AnyCard, 1, "Bulldoze: choose a card to put on top of your deck", false, 1);
                ctx.ShuffleMyDeck();
                if (found.Count > 0) ctx.Me.Deck.Insert(0, found[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TremulousFist.HandleAttack(ctx, atk); }
        }

        /// <summary>Boiling Blood: if the opponent has any Pokémon-GX or Pokémon-EX in play, its attacks cost [C][C][C] less. Voltage Claw: 70 more against a Special Energy.</summary>
        private sealed class LycanrocCEC : Mon
        {
            private static readonly Mon VoltageClaw = new CardsFST2.ConditionBonusMon("Voltage Claw", 70, DefendingHasSpecial);

            private static bool DefendingHasSpecial(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy); }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !src.OpponentPokemon().Any(m => m.Def.IsGXorEX)) return;
                for (int k = 0; k < 3; k++)
                {
                    int i = cost.IndexOf(EnergyType.Colorless);
                    if (i < 0) break;
                    cost.RemoveAt(i);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return VoltageClaw.HandleAttack(ctx, atk); }
        }
    }
}
