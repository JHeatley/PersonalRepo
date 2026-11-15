using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>HS—Triumphant (TM; v173): Trainers, Rescue Energy, the Pokémon Prime, Palkia &amp; Dialga LEGEND and Alph Lithograph FOUR.</summary>
    public static partial class CardsTM
    {
        private static void RegisterTrainersAndPrimes(CardScriptRegistry r)
        {
            r.RegisterByKey("TM-85", new CardsUL.SimpleTrainer(c => c.Me.Prizes.Count > c.Opp.Prizes.Count, c =>
                c.AddEffect(new BlackBeltEffect("Your Active Pokémon's attacks do 40 more damage this turn (Black Belt)", c.Player, c.Turn))));
            r.RegisterByKey("TM-86", new IndigoPlateau());
            r.RegisterByKey("TM-87", new CardsUL.SimpleTrainer(c => c.Me.Hand.Count(x => x != c.Card) >= 2 && c.Me.Discard.Any(x => x.Def.IsTrainer && x.Def.Name != "Junk Arm"), c =>
                {
                    // Only a Trainer that was in the discard pile before (not one just discarded for it: PUI Rules Team, 2011-11-03).
                    var before = c.Me.Discard.Where(x => x.Def.IsTrainer && x.Def.Name != "Junk Arm").ToList();
                    foreach (var x in c.ChooseCards(c.Me.Hand.Where(x => x != c.Card).ToList(), 2, 2, "Junk Arm: discard 2 cards from your hand")) c.Engine.DiscardFromHand(c.Player, x);
                    var pile = before.Where(x => c.Me.Discard.Contains(x)).ToList();
                    if (pile.Count > 0) c.DiscardToHand(c.ChooseCards(pile, 1, 1, "Junk Arm: put which Trainer card into your hand?"));
                }));
            r.RegisterByKey("TM-88", new CardsUL.SimpleTrainer(c => c.Me.Bench.Count > 0 || c.Opp.Bench.Count > 0, c =>
                {
                    if (c.MyBench().Count > 0)
                    {
                        var mine = c.ChoosePokemon(c.MyBench(), "Seeker: return which of your Benched Pokémon to your hand?");
                        if (mine != null) c.Engine.ReturnPokemonToHand(mine);
                    }
                    if (c.Opp.Bench.Count > 0)
                    {
                        var theirs = ChooseMonsFor(c, c.Opponent, c.Opp.Bench.ToList(), 1, 1, "Seeker: return which of your Benched Pokémon to your hand?");
                        if (theirs.Count > 0) c.Engine.ReturnPokemonToHand(theirs[0]);
                    }
                }));
            r.RegisterByKey("TM-89", new CardsUL.SimpleTrainer(c => c.Me.Prizes.Count > c.Opp.Prizes.Count && c.Me.Deck.Count > 0, c =>
                c.SearchToHand(d => true, 2, "Twins: search your deck for any 2 cards", true, Math.Min(2, c.Me.Deck.Count))));
            r.RegisterByKey("TM-90", new RescueEnergy());

            r.RegisterByKey("TM-91", new AbsolEyeOfDisaster());
            r.RegisterByKey("TM-92", new PowerFx("Forest Breath", c => c.PokemonIsActive && c.Me.Hand.Any(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)), c =>
                {
                    var grass = c.Me.Hand.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Grass)).ToList();
                    c.AttachCardsAnyWay(grass, 1, (x, m) => true, "Forest Breath: attach a Grass Energy card from your hand", false);
                }, new Fx().After("Time Circle", (c, a) =>
                {
                    if (c.InPlay(c.Pokemon)) c.AddEffect(new ShieldFromEffect(c.Pokemon.Name + " is protected from Stage 1 and Stage 2 Pokémon (Time Circle)", c.Player, c.Pokemon.Uid, c.Turn + 1,
                        (eng, m) => m.Def.Stage == Stage.Stage1 || m.Def.Stage == Stage.Stage2));
                })));
            r.RegisterByKey("TM-93", new ElectrodeEnergymite());
            // Only with Fighting Energy to move (PUI Rules Team, 2010-11-18).
            r.RegisterByKey("TM-95", new PowerFx("Fighting Tag", c => c.Me.Bench.Contains(c.Pokemon) && c.Me.Active != null && c.Me.Active.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Me.Active, e, EnergyType.Fighting)), c =>
                {
                    var active = c.Me.Active;
                    foreach (var e in active.Energy.Where(e => CardsDRI1.ProvidesType(c, active, e, EnergyType.Fighting)).ToList()) c.Engine.MoveEnergy(active, c.Pokemon, e);
                    c.Engine.SwitchActive(c.Player, c.Pokemon);
                }, new Fx()
                .After("Crushing Punch", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.InPlay(d) && c.CanAffect(d) && d.Energy.Any(e => e.Def.IsSpecialEnergy))
                        c.DiscardEnergyFrom(d, 1, "Crushing Punch: discard which Special Energy card?", e => e.Def.IsSpecialEnergy);
                })
                .Damage("Champ Buster", (c, a) => a.Damage + 10 * c.MyBench().Count(m => m.DamageCounters > 0))));
            r.RegisterByKey("TM-96", new PowerFx("Magnetic Draw", c => c.Me.Hand.Count < 6 && c.Me.Deck.Count > 0, c => c.DrawUntil(6), new Fx()
                .Effect("Lost Burn", (c, a) =>
                {
                    var holders = new List<KeyValuePair<PokemonInPlay, CardInstance>>();
                    foreach (var m in c.MyPokemon()) foreach (var e in m.Energy) holders.Add(new KeyValuePair<PokemonInPlay, CardInstance>(m, e));
                    var cards = holders.Select(p => p.Value).ToList();
                    var picks = cards.Count == 0 ? new List<CardInstance>() : c.ChooseCards(cards, 0, cards.Count, "Lost Burn: put any Energy cards attached to your Pokémon in the Lost Zone (50 damage each)");
                    c.DamageDefending(50 * picks.Count);
                    foreach (var e in picks) EnergyToLostZone(c, holders.First(p => p.Value == e).Key, e);
                })));
            r.RegisterByKey("TM-97", new MewLostLink());
            r.RegisterByKey("TM-98", new YanmegaInsight());

            var palkiaDialga = new Fx()
                .Effect("Sudden Delete", (c, a) =>
                {
                    if (c.OpponentBench().Count == 0) return;
                    var t = c.ChoosePokemon(c.OpponentBench(), "Sudden Delete: return which of your opponent's Benched Pokémon to their hand?");
                    if (t != null && c.CanAffect(t)) c.Engine.ReturnPokemonToHand(t);
                })
                .Effect("Time Control", (c, a) =>
                {
                    DiscardTypeFromSelf(c, EnergyType.Metal, 99);
                    for (int i = 0; i < 2 && c.Opp.Deck.Count > 0; i++)
                    {
                        var top = c.Opp.Deck[0];
                        c.Opp.Deck.RemoveAt(0);
                        c.Opp.Prizes.Add(top);
                    }
                    c.Log("Time Control: the top 2 cards of " + c.Engine.PlayerName(c.Opponent) + "'s deck were added to their Prize cards.");
                });
            r.RegisterByKey("TM-101", palkiaDialga);
            r.RegisterByKey("TM-102", palkiaDialga);
            r.RegisterByKey("TM-FOUR", new CardsHS.AlphLithograph(c =>
                {
                    var faceDown = c.Me.Prizes.Where(p => !c.Me.FaceUpPrizeIds.Contains(p.Id)).ToList();
                    c.Engine.ShowCards(c.Player, faceDown, "Alph Lithograph", "Alph Lithograph: your face-down Prize cards", c.Source);
                    c.Log(c.Engine.PlayerName(c.Player) + " looked at their face-down Prize cards (Alph Lithograph).");
                }, c => c.Me.Prizes.Count > 0));
        }

        /// <summary>Black Belt: this turn your Active Pokémon's attacks do 40 more to the opponent's Active Pokémon (before W/R).</summary>
        private sealed class BlackBeltEffect : ContinuousEffect
        {
            public BlackBeltEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Owner != Owner || target == null || target.Owner == Owner || damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (engine.State.Players[Owner].Active != attacker || engine.State.Players[target.Owner].Active != target) return damage;
                return damage + 40;
            }
        }

        /// <summary>Indigo Plateau (Stadium): each Pokémon LEGEND in play gets +30 HP.</summary>
        private sealed class IndigoPlateau : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon != null && mon.Def.IsLegend ? hp + 30 : hp; }
        }

        /// <summary>Rescue Energy: [C]; when the Pokémon is Knocked Out by attack damage, it goes back to your hand (its attached cards are discarded).</summary>
        private sealed class RescueEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.Colorless }; }

            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return mon != null && mon == src.Pokemon && (byOpponentsAttack || src.Engine.ResolvingAttack != null);
            }
        }

        /// <summary>Eye of Disaster (Poké-Body): while Absol is your Active Pokémon, a Basic Pokémon the opponent benches from their hand gets 2 damage counters. Vicious Claw.</summary>
        private sealed class AbsolEyeOfDisaster : BodyMon
        {
            public AbsolEyeOfDisaster() : base(new Fx().On("Vicious Claw", (c, a) =>
                {
                    var mons = c.Me.Hand.Where(x => x.Def.IsPokemon).ToList();
                    if (mons.Count == 0) return DidNothing(c);
                    var card = c.ChooseCards(mons, 1, 1, "Vicious Claw: put which Pokémon from your hand in the Lost Zone?")[0];
                    c.Me.Hand.Remove(card);
                    CardsCRZ1.PutInLostZone(c, card);
                    c.DamageDefending(a.Damage);
                    return true;
                })) { }

            public override void OnPokemonBenchedFromHand(EffectContext src, PokemonInPlay benched)
            {
                if (benched == null || benched.Owner == src.Player || !benched.Def.IsBasicPokemon || !src.PokemonIsActive || !Works(src, "Eye of Disaster")) return;
                src.Engine.PlaceDamageCounters(benched, 2, "Eye of Disaster");
            }
        }

        /// <summary>Energymite (Poké-Power): Electrode is Knocked Out; attach any Energy in your top 7 cards to your Pokémon, discard the rest. Gigashock.</summary>
        private sealed class ElectrodeEnergymite : BodyMon
        {
            public ElectrodeEnergymite() : base(new Fx().After("Gigashock", (c, a) => HitBenched(c, 2, 10, "Gigashock"))) { }

            protected override string Activated { get { return "Energymite"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx); }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Energymite"; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.KnockOut(ctx.Pokemon, "Energymite");
                var top = ctx.LookAtTop(7);
                var energy = top.Where(x => x.Def.IsEnergy).ToList();
                if (energy.Count > 0 && ctx.MyPokemon().Count > 0) ctx.AttachCardsAnyWay(energy, energy.Count, (x, m) => true, "Energymite: attach Energy cards to your Pokémon", true);
                foreach (var x in top) if (ctx.Me.Deck.Remove(x)) ctx.Me.Discard.Add(x);
            }
        }

        /// <summary>Lost Link (Poké-Body): Mew may use the attacks of the Pokémon in both Lost Zones. See Off.</summary>
        private sealed class MewLostLink : BodyMon
        {
            public MewLostLink() : base(new Fx().Effect("See Off", (c, a) =>
                {
                    var picks = c.SearchMyDeck(d => d.IsPokemon, 1, "See Off: search your deck for a Pokémon to put in the Lost Zone");
                    foreach (var x in picks) CardsCRZ1.PutInLostZone(c, x);
                    c.ShuffleMyDeck();
                })) { }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (!Works(src, "Lost Link")) return list;
                var seen = new HashSet<string>();
                foreach (var card in src.Me.LostZone.Concat(src.Opp.LostZone))
                {
                    var d = card.Def;
                    // Not a LEGEND half: a LEGEND out of play has no attacks (HS:Triumphant FAQ).
                    if (!d.IsPokemon || d.IsLegend || !seen.Add(d.BaseKey)) continue;
                    for (int i = 0; i < d.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(d, i));
                }
                return list;
            }
        }

        /// <summary>Insight (Poké-Body): with as many cards in your hand as the opponent, its attacks cost nothing. Linear Attack. Sonicboom.</summary>
        private sealed class YanmegaInsight : BodyMon
        {
            public YanmegaInsight() : base(new Fx()
                .Effect("Linear Attack", (c, a) => HitAny(c, 40, "Linear Attack"))
                .Effect("Sonicboom", (c, a) => { if (c.Defending != null) c.DamageWith(c.Defending, a.Damage, false, false, false); })) { }

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (ctx.Me.Hand.Count == ctx.Opp.Hand.Count && Works(ctx, "Insight")) cost.Clear();
            }
        }
    }
}
