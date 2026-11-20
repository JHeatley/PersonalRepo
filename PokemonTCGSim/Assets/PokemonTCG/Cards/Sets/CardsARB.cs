using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>Arceus (AR; v173): Uncommons, Commons and Trainers.</summary>
    public static partial class CardsAR
    {
        private static void RegisterCommons(CardScriptRegistry r)
        {
            r.RegisterByKey("AR-33", new Fx()
                .Effect("Wide Laser", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 10); })
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("AR-34", new Fx().Damage("Extrasensory", (c, a) => a.Damage + (c.Me.Hand.Count == c.Opp.Hand.Count ? 30 : 0)));
            r.RegisterByKey("AR-35", new CoinsMon("Slam", 2));
            r.RegisterByKey("AR-36", new Fx()
                .With(new ParalyzeFlipMon("Lick"))
                .After("Night Shade", (c, a) => HitBenched(c, 1, 10, "Night Shade")));
            r.RegisterByKey("AR-37", new RecoilMon("Double-edge", 10));
            r.RegisterByKey("AR-38", new DodgeMon("Detect", false));
            r.RegisterByKey("AR-39", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("AR-40", new Fx().Effect("Endure", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new EndureEffect(c.Pokemon.Name + " can't be Knocked Out by damage from attacks (Endure)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("AR-41", new HaunterHiddenPoison());
            r.RegisterByKey("AR-42", new Fx().After("Shadow Bind", (c, a) => { if (c.Defending != null && c.InPlay(c.Defending)) c.DefendingCantRetreat(c.Defending); }));
            r.RegisterByKey("AR-43", new Fx().Effect("Discharge", (c, a) =>
                {
                    int n = DiscardTypeFromSelf(c, EnergyType.Lightning, 99);
                    c.DamageDefending(40 * (n == 0 ? 0 : c.FlipCoins(n)));
                }));
            r.RegisterByKey("AR-45", new Fx().Damage("Water Ball", (c, a) => 20 + 10 * c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water)));
            r.RegisterByKey("AR-46", new CardsSIT1.AscensionMon());
            r.RegisterByKey("AR-47", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("AR-48", new Fx()
                .Effect("Scrunch", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new NoDamageEffect(c.Pokemon.Name + " is protected from damage (Scrunch)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                })
                .Damage("Dragon Bump", (c, a) =>
                {
                    var me = c.Pokemon;
                    bool both = me.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Fire)) && me.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Water));
                    return a.Damage + (both ? 20 : 0);
                }));
            r.RegisterByKey("AR-49", new Fx()
                .After("Magical Needle", (c, a) =>
                {
                    var hurt = c.MyBench().Where(m => m.DamageCounters > 0).ToList();
                    if (hurt.Count == 0) return;
                    var t = c.ChoosePokemon(hurt, "Magical Needle: remove 2 damage counters from which Benched Pokémon?");
                    if (t != null) c.Heal(t, 20);
                })
                .After("Cloak Scale", (c, a) =>
                {
                    var d = c.Defending;
                    if (c.MyPokemon(m => m.Name == "Wormadam Sandy Cloak").Count == 0 || d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    c.ApplyCondition(d, SpecialCondition.Burned);
                    c.ApplyCondition(d, SpecialCondition.Confused);
                    c.ApplyCondition(d, SpecialCondition.Poisoned);
                }));
            r.RegisterByKey("AR-50", new Fx().Damage("Cloak Headbutt", (c, a) => a.Damage + (c.MyPokemon(m => m.Name == "Wormadam Trash Cloak").Count > 0 ? 30 : 0)));
            r.RegisterByKey("AR-51", new Fx()
                .Effect("Cloak Shard", (c, a) => HitAny(c, c.MyPokemon(m => m.Name == "Wormadam Plant Cloak").Count > 0 ? 40 : 10, "Cloak Shard"))
                .After("Serve Trash", (c, a) =>
                {
                    if (c.Opp.Discard.Count == 0 || !c.AskYesNo("Serve Trash: put 1 card from your opponent's discard pile on top of their deck?")) return;
                    var card = c.ChooseCards(c.Opp.Discard.ToList(), 1, 1, "Serve Trash: put which card on top of your opponent's deck?")[0];
                    c.Opp.Discard.Remove(card);
                    c.Opp.Deck.Insert(0, card);
                    c.Log(card.Def.Name + " was put on top of " + c.Engine.PlayerName(c.Opponent) + "'s deck (Serve Trash).");
                }));
            r.RegisterByKey("AR-52", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("AR-53", new Fx()
                .Damage("Raid", (c, a) => c.Pokemon.TurnEnteredPlay == c.Turn ? 40 : a.Damage)
                .With(new CoinsMon("Fury Attack", 3)));
            r.RegisterByKey("AR-54", new CardsPAR2.FlipConditionMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("AR-55", new Fx().Effect("Bunny Hop", (c, a) => HitAny(c, 20, "Bunny Hop", m => m.DamageCounters == 0)));
            r.RegisterByKey("AR-56", new BurmyCloakEvolution());
            r.RegisterByKey("AR-57", new BurmyCloakEvolution());
            r.RegisterByKey("AR-58", new BurmyCloakEvolution());
            r.RegisterByKey("AR-59", new SearchAttackMon("Call for Friends", d => d.IsBasicPokemon && d.HasType(EnergyType.Fire), 1));
            r.RegisterByKey("AR-61", new CardsDPEra.AstonishMon("Astonish"));
            r.RegisterByKey("AR-62", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("AR-65", new Fx().Damage("Stone Barrage", (c, a) => 10 * c.FlipUntilTails()));
            r.RegisterByKey("AR-66", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("AR-67", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 2));
            r.RegisterByKey("AR-68", new Fx().Effect("Sand Attack", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new AttackFlipEffect(d.Name + " must flip a coin to attack (Sand Attack)", c.Player, d.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("AR-69", new ComboMon(new CardsDPEra.HardenMon("Harden", 30), new FlipBonusMon("Knock Away", 10)));
            r.RegisterByKey("AR-70", new ComboMon(new DrawAttackMon("Collect", 3), new ParalyzeFlipMon("Tickle")));
            r.RegisterByKey("AR-71", new Fx().Effect("Mega Shot", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Lightning, 99);
                    HitAny(c, 40, "Mega Shot");
                }));
            r.RegisterByKey("AR-72", new FlipBonusMon("Stomp", 20));
            r.RegisterByKey("AR-73", new TailsNothingMon("Lunge"));
            r.RegisterByKey("AR-74", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("AR-75", new ComboMon(new ParalyzeFlipMon("Ice Breath"), new CoinsMon("Double Headbutt", 2)));
            r.RegisterByKey("AR-76", new ComboMon(new CardsTWM1.HealSelfMon("Absorb", 10), new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep)));
            r.RegisterByKey("AR-77", new ComboMon(new DrawAttackMon("Collect", 1), new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("AR-78", new FlipBonusMon("Tail Crush", 10));
            r.RegisterByKey("AR-80", new LockAfterAttackMon("Slashing Strike", true));

            r.RegisterByKey("AR-83", new BenchShield());
            r.RegisterByKey("AR-85", new DepartmentStoreGirl());
            r.RegisterByKey("AR-87", new ExpertBelt());
            r.RegisterByKey("AR-88", new LuckyEgg());
        }

        /// <summary>Hidden Poison (Poké-Body): when your Active Haunter is damaged by an opponent's attack, the attacker is Poisoned. Tongue Spring.</summary>
        private sealed class HaunterHiddenPoison : BodyMon
        {
            public HaunterHiddenPoison() : base(new Fx().Effect("Tongue Spring", (c, a) => HitAny(c, 20, "Tongue Spring"))) { }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Hidden Poison");
                ctx.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Cloak Evolution (Poké-Body): Burmy can evolve during the turn you play it.</summary>
        private sealed class BurmyCloakEvolution : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return target != null && target == src.Pokemon && Works(src, "Cloak Evolution");
            }
        }

        /// <summary>Bench Shield (Pokémon Tool): while the holder is on your Bench, prevent all damage done to it by attacks.</summary>
        private sealed class BenchShield : CardScript
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target == src.Pokemon && src.Me.Bench.Contains(src.Pokemon) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Department Store Girl (Supporter): search your deck for up to 3 Pokémon Tool cards.</summary>
        private sealed class DepartmentStoreGirl : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsTool, 3, "Department Store Girl: search your deck for up to 3 Pokémon Tool cards"); }
        }

        /// <summary>Expert Belt (Pokémon Tool): +20 HP, its attacks do 20 more to the opponent's Active Pokémon; Knocked Out, the opponent takes 1 more Prize card.</summary>
        private sealed class ExpertBelt : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon != null && mon == src.Pokemon ? hp + 20 : hp; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker != src.Pokemon || info.Target == null || info.Target != src.Opp.Active || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 20;
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return knockedOut != null && knockedOut == src.Pokemon && taker != src.Player ? prizes + 1 : prizes;
            }
        }

        /// <summary>Lucky Egg (Pokémon Tool): when the holder is Knocked Out by damage from an opponent's attack, draw until you have 7 cards.</summary>
        private sealed class LuckyEgg : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack) return;
                src.Log("Lucky Egg: " + src.Engine.PlayerName(src.Player) + " draws until they have 7 cards.");
                src.DrawUntil(7);
            }
        }
    }
}
