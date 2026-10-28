using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Journey Together (JTG) 71-158. Pokémon whose attacks only do their printed damage need no script:
    /// Impidimp, Morgrem, Milcery, Cubone, Piloswine, Toedscool, Koffing, Lokix, Cufant, Copperajah, Sentret, Furret,
    /// Minccino, Cinccino, Drampa, Hop's Corvisquire, Lechonk.
    /// </summary>
    public static class CardsJTG2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("JTG-73", new GrimmsnarlJTG());
            r.RegisterByKey("JTG-75", new AlcremieExJTG());
            r.RegisterByKey("JTG-77", new CallForFamilyMon());
            r.RegisterByKey("JTG-79", new MamoswineExJTG());
            r.RegisterByKey("JTG-80", new LarvitarJTG());
            r.RegisterByKey("JTG-81", new RecoilMon("Take Down", 20));
            r.RegisterByKey("JTG-82", new RegirockJTG());
            r.RegisterByKey("JTG-83", new TailsNothingMon("Focus Fist"));
            r.RegisterByKey("JTG-84", new RockruffJTG());
            r.RegisterByKey("JTG-85", new LycanrocJTG());
            r.RegisterByKey("JTG-86", new SearchAttackMon("Turf Maker", d => d.IsStadiumCard, 1));
            r.RegisterByKey("JTG-87", new HopsSandacondaJTG());
            r.RegisterByKey("JTG-89", new ToedscruelJTG());
            r.RegisterByKey("JTG-90", new KlawfJTG());
            r.RegisterByKey("JTG-92", new WeezingJTG());
            r.RegisterByKey("JTG-93", new FlipBonusMon("Trip Over", 20));
            r.RegisterByKey("JTG-94", new PaldeanClodsireExJTG());
            r.RegisterByKey("JTG-95", new TyranitarJTG());
            r.RegisterByKey("JTG-99", new PangoroJTG());
            r.RegisterByKey("JTG-101", new CardsSHF2.SpitShotMon("Drop Shot", 120));
            r.RegisterByKey("JTG-102", new LockAfterAttackMon("Iron Buster", false));
            r.RegisterByKey("JTG-103", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("JTG-104", new ConditionAttackMon("Spinning Gears", SpecialCondition.Confused));
            r.RegisterByKey("JTG-105", new CoinsMon("Triple Smash", 3));
            r.RegisterByKey("JTG-106", new RecoilMon("Bounding Bite", 30));
            r.RegisterByKey("JTG-107", new MagearnaJTG());
            r.RegisterByKey("JTG-108", new HopsCorviknightJTG());
            r.RegisterByKey("JTG-112", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("JTG-113", new ShieldAttackMon("Guard Press", 30));
            r.RegisterByKey("JTG-114", new SalamenceExJTG());
            r.RegisterByKey("JTG-115", new DruddigonJTG());
            r.RegisterByKey("JTG-122", new DodgeMon("Stealth Attack", false));
            r.RegisterByKey("JTG-123", new FruitBearingMon("Fruit Bearing", 3));
            r.RegisterByKey("JTG-124", new AudinoJTG());
            r.RegisterByKey("JTG-127", new DrawAttackMon("Rapid Draw", 1));
            r.RegisterByKey("JTG-128", new NoivernJTG());
            r.RegisterByKey("JTG-129", new KomalaJTG());
            r.RegisterByKey("JTG-131", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("JTG-132", new DrawAttackMon("Gluttonous Tail", 2));
            r.RegisterByKey("JTG-133", new HopsRookideeJTG());
            r.RegisterByKey("JTG-137", new CardsPFL.FlipUntilTailsTimesMon("Ceaseless Spitting"));
            r.RegisterByKey("JTG-140", new OinkologneJTG());
            r.RegisterByKey("JTG-141", new DrawAttackMon("Add On", 2));

            r.RegisterByName("Billy & O'Nare", new BillyAndONare());
            r.RegisterByName("Iris's Fighting Spirit", new IrissFightingSpirit());
            r.RegisterByName("Levincia", new Levincia());
            r.RegisterByName("Lillie's Pearl", new LilliesPearl());
            r.RegisterByName("Redeemable Ticket", new RedeemableTicket());
            r.RegisterByName("Super Potion", new SuperPotion());
        }

        // ================================================================== Shared helpers

        /// <summary>Crunch-style: after the damage, discard Energy from the opponent's Active Pokémon (chosen by the attacker).</summary>
        private static void DiscardFromDefending(EffectContext ctx, PokemonInPlay defending, int count, string attack)
        {
            if (count <= 0 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return;
            ctx.DiscardEnergyFrom(defending, Math.Min(count, defending.Energy.Count),
                attack + ": discard " + (count == 1 ? "an Energy" : count + " Energy") + " from " + defending.Name);
        }

        // ================================================================== Psychic

        private sealed class GrimmsnarlJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadowy Knot") return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                ctx.DamageDefending(atk.Damage * cost);
                return true;
            }
        }

        /// <summary>Confectionary Gift: once during your turn, heal 30 damage from 1 of your Pokémon.</summary>
        private sealed class AlcremieExJTG : Mon
        {
            protected override string Activated { get { return "Confectionary Gift"; } }

            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
            }

            protected override bool AbilityReady(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Hurt(ctx), "Confectionary Gift: heal 30 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 30);
            }
        }

        // ================================================================== Fighting

        /// <summary>Mammoth Hauler: once during your turn, search for a Pokémon. Rumbling March: +40 per Stage 2 on your Bench.</summary>
        private sealed class MamoswineExJTG : Mon
        {
            protected override string Activated { get { return "Mammoth Hauler"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon, 1, "Mammoth Hauler: choose a Pokémon to put into your hand");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rumbling March") return false;
                int stage2 = ctx.Me.Bench.Count(b => b.Def.Stage == Stage.Stage2);
                ctx.DamageDefending(atk.Damage + 40 * stage2);
                return true;
            }
        }

        private sealed class LarvitarJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crunch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) DiscardFromDefending(ctx, defending, 1, "Crunch");
                return true;
            }
        }

        /// <summary>Rock Armor: with any Energy attached, it takes 30 less damage from attacks. Breaching Lariat ignores Resistance.</summary>
        private sealed class RegirockJTG : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || me.Energy.Count == 0) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breaching Lariat") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }

        /// <summary>Dig It Up: look at the top card of your deck; you may discard it.</summary>
        private sealed class RockruffJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig It Up") return false;
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return true;
                if (ctx.AskYesNo("Dig It Up: the top card of your deck is " + top[0].Def.Name + ". Discard it?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                else
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " left the top card of " + ctx.Engine.Their(ctx.Player) + " deck where it was.");
                return true;
            }
        }

        /// <summary>Spike-Clad: on evolving from the hand, attach up to 2 Spiky Energy from the discard pile. Clamping Fangs: +40 per damage counter.</summary>
        private sealed class LycanrocJTG : Mon
        {
            private static List<CardInstance> Spiky(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.Name == "Spiky Energy").ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var spiky = Spiky(ctx);
                if (!TriggerOnce(ctx, "Spike-Clad", spiky.Count > 0, "Spike-Clad: attach up to 2 Spiky Energy from your discard pile to " + ctx.Pokemon.Name + "?"))
                    return;
                var picks = ctx.ChooseCards(spiky, 1, Math.Min(2, spiky.Count), "Spike-Clad: choose up to 2 Spiky Energy to attach to " + ctx.Pokemon.Name);
                foreach (var card in picks)
                {
                    if (!ctx.Me.Discard.Remove(card)) continue;
                    ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clamping Fangs") return false;
                var defending = ctx.Defending;
                int counters = defending == null ? 0 : defending.DamageCounters;
                ctx.DamageDefending(atk.Damage + 40 * counters);
                return true;
            }
        }

        /// <summary>Rumble: the Defending Pokémon can't retreat. Break Ground: also 20 to each of your own Benched Pokémon.</summary>
        private sealed class HopsSandacondaJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rumble")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Break Ground")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var benched in ctx.MyBench()) ctx.DamageTarget(benched, 20);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Secret Forest Path: while this Pokémon is on your Bench, your Active Pokémon's Retreat Cost is 2 less.</summary>
        private sealed class ToedscruelJTG : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || !src.Me.Bench.Contains(me) || mon != src.Me.Active) return cost;
                return cost - 2;
            }
        }

        /// <summary>Snipping Pincers: flip 2 coins; discard an Energy from the opponent's Active Pokémon for each heads.</summary>
        private sealed class KlawfJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snipping Pincers") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                DiscardFromDefending(ctx, defending, heads, "Snipping Pincers");
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Pervasive Gas: Confused. Crazy Blast: +120 if this Pokémon used Pervasive Gas during your last turn.</summary>
        private sealed class WeezingJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pervasive Gas")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Crazy Blast")
                {
                    var me = ctx.Pokemon;
                    bool gassed = me.LastAttackName == "Pervasive Gas" && me.LastAttackTurn == ctx.MyLastTurn;
                    ctx.DamageDefending(atk.Damage + (gassed ? 120 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Poison Ring: Poisoned and can't retreat during the opponent's next turn. Giga Impact: can't attack next turn.</summary>
        private sealed class PaldeanClodsireExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poison Ring")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Giga Impact")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Daunting Gaze: while this Pokémon is Active, your opponent can't play Item cards from their hand.</summary>
        private sealed class TyranitarJTG : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && player != src.Player && card.Def.IsItem && src.PokemonIsActive;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cracking Stomp") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                return true;
            }
        }

        /// <summary>Torment: the Defending Pokémon can't use the chosen attack next turn. Power Tackle: can't attack next turn.</summary>
        private sealed class PangoroJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Torment")
                {
                    var defending = ctx.Defending;
                    if (defending == null) return true;
                    int index = ChooseAttackOf(ctx, defending.Def, "Torment: choose 1 of " + defending.Name + "'s attacks");
                    if (index < 0) return true;
                    string name = defending.Def.Attacks[index].Name;
                    ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (Torment)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, name));
                    return true;
                }
                if (atk.Name == "Power Tackle")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        /// <summary>Auto Heal: while this Pokémon is Active, attaching an Energy card from your hand heals 90 from that Pokémon.</summary>
        private sealed class MagearnaJTG : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || !src.PokemonIsActive || target == null || target.DamageCounters == 0) return;
                src.Log("Auto Heal (" + me.Name + ") heals " + target.Name + ".");
                src.Heal(target, 90);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Draw") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Shoot Through: also 50 to 1 Benched Pokémon. Steel Wing: takes 60 less damage next turn.</summary>
        private sealed class HopsCorviknightJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shoot Through")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageOneBenched(50);
                    return true;
                }
                if (atk.Name == "Steel Wing")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(60);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Dragon

        /// <summary>Wide Blast: 50 to each of the opponent's Benched Pokémon. Dragon Impact: discard 2 Energy from this Pokémon.</summary>
        private sealed class SalamenceExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Wide Blast")
                {
                    foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 50);
                    return true;
                }
                if (atk.Name == "Dragon Impact")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Dragon Impact: discard 2 Energy from " + ctx.Pokemon.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Dragon's Fury: attach a Basic Fire Energy from your discard pile to 1 of your Dragon Pokémon.</summary>
        private sealed class DruddigonJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon's Fury") return false;
                ctx.DamageDefending(atk.Damage);
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count > 0)
                    ctx.AttachCardsAnyWay(fire, 1, (c, m) => ctx.Engine.IsType(m, EnergyType.Dragon),
                        "Dragon's Fury: attach a Basic Fire Energy to 1 of your Dragon Pokémon", false);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Fruit Bearing: discard a card from your hand; if you did, draw 3 cards. Used for Fruit Bearing, Stick 'n' Draw (the attack name and the numbers are passed in).</summary>
        internal sealed class FruitBearingMon : Mon
        {
            private readonly string attack;
            private readonly int cards;

            public FruitBearingMon(string attack, int cards)
            {
                this.attack = attack;
                this.cards = cards;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Me.Hand.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, attack + ": discard a card from your hand");
                if (pick.Count == 0) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(cards);
                return true;
            }
        }

        /// <summary>Beckon: put a Supporter card from your discard pile into your hand.</summary>
        private sealed class AudinoJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Beckon") return false;
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(supporters, 1, 1, "Beckon: choose a Supporter to put into your hand"));
                return true;
            }
        }

        /// <summary>Tuning Echo: with as many cards in hand as the opponent, Frightening Howl costs no Energy. Frightening Howl: Confused.</summary>
        private sealed class NoivernJTG : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || attack.Name != "Frightening Howl") return;
                if (src.Me.Hand.Count == src.Opp.Hand.Count) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frightening Howl") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Slumbering Smack: both Active Pokémon fall Asleep; during your next turn this Pokémon's attacks do 100 more damage.</summary>
        private sealed class KomalaJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slumbering Smack") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (!ctx.InPlay(me) || !ctx.IsActive(me)) return true;
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                ctx.AddEffect(new SelfAttackBonusEffect(me.Name + "'s attacks do 100 more damage during its next turn (Slumbering Smack)",
                    ctx.Player, me.Uid, ctx.Turn, 100));
                return true;
            }
        }

        /// <summary>Intimidating Stare: the Defending Pokémon's attacks do 20 less damage during the opponent's next turn.</summary>
        private sealed class HopsRookideeJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intimidating Stare") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 20 less damage during its next turn (Intimidating Stare)",
                    ctx.Player, defending.Uid, ctx.Turn + 1, 20));
                return true;
            }
        }

        /// <summary>Aromatic Stomps: flip; if heads, the Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class OinkologneJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aromatic Stomps") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Aromatic Stomps)",
                        ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Draw 2 cards; then, with 10 or more cards in hand, draw 2 more.</summary>
        private sealed class BillyAndONare : CardScript
        {
            // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.Me.Hand.Count >= 10) ctx.Draw(2);
            }
        }

        /// <summary>Discard another card from your hand (required to play it), then draw until you have 6.</summary>
        private sealed class IrissFightingSpirit : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c != ctx.Card);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Iris's Fighting Spirit: discard a card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.DrawUntil(6);
            }
        }

        /// <summary>Once during each player's turn: put up to 2 Basic Lightning Energy from your discard pile into your hand.</summary>
        private sealed class Levincia : StadiumAction
        {
            private static List<CardInstance> Lightning(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
            }

            protected override bool Ready(EffectContext ctx) { return Lightning(ctx).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var energy = Lightning(ctx);
                if (energy.Count == 0) return false;
                ctx.DiscardToHand(ctx.ChooseCards(energy, 1, Math.Min(2, energy.Count), "Levincia: put up to 2 Basic Lightning Energy into your hand"));
                return false;
            }
        }

        /// <summary>If the Lillie's Pokémon holding this is Knocked Out by damage from an opponent's attack, they take 1 fewer Prize card.</summary>
        private sealed class LilliesPearl : CardScript
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || taker == src.Player) return prizes;
                if (!knockedOut.Def.IsOwnersPokemon("Lillie")) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                return prizes - 1;
            }
        }

        /// <summary>Put your Prize cards on the bottom of your deck in random order, then take that many new Prize cards from the top.</summary>
        private sealed class RedeemableTicket : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Prizes.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var prizes = me.Prizes.ToList();
                int n = prizes.Count;
                if (n == 0) return;
                me.Prizes.Clear();
                me.FaceUpPrizeIds.Clear();
                ctx.State.Rng.Shuffle(prizes);
                me.Deck.AddRange(prizes);
                int take = Math.Min(n, me.Deck.Count);
                me.Prizes.AddRange(me.Deck.Take(take));
                me.Deck.RemoveRange(0, take);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + n + " Prize cards on the bottom of " + ctx.Engine.Their(ctx.Player)
                    + " deck and took " + take + " new Prize cards from the top.");
            }
        }

        /// <summary>Heal 60 damage from 1 of your Pokémon; if you did, discard an Energy from it.</summary>
        private sealed class SuperPotion : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Super Potion: heal 60 damage from which of your Pokémon?");
                if (target == null) return;
                if (ctx.Heal(target, 60) > 0 && target.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(target, 1, "Super Potion: discard an Energy from " + target.Name);
            }
        }
    }
}
