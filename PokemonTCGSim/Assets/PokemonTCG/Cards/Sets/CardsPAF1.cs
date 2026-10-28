using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paldean Fates (PAF) 1-91 and the Trainers from the rest of the set: the cards that weren't already in the pool. Every
    /// card is G-mark (Expanded only). Reprints of cards already in the pool (Pineco, Pikachu, Xatu, Mimikyu, Fidough,
    /// Greavard, Great Tusk ex, Charizard ex, Maschiff, Revavroom, Iron Treads ex, Squawkabilly ex, Iono, Professor's Research,
    /// Rare Candy, Ultra Ball, Judge...) are alternate prints of those cards. Pokémon whose attacks only do their printed
    /// damage need no script: Charcadet, Ralts, Whimsicott, Phanpy, Scraggy, Noibat.
    /// The shiny Pokémon from 92 on are in CardsPAF2.
    /// </summary>
    public static class CardsPAF1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAF-2", new ForretressExPAF());
            r.RegisterByKey("PAF-3", new ComboMon(new DrawAttackMon("Double Draw", 2), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("PAF-4", new CoinsMon("Furious Kicks", 3));
            r.RegisterByKey("PAF-5", new ToedscruelExPAF());
            r.RegisterByKey("PAF-6", new EspathraExPAF());
            r.RegisterByKey("PAF-7", new DiscardStadiumMon("Blazing Destruction"));
            r.RegisterByKey("PAF-8", new CharmeleonPAF());
            r.RegisterByKey("PAF-9", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("PAF-10", new LockAfterAttackMon("Volcanic Heat", false));
            r.RegisterByKey("PAF-11", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("PAF-12", new CameruptPAF());
            r.RegisterByKey("PAF-13", new RecoilMon("Heat Tackle", 40));
            r.RegisterByKey("PAF-15", new ArmarougePAF());
            r.RegisterByKey("PAF-16", new ComboMon(new BasicEnergyFromDiscardMon("Into the Deep", 3), new ParalyzeFlipMon("Freeze-Dry")));
            r.RegisterByKey("PAF-17", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("PAF-19", new RaichuPAF());
            r.RegisterByKey("PAF-20", new ChinchouPAF());
            r.RegisterByKey("PAF-21", new Cards30C.TypeEnergyBonusMon("Aqua Spark", EnergyType.Water, 120));
            r.RegisterByKey("PAF-22", new UnitedWingsMon());
            r.RegisterByKey("PAF-23", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("PAF-24", new ExeggutorPAF());
            r.RegisterByKey("PAF-25", new CoinsMon("Triple Strike", 3));
            r.RegisterByKey("PAF-28", new CardsMEG.DefendingEnergyBonusMon("Psychic", 20));
            r.RegisterByKey("PAF-29", new GardevoirExPAF());
            r.RegisterByKey("PAF-30", new ChimechoPAF());
            r.RegisterByKey("PAF-31", new MimeJrPAF());
            r.RegisterByKey("PAF-32", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PAF-33", new SwoobatPAF());
            r.RegisterByKey("PAF-34", new TailsNothingMon("Fickle Attack"));
            r.RegisterByKey("PAF-36", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2), new SwitchSelfMon("Spin Turn", false)));
            r.RegisterByKey("PAF-39", new DachsbunPAF());
            r.RegisterByKey("PAF-40", new CeruledgePAF());
            r.RegisterByKey("PAF-41", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PAF-43", new HoundstonePAF());
            r.RegisterByKey("PAF-44", new DodgeMon("Chest-ouflage", false));
            r.RegisterByKey("PAF-45", new RecoilMon("Monkey Beatdown", 10));
            r.RegisterByKey("PAF-46", new RecoilMon("Raging Punch", 20));
            r.RegisterByKey("PAF-47", new AnnihilapePAF());
            r.RegisterByKey("PAF-49", new ComboMon(new CardsSSP2.StompOffMon("Cragalanche", 2), new LockAfterAttackMon("Powerful Spin", false)));
            r.RegisterByKey("PAF-50", new DodgeMon("Hide", false));
            r.RegisterByKey("PAF-51", new CardsFLF1.NoResistanceMon("Feint"));
            r.RegisterByKey("PAF-52", new GrapploctPAF());
            r.RegisterByKey("PAF-55", new DrawAttackMon("Allure", 1));
            r.RegisterByKey("PAF-56", new ConditionAttackMon("Dark Slumber", SpecialCondition.Asleep));
            r.RegisterByKey("PAF-57", new GengarPAF());
            r.RegisterByKey("PAF-58", new SearchAttackMon("Find a Home", d => d.IsStadiumCard, 1));
            r.RegisterByKey("PAF-59", new PaldeanClodsireExPAF());
            r.RegisterByKey("PAF-61", new ScraftyPAF());
            r.RegisterByKey("PAF-63", new MabosstiffPAF());
            r.RegisterByKey("PAF-64", new BasicEnergyFromDiscardMon("Rev", 2));
            r.RegisterByKey("PAF-67", new GholdengoPAF());
            r.RegisterByKey("PAF-69", new NoivernExPAF());
            r.RegisterByKey("PAF-70", new DodgeMon("Acceleration Drive", false));
            r.RegisterByKey("PAF-71", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("PAF-72", new OinkolognePAF());
            r.RegisterByKey("PAF-73", new DrawAttackMon("Collect", 2));
            r.RegisterByKey("PAF-74", new MausholdPAF());

            r.RegisterByName("Artazon", new Artazon());
            r.RegisterByName("Atticus", new Atticus());
            r.RegisterByName("Clive", new Clive());
            r.RegisterByName("Electric Generator", new ElectricGenerator());
            r.RegisterByName("Moonlit Hill", new MoonlitHill());
            r.RegisterByName("Nemona", new Nemona());
            r.RegisterByName("Nemona's Backpack", new NemonasBackpack());
            r.RegisterByName("Nest Ball", new NestBall());
            r.RegisterByName("Paldean Student", new PaldeanStudent());
            r.RegisterByName("Technical Machine: Crisis Punch", new TechnicalMachineCrisisPunch());
            r.RegisterByName("Arven", new Arven());
            r.RegisterByName("Penny", new Penny());
        }

        // ================================================================== Shared helpers

        /// <summary>Pokémon in your discard pile that have the United Wings attack.</summary>
        public static int UnitedWingsInDiscard(EffectContext ctx)
        {
            return ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => a.Name == "United Wings"));
        }

        /// <summary>
        /// United Wings: 20 for each Pokémon in your discard pile that has the United Wings attack. United Thunder
        /// (Kilowattrel): 10 for each of them to 1 of your opponent's Benched Pokémon.
        /// </summary>
        public sealed class UnitedWingsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "United Wings")
                {
                    ctx.DamageDefending(atk.Damage * UnitedWingsInDiscard(ctx));
                    return true;
                }
                if (atk.Name == "United Thunder")
                {
                    int n = UnitedWingsInDiscard(ctx);
                    if (n > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10 * n);
                    return true;
                }
                return false;
            }
        }

        /// <summary>"Discard a Stadium in play."</summary>
        public sealed class DiscardStadiumMon : Mon
        {
            private readonly string attack;

            public DiscardStadiumMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        /// <summary>"Put up to N Basic Energy cards from your discard pile into your hand" (Into the Deep, Rev, Berry Search).</summary>
        public sealed class BasicEnergyFromDiscardMon : Mon
        {
            private readonly string attack;
            private readonly int max;

            public BasicEnergyFromDiscardMon(string attack, int max)
            {
                this.attack = attack;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 0, Math.Min(max, pile.Count), atk.Name + ": put up to " + max + " Basic Energy cards from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>"Put this Pokémon and all attached cards into your hand" after the damage (Fade Out, Curl Up).</summary>
        public sealed class ReturnSelfMon : Mon
        {
            private readonly string attack;

            public ReturnSelfMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>"As often as you like during your turn, you may move a [T] Energy from 1 of your Benched Pokémon to your Active Pokémon."</summary>
        public static bool CanMoveTypedEnergyToActive(EffectContext ctx, EnergyType type)
        {
            return ctx.Me.Active != null && ctx.Me.Bench.Any(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, type)));
        }

        public static void MoveTypedEnergyToActive(EffectContext ctx, EnergyType type, string ability)
        {
            RepeatableAbility.Repeat(ctx, () => CanMoveTypedEnergyToActive(ctx, type), offerDone =>
            {
                var active = ctx.Me.Active;
                var holders = ctx.Me.Bench.Where(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, type))).ToList();
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, ability + ": move a " + type + " Energy from which Benched Pokémon to "
                                                                 + active.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var energy = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, type)).ToList();
                var card = energy.Select(e => e.Def.Name).Distinct().Count() == 1 ? energy[0]
                    : ctx.ChooseCards(energy, 1, 1, ability + ": move which Energy from " + from.Name + "?")[0];
                return ctx.Engine.MoveEnergy(from, active, card);
            });
        }

        /// <summary>Basic Pokémon in play that can be put into the hand (Penny). An Active one only if there's a Bench to promote from.</summary>
        private static List<PokemonInPlay> PennyTargets(EffectContext ctx)
        {
            return ctx.MyPokemon(m => m.IsBasic && m.Stack.Count == 1 && ctx.Engine.CanPutIntoHand(m) && (!ctx.IsActive(m) || ctx.Me.Bench.Count > 0));
        }

        // ================================================================== Grass

        /// <summary>
        /// Exploding Energy: search for up to 5 Basic Grass Energy, attach them to your Pokémon in any way you like, shuffle;
        /// if you searched, this Pokémon is Knocked Out. Guard Press: takes 30 less next turn.
        /// </summary>
        private sealed class ForretressExPAF : Mon
        {
            private static readonly Mon Press = new CardsASC.SelfGuardMon("Guard Press", 30);

            protected override string Activated { get { return "Exploding Energy"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Exploding Energy"; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 5, "Exploding Energy: choose up to 5 Basic Grass Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => true, "Exploding Energy");
                ctx.ShuffleMyDeck();
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.KnockOut(ctx.Pokemon, "Exploding Energy");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Press.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Protective Mycelium: prevent all effects of the opponent's attacks on your Pokémon that have Energy attached.
        /// Colony Rush: 40 more for each of your Benched Pokémon with any Grass Energy attached.
        /// </summary>
        private sealed class ToedscruelExPAF : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && target != null && target.Owner == src.Player && target.Energy.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colony Rush") return false;
                int n = ctx.Me.Bench.Count(m => ctx.Engine.HasEnergyOfType(m, EnergyType.Grass));
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>Dazzling Gaze: while Active, the opponent's Active Pokémon's attacks cost 1 Colorless more. Psy Ball: 30 more per Energy on both Active Pokémon.</summary>
        private sealed class EspathraExPAF : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || mon == null || mon.Owner == src.Player || !src.State.IsActive(mon)) return;
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psy Ball") return false;
                int n = ctx.EnergyCount(ctx.Pokemon) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Flare Veil: prevent all effects of the opponent's attacks on this Pokémon.</summary>
        private sealed class CharmeleonPAF : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && target == src.Pokemon;
            }
        }

        /// <summary>Super Singe: Burned. Cinder Cannon: you may discard a Fighting Energy from this Pokémon for 120 more.</summary>
        private sealed class CameruptPAF : Mon
        {
            private static readonly Mon Singe = new ConditionAttackMon("Super Singe", SpecialCondition.Burned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cinder Cannon") return Singe.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                var fighting = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fighting)).ToList();
                bool bonus = false;
                if (fighting.Count > 0 && ctx.AskYesNo("Cinder Cannon: discard a Fighting Energy from " + me.Name + " to do 120 more damage?"))
                {
                    var pick = ctx.ChooseCards(fighting, 1, 1, "Cinder Cannon: discard which Energy?")[0];
                    bonus = ctx.Engine.DiscardEnergy(me, pick);
                }
                ctx.DamageDefending(atk.Damage + (bonus ? 120 : 0));
                return true;
            }
        }

        /// <summary>Fire Off: as often as you like, move a Fire Energy from 1 of your Benched Pokémon to your Active Pokémon. Flame Cannon: Burned.</summary>
        private sealed class ArmarougePAF : Mon
        {
            private static readonly Mon Cannon = new ConditionAttackMon("Flame Cannon", SpecialCondition.Burned);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Fire Off" && ctx.IsMyTurn && CanMoveTypedEnergyToActive(ctx, EnergyType.Fire);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { MoveTypedEnergyToActive(ctx, EnergyType.Fire, "Fire Off"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cannon.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Electrical Grounding: when 1 of your other Pokémon is Knocked Out by damage from the opponent's attack, you may move a
        /// Lightning Energy from it to this Pokémon. Thunder: 50 to itself.
        /// </summary>
        private sealed class RaichuPAF : Mon
        {
            private static readonly Mon Thunder = new RecoilMon("Thunder", 50);

            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null || mon == me || !src.InPlay(me) || src.Engine.IsKnockedOut(me)) return;
                var lightning = mon.Energy.Where(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return;
                if (!src.AskYesNo("Electrical Grounding: move a Lightning Energy from " + mon.Name + " to " + me.Name + "?")) return;
                var pick = src.ChooseCards(lightning, 1, 1, "Electrical Grounding: move which Energy?")[0];
                Announce(src, "Electrical Grounding");
                src.Engine.MoveEnergy(mon, me, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Thunder.HandleAttack(ctx, atk); }
        }

        /// <summary>Shine On: look at the top card of your deck; you may put it on the bottom.</summary>
        private sealed class ChinchouPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shine On") return false;
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return true;
                if (ctx.AskYesNo("Shine On: the top card of your deck is " + top[0].Def.Name + ". Put it on the bottom of your deck?"))
                {
                    ctx.Me.Deck.Remove(top[0]);
                    ctx.Me.Deck.Add(top[0]);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top card of their deck on the bottom.");
                }
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Hypnoblast: Asleep. Psychic Loop: put 2 Energy attached to this Pokémon into your hand.</summary>
        private sealed class ExeggutorPAF : Mon
        {
            private static readonly Mon Hypnoblast = new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Loop") return Hypnoblast.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                var picks = me.Energy.Count <= 2 ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), 2, 2, "Psychic Loop: put which 2 Energy into your hand?");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                return true;
            }
        }

        /// <summary>
        /// Psychic Embrace: as often as you like, attach a Basic Psychic Energy from your discard pile to 1 of your Psychic Pokémon
        /// and put 2 damage counters on it (not on a Pokémon that would be Knocked Out). Miracle Force: recovers from all Special Conditions.
        /// </summary>
        private sealed class GardevoirExPAF : Mon
        {
            private static List<PokemonInPlay> EmbraceTargets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Psychic) && ctx.RemainingHp(m) > 20);
            }

            private static bool CanEmbrace(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)) && EmbraceTargets(ctx).Count > 0;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Psychic Embrace" && ctx.IsMyTurn && CanEmbrace(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanEmbrace(ctx), offerDone =>
                {
                    var target = RepeatableAbility.ChoosePokemonOrDone(ctx, EmbraceTargets(ctx),
                        "Psychic Embrace: attach a Basic Psychic Energy from your discard pile to which Pokémon (2 damage counters)?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (target == null) return false;
                    var card = ctx.Me.Discard.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic));
                    if (card == null || !ctx.Me.Discard.Remove(card)) return false;
                    ctx.Engine.AttachEnergyCard(card, target);
                    ctx.Engine.PlaceDamageCounters(target, 2, "Psychic Embrace");
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miracle Force") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Sleep Inducer: switch in 1 of the opponent's Benched Pokémon; the new Active Pokémon is Asleep.</summary>
        private sealed class ChimechoPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Inducer") return false;
                var pulled = ctx.GustOpponent("Sleep Inducer: switch in which of your opponent's Benched Pokémon?");
                if (pulled != null && ctx.InPlay(pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Mimed Games: the opponent chooses an attack of 1 of their Pokémon in play; Mime Jr. uses it as this attack. They can't
        /// choose an attack they couldn't use (one whose condition isn't met) or their own Mimed Games; with nothing else to
        /// choose, it does nothing (Compendium, 2024-02-01).
        /// </summary>
        private sealed class MimeJrPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mimed Games") return false;
                var opts = new List<DecisionOption>();
                var sources = new List<KeyValuePair<CardDef, int>>();
                foreach (var m in ctx.Opp.InPlay())
                {
                    var owner = new EffectContext(ctx.Engine, m.Owner, m.Top, m);
                    var script = ctx.Engine.Scripts.For(m.Def);
                    for (int i = 0; i < m.Def.Attacks.Count; i++)
                    {
                        var a = m.Def.Attacks[i];
                        if (a.Name == "Mimed Games" || !script.CanUseAttack(owner, i)) continue;
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        if (ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                        opts.Add(new DecisionOption { Label = m.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = m.Def, Index = opts.Count });
                        sources.Add(new KeyValuePair<CardDef, int>(m.Def, i));
                    }
                }
                if (opts.Count == 0)
                {
                    ctx.Log("Mimed Games: there was no attack to choose.");
                    return true;
                }
                int pick = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption,
                    "Mimed Games: choose an attack from 1 of your Pokémon in play for " + ctx.Engine.Possessive(ctx.Player) + " Mime Jr. to use", opts, ctx.Source, false);
                UseCopiedAttack(ctx, sources[pick].Key, sources[pick].Value);
                return true;
            }
        }

        /// <summary>Attraction Wave: Confused, with 6 damage counters instead of 3 for this Confusion.</summary>
        private sealed class SwoobatPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attraction Wave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Confused))
                    defending.ConfusionCounters = 6;
                return true;
            }
        }

        /// <summary>Well-Baked Body: can't be Burned; prevent all damage done to it by the opponent's Fire Pokémon's attacks.</summary>
        private sealed class DachsbunPAF : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & SpecialCondition.Burned) != 0;
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && src.Engine.IsType(info.Attacker, EnergyType.Fire);
            }
        }

        /// <summary>Life Sucker: heal 30 from this Pokémon. Fighting Sword: 100 more against a Pokémon ex or Pokémon V.</summary>
        private sealed class CeruledgePAF : Mon
        {
            private static readonly Mon Sucker = new CardsTWM1.HealSelfMon("Life Sucker", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fighting Sword") return Sucker.HandleAttack(ctx, atk);
                bool big = ctx.Defending != null && TefPokemon.IsExOrV(ctx.Defending.Def);
                ctx.DamageDefending(atk.Damage + (big ? 100 : 0));
                return true;
            }
        }

        /// <summary>Last Respects: 10 more for each Psychic Pokémon in your discard pile.</summary>
        private sealed class HoundstonePAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Last Respects") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Psychic));
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Rage Fist: 70 for each Prize card your opponent has taken. Dynamite Punch: 50 to itself.</summary>
        private sealed class AnnihilapePAF : Mon
        {
            private static readonly Mon Punch = new RecoilMon("Dynamite Punch", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rage Fist") return Punch.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage * ctx.OpponentPrizesTaken);
                return true;
            }
        }

        /// <summary>Slow-Acting Syncope: at the end of the opponent's next turn, the Defending Pokémon is Knocked Out.</summary>
        private sealed class GrapploctPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slow-Acting Syncope") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of its next turn (Slow-Acting Syncope)",
                    ctx.Player, defending.Uid, ctx.Turn, "Slow-Acting Syncope", 0));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Night Gate: once during your turn, switch your Active Pokémon with 1 of your Benched Pokémon. Nightmare: Asleep.</summary>
        private sealed class GengarPAF : Mon
        {
            private static readonly Mon Nightmare = new ConditionAttackMon("Nightmare", SpecialCondition.Asleep);

            protected override string Activated { get { return "Night Gate"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.SwitchMyActive("Night Gate: switch your Active Pokémon with which Benched Pokémon?"); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Nightmare.HandleAttack(ctx, atk); }
        }

        /// <summary>Toxic Wetland: once during your turn, if a Stadium is in play, the opponent's Active Pokémon is Poisoned. Needle Bone: on tails, can't attack next turn.</summary>
        private sealed class PaldeanClodsireExPAF : Mon
        {
            protected override string Activated { get { return "Toxic Wetland"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.State.Stadium != null && ctx.Opp.Active != null; }
            protected override void Ability(EffectContext ctx) { ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Poisoned); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Needle Bone") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Lambaste: if the Defending Pokémon is a Basic Pokémon, it can't attack during the opponent's next turn.</summary>
        private sealed class ScraftyPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lambaste") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !defending.IsBasic || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its owner's next turn (Lambaste)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Intimidating Howl: once during your turn, switch out the opponent's Active Pokémon (they choose the new one). Wild Tackle: 30 to itself.</summary>
        private sealed class MabosstiffPAF : Mon
        {
            private static readonly Mon Tackle = new RecoilMon("Wild Tackle", 30);

            protected override string Activated { get { return "Intimidating Howl"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Lavish Hospitality: attach any number of Basic Metal Energy from your hand to your Pokémon. Scintillating Surfing: a coin per Metal Energy, 80 each heads.</summary>
        private sealed class GholdengoPAF : Mon
        {
            private static readonly Mon Surfing = new CardsPOR.FlipPerTypedEnergyMon("Scintillating Surfing", EnergyType.Metal);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lavish Hospitality") return Surfing.HandleAttack(ctx, atk);
                CardsSSP2.AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Metal), 99, m => true, "Lavish Hospitality");
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Covert Flight: prevent all damage from Basic Pokémon's attacks next turn. Dominating Echo: during the opponent's next
        /// turn, they can't play Special Energy or Stadium cards from their hand.
        /// </summary>
        private sealed class NoivernExPAF : Mon
        {
            private static readonly Mon Flight = new CardsCRI.BasicShieldAttackMon("Covert Flight");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dominating Echo") return Flight.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsMEG.StadiumLockEffect("Dominating Echo: no Stadium cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                ctx.AddEffect(new SpecialEnergyLockEffect("Dominating Echo: no Special Energy cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>The player can't play Special Energy cards from their hand.</summary>
        public sealed class SpecialEnergyLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public SpecialEnergyLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card.Def.IsSpecialEnergy;
            }
        }

        // ================================================================== Colorless

        /// <summary>Finest Selection: flip 3 coins; put up to that many cards from your discard pile into your hand. Perfume Press: Confused.</summary>
        private sealed class OinkolognePAF : Mon
        {
            private static readonly Mon Perfume = new ConditionAttackMon("Perfume Press", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Finest Selection") return Perfume.HandleAttack(ctx, atk);
                int heads = ctx.FlipCoins(3);
                var pile = ctx.Me.Discard.ToList();
                if (heads == 0 || pile.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 0, Math.Min(heads, pile.Count), "Finest Selection: put up to " + heads + " cards from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Family Attack: 70 for each of your Maushold in play.</summary>
        private sealed class MausholdPAF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Family Attack") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.InPlay().Count(m => m.Name == "Maushold"));
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Artazon: once during each player's turn, they may put a Basic Pokémon without a Rule Box from their deck onto their Bench.</summary>
        private sealed class Artazon : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToBench(d => !d.HasRuleBox, 1, "Artazon: put a Basic Pokémon that doesn't have a Rule Box onto your Bench");
                return false;
            }
        }

        /// <summary>Atticus: only if the opponent's Active Pokémon is Poisoned. Shuffle your hand into your deck and draw 7.</summary>
        private sealed class Atticus : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Opp.Active.Has(SpecialCondition.Poisoned);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(7);
            }
        }

        /// <summary>Clive: the opponent reveals their hand; draw 2 cards for each Supporter in it.</summary>
        private sealed class Clive : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                int supporters = ctx.Opp.Hand.Count(c => c.Def.IsSupporter);
                if (supporters > 0) ctx.Draw(2 * supporters);
            }
        }

        /// <summary>Electric Generator: look at the top 5 cards; attach up to 2 Basic Lightning Energy there to your Benched Lightning Pokémon; shuffle the rest back.</summary>
        private sealed class ElectricGenerator : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(5);
                var pile = top.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (pile.Count > 0)
                    ctx.AttachCardsAnyWay(pile, 2, (c, m) => !ctx.IsActive(m) && ctx.Engine.IsType(m, EnergyType.Lightning), "Electric Generator", true);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Moonlit Hill: once during each player's turn, they may discard a Basic Psychic Energy from their hand to heal 30 from
        /// each of their Pokémon. The discard is a cost, so not with nothing to heal (Compendium, 2024-02-15).
        /// </summary>
        private sealed class MoonlitHill : StadiumAction
        {
            protected override bool Ready(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)) && CardsMEG.AnyHurt(ctx);
            }

            protected override bool Use(EffectContext ctx)
            {
                var psychic = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)).ToList();
                if (psychic.Count == 0) return false;
                ctx.Engine.DiscardFromHand(ctx.Player, psychic[0]);
                foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 30);
                return false;
            }
        }

        /// <summary>Nemona: draw 3 cards.</summary>
        private sealed class Nemona : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Nemona's Backpack: put up to 2 Nemona cards from your discard pile into your hand.</summary>
        private sealed class NemonasBackpack : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.Name == "Nemona"); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var nemona = ctx.Me.Discard.Where(c => c.Def.Name == "Nemona").ToList();
                if (nemona.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(nemona, 1, Math.Min(2, nemona.Count), "Nemona's Backpack: put up to 2 Nemona cards into your hand"));
            }
        }

        /// <summary>Nest Ball: put a Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class NestBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToBench(d => true, 1, "Nest Ball: put a Basic Pokémon onto your Bench");
            }
        }

        /// <summary>Paldean Student: search for a Pokémon without a Rule Box, plus 1 more for each other Paldean Student in your discard pile.</summary>
        private sealed class PaldeanStudent : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int extra = ctx.Me.Discard.Count(c => c.Def.Name == "Paldean Student" && c != ctx.Card);
                int max = 1 + extra;
                ctx.SearchToHand(d => d.IsPokemon && !d.HasRuleBox, max,
                    "Paldean Student: search your deck for up to " + max + " Pokémon that don't have a Rule Box");
            }
        }

        /// <summary>Arven: search your deck for an Item card and a Pokémon Tool card.</summary>
        private sealed class Arven : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var item = ctx.SearchMyDeck(d => d.IsItem, 1, "Arven: choose an Item card");
                var tool = ctx.SearchMyDeck(d => d.IsTool, 1, "Arven: choose a Pokémon Tool card");
                ctx.Me.Hand.AddRange(item);
                ctx.Me.Hand.AddRange(tool);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Penny: put 1 of your Basic Pokémon and all attached cards into your hand.</summary>
        private sealed class Penny : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return PennyTargets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = PennyTargets(ctx);
                if (targets.Count == 0) return;
                var pick = ctx.ChoosePokemon(targets, "Penny: put which Basic Pokémon and all attached cards into your hand?");
                if (pick != null) ctx.Engine.ReturnPokemonToHand(pick);
            }
        }

        /// <summary>Technical Machine: Crisis Punch: the holder can use Crisis Punch (280) only when the opponent has exactly 1 Prize card left. Discarded at the end of your turn.</summary>
        private sealed class TechnicalMachineCrisisPunch : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex) { return ctx.Opp.Prizes.Count == 1; }

            /// <summary>Discarded at the end of your turn by the engine, even while Tools have no effect (your ruling, v97).</summary>
            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                ctx.DamageDefending(ctx.Attack(attackIndex).Damage);
            }
        }
    }
}
