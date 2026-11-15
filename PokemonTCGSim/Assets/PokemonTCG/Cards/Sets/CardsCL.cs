using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Call of Legends (CL, 2011; v173): the cards not already in the pool (most of the set reprints earlier HGSS cards). Unlimited
    /// only. The shiny legendary Pokémon (SL1-SL11) share their scripts with the regular cards of the same name.
    /// </summary>
    public static class CardsCL
    {
        private static Mon ExtremeSpeed(EnergyType type, Mon attacks) { return new ExtremeSpeedMon(type, attacks); }

        public static void Register(CardScriptRegistry r)
        {
            var deoxys = new Fx().After("Cell Storm", (c, a) =>
            {
                DiscardTypeFromSelf(c, EnergyType.Psychic, 2);
                if (c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, 60);
            });
            var dialga = new Fx().After("Time Rewind", (c, a) =>
            {
                foreach (var x in c.Me.Hand.ToList()) { c.Me.Hand.Remove(x); c.Me.Deck.Add(x); }
                c.ShuffleMyDeck();
            });
            var groudon = new Fx().After("Volcano Stomp", (c, a) => c.Engine.DiscardTopOfDeck(c.Flip() ? c.Opponent : c.Player, 4));
            var hoOh = new Fx().After("Scorching Wing", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 99); });
            var kyogre = new Fx().Effect("Destructive Tsunami", (c, a) =>
            {
                var targets = c.Flip() ? c.OpponentPokemon() : c.MyPokemon();
                foreach (var m in targets.ToList()) if (c.InPlay(m)) c.DamageTarget(m, 40);
            });
            var lugia = new Fx().Effect("Linear Attack", (c, a) => HitAny(c, 30, "Linear Attack"));
            var palkia = new Fx().After("Wormhole", (c, a) =>
            {
                if (c.InPlay(c.Pokemon) && c.MyBench().Count > 0) c.SwitchSelfWithBench();
                if (c.Opp.Bench.Count > 0) c.OpponentSwitchesActive();
            });
            var rayquaza = new Fx().After("Inferno Spear", (c, a) =>
            {
                DiscardTypeFromSelf(c, EnergyType.Fire, 1);
                DiscardTypeFromSelf(c, EnergyType.Lightning, 1);
            });
            foreach (var k in new[] { "CL-2", "CL-SL1" }) r.RegisterByKey(k, deoxys);
            foreach (var k in new[] { "CL-3", "CL-SL2" }) r.RegisterByKey(k, dialga);
            foreach (var k in new[] { "CL-6", "CL-SL4" }) r.RegisterByKey(k, groudon);
            foreach (var k in new[] { "CL-9", "CL-SL5" }) r.RegisterByKey(k, hoOh);
            foreach (var k in new[] { "CL-12", "CL-SL6" }) r.RegisterByKey(k, kyogre);
            foreach (var k in new[] { "CL-15", "CL-SL7" }) r.RegisterByKey(k, lugia);
            foreach (var k in new[] { "CL-19", "CL-SL8" }) r.RegisterByKey(k, palkia);
            foreach (var k in new[] { "CL-20", "CL-SL10" }) r.RegisterByKey(k, rayquaza);
            r.RegisterByKey("CL-SL3", ExtremeSpeed(EnergyType.Fire, new Fx().After("Wild Blaze", (c, a) => c.Engine.DiscardTopOfDeck(c.Player, 3))));
            r.RegisterByKey("CL-SL9", ExtremeSpeed(EnergyType.Lightning, new Fx().After("Raging Thunder", (c, a) =>
            {
                var t = c.ChoosePokemon(c.MyPokemon(), "Raging Thunder: do 20 damage to which of your Pokémon?");
                if (t != null) c.DamageWith(t, 20, false, false, false);
            })));
            r.RegisterByKey("CL-SL11", ExtremeSpeed(EnergyType.Water, new Fx().Effect("Tsunami", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 20); })));

            r.RegisterByKey("CL-13", new Fx()
                .Damage("Miasma Wind", (c, a) => 50 * (c.Defending == null ? 0 : CardsSV.ConditionCount(c.Defending)))
                .With(new ConditionAttackMon("Sooting Scent", SpecialCondition.Asleep)));
            r.RegisterByKey("CL-14", new Fx()
                .Damage("Dimension Sphere", (c, a) => a.Damage + 20 * c.Me.LostZone.Count(x => x.Def.IsPokemon))
                .Effect("Sky Uppercut", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, true, false, false); }));
            r.RegisterByKey("CL-18", new BenchTriggerFx("Self-Generation", c => c.Me.Hand.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Lightning)), c =>
                {
                    var hand = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Lightning)).ToList();
                    foreach (var x in c.ChooseCards(hand, 0, Math.Min(2, hand.Count), "Self-Generation: attach up to 2 Lightning Energy cards to " + c.Pokemon.Name))
                        if (c.Me.Hand.Remove(x) && !c.Engine.AttachEnergyCard(x, c.Pokemon, true) && !c.Me.Discard.Contains(x)) c.Me.Hand.Add(x);
                }, new Fx().After("Shocking Bolt", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) foreach (var e in c.Pokemon.Energy.ToList()) EnergyToLostZone(c, c.Pokemon, e);
                })));
            r.RegisterByKey("CL-29", new PowerFx("Trick Reveal", null, c =>
                {
                    c.Engine.ShowCards(c.Player, c.Opp.Hand.ToList(), "Trick Reveal", "Trick Reveal: your opponent's hand", c.Source);
                    c.Engine.ShowCards(c.Opponent, c.Me.Hand.ToList(), "Trick Reveal", "Trick Reveal: your opponent's hand", c.Source);
                    c.Log("Trick Reveal: both players revealed their hands.");
                }, new CoinsMon("Juggling", 4)));
            r.RegisterByKey("CL-33", new Fx()
                .Effect("Layabout", (c, a) =>
                {
                    if (c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, c.Pokemon.DamageCounters * 10);
                    c.SelfCantUseAttackNextTurn("Layabout");
                })
                .After("Clomp Clomp Clobber", (c, a) =>
                {
                    if (!c.InPlay(c.Pokemon) || c.Pokemon.Energy.Count == 0) return;
                    var e = c.Pokemon.Energy.Count == 1 ? c.Pokemon.Energy[0] : c.ChooseCards(c.Pokemon.Energy.ToList(), 1, 1, "Clomp Clomp Clobber: put which Energy card in the Lost Zone?")[0];
                    EnergyToLostZone(c, c.Pokemon, e);
                }));
            r.RegisterByKey("CL-34", new Fx()
                .Damage("Grind", (c, a) => 20 * c.EnergyCount(c.Pokemon))
                .After("Plow Over", (c, a) =>
                {
                    var d = c.Defending;
                    if (d == null || !c.InPlay(d) || !c.CanAffect(d)) return;
                    if (c.Flip()) { c.ApplyCondition(d, SpecialCondition.Paralyzed); return; }
                    if (d.Energy.Count == 0) return;
                    var e = d.Energy.Count == 1 ? d.Energy[0] : c.ChooseCards(d.Energy.ToList(), 1, 1, "Plow Over: put which Energy card in the Lost Zone?")[0];
                    EnergyToLostZone(c, d, e);
                }));
            r.RegisterByKey("CL-39", new Fx()
                .Effect("Swords Dance", (c, a) => c.Pokemon.Markers["Swords Dance"] = c.Turn + 2)
                .On("Lost Claw", (c, a) =>
                {
                    int t;
                    c.DamageDefending(c.Pokemon.Markers.TryGetValue("Swords Dance", out t) && t == c.Turn ? 80 : a.Damage);
                    var card = RandomFromHand(c, c.Opp);
                    if (card != null && c.Opp.Hand.Remove(card)) CardsCRZ1.PutInLostZone(c, card);
                    return true;
                }));
            r.RegisterByKey("CL-47", new SweetSleepingFaceMon(new Fx().Effect("Sleepy Lost", (c, a) =>
                {
                    if (c.Opp.Deck.Count > 0)
                    {
                        var top = c.Opp.Deck[0];
                        c.Opp.Deck.RemoveAt(0);
                        CardsCRZ1.PutInLostZone(c, top);
                    }
                    c.ApplyCondition(c.Pokemon, SpecialCondition.Asleep);
                })));
            r.RegisterByKey("CL-50", new LockAfterAttackMon("Tumble Over", false));
            r.RegisterByKey("CL-51", new Fx()
                .Effect("Poison Buildup", (c, a) => c.ApplyCondition(c.Pokemon, SpecialCondition.Poisoned))
                .On("Poison Effect", (c, a) =>
                {
                    bool poisoned = c.Pokemon.Has(SpecialCondition.Poisoned);
                    c.DamageDefending(a.Damage + (poisoned ? 60 : 0));
                    if (poisoned && c.InPlay(c.Pokemon)) c.Engine.RemoveCondition(c.Pokemon, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("CL-53", new ComboMon(new CardsTWM1.HealSelfMon("Nap", 10), new RecoilMon("Reckless Charge", 10)));
            r.RegisterByKey("CL-55", new Fx().After("Fireworks", (c, a) => { if (c.InPlay(c.Pokemon) && !c.Flip()) DiscardTypeFromSelf(c, EnergyType.Fire, 1); }));
            r.RegisterByKey("CL-66", new PhanpyUltraThickSkin());
            r.RegisterByKey("CL-69", new Fx()
                .Effect("Prehistoric Wisdom", (c, a) =>
                {
                    if (c.Me.Hand.Count == 0) { DidNothing(c); return; }
                    var card = c.ChooseCards(c.Me.Hand.ToList(), 1, 1, "Prehistoric Wisdom: put which card from your hand in the Lost Zone?")[0];
                    c.Me.Hand.Remove(card);
                    CardsCRZ1.PutInLostZone(c, card);
                    c.Draw(3);
                })
                .With(new ShieldAttackMon("Granite Head", 30)));
            r.RegisterByKey("CL-70", new TailsNothingMon("Whismy Tackle"));
            r.RegisterByKey("CL-72", new Fx()
                .Effect("Nutritional Support", (c, a) => SearchAttach(c, d => CardsRS.BasicOf(d, EnergyType.Grass), "Nutritional Support"))
                .With(new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("CL-73", new Fx().Effect("Fake Tears", (c, a) =>
                {
                    if (!c.Flip()) return;
                    c.AddEffect(new ItemLockEffect(c.Engine.PlayerName(c.Opponent) + " can't play Trainer cards during their next turn (Fake Tears)", c.Player, c.Opponent, c.Turn + 1));
                    c.AddEffect(new TakenChangeEffect(c.Pokemon.Name + " takes 30 less damage next turn (Fake Tears)", c.Player, c.Pokemon.Uid, c.Turn + 1, -30));
                }));
            r.RegisterByKey("CL-74", new Fx().Damage("Aqua Tail", (c, a) => a.Damage + 20 * c.FlipCoins(c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water))));

            r.RegisterByKey("CL-80", new CardsUL.SimpleTrainer(c => c.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsSpecialEnergy)).Count > 0, c =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsSpecialEnergy)), "Lost Remover: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    var special = t.Energy.Where(e => e.Def.IsSpecialEnergy).ToList();
                    var e2 = special.Count == 1 ? special[0] : c.ChooseCards(special, 1, 1, "Lost Remover: put which Special Energy card in the Lost Zone?")[0];
                    EnergyToLostZone(c, t, e2);
                }));
            r.RegisterByKey("CL-81", new LostWorld());
            r.RegisterByKey("CL-84", new CardsUL.SimpleTrainer(c => c.Me.Deck.Count > 0, c =>
                {
                    var top = c.Me.Deck.Take(4).ToList();
                    foreach (var x in top) c.Me.Deck.Remove(x);
                    var keep = c.ChooseCards(top, 0, top.Count, "Research Record: keep which cards on top of your deck? (the rest go to the bottom)");
                    var bottom = top.Where(x => !keep.Contains(x)).ToList();
                    if (keep.Count > 0) CardsCRZ1.PutOnTopInOrder(c, keep, "Research Record");
                    c.Me.Deck.AddRange(bottom);
                }));
        }

        /// <summary>Extreme Speed (Poké-Body): Retreat Cost [C] less for each Energy of a type attached.</summary>
        private sealed class ExtremeSpeedMon : BodyMon
        {
            private readonly EnergyType type;

            public ExtremeSpeedMon(EnergyType type, Mon attacks) : base(attacks) { this.type = type; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon != src.Pokemon || !Works(src, "Extreme Speed")) return cost;
                return Math.Max(0, cost - src.Engine.EnergyCountOfType(mon, type));
            }
        }

        /// <summary>Ultra-Thick Skin (Poké-Body): with any Energy attached, attack damage to it is 10 less. Rock Smash.</summary>
        private sealed class PhanpyUltraThickSkin : BodyMon
        {
            public PhanpyUltraThickSkin() : base(new FlipBonusMon("Rock Smash", 10)) { }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || src.Pokemon.Energy.Count == 0 || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !Works(src, "Ultra-Thick Skin")) return damage;
                return Math.Max(0, damage - 10);
            }
        }

        /// <summary>The player can't play Trainer cards (Items) from their hand (Fake Tears).</summary>
        private sealed class ItemLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public ItemLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected && card != null && CardsDPEra.DpTrainer(card.Def); }
        }

        /// <summary>Lost World (Stadium): once during each player's turn, if their opponent has 6 or more Pokémon in the Lost Zone, they may win the game.</summary>
        private sealed class LostWorld : CardScript
        {
            public override bool CanUseStadium(EffectContext ctx) { return ctx.Opp.LostZone.Count(c => c.Def.IsPokemon) >= 6; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.Engine.EndGame(ctx.Player, "Lost World");
                return true;
            }
        }
    }
}
