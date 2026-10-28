using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v80: Team Up (TEU 1-196) and the Prism Star rule.</summary>
    public class TeuTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Shaymin = "TEU-10";
        const string TapuKoko = "TEU-51";
        const string GengarMimikyu = "TEU-53";
        const string MrMimeGX = "TEU-67";
        const string Kabutops = "TEU-78";
        const string Absol = "TEU-88";
        const string Hoopa = "TEU-96";
        const string Cobalion = "TEU-106";
        const string Klefki = "TEU-110";
        const string Mimikyu = "TEU-112";
        const string EeveeSnorlax = "TEU-120";
        const string BlackMarket = "TEU-134";
        const string BuffPadding = "TEU-136";
        const string Nanu = "TEU-150";
        const string Labyrinth = "TEU-158";
        const string MagikarpWailord = "TEU-160";
        const string Wartortle = "TEU-24";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "TEU" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(148, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Magikarp & Wailord-GX", C(MagikarpWailord).Name, "printed only as a full art");
            Assert.AreEqual(MagikarpWailord, C("TEU-183").BaseKey, "and as a rainbow rare");
            Assert.AreEqual("30C-CC25", C("TEU-33").BaseKey, "Pikachu & Zekrom-GX is the card already in the pool");
            Assert.AreEqual("Nidoran♀", C("TEU-55").EvolvesFrom);
            Assert.AreEqual("Unidentified Fossil", C("TEU-75").EvolvesFrom);
            Assert.AreEqual(0, C("TEU-73").Attacks[0].Damage, "Special Combo has no damage of its own");
            Assert.AreEqual("Team Up", C("TEU-1").PrintSetName);
        }

        [Test]
        public void Prism_Star_Cards_Are_One_Of_A_Name_And_Are_Read_From_Deck_Lists()
        {
            Assert.IsTrue(C(TapuKoko).IsPrismStar);
            Assert.IsTrue(C(BlackMarket).IsPrismStar);
            Assert.IsFalse(C("TEU-52").IsPrismStar);
            Assert.AreEqual("Prism Star", C(Shaymin).Mechanic);
            Assert.AreEqual(1, C(Shaymin).PrizeValue);

            var deck = new List<CardDef> { C(TapuKoko), C(TapuKoko) };
            while (deck.Count < 60) deck.Add(C(Lightning));
            Assert.IsTrue(Decklist.Validate(deck, (ICollection<string>)null).Any(p => p.Contains("Prism Star")));
            deck[1] = C(Shaymin);
            Assert.IsFalse(Decklist.Validate(deck, (ICollection<string>)null).Any(p => p.Contains("Prism Star")), "different names are fine");

            var errors = new List<string>();
            var entries = Decklist.Parse("1 Tapu Koko Prism Star\n1 Shaymin {*}\n1 Black Market ◇ TEU 134", db, errors);
            CollectionAssert.IsEmpty(errors);
            Assert.AreEqual(TapuKoko, entries[0].Card.BaseKey);
            Assert.AreEqual(Shaymin, entries[1].Card.BaseKey);
            Assert.AreEqual(BlackMarket, entries[2].Card.BaseKey);
        }

        [Test]
        public void A_Prism_Star_Card_Goes_To_The_Lost_Zone_Instead_Of_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var shaymin = e.ScenarioBench(0, C(Shaymin));
            e.ScenarioAttach(shaymin, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            var market = e.ScenarioAddToHand(0, C(BlackMarket));
            e.ScenarioSetTurn(0, 3);
            var me = e.State.Players[0];
            e.DiscardFromHand(0, market);
            Assert.IsTrue(me.LostZone.Contains(market));
            Assert.IsFalse(me.Discard.Contains(market));

            e.DiscardPokemonFromPlay(shaymin);
            Assert.AreEqual(1, me.LostZone.Count(c => c.Def.Name == "Shaymin ◇"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Psychic Energy"), "its Energy is discarded as usual");
        }

        [Test]
        public void Dance_Of_The_Ancients_Gives_2_Benched_Pokemon_A_Lightning_Energy_And_Leaves_For_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dance of the Ancients"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var koko = e.ScenarioBench(0, C(TapuKoko));
            e.ScenarioAttach(koko, C(Lightning));
            var a = e.ScenarioBench(0, C(Snubbull));
            var b = e.ScenarioBench(0, C(Mewtwo));
            e.ScenarioAddToDiscard(0, C(Lightning), 2);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, a.Energy.Count);
            Assert.AreEqual(1, b.Energy.Count);
            Assert.IsFalse(me.Bench.Contains(koko));
            Assert.AreEqual(1, me.LostZone.Count(c => c.Def.Name == "Tapu Koko ◇"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Lightning Energy"), "its own Energy was discarded");
        }

        [Test]
        public void Wondrous_Labyrinth_Makes_Non_Fairy_Attacks_Cost_More_And_Ignores_Items()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wartortle = e.ScenarioActive(0, C(Wartortle));
            var mimikyu = e.ScenarioActive(1, C(Mimikyu));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.AttackCost(wartortle, wartortle.Def.Attacks[0]).Count);
            e.ScenarioSetStadium(1, C(Labyrinth));
            Assert.AreEqual(3, e.AttackCost(wartortle, wartortle.Def.Attacks[0]).Count);
            Assert.AreEqual(2, e.AttackCost(mimikyu, mimikyu.Def.Attacks[1]).Count, "Fairy Pokémon are unaffected");

            // Lost Vacuum (an Item) can't take it: only a Pokémon Tool is offered.
            var e2 = Scenario(new ScriptedAgent().Then("Play Lost Vacuum"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Snubbull));
            var theirs = e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioAttachTool(theirs, C(BuffPadding));
            e2.ScenarioSetStadium(1, C(Labyrinth));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioAddToHand(0, C("CRZ-135"));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual("Wondrous Labyrinth ◇", e2.State.Stadium.Def.Name);
            Assert.AreEqual(0, theirs.Tools.Count, "the Tool went instead");
        }

        [Test]
        public void Black_Market_Takes_1_Prize_Card_Less_For_A_Darkness_Pokemon_With_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var wartortle = e.ScenarioActive(0, C(Wartortle));
            e.ScenarioAttach(wartortle, C(Water));
            e.ScenarioAttach(wartortle, C(Water));
            var hoopa = e.ScenarioActive(1, C(Hoopa));
            e.ScenarioAttach(hoopa, C(Darkness));
            hoopa.DamageCounters = 17;
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetStadium(1, C(BlackMarket));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "1 Prize card for Hoopa-GX instead of 2");
        }

        [Test]
        public void Klefki_Makes_Metal_Resistance_40_But_Gives_None_To_Pokemon_Without_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            var cobalion = e.ScenarioActive(1, C(Cobalion));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mewtwo, cobalion, 60);
            Assert.AreEqual(4, cobalion.DamageCounters, "Psychic Resistance -20");
            cobalion.DamageCounters = 0;
            e.ScenarioBench(1, C(Klefki));
            e.DealAttackDamage(mewtwo, cobalion, 60);
            Assert.AreEqual(2, cobalion.DamageCounters, "-40 with Key of Secrets");
            Assert.AreEqual(40, e.ResistanceAmountOf(cobalion));
            var klefki = e.State.Players[1].Bench[0];
            Assert.AreEqual(40, e.ResistanceAmountOf(klefki), "Klefki is a Metal Pokémon too");
            Assert.AreEqual(mewtwo.Def.ResistanceAmount, e.ResistanceAmountOf(mewtwo), "only its owner's Metal Pokémon");
        }

        [Test]
        public void Devilish_Hands_Adds_Up_The_30s_And_Ignores_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Devilish Hands-GX"), new ScriptedAgent());
            var hoopa = e.ScenarioActive(0, C(Hoopa));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(hoopa, C(Darkness));
            var gengar = e.ScenarioActive(1, C(GengarMimikyu));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, gengar.DamageCounters, "6 x 30, no Weakness");
            Assert.AreEqual(0, benched.DamageCounters, "only Pokémon-GX and Pokémon-EX can be chosen");
            Assert.IsTrue(e.State.Players[0].GxUsed);
        }

        [Test]
        public void Magic_Odds_Prevents_Odd_Tens_Of_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            var mime = e.ScenarioActive(1, C(MrMimeGX));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mewtwo, mime, 30);
            Assert.AreEqual(0, mime.DamageCounters);
            e.DealAttackDamage(mewtwo, mime, 40);
            Assert.AreEqual(4, mime.DamageCounters);
            e.DealAttackDamage(mewtwo, mime, 250);
            Assert.AreEqual(4, mime.DamageCounters, "250 is prevented too");
        }

        [Test]
        public void Horror_House_Stops_The_Opponent_Playing_Cards_And_Both_Draw_To_7()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Horror House-GX"), new ScriptedAgent());
            var gm = e.ScenarioActive(0, C(GengarMimikyu));
            e.ScenarioAttach(gm, C(Psychic));
            e.ScenarioAttach(gm, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(BuffPadding));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
            Assert.AreEqual(7, e.State.Players[1].Hand.Count);
            NextTurn(e);
            var actions = e.GetMainActions(1);
            Assert.IsFalse(actions.Any(o => o.Action == ActionType.BenchBasic || o.Action == ActionType.AttachEnergy || o.Action == ActionType.PlayTrainer),
                string.Join(" | ", actions.Select(o => o.Label).ToArray()));
        }

        [Test]
        public void Fossilized_Memories_Locks_Supporters_And_Dark_Ambition_Raises_Retreat()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Kabutops));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Nanu));
            e.ScenarioAddToDiscard(1, C(Absol), 1);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label.Contains("Nanu")), "no Supporters while Kabutops is Active");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Absol));
            var theirs = e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(1, 4);
            Assert.AreEqual(theirs.Def.RetreatCost + 1, e2.RetreatCostOf(theirs));
        }

        [Test]
        public void Nanu_Swaps_A_Basic_In_Play_For_A_Basic_Darkness_Pokemon_From_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Nanu"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttach(mon, C(Psychic));
            mon.DamageCounters = 2;
            e.ScenarioAddToHand(0, C(Nanu));
            e.ScenarioAddToDiscard(0, C(Absol), 1);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual("Absol", me.Active.Name);
            Assert.AreSame(mon, me.Active, "the same Pokémon in play");
            Assert.AreEqual(1, mon.Energy.Count);
            Assert.AreEqual(2, mon.DamageCounters);
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Snubbull"));
        }

        [Test]
        public void Copycat_Uses_The_Attack_The_Opponent_Used_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Copycat"), new ScriptedAgent().Then("Attack: Tackle"));
            var mimikyu = e.ScenarioActive(0, C(Mimikyu));
            e.ScenarioAttach(mimikyu, C("SUM-Y"));
            e.ScenarioAttach(mimikyu, C(Psychic));
            var wartortle = e.ScenarioActive(1, C(Wartortle));
            e.ScenarioAttach(wartortle, C(Water));
            e.ScenarioAttach(wartortle, C(Water));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(3, mimikyu.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(3, wartortle.DamageCounters, "Tackle, used as Copycat");
        }

        [Test]
        public void Buff_Padding_Gives_50_HP_At_Retreat_Cost_4()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var big = e.ScenarioActive(0, C(EeveeSnorlax));
            var small = e.ScenarioBench(0, C(Snubbull));
            e.ScenarioAttachTool(big, C(BuffPadding));
            e.ScenarioAttachTool(small, C(BuffPadding));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(320, e.MaxHp(big));
            Assert.AreEqual(small.Def.Hp, e.MaxHp(small));
        }
    }
}
