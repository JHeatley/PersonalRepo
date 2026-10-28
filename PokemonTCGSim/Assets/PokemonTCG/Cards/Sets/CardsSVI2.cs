using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Scarlet &amp; Violet (SVI) 100-198: the Pokémon and Trainers that weren't already in the pool (all G-mark, Expanded only).
    /// Koraidon and Varoom (140) are reprints of promos already in the pool; Energy Search, Poké Ball and Potion are reprints
    /// of the Perfect Order cards. Pokémon whose attacks only do their printed damage need no script: two Flittle
    /// (100, 101), Riolu, Sandile, Silicobra, Croagunk, Maschiff (135), Varoom (141), Zangoose (146), Starly, Staravia,
    /// Tandemaus (159).
    /// </summary>
    public static class CardsSVI2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SVI-102", new SwitchSelfMon("Dash Off", false));
            r.RegisterByKey("SVI-103", new ComboMon(new NoRetreatMon("Eye Strength"), new CardsMEG.DefendingEnergyBonusMon("Psychic", 50)));
            r.RegisterByKey("SVI-105", new UnderworldStrollMon());
            r.RegisterByKey("SVI-110", new CardsFLF1.NoResistanceMon("Feint"));
            r.RegisterByKey("SVI-111", new ComboMon(new CardsPOR.MemoryLockMon("Acu-Punch-Ture"), new TailsNothingMon("Kick Shot")));
            r.RegisterByKey("SVI-116", new PaybackMon("Payback", 90));
            r.RegisterByKey("SVI-117", new KrookodileSVI());
            r.RegisterByKey("SVI-120", new SandstormSprayMon());
            r.RegisterByKey("SVI-121", new CardsDRI2.MudCoatMon(20));
            r.RegisterByKey("SVI-122", new AdrenalineHammerMon());
            r.RegisterByKey("SVI-123", new ComboMon(new CardsPAF1.DiscardStadiumMon("Bedrock Breaker"), new RecoilMon("Gigantic Tusks", 50)));
            r.RegisterByKey("SVI-126", new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("SVI-127", new MukSVI());
            r.RegisterByKey("SVI-128", new ComboMon(new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned), new CardsBLK.VenoshockMon(120)));
            r.RegisterByKey("SVI-129", new ComboMon(new CardsPOR.FollowMeMon("Taunt"), new DoomDecreeMon()));
            r.RegisterByKey("SVI-131", new ToxicroakExSVI());
            r.RegisterByKey("SVI-136", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("SVI-138", new ComboMon(new SearchAttackMon("Knickknack Carrying", d => d.IsTool, 3), new NoRetreatMon("Clutch")));
            r.RegisterByKey("SVI-139", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Continuous Spin"), new ShieldAttackMon("Rolling Shell", 50)));
            r.RegisterByKey("SVI-143", new ComboMon(new TripleLaserMon(), new SwitchSelfMon("Cybernetic Wheels", false)));
            r.RegisterByKey("SVI-144", new CardsPFL.FlipUntilTailsTimesMon("Egg Rolling"));
            r.RegisterByKey("SVI-145", new BlisseySVI());
            r.RegisterByKey("SVI-147", new CardsWHT.DragOffMon("Drag Off", 30));
            r.RegisterByKey("SVI-150", new ComboMon(new CardsCRI.BasicShieldAttackMon("Tailspin Away"), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            r.RegisterByKey("SVI-153", new ComboMon(new ExpertNurturerMon(), new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("SVI-154", new CardsASC.PushDownMon("Repelling Odor"));
            r.RegisterByKey("SVI-155", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SVI-157", new CardsSVI1.FlipTailsLockMon("Leg Stomp"));
            r.RegisterByKey("SVI-158", new ComboMon(new MaddeningScentMon("Maddening Scent", 30), new CardsSVI1.FlipTailsLockMon("Heavy Stomp")));
            r.RegisterByKey("SVI-160", new CoinsMon("Double Hit", 2));
            r.RegisterByKey("SVI-162", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new DodgeMon("Fly", true)));
            r.RegisterByKey("SVI-163", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SVI-164", new DrawAttackMon("Touring", 2));
            r.RegisterByKey("SVI-165", new RecoilMon("Nosedive", 20));

            r.RegisterByName("Defiance Band", new DefianceBand());
            r.RegisterByName("Exp. Share", new ExpShare());
            r.RegisterByName("Jacq", new Jacq());
            r.RegisterByName("Katy", new Katy());
            r.RegisterByName("Mesagoza", new Mesagoza());
            r.RegisterByName("Miriam", new Miriam());
            r.RegisterByName("Pal Pad", new PalPad());
            r.RegisterByName("Picnic Basket", new PicnicBasket());
            r.RegisterByName("Rock Chestplate", new RockChestplate());
            r.RegisterByName("Rocky Helmet", new RockyHelmet());
            r.RegisterByName("Vitality Band", new VitalityBand());
            r.RegisterByName("Youngster", new Youngster());
        }

        // ================================================================== Psychic

        /// <summary>Underworld Stroll: the opponent reveals their hand; put a Supporter card from it on the bottom of their deck.</summary>
        private sealed class UnderworldStrollMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Underworld Stroll") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                var supporters = ctx.Opp.Hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                var pick = ctx.ChooseCards(supporters, 1, 1, "Underworld Stroll: put which Supporter card on the bottom of your opponent's deck?");
                foreach (var card in pick)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + pick[0].Def.Name + " on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck.");
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Payback: 90 more damage if the opponent has exactly 1 Prize card remaining. Used for Payback, Knuckle Justice (the attack name and the numbers are passed in).</summary>
        internal sealed class PaybackMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public PaybackMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.Prizes.Count == 1 ? amount : 0));
                return true;
            }
        }

        /// <summary>
        /// Chomp Chomp Bite: flip a coin until you get tails; for each heads discard an Energy from the opponent's Active Pokémon.
        /// Earthquake: 30 damage to each of your Benched Pokémon too.
        /// </summary>
        private sealed class KrookodileSVI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Chomp Chomp Bite")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    int heads = ctx.FlipUntilTails();
                    if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.DiscardEnergyFrom(defending, heads, "Chomp Chomp Bite: discard " + heads + " Energy from " + defending.Name);
                    return true;
                }
                if (atk.Name == "Earthquake")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.MyBench()) ctx.DamageTarget(mon, 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sandstorm Spray: discard 2 Energy from this Pokémon; if you discarded any, the opponent shuffles their Active Pokémon
        /// and all attached cards into their deck.
        /// </summary>
        private sealed class SandstormSprayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sandstorm Spray") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                int discarded = ctx.InPlay(me) ? ctx.DiscardEnergyFrom(me, 2, "Sandstorm Spray: discard 2 Energy from " + me.Name) : 0;
                if (discarded == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                else ctx.Log("The effect of Sandstorm Spray on " + defending.Name + " was prevented.");
                return true;
            }
        }

        /// <summary>Adrenaline Hammer: this Pokémon is now Confused.</summary>
        private sealed class AdrenalineHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Adrenaline Hammer") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Poison Sacs: the opponent's Poisoned Pokémon stay Poisoned when they evolve or devolve, with the same damage counters
        /// (your ruling, v41: Toxic Ripper's 6 stay 6). Toxic Strike: the opponent's Active Pokémon is now Poisoned.
        /// </summary>
        private sealed class MukSVI : Mon
        {
            private static readonly Mon ToxicStrike = new ConditionAttackMon("Toxic Strike", SpecialCondition.Poisoned);

            public override bool KeepsPoisonOnEvolve(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ToxicStrike.HandleAttack(ctx, atk); }
        }

        /// <summary>Doom Decree: flip 2 coins; if both are heads, the opponent's Active Pokémon is Knocked Out.</summary>
        private sealed class DoomDecreeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom Decree") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, "Doom Decree");
                return true;
            }
        }

        /// <summary>
        /// Nasty Plot: search your deck for up to 2 cards and put them into your hand. Toxic Ripper: the opponent's Active Pokémon
        /// is now Poisoned and takes 6 damage counters instead of 1 during Pokémon Checkup.
        /// </summary>
        private sealed class ToxicroakExSVI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Nasty Plot")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 2, "Nasty Plot: choose up to 2 cards", false);
                    return true;
                }
                if (atk.Name == "Toxic Ripper")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 6;
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        /// <summary>Triple Laser: 30 damage to 3 of the opponent's Pokémon (all of them if they have fewer).</summary>
        private sealed class TripleLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Laser") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                int n = Math.Min(3, all.Count);
                foreach (var target in ctx.ChoosePokemonMany(all, n, n, "Triple Laser: choose " + n + " of your opponent's Pokémon to damage"))
                    ctx.DamageTarget(target, 30);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Busybody Nurse: once during your turn, your Active Pokémon recovers from all Special Conditions. Happy Cyclone: move all
        /// Energy from this Pokémon to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class BlisseySVI : Mon
        {
            private static readonly Mon HappyCyclone = new CardsSVI1.MoveAllEnergyToBenchMon("Happy Cyclone");

            protected override string Activated { get { return "Busybody Nurse"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None;
            }

            protected override void Ability(EffectContext ctx) { ctx.Engine.RemoveAllConditions(ctx.Me.Active); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HappyCyclone.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Expert Nurturer: search your deck for a card that evolves from 1 of your Pokémon and put it onto that Pokémon to evolve
        /// it. You may look through your deck first; it works on the first turn and on Pokémon put into play this turn (Compendium,
        /// 2023-03-30).
        /// </summary>
        private sealed class ExpertNurturerMon : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.CantEvolveOnTurn != ctx.Turn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Expert Nurturer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var mine = Targets(ctx);
                if (ctx.Me.Deck.Count == 0 || mine.Count == 0) return true;
                var names = new HashSet<string>(mine.Select(m => m.Name));
                var found = ctx.SearchMyDeck(d => d.IsPokemon && !d.IsLevelUp && d.EvolvesFrom != null && names.Contains(d.EvolvesFrom)
                    && !ctx.Engine.Scripts.For(d).PutIntoPlayOnlyByEffect, 1, "Expert Nurturer: choose a card that evolves from 1 of your Pokémon", false);
                if (found.Count > 0)
                {
                    var card = found[0];
                    var candidates = mine.Where(m => m.Name == card.Def.EvolvesFrom && ctx.InPlay(m)).ToList();
                    var target = candidates.Count == 1 ? candidates[0] : ctx.ChoosePokemon(candidates, "Expert Nurturer: evolve which Pokémon into " + card.Def.Name + "?");
                    if (target != null) ctx.Engine.EvolvePokemon(target, card);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Maddening Scent: 30 more damage for each of the opponent's Benched Pokémon. Used for Maddening Scent, Combat Beak, Combat Thunder (the attack name and the numbers are passed in).</summary>
        internal sealed class MaddeningScentMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public MaddeningScentMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + bonus * ctx.Opp.Bench.Count);
                return true;
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Defiance Band: if you have more Prize cards remaining than your opponent, the holder's attacks do 30 more damage to the opponent's Active Pokémon.</summary>
        private sealed class DefianceBand : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Me.Prizes.Count > src.Opp.Prizes.Count ? damage + 30 : damage;
            }
        }

        /// <summary>Vitality Band: the holder's attacks do 10 more damage to the opponent's Active Pokémon.</summary>
        private sealed class VitalityBand : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 10;
            }
        }

        /// <summary>Rock Chestplate: the [F] Pokémon this card is attached to takes 30 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class RockChestplate : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || damage <= 0) return damage;
                return src.Engine.IsType(holder, EnergyType.Fighting) ? damage - 30 : damage;
            }
        }

        /// <summary>
        /// Rocky Helmet: if the holder is your Active Pokémon and is damaged by an opponent's attack (even if it's Knocked Out), 2
        /// damage counters on the Attacking Pokémon. Only when damage is actually done (Compendium).
        /// </summary>
        private sealed class RockyHelmet : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 2, "Rocky Helmet");
            }
        }

        /// <summary>
        /// Exp. Share: when your Active Pokémon is Knocked Out by damage from an opponent's attack, you may move 1 basic Energy card
        /// that was attached to it to the holder. A Pokémon promoted afterwards comes too late (Compendium).
        /// </summary>
        private sealed class ExpShare : CardScript
        {
            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var holder = src.Pokemon;
                if (!byOpponentsAttack || holder == null || mon == null || mon == holder || !mon.HitWhileActive) return;
                if (!src.InPlay(holder) || src.Engine.IsKnockedOut(holder)) return;
                var basic = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basic.Count == 0) return;
                if (!src.AskYesNo("Exp. Share: move a basic Energy card from " + mon.Name + " to " + holder.Name + "?")) return;
                var pick = basic.Count == 1 ? basic[0] : src.ChooseCards(basic, 1, 1, "Exp. Share: move which Energy?")[0];
                src.Engine.MoveEnergy(mon, holder, pick);
            }
        }

        // ================================================================== Items

        /// <summary>Pal Pad: shuffle up to 2 Supporter cards from your discard pile into your deck.</summary>
        private sealed class PalPad : CardScript
        {
            private static List<CardInstance> Supporters(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Supporters(ctx).Count > 0 && !ctx.Engine.TrainerDiscardToDeckBlocked(ctx.Player);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Supporters(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(2, cards.Count), "Pal Pad: shuffle up to 2 Supporter cards into your deck");
                var moved = ctx.DiscardToDeck(picks);
                if (moved.Count > 0) ctx.Engine.Reveal(ctx.Player, moved, "shuffled these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Picnic Basket: heal 30 damage from each Pokémon (both yours and your opponent's).</summary>
        private sealed class PicnicBasket : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in Hurt(ctx)) ctx.Heal(mon, 30);
            }
        }

        // ================================================================== Supporters

        /// <summary>Jacq: search your deck for up to 2 Evolution Pokémon, reveal them, and put them into your hand.</summary>
        private sealed class Jacq : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution, 2, "Jacq: choose up to 2 Evolution Pokémon");
            }
        }

        /// <summary>Katy: shuffle your hand into your deck, then draw 8 cards. Your turn ends.</summary>
        private sealed class Katy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Any(c => c != ctx.Card);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(8);
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Miriam: shuffle up to 5 Pokémon from your discard pile into your deck; if you shuffled any, draw 3 cards.</summary>
        private sealed class Miriam : CardScript
        {
            private static List<CardInstance> Pokemon(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pokemon(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Pokemon(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(5, cards.Count), "Miriam: shuffle up to 5 Pokémon from your discard pile into your deck");
                var moved = ctx.DiscardToDeck(picks);
                if (moved.Count > 0) ctx.Engine.Reveal(ctx.Player, moved, "shuffled these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck");
                ctx.ShuffleMyDeck();
                if (moved.Count > 0) ctx.Draw(3);
            }
        }

        /// <summary>Youngster: shuffle your hand into your deck, then draw 5 cards.</summary>
        private sealed class Youngster : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Any(c => c != ctx.Card);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
            }
        }

        // ================================================================== Stadium

        /// <summary>Mesagoza: once during each player's turn, they may flip a coin; on heads they search their deck for a Pokémon and put it into their hand.</summary>
        private sealed class Mesagoza : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(d => d.IsPokemon, 1, "Mesagoza: search your deck for a Pokémon");
                return false;
            }
        }
    }
}
