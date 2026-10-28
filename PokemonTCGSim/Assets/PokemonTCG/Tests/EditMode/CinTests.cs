using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v86: Crimson Invasion (CIN 1-124).</summary>
    public class CinTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Charmander = "TEU-12";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string Gogoat = "CIN-11";
        const string Starmie = "CIN-16";
        const string Swinub = "CIN-19";
        const string Piloswine = "CIN-20";
        const string Mamoswine = "CIN-21";
        const string Regice = "CIN-28";
        const string AlolanGeodude = "CIN-32";
        const string AlolanGolem = "CIN-34";
        const string Gengar = "CIN-38";
        const string Mismagius = "CIN-40";
        const string Grumpig = "CIN-42";
        const string Chimecho = "CIN-43";
        const string Regirock = "CIN-53";
        const string Bewear = "CIN-56";
        const string Houndour = "CIN-58";
        const string Registeel = "CIN-68";
        const string Miltank = "CIN-78";
        const string Regigigas = "CIN-84";
        const string DashingPouch = "CIN-92";
        const string DevouredField = "CIN-93";
        const string Gladion = "CIN-95";
        const string SeaOfNothingness = "CIN-99";
        const string CounterEnergy = "CIN-100";
        const string Olivia = "CIN-111";
        const string WishfulBaton = "CIN-121";
        const string WarpEnergy = "CIN-123";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "CIN" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(93, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Crimson Invasion", C(Gogoat).PrintSetName);
            Assert.AreEqual("HIF-SV62", C("CIN-49").BaseKey, "Nihilego-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("DRM-33", C("CIN-62").BaseKey, "Hydreigon is Dragon Majesty 33's card");
            Assert.AreEqual("PAR-160", C("CIN-91").BaseKey, "Counter Catcher was already in the pool");
            Assert.AreEqual("UPR-153", C("CIN-96").BaseKey, "Lusamine came with Ultra Prism's secret rare");
            Assert.AreEqual("MEE-11", C("CIN-124").BaseKey, "the gold Water Energy is the basic Water Energy");
            Assert.AreEqual("HIF-SV71", C("CIN-63a").BaseKey);
            Assert.AreEqual("CIN-84", C("CIN-84a").BaseKey);
            Assert.AreEqual("CIN-18", C("CIN-112").BaseKey, "rainbow Gyarados-GX");
            Assert.AreEqual(0, C("CIN-6").Attacks[1].Damage, "Hunt's 40 is only in the text");
            Assert.AreEqual(0, C("CIN-74").Attacks[0].Damage, "so is Tropical Head's damage");
            Assert.AreEqual(0, C("CIN-12").Attacks[0].Cost.Length, "Dance of Flames costs nothing");
            Assert.AreEqual(Stage.Stage2, C(Mamoswine).Stage);
        }

        [Test]
        public void Sea_Of_Nothingness_Keeps_Special_Conditions_When_A_Pokemon_Evolves()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var swinub = e.ScenarioActive(0, C(Swinub));
                e.ScenarioActive(1, C(Snubbull));
                if (pass == 1) e.ScenarioSetStadium(1, C(SeaOfNothingness));
                e.ScenarioSetTurn(0, 3);
                e.ApplyCondition(swinub, SpecialCondition.Burned);
                e.ApplyCondition(swinub, SpecialCondition.Asleep);
                var card = e.ScenarioAddToHand(0, C(Piloswine));
                e.State.Players[0].Hand.Remove(card);
                e.EvolvePokemon(swinub, card);
                Assert.AreEqual("Piloswine", swinub.Name);
                if (pass == 0) Assert.AreEqual(SpecialCondition.None, swinub.Conditions, "evolving removes them");
                else
                {
                    Assert.IsTrue(swinub.Has(SpecialCondition.Burned), "Sea of Nothingness keeps them");
                    Assert.IsTrue(swinub.Has(SpecialCondition.Asleep));
                }
            }
        }

        [Test]
        public void Dashing_Pouch_Returns_Every_Energy_Discarded_For_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat Alolan Geodude"), new ScriptedAgent());
            var geodude = e.ScenarioActive(0, C(AlolanGeodude));
            e.ScenarioAttach(geodude, C(CounterEnergy));
            e.ScenarioAttach(geodude, C(Psychic));
            e.ScenarioAttachTool(geodude, C(DashingPouch));
            e.ScenarioBench(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual("Snubbull", me.Active.Name);
            Assert.AreEqual(2, me.Hand.Count(c => c.Def.IsEnergy), "the Special Energy too, not only the basic Energy");
            Assert.AreEqual(0, me.Discard.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Counter_Energy_Gives_2_Of_Any_Type_Only_When_Behind_And_Not_On_A_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var geodude = e.ScenarioActive(0, C(AlolanGeodude));
            e.ScenarioAttach(geodude, C(CounterEnergy));
            var golem = e.ScenarioBench(0, C(AlolanGolem));
            e.ScenarioAttach(golem, C(CounterEnergy));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.EnergyCount(geodude), "same number of Prize cards left");
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.AreEqual(2, e.EnergyCount(geodude), "6 left against 5");
            Assert.IsTrue(e.HasEnergyOfType(geodude, EnergyType.Fire));
            Assert.AreEqual(1, e.EnergyCount(golem), "not on a Pokémon-GX");
        }

        [Test]
        public void Warp_Energy_Switches_The_Active_Pokemon_It_Is_Attached_To()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Warp Energy to Snubbull"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Snubbull));
            var benched = e.ScenarioBench(0, C(Mewtwo));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(WarpEnergy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(benched, e.State.Players[0].Active);
            Assert.AreEqual(1, active.Energy.Count, "the Warp Energy stays on the Pokémon that went to the Bench");
        }

        [Test]
        public void Gladion_Trades_A_Prize_Card_For_Itself()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Gladion"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Gladion));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(6, me.Prizes.Count, "still 6 Prize cards");
            Assert.IsTrue(me.Prizes.Any(c => c.Def.Name == "Gladion"));
            Assert.AreEqual(1, me.Hand.Count, "the Prize card");
            Assert.AreEqual(0, me.Discard.Count(c => c.Def.Name == "Gladion"));
            Assert.AreEqual(0, me.FaceUpPrizeIds.Count);
        }

        [Test]
        public void Heavy_Rock_GX_Stops_The_Opponent_Playing_Any_Card_From_Their_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Rock-GX"), new ScriptedAgent());
            var golem = e.ScenarioActive(0, C(AlolanGolem));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(golem, C(Lightning));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Olivia));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var actions = e.GetMainActions(1);
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Bench") || o.Label.StartsWith("Attach") || o.Label.StartsWith("Play")));
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Label.StartsWith("Bench Snubbull")), "only for 1 turn");
        }

        [Test]
        public void Chaos_Wheel_Stops_Tools_Stadiums_And_Special_Energy_But_Not_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chaos Wheel"), new ScriptedAgent());
            var mismagius = e.ScenarioActive(0, C(Mismagius));
            e.ScenarioAttach(mismagius, C(Psychic));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAddToHand(1, C(DashingPouch));
            e.ScenarioAddToHand(1, C(DevouredField));
            e.ScenarioAddToHand(1, C(CounterEnergy));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Olivia));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var labels = e.GetMainActions(1).Select(o => o.Label).ToList();
            Assert.IsFalse(labels.Any(l => l.Contains("Dashing Pouch") || l.Contains("Devoured Field") || l.Contains("Counter Energy")));
            Assert.IsTrue(labels.Any(l => l.StartsWith("Attach Psychic Energy")));
            Assert.IsTrue(labels.Contains("Play Olivia"), "a Supporter is fine");
        }

        [Test]
        public void Bell_Of_Silence_Stops_Pokemon_With_An_Ability()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bell of Silence"), new ScriptedAgent());
            var chimecho = e.ScenarioActive(0, C(Chimecho));
            e.ScenarioAttach(chimecho, C(Psychic));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAddToHand(1, C(Miltank));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var labels = e.GetMainActions(1).Select(o => o.Label).ToList();
            Assert.IsFalse(labels.Contains("Bench Miltank"), "Miltank has Moomoo Malt");
            Assert.IsTrue(labels.Contains("Bench Snubbull"));
        }

        [Test]
        public void Regigigas_Can_Only_Attack_With_The_Three_Regis_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var regigigas = e.ScenarioActive(0, C(Regigigas));
            for (int i = 0; i < 5; i++) e.ScenarioAttach(regigigas, C(Psychic));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioBench(0, C(Regirock));
            e.ScenarioBench(0, C(Regice));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Giant Stomp")), "no Registeel");
            e.ScenarioBench(0, C(Registeel));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Giant Stomp")));
        }

        [Test]
        public void Iceberg_Shield_Stops_Stage_2_Attackers_With_Regirock_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C(Mamoswine));
            var basic = e.ScenarioBench(0, C(Snubbull));
            var regice = e.ScenarioActive(1, C(Regice));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mamoswine, regice, 60);
            Assert.AreEqual(6, regice.DamageCounters, "no Regirock yet");
            e.ScenarioBench(1, C(Regirock));
            e.DealAttackDamage(mamoswine, regice, 60);
            Assert.AreEqual(6, regice.DamageCounters, "Mamoswine is a Stage 2 Pokémon");
            e.DealAttackDamage(basic, regice, 30);
            Assert.AreEqual(9, regice.DamageCounters, "a Basic Pokémon's damage isn't stopped");
        }

        [Test]
        public void Sap_Sipper_Adds_80_Against_Grass_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gogoat = e.ScenarioActive(0, C(Gogoat));
            var turtwig = e.ScenarioActive(1, C(Turtwig));
            var other = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(gogoat, turtwig, 80);
            Assert.AreEqual(16, turtwig.DamageCounters, "80 + 80 (Grass has no Weakness to Grass)");
            e.DealAttackDamage(gogoat, other, 20);
            Assert.AreEqual(2, other.DamageCounters, "Snubbull isn't a Grass Pokémon");
        }

        [Test]
        public void Fluffy_Takes_30_Off_Attacks_From_Non_Fire_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            var charmander = e.ScenarioBench(0, C(Charmander));
            var bewear = e.ScenarioActive(1, C(Bewear));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mewtwo, bewear, 50);
            Assert.AreEqual(7, bewear.DamageCounters, "50 x 2 for Weakness to Psychic, then 30 less");
            e.DealAttackDamage(charmander, bewear, 50);
            Assert.AreEqual(12, bewear.DamageCounters, "a Fire Pokémon's damage isn't reduced");
        }

        [Test]
        public void Moomoo_Malt_Heals_90_When_You_Attach_Energy_From_Your_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Snubbull"), new ScriptedAgent());
            e.ScenarioActive(0, C(Miltank));
            var hurt = e.ScenarioBench(0, C(Snubbull));
            hurt.DamageCounters = 5;
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, hurt.DamageCounters);
        }

        [Test]
        public void Gnawing_Curse_Comes_Before_Moomoo_Malt_So_The_Healing_Counts()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Snubbull"), new ScriptedAgent());
            e.ScenarioActive(0, C(Miltank));
            var hurt = e.ScenarioBench(0, C(Snubbull));
            hurt.DamageCounters = 5;
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioBench(1, C(Gengar));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, hurt.DamageCounters, "5 + 2 from Gnawing Curse, then 90 healed (Compendium, 2017-11-02)");
        }

        [Test]
        public void Wishful_Baton_Moves_3_Basic_Energy_To_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Psychic Energy|Psychic Energy|Psychic Energy"));
            var attacker = e.ScenarioActive(0, C(Celesteela));
            var holder = e.ScenarioActive(1, C(Snubbull));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(holder, C(Psychic));
            e.ScenarioAttachTool(holder, C(WishfulBaton));
            var benched = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, holder, 200);
            e.ResolveKnockouts();
            Assert.IsNull(e.State.FindPokemon(holder.Uid));
            Assert.AreEqual(3, benched.Energy.Count);
        }

        [Test]
        public void Escape_Shuffles_Starmie_And_Its_Cards_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Escape"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var starmie = e.ScenarioBench(0, C(Starmie));
            e.ScenarioAttach(starmie, C(Water));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.IsNull(e.State.FindPokemon(starmie.Uid));
            Assert.AreEqual(deck + 2, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Gnawing_Curse_Puts_2_Counters_On_Each_Energy_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy to Snubbull"));
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioBench(0, C(Gengar));
            var target = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, target.DamageCounters);
        }

        [Test]
        public void Psych_Up_Does_60_More_On_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psych Up").Then("Attack: Psych Up"), new ScriptedAgent());
            var grumpig = e.ScenarioActive(0, C(Grumpig));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(grumpig, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, target.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(18, target.DamageCounters, "60 + 120");
        }

        [Test]
        public void Devoured_Field_Adds_10_To_Darkness_And_Dragon_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var houndour = e.ScenarioActive(0, C(Houndour));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(houndour, target, 10);
            Assert.AreEqual(1, target.DamageCounters);
            e.ScenarioSetStadium(1, C(DevouredField));
            e.DealAttackDamage(houndour, target, 10);
            Assert.AreEqual(3, target.DamageCounters, "10 + 10");
        }
    }
}
