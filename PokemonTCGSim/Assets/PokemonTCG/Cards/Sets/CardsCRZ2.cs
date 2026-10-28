using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crown Zenith (CRZ): the Galarian Gallery Pokémon (GG1-GG56, GG67-GG70) that weren't already in the pool, and the
    /// Trainers. Gallery cards that are second prints of CRZ 1-160 (GG11, GG15, GG22, GG26, GG29, GG35, GG37, GG42, GG43,
    /// GG47, GG54, GG55, GG65) use the main card's script. Pokémon whose attacks only do their printed damage need no
    /// script: Turtwig, Mareep.
    /// </summary>
    public static class CardsCRZ2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("CRZ-GG1", new HisuianVoltorbCRZ());
            r.RegisterByKey("CRZ-GG2", new KricketuneCRZ());
            r.RegisterByKey("CRZ-GG3", new BoltsplosionMon());
            r.RegisterByKey("CRZ-GG4", new OricorioCRZ());
            r.RegisterByKey("CRZ-GG5", new RagingFreezeMon());
            r.RegisterByKey("CRZ-GG6", new ManaphyCRZ());
            r.RegisterByKey("CRZ-GG7", new CardsFCO3.DoTheWaveMon("Line Force", 20));
            r.RegisterByKey("CRZ-GG8", new ComboMon(new ExplosiveBoltMon(), new CardsCRZ1.SpreadAllMon("High-Voltage Current")));
            r.RegisterByKey("CRZ-GG9", new ToxtricityCRZ());
            r.RegisterByKey("CRZ-GG10", new MewCRZ());
            r.RegisterByKey("CRZ-GG12", new PhotonBoostMon());
            r.RegisterByKey("CRZ-GG13", new DiancieCRZ());
            r.RegisterByKey("CRZ-GG14", new ComfeyCRZ());
            r.RegisterByKey("CRZ-GG16", new ComboMon(new CardsCRZ1.SpreadAllMon("Swirling Disaster"), new CardsPFL.DamagedBonusMon("Claw Rend", 70)));
            r.RegisterByKey("CRZ-GG17", new ThievulCRZ());
            r.RegisterByKey("CRZ-GG18", new MagnezoneCRZ());
            r.RegisterByKey("CRZ-GG19", new AltariaCRZ());
            r.RegisterByKey("CRZ-GG20", new LatiasCRZ());
            r.RegisterByKey("CRZ-GG21", new HisuianGoodraCRZ());
            r.RegisterByKey("CRZ-GG23", new DunsparceCRZ());
            r.RegisterByKey("CRZ-GG24", new MiltankCRZ());
            r.RegisterByKey("CRZ-GG25", new BibarelCRZ());
            r.RegisterByKey("CRZ-GG27", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("CRZ-GG28", new CardsPAR2.FlipConditionMon("Perplex", SpecialCondition.Confused));
            r.RegisterByKey("CRZ-GG30", new PikachuCRZ());
            r.RegisterByKey("CRZ-GG32", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("CRZ-GG33", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("CRZ-GG36", new RondoV("Burning Rondo"));
            r.RegisterByKey("CRZ-GG38", new RondoV("Blizzard Rondo"));
            r.RegisterByKey("CRZ-GG39", new LumineonV());
            r.RegisterByKey("CRZ-GG40", new ComboMon(new NoRetreatMon("Icicle Shot"), new CrystalStarMon("Crystal Star")));
            r.RegisterByKey("CRZ-GG41", new RondoV("Lightning Rondo"));
            r.RegisterByKey("CRZ-GG44", new MewtwoVstar());
            r.RegisterByKey("CRZ-GG45", new DeoxysVmax());
            r.RegisterByKey("CRZ-GG46", new ComboMon(new CardsASR1.AxeBreakMon("Psychic Javelin"), new CardsCRZ1.BothActiveEnergyMon("Star Force", 60)));
            r.RegisterByKey("CRZ-GG48", new ZacianVCel());
            r.RegisterByKey("CRZ-GG49", new DrapionV());
            r.RegisterByKey("CRZ-GG50", new DarkraiVstar());
            r.RegisterByKey("CRZ-GG51", new ComboMon(new BasketCrashMon(), new CardsTWM1.SelfDiscardMon("Shadow Slash", 1)));
            r.RegisterByKey("CRZ-GG52", new HisuianSamurottVstar());
            r.RegisterByKey("CRZ-GG53", new HoopaV());
            r.RegisterByKey("CRZ-GG56", new HisuianZoroarkVstar());
            r.RegisterByKey("CRZ-GG67", new OriginPalkiaVstar());
            r.RegisterByKey("CRZ-GG68", new ComboMon(new CardsMEG.EnergyBonusMon("Metal Blast", EnergyType.Metal, 40), new StarChronosMon()));
            r.RegisterByKey("CRZ-GG69", new GiratinaVstar());
            r.RegisterByKey("CRZ-GG70", new ArceusVstar());

            // Trainers (by name: every printing plays the same).
            r.RegisterByName("Bea", new Bea());
            r.RegisterByName("Bede", new Bede());
            r.RegisterByName("Digging Duo", new DiggingDuo());
            r.RegisterByName("Friends in Hisui", new DrawThree());
            r.RegisterByName("Friends in Sinnoh", new DrawThree());
            r.RegisterByName("Hop", new DrawThree());
            r.RegisterByName("Leon", new Leon());
            r.RegisterByName("Lost Vacuum", new LostVacuum());
            r.RegisterByName("Nessa", new Nessa());
            r.RegisterByName("Raihan", new Raihan());
            r.RegisterByName("Rescue Carrier", new RescueCarrier());
            r.RegisterByName("Sky Seal Stone", new SkySealStone());
            r.RegisterByName("Trekking Shoes", new TrekkingShoes());
            r.RegisterByName("Elesa's Sparkle", new ElesasSparkle());
            r.RegisterByName("Volo", new Volo());
            r.RegisterByName("Adaman", new Adaman());
            r.RegisterByName("Cheren's Care", new CherensCare());
            r.RegisterByName("Colress's Experiment", new ColresssExperiment());
            r.RegisterByName("Cynthia's Ambition", new CynthiasAmbition());
            r.RegisterByName("Gardenia's Vigor", new GardeniasVigor());
            r.RegisterByName("Grant", new Grant());
            r.RegisterByName("Irida", new Irida());
            r.RegisterByName("Melony", new Melony());
            r.RegisterByName("Roxanne", new Roxanne());
        }

        // ================================================================== Grass

        /// <summary>Cheerful Charge: only if you went second, on your first turn: a basic Energy from your deck onto each of up to 2 Benched Pokémon.</summary>
        private sealed class HisuianVoltorbCRZ : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Cheerful Charge") return true;
                return ctx.Player != ctx.State.FirstPlayer && ctx.IsMyFirstTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cheerful Charge") return false;
                var bench = ctx.MyBench();
                if (bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var chosen = ctx.ChoosePokemonMany(bench, 0, Math.Min(2, bench.Count), "Cheerful Charge: choose up to 2 of your Benched Pokémon");
                foreach (var mon in chosen)
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Cheerful Charge: search your deck for a basic Energy card for " + mon.Name);
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, mon);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Swelling Tune: your Grass Pokémon except Kricketune get +40 HP (only 1 Swelling Tune at a time).</summary>
        private sealed class KricketuneCRZ : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon.Owner != src.Player || mon.Name == "Kricketune") return hp;
                if (!src.Engine.IsType(mon, EnergyType.Grass) || !CardsCRZ1.IsLeadSource(src, "Swelling Tune")) return hp;
                return hp + 40;
            }
        }

        // ================================================================== Fire

        /// <summary>Boltsplosion: 120 more damage if Electivire is on your Bench.</summary>
        private sealed class BoltsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Boltsplosion") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Any(m => m.Name == "Electivire") ? 120 : 0));
                return true;
            }
        }

        /// <summary>Lesson in Zeal: your Fusion Strike Pokémon take 20 less damage from the opponent's attacks (only 1 at a time). Glistening Droplets.</summary>
        private sealed class OricorioCRZ : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target.Owner != src.Player || !info.Target.Def.IsFusionStrike || a == null || a.Owner == src.Player) return damage;
                return CardsCRZ1.IsLeadSource(src, "Lesson in Zeal") ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glistening Droplets") return false;
                ctx.PlaceCountersAnyWay(5, () => ctx.OpponentPokemon());
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Raging Freeze: Paralyzed if any of your Pokémon were Knocked Out by damage from an opponent's attack during their last turn.</summary>
        private sealed class RagingFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Freeze") return false;
                var defending = ctx.Defending;
                bool angry = ctx.WasKnockedOutByAttackLastOpponentTurn();
                ctx.DamageDefending(atk.Damage);
                if (angry && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Wave Veil: prevent all damage done to your Benched Pokémon by attacks from your opponent's Pokémon.</summary>
        private sealed class ManaphyCRZ : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target.Owner != src.Player || src.IsActive(info.Target) || a == null || a.Owner == src.Player) return damage;
                return 0;
            }
        }

        // ================================================================== Lightning

        /// <summary>Explosive Bolt: 90 more damage if any of your Benched Magmortar have damage counters on them.</summary>
        private sealed class ExplosiveBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Bolt") return false;
                bool hurt = ctx.Me.Bench.Any(m => m.Name == "Magmortar" && m.DamageCounters > 0);
                ctx.DamageDefending(atk.Damage + (hurt ? 90 : 0));
                return true;
            }
        }

        /// <summary>Maximum Downer: if all of your Pokémon in play are Fusion Strike Pokémon, the opponent's Pokémon VMAX get -30 HP.</summary>
        private sealed class ToxtricityCRZ : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon.Owner == src.Player || !mon.Def.IsVmax) return hp;
                return src.Me.InPlay().All(m => m.Def.IsFusionStrike) ? hp - 30 : hp;
            }
        }

        /// <summary>Pika Dash: with any Energy attached it has no Retreat Cost. Whimsy Tackle: tails, nothing.</summary>
        private sealed class PikachuCRZ : Mon
        {
            private static readonly Mon whimsy = new TailsNothingMon("Whimsy Tackle");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && mon.Energy.Count > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return whimsy.HandleAttack(ctx, atk); }
        }

        // ================================================================== Psychic

        /// <summary>Mysterious Tail: once during your turn, if Active, look at the top 6 cards and put an Item there into your hand.</summary>
        private sealed class MewCRZ : Mon
        {
            protected override string Activated { get { return "Mysterious Tail"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var items = top.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0)
                {
                    var pick = ctx.ChooseCards(items, 0, 1, "Mysterious Tail: put an Item card from among these into your hand");
                    if (pick.Count > 0)
                    {
                        ctx.DeckToHand(pick);
                        ctx.Engine.Reveal(ctx.Player, pick, "put this Item card into their hand");
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Photon Boost: 80 more damage if this Pokémon has any Fusion Strike Energy attached.</summary>
        private sealed class PhotonBoostMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Photon Boost") return false;
                bool boost = ctx.Pokemon.Energy.Any(e => e.Def.Name == "Fusion Strike Energy");
                ctx.DamageDefending(atk.Damage + (boost ? 80 : 0));
                return true;
            }
        }

        /// <summary>Princess's Curtain: while Active, Supporters the opponent plays from their hand don't affect your Benched Basic Pokémon.</summary>
        private sealed class DiancieCRZ : Mon
        {
            private static readonly Mon spikeDraw = new DrawAttackMon("Spike Draw", 2);

            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.IsActive(src.Pokemon) && trainer != null && trainer.IsSupporter && target.Owner == src.Player && !src.IsActive(target) && target.IsBasic;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return spikeDraw.HandleAttack(ctx, atk); }
        }

        /// <summary>Flower Selecting: once during your turn, if Active, look at the top 2 cards: 1 into your hand, the other to the Lost Zone.</summary>
        private sealed class ComfeyCRZ : Mon
        {
            protected override string Activated { get { return "Flower Selecting"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                var keep = ctx.ChooseCards(top, 1, 1, "Flower Selecting: choose the card to put into your hand (the other goes to the Lost Zone)");
                ctx.DeckToHand(keep);
                foreach (var c in top)
                {
                    if (keep.Contains(c) || !ctx.Me.Deck.Remove(c)) continue;
                    CardsCRZ1.PutInLostZone(ctx, c);
                }
            }
        }

        /// <summary>Baffling: if your opponent has 2 or fewer Prize cards left, their Supporters don't affect your Benched Pokémon V.</summary>
        private sealed class ThievulCRZ : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Opp.Prizes.Count <= 2 && trainer != null && trainer.IsSupporter && target.Owner == src.Player && !src.IsActive(target) && target.Def.IsPokemonV;
            }
        }

        // ================================================================== Metal and Dragon

        /// <summary>Giga Magnet: once during your turn, look at the top 6 cards and attach any Metal Energy cards there to your Pokémon.</summary>
        private sealed class MagnezoneCRZ : Mon
        {
            protected override string Activated { get { return "Giga Magnet"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var metal = top.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Metal)).ToList();
                if (metal.Count > 0) ctx.AttachCardsAnyWay(metal, metal.Count, (c, m) => true, "Giga Magnet: attach a Metal Energy card to 1 of your Pokémon");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Tempting Tune: once during your turn, search your deck for a Supporter, reveal it, shuffle, then put it on top.</summary>
        private sealed class AltariaCRZ : Mon
        {
            protected override string Activated { get { return "Tempting Tune"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsSupporter, 1, "Tempting Tune: search your deck for a Supporter card to put on top of your deck");
                ctx.ShuffleMyDeck();
                if (found.Count > 0) ctx.Me.Deck.Insert(0, found[0]);
            }
        }

        /// <summary>Red Assist: once during your turn, attach a Psychic Energy card from your hand to 1 of your Latios. Dyna Barrier.</summary>
        private sealed class LatiasCRZ : Mon
        {
            protected override string Activated { get { return "Red Assist"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)) && ctx.MyPokemon(m => m.Name == "Latios").Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                CardsCRZ1.AttachEnergyFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 1, m => m.Name == "Latios", "Red Assist: attach a Psychic Energy card to 1 of your Latios");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dyna Barrier") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new CardsPBL.AttackerShieldEffect(ctx.Pokemon.Name + " takes no damage from Pokémon VMAX during the opponent's next turn (Dyna Barrier)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 9999, m => m.Def.IsVmax));
                return true;
            }
        }

        /// <summary>Metal Lodging: prevent all damage done to your Basic Pokémon with Metal Energy attached by attacks from the opponent's Pokémon V.</summary>
        private sealed class HisuianGoodraCRZ : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                var t = info.Target;
                if (t.Owner != src.Player || !t.IsBasic || a == null || a.Owner == src.Player || !a.Def.IsPokemonV) return damage;
                return src.Engine.HasEnergyOfType(t, EnergyType.Metal) ? 0 : damage;
            }
        }

        // ================================================================== Colorless

        /// <summary>Mysterious Nest: Colorless Pokémon in play (both players') have no Weakness.</summary>
        private sealed class DunsparceCRZ : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && src.Engine.IsType(mon, EnergyType.Colorless);
            }
        }

        /// <summary>Miracle Body: prevent all damage done to it by attacks from the opponent's Pokémon V. Rout: 20 more for each of their Benched Pokémon.</summary>
        private sealed class MiltankCRZ : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target != src.Pokemon || a == null || a.Owner == src.Player || !a.Def.IsPokemonV) return damage;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rout") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.Opp.Bench.Count);
                return true;
            }
        }

        /// <summary>Industrious Incisors: once during your turn, draw until you have 5 cards. Tail Smash: tails, nothing.</summary>
        private sealed class BibarelCRZ : Mon
        {
            private static readonly Mon tailSmash = new TailsNothingMon("Tail Smash");

            protected override string Activated { get { return "Industrious Incisors"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 5 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(5); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return tailSmash.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon V, VMAX and VSTAR

        /// <summary>Entei V, Suicune V, Raikou V. Fleet-Footed: once during your turn, if Active, draw a card. Rondo: 20 more for each Benched Pokémon (both sides).</summary>
        private sealed class RondoV : Mon
        {
            private readonly string attack;

            public RondoV(string attack) { this.attack = attack; }

            protected override string Activated { get { return "Fleet-Footed"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + 20 * CardsCRZ1.AllBenched(ctx));
                return true;
            }
        }

        /// <summary>Luminous Sign: when played from your hand to your Bench, search for a Supporter. Aqua Return: shuffle this Pokémon into your deck.</summary>
        private sealed class LumineonV : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0) return;
                if (!TriggerOnce(ctx, "Luminous Sign", true, "Use Luminous Sign? Search your deck for a Supporter card and put it into your hand.")) return;
                ctx.SearchToHand(d => d.IsSupporter, 1, "Luminous Sign: search your deck for a Supporter card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Return") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Crystal Star (VSTAR Power): during the opponent's next turn, prevent all damage from and effects of attacks done to this Pokémon. Used for Crystal Star, Total Shelter-GX (the attack name is passed in).</summary>
        internal sealed class CrystalStarMon : Mon
        {
            private readonly string attack;

            public CrystalStarMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Psy Purge: discard up to 3 Psychic Energy from your Pokémon, 90 for each. Star Raid (VSTAR Power): 120 to each of the opponent's Pokémon V.</summary>
        private sealed class MewtwoVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Psy Purge")
                {
                    var holderOf = new Dictionary<int, PokemonInPlay>();
                    var cards = new List<CardInstance>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var e in CardsCRZ1.EnergyOfType(ctx, mon, EnergyType.Psychic))
                        {
                            cards.Add(e);
                            holderOf[e.Id] = mon;
                        }
                    int n = 0;
                    if (cards.Count > 0)
                    {
                        var picks = ctx.ChooseCards(cards, 0, Math.Min(3, cards.Count), "Psy Purge: discard up to 3 Psychic Energy from your Pokémon (90 damage for each)");
                        foreach (var e in picks) if (ctx.Engine.DiscardEnergy(holderOf[e.Id], e)) n++;
                    }
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                if (atk.Name == "Star Raid")
                {
                    foreach (var mon in ctx.OpponentPokemon(m => m.Def.IsPokemonV)) ctx.DamageWith(mon, 120, false, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Protective DNA: all your Pokémon take 30 less damage from attacks from the opponent's Pokémon VSTAR. Max Drain: heal 30.</summary>
        private sealed class DeoxysVmax : Mon
        {
            private static readonly Mon maxDrain = new CardsTWM1.HealSelfMon("Max Drain", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target.Owner != src.Player || a == null || a.Owner == src.Player || !a.Def.IsVstar) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return maxDrain.HandleAttack(ctx, atk); }
        }

        /// <summary>Roar of the Sword: search for a Psychic Energy card and attach it to 1 of your Pokémon; your turn ends. Storm Slash.</summary>
        private sealed class ZacianVCel : Mon
        {
            private static readonly Mon stormSlash = new CardsMEG.EnergyBonusMon("Storm Slash", EnergyType.Psychic, 30);

            protected override string Activated { get { return "Roar of the Sword"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 1, "Roar of the Sword: search your deck for a Psychic Energy card");
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Roar of the Sword: attach " + card.Def.Name + " to which of your Pokémon?");
                    ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return stormSlash.HandleAttack(ctx, atk); }
        }

        /// <summary>Wild Style: its attacks cost [C] less for each of the opponent's Single, Rapid and Fusion Strike Pokémon. Dynamic Tail: 60 to 1 of your Pokémon.</summary>
        private sealed class DrapionV : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon) return;
                int n = src.Opp.InPlay().Count(m => !string.IsNullOrEmpty(m.Def.Strike));
                for (int i = 0; i < n; i++) if (!cost.Remove(EnergyType.Colorless)) break;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Tail") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Dynamic Tail: do 60 damage to which of your Pokémon?");
                ctx.DamageTarget(target, 60);
                return true;
            }
        }

        /// <summary>Star Abyss (VSTAR Power): put up to 2 Item cards from your discard pile into your hand. Dark Pulse: 30 more per Darkness Energy on your Pokémon.</summary>
        private sealed class DarkraiVstar : Mon
        {
            protected override string Activated { get { return "Star Abyss"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsItem); }

            protected override void Ability(EffectContext ctx)
            {
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                var picks = ctx.ChooseCards(items, 1, Math.Min(2, items.Count), "Star Abyss: put up to 2 Item cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Pulse") return false;
                int dark = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Darkness));
                ctx.DamageDefending(atk.Damage + 30 * dark);
                return true;
            }
        }

        /// <summary>Basket Crash: discard up to 2 Pokémon Tools from your opponent's Pokémon.</summary>
        private sealed class BasketCrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Basket Crash") return false;
                var holderOf = new Dictionary<int, PokemonInPlay>();
                var tools = new List<CardInstance>();
                foreach (var mon in ctx.OpponentPokemon(m => ctx.CanAffect(m)))
                    foreach (var t in mon.Tools)
                    {
                        tools.Add(t);
                        holderOf[t.Id] = mon;
                    }
                if (tools.Count == 0) return true;
                var picks = ctx.ChooseCards(tools, 0, Math.Min(2, tools.Count), "Basket Crash: discard up to 2 Pokémon Tools from your opponent's Pokémon");
                foreach (var t in picks) ctx.Engine.DiscardTool(holderOf[t.Id], t);
                return true;
            }
        }

        /// <summary>Moon Cleave Star (VSTAR Power): 4 damage counters on 1 of your opponent's Pokémon. Merciless Blade.</summary>
        private sealed class HisuianSamurottVstar : Mon
        {
            private static readonly Mon merciless = new CardsPFL.DamagedBonusMon("Merciless Blade", 110);

            protected override string Activated { get { return "Moon Cleave Star"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Moon Cleave Star: put 4 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return merciless.HandleAttack(ctx, atk); }
        }

        /// <summary>Two-Faced: while in play it's Psychic and Darkness type. Shadow Impact: 3 damage counters on 1 of your Pokémon.</summary>
        private sealed class HoopaV : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Psychic)) types.Add(EnergyType.Psychic);
                if (!types.Contains(EnergyType.Darkness)) types.Add(EnergyType.Darkness);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Shadow Impact: put 3 damage counters on which of your Pokémon?");
                ctx.PlaceCounters(target, 3);
                return true;
            }
        }

        /// <summary>Phantom Star (VSTAR Power): discard your hand and draw 7 cards. Ticking Curse: 50 for each of your Pokémon with damage counters.</summary>
        private sealed class HisuianZoroarkVstar : Mon
        {
            protected override string Activated { get { return "Phantom Star"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(7);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ticking Curse") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.DamageCounters > 0).Count);
                return true;
            }
        }

        /// <summary>Star Portal (VSTAR Power): attach up to 3 Water Energy cards from your discard pile to your Water Pokémon. Subspace Swell.</summary>
        private sealed class OriginPalkiaVstar : Mon
        {
            protected override string Activated { get { return "Star Portal"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water).Count > 0 && ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Water)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                ctx.AttachCardsAnyWay(pile, 3, (c, m) => ctx.Engine.IsType(m, EnergyType.Water), "Star Portal: attach a Water Energy card to 1 of your Water Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Subspace Swell") return false;
                ctx.DamageDefending(atk.Damage + 20 * CardsCRZ1.AllBenched(ctx));
                return true;
            }
        }

        /// <summary>Star Chronos (VSTAR Power): take another turn after this one, skipping Pokémon Checkup (engine: GameEngine.StartTurn).</summary>
        private sealed class StarChronosMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Chronos") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.State.ExtraTurnFor = ctx.Player;
                ctx.State.SkipCheckup = true;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " will take another turn after this one (Pokémon Checkup is skipped).");
                return true;
            }
        }

        /// <summary>Lost Impact: put 2 Energy attached to your Pokémon in the Lost Zone. Star Requiem (VSTAR Power): with 10+ cards in your Lost Zone, Knock Out the Defending Pokémon.</summary>
        private sealed class GiratinaVstar : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Star Requiem" || ctx.Me.LostZone.Count >= 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lost Impact")
                {
                    ctx.DamageDefending(atk.Damage);
                    var holderOf = new Dictionary<int, PokemonInPlay>();
                    var energy = new List<CardInstance>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var e in mon.Energy)
                        {
                            energy.Add(e);
                            holderOf[e.Id] = mon;
                        }
                    var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Lost Impact: put 2 Energy attached to your Pokémon in the Lost Zone");
                    foreach (var e in picks)
                    {
                        if (!holderOf[e.Id].Energy.Remove(e)) continue;
                        CardsCRZ1.PutInLostZone(ctx, e);
                    }
                    return true;
                }
                if (atk.Name == "Star Requiem")
                {
                    var defending = ctx.Defending;
                    if (defending != null && ctx.CanAffect(defending)) ctx.Engine.KnockOut(defending, "Star Requiem");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Starbirth (VSTAR Power): search for up to 2 cards. Trinity Nova: up to 3 basic Energy from your deck onto your Pokémon V.</summary>
        private sealed class ArceusVstar : Mon
        {
            protected override string Activated { get { return "Starbirth"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 2, "Starbirth: choose up to 2 cards to put into your hand", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trinity Nova") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                if (!ctx.MyPokemon().Any(m => m.Def.IsPokemonV))
                {
                    ctx.ShuffleMyDeck();
                    return true;
                }
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Trinity Nova: choose up to 3 basic Energy cards for your Pokémon V");
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (c, m) => m.Def.IsPokemonV, "Trinity Nova", false);
                foreach (var card in found)
                    if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card)) && !ctx.Me.Deck.Contains(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Draw 3 cards (Friends in Hisui, Friends in Sinnoh, Hop).</summary>
        private sealed class DrawThree : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Bea: discard the top 5 cards of your deck; attach the Energy cards discarded to your Benched Fighting Pokémon in any way.</summary>
        private sealed class Bea : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                var energy = discarded.Where(c => c.Def.IsEnergy && ctx.Me.Discard.Contains(c)).ToList();
                if (energy.Count == 0) return;
                ctx.AttachCardsAnyWay(energy, energy.Count, (c, m) => ctx.Me.Bench.Contains(m) && ctx.Engine.IsType(m, EnergyType.Fighting),
                    "Bea: attach a discarded Energy card to 1 of your Benched Fighting Pokémon");
            }
        }

        /// <summary>Bede: attach a basic Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class Bede : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Bench.Count > 0 && ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsBasicEnergy);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                CardsCRZ1.AttachEnergyFromHand(ctx, d => d.IsBasicEnergy, 1, m => ctx.Me.Bench.Contains(m), "Bede: attach a basic Energy card to 1 of your Benched Pokémon");
            }
        }

        /// <summary>Digging Duo: flip a coin; look at the bottom 8 (heads) or 3 (tails) cards and put 1 into your hand; shuffle the rest back.</summary>
        private sealed class DiggingDuo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(ctx.Flip() ? 8 : 3, ctx.Me.Deck.Count);
                var bottom = ctx.Me.Deck.Skip(ctx.Me.Deck.Count - n).ToList();
                ctx.Engine.NoteLookedAt(bottom, "Bottom " + bottom.Count + (bottom.Count == 1 ? " card" : " cards") + " of your deck", ctx.Player);
                var pick = ctx.ChooseCards(bottom, 1, 1, "Digging Duo: put 1 of these cards into your hand");
                ctx.DeckToHand(pick);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Leon: during this turn, your Pokémon's attacks do 30 more damage to your opponent's Active Pokémon.</summary>
        private sealed class Leon : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new DamageBonusEffect("Leon: your attacks do 30 more damage to the opponent's Active Pokémon this turn", ctx.Player, ctx.Turn, 30, false));
            }
        }

        /// <summary>Lost Vacuum: put another card from your hand in the Lost Zone, then put a Tool attached to any Pokémon, or the Stadium, in the Lost Zone.</summary>
        private sealed class LostVacuum : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                bool cost = ctx.Me.Hand.Any(c => c != ctx.Card);
                bool target = ctx.State.Stadium != null || ctx.State.Players.Any(p => p.InPlay().Any(m => m.Tools.Count > 0));
                return cost && target;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var cost = ctx.ChooseCards(hand, 1, 1, "Lost Vacuum: put a card from your hand in the Lost Zone");
                if (cost.Count == 0 || !ctx.Me.Hand.Remove(cost[0])) return;
                CardsCRZ1.PutInLostZone(ctx, cost[0]);

                var labels = new List<string>();
                var holders = new List<PokemonInPlay>();
                var tools = new List<CardInstance>();
                foreach (var p in ctx.State.Players)
                    foreach (var mon in p.InPlay())
                        foreach (var t in mon.Tools)
                        {
                            labels.Add(t.Def.Name + " (on " + ctx.Engine.OwnedName(mon) + ")");
                            holders.Add(mon);
                            tools.Add(t);
                        }
                var stadium = ctx.State.Stadium;
                if (stadium != null && ctx.Engine.StadiumIgnoresCurrentTrainer()) stadium = null; // a Prism Star Stadium (v80)
                if (stadium != null) labels.Add(stadium.Def.Name + " (Stadium)");
                if (labels.Count == 0) return;
                int pick = ctx.ChooseOption("Lost Vacuum: put which Pokémon Tool or Stadium in the Lost Zone?", labels.ToArray());
                if (pick < 0) return;
                if (pick < tools.Count)
                {
                    var holder = holders[pick];
                    var tool = tools[pick];
                    if (holder.Tools.Remove(tool)) CardsCRZ1.PutInLostZone(ctx, tool);
                    return;
                }
                if (stadium == null || ctx.State.Stadium != stadium) return;
                int owner = ctx.State.StadiumOwner;
                var partner = ctx.Engine.TakeStadiumPartner(); // the other half of a Legendary Stadium goes too (v152)
                ctx.State.Stadium = null;
                ctx.State.StadiumOwner = -1;
                CardsCRZ1.PutInLostZone(ctx, stadium);
                if (partner != null) CardsCRZ1.PutInLostZone(ctx, partner);
                ctx.Engine.Scripts.For(stadium.Def).OnLeavePlay(new EffectContext(ctx.Engine, owner, stadium, null));
            }
        }

        /// <summary>Nessa: put up to 4 in any combination of Water Pokémon and Water Energy cards from your discard pile into your hand.</summary>
        private sealed class Nessa : CardScript
        {
            private static bool Fits(CardDef d)
            {
                return (d.IsPokemon && d.PokemonType == EnergyType.Water) || CardsSSP1.IsBasicOf(d, EnergyType.Water);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => Fits(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => Fits(c.Def)).ToList();
                if (pile.Count == 0) return;
                var picks = ctx.ChooseCards(pile, 1, Math.Min(4, pile.Count), "Nessa: put up to 4 Water Pokémon and Water Energy cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Raihan: only after a Knock Out last turn, and only with a basic Energy card in your discard pile (Compendium, 2021-08-26). Attach a basic Energy from your discard pile to 1 of your Pokémon; if you do, search for a card.</summary>
        private sealed class Raihan : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.WasKnockedOutLastOpponentTurn() && ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0 || !ctx.Me.HasPokemonInPlay) return;
                int attached = ctx.AttachCardsAnyWay(pile, 1, (c, m) => true, "Raihan: attach a basic Energy card from your discard pile", false);
                if (attached > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Raihan: search your deck for a card", false);
            }
        }

        /// <summary>Rescue Carrier: put up to 2 Pokémon with 90 HP or less from your discard pile into your hand.</summary>
        private sealed class RescueCarrier : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsPokemon && d.Hp <= 90; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => Fits(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => Fits(c.Def)).ToList();
                if (pile.Count == 0) return;
                var picks = ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Rescue Carrier: put up to 2 Pokémon with 90 HP or less into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>
        /// Sky Seal Stone (Tool): the Pokémon V holding it can use Star Order (VSTAR Power): this turn, if the opponent's Active
        /// Pokémon VSTAR or VMAX is Knocked Out by damage from an attack of your Basic Pokémon V, take 1 more Prize card.
        /// </summary>
        private sealed class SkySealStone : CardScript
        {
            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return abilityIndex == 0 && ctx.IsMyTurn && ctx.Pokemon != null && ctx.Pokemon.Def.IsPokemonV && !ctx.Me.VstarUsed;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                int owner = ctx.Player;
                ctx.AddEffect(new ExtraPrizeEffect("Star Order: 1 more Prize card if the opponent's Active Pokémon VSTAR or VMAX is Knocked Out by your Basic Pokémon V's attack this turn", owner, ctx.Turn,
                    (e, ko) => ko.HitByAttack && ko.HitByPlayer == owner && !ko.ForcedKnockOut && ko.HitWhileActive && (ko.Def.IsVstar || ko.Def.IsVmax)
                               && ko.HitByAttackerDef != null && ko.HitByAttackerDef.IsBasicPokemon && ko.HitByAttackerDef.IsPokemonV));
            }
        }

        /// <summary>Trekking Shoes: look at the top card; put it into your hand, or discard it and draw a card.</summary>
        private sealed class TrekkingShoes : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return;
                var card = top[0];
                int pick = ctx.ChooseOption("Trekking Shoes: the top card of your deck is " + card.Def.Name + ".", "Put it into your hand", "Discard it and draw a card");
                if (pick == 0)
                {
                    ctx.DeckToHand(top);
                    return;
                }
                ctx.Me.Deck.Remove(card);
                ctx.Me.Discard.Add(card);
                ctx.Engine.Emit(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + card.Def.Name + " from the top of " + ctx.Engine.Their(ctx.Player) + " deck.", card.Id);
                ctx.Draw(1);
            }
        }

        /// <summary>Elesa's Sparkle: for each of up to 2 of your Fusion Strike Pokémon, search for a Fusion Strike Energy and attach it to that Pokémon.</summary>
        private sealed class ElesasSparkle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => m.Def.IsFusionStrike).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.MyPokemon(m => m.Def.IsFusionStrike);
                var chosen = ctx.ChoosePokemonMany(targets, 1, Math.Min(2, targets.Count), "Elesa's Sparkle: choose up to 2 of your Fusion Strike Pokémon");
                foreach (var mon in chosen)
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var found = ctx.SearchMyDeck(d => d.Name == "Fusion Strike Energy", 1, "Elesa's Sparkle: search for a Fusion Strike Energy for " + mon.Name);
                    foreach (var e in found) ctx.Engine.AttachEnergyCard(e, mon);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Volo: discard 1 of your Benched Pokémon V and all attached cards.</summary>
        private sealed class Volo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.Def.IsPokemonV); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.Me.Bench.Where(m => m.Def.IsPokemonV).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Volo: discard which of your Benched Pokémon V?");
                if (target != null) ctx.Engine.DiscardPokemonFromPlay(target);
            }
        }

        /// <summary>Adaman: only if you discard 2 Metal Energy cards from your hand. Search your deck for up to 2 cards.</summary>
        private sealed class Adaman : CardScript
        {
            private static bool IsMetal(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Metal); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card && IsMetal(c.Def)) >= 2 && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var metal = ctx.Me.Hand.Where(c => IsMetal(c.Def)).ToList();
                if (metal.Count < 2) return;
                var cost = metal.Count == 2 ? metal : ctx.ChooseCards(metal, 2, 2, "Adaman: discard 2 Metal Energy cards from your hand");
                foreach (var card in cost) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.SearchToHand(d => true, 2, "Adaman: choose up to 2 cards to put into your hand", false);
            }
        }

        /// <summary>Cheren's Care: put 1 of your Colorless Pokémon that has damage counters and all attached cards into your hand.</summary>
        private sealed class CherensCare : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Colorless)).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Colorless));
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Cheren's Care: put which Colorless Pokémon and all attached cards into your hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
            }
        }

        /// <summary>Colress's Experiment: look at the top 5 cards, put 3 into your hand and the rest in the Lost Zone.</summary>
        private sealed class ColresssExperiment : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(5);
                var keep = top.Count <= 3 ? top : ctx.ChooseCards(top, 3, 3, "Colress's Experiment: choose 3 cards to put into your hand (the rest go to the Lost Zone)");
                ctx.DeckToHand(keep);
                foreach (var c in top)
                {
                    if (keep.Contains(c) || !ctx.Me.Deck.Remove(c)) continue;
                    CardsCRZ1.PutInLostZone(ctx, c);
                }
            }
        }

        /// <summary>Cynthia's Ambition: draw until you have 5 cards, or 8 if any of your Pokémon were Knocked Out during your opponent's last turn.</summary>
        private sealed class CynthiasAmbition : CardScript
        {
            private static int Target(EffectContext ctx) { return ctx.WasKnockedOutLastOpponentTurn() ? 8 : 5; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < Target(ctx);
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(Target(ctx)); }
        }

        /// <summary>Gardenia's Vigor: draw 2 cards; if you drew any, attach up to 2 Grass Energy cards from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class GardeniasVigor : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int drawn = ctx.Draw(2);
                if (drawn == 0 || ctx.Me.Bench.Count == 0) return;
                var grass = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
                if (grass.Count == 0) return;
                var picks = ctx.ChooseCards(grass, 0, Math.Min(2, grass.Count), "Gardenia's Vigor: attach up to 2 Grass Energy cards from your hand to 1 of your Benched Pokémon");
                if (picks.Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Gardenia's Vigor: attach them to which Benched Pokémon?");
                foreach (var card in picks) CardsCRZ1.AttachFromHand(ctx, card, target);
            }
        }

        /// <summary>
        /// Grant: this turn your Fighting Pokémon's attacks do 30 more damage to the opponent's Active Pokémon. From the discard
        /// pile, during your turn: discard 2 other cards (not Grant) from your hand to put this Grant into your hand (that
        /// doesn't use up your Supporter for the turn).
        /// </summary>
        private sealed class Grant : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new ConditionalDamageBonusEffect("Grant: your Fighting Pokémon's attacks do 30 more damage to the opponent's Active Pokémon this turn",
                    ctx.Player, ctx.Turn, 30, (engine, attacker, target) => engine.IsType(attacker, EnergyType.Fighting)));
            }

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.Me.Hand.Count(c => c.Def.Name != "Grant") >= 2 && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c.Def.Name != "Grant").ToList();
                if (others.Count < 2) return;
                var cost = ctx.ChooseCards(others, 2, 2, "Grant: discard 2 cards (not Grant) from your hand to put this Grant into your hand");
                if (cost.Count < 2) return;
                foreach (var card in cost) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DiscardToHand(new List<CardInstance> { ctx.Card });
            }
        }

        /// <summary>Irida: search your deck for a Water Pokémon and an Item card.</summary>
        private sealed class Irida : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.PokemonType == EnergyType.Water, 1, "Irida: search your deck for a Water Pokémon");
                found.AddRange(ctx.SearchMyDeck(d => d.IsItem, 1, "Irida: search your deck for an Item card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Melony: attach a Water Energy card from your discard pile to 1 of your Pokémon V; if you do, draw 3 cards.</summary>
        private sealed class Melony : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water).Count > 0 && ctx.MyPokemon(m => m.Def.IsPokemonV).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                int attached = ctx.AttachCardsAnyWay(pile, 1, (c, m) => m.Def.IsPokemonV, "Melony: attach a Water Energy card to 1 of your Pokémon V", false);
                if (attached > 0) ctx.Draw(3);
            }
        }

        /// <summary>Roxanne: only if your opponent has 3 or fewer Prize cards left. Both shuffle their hands into their decks; you draw 6, they draw 2.</summary>
        private sealed class Roxanne : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Prizes.Count <= 3; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Draw(6);
                ctx.Engine.DrawCards(ctx.Opponent, 2);
            }
        }
    }
}
