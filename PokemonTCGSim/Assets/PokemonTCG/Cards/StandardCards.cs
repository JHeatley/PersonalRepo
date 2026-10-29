using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Registers every implemented card script. Pokémon are registered by printing key (different printings
    /// of the same Pokémon have different attacks); Trainers by name (reprints share text).
    /// Cards with only plain damage attacks (Dreepy, Drakloak's attack, etc.) need no entry beyond their abilities.
    /// </summary>
    public static class StandardCards
    {
        public static CardScriptRegistry CreateRegistry()
        {
            var r = new CardScriptRegistry();

            // Pokémon
            r.RegisterByKey("TWM-129", new Drakloak());
            r.RegisterByKey("TWM-130", new DragapultEx());
            r.RegisterByKey("TWM-95", new Munkidori());
            r.RegisterByKey("ASC-16", new Budew());
            r.RegisterByKey("JTG-120", new Dunsparce());
            r.RegisterByKey("TEF-129", new Dudunsparce());
            r.RegisterByKey("JTG-121", new DudunsparceEx());
            r.RegisterByKey("POR-62", new MeowthEx());
            r.RegisterByKey("ASC-142", new FezandipitiEx());

            // Supporters
            r.RegisterByName("Lillie's Determination", new LilliesDetermination());
            r.RegisterByName("Boss's Orders", new BosssOrders());
            r.RegisterByName("Crispin", new Crispin());
            r.RegisterByName("Rosa's Encouragement", new RosasEncouragement());

            // Items
            r.RegisterByName("Buddy-Buddy Poffin", new BuddyBuddyPoffin());
            r.RegisterByName("Crushing Hammer", new CrushingHammer());
            r.RegisterByName("Poké Pad", new PokePad());
            r.RegisterByName("Night Stretcher", new NightStretcher());
            r.RegisterByName("Ultra Ball", new UltraBall());
            r.RegisterByName("Special Red Card", new SpecialRedCard());
            r.RegisterByName("Unfair Stamp", new UnfairStamp());

            // Stadiums
            r.RegisterByName("Risky Ruins", new RiskyRuins());

            // N's Zoroark ex list
            NsZoroarkCards.Register(r);

            // Recife Regional Championship decks (Limitless tournament 580)
            RecifeTrainers.Register(r);
            RecifeEnergy.Register(r);
            RecifePokemonA.Register(r);
            RecifePokemonB.Register(r);

            // Temporal Forces (v8)
            TefPokemon.Register(r);
            TefTrainers.Register(r);

            // Stellar Crown (v11)
            CardsSCR1.Register(r);
            CardsSCR2.Register(r);

            // Journey Together (v12)
            CardsJTG1.Register(r);
            CardsJTG2.Register(r);

            // Twilight Masquerade (v13)
            CardsTWM1.Register(r);
            CardsTWM2.Register(r);

            // Shrouded Fable (v14)
            CardsSFA.Register(r);

            // Surging Sparks (v15)
            CardsSSP1.Register(r);
            CardsSSP2.Register(r);
            CardsSSP3.Register(r);

            // Prismatic Evolutions (v16)
            CardsPRE.Register(r);

            // Destined Rivals (v18)
            CardsDRI1.Register(r);
            CardsDRI2.Register(r);
            // White Flare / Black Bolt (v18)
            CardsWHT.Register(r);
            CardsBLK.Register(r);
            // Mega Evolution / Phantasmal Flames / Mega Evolution promos (v18)
            CardsMEG.Register(r);
            CardsPFL.Register(r);
            CardsMEP.Register(r);
            // Ascended Heroes (v19)
            CardsASC.Register(r);
            // Perfect Order (v19)
            CardsPOR.Register(r);
            // Chaos Rising (v19)
            CardsCRI.Register(r);
            // Pitch Black (v19)
            CardsPBL.Register(r);
            // 30th Celebration (v19)
            Cards30C.Register(r);
            // Scarlet & Violet promos (v19)
            CardsSVP.Register(r);
            // 30th Celebration Classic Collection (v20): Unlimited (before Black & White) and Expanded reprints
            Cards30CC.Register(r);
            // Paldean Fates (v36): G-mark, Expanded only
            CardsPAF1.Register(r);
            CardsPAF2.Register(r);
            // Paradox Rift (v37): G-mark, Expanded only
            CardsPAR1.Register(r);
            CardsPAR2.Register(r);
            // Pokémon 151 (v38): G-mark, Expanded only
            CardsMEW1.Register(r);
            CardsMEW2.Register(r);
            // Obsidian Flames (v39): G-mark, Expanded only
            CardsOBF1.Register(r);
            CardsOBF2.Register(r);
            // Paldea Evolved (v40): G-mark, Expanded only
            CardsPAL1.Register(r);
            CardsPAL2.Register(r);
            // Scarlet & Violet (v41): G-mark, Expanded only
            CardsSVI1.Register(r);
            CardsSVI2.Register(r);
            // Wizards Black Star Promos (v48): before Black & White, Unlimited only
            CardsWP.Register(r);
            CardsCRZ1.Register(r);
            CardsCRZ2.Register(r);
            CardsSIT1.Register(r);
            CardsSIT2.Register(r);
            CardsLOR1.Register(r);
            CardsLOR2.Register(r);
            CardsPGO.Register(r);
            CardsASR1.Register(r);
            CardsASR2.Register(r);
            CardsBRS1.Register(r);
            CardsBRS2.Register(r);
            CardsFST1.Register(r);
            CardsFST2.Register(r);
            CardsCEL.Register(r);
            CardsEVS1.Register(r);
            CardsEVS2.Register(r);
            CardsCRE1.Register(r);
            CardsCRE2.Register(r);
            CardsBST1.Register(r);
            CardsBST2.Register(r);
            CardsSHF1.Register(r);
            CardsSHF2.Register(r);
            CardsVIV1.Register(r);
            CardsVIV2.Register(r);
            CardsCPA.Register(r);
            CardsDAA1.Register(r);
            CardsDAA2.Register(r);
            CardsRCL1.Register(r);
            CardsRCL2.Register(r);
            CardsSSH1.Register(r);
            CardsSSH2.Register(r);
            CardsSP1.Register(r);
            CardsSP2.Register(r);
            CardsCEC1.Register(r);
            CardsCEC2.Register(r);
            CardsCEC3.Register(r);
            CardsCEC4.Register(r);
            CardsHIF1.Register(r);
            CardsHIF2.Register(r);
            CardsHIF3.Register(r);
            CardsUNM1.Register(r);
            CardsUNM2.Register(r);
            CardsUNM3.Register(r);
            CardsUNM4.Register(r);
            CardsUNB1.Register(r);
            CardsUNB2.Register(r);
            CardsUNB3.Register(r);
            CardsUNB4.Register(r);
            CardsDET.Register(r);
            CardsTEU1.Register(r);
            CardsTEU2.Register(r);
            CardsTEU3.Register(r);
            CardsTEU4.Register(r);
            CardsLOT1.Register(r);
            CardsLOT2.Register(r);
            CardsLOT3.Register(r);
            CardsLOT4.Register(r);
            CardsLOT5.Register(r);
            CardsDRM1.Register(r);
            CardsDRM2.Register(r);
            // Celestial Storm (v83)
            CardsCES1.Register(r);
            CardsCES2.Register(r);
            CardsCES3.Register(r);
            CardsCES4.Register(r);
            // Forbidden Light (v84)
            CardsFLI1.Register(r);
            CardsFLI2.Register(r);
            CardsFLI3.Register(r);
            CardsFLI4.Register(r);
            // Ultra Prism (v85): after Forbidden Light, whose scripts some of its cards reuse
            CardsUPR1.Register(r);
            CardsUPR2.Register(r);
            CardsUPR3.Register(r);
            CardsUPR4.Register(r);
            CardsUPR5.Register(r);
            // Crimson Invasion (v86)
            CardsCIN1.Register(r);
            CardsCIN2.Register(r);
            CardsCIN3.Register(r);
            CardsCIN4.Register(r);
            CardsSLG1.Register(r);
            CardsSLG2.Register(r);
            CardsBUS1.Register(r);
            CardsBUS2.Register(r);
            CardsBUS3.Register(r);
            CardsBUS4.Register(r);
            CardsBUS5.Register(r);
            CardsGRI1.Register(r);
            CardsGRI2.Register(r);
            CardsGRI3.Register(r);
            CardsGRI4.Register(r);
            CardsGRI5.Register(r);
            CardsSUM1.Register(r);
            CardsSUM2.Register(r);
            CardsSUM3.Register(r);
            CardsSUM4.Register(r);
            CardsSUM5.Register(r);
            CardsSMP1.Register(r);
            CardsSMP2.Register(r);
            CardsSMP3.Register(r);
            CardsSMP4.Register(r);
            // Evolutions (EVO, v92).
            CardsEVO1.Register(r);
            CardsEVO2.Register(r);
            CardsEVO3.Register(r);
            // Steam Siege (STS, v93).
            CardsSTS1.Register(r);
            CardsSTS2.Register(r);
            CardsSTS3.Register(r);
            // Fates Collide (FCO, v94).
            CardsFCO1.Register(r);
            CardsFCO2.Register(r);
            CardsFCO3.Register(r);
            CardsGEN1.Register(r);
            CardsGEN2.Register(r);
            CardsBKP1.Register(r);
            CardsBKP2.Register(r);
            CardsBKP3.Register(r);
            CardsBKT1.Register(r);
            CardsBKT2.Register(r);
            CardsBKT3.Register(r);
            CardsAOR1.Register(r);
            CardsAOR2.Register(r);
            CardsAOR3.Register(r);
            CardsROS1.Register(r);
            CardsROS2.Register(r);
            CardsROS3.Register(r);
            CardsDCR.Register(r);
            CardsPRC1.Register(r);
            CardsPRC2.Register(r);
            CardsPRC3.Register(r);
            CardsPHF1.Register(r);
            CardsPHF2.Register(r);
            CardsPHF3.Register(r);
            CardsFFI1.Register(r);
            CardsFFI2.Register(r);
            CardsFFI3.Register(r);
            CardsFLF1.Register(r);
            CardsFLF2.Register(r);
            CardsFLF3.Register(r);
            CardsXY1.Register(r);
            CardsXY2.Register(r);
            CardsXY3.Register(r);
            CardsKSS.Register(r);
            CardsXYP1.Register(r);
            CardsXYP2.Register(r);
            CardsXYP3.Register(r);
            CardsLTR1.Register(r);
            CardsLTR2.Register(r);
            CardsLTR3.Register(r);
            CardsPLB1.Register(r);
            CardsPLB2.Register(r);
            CardsPLF1.Register(r);
            CardsPLF2.Register(r);
            CardsPLS1.Register(r);
            CardsPLS2.Register(r);
            CardsBCR1.Register(r);
            CardsBCR2.Register(r);
            CardsBS1.Register(r);
            CardsBS2.Register(r);
            CardsJU.Register(r);
            CardsFO.Register(r);
            CardsTR.Register(r);
            CardsG1.Register(r);
            CardsDRV.Register(r);
            CardsDRX.Register(r);
            CardsG2.Register(r);
            CardsDEX.Register(r);
            CardsNXD.Register(r);
            CardsNVI.Register(r);
            CardsEPO.Register(r);
            CardsBLW.Register(r);
            CardsBWP.Register(r);
            CardsHSP.Register(r);
            CardsDPP.Register(r);
            CardsNP.Register(r);
            CardsN1.Register(r);
            CardsN1T.Register(r);
            CardsN2.Register(r);
            CardsN2T.Register(r);
            CardsSI.Register(r);
            CardsN3.Register(r);
            CardsN3T.Register(r);
            CardsN4.Register(r);
            CardsN4T.Register(r);
            CardsLC.Register(r);
            CardsE1.Register(r);
            CardsE1T.Register(r);
            CardsBG.Register(r);
            CardsE2.Register(r);
            CardsE2B.Register(r);
            CardsE2T.Register(r);
            CardsE3.Register(r);
            CardsE3B.Register(r);
            CardsE3T.Register(r);
            CardsRM.Register(r);
            CardsRS.Register(r);
            CardsRSB.Register(r);
            CardsRST.Register(r);
            CardsMP.Register(r);
            CardsDLR.Register(r);
            CardsM6.Register(r);
            CardsFLO.Register(r);
            CardsChina.Register(r);
            CardsSS.Register(r);
            CardsSSB.Register(r);
            CardsSST.Register(r);
            CardsDR.Register(r);
            CardsDRB.Register(r);
            CardsDRT.Register(r);
            CardsMA.Register(r);
            CardsMAB.Register(r);
            CardsMAT.Register(r);
            CardsHL.Register(r);
            CardsHLB.Register(r);
            CardsHLT.Register(r);
            CardsRG.Register(r);
            CardsRGB.Register(r);
            CardsRGT.Register(r);
            CardsP1.Register(r);
            CardsTRR.Register(r);
            CardsTRRB.Register(r);
            CardsTRRT.Register(r);
            CardsDX.Register(r);
            CardsDXB.Register(r);
            CardsDXT.Register(r);
            CardsEM.Register(r);
            CardsEMB.Register(r);
            CardsEMT.Register(r);
            CardsP2.Register(r);
            CardsUF.Register(r);
            CardsUFB.Register(r);
            CardsUFT.Register(r);
            CardsDS.Register(r);
            CardsDSB.Register(r);
            CardsDST.Register(r);
            CardsLM.Register(r);
            CardsLMB.Register(r);
            CardsLMT.Register(r);

            return r;
        }
    }

    // ================================================================== Pokémon

    /// <summary>Recon Directive: look at the top 2 cards, put 1 in hand and the other on the bottom.</summary>
    public sealed class Drakloak : CardScript
    {
        const string Ability = "Recon Directive";

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            return ctx.IsMyTurn && !ctx.AbilityUsedThisTurn(Ability) && ctx.Me.Deck.Count > 0;
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            var deck = ctx.Me.Deck;
            int n = System.Math.Min(2, deck.Count);
            var looked = deck.Take(n).ToList();
            deck.RemoveRange(0, n);
            if (looked.Count == 1)
            {
                ctx.Me.Hand.Add(looked[0]);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 card into their hand.");
                return;
            }
            var pick = ctx.ChooseCards(looked, 1, 1, "Put 1 card into your hand (the other goes to the bottom of your deck)")[0];
            ctx.Me.Hand.Add(pick);
            foreach (var c in looked) if (c != pick) deck.Add(c);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 card into their hand and 1 on the bottom of their deck.");
        }
    }

    /// <summary>Phantom Dive: 200 damage, then 6 damage counters on the opponent's Bench in any way.</summary>
    public sealed class DragapultEx : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            ctx.DamageDefending(atk.Damage);
            if (atk.Name != "Phantom Dive") return;
            ctx.PlaceCountersAnyWay(6, () => ctx.OpponentBench());
        }
    }

    /// <summary>Adrena-Brain (needs Darkness Energy): move up to 3 damage counters from 1 of your Pokémon to 1 of theirs. Mind Bend: Confused.</summary>
    public sealed class Munkidori : CardScript
    {
        const string Ability = "Adrena-Brain";

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            if (!ctx.IsMyTurn || ctx.AbilityUsedThisTurn(Ability)) return false;
            if (!ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Darkness)) return false;
            if (!ctx.Opp.HasPokemonInPlay) return false;
            return ctx.MyPokemon().Any(m => m.DamageCounters > 0);
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            var damaged = ctx.MyPokemon().Where(m => m.DamageCounters > 0).ToList();
            var from = ctx.ChoosePokemon(damaged, "Adrena-Brain: move damage counters from which of your Pokémon?");
            int amount = ctx.AskNumber(1, System.Math.Min(3, from.DamageCounters), "How many damage counters to move?");
            var to = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Adrena-Brain: move them to which of your opponent's Pokémon?");
            ctx.Engine.MoveDamageCounters(from, to, amount, Ability);
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Mind Bend") ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Confused);
        }
    }

    /// <summary>Itchy Pollen: during the opponent's next turn they can't play Items.</summary>
    public sealed class Budew : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Itchy Pollen")
            {
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Items during their next turn.");
            }
        }
    }

    /// <summary>Trading Places: switch this Pokémon with 1 of your Benched Pokémon.</summary>
    public sealed class Dunsparce : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Trading Places")
            {
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Active != ctx.Pokemon) return;
                var target = ctx.ChoosePokemon(ctx.Me.Bench.ToList(), "Trading Places: switch with which Benched Pokémon?");
                ctx.Engine.SwitchActive(ctx.Player, target);
                return;
            }
            ctx.DamageDefending(atk.Damage);
        }
    }

    /// <summary>Run Away Draw: draw 3, then shuffle this Pokémon and everything attached into the deck.</summary>
    public sealed class Dudunsparce : CardScript
    {
        const string Ability = "Run Away Draw";

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            return ctx.IsMyTurn && !ctx.AbilityUsedThisTurn(Ability) && ctx.Me.Deck.Count > 0;
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            int drawn = ctx.Draw(3);
            if (drawn > 0) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
        }
    }

    /// <summary>Tenacious Tail: 60 for each of the opponent's Pokémon ex. Destructive Drill: plain 150.</summary>
    public sealed class DudunsparceEx : CardScript
    {
        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Tenacious Tail")
            {
                int exCount = ctx.OpponentPokemon().Count(m => m.Def.IsEx || m.Def.IsMegaEx);
                ctx.DamageDefending(atk.Damage * exCount);
                return;
            }
            // Destructive Drill: not affected by any effects on the opponent's Active Pokémon.
            ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
        }
    }

    /// <summary>Last-Ditch Catch (when played from hand to Bench): search for a Supporter. Tuck Tail: return to hand.</summary>
    public sealed class MeowthEx : CardScript
    {
        const string Limit = "Last-Ditch";

        public override void OnPlayedFromHandToBench(EffectContext ctx)
        {
            if (!ctx.IsMyTurn || ctx.NamedLimitUsedThisTurn(Limit) || ctx.Me.Deck.Count == 0) return;
            if (!ctx.AskYesNo("Use Last-Ditch Catch to search your deck for a Supporter?")) return;
            ctx.MarkNamedLimit(Limit);
            ctx.MarkAbilityUsed("Last-Ditch Catch");
            ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Meowth ex used Last-Ditch Catch.", ctx.Card.Id, ctx.Pokemon.Uid);
            var found = ctx.SearchMyDeck(d => d.IsSupporter, 1, "Choose a Supporter to put into your hand");
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            ctx.DamageDefending(atk.Damage);
            if (atk.Name == "Tuck Tail") ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
        }
    }

    /// <summary>Flip the Script: draw 3 if one of your Pokémon was KO'd last turn. Cruel Arrow: 100 to any 1 Pokémon.</summary>
    public sealed class FezandipitiEx : CardScript
    {
        const string Ability = "Flip the Script";

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            return ctx.IsMyTurn
                && !ctx.AbilityUsedThisTurn(Ability)
                && !ctx.NamedLimitUsedThisTurn(Ability)
                && ctx.WasKnockedOutLastOpponentTurn()
                && ctx.Me.Deck.Count > 0;
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Ability);
            ctx.MarkNamedLimit(Ability);
            ctx.Draw(3);
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (atk.Name == "Cruel Arrow")
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Cruel Arrow: choose 1 of your opponent's Pokémon");
                // Weakness/Resistance only apply to the Active Pokémon; DealAttackDamage handles that.
                ctx.DamageTarget(target, atk.Damage);
                return;
            }
            ctx.DamageDefending(atk.Damage);
        }
    }

    // ================================================================== Supporters

    public sealed class LilliesDetermination : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
            ctx.Draw(ctx.Me.Prizes.Count == 6 ? 8 : 6);
        }
    }

    public sealed class BosssOrders : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Opp.Bench.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            // Only Pokémon this card can affect can be chosen (Fraxure's Unnerve, Diancie's Princess's Curtain, Thievul's Baffling, v49).
            var targets = ctx.GustTargets();
            if (targets.Count == 0)
            {
                ctx.Log("Boss's Orders: none of your opponent's Benched Pokémon can be switched in.");
                return;
            }
            var target = ctx.ChoosePokemon(targets, "Boss's Orders: choose a Benched Pokémon to switch in");
            ctx.Engine.SwitchActive(ctx.Opponent, target);
        }
    }

    public sealed class Crispin : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var first = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Crispin: choose a Basic Energy (1 of up to 2)");
            var found = new List<CardInstance>(first);
            if (first.Count == 1)
            {
                var firstType = first[0].Def.EnergyProvides;
                var second = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides != firstType, 1, "Crispin: choose a Basic Energy of a different type");
                found.AddRange(second);
            }

            if (found.Count == 1)
            {
                ctx.Me.Hand.Add(found[0]);
            }
            else if (found.Count == 2)
            {
                var toHand = ctx.ChooseCards(found, 1, 1, "Crispin: choose the Energy to put into your hand (the other is attached)")[0];
                var toAttach = found[0] == toHand ? found[1] : found[0];
                ctx.Me.Hand.Add(toHand);
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Crispin: attach " + toAttach.Def.Name + " to which Pokémon?");
                ctx.Engine.AttachEnergyCard(toAttach, target);
            }
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class RosasEncouragement : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count
                && ctx.MyPokemon().Any(m => m.Def.Stage == Stage.Stage2)
                && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy);
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var stage2 = ctx.MyPokemon().Where(m => m.Def.Stage == Stage.Stage2).ToList();
            var target = ctx.ChoosePokemon(stage2, "Rosa's Encouragement: choose a Stage 2 Pokémon");
            var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
            var picks = ctx.ChooseCards(energy, 0, 2, "Choose up to 2 Basic Energy from your discard pile to attach");
            foreach (var e in picks)
            {
                ctx.Me.Discard.Remove(e);
                ctx.Engine.AttachEnergyCard(e, target);
            }
        }
    }

    // ================================================================== Items

    public sealed class BuddyBuddyPoffin : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToBench(d => d.Hp <= 70, 2, "Choose up to 2 Basic Pokémon with 70 HP or less for your Bench");
        }
    }

    public sealed class CrushingHammer : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.OpponentPokemon().Any(m => m.Energy.Count > 0);
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            if (!ctx.Flip()) return;
            var withEnergy = ctx.OpponentPokemon().Where(m => m.Energy.Count > 0).ToList();
            var target = ctx.ChoosePokemon(withEnergy, "Crushing Hammer: choose 1 of your opponent's Pokémon");
            var energy = ctx.ChooseCards(target.Energy, 1, 1, "Choose an Energy to discard")[0];
            ctx.Engine.DiscardEnergy(target, energy);
        }
    }

    public sealed class PokePad : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(d => d.IsPokemon && !d.HasRuleBox, 1, "Choose a Pokémon without a Rule Box");
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class NightStretcher : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Discard.Any(Eligible);
        }

        private static bool Eligible(CardInstance c)
        {
            return c.Def.IsPokemon || c.Def.IsBasicEnergy;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var options = ctx.Me.Discard.Where(Eligible).ToList();
            var pick = ctx.ChooseCards(options, 1, 1, "Night Stretcher: put a Pokémon or Basic Energy into your hand")[0];
            // Through DiscardToHand so Toedscruel's Slime Mold Colony stops it (v38 fix).
            ctx.DiscardToHand(new List<CardInstance> { pick });
        }
    }

    public sealed class UltraBall : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            // ctx.Card is still in hand while legality is checked. The discard is a cost, so with an empty deck there's no
            // effect and it can't be played (Compendium: Lost Vacuum, 2023-06-01; same as the other search Items here).
            return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Deck.Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var discards = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Ultra Ball: discard 2 cards from your hand");
            foreach (var c in discards) ctx.Engine.DiscardFromHand(ctx.Player, c);
            var found = ctx.SearchMyDeck(d => d.IsPokemon, 1, "Choose a Pokémon to put into your hand");
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class SpecialRedCard : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Opp.Prizes.Count <= 3;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var opp = ctx.Opp;
            var hand = opp.Hand.ToList();
            opp.Hand.Clear();
            ctx.State.Rng.Shuffle(hand);
            opp.Deck.AddRange(hand);
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + hand.Count + " cards from their hand on the bottom of their deck.");
            if (hand.Count > 0) ctx.Engine.DrawCards(ctx.Opponent, 3);
        }
    }

    public sealed class UnfairStamp : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.WasKnockedOutLastOpponentTurn();
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
            ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
            ctx.Draw(5);
            ctx.Engine.DrawCards(ctx.Opponent, 2);
        }
    }

    // ================================================================== Stadiums

    /// <summary>Whenever a player puts a Basic non-Darkness Pokémon onto their Bench during their turn, 2 damage counters on it.</summary>
    public sealed class RiskyRuins : CardScript
    {
        public override void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched)
        {
            if (ctx.State.CurrentPlayer != benched.Owner) return;
            var d = benched.Def;
            if (d.Stage != Stage.Basic || d.PokemonType == EnergyType.Darkness) return;
            ctx.Engine.PlaceDamageCounters(benched, 2, "Risky Ruins");
        }
    }
}
