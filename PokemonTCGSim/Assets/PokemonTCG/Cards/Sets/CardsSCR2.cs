using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Stellar Crown (SCR) 70-142: Pokémon (by printing key), Trainers and the Antique Fossils (by name).
    /// Pokémon whose attacks only do their printed damage (Greavard, Rhyhorn, Meditite SCR-77, Riolu, Lucario ex,
    /// Crabrawler, Impidimp, Morgrem, Meltan SCR-102, Varoom, Revavroom, Glameow, Fletchinder, Lechonk, Cyclizar)
    /// need no script. The fossils' play rules are in the engine (CardDef.IsFossil); only their Abilities are here.
    /// </summary>
    public static class CardsSCR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SCR-71", new IronBoulder());
            r.RegisterByKey("SCR-72", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SCR-73", new Marowak());
            r.RegisterByKey("SCR-75", new DestructiveHornMon("Destructive Horn"));
            r.RegisterByKey("SCR-76", new Rhyperior());
            r.RegisterByKey("SCR-78", new CardsJTG1.HoldStillMon("Calm Mind", 20));
            r.RegisterByKey("SCR-79", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("SCR-80", new MedichamEx());
            r.RegisterByKey("SCR-83", new KnockOffMon());
            r.RegisterByKey("SCR-84", new Mienshao());
            r.RegisterByKey("SCR-85", new ParalyzeFlipMon("Leer"));
            r.RegisterByKey("SCR-86", new Diancie());
            r.RegisterByKey("SCR-88", new Falinks());
            r.RegisterByKey("SCR-89", new GarganaclEx());
            r.RegisterByKey("SCR-90", new Koraidon());
            r.RegisterByKey("SCR-91", new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("SCR-92", new Swalot());
            r.RegisterByKey("SCR-93", new Pangoro());
            r.RegisterByKey("SCR-96", new Grimmsnarl());
            r.RegisterByKey("SCR-97", new RecoilMon("Reckless Charge", 20));
            r.RegisterByKey("SCR-98", new Jirachi());
            r.RegisterByKey("SCR-99", new DrawAttackMon("Allure", 1));
            r.RegisterByKey("SCR-100", new RecoilMon("Iron Tackle", 20));
            r.RegisterByKey("SCR-101", new Klinklang());
            r.RegisterByKey("SCR-103", new SearchAttackMon("Knickknack Carrying", d => d.IsTool, 1));
            r.RegisterByKey("SCR-104", new Melmetal());
            r.RegisterByKey("SCR-105", new CoinsMon("Iron Swing", 2));
            r.RegisterByKey("SCR-106", new RagingHammerMon());
            r.RegisterByKey("SCR-107", new Archaludon());
            r.RegisterByKey("SCR-110", new OrthwormEx());
            r.RegisterByKey("SCR-112", new DestructiveHornMon("Destructive Horn"));
            r.RegisterByKey("SCR-113", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("SCR-117", new DodgeMon("Nyan Roll", false));
            r.RegisterByKey("SCR-120", new ShieldAttackMon("Storm Barrier", 50));
            r.RegisterByKey("SCR-121", new CardsASC.PushDownMon("Send Back"));
            r.RegisterByKey("SCR-123", new Talonflame());
            r.RegisterByKey("SCR-124", new KnockOverMon(false));
            r.RegisterByKey("SCR-125", new KnockOverMon(true));
            r.RegisterByKey("SCR-128", new TerapagosEx());

            r.RegisterByName("Antique Cover Fossil", new AntiqueCoverFossil());
            r.RegisterByName("Antique Root Fossil", new AntiqueRootFossil());
            r.RegisterByName("Deluxe Bomb", new DeluxeBomb());
            r.RegisterByName("Grand Tree", new GrandTree());
            r.RegisterByName("Gravity Gemstone", new GravityGemstone());
            r.RegisterByName("Kofu", new Kofu());
            r.RegisterByName("Lacey", new Lacey());
            r.RegisterByName("Occa Berry", new TypeBerry(EnergyType.Fire, "Occa Berry"));
            r.RegisterByName("Payapa Berry", new TypeBerry(EnergyType.Psychic, "Payapa Berry"));
            r.RegisterByName("Sparkling Crystal", new SparklingCrystal());
        }

        // ================================================================== Shared

        /// <summary>"Flip a coin. If heads, discard an Energy from your opponent's Active Pokémon." (after the damage)</summary>
        private sealed class DestructiveHornMon : Mon
        {
            private readonly string attack;
            public DestructiveHornMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, atk.Name + ": discard an Energy from your opponent's Active Pokémon");
                return true;
            }
        }

        /// <summary>Knock Over: "You may discard a Stadium in play." Dubwool also has Soft Wool (takes 30 less damage).</summary>
        private sealed class KnockOverMon : Mon
        {
            private readonly bool softWool;
            public KnockOverMon(bool softWool) { this.softWool = softWool; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return softWool && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Knock Over: discard " + ctx.State.Stadium.Def.Name + "?"))
                    ctx.Engine.DiscardStadium();
                return true;
            }
        }

        // ================================================================== Psychic

        private sealed class IronBoulder : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Adjusted Horn") return false;
                if (ctx.Me.Hand.Count != ctx.Opp.Hand.Count) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Fighting

        private sealed class Marowak : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Growl")
                {
                    var defending = ctx.Defending;
                    if (defending != null && ctx.CanAffect(defending))
                        ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 40 less damage during its next turn (Growl)",
                            ctx.Player, defending.Uid, ctx.Turn + 1, 40));
                    return true;
                }
                if (atk.Name == "Bone Vengeance")
                {
                    bool hurtCubone = ctx.Me.Bench.Any(m => m.Name == "Cubone" && m.DamageCounters > 0);
                    ctx.DamageDefending(atk.Damage + (hurtCubone ? 120 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Wide Wall: while Active, the opponent's Supporters played from their hand have no effect on your Pokémon.</summary>
        private sealed class Rhyperior : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return trainer != null && trainer.IsSupporter && src.PokemonIsActive && target.Owner == src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drill Run") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Drill Run: discard an Energy from your opponent's Active Pokémon");
                return true;
            }
        }

        private sealed class MedichamEx : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Chi-Atsu")
                {
                    var defending = ctx.Defending;
                    if (defending == null) return true;
                    int counters = (ctx.RemainingHp(defending) - 50) / 10;
                    if (counters > 0) ctx.PlaceCounters(defending, counters);
                    return true;
                }
                if (atk.Name == "Yoga Kick")
                {
                    ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Knock Off: discard a random card from the opponent's hand.</summary>
        internal sealed class KnockOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Off") return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.DiscardRandomFromOpponentHand(ctx);
                return true;
            }
        }

        private sealed class Mienshao : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gale Roundhouse") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.Hand.Count <= 5 ? 60 : 0));
                return true;
            }
        }

        private sealed class Diancie : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diffuse Reflection") return false;
                int special = ctx.OpponentPokemon().Sum(m => m.Energy.Count(e => e.Def.IsAttachedSpecialEnergy));
                ctx.DamageDefending(atk.Damage * special);
                return true;
            }
        }

        /// <summary>Form Ranks: up to 2 Basic Pokémon onto the Bench. All-Out Attack: 90 more if this Pokémon used Form Ranks during your last turn.</summary>
        private sealed class Falinks : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Form Ranks")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => true, 2, "Form Ranks: put up to 2 Basic Pokémon onto your Bench");
                    return true;
                }
                if (atk.Name == "All-Out Attack")
                {
                    var self = ctx.Pokemon;
                    bool formed = self.LastAttackName == "Form Ranks" && self.LastAttackTurn == ctx.MyLastTurn;
                    ctx.DamageDefending(atk.Damage + (formed ? 90 : 0));
                    return true;
                }
                return false;
            }
        }

        private sealed class GarganaclEx : Mon
        {
            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return target == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Block Hammer") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(60);
                return true;
            }
        }

        private sealed class Koraidon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Resolute Fang") return false;
                bool ex = ctx.Defending != null && ctx.Defending.Def.IsEx;
                ctx.DamageDefending(atk.Damage + (ex ? 70 : 0));
                return true;
            }
        }

        // ================================================================== Darkness

        private sealed class Swalot : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Devouring Mouth")
                {
                    var defending = ctx.Defending;
                    bool more = defending != null && ctx.EnergyCount(ctx.Pokemon) > ctx.EnergyCount(defending);
                    ctx.DamageDefending(atk.Damage + (more ? 160 : 0));
                    return true;
                }
                if (atk.Name == "Venomous Hit")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    return true;
                }
                return false;
            }
        }

        private sealed class Pangoro : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pull")
                {
                    ctx.GustOpponent("Pull: choose 1 of your opponent's Benched Pokémon to switch in");
                    return true;
                }
                if (atk.Name == "Tantrum")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        private sealed class Grimmsnarl : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Goad 'n' Grab")
                {
                    if (ctx.OpponentSwitchesActive()) ctx.DamageDefending(160);
                    return true;
                }
                if (atk.Name == "Knuckle Sandwich")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Knuckle Sandwich: discard 2 Energy from this Pokémon");
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        private sealed class Jirachi : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swelling Wish") return false;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Swelling Wish: choose a Basic Energy from your discard pile")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Swelling Wish: attach " + card.Def.Name + " to which Benched Pokémon?");
                if (target == null) return true;
                ctx.Me.Discard.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Emergency Rotation (from the hand): if the opponent has a Stage 2 in play, put this Pokémon onto your Bench.</summary>
        private sealed class Klinklang : Mon
        {
            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && !ctx.NamedLimitUsedThisTurn("Emergency Rotation") && ctx.BenchSpace > 0
                       && ctx.Opp.InPlay().Any(m => m.Def.Stage == Stage.Stage2);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                ctx.MarkNamedLimit("Emergency Rotation");
                if (!ctx.Me.Hand.Contains(ctx.Card) || ctx.BenchSpace <= 0) return;
                ctx.Me.Hand.Remove(ctx.Card);
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Ray") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        private sealed class Melmetal : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reforged Axe") return false;
                if (ctx.Pokemon.Tool == null || !ctx.Engine.DiscardTool(ctx.Pokemon)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Raging Hammer: 10 more damage for each damage counter on this Pokémon.</summary>
        internal sealed class RagingHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Hammer") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>Metal Bridge: your Pokémon with Metal Energy attached have no Retreat Cost.</summary>
        private sealed class Archaludon : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon.Owner != src.Player) return cost;
                bool metal = mon.Energy.Any(e => src.Engine.EnergyUnitsOf(mon, e).Any(u => EnergyMask.Provides(u, EnergyType.Metal)));
                return metal ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Blaster") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Pummeling Payback: when damaged by an opponent's attack, 2 damage counters on the attacker for each Metal Energy on this Pokémon.</summary>
        private sealed class OrthwormEx : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                var self = ctx.Pokemon;
                int metal = self.Energy.Count(e => ctx.Engine.EnergyUnitsOf(self, e).Any(u => EnergyMask.Provides(u, EnergyType.Metal)));
                if (metal > 0) ctx.PlaceCounters(attacker, 2 * metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Tomb") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        // ================================================================== Colorless

        private sealed class Talonflame : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aero Chase") return false;
                bool heavy = ctx.Defending != null && ctx.Engine.RetreatCostOf(ctx.Defending) >= 2;
                ctx.DamageDefending(atk.Damage + (heavy ? 110 : 0));
                return true;
            }
        }

        /// <summary>Unified Beatdown: 30 for each of your Benched Pokémon (not on your first turn if you went second). Crown Opal: no damage from Basic non-Colorless Pokémon next turn.</summary>
        private sealed class TerapagosEx : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                if (atk.Name != "Unified Beatdown") return true;
                bool wentSecond = ctx.State.FirstPlayer != ctx.Player;
                return !(wentSecond && ctx.IsMyFirstTurn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Unified Beatdown")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Me.Bench.Count);
                    return true;
                }
                if (atk.Name == "Crown Opal")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new CrownOpalEffect(ctx.Pokemon.Name + " takes no damage from attacks by Basic non-Colorless Pokémon during the opponent's next turn (Crown Opal)",
                            ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                    return true;
                }
                return false;
            }
        }

        private sealed class CrownOpalEffect : ContinuousEffect
        {
            public CrownOpalEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || !info.AgainstOpponent) return damage;
                var a = info.Attacker;
                bool basicNonColorless = a.Def.IsBasicPokemon && !engine.IsType(a, EnergyType.Colorless);
                return basicNonColorless ? 0 : damage;
            }
        }

        // ================================================================== Antique Fossils (Abilities only)

        /// <summary>Protective Cover: prevent all effects of the opponent's attacks done to this Pokémon (not damage).</summary>
        private sealed class AntiqueCoverFossil : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Primal Root: while Active, attacks used by the opponent's Basic Pokémon cost Colorless more.</summary>
        private sealed class AntiqueRootFossil : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive) return;
                if (mon.Owner != src.Player && mon.Def.IsBasicPokemon) cost.Add(EnergyType.Colorless);
            }
        }

        // ================================================================== Trainers

        /// <summary>Deluxe Bomb: when the Active holder is damaged by an opponent's attack, 12 damage counters on the attacker, then discard this card.</summary>
        private sealed class DeluxeBomb : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                int placed = ctx.Engine.PlaceDamageCounters(attacker, 12, "Deluxe Bomb");
                if (placed > 0 && ctx.Pokemon != null && ctx.Pokemon.HasTool(ctx.Card)) ctx.Engine.DiscardTool(ctx.Pokemon, ctx.Card);
            }
        }

        /// <summary>Grand Tree: once during each player's turn, evolve a Basic into a Stage 1 from the deck, then optionally into a Stage 2.</summary>
        private sealed class GrandTree : StadiumAction
        {
            private static List<PokemonInPlay> Basics(EffectContext ctx)
            {
                // Antique Fossils in play are Basic Pokémon. Lunatone's New Moon keeps the Stadium off shielded Pokémon.
                return ctx.MyPokemon(m => m.IsBasic && m.Stack.Count == 1 && !ctx.Engine.StadiumShielded(m));
            }

            protected override bool Ready(EffectContext ctx)
            {
                // Compendium (Stellar Crown FAQ, 2024-09-12): not usable unless a Basic in play has a known Stage 1 to evolve into.
                return ctx.Me.Deck.Count > 0 && !ctx.State.IsPlayersFirstTurn
                    && Basics(ctx).Any(m => m.TurnEnteredPlay < ctx.Turn && ctx.Engine.PreEvolutionOf.ContainsValue(m.Name));
            }

            protected override bool Use(EffectContext ctx)
            {
                var basics = Basics(ctx);
                var stage1 = ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage1 && basics.Any(m => ctx.Engine.CanEvolveInto(m, d)), 1,
                    "Grand Tree: choose a Stage 1 Pokémon that evolves from 1 of your Basic Pokémon");
                if (stage1.Count == 0)
                {
                    ctx.ShuffleMyDeck();
                    return false;
                }
                var card = stage1[0];
                var targets = basics.Where(m => ctx.Engine.CanEvolveInto(m, card.Def)).ToList();
                var target = ctx.ChoosePokemon(targets, "Grand Tree: evolve which Pokémon into " + card.Def.Name + "?");
                if (target == null)
                {
                    ctx.Me.Deck.Add(card);
                    ctx.ShuffleMyDeck();
                    return false;
                }
                ctx.Engine.EvolvePokemon(target, card, false);
                if (ctx.Me.Deck.Count > 0)
                {
                    string from = target.Name;
                    var stage2 = ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage2 && d.EvolvesFrom == from, 1,
                        "Grand Tree: you may choose a Stage 2 Pokémon that evolves from " + from);
                    if (stage2.Count > 0) ctx.Engine.EvolvePokemon(target, stage2[0], false);
                }
                ctx.ShuffleMyDeck();
                return false;
            }
        }

        /// <summary>Gravity Gemstone: while the holder is Active, both Active Pokémon's Retreat Cost is Colorless more.</summary>
        private sealed class GravityGemstone : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || !src.State.IsActive(mon)) return cost;
                return cost + 1;
            }
        }

        /// <summary>Kofu: put 2 cards from your hand on the bottom of your deck, then draw 4.</summary>
        private sealed class Kofu : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count < 2) return;
                for (int i = 0; i < 2; i++)
                {
                    var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Kofu: choose a card to put on the bottom of your deck (" + (i + 1) + " of 2)")[0];
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 2 cards on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck.");
                ctx.Draw(4);
            }
        }

        /// <summary>Lacey: shuffle your hand into your deck and draw 4, or 8 if your opponent has 3 or fewer Prize cards left.</summary>
        private sealed class Lacey : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Opp.Prizes.Count <= 3 ? 8 : 4);
            }
        }

        /// <summary>Type berries: damage from an opponent's [type] Pokémon's attack is 60 less, then the berry is discarded.</summary>
        private sealed class TypeBerry : CardScript
        {
            private readonly EnergyType type;
            private readonly string name;

            public TypeBerry(EnergyType type, string name)
            {
                this.type = type;
                this.name = name;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || damage <= 0) return damage;
                if (!src.Engine.IsType(info.Attacker, type) || !holder.HasTool(src.Card)) return damage;
                src.Log(name + " reduced the damage to " + holder.Name + " by 60.");
                src.Engine.DiscardTool(holder, src.Card);
                return damage - 60;
            }
        }

        /// <summary>Sparkling Crystal: attacks of the Tera Pokémon it's attached to cost 1 Energy less (any type).</summary>
        private sealed class SparklingCrystal : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !mon.Def.IsTera || cost.Count == 0) return;
                // Remove the symbol that helps most: one whose removal makes the attack payable, else a Colorless one.
                var units = src.Engine.EnergyUnits(mon);
                foreach (var t in cost.Distinct().ToList())
                {
                    var trial = new List<EnergyType>(cost);
                    trial.Remove(t);
                    if (EnergyMask.CanPay(units, trial))
                    {
                        cost.Remove(t);
                        return;
                    }
                }
                if (!cost.Remove(EnergyType.Colorless)) cost.RemoveAt(cost.Count - 1);
            }
        }
    }
}
