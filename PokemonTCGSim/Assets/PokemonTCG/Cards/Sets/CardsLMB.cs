using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legend Maker Uncommon and Common Pokémon (LM 29-71; v170). Chinchou 50 is Hidden Legends' (a print); Shuppet 63 and Spheal
    /// 65 have no card text. Compendium rulings: Flick Poison does nothing without an opponent's Benched Pokémon ("the new Defending
    /// Pokémon"); Pokémon put onto the Bench by Influence or Strange Cave are Basic Pokémon (a Stage 1 Pokémon that isn't evolved);
    /// Plunder leaves Fossils alone (they aren't attached to anything). 1-on-1: "1 of the Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsLMB
    {
        /// <summary>The Pokémon Influence and Strange Cave put onto the Bench as Basic Pokémon.</summary>
        internal static readonly string[] Ancients = { "Omanyte", "Kabuto", "Aerodactyl", "Lileep", "Anorith" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LM-29", new ComboMon(new InfluenceMon(), new StretchClawsMon()));
            r.RegisterByKey("LM-30", new CascoonEmerge());
            r.RegisterByKey("LM-31", new DunsparceDeadlock());
            r.RegisterByKey("LM-32", new ComboMon(new ConditionAttackMon("Supersonic", SpecialCondition.Confused), new FlipBonusMon("Tumbling Attack", 20)));
            r.RegisterByKey("LM-33", new ComboMon(new ParalyzeFlipMon("Body Slam"), new CardsASR1.PlusMon("Do the Wave", 10, c => c.Me.Bench.Count)));
            r.RegisterByKey("LM-34", new CardsFST1.TakeLessMon(10, new CardsFST1.TwoBenchedMon("Rock Slide", 10)));
            r.RegisterByKey("LM-35", new ComboMon(new CardsSSB.TimeSpiralFlipMon(), new HauntMon()));
            r.RegisterByKey("LM-36", new KabutoAncientProtection());
            r.RegisterByKey("LM-37", new KecleonReactiveColors());
            r.RegisterByKey("LM-38", new ComboMon(new RestMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("LM-39", new MachokeParanoid());
            r.RegisterByKey("LM-40", new MisdreavusDeepSleep());
            r.RegisterByKey("LM-41", new CardsUF.PlunderMon());
            r.RegisterByKey("LM-42", new RoseliaReactiveAroma());
            r.RegisterByKey("LM-43", new SealeoPowerCirculation());
            r.RegisterByKey("LM-44", new TangelaReactiveHealing());
            r.RegisterByKey("LM-45", new TentacruelReactiveShield());
            r.RegisterByKey("LM-46", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused),
                new CardsASR1.PlusMon("Bi-Blast", 20, c => CardsLM.HasReact(c.Pokemon) ? 1 : 0)));
            r.RegisterByKey("LM-47", new ConditionAttackMon("Sleep Seed", SpecialCondition.Asleep));
            r.RegisterByKey("LM-48", new CardsMAB.AttachToSelfFromHandMon("Self Charge", d => d.IsEnergy && d.Name == "Metal Energy"));
            r.RegisterByKey("LM-49", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("LM-51", new CardsFCO2.FlipProtectMon("Iron Defense"));
            r.RegisterByKey("LM-52", new ComboMon(new ParalyzeFlipMon("Lick"), new ConditionAttackMon("Smog", SpecialCondition.Poisoned)));
            r.RegisterByKey("LM-53", new CardsBS1.SelfdestructMon("Selfdestruct", 10, 50));
            r.RegisterByKey("LM-54", new CardsSIT1.AscensionMon());
            r.RegisterByKey("LM-55", new ComboMon(new ParalyzeFlipMon("Body Slam"), new FlipBonusMon("Firebreathing", 20)));
            r.RegisterByKey("LM-56", new ComboMon(new TenderTentaclesMon(), new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("LM-57", new Cards30CC.BenchHitMon("Mach Punch", 10));
            r.RegisterByKey("LM-58", new CardsSS.BabyEvolutionMon("Magmar", new IgniteMon()));
            r.RegisterByKey("LM-59", new MagneticSwirlMon());
            r.RegisterByKey("LM-60", new OmanyteAncientTentacles());
            r.RegisterByKey("LM-61", new ComboMon(new SearchAttackMon("Drawup Power", d => d.IsEnergy, 1), new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble")));
            r.RegisterByKey("LM-62", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("LM-63", new PlainMon());
            r.RegisterByKey("LM-64", new ComboMon(new SearchAttackMon("Important Errands", CardsLM.IsReact, 1), new FlipBonusMon("Jump On", 20)));
            r.RegisterByKey("LM-65", new PlainMon());
            r.RegisterByKey("LM-66", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("LM-67", new ComboMon(new CardsMA.BenchOnlyMon("Pebble Throw", 10), new CoinsMon("Double Pinchers", 2)));
            r.RegisterByKey("LM-68", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new RecoilMon("Bouncing Ball", 10)));
            r.RegisterByKey("LM-69", new ComboMon(new CardsRS.TextDamageMon("Double Tackle", 20), new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool")));
            r.RegisterByKey("LM-70", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, new StringPullMon()));
            r.RegisterByKey("LM-71", new CardsSS.BabyEvolutionMon("Wobbuffet", new CardsCEC1.ConfusionWaveMon("Confusion Wave")));
        }

        // ================================================================== Attacks

        /// <summary>Influence: up to 2 Omanyte, Kabuto, Aerodactyl, Lileep or Anorith from your deck onto your Bench, as Basic Pokémon.</summary>
        private sealed class InfluenceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Influence") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(2, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && Ancients.Contains(d.Name) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Influence: choose up to 2 Omanyte, Kabuto, Aerodactyl, Lileep or Anorith to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace <= 0) { ctx.Me.Deck.Add(card); continue; }
                    var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    if (mon != null) mon.Markers[PokemonInPlay.ConsideredBasicMarker] = 1;
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Stretch Claws: with React Energy on this Pokémon, also 20 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class StretchClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stretch Claws") return false;
                bool react = CardsLM.HasReact(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage);
                if (react && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Haunt: 2 damage counters on the Defending Pokémon.</summary>
        private sealed class HauntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haunt") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.PlaceCounters(defending, 2);
                return true;
            }
        }

        /// <summary>Rest: remove all Special Conditions and 5 damage counters (all if fewer) from this Pokémon; it's now Asleep.</summary>
        private sealed class RestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, 50);
                if (ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Tender Tentacles: with React Energy on this Pokémon, 2 damage counters come off it.</summary>
        private sealed class TenderTentaclesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tender Tentacles") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && CardsLM.HasReact(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>Ignite: if Full Flame is in play, the Defending Pokémon is now Burned.</summary>
        private sealed class IgniteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ignite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool fullFlame = ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == "Full Flame";
                if (fullFlame && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Magnetic Swirl: flip a coin; tails, nothing; heads, the damage and an Energy attached to the Defending Pokémon is discarded.</summary>
        private sealed class MagneticSwirlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Swirl") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Magnetic Swirl: discard an Energy attached to " + defending.Name);
                return true;
            }
        }

        /// <summary>String Pull: switch 1 of the opponent's Benched Pokémon (your choice) with their Active Pokémon.</summary>
        private sealed class StringPullMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "String Pull") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.GustTargets().Count > 0) ctx.GustOpponent("String Pull: switch which of your opponent's Benched Pokémon into the Active Spot?");
                return true;
            }
        }

        // ================================================================== Pokémon with Poké-Powers and Poké-Bodies

        /// <summary>Emerge (Poké-Power): once during your turn, if Cascoon is Active, flip a coin; heads, evolve it with a card from your deck.</summary>
        private sealed class CascoonEmerge : Mon
        {
            protected override string Activated { get { return "Emerge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) CardsEVS1.EvolveFromDeck(ctx, ctx.Pokemon, "Emerge");
            }
        }

        /// <summary>
        /// Deadlock (Poké-Body): while Dunsparce is your Active Pokémon, your opponent's Dunsparce can't attack. Down Draw: draw 2
        /// from the bottom of your deck. Tripping Turn: Confused, and you may switch Dunsparce with 1 of your Benched Pokémon.
        /// </summary>
        private sealed class DunsparceDeadlock : Mon
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && mon != null && mon.Owner != src.Player && mon.Name == "Dunsparce" && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Down Draw")
                {
                    var deck = ctx.Me.Deck;
                    int n = Math.Min(2, deck.Count);
                    for (int i = 0; i < n; i++)
                    {
                        var card = deck[deck.Count - 1];
                        deck.RemoveAt(deck.Count - 1);
                        ctx.Me.Hand.Add(card);
                    }
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew " + n + (n == 1 ? " card" : " cards") + " from the bottom of the deck.");
                    return true;
                }
                if (atk.Name != "Tripping Turn") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Tripping Turn: switch Dunsparce with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Ancient Protection (Poké-Body): your Omanyte, Omastar, Kabuto, Kabutops and Kabutops ex have no Weakness. Granite Head.</summary>
        private sealed class KabutoAncientProtection : Mon
        {
            private static readonly string[] Names = { "Omanyte", "Omastar", "Kabuto", "Kabutops", "Kabutops ex" };
            private static readonly Mon Granite = new ShieldAttackMon("Granite Head", 10);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Owner == src.Player && Names.Contains(mon.Name); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Granite.HandleAttack(ctx, atk); }
        }

        /// <summary>Reactive Colors (Poké-Body): with React Energy attached, Kecleon is Grass, Fire, Water, Lightning, Psychic and Fighting. Tongue Whip. Quick Attack.</summary>
        private sealed class KecleonReactiveColors : Mon
        {
            private static readonly EnergyType[] Colors = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting };
            private static readonly Mon Attacks = new ComboMon(new CardsSSB.AnyOpponentTextMon("Tongue Whip", 10), new FlipBonusMon("Quick Attack", 20));

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon != src.Pokemon || !CardsLM.HasReact(mon)) return;
                types.Clear();
                types.AddRange(Colors);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Paranoid (Poké-Body): while Machoke is Confused, its attacks do 50 more to the Defending Pokémon (before W/R). Split Kick.</summary>
        private sealed class MachokeParanoid : Mon
        {
            private static readonly Mon Kick = new CardsRS.TextDamageMon("Split Kick", 20);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !me.Has(SpecialCondition.Confused) || !info.ToOpponentsActive) return damage;
                return damage > 0 && src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? damage + 50 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kick.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Deep Sleep (Poké-Body): while Misdreavus is your Active Pokémon, each Asleep Pokémon's owner flips 2 coins between turns;
        /// it wakes only on 2 heads. Return Trance: an Energy card from Misdreavus to your hand; Asleep. Dream Eater.
        /// </summary>
        private sealed class MisdreavusDeepSleep : Mon
        {
            private static readonly Mon DreamEater = new CardsE1.DreamEaterMon();

            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) ? Math.Max(coins, 2) : coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return Trance") return DreamEater.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me) && me.Energy.Count > 0)
                {
                    var card = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Return Trance: return which Energy card to your hand?")[0];
                    ctx.Engine.ReturnEnergyToHand(me, card);
                }
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Reactive Aroma (Poké-Body): with React Energy on Roselia, 1 damage counter comes off each of your Pokémon (not Pokémon-ex)
        /// that has React Energy attached, between turns; only 1 Reactive Aroma each turn. Flick Poison.
        /// </summary>
        private sealed class RoseliaReactiveAroma : Mon
        {
            private const string Body = "Reactive Aroma";

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !CardsLM.HasReact(me) || src.NamedLimitUsedThisTurn(Body)) return;
                var targets = src.MyPokemon(m => !m.Def.IsOldEx && CardsLM.HasReact(m) && m.DamageCounters > 0);
                if (targets.Count == 0) return;
                src.MarkNamedLimit(Body);
                Announce(src, Body);
                foreach (var mon in targets) src.Heal(mon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flick Poison") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.GustTargets().Count == 0) return true;
                var target = ctx.GustOpponent("Flick Poison: switch which of your opponent's Benched Pokémon into the Active Spot?");
                if (target != null && ctx.IsActive(target)) ctx.ApplyCondition(target, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Power Circulation (Poké-Power): once during your turn, a basic Energy card from your discard pile on top of your deck; then 1 damage counter on Sealeo.</summary>
        private sealed class SealeoPowerCirculation : Mon
        {
            private const string Power = "Power Circulation";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.BasicEnergyInDiscard().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.BasicEnergyInDiscard();
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": put which basic Energy card on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, Power, Power + ": the card put on top of your opponent's deck", ctx.Source);
                ctx.Me.Deck.Insert(0, card);
                ctx.Log(Power + ": " + card.Def.Name + " was put on top of " + ctx.Engine.Possessive(ctx.Player) + " deck.");
                ctx.PlaceCounters(ctx.Pokemon, 1);
            }
        }

        /// <summary>
        /// Reactive Healing (Poké-Body): attaching a React Energy card from your hand to Tangela removes all its damage counters.
        /// Entangling Vines: a Basic Defending Pokémon can't attack next turn. Gentle Wrap.
        /// </summary>
        private sealed class TangelaReactiveHealing : Mon
        {
            private static readonly Mon Wrap = new NoRetreatMon("Gentle Wrap");

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !CardsLM.IsReact(energy.Def) || me.DamageCounters == 0) return;
                Announce(src, "Reactive Healing");
                src.Heal(me, me.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entangling Vines") return Wrap.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                bool basic = defending != null && defending.IsBasic;
                ctx.DamageDefending(atk.Damage);
                if (basic && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Entangling Vines)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Reactive Shield (Poké-Body): with React Energy on this Tentacruel, prevent all effects of attacks, including damage, done to
        /// any of your Tentacruel by the opponent's Pokémon-ex. Water Arrow. Fury Strikes.
        /// </summary>
        private sealed class TentacruelReactiveShield : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsSSB.AnyOpponentTextMon("Water Arrow", 20), new CoinsMon("Fury Strikes", 3));

            private static bool Shields(EffectContext src, PokemonInPlay target, PokemonInPlay attacker)
            {
                return src.Pokemon != null && CardsLM.HasReact(src.Pokemon) && target != null && target.Owner == src.Player && target.Name == "Tentacruel"
                    && attacker != null && attacker.Owner != src.Player && attacker.Def.IsOldEx;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Shields(src, info.Target, info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Shields(src, target, src.Engine.CurrentSourcePokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Ancient Tentacles (Poké-Body): damage done by your Omanyte, Omastar, Kabuto, Kabutops and Kabutops ex to the opponent's Pokémon isn't affected by Resistance. Rising Lunge.</summary>
        private sealed class OmanyteAncientTentacles : Mon
        {
            private static readonly string[] Names = { "Omanyte", "Omastar", "Kabuto", "Kabutops", "Kabutops ex" };
            private static readonly Mon Lunge = new FlipBonusMon("Rising Lunge", 20);

            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                return info.Attacker != null && info.Attacker.Owner == src.Player && info.AgainstOpponent && Names.Contains(info.Attacker.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lunge.HandleAttack(ctx, atk); }
        }
    }
}
