using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Prismatic Evolutions (PRE): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Exeggcute, Pinsir, Litleo, Slowpoke, Spritzee, Larvitar, Pupitar, Hippopotas, Hippowdon,
    /// Sneasel, Houndour, Bronzor, Buneary, Lopunny, Noibat.
    /// </summary>
    public static class CardsPRE
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PRE-2", new CardsTWM1.HealSelfMon("Mega Drain", 30));
            r.RegisterByKey("PRE-6", new LeafeonExPRE());
            r.RegisterByKey("PRE-13", new FlareonPRE());
            r.RegisterByKey("PRE-14", new FlareonExPRE());
            r.RegisterByKey("PRE-16", new LockAfterAttackMon("Flame Tackle", false));
            r.RegisterByKey("PRE-19", new SlowkingPRE());
            r.RegisterByKey("PRE-22", new VaporeonPRE());
            r.RegisterByKey("PRE-23", new VaporeonExPRE());
            r.RegisterByKey("PRE-24", new Cards30CC.BenchHitMon("Overrun", 30));
            r.RegisterByKey("PRE-26", new GlaceonExPRE());
            r.RegisterByKey("PRE-28", new RecoilMon("Thunder", 30));
            r.RegisterByKey("PRE-29", new JolteonPRE());
            r.RegisterByKey("PRE-30", new JolteonExPRE());
            r.RegisterByKey("PRE-31", new CardsMEW1.MoreMoonMon("Amp You Very Much"));
            r.RegisterByKey("PRE-33", new EspeonPRE());
            r.RegisterByKey("PRE-34", new EspeonExPRE());
            r.RegisterByKey("PRE-39", new ConditionAttackMon("Perfume Press", SpecialCondition.Confused));
            r.RegisterByKey("PRE-40", new SylveonPRE());
            r.RegisterByKey("PRE-42", new ScreamTailPRE());
            r.RegisterByKey("PRE-49", new GroudonPRE());
            r.RegisterByKey("PRE-50", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("PRE-51", new LucarioExPRE());
            r.RegisterByKey("PRE-56", new SandyShocksExPRE());
            r.RegisterByKey("PRE-59", new UmbreonPRE());
            r.RegisterByKey("PRE-60", new UmbreonExPRE());
            r.RegisterByKey("PRE-63", new HoundoomPRE());
            r.RegisterByKey("PRE-64", new TyranitarExPRE());
            r.RegisterByKey("PRE-67", new CardsPAL2.ArmThrustPracticeMon());
            r.RegisterByKey("PRE-68", new LockAfterAttackMon("Iron Buster", false));
            r.RegisterByKey("PRE-75", new EeveeExPRE());
            r.RegisterByKey("PRE-76", new CoinsMon("Toss-and-Turn Press", 3));
            r.RegisterByKey("PRE-77", new HoothootPRE());
            r.RegisterByKey("PRE-81", new MiltankPRE());
            r.RegisterByKey("PRE-82", new LugiaExPRE());
            r.RegisterByKey("PRE-86", new RegigigasPRE());
            r.RegisterByKey("PRE-87", new ShayminPRE());
            r.RegisterByKey("PRE-88", new CardsDRI2.MudCoatMon(20));
            r.RegisterByKey("PRE-89", new HawluchaPRE());
            r.RegisterByKey("PRE-91", new NoivernExPRE());

            r.RegisterByName("Amarys", new Amarys());
            r.RegisterByName("Earthen Vessel", new DiscardThenSearch(d => d.IsBasicEnergy, 2, "Earthen Vessel: choose up to 2 Basic Energy cards"));
            r.RegisterByName("Friends in Paldea", new DrawSupporter(3));
            r.RegisterByName("Haban Berry", new CardsSSP1.TypeBerry(EnergyType.Dragon));
            r.RegisterByName("Larry's Skill", new LarrysSkill());
            r.RegisterByName("Max Rod", new MaxRod());
            r.RegisterByName("Professor Sada's Vitality", new ProfessorSadasVitality());
            r.RegisterByName("Professor Turo's Scenario", new ProfessorTurosScenario());
            r.RegisterByName("Professor's Research", new ProfessorsResearch());
            r.RegisterByName("Techno Radar", new DiscardThenSearch(d => d.IsPokemon && d.IsFuture, 2, "Techno Radar: choose up to 2 Future Pokémon"));
            r.RegisterByName("Treasure Tracker", new SearchTools());
        }

        private static bool IsExOrMega(CardDef d) { return d.IsEx || d.IsMegaEx; }

        /// <summary>"You may put 2 Energy attached to your opponent's Active Pokémon into their hand."</summary>
        private static void BounceTwoEnergy(EffectContext ctx, PokemonInPlay defending, string attack)
        {
            if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.Engine.CanPutIntoHand(defending)) return;
            if (!ctx.AskYesNo(attack + ": put 2 Energy from " + defending.Name + " into your opponent's hand?")) return;
            var picks = defending.Energy.Count <= 2 ? defending.Energy.ToList()
                : ctx.ChooseCards(defending.Energy.ToList(), 2, 2, attack + ": choose 2 Energy");
            foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
        }

        /// <summary>Search for up to N Basic Energy and attach them to your Pokémon (any way you like, or all to one Pokémon).</summary>
        private static void SearchAndAttach(EffectContext ctx, int max, bool oneTarget, string attack)
        {
            if (ctx.Me.Deck.Count == 0) return;
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, max, attack + ": choose up to " + max + " Basic Energy to attach");
            if (found.Count > 0)
            {
                PokemonInPlay single = oneTarget ? ctx.ChoosePokemon(ctx.MyPokemon(), attack + ": attach them to which of your Pokémon?") : null;
                foreach (var c in found)
                {
                    var target = single ?? ctx.ChoosePokemon(ctx.MyPokemon(), attack + ": attach " + c.Def.Name + " to which Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(c, target);
                    else ctx.Me.Deck.Add(c);
                }
            }
            ctx.ShuffleMyDeck();
        }

        /// <summary>"Does N more damage if the opponent's Active Pokémon is a Pokémon ex or Pokémon V" (Fighting Blaze and friends).</summary>
        private static void ExOrVBonus(EffectContext ctx, AttackDef atk, int bonus)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage + (defending != null && TefPokemon.IsExOrV(defending.Def) ? bonus : 0));
        }

        // ================================================================== Grass

        /// <summary>Verdant Storm: 60 per Energy on all the opponent's Pokémon. Moss Agate: heal 100 from each of your Benched Pokémon.</summary>
        private sealed class LeafeonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Verdant Storm")
                {
                    ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m)));
                    return true;
                }
                if (atk.Name == "Moss Agate")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var b in ctx.MyBench()) ctx.Heal(b, 100);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Destructive Flame: flip, discard an Energy. Fighting Blaze: +90 against a Pokémon ex or V.</summary>
        private sealed class FlareonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Destructive Flame")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                        ctx.DiscardEnergyFrom(defending, 1, "Destructive Flame: discard an Energy from " + defending.Name);
                    return true;
                }
                if (atk.Name == "Fighting Blaze")
                {
                    ExOrVBonus(ctx, atk, 90);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Burning Charge: attach up to 2 Basic Energy from your deck to 1 of your Pokémon. Carnelian: can't attack next turn.</summary>
        private sealed class FlareonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Burning Charge")
                {
                    ctx.DamageDefending(atk.Damage);
                    SearchAndAttach(ctx, 2, true, atk.Name);
                    return true;
                }
                if (atk.Name == "Carnelian")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        private sealed class SlowkingPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wash the Slate Clean") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                BounceTwoEnergy(ctx, defending, atk.Name);
                return true;
            }
        }

        /// <summary>Spiral Drain: heal 30. Fighting Whirlpool: +90 against a Pokémon ex or V.</summary>
        private sealed class VaporeonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Spiral Drain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                if (atk.Name == "Fighting Whirlpool")
                {
                    ExOrVBonus(ctx, atk, 90);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Severe Squall: 60 to each of the opponent's Pokémon ex, no Weakness or Resistance. Aquamarine: can't attack next turn.</summary>
        private sealed class VaporeonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Severe Squall")
                {
                    foreach (var m in ctx.OpponentPokemon(x => IsExOrMega(x.Def))) ctx.DamageWith(m, 60, false, false, false);
                    return true;
                }
                if (atk.Name == "Aquamarine")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Frost Bullet: also 30 to 1 Benched Pokémon. Euclase: Knock Out 1 of the opponent's Pokémon with exactly 6 damage counters.</summary>
        private sealed class GlaceonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Frost Bullet")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageOneBenched(30);
                    return true;
                }
                if (atk.Name == "Euclase")
                {
                    var six = ctx.OpponentPokemon(m => m.DamageCounters == 6);
                    if (six.Count == 0) return true;
                    var target = ctx.ChoosePokemon(six, "Euclase: Knock Out which of your opponent's Pokémon?");
                    if (target != null) ctx.Engine.KnockOut(target, "Euclase");
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning

        /// <summary>Linear Attack: 30 to 1 of the opponent's Pokémon. Fighting Lightning: +90 against a Pokémon ex or V.</summary>
        private sealed class JolteonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Linear Attack")
                {
                    ctx.DamageAnyOpponent(30);
                    return true;
                }
                if (atk.Name == "Fighting Lightning")
                {
                    ExOrVBonus(ctx, atk, 90);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Flashing Spear: discard up to 2 Basic Energy from your Benched Pokémon, 90 more each. Dravite: can't attack next turn.</summary>
        private sealed class JolteonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flashing Spear")
                {
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var b in ctx.MyBench())
                        foreach (var e in b.Energy)
                            if (e.Def.IsBasicEnergy) holders[e] = b;
                    int discarded = 0;
                    if (holders.Count > 0)
                    {
                        var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, Math.Min(2, holders.Count),
                            "Flashing Spear: discard up to 2 Basic Energy from your Benched Pokémon (90 more damage each)");
                        foreach (var e in picks)
                            if (ctx.Engine.DiscardEnergy(holders[e], e)) discarded++;
                    }
                    ctx.DamageDefending(atk.Damage + 90 * discarded);
                    return true;
                }
                if (atk.Name == "Dravite")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        /// <summary>Psychic Assault: +10 per damage counter on the opponent's Active Pokémon. Psy Bolt: flip; heads Paralyzed.</summary>
        private sealed class EspeonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Psychic Assault")
                {
                    ctx.DamageDefending(atk.Damage + 10 * (defending == null ? 0 : defending.DamageCounters));
                    return true;
                }
                if (atk.Name == "Psy Bolt")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Psych Out: discard a random card from the opponent's hand. Amazez: devolve each of the opponent's evolved Pokémon, shuffling the top card into their deck.</summary>
        private sealed class EspeonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Psych Out")
                {
                    ctx.DamageDefending(atk.Damage);
                    TefPokemon.DiscardRandomFromOpponentHand(ctx);
                    return true;
                }
                if (atk.Name == "Amazez")
                {
                    var removed = new List<CardInstance>();
                    foreach (var m in ctx.OpponentPokemon(x => x.Stack.Count > 1 && !x.Def.IsFossil && !x.Def.IsMultiCard))
                    {
                        if (!ctx.CanAffect(m)) continue;
                        var top = m.Top;
                        string before = m.Name;
                        m.Stack.RemoveAt(m.Stack.Count - 1);
                        m.Conditions = SpecialCondition.None;
                        ctx.Engine.ClearEffectsOn(m);
                        removed.Add(top);
                        ctx.Engine.Emit(GameEventType.Info, m.Owner, before + " devolved into " + m.Name + "; " + top.Def.Name + " was shuffled into the deck.", m.Top.Id, m.Uid);
                    }
                    if (removed.Count > 0)
                    {
                        ctx.Opp.Deck.AddRange(removed);
                        ctx.Engine.ShuffleDeck(ctx.Opponent);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>Safeguard: prevent all damage done to this Pokémon by attacks from the opponent's Pokémon ex.</summary>
        private sealed class SylveonPRE : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && IsExOrMega(info.Attacker.Def);
            }
        }

        /// <summary>Roaring Scream: 20 damage per damage counter on this Pokémon to 1 of the opponent's Pokémon.</summary>
        private sealed class ScreamTailPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roaring Scream") return false;
                int damage = 20 * ctx.Pokemon.DamageCounters;
                if (damage > 0) ctx.DamageAnyOpponent(damage);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Swelling Power: attach a Basic Fighting Energy from your hand to 1 of your Pokémon. Magma Purge: discard up to 4 Energy from your Pokémon, 60 each.</summary>
        private sealed class GroudonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Swelling Power")
                {
                    CardsSSP2.AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Fighting), 1, m => true, "Swelling Power: attach a Basic Fighting Energy");
                    return true;
                }
                if (atk.Name == "Magma Purge")
                {
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var m in ctx.MyPokemon())
                        foreach (var e in m.Energy) holders[e] = m;
                    int discarded = 0;
                    if (holders.Count > 0)
                    {
                        var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, Math.Min(4, holders.Count),
                            "Magma Purge: discard up to 4 Energy from your Pokémon (60 damage each)");
                        foreach (var e in picks)
                            if (ctx.Engine.DiscardEnergy(holders[e], e)) discarded++;
                    }
                    ctx.DamageDefending(atk.Damage * discarded);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Tornado Rush: during your next turn, this Pokémon's Tornado Rush does 100 more damage.</summary>
        private sealed class LucarioExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tornado Rush") return false;
                int uid = ctx.Pokemon.Uid;
                int bonus = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>()
                    .Where(e => e.TargetUid == uid && e.AttackName == "Tornado Rush").Sum(e => e.Bonus);
                ctx.DamageDefending(atk.Damage + bonus);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new AttackBoostEffect(ctx.Pokemon.Name + "'s Tornado Rush does 100 more damage during its next turn", ctx.Player, uid, ctx.Turn + 2, "Tornado Rush", 100);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
        }

        /// <summary>Magnetic Absorption: once during your turn, with the opponent at 4 or fewer Prize cards, attach a Basic Fighting Energy from your discard pile to it. Earthen Spike: can't attack next turn.</summary>
        private sealed class SandyShocksExPRE : Mon
        {
            protected override string Activated { get { return "Magnetic Absorption"; } }

            private static List<CardInstance> Fighting(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Prizes.Count <= 4 && Fighting(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var energy = Fighting(ctx);
                if (energy.Count == 0) return;
                var card = ctx.ChooseCards(energy, 1, 1, "Magnetic Absorption: choose a Basic Fighting Energy to attach to " + ctx.Pokemon.Name)[0];
                ctx.Me.Discard.Remove(card);
                ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earthen Spike") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Feint Attack: 50 to 1 of the opponent's Pokémon, ignoring Weakness, Resistance and effects. Pitch-Black Blade: can't attack next turn.</summary>
        private sealed class UmbreonPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Feint Attack")
                {
                    ctx.DamageAnyOpponent(50, true, true);
                    return true;
                }
                if (atk.Name == "Pitch-Black Blade")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Moon Mirage: Confused. Onyx: discard all Energy from this Pokémon and take a Prize card.</summary>
        private sealed class UmbreonExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Moon Mirage")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Onyx")
                {
                    ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                    ctx.Engine.TakePrizeCards(ctx.Player, 1);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Call to Muster: attach up to 2 Basic Energy from your deck to your Pokémon in any way you like.</summary>
        private sealed class HoundoomPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call to Muster") return false;
                SearchAndAttach(ctx, 2, false, atk.Name);
                return true;
            }
        }

        /// <summary>Grind: 50 per Energy on this Pokémon. Tyrannical Crush: discard a random card from the opponent's hand.</summary>
        private sealed class TyranitarExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Grind")
                {
                    ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                    return true;
                }
                if (atk.Name == "Tyrannical Crush")
                {
                    ctx.DamageDefending(atk.Damage);
                    TefPokemon.DiscardRandomFromOpponentHand(ctx);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        // ================================================================== Colorless

        /// <summary>Rainbow DNA: can evolve from the hand into any Pokémon ex that evolves from Eevee.</summary>
        private sealed class EeveeExPRE : Mon
        {
            public override bool AllowsEvolvingInto(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return src.Pokemon != null && target == src.Pokemon && evolution.EvolvesFrom == "Eevee" && IsExOrMega(evolution);
            }
        }

        /// <summary>Insomnia: this Pokémon can't be Asleep.</summary>
        internal sealed class HoothootPRE : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & SpecialCondition.Asleep) != 0;
            }
        }

        /// <summary>Moomoo Rolling: only if this Pokémon used Rollout during your last turn.</summary>
        private sealed class MiltankPRE : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Moomoo Rolling") return true;
                var me = ctx.Pokemon;
                return me.LastAttackName == "Rollout" && me.LastAttackTurn == ctx.MyLastTurn;
            }
        }

        /// <summary>Hyper Whirlpool: flip until tails; discard an Energy from the opponent's Active Pokémon per heads.</summary>
        private sealed class LugiaExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Whirlpool") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, Math.Min(heads, defending.Energy.Count), "Hyper Whirlpool: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        private sealed class RegigigasPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jewel Breaker") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Def.IsTera ? 230 : 0));
                return true;
            }
        }

        /// <summary>Reflect Energy: move an Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class ShayminPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflect Energy") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Reflect Energy: move which Energy?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Reflect Energy: move " + energy.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                return true;
            }
        }

        private sealed class HawluchaPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rising Tackle") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && IsExOrMega(defending.Def) ? 50 : 0));
                return true;
            }
        }

        /// <summary>Strafe: you may switch this Pokémon with a Benched one. Sonic Blast: 30 to itself.</summary>
        private sealed class NoivernExPRE : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Strafe")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Bench.Count > 0 && ctx.PokemonIsActive && ctx.AskYesNo("Strafe: switch " + ctx.Pokemon.Name + " with 1 of your Benched Pokémon?"))
                        ctx.SwitchSelfWithBench();
                    return true;
                }
                if (atk.Name == "Sonic Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(30);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Trainers

        /// <summary>At the end of the turn Amarys was played, its player discards their hand if it has 5 or more cards.</summary>
        private sealed class AmarysEffect : ContinuousEffect
        {
            public AmarysEffect(string name, int owner, int turn) : base(name, owner, -1, turn, false) { }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn || engine.State.CurrentPlayer != Owner) return;
                if (engine.State.Players[Owner].Hand.Count >= 5) engine.DiscardHand(Owner);
            }
        }

        /// <summary>Draw 4 cards. At the end of this turn, discard your hand if it has 5 or more cards.</summary>
        private sealed class Amarys : CardScript
        {
            // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(4);
                ctx.AddEffect(new AmarysEffect("Amarys: at the end of this turn, a hand of 5 or more cards is discarded", ctx.Player, ctx.Turn));
            }
        }

        /// <summary>Discard another card from your hand to search for cards (Earthen Vessel, Techno Radar).</summary>
        private sealed class DiscardThenSearch : CardScript
        {
            private readonly Func<CardDef, bool> filter;
            private readonly int count;
            private readonly string prompt;

            public DiscardThenSearch(Func<CardDef, bool> filter, int count, string prompt)
            {
                this.filter = filter;
                this.count = count;
                this.prompt = prompt;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card) && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, ctx.SourceName + ": discard a card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.SearchToHand(filter, count, prompt);
            }
        }

        private sealed class DrawSupporter : CardScript
        {
            private readonly int count;

            public DrawSupporter(int count) { this.count = count; }

            // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(count); }
        }

        /// <summary>Discard your hand; search for a Pokémon, a Supporter and a Basic Energy.</summary>
        private sealed class LarrysSkill : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                if (ctx.Me.Deck.Count == 0) return;
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon, 1, "Larry's Skill: choose a Pokémon"));
                found.AddRange(ctx.SearchMyDeck(d => d.IsSupporter, 1, "Larry's Skill: choose a Supporter"));
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Larry's Skill: choose a Basic Energy"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Put up to 5 Pokémon and Basic Energy cards in any combination from your discard pile into your hand.</summary>
        private sealed class MaxRod : CardScript
        {
            private static List<CardInstance> Options(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Options(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var options = Options(ctx);
                if (options.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(options, 1, Math.Min(5, options.Count), "Max Rod: put up to 5 Pokémon and Basic Energy cards into your hand"));
            }
        }

        /// <summary>Up to 2 of your Ancient Pokémon each get a Basic Energy from your discard pile; if any did, draw 3 cards.</summary>
        private sealed class ProfessorSadasVitality : CardScript
        {
            private static List<PokemonInPlay> Ancient(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsAncient); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Ancient(ctx).Count > 0 && ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var ancients = Ancient(ctx);
                if (ancients.Count == 0) return;
                var targets = ctx.ChoosePokemonMany(ancients, 1, Math.Min(2, ancients.Count), "Professor Sada's Vitality: choose up to 2 of your Ancient Pokémon");
                int attached = 0;
                foreach (var t in targets)
                {
                    var energy = ctx.BasicEnergyInDiscard();
                    if (energy.Count == 0) break;
                    var card = ctx.ChooseCards(energy, 1, 1, "Professor Sada's Vitality: choose a Basic Energy to attach to " + t.Name)[0];
                    ctx.Me.Discard.Remove(card);
                    ctx.Engine.AttachEnergyCard(card, t);
                    attached++;
                }
                if (attached > 0) ctx.Draw(3);
            }
        }

        /// <summary>Put 1 of your Pokémon into your hand; discard all cards attached to it.</summary>
        private sealed class ProfessorTurosScenario : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.CanPutIntoHand(m) && (ctx.Me.Active != m || ctx.Me.Bench.Count > 0));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Professor Turo's Scenario: put which of your Pokémon into your hand?");
                if (mon == null) return;
                ctx.Engine.RemoveFromPlay(mon);
                var me = ctx.Me;
                me.Hand.AddRange(mon.Stack);
                me.Discard.AddRange(mon.Energy);
                me.Discard.AddRange(mon.Tools);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, mon.Name + " was put into " + ctx.Engine.Their(ctx.Player) + " hand; the cards attached to it were discarded.", mon.Top.Id, mon.Uid);
            }
        }

        /// <summary>Discard your hand and draw 7 cards.</summary>
        private sealed class ProfessorsResearch : CardScript
        {
            // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(7);
            }
        }

        /// <summary>Search for up to 5 Pokémon Tool cards.</summary>
        private sealed class SearchTools : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsTool, 5, "Treasure Tracker: choose up to 5 Pokémon Tools"); }
        }
    }
}
