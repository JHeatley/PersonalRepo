using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Pokémon 151 (MEW) 100-165: the Pokémon, Trainers and Antique Fossils that weren't already in the pool (all G-mark,
    /// Expanded only). Reprints already in the pool (Voltorb, Electrode, Kangaskhan ex, Mr. Mime's alternate print, Jynx ex's
    /// alternate print, Vaporeon, Jolteon, Flareon, Snorlax, Zapdos ex, Mewtwo, Mew ex...) are alternate prints. Pokémon whose
    /// attacks only do their printed damage need no script: Horsea, Dratini.
    /// </summary>
    public static class CardsMEW2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEW-102", new CardsPFL.FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("MEW-103", new CardsMEG.DefendingEnergyBonusMon("Psychic", 30));
            r.RegisterByKey("MEW-104", new CuboneMEW());
            r.RegisterByKey("MEW-105", new ComboMon(new Cards30CC.BenchHitMon("Bone Throw", 30), new LockAfterAttackMon("Boundless Power", false)));
            r.RegisterByKey("MEW-106", new HitmonleeMEW());
            r.RegisterByKey("MEW-107", new CardsASC.CounterattackMon("Counterattack", 3, new CardsASC.NextTurnBoostMon("Excited Punch", 60)));
            r.RegisterByKey("MEW-108", new CardsBLK.NoAttackNextTurnMon("Tongue-Tied"));
            r.RegisterByKey("MEW-109", new ConditionAttackMon("Suspicious Gas", SpecialCondition.Confused));
            r.RegisterByKey("MEW-110", new WeezingMEW());
            r.RegisterByKey("MEW-111", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("MEW-112", new SupporterBonusMon("Charismatic Drill", "Giovanni's Charisma", 140));
            r.RegisterByKey("MEW-113", new ChanseyMEW());
            r.RegisterByKey("MEW-114", new SupporterBonusMon("Tactful Tangling", "Erika's Invitation", 60));
            r.RegisterByKey("MEW-117", new SeadraMEW());
            r.RegisterByKey("MEW-118", new CoinsMon("Triple Strike", 3));
            r.RegisterByKey("MEW-119", new ComboMon(new DodgeMon("Swim Freely", false), new CardsMEG.EnergyBonusMon("Aqua Horn", EnergyType.Water, 30)));
            r.RegisterByKey("MEW-122", new MrMimeMEW());
            r.RegisterByKey("MEW-123", new ScytherMEW());
            r.RegisterByKey("MEW-124", new ComboMon(new HeartStoppingKissMon(), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("MEW-125", new PartnerBonusMon("Electro Combo", "Magmar", 40));
            r.RegisterByKey("MEW-126", new PartnerBonusMon("Flare Combo", "Electabuzz", 80));
            r.RegisterByKey("MEW-127", new BehindBonusMon("Reckless Throw", 90));
            r.RegisterByKey("MEW-128", new ComboMon(new BenchSearchAttackMon("Gather the Crew", 1), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("MEW-129", new CardsSVP.FlatOutDashMon("Splashy Splash"));
            r.RegisterByKey("MEW-130", new GyaradosMEW());
            r.RegisterByKey("MEW-131", new SearchAttackMon("Hop on My Back", d => d.IsPokemon, 2));
            r.RegisterByKey("MEW-133", new EeveeMEW());
            r.RegisterByKey("MEW-137", new PorygonMEW());
            r.RegisterByKey("MEW-138", new OmanyteMEW());
            r.RegisterByKey("MEW-139", new OmastarMEW());
            r.RegisterByKey("MEW-140", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("MEW-141", new KabutopsMEW());
            r.RegisterByKey("MEW-142", new AerodactylMEW());
            r.RegisterByKey("MEW-144", new CardsSVP.FloatMon(EnergyType.Water, new CardsMEW1.SpreadBenchMon("Blizzard", 10)));
            r.RegisterByKey("MEW-146", new CardsSVP.FloatMon(EnergyType.Fire, new BlazingFlightMon()));
            r.RegisterByKey("MEW-148", new LockAfterAttackMon("Aqua Slash", false));
            r.RegisterByKey("MEW-149", new DragoniteMEW());

            r.RegisterByName("Antique Dome Fossil", new PlainFossil());
            r.RegisterByName("Antique Helix Fossil", new PlainFossil());
            r.RegisterByName("Antique Old Amber", new PlainFossil());
            r.RegisterByName("Big Air Balloon", new BigAirBalloon());
            r.RegisterByName("Bill's Transfer", new BillsTransfer());
            r.RegisterByName("Cycling Road", new CyclingRoad());
            r.RegisterByName("Daisy's Help", new DaisysHelp());
            r.RegisterByName("Energy Sticker", new EnergySticker());
            r.RegisterByName("Erika's Invitation", new ErikasInvitation());
            r.RegisterByName("Giovanni's Charisma", new GiovannisCharisma());
            r.RegisterByName("Grabber", new Grabber());
            r.RegisterByName("Leftovers", new Leftovers());
            r.RegisterByName("Protective Goggles", new ProtectiveGoggles());
            r.RegisterByName("Rigid Band", new RigidBand());
        }

        // ================================================================== Shared simple attacks

        /// <summary>"If [partner] is on your Bench, this attack does N more damage" (Electro Combo, Flare Combo).</summary>
        private sealed class PartnerBonusMon : Mon
        {
            private readonly string attack;
            private readonly string partner;
            private readonly int bonus;

            public PartnerBonusMon(string attack, string partner, int bonus)
            {
                this.attack = attack;
                this.partner = partner;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool there = ctx.Me.Bench.Any(m => m.Name == partner);
                ctx.DamageDefending(atk.Damage + (there ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If you played [Supporter] from your hand during this turn, this attack does N more damage" (Charismatic Drill, Tactful Tangling).</summary>
        private sealed class SupporterBonusMon : Mon
        {
            private readonly string attack;
            private readonly string supporter;
            private readonly int bonus;

            public SupporterBonusMon(string attack, string supporter, int bonus)
            {
                this.attack = attack;
                this.supporter = supporter;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool played = ctx.Me.PlayedSupporterThisTurn(supporter);
                ctx.DamageDefending(atk.Damage + (played ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If you have more Prize cards remaining than your opponent, this attack does N more damage."</summary>
        private sealed class BehindBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public BehindBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool behind = ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
                ctx.DamageDefending(atk.Damage + (behind ? bonus : 0));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Cheering Bone: while this Pokémon is on your Bench, attacks used by your Marowak do 30 more damage to the opponent's
        /// Active Pokémon (each Cubone adds 30). Hit Twice: flip 2 coins, 10 damage for each heads.
        /// </summary>
        private sealed class CuboneMEW : Mon
        {
            private static readonly Mon HitTwice = new CoinsMon("Hit Twice", 2);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || src.PokemonIsActive) return damage;
                if (info.Attacker == null || info.Attacker.Owner != src.Player || info.Attacker.Name != "Marowak" || !info.ToOpponentsActive) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HitTwice.HandleAttack(ctx, atk); }
        }

        /// <summary>Twister Kick: 10 damage to each of your opponent's Pokémon, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class HitmonleeMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister Kick") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 10);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Let's Have a Blast: if this Pokémon is Knocked Out in the Active Spot by damage from an opponent's attack, flip a coin;
        /// on heads, the Attacking Pokémon is Knocked Out. Spinning Fumes: 10 damage to each of the opponent's Benched Pokémon too.
        /// </summary>
        private sealed class WeezingMEW : Mon
        {
            private static readonly Mon Fumes = new CardsMEW1.SpreadBenchMon("Spinning Fumes", 10);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                Announce(src, "Let's Have a Blast");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Let's Have a Blast");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fumes.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Lucky Bonus: taken as a face-down Prize card during your turn with room on your Bench, you may put it onto your Bench
        /// instead; then flip a coin and on heads take 1 more Prize card. It works from the Prize cards, so Ability blockers in
        /// play don't stop it, and a Chansey taken as that extra Prize card can use it too (your rulings, v38).
        /// </summary>
        private sealed class ChanseyMEW : Mon
        {
            public override int OnTakenAsPrize(EffectContext ctx)
            {
                if (!ctx.Engine.BenchHasRoom(ctx.Player)) return -1;
                if (!ctx.AskYesNo("Lucky Bonus: put Chansey onto your Bench instead of into your hand?")) return -1;
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put Chansey onto the Bench (Lucky Bonus).");
                bool heads = ctx.Flip();
                if (heads) ctx.Log("Lucky Bonus: heads, " + ctx.Engine.PlayerName(ctx.Player) + " takes 1 more Prize card.");
                return heads ? 1 : 0;
            }
        }

        // ================================================================== Water

        /// <summary>Blinding Ink: during the opponent's next turn, if the Defending Pokémon tries to attack, they flip 2 coins; if either is tails, the attack doesn't happen.</summary>
        private sealed class SeadraMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blinding Ink") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsCRI.AttackCoinsEffect(defending.Name + " must flip 2 heads to attack during its next turn (Blinding Ink)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 2));
                return true;
            }
        }

        /// <summary>Untamed One: when you play this Pokémon from your hand to evolve, you must discard the top 5 cards of your deck. Hyper Beam: discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class GyaradosMEW : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.IsMyTurn || ctx.Me.Deck.Count == 0) return;
                Announce(ctx, "Untamed One");
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Beam") return false;
                CardsDRI2.CrunchMon.DiscardDefendingEnergy(ctx, atk);
                return true;
            }
        }

        /// <summary>Tentacular Return: put an Energy attached to the opponent's Active Pokémon into their hand.</summary>
        private sealed class OmanyteMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacular Return") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0) return true;
                var pick = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Tentacular Return: put which Energy into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, pick);
                return true;
            }
        }

        /// <summary>Primordial Tentacles: while this Pokémon is Active, the opponent's Active Pokémon can't retreat. Aqua Split: 30 damage to 2 of the opponent's Benched Pokémon too.</summary>
        private sealed class OmastarMEW : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon.Owner != src.Player && src.State.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Split") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var t in ctx.ChoosePokemonMany(bench, n, n, "Aqua Split: choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(t, 30);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Mimic Barrier: if this Pokémon and the opponent's Active Pokémon have the same amount of Energy attached (Energy
        /// provided, as everywhere in the engine), prevent all damage done to this Pokémon by the opponent's attacks.
        /// Psypower: put 3 damage counters on the opponent's Pokémon in any way you like.
        /// </summary>
        private sealed class MrMimeMEW : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !info.AgainstOpponent) return false;
                var opp = src.Opp.Active;
                return opp != null && src.Engine.EnergyCount(me) == src.Engine.EnergyCount(opp);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psypower") return false;
                ctx.PlaceCountersAnyWay(3, () => ctx.OpponentPokemon());
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Helpful Slash: attach a Basic Grass Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class ScytherMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Helpful Slash") return false;
                ctx.DamageDefending(atk.Damage);
                var grass = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
                var bench = ctx.Me.Bench.ToList();
                if (grass.Count == 0 || bench.Count == 0) return true;
                var energy = grass[0];
                var target = ctx.ChoosePokemon(bench, "Helpful Slash: attach a Basic Grass Energy to which of your Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(energy)) ctx.Engine.AttachEnergyCard(energy, target);
                return true;
            }
        }

        // ================================================================== Water (Jynx ex)

        /// <summary>Heart-Stopping Kiss: if the opponent's Active Pokémon is Asleep, it's Knocked Out.</summary>
        private sealed class HeartStoppingKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heart-Stopping Kiss") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Asleep))
                {
                    ctx.Log("Heart-Stopping Kiss: " + (defending == null ? "there's no Defending Pokémon" : defending.Name + " isn't Asleep") + ".");
                    return true;
                }
                ctx.Engine.KnockOut(defending, "Heart-Stopping Kiss");
                return true;
            }
        }

        // ================================================================== Colorless (Eevee, Porygon, Aerodactyl)

        /// <summary>Colorful Friends: search your deck for up to 3 Pokémon of different types, reveal them and put them into your hand.</summary>
        private sealed class EeveeMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Friends") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var taken = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var types = new HashSet<EnergyType>(taken.Select(x => x.Def.PokemonType));
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && !types.Contains(d.PokemonType), 1,
                        "Colorful Friends: choose a Pokémon of a type you haven't taken (" + (i + 1) + " of up to 3)");
                    if (found.Count == 0) break;
                    taken.AddRange(found);
                }
                ctx.Me.Hand.AddRange(taken);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>"Until the Defending Pokémon leaves the Active Spot, its Weakness is now that type." It also ends if that Pokémon evolves.</summary>
        public sealed class WeaknessChangeEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public WeaknessChangeEffect(string name, int owner, int targetUid, EnergyType type)
                : base(name, owner, targetUid, int.MaxValue / 2, true)
            {
                this.type = type;
            }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon.Uid == TargetUid ? type : weakness;
            }
        }

        /// <summary>
        /// Conversion: choose a type; until the Defending Pokémon leaves the Active Spot, its Weakness is that type (the amount
        /// doesn't change). A Pokémon with no Weakness gets ×2 Weakness to that type (your ruling, v38). Protective Goggles still
        /// win (your ruling).
        /// </summary>
        private sealed class PorygonMEW : Mon
        {
            private static readonly EnergyType[] Choices =
            {
                EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic,
                EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Dragon
            };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conversion") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Conversion on " + defending.Name + " was prevented.");
                    return true;
                }
                var labels = Choices.Select(t => t.ToString()).ToArray();
                int pick = ctx.ChooseOption("Conversion: choose " + defending.Name + "'s new Weakness", labels);
                var chosen = Choices[pick];
                ctx.AddEffect(new WeaknessChangeEffect(defending.Name + "'s Weakness is " + chosen + " until it leaves the Active Spot (Conversion)",
                    ctx.Player, defending.Uid, chosen));
                return true;
            }
        }

        /// <summary>Devolution Ray: if the opponent's Active Pokémon is evolved, the highest Stage Evolution card on it goes into their hand.</summary>
        private sealed class AerodactylMEW : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devolution Ray") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Stack.Count > 1) ctx.Engine.Devolve(defending, 1);
                return true;
            }
        }

        // ================================================================== Fighting (Kabutops)

        /// <summary>
        /// Ancient Way: Weakness of the opponent's Active Pokémon is ×4 against attacks from any of your Pokémon, wherever
        /// Kabutops is (your ruling, v38). Draining Blade: heal 30 damage from this Pokémon.
        /// </summary>
        private sealed class KabutopsMEW : Mon
        {
            private static readonly Mon Blade = new CardsTWM1.HealSelfMon("Draining Blade", 30);

            public override int WeaknessMultiplier(EffectContext src, DamageInfo info)
            {
                if (src.Pokemon == null || info.Attacker == null || info.Attacker.Owner != src.Player) return 2;
                return info.Target.Owner != src.Player && info.TargetIsActive ? 4 : 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blade.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire (Moltres)

        /// <summary>Blazing Flight: discard 2 Fire Energy from this Pokémon (even with no Benched target: your ruling, v38), then 120 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class BlazingFlightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blazing Flight") return false;
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 2, "Blazing Flight: discard 2 Fire Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageOneBenched(120);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Jet Cruise: your Pokémon in play have no Retreat Cost. Dragon Pulse: discard the top 2 cards of your deck.</summary>
        private sealed class DragoniteMEW : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Antique Dome Fossil, Antique Helix Fossil, Antique Old Amber: no Ability; the Antique Fossil rules are in the engine.</summary>
        private sealed class PlainFossil : CardScript { }

        /// <summary>Big Air Balloon: the Stage 2 Pokémon this card is attached to has no Retreat Cost.</summary>
        private sealed class BigAirBalloon : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.Stage == Stage.Stage2 ? 0 : cost;
            }
        }

        /// <summary>Bill's Transfer: look at the top 8 cards of your deck, put any number of Pokémon there into your hand, shuffle the rest back.</summary>
        private sealed class BillsTransfer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(8);
                var mons = top.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count > 0)
                {
                    var picks = ctx.ChooseCards(mons, 0, mons.Count, "Bill's Transfer: put any number of these Pokémon into your hand");
                    if (picks.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Player, picks, "put these Pokémon into " + ctx.Engine.Their(ctx.Player) + " hand");
                        ctx.DeckToHand(picks);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Cycling Road: once during each player's turn, they may discard a Basic Energy card from their hand to draw a card (only with a card left in the deck: your ruling, v38).</summary>
        private sealed class CyclingRoad : StadiumAction
        {
            protected override bool Ready(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy);
            }

            protected override bool Use(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0) return false;
                var pick = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Cycling Road: discard which Basic Energy card to draw a card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(1);
                return false;
            }
        }

        /// <summary>Daisy's Help: draw 2 cards, then look at your face-down Prize cards (they stay in the same order).</summary>
        private sealed class DaisysHelp : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                var me = ctx.Me;
                var faceDown = me.Prizes.Where(c => !me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count == 0) return;
                ctx.Engine.NoteLookedAt(faceDown, "Your face-down Prize cards (Daisy's Help)", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.Their(ctx.Player) + " face-down Prize cards.");
            }
        }

        /// <summary>Energy Sticker: flip a coin; on heads, attach a Basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EnergySticker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Bench.Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                var bench = ctx.Me.Bench.ToList();
                if (energy.Count == 0 || bench.Count == 0) return;
                var pick = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Energy Sticker: choose a Basic Energy card from your discard pile")[0];
                var target = ctx.ChoosePokemon(bench, "Energy Sticker: attach " + pick.Def.Name + " to which of your Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, target);
            }
        }

        /// <summary>
        /// Erika's Invitation: the opponent reveals their hand; you must put a Basic Pokémon from it onto their Bench, then switch
        /// it into the Active Spot. It can't be played into a full Bench (Compendium, 2023).
        /// </summary>
        private sealed class ErikasInvitation : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Engine.BenchHasRoom(ctx.Opponent); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon).ToList();
                if (basics.Count == 0 || !ctx.Engine.BenchHasRoom(ctx.Opponent)) return;
                var pick = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Erika's Invitation: put which Basic Pokémon onto your opponent's Bench?")[0];
                ctx.Opp.Hand.Remove(pick);
                // From their hand: Gapejaw Bog hits it (Compendium, 2023-09).
                var mon = ctx.Engine.PutOntoBench(ctx.Opponent, pick, false, true);
                if (mon != null && ctx.Opp.Bench.Contains(mon)) ctx.Engine.SwitchActive(ctx.Opponent, mon);
            }
        }

        /// <summary>
        /// Giovanni's Charisma: put an Energy attached to the opponent's Active Pokémon into their hand; if you do, attach an Energy
        /// card from your hand to your Active Pokémon. It can only be played while their Active Pokémon has Energy, and the
        /// attachment is optional (your rulings, v38).
        /// </summary>
        private sealed class GiovannisCharisma : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending == null || defending.Energy.Count == 0) return;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Giovanni's Charisma on " + defending.Name + " was prevented.");
                    return;
                }
                var pick = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Giovanni's Charisma: put which Energy into your opponent's hand?")[0];
                if (!ctx.Engine.ReturnEnergyToHand(defending, pick)) return;
                var active = ctx.Me.Active;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (active == null || energy.Count == 0) return;
                var picks = ctx.ChooseCards(energy, 0, 1, "Giovanni's Charisma: you may attach an Energy card from your hand to " + active.Name);
                if (picks.Count == 0) return;
                ctx.Me.Hand.Remove(picks[0]);
                ctx.Engine.AttachEnergyCard(picks[0], active, true);
            }
        }

        /// <summary>Grabber: the opponent reveals their hand; you must put a Pokémon from it on the bottom of their deck (Compendium, 2023-09-28).</summary>
        private sealed class Grabber : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                var mons = ctx.Opp.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count == 0) return;
                var pick = mons.Count == 1 ? mons[0] : ctx.ChooseCards(mons, 1, 1, "Grabber: put which Pokémon on the bottom of your opponent's deck?")[0];
                ctx.Opp.Hand.Remove(pick);
                ctx.Opp.Deck.Add(pick);
                ctx.Log(pick.Def.Name + " was put on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Grabber).");
            }
        }

        /// <summary>Leftovers: at the end of your turn, if the holder is in the Active Spot, heal 20 damage from it.</summary>
        private sealed class Leftovers : CardScript
        {
            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || !src.PokemonIsActive || holder.DamageCounters == 0) return;
                src.Heal(holder, 20);
            }
        }

        /// <summary>Protective Goggles: the Basic Pokémon this card is attached to has no Weakness, whatever gives it one (your ruling, v38).</summary>
        private sealed class ProtectiveGoggles : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.IsBasic;
            }
        }

        /// <summary>Rigid Band: the Stage 1 Pokémon this card is attached to takes 30 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class RigidBand : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || damage <= 0) return damage;
                return holder.Def.Stage == Stage.Stage1 ? damage - 30 : damage;
            }
        }
    }
}
