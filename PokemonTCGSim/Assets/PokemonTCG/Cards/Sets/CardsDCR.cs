using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Double Crisis (DCR 1-34, v100): Team Magma and Team Aqua. "Team Aqua Pokémon" and "Team Magma Pokémon" are the Pokémon with
    /// Team Aqua's / Team Magma's in their name (the Classic Collection's Team Magma's Groudon counts too).
    /// </summary>
    public static class CardsDCR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DCR-1", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("DCR-2", new CameruptDCR());
            r.RegisterByKey("DCR-4", new ComboMon(new CardsMEG.SnipeMon("Splatter", 20), new TeamBonusMon("Hail Storm", Magma, 60)));
            r.RegisterByKey("DCR-5", new ComboMon(new CardsASR1.TimesMon("Power Blow", c => c.EnergyCount(c.Pokemon)), new DualBlizzardMon()));
            r.RegisterByKey("DCR-6", new PowerSaverMon(Aqua, new CardsOBF1.RetreatCostDamageMon("Aqua Impact", 20, false)));
            r.RegisterByKey("DCR-8", new MukDCR());
            r.RegisterByKey("DCR-9", new ComboMon(new CardsPAR2.FlipConditionMon("Venomous Fang", SpecialCondition.Poisoned), new VenomTailMon()));
            r.RegisterByKey("DCR-10", new CardsEPO.TelekinesisMon(20));
            r.RegisterByKey("DCR-11", new ClaydolDCR());
            r.RegisterByKey("DCR-13", new RecoilMon("Take Down", 10));
            r.RegisterByKey("DCR-14", new ComboMon(new RockStompMon(), new DamagedBenchMon("Boulder Storm", 20)));
            r.RegisterByKey("DCR-15", new PowerSaverMon(Magma, new CardsPFL.DamagedBonusMon("Magma Quake", 80)));
            r.RegisterByKey("DCR-16", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("DCR-17", new InfiltrateMon());
            r.RegisterByKey("DCR-18", new ComboMon(new TeampactMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Crunch")));
            r.RegisterByKey("DCR-19", new TeamBonusMon("Hostile Fang", Aqua, 40));
            r.RegisterByKey("DCR-20", new CoinsMon("Fin Smack", 2));
            r.RegisterByKey("DCR-21", new SharpedoDCR());
            r.RegisterByKey("DCR-22", new ComboMon(new MagmaCallForFamilyMon(), new CardsASR1.TimesMon("Team Play", c => c.Me.Bench.Count(m => IsTeam(m.Def, Magma)))));

            r.RegisterByName("Aqua Diffuser", new AquaDiffuser());
            r.RegisterByName("Magma Pointer", new MagmaPointer());
            r.RegisterByName("Team Aqua Admin", new TeamAquaAdmin());
            r.RegisterByName("Team Aqua Grunt", new TeamGrunt(Aqua));
            r.RegisterByName("Team Aqua's Great Ball", new TeamGreatBall(Aqua, EnergyType.Water));
            r.RegisterByName("Team Aqua's Secret Base", new TeamAquasSecretBase());
            r.RegisterByName("Team Magma Admin", new TeamMagmaAdmin());
            r.RegisterByName("Team Magma Grunt", new TeamGrunt(Magma));
            r.RegisterByName("Team Magma's Great Ball", new TeamGreatBall(Magma, EnergyType.Fighting));
            r.RegisterByName("Team Magma's Secret Base", new TeamMagmasSecretBase());
            r.RegisterByName("Double Aqua Energy", new DoubleTeamEnergy(Aqua, EnergyType.Water));
            r.RegisterByName("Double Magma Energy", new DoubleTeamEnergy(Magma, EnergyType.Fighting));
        }

        internal const string Aqua = "Team Aqua";
        internal const string Magma = "Team Magma";

        /// <summary>A Team Aqua / Team Magma Pokémon: "Team Aqua's" / "Team Magma's" in its name.</summary>
        internal static bool IsTeam(CardDef d, string team)
        {
            return d != null && d.IsPokemon && d.Name.StartsWith(team + "'s", StringComparison.Ordinal);
        }

        internal static bool IsTeam(PokemonInPlay mon, string team) { return mon != null && IsTeam(mon.Def, team); }

        // ================================================================== Pokémon

        /// <summary>
        /// Power Saver: with 4 or fewer Team Aqua (Team Magma) Pokémon in play, both players' and itself included, it can't attack
        /// (2015-04-23).
        /// </summary>
        private sealed class PowerSaverMon : Mon
        {
            private readonly string team;
            private readonly Mon attacks;

            public PowerSaverMon(string team, Mon attacks)
            {
                this.team = team;
                this.attacks = attacks;
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon) return false;
                int n = 0;
                for (int side = 0; side < 2; side++)
                    n += src.State.Players[side].InPlay().Count(m => IsTeam(m, team));
                return n <= 4;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"If your opponent's Active Pokémon is a Team Magma (Aqua) Pokémon, this attack does N more damage."</summary>
        private sealed class TeamBonusMon : Mon
        {
            private readonly string attack;
            private readonly string team;
            private readonly int bonus;

            public TeamBonusMon(string attack, string team, int bonus)
            {
                this.attack = attack;
                this.team = team;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (IsTeam(ctx.Defending, team) ? bonus : 0));
                return true;
            }
        }

        /// <summary>
        /// Burning Draft: once during your turn, attach a basic Fighting or Fire Energy card from your discard pile to this Pokémon.
        /// Flame Ball: move a basic Energy from this Pokémon to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class CameruptDCR : Mon
        {
            protected override string Activated { get { return "Burning Draft"; } }

            private static List<CardInstance> Pile(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting || d.EnergyProvides == EnergyType.Fire);
            }

            protected override bool AbilityReady(EffectContext ctx) { return Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                ctx.AttachCardsAnyWay(Pile(ctx), 1, (card, mon) => mon == me, "Burning Draft: attach a Fighting or Fire Energy card from your discard pile to " + me.Name, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Ball") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var basics = me.Energy.Where(en => en.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Flame Ball: move which basic Energy?")[0];
                var bench = ctx.MyBench();
                var target = ctx.ChoosePokemon(bench, "Flame Ball: move " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Dual Blizzard: discard 2 Water Energy from this Pokémon, then 80 damage to each of 2 of the opponent's Pokémon.</summary>
        private sealed class DualBlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Blizzard") return false;
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 2, "Dual Blizzard: discard 2 Water Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water));
                var targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return true;
                var picks = targets.Count <= 2 ? targets : ctx.ChoosePokemonMany(targets, 2, 2, "Dual Blizzard: choose 2 of your opponent's Pokémon (80 damage each)");
                foreach (var target in picks)
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, atk.Damage);
                return true;
            }
        }

        /// <summary>Sludge Festival: every Pokémon in play that isn't a Team Aqua Pokémon retreats for 1 more. Pester: 60 more against a Special Condition.</summary>
        private sealed class MukDCR : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon == null || IsTeam(mon, Aqua) ? cost : cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pester") return false;
                var d = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (d != null && d.Conditions != SpecialCondition.None ? 60 : 0));
                return true;
            }
        }

        /// <summary>Venom Tail: if the opponent's Active Pokémon has a Special Condition, discard an Energy from it.</summary>
        private sealed class VenomTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Venom Tail") return false;
                var defending = ctx.Defending;
                bool affected = defending != null && defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage);
                if (affected && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Venom Tail: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Magma Switch: once during your turn, move a basic Energy from 1 of your Pokémon to 1 of your Team Magma Pokémon (only the
        /// receiving Pokémon must be Team Magma: 2015-04-23).
        /// </summary>
        private sealed class ClaydolDCR : Mon
        {
            protected override string Activated { get { return "Magma Switch"; } }

            private static List<CardInstance> Movable(EffectContext ctx)
            {
                var list = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon())
                    if (ctx.MyPokemon().Any(m => m != mon && IsTeam(m, Magma)))
                        list.AddRange(mon.Energy.Where(en => en.Def.IsBasicEnergy));
                return list;
            }

            protected override bool AbilityReady(EffectContext ctx) { return Movable(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Movable(ctx);
                var card = ctx.ChooseCards(cards, 1, 1, "Magma Switch: move which basic Energy?")[0];
                var giver = ctx.MyPokemon().FirstOrDefault(m => m.Energy.Contains(card));
                if (giver == null) return;
                var targets = ctx.MyPokemon().Where(m => m != giver && IsTeam(m, Magma)).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Magma Switch: move " + card.Def.Name + " to which Team Magma Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(giver, target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>
        /// Rock Stomp: discard any number of Fighting Energy from your Pokémon; 40 damage for each Fighting Energy discarded. A Double
        /// Magma Energy counts as 2 (your ruling, v100).
        /// </summary>
        private sealed class RockStompMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Stomp") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var en in mon.Energy)
                        if (CardsDRI1.ProvidesType(ctx, mon, en, EnergyType.Fighting)) holders[en] = mon;
                int fighting = 0;
                if (holders.Count > 0)
                {
                    var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Rock Stomp: discard any number of Fighting Energy from your Pokémon (40 damage each)");
                    foreach (var card in picks)
                    {
                        var holder = holders[card];
                        int units = ctx.Engine.EnergyUnitsOf(holder, card).Count(u => EnergyMask.Provides(u, EnergyType.Fighting));
                        if (ctx.Engine.DiscardEnergy(holder, card)) fighting += units;
                    }
                }
                if (fighting > 0) ctx.DamageDefending(atk.Damage * fighting);
                else ctx.Log(ctx.Pokemon.Name + "'s Rock Stomp did no damage.");
                return true;
            }
        }

        /// <summary>"Also does N damage to each of the opponent's Benched Pokémon that has any damage counters on it" (Boulder Storm).</summary>
        internal sealed class DamagedBenchMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public DamagedBenchMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var hurt = ctx.Opp.Bench.Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in hurt)
                    if (ctx.InPlay(mon) && ctx.Opp.Bench.Contains(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Infiltrate: your opponent reveals their hand.</summary>
        private sealed class InfiltrateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Infiltrate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsTWM1.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Teampact: flip a coin for each of your Team Aqua Pokémon in play; 30 damage for each heads.</summary>
        private sealed class TeampactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teampact") return false;
                int coins = ctx.MyPokemon().Count(m => IsTeam(m, Aqua));
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>Aqua Search: once during your turn, search your deck for a Team Aqua Pokémon and put it into your hand.</summary>
        private sealed class SharpedoDCR : Mon
        {
            protected override string Activated { get { return "Aqua Search"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => IsTeam(d, Aqua), 1, "Aqua Search: choose a Team Aqua Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>Call for Family (Zangoose): up to 2 Basic Team Magma Pokémon from your deck onto your Bench.</summary>
        private sealed class MagmaCallForFamilyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => IsTeam(d, Magma), 2, "Call for Family: put up to 2 Basic Team Magma Pokémon onto your Bench");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Aqua Diffuser / Magma Pointer: the team's Pokémon holding it can use the attack on the card (it still needs the Energy). It
        /// counts as the turn's attack (2015-04-23).
        /// </summary>
        private abstract class TeamToolAttack : CardScript
        {
            protected abstract string Team { get; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !IsTeam(mon, Team) || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { Use(ctx); }

            protected abstract void Use(EffectContext ctx);
        }

        /// <summary>Aqua Diffuser: the opponent's Active Pokémon is now Confused and Poisoned.</summary>
        private sealed class AquaDiffuser : TeamToolAttack
        {
            protected override string Team { get { return Aqua; } }

            protected override void Use(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (defending == null) return;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Magma Pointer: 20 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class MagmaPointer : TeamToolAttack
        {
            protected override string Team { get { return Magma; } }

            protected override void Use(EffectContext ctx) { ctx.DamageAnyOpponent(20); }
        }

        /// <summary>Team Aqua Admin: attach a basic Energy card from your discard pile to your Active Team Aqua Pokémon.</summary>
        private sealed class TeamAquaAdmin : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return IsTeam(ctx.Me.Active, Aqua) && ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (!IsTeam(active, Aqua)) return;
                ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 1, (card, mon) => mon == active,
                    "Team Aqua Admin: attach a basic Energy card from your discard pile to " + active.Name, false);
            }
        }

        /// <summary>Team Magma Admin: put up to 3 Team Magma Pokémon from your discard pile into your hand.</summary>
        private sealed class TeamMagmaAdmin : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => IsTeam(c.Def, Magma)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => IsTeam(c.Def, Magma)).ToList();
                if (pile.Count == 0) return;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(3, pile.Count), "Team Magma Admin: put up to 3 Team Magma Pokémon into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Team Aqua / Magma Grunt: discard a card from your hand (needed to play it), draw 3; 4 if it was the team's Pokémon.</summary>
        private sealed class TeamGrunt : CardScript
        {
            private readonly string team;

            public TeamGrunt(string team) { this.team = team; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 1; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, team + " Grunt: discard a card from your hand")[0];
                bool bonus = IsTeam(pick.Def, team);
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(bonus ? 4 : 3);
            }
        }

        /// <summary>Team Aqua's / Team Magma's Great Ball: search for a Basic Pokémon of the team and a basic Energy of its type.</summary>
        private sealed class TeamGreatBall : CardScript
        {
            private readonly string team;
            private readonly EnergyType type;

            public TeamGreatBall(string team, EnergyType type)
            {
                this.team = team;
                this.type = type;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && IsTeam(d, team), 1, ctx.SourceName + ": choose a Basic " + team + " Pokémon");
                found.AddRange(ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, type), 1, ctx.SourceName + ": choose a basic " + type + " Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Team Aqua's Secret Base: every Pokémon in play that isn't a Team Aqua Pokémon retreats for 1 more.</summary>
        private sealed class TeamAquasSecretBase : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || IsTeam(mon, Aqua) || src.Engine.StadiumShielded(mon)) return cost;
                return cost + 1;
            }
        }

        /// <summary>Team Magma's Secret Base: a Basic Pokémon (not Team Magma) put onto a Bench from the hand gets 2 damage counters, either player's.</summary>
        private sealed class TeamMagmasSecretBase : CardScript
        {
            public override void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched)
            {
                if (!ctx.Engine.BenchingFromHand || benched == null || !benched.IsBasic || IsTeam(benched, Magma) || ctx.Engine.StadiumShielded(benched)) return;
                ctx.Engine.PlaceDamageCounters(benched, 2, "Team Magma's Secret Base");
            }
        }

        /// <summary>
        /// Double Aqua / Magma Energy: only on the team's Pokémon (discarded from anything else); 2 Water (Fighting) Energy there;
        /// discarded at the end of the turn it was attached (it can't stay past a turn's end).
        /// </summary>
        private sealed class DoubleTeamEnergy : CardScript
        {
            private readonly string team;
            private readonly EnergyType type;

            public DoubleTeamEnergy(string team, EnergyType type)
            {
                this.team = team;
                this.type = type;
            }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (!IsTeam(ctx.Pokemon, team)) return new List<int>();
                return RecifeEnergy.Units(EnergyMask.Of(type), 2);
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return IsTeam(target, team); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !IsTeam(ctx.Pokemon, team); }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                if (src.Pokemon != null && src.Pokemon.Energy.Contains(src.Card)) src.Engine.DiscardEnergy(src.Pokemon, src.Card);
            }
        }
    }
}
