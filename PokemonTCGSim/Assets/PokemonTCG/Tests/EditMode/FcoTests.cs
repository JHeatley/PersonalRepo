using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v94: Fates Collide (FCO 1-125): Marowak's Bodyguard (your ruling), Restored Pokémon and their Fossils, Chaos Tower, Power
    /// Memory, Strong Energy, Mew's Memories of Dawn, Alakazam-EX's Kinesis and the rest.
    /// </summary>
    public class FcoTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Fairy = "SUM-Y";
        const string Grass = "MEE-9";
        const string DoubleColorless = "SLG-69";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string PidgeotEX = "EVO-64";
        const string MPidgeotEX = "EVO-65";
        const string Numel = "MEG-21";
        const string MCamerupt = "MEG-22";
        const string Budew = "ASC-16";
        const string Carnivine = "SLG-6";
        const string Cubone = "JTG-76";
        const string FanClub = "UPR-133";

        const string GlaceonEX = "FCO-20";
        const string Binacle = "FCO-22";
        const string Barbaracle = "FCO-23";
        const string Rotom = "FCO-24";
        const string AlakazamEX = "FCO-25";
        const string MAlakazamEX = "FCO-26";
        const string Mew = "FCO-29";
        const string Gothita = "FCO-32";
        const string Omanyte = "FCO-17";
        const string Omastar = "FCO-18";
        const string OmastarBreak = "FCO-19";
        const string Marowak = "FCO-37";
        const string Kabuto = "FCO-38";
        const string RegirockEX = "FCO-43";
        const string Hawlucha = "FCO-48";
        const string CarbinkKeeper = "FCO-49";
        const string CarbinkSafeguard = "FCO-50";
        const string CarbinkBreak = "FCO-51";
        const string ZygardeEX = "FCO-54";
        const string UmbreonEX = "FCO-55";
        const string Vullaby = "FCO-57";
        const string Mandibuzz = "FCO-58";
        const string Bronzor = "FCO-60";
        const string Bronzong = "FCO-61";
        const string Aerodactyl = "FCO-76";
        const string Snorlax = "FCO-77";
        const string Whismur = "FCO-80";
        const string Loudred = "FCO-81";
        const string Exploud = "FCO-82";
        const string AltariaEX = "FCO-83";
        const string MinccinoTail = "FCO-87";
        const string CinccinoCovet = "FCO-88";
        const string AlakazamLink = "FCO-90";
        const string BentSpoon = "FCO-93";
        const string ChaosTower = "FCO-94";
        const string DomeFossil = "FCO-96";
        const string EnergyPouch = "FCO-97";
        const string FairyGarden = "FCO-100";
        const string LassesSpecial = "FCO-103";
        const string MegaCatcher = "FCO-104";
        const string PowerMemory = "FCO-108";
        const string RandomReceiver = "FCO-109";
        const string ScorchedEarth = "FCO-110";
        const string StrongEnergy = "FCO-115";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private PokemonInPlay MarowakOnBench(GameEngine e, int player)
        {
            var marowak = e.ScenarioBench(player, C(Cubone));
            e.ScenarioEvolve(marowak, C(Marowak));
            return marowak;
        }

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Restored_Pokemon_And_Power_Memorys_Attack()
        {
            var own = db.Cards.Where(d => d.SetCode == "FCO" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(109, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("FCO-20", C("FCO-116").BaseKey, "secret rare Glaceon-EX");
            Assert.AreEqual("FCO-25", C("FCO-125").BaseKey, "secret rare Alakazam-EX");
            Assert.AreEqual(Stage.Restored, C(Omanyte).Stage);
            Assert.AreEqual(Stage.Restored, C(Kabuto).Stage);
            Assert.AreEqual(Stage.Restored, C(Aerodactyl).Stage);
            Assert.IsTrue(C(OmastarBreak).IsBreak);
            Assert.IsTrue(C(MAlakazamEX).IsXYMegaEvolution);
            Assert.AreEqual("All Cells Burn", C(PowerMemory).Attacks.Single().Name);
            Assert.AreEqual(200, C(PowerMemory).Attacks.Single().Damage);
        }

        // ------------------------------------------------------------------ Bodyguard (your ruling)

        [Test]
        public void Bodyguard_Stops_The_Opponents_Item_Lock()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Itchy Pollen"));
            e.ScenarioActive(0, C(SnorlaxGX));
            MarowakOnBench(e, 0);
            e.ScenarioActive(1, C(Budew));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(-1, e.State.Players[0].ItemLockTurn, "Bodyguard prevents the lock");
            Assert.IsFalse(e.ItemLocked(0));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Itchy Pollen"));
            e2.ScenarioActive(0, C(SnorlaxGX));
            e2.ScenarioActive(1, C(Budew));
            e2.ScenarioSetTurn(1, 4);
            Step(e2);
            Assert.AreEqual(5, e2.State.Players[0].ItemLockTurn, "without Marowak the lock works");
        }

        [Test]
        public void Bodyguard_Keeps_The_Hand_And_Deck_As_They_Were()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Covet"));
            e.ScenarioActive(0, C(SnorlaxGX));
            MarowakOnBench(e, 0);
            var first = e.ScenarioAddToHand(0, C(Psychic));
            var second = e.ScenarioAddToHand(0, C(DoubleColorless));
            e.ScenarioAddToDeck(0, C(Water), 3);
            var deckBefore = e.State.Players[0].Deck.ToList();
            var cinccino = e.ScenarioActive(1, C(MinccinoTail));
            e.ScenarioEvolve(cinccino, C(CinccinoCovet));
            e.ScenarioAttach(cinccino, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            CollectionAssert.AreEqual(new[] { first, second }, e.State.Players[0].Hand);
            CollectionAssert.AreEqual(deckBefore, e.State.Players[0].Deck);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Covet"));
            e2.ScenarioActive(0, C(SnorlaxGX));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioAddToHand(0, C(DoubleColorless));
            e2.ScenarioAddToDeck(0, C(Water), 3);
            var c2 = e2.ScenarioActive(1, C(MinccinoTail));
            e2.ScenarioEvolve(c2, C(CinccinoCovet));
            e2.ScenarioAttach(c2, C(Psychic));
            e2.ScenarioSetTurn(1, 4);
            Step(e2);
            Assert.AreEqual(1, e2.State.Players[0].Hand.Count, "without Marowak, Covet puts a card on the bottom of the deck");
            Assert.AreEqual(24, e2.State.Players[0].Deck.Count, "20 + 3 + the card from the hand");
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Hand_Block_Stops_Special_Energy_From_The_Hand_While_You_Have_A_Stadium()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var barb = e.ScenarioActive(0, C(Binacle));
            e.ScenarioEvolve(barb, C(Barbaracle));
            e.ScenarioSetStadium(0, C(FairyGarden));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(1, C(DoubleColorless));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            var acts = e.GetMainActions(1);
            Assert.IsFalse(acts.Any(o => o.Action == ActionType.AttachEnergy && o.Card.Name == "Double Colorless Energy"));
            Assert.IsTrue(acts.Any(o => o.Action == ActionType.AttachEnergy && o.Card.Name == "Psychic Energy"));
            e.ScenarioSetStadium(1, C(FairyGarden));
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Action == ActionType.AttachEnergy && o.Card.Name == "Double Colorless Energy"), "not the opponent's Stadium");
        }

        [Test]
        public void Memories_Of_Dawn_Uses_The_Attacks_Of_Your_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(Mew));
            e.ScenarioAttach(mew, C(Fighting));
            e.ScenarioAttach(mew, C(Psychic));
            e.ScenarioBench(0, C(Hawlucha));
            MarowakOnBench(e, 0);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.StartsWith("Attack: Wicked Jab")), "Hawlucha is a Basic Pokémon");
            Assert.IsFalse(labels.Any(l => l.Contains("Bonemerang")), "Marowak isn't");
        }

        [Test]
        public void Kinesis_Puts_Counters_When_M_Alakazam_EX_Is_Played_On_It()
        {
            var e = Scenario(new ScriptedAgent().Then("into M Alakazam-EX", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(AlakazamEX));
            e.ScenarioAddToHand(0, C(MAlakazamEX));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            var benched = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, foe.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters);
            Assert.AreEqual("M Alakazam-EX", e.State.Players[0].Active.Name);
        }

        [Test]
        public void Alakazam_Spirit_Link_Keeps_The_Turn_Going()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ala = e.ScenarioActive(0, C(AlakazamEX));
            e.ScenarioAttachTool(ala, C(AlakazamLink));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNotNull(e.SpiritLinkFor(ala, C(MAlakazamEX)));
        }

        [Test]
        public void Restoring_Beam_And_Dangerous_Tentacle_On_Omastar_Break()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Restoring Beam", "Kabuto", "Use Dangerous Tentacle"), new ScriptedAgent());
            var omastar = e.ScenarioActive(0, C(Omanyte));
            e.ScenarioEvolve(omastar, C(Omastar));
            e.ScenarioEvolve(omastar, C(OmastarBreak));
            e.ScenarioAddToDeck(0, C(Kabuto), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioBench(1, C(PidgeotEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Kabuto", e.State.Players[0].Bench.Single().Name, "the BREAK keeps Omastar's Restoring Beam");
            Step(e);
            Assert.AreEqual("Pidgeot-EX", e.State.Players[1].Active.Name);
        }

        [Test]
        public void Dome_Fossil_Kabuto_Benches_Kabuto_From_The_Bottom_7()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Dome Fossil Kabuto"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(DomeFossil));
            e.ScenarioAddToDeck(0, C(Kabuto), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Kabuto", e.State.Players[0].Bench.Single().Name);
        }

        [Test]
        public void Regi_Power_Adds_10_To_Other_Fighting_Pokemon_And_Strong_Energy_Adds_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wicked Jab"), new ScriptedAgent());
            var haw = e.ScenarioActive(0, C(Hawlucha));
            e.ScenarioAttach(haw, C(StrongEnergy));
            e.ScenarioAttach(haw, C(Fighting));
            e.ScenarioBench(0, C(RegirockEX));
            e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(StrongEnergy));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            var acts = e.GetMainActions(0);
            Assert.IsFalse(acts.Any(o => o.Label.StartsWith("Attach Strong Energy") && o.Label.Contains("Bounsweet")), "only on Fighting Pokémon");
            Assert.IsTrue(acts.Any(o => o.Label.StartsWith("Attach Strong Energy") && o.Label.Contains("Hawlucha")));
            Step(e);
            Assert.AreEqual(6, lapras.DamageCounters, "30 + 20 (Strong Energy) + 10 (Regi Power)");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Bedrock Press"), new ScriptedAgent());
            var regi = e2.ScenarioActive(0, C(RegirockEX));
            for (int i = 0; i < 3; i++) e2.ScenarioAttach(regi, C(Fighting));
            var lapras2 = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(10, lapras2.DamageCounters, "not Regirock-EX's own attacks");
        }

        [Test]
        public void Energy_Keeper_Keeps_Basic_Energy_On_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Crunch"), new ScriptedAgent());
            var carn = e.ScenarioActive(0, C(Carnivine));
            e.ScenarioAttach(carn, C(Grass));
            e.ScenarioAttach(carn, C(Psychic));
            var haw = e.ScenarioActive(1, C(Hawlucha));
            e.ScenarioAttach(haw, C(Fighting));
            e.ScenarioBench(1, C(CarbinkKeeper));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, haw.Energy.Count);
            Assert.AreEqual(3, haw.DamageCounters);
        }

        [Test]
        public void Safeguard_Stops_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var carbink = e.ScenarioActive(0, C(CarbinkSafeguard));
            var regi = e.ScenarioActive(1, C(RegirockEX));
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(regi, carbink, 100);
            Assert.AreEqual(0, carbink.DamageCounters);
            e.DealAttackDamage(lapras, carbink, 30);
            Assert.AreEqual(3, carbink.DamageCounters, "Lapras-GX isn't a Pokémon-EX");
        }

        [Test]
        public void Metal_Fortress_Protects_The_Bench_Not_The_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Bounsweet));
            var bronzong = e.ScenarioBench(0, C(Bronzor));
            e.ScenarioEvolve(bronzong, C(Bronzong));
            var benched = e.ScenarioBench(0, C(Bounsweet));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, benched, 30);
            Assert.AreEqual(0, benched.DamageCounters);
            e.DealAttackDamage(foe, active, 30);
            Assert.AreEqual(3, active.DamageCounters);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Crystal_Ray_Stops_Damage_From_Evolution_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Crystal Ray"), new ScriptedAgent());
            var glaceon = e.ScenarioActive(0, C(GlaceonEX));
            e.ScenarioAttach(glaceon, C(Water));
            e.ScenarioAttach(glaceon, C(DoubleColorless));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var marowak = MarowakOnBench(e, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(marowak, glaceon, 50);
            Assert.AreEqual(0, glaceon.DamageCounters, "Marowak is an Evolution Pokémon");
            e.DealAttackDamage(lapras, glaceon, 50);
            Assert.AreEqual(5, glaceon.DamageCounters, "Lapras-GX is a Basic Pokémon");
        }

        [Test]
        public void Endgame_Takes_2_More_Prize_Cards_For_An_Xy_Mega()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Endgame"), new ScriptedAgent());
            var umbreon = e.ScenarioActive(0, C(UmbreonEX));
            e.ScenarioAttach(umbreon, C(Darkness));
            e.ScenarioAttach(umbreon, C(DoubleColorless));
            var mega = e.ScenarioActive(1, C(PidgeotEX));
            e.ScenarioEvolve(mega, C(MPidgeotEX));
            mega.DamageCounters = 15;
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Prizes.Count, "2 for the Pokémon-EX and 2 more");
        }

        [Test]
        public void Toss_And_Turn_Works_While_Asleep_For_90_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Toss and Turn"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(snorlax, C(Psychic));
            snorlax.Conditions = SpecialCondition.Asleep;
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.StartsWith("Attack: Toss and Turn")));
            Assert.IsFalse(labels.Any(l => l.StartsWith("Attack: Swallow")), "the other attack still can't be used");
            Step(e);
            Assert.AreEqual(12, lapras.DamageCounters);
        }

        [Test]
        public void Cacophony_Discards_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cacophony"), new ScriptedAgent());
            var exploud = e.ScenarioActive(0, C(Whismur));
            e.ScenarioEvolve(exploud, C(Loudred));
            e.ScenarioEvolve(exploud, C(Exploud));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(exploud, C(Psychic));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.AreEqual("Lapras-GX", e.State.Players[1].Active.Name, "still there during the opponent's turn");
            e.EndTurn();
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Lapras-GX"));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "discarding isn't a Knock Out");
        }

        [Test]
        public void Power_Memory_Gives_Zygarde_EX_All_Cells_Burn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: All Cells Burn"), new ScriptedAgent());
            var zygarde = e.ScenarioActive(0, C(ZygardeEX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(zygarde, C(Fighting));
            e.ScenarioAttachTool(zygarde, C(PowerMemory));
            var haw = e.ScenarioBench(0, C(Hawlucha));
            e.ScenarioAttachTool(haw, C(PowerMemory));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, zygarde.Energy.Count, "3 Energy discarded");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Snorlax-GX"), "200 x2 Knocks Out Snorlax-GX");
            Assert.IsEmpty(e.AttacksOf(haw).Where(a => a.Source.Name == "Power Memory").ToList(), "only Zygarde-EX can use it");
        }

        [Test]
        public void Bone_Drop_Only_Hits_Pokemon_With_Abilities()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bone Drop"), new ScriptedAgent());
            var mandibuzz = e.ScenarioActive(0, C(Vullaby));
            e.ScenarioEvolve(mandibuzz, C(Mandibuzz));
            e.ScenarioAttach(mandibuzz, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var regi = e.ScenarioBench(1, C(RegirockEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, lapras.DamageCounters);
            Assert.AreEqual(6, regi.DamageCounters, "Regirock-EX has Regi Power");
        }

        [Test]
        public void Plasmagic_Moves_2_Counters_From_Each_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Plasmagic"), new ScriptedAgent());
            var rotom = e.ScenarioActive(0, C(Rotom));
            e.ScenarioAttach(rotom, C(Psychic));
            e.ScenarioAttach(rotom, C(Psychic));
            rotom.DamageCounters = 3;
            var benched = e.ScenarioBench(0, C(Bounsweet));
            benched.DamageCounters = 1;
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, rotom.DamageCounters);
            Assert.AreEqual(0, benched.DamageCounters);
            Assert.AreEqual(3, lapras.DamageCounters);
        }

        [Test]
        public void Diamond_Gift_Attaches_2_Energy_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Diamond Gift"), new ScriptedAgent());
            var carbink = e.ScenarioActive(0, C(CarbinkKeeper));
            e.ScenarioEvolve(carbink, C(CarbinkBreak));
            e.ScenarioAttach(carbink, C(Fighting));
            e.ScenarioAddToDiscard(0, C(Fighting), 2);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, carbink.Energy.Count);
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Chaos_Tower_Faces_One_Way_And_Cures_What_It_Prevents()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Chaos Tower", "Your Pokémon can't be Confused"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(ChaosTower));
            var theirs = e.ScenarioActive(1, C(SnorlaxGX));
            mine.Conditions = SpecialCondition.Confused;
            theirs.Conditions = SpecialCondition.Asleep;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(mine.Has(SpecialCondition.Confused), "removed");
            Assert.IsFalse(theirs.Has(SpecialCondition.Asleep), "removed");
            Assert.IsFalse(e.ApplyCondition(mine, SpecialCondition.Poisoned));
            Assert.IsTrue(e.ApplyCondition(mine, SpecialCondition.Asleep));
            Assert.IsFalse(e.ApplyCondition(theirs, SpecialCondition.Paralyzed));
            Assert.IsTrue(e.ApplyCondition(theirs, SpecialCondition.Confused));
        }

        [Test]
        public void Bent_Spoon_Stops_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psybeam"), new ScriptedAgent());
            var gothita = e.ScenarioActive(0, C(Gothita));
            e.ScenarioAttach(gothita, C(Psychic));
            e.ScenarioAttach(gothita, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C(BentSpoon));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, lapras.DamageCounters);
            Assert.IsFalse(lapras.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Fairy_Garden_Removes_The_Retreat_Cost_Of_Pokemon_With_Fairy_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAttach(snorlax, C(Fairy));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetStadium(1, C(FairyGarden));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(snorlax));
            Assert.AreEqual(2, e.RetreatCostOf(lapras));
        }

        [Test]
        public void Energy_Pouch_Returns_Basic_Energy_When_Knocked_Out_By_An_Attack()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var holder = e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAttach(holder, C(Grass));
            e.ScenarioAttach(holder, C(DoubleColorless));
            e.ScenarioAttachTool(holder, C(EnergyPouch));
            e.ScenarioBench(0, C(Bounsweet));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, holder, 200);
            e.ResolveKnockouts();
            var me = e.State.Players[0];
            Assert.AreEqual(1, me.Hand.Count(c => c.Def.Name == "Grass Energy"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Double Colorless Energy"));
        }

        [Test]
        public void Random_Receiver_Finds_The_First_Supporter()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Random Receiver"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(RandomReceiver));
            e.ScenarioAddToDeck(0, C(Water), 2);
            e.ScenarioAddToDeck(0, C(FanClub), 1);
            e.ScenarioAddToDeck(0, C(Water), 2);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Pokémon Fan Club", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual(24, e.State.Players[0].Deck.Count, "20 + 5, less the Supporter");
        }

        [Test]
        public void Lasses_Special_Draws_For_Each_Benched_Basic_Of_The_Opponent()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lass's Special"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(LassesSpecial));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioBench(1, C(Bounsweet));
            MarowakOnBench(e, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Marowak isn't a Basic Pokémon");
        }

        [Test]
        public void Mega_Catcher_Only_Pulls_Up_Xy_Era_Megas()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mega Catcher"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(MegaCatcher));
            e.ScenarioActive(1, C(LaprasGX));
            var mega = e.ScenarioBench(1, C(PidgeotEX));
            e.ScenarioEvolve(mega, C(MPidgeotEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("M Pidgeot-EX", e.State.Players[1].Active.Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Bounsweet));
            e2.ScenarioAddToHand(0, C(MegaCatcher));
            e2.ScenarioActive(1, C(LaprasGX));
            var camerupt = e2.ScenarioBench(1, C(Numel));
            e2.ScenarioEvolve(camerupt, C(MCamerupt));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label == "Play Mega Catcher"), "a Mega Evolution Pokémon ex isn't an XY-era Mega");
        }

        [Test]
        public void Scorched_Earth_Trades_A_Fighting_Energy_For_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Scorched Earth"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioSetStadium(1, C(ScorchedEarth));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Fighting Energy"));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Use Scorched Earth"), "once a turn");
        }

        [Test]
        public void Altaria_EX_Shining_Wind_Removes_Weakness_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shining Wind"), new ScriptedAgent());
            var altaria = e.ScenarioActive(0, C(AltariaEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(altaria, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            Assert.IsNull(e.WeaknessOf(altaria));
        }
    }
}
