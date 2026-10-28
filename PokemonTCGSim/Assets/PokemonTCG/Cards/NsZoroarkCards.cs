using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Cards added for the N's Zoroark ex list (Limitless deck list 30007).</summary>
    public static class NsZoroarkCards
    {
        public static void Register(CardScriptRegistry r)
        {
            // Pokémon
            r.RegisterByKey("JTG-98", new NsZoroarkEx());
            r.RegisterByKey("ASC-155", new NsZekrom());
            r.RegisterByKey("JTG-116", new NsReshiram());
            r.RegisterByKey("SFA-39", new PecharuntEx());
            r.RegisterByKey("MEG-88", new Yveltal());

            // Supporters
            r.RegisterByName("Black Belt's Training", new BlackBeltsTraining());
            r.RegisterByName("Cyrano", new Cyrano());
            r.RegisterByName("Judge", new Judge());

            // Items and Tools
            r.RegisterByName("N's PP Up", new NsPPUp());
            r.RegisterByName("Punk Helmet", new PunkHelmet());

            // Stadiums
            r.RegisterByName("N's Castle", new NsCastle());
            r.RegisterByName("Team Rocket's Watchtower", new TeamRocketsWatchtower());
        }

        /// <summary>"N's Pokémon" are Pokémon with "N's" in their name.</summary>
        public static bool IsNsPokemon(CardDef d)
        {
            return d != null && d.IsPokemon && d.Name.StartsWith("N's ");
        }
    }

    // ================================================================== Pokémon

    /// <summary>
    /// Trade: discard a card from your hand to draw 2 (once per turn).
    /// Night Joker: choose 1 of your Benched N's Pokémon's attacks and use it as this attack.
    /// </summary>
    public sealed class NsZoroarkEx : CardScript
    {
        const string Ability = "Trade";
        const string NightJoker = "Night Joker";

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            return ctx.IsMyTurn && !ctx.AbilityUsedThisTurn(Ability) && ctx.Me.Hand.Count >= 1 && ctx.Me.Deck.Count > 0;
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            var discard = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Trade: discard a card from your hand")[0];
            ctx.Engine.DiscardFromHand(ctx.Player, discard);
            ctx.Draw(2);
        }

        public override bool CanUseAttack(EffectContext ctx, int attackIndex)
        {
            if (ctx.Attack(attackIndex).Name != NightJoker) return true;
            return CopyableAttacks(ctx).Count > 0;
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name != NightJoker)
            {
                ctx.DamageDefending(atk.Damage);
                return;
            }
            var choices = CopyableAttacks(ctx);
            if (choices.Count == 0) return;
            var opts = choices.Select(c => new DecisionOption
            {
                Label = c.Key.Name + ": " + c.Key.Attacks[c.Value].Name + (c.Key.Attacks[c.Value].Damage > 0 ? " (" + c.Key.Attacks[c.Value].Damage + c.Key.Attacks[c.Value].DamageSuffix + ")" : ""),
                Card = c.Key,
                Index = c.Value
            }).ToList();
            int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Night Joker: choose an attack to use", opts, ctx.Source);
            var chosen = choices[pick];
            ctx.Log("Night Joker uses " + chosen.Key.Name + "'s " + chosen.Key.Attacks[chosen.Value].Name + ".");
            var copyCtx = new EffectContext(ctx.Engine, ctx.Player, ctx.Card, ctx.Pokemon, chosen.Key);
            ctx.Engine.Scripts.For(chosen.Key).ResolveAttack(copyCtx, chosen.Value);
        }

        /// <summary>Attacks on Benched N's Pokémon. Night Joker itself is excluded so copies can't loop forever.</summary>
        private static List<KeyValuePair<CardDef, int>> CopyableAttacks(EffectContext ctx)
        {
            var list = new List<KeyValuePair<CardDef, int>>();
            var seen = new HashSet<string>();
            foreach (var b in ctx.Me.Bench)
            {
                if (!NsZoroarkCards.IsNsPokemon(b.Def)) continue;
                for (int i = 0; i < b.Def.Attacks.Count; i++)
                {
                    if (b.Def.Attacks[i].Name == NightJoker) continue;
                    if (seen.Add(b.Def.BaseKey + "|" + i)) list.Add(new KeyValuePair<CardDef, int>(b.Def, i));
                }
            }
            return list;
        }
    }

    /// <summary>Shred: 70, not affected by effects on the Defending Pokémon. Rampaging Thunder: 250, can't attack next turn.</summary>
    public sealed class NsZekrom : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Shred")
            {
                ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return;
            }
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Rampaging Thunder") ctx.SelfCantAttackNextTurn();
        }
    }

    /// <summary>Powerful Rage: 20 for each damage counter on this Pokémon.</summary>
    public sealed class NsReshiram : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Powerful Rage")
            {
                ctx.DamageDefending(atk.Damage * ctx.Pokemon.DamageCounters);
                return;
            }
            ctx.DamageDefending(atk.Damage);
        }
    }

    /// <summary>
    /// Subjugating Chains: switch a Benched Darkness Pokémon (not Pecharunt ex) into the Active Spot; it becomes Poisoned.
    /// Irritated Outburst: 60 for each Prize card the opponent has taken.
    /// </summary>
    public sealed class PecharuntEx : CardScript
    {
        const string Ability = "Subjugating Chains";

        private static List<PokemonInPlay> Candidates(EffectContext ctx)
        {
            return ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Darkness) && m.Def.Name != "Pecharunt ex").ToList();
        }

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            return ctx.IsMyTurn
                && ctx.Me.Active != null
                && !ctx.AbilityUsedThisTurn(Ability)
                && !ctx.NamedLimitUsedThisTurn(Ability)
                && Candidates(ctx).Count > 0;
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            ctx.MarkNamedLimit(Ability);
            var target = ctx.ChoosePokemon(Candidates(ctx), "Subjugating Chains: switch which Benched Darkness Pokémon into the Active Spot?");
            ctx.Engine.SwitchActive(ctx.Player, target);
            ctx.ApplyCondition(target, SpecialCondition.Poisoned);
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Irritated Outburst")
            {
                int taken = System.Math.Max(0, ctx.Engine.PrizeCount - ctx.Opp.Prizes.Count);
                ctx.DamageDefending(atk.Damage * taken);
                return;
            }
            ctx.DamageDefending(atk.Damage);
        }
    }

    /// <summary>Clutch: during the opponent's next turn, the Defending Pokémon can't retreat.</summary>
    public sealed class Yveltal : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Clutch") ctx.DefendingCantRetreat(defending);
        }
    }

    // ================================================================== Supporters

    public sealed class BlackBeltsTraining : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.AddEffect(new DamageBonusEffect("Black Belt's Training: attacks do 40 more to the opponent's Active Pokémon ex this turn", ctx.Player, ctx.Turn, 40, true));
        }
    }

    public sealed class Cyrano : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(d => d.IsPokemon && (d.IsEx || d.IsMegaEx), 3, "Cyrano: choose up to 3 Pokémon ex");
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class Judge : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
            ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
            ctx.Draw(4);
            ctx.Engine.DrawCards(ctx.Opponent, 4);
        }
    }

    // ================================================================== Items and Tools

    public sealed class NsPPUp : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx)
        {
            return ctx.Me.Bench.Where(m => NsZoroarkCards.IsNsPokemon(m.Def)).ToList();
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return Targets(ctx).Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy);
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList(), 1, 1, "N's PP Up: choose a Basic Energy from your discard pile")[0];
            var target = ctx.ChoosePokemon(Targets(ctx), "N's PP Up: attach " + energy.Def.Name + " to which Benched N's Pokémon?");
            ctx.Me.Discard.Remove(energy);
            ctx.Engine.AttachEnergyCard(energy, target);
        }
    }

    /// <summary>If the Darkness Pokémon holding this is Active and damaged by an opponent's attack, 4 damage counters on the attacker.</summary>
    public sealed class PunkHelmet : CardScript
    {
        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (!holderWasActive || ctx.Pokemon.Def.PokemonType != EnergyType.Darkness) return;
            ctx.Engine.PlaceDamageCounters(attacker, 4, "Punk Helmet");
        }
    }

    // ================================================================== Stadiums

    /// <summary>N's Pokémon in play (both players') have no Retreat Cost.</summary>
    public sealed class NsCastle : CardScript
    {
        public override int ModifyRetreatCost(EffectContext ctx, PokemonInPlay mon, int cost)
        {
            return NsZoroarkCards.IsNsPokemon(mon.Def) ? 0 : cost;
        }
    }

    /// <summary>Colorless Pokémon in play (both players') have no Abilities.</summary>
    public sealed class TeamRocketsWatchtower : CardScript
    {
        public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon)
        {
            return mon.Def.PokemonType == EnergyType.Colorless;
        }
    }
}
