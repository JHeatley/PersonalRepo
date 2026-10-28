using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v87: Shining Legends (SLG 1-78).</summary>
    public class SlgTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string Bulbasaur = "SLG-1";
        const string Ivysaur = "SLG-2";
        const string Venusaur = "SLG-3";
        const string Breloom = "SLG-5";
        const string Carnivine = "SLG-6";
        const string Shaymin = "SLG-7";
        const string Virizion = "SLG-8";
        const string Genesect = "SLG-9";
        const string Torkoal = "SLG-11";
        const string Volcarona = "SLG-13";
        const string Croconaw = "SLG-19";
        const string Manaphy = "SLG-25";
        const string Keldeo = "SLG-26";
        const string Pikachu = "SLG-28";
        const string RaichuGX = "SLG-29";
        const string Plusle = "SLG-33";
        const string Minun = "SLG-34";
        const string Arbok = "SLG-37";
        const string Mew = "SLG-40";
        const string Jirachi = "SLG-42";
        const string Marshadow = "SLG-45";
        const string Spiritomb = "SLG-47";
        const string ZoroarkGX = "SLG-53";
        const string Hoopa = "SLG-55";
        const string Arceus = "SLG-57";
        const string DamageMover = "SLG-58";
        const string Breeder = "SLG-63";
        const string Sophocles = "SLG-65";
        const string DoubleColorless = "SLG-69";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "SLG" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(59, own.Count);
            // Cards banned in Expanded are Unlimited only, and basic Energy is Standard as always.
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == (Formats.IsExpandedBanned(d) ? CardCategory.Unlimited
                : d.IsBasicEnergy ? CardCategory.Standard : CardCategory.Expanded)), "no regulation mark: Expanded");
            Assert.AreEqual("Shining Legends", C(Venusaur).PrintSetName);
            Assert.AreEqual("HIF-SV59", C("SLG-39").BaseKey, "Mewtwo-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("UPR-125", C("SLG-62").BaseKey, "Lillie was already in the pool");
            Assert.AreEqual("CIN-123", C("SLG-70").BaseKey, "so was Warp Energy");
            Assert.AreEqual("SLG-10", C("SLG-71").BaseKey, "full art Entei-GX");
            Assert.AreEqual("SLG-53", C("SLG-77a").BaseKey);
            Assert.AreEqual(0, C("SLG-10").Attacks[2].Damage, "Brave Burn-GX's 150 is only in the text");
            Assert.AreEqual(0, C("SLG-57").Attacks[0].Damage, "so is Ultimate Arrow's 30");
            Assert.AreEqual(EnergyType.Dragon, C("SLG-56").PokemonType);
            Assert.AreEqual(Stage.Stage2, C(Venusaur).Stage);
        }

        [Test]
        public void Jungle_Totem_Doubles_Basic_Grass_Energy_On_Your_Own_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Venusaur));
            var turtwig = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(turtwig, C(Grass));
            e.ScenarioAttach(turtwig, C(Fire));
            e.ScenarioActive(1, C(Snubbull));
            var theirs = e.ScenarioBench(1, C(Turtwig));
            e.ScenarioAttach(theirs, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.EnergyCount(turtwig), "Grass Grass from the Grass Energy, and the Fire Energy");
            Assert.AreEqual(1, e.EnergyCount(theirs), "only your own Pokémon");
            e.ScenarioBench(0, C(Venusaur));
            Assert.AreEqual(3, e.EnergyCount(turtwig), "a second Jungle Totem doesn't add more");
        }

        [Test]
        public void Hibernation_Spore_Makes_The_Opponent_Flip_2_Coins_To_Wake_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hibernation Spore"), new ScriptedAgent());
            var breloom = e.ScenarioActive(0, C(Breloom));
            e.ScenarioAttach(breloom, C(Grass));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Asleep));
            Assert.AreEqual(2, target.SleepCoins);
        }

        [Test]
        public void Flick_Poison_Brings_Up_A_Benched_Pokemon_And_Poisons_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flick Poison"), new ScriptedAgent());
            var carnivine = e.ScenarioActive(0, C(Carnivine));
            e.ScenarioAttach(carnivine, C(Grass));
            var old = e.ScenarioActive(1, C(Snubbull));
            var benched = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(benched, e.State.Players[1].Active);
            Assert.IsTrue(benched.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(old.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Rally_Back_Does_90_More_After_A_Knock_Out_By_An_Attack()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Rally Back"), new ScriptedAgent());
                var shaymin = e.ScenarioActive(0, C(Shaymin));
                e.ScenarioAttach(shaymin, C(Grass));
                e.ScenarioAttach(shaymin, C(Psychic));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1) e.State.Players[0].LastKnockedOutByAttackTurn = 2;
                Step(e);
                Assert.AreEqual(pass == 0 ? 3 : 12, target.DamageCounters);
            }
        }

        [Test]
        public void Wrapped_In_Wind_Attaches_A_Basic_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wrapped in Wind"), new ScriptedAgent());
            var virizion = e.ScenarioActive(0, C(Virizion));
            e.ScenarioAttach(virizion, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            Assert.AreEqual(2, virizion.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Energy_Reload_Moves_Only_Grass_Energy_To_Shining_Genesect()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Reload"), new ScriptedAgent());
            var genesect = e.ScenarioActive(0, C(Genesect));
            var turtwig = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(turtwig, C(Psychic));
            e.ScenarioAttach(turtwig, C(Grass));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, genesect.Energy.Count);
            Assert.AreEqual("Grass Energy", genesect.Energy[0].Def.Name);
            Assert.AreEqual("Psychic Energy", turtwig.Energy.Single().Def.Name);
        }

        [Test]
        public void High_Pressure_Heat_Does_50_More_On_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: High-Pressure Heat").Then("Attack: High-Pressure Heat"), new ScriptedAgent());
            var torkoal = e.ScenarioActive(0, C(Torkoal));
            e.ScenarioAttach(torkoal, C(Fire));
            e.ScenarioAttach(torkoal, C(Fire));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(11, target.DamageCounters, "30 + 80");
        }

        [Test]
        public void Heat_Cyclone_Lets_The_Opponent_Choose_Their_New_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Heat Cyclone"), new ScriptedAgent());
            e.ScenarioActive(0, C(Volcarona));
            var old = e.ScenarioActive(1, C(Snubbull));
            var benched = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(benched, e.State.Players[1].Active);
            Assert.IsTrue(e.State.Players[1].Bench.Contains(old));
        }

        [Test]
        public void Plunge_Takes_The_Energy_And_Switches_In_Even_With_No_Energy()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Use Plunge"), new ScriptedAgent());
                var active = e.ScenarioActive(0, C(Snubbull));
                if (pass == 0)
                {
                    e.ScenarioAttach(active, C(Psychic));
                    e.ScenarioAttach(active, C(Water));
                }
                var croconaw = e.ScenarioBench(0, C(Croconaw));
                e.ScenarioActive(1, C(Mewtwo));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(croconaw, e.State.Players[0].Active, "it switches in either way (your ruling)");
                Assert.AreEqual(pass == 0 ? 2 : 0, croconaw.Energy.Count);
                Assert.AreEqual(0, active.Energy.Count);
            }
        }

        [Test]
        public void Blessings_Of_The_Deep_Heals_Only_A_Pokemon_With_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Blessings of the Deep"), new ScriptedAgent());
            e.ScenarioActive(0, C(Manaphy));
            var wet = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(wet, C(Water));
            wet.DamageCounters = 3;
            var dry = e.ScenarioBench(0, C(Snubbull));
            dry.DamageCounters = 3;
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, wet.DamageCounters);
            Assert.AreEqual(3, dry.DamageCounters);
        }

        [Test]
        public void Bail_Out_Takes_A_Water_Pokemon_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bail Out"), new ScriptedAgent());
            var keldeo = e.ScenarioActive(0, C(Keldeo));
            e.ScenarioAttach(keldeo, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Croconaw), 1);
            e.ScenarioAddToDiscard(0, C(Turtwig), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.IsTrue(me.Hand.Any(c => c.Def.Name == "Croconaw"));
            Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Turtwig"), "Turtwig is a Grass Pokémon");
        }

        [Test]
        public void Powerful_Spark_Counts_Lightning_Energy_On_All_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Powerful Spark"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(RaichuGX));
            e.ScenarioAttach(raichu, C(Lightning));
            e.ScenarioAttach(raichu, C(Lightning));
            var pikachu = e.ScenarioBench(0, C(Pikachu));
            e.ScenarioAttach(pikachu, C(Lightning));
            e.ScenarioAttach(pikachu, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, target.DamageCounters, "20 + 3 x 20");
        }

        [Test]
        public void Tag_Team_Boost_Needs_Minun_On_The_Bench()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Tag Team Boost"), new ScriptedAgent());
                var plusle = e.ScenarioActive(0, C(Plusle));
                e.ScenarioAttach(plusle, C(Lightning));
                if (pass == 1) e.ScenarioBench(0, C(Minun));
                var target = e.ScenarioActive(1, C(Mewtwo));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 1 : 6, target.DamageCounters);
            }
        }

        [Test]
        public void Intimidating_Pattern_Weakens_The_Opponents_Active_Pokemon_While_Arbok_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snubbull = e.ScenarioActive(0, C(Snubbull));
            var arbok = e.ScenarioActive(1, C(Arbok));
            var benched = e.ScenarioBench(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snubbull, arbok, 50);
            Assert.AreEqual(2, arbok.DamageCounters, "50 - 30");
            e.DealAttackDamage(snubbull, benched, 50);
            Assert.AreEqual(2, benched.DamageCounters, "all of that attack's damage");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e2.ScenarioActive(0, C(Snubbull));
            var active = e2.ScenarioActive(1, C(Mewtwo));
            e2.ScenarioBench(1, C(Arbok));
            e2.ScenarioSetTurn(0, 3);
            e2.DealAttackDamage(attacker, active, 50);
            Assert.AreEqual(5, active.DamageCounters, "Arbok on the Bench does nothing");
        }

        [Test]
        public void Legendary_Guidance_Attaches_2_Energy_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Legendary Guidance", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(Mew));
            e.ScenarioAttach(mew, C(Psychic));
            var turtwig = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(3, mew.Energy.Count + turtwig.Energy.Count);
            Assert.AreEqual(deck - 2, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Stellar_Reign_Puts_The_Evolution_Cards_Into_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stellar Reign"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C(Jirachi));
            e.ScenarioAttach(jirachi, C(Psychic));
            var target = e.ScenarioActive(1, C(Bulbasaur));
            e.ScenarioEvolve(target, C(Ivysaur));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Bulbasaur", target.Name);
            Assert.AreEqual(1, target.DamageCounters);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Ivysaur"));
        }

        [Test]
        public void Let_Loose_Gives_Both_Players_A_New_Hand_Of_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Marshadow"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(Marshadow));
            e.ScenarioAddToHand(0, C(Grass));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].InPlay().Count());
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Cursed_Whirlpool_Stops_Retreating_Only_While_Spiritomb_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Spiritomb));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.RetreatPrevented(theirs));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Turtwig));
            e2.ScenarioBench(0, C(Spiritomb));
            var other = e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e2.RetreatPrevented(other));
        }

        [Test]
        public void Trade_Discards_1_And_Draws_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Trade"), new ScriptedAgent());
            e.ScenarioActive(0, C(ZoroarkGX));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count);
            Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Trickster_GX_Can_Copy_A_GX_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trickster-GX", "Voltail-GX"), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(ZoroarkGX));
            e.ScenarioAttach(zoroark, C(Darkness));
            e.ScenarioAttach(zoroark, C(Darkness));
            var raichu = e.ScenarioActive(1, C(RaichuGX));
            e.State.Players[1].GxUsed = true;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, raichu.DamageCounters);
            Assert.IsTrue(raichu.Has(SpecialCondition.Paralyzed));
            Assert.IsTrue(e.State.Players[0].GxUsed, "Trickster-GX is the GX attack (Compendium, 2017-12-07)");
        }

        [Test]
        public void Scoundrel_Guard_Stops_Pokemon_GX_And_EX_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var celesteela = e.ScenarioActive(0, C(Celesteela));
            var snubbull = e.ScenarioBench(0, C(Snubbull));
            var hoopa = e.ScenarioActive(1, C(Hoopa));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(celesteela, hoopa, 100);
            Assert.AreEqual(0, hoopa.DamageCounters);
            e.DealAttackDamage(snubbull, hoopa, 30);
            Assert.AreEqual(3, hoopa.DamageCounters);
        }

        [Test]
        public void Fabled_Defense_Protects_The_Bench_Only_While_Arceus_Is_Active()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var attacker = e.ScenarioActive(0, C(Snubbull));
                PokemonInPlay benched;
                if (pass == 0)
                {
                    e.ScenarioActive(1, C(Arceus));
                    benched = e.ScenarioBench(1, C(Turtwig));
                }
                else
                {
                    e.ScenarioActive(1, C(Turtwig));
                    e.ScenarioBench(1, C(Arceus));
                    benched = e.ScenarioBench(1, C(Mewtwo));
                }
                e.ScenarioSetTurn(0, 3);
                e.DealAttackDamage(attacker, benched, 30);
                Assert.AreEqual(pass == 0 ? 0 : 3, benched.DamageCounters);
            }
        }

        [Test]
        public void Damage_Mover_Moves_3_Counters_Or_All_Of_Fewer()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Play Damage Mover"), new ScriptedAgent());
                var hurt = e.ScenarioActive(0, C(Mewtwo));
                hurt.DamageCounters = pass == 0 ? 5 : 2;
                var healthy = e.ScenarioBench(0, C(Turtwig));
                e.ScenarioActive(1, C(Snubbull));
                e.ScenarioAddToHand(0, C(DamageMover));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 2 : 0, hurt.DamageCounters);
                Assert.AreEqual(pass == 0 ? 3 : 2, healthy.DamageCounters, "only 2 to move: both go (your ruling)");
            }
        }

        [Test]
        public void Sophocles_Needs_2_Other_Cards_To_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sophocles"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(Sophocles));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Sophocles"));
            e.ScenarioAddToHand(0, C(Fire));
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(4, me.Hand.Count);
            Assert.AreEqual(3, me.Discard.Count, "the 2 cards and Sophocles");
        }

        [Test]
        public void Pokemon_Breeder_Draws_2_And_Heals_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Breeder"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Mewtwo));
            active.DamageCounters = 3;
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Breeder));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(1, active.DamageCounters);
        }

        [Test]
        public void Double_Colorless_Energy_Provides_2_Colorless()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snubbull = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttach(snubbull, C(DoubleColorless));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.EnergyCount(snubbull));
            Assert.AreEqual(1, snubbull.Energy.Count);
        }
    }
}
