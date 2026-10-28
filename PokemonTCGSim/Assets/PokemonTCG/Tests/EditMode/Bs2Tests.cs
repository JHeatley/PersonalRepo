using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v119: Base Set 2 (BS2 1-130) is all reprints of Base Set and Jungle cards. Every card is a printing of the card it reprints
    /// (same name, HP, Weakness, Resistance, Retreat Cost, Pokémon Power, attacks), and the old Trainers keep their old text.
    /// </summary>
    public class Bs2Tests : ScenarioTestBase
    {
        private List<CardDef> Bs2Prints()
        {
            return db.All.Where(d => d.SetCode == "BS2").OrderBy(d => int.Parse(d.Number)).ToList();
        }

        [Test]
        public void All_130_Cards_Are_Printings_Of_Base_Set_Or_Jungle_Cards()
        {
            var prints = Bs2Prints();
            Assert.AreEqual(130, prints.Count);
            Assert.AreEqual(100, prints.Count(d => d.IsPokemon));
            Assert.AreEqual(23, prints.Count(d => d.IsTrainer));
            Assert.AreEqual(7, prints.Count(d => d.IsEnergy));
            foreach (var p in prints)
            {
                Assert.IsTrue(p.IsAlternatePrint, p.Key + " is a reprint, not a card of its own");
                Assert.AreEqual("Base Set 2", p.PrintSetName, p.Key);
                Assert.IsTrue(db.PrintingsOf(p).Any(o => o.SetCode == "BS" || o.SetCode == "JU"), p.Key + " " + p.Name + " reprints a Base Set or Jungle card");
            }
            Assert.AreEqual(0, db.Cards.Count(d => d.SetCode == "BS2"), "no new cards");
        }

        [Test]
        public void The_Pokemon_Match_The_Cards_They_Reprint()
        {
            Assert.AreEqual(C("BS-1").BaseKey, C("BS2-1").BaseKey, "Alakazam");
            Assert.AreEqual(C("BS-2").BaseKey, C("BS2-2").BaseKey, "Blastoise");
            Assert.AreEqual(C("BS-4").BaseKey, C("BS2-4").BaseKey, "Charizard");
            Assert.AreEqual("JU-1", C("BS2-5").BaseKey, "Clefable");
            Assert.AreEqual("JU-11", C("BS2-30").BaseKey, "Snorlax");
            Assert.AreEqual("JU-13", C("BS2-31").BaseKey, "Venomoth");
            Assert.AreEqual("BS-9", C("BS2-9").BaseKey, "Magneton");
            Assert.AreEqual(C("BS-58").BaseKey, C("BS2-87").BaseKey, "Pikachu");
            var mime = C("BS2-27");
            Assert.AreEqual("Mr. Mime", mime.Name);
            Assert.AreEqual("Invisible Wall", mime.Abilities[0].Name);
            Assert.AreEqual(40, mime.Hp);
        }

        [Test]
        public void The_Trainers_Keep_Their_Base_Set_Text()
        {
            Assert.AreEqual("BS-81", C("BS2-110").BaseKey, "Energy Retrieval: the old one (trade a card for 2 basic Energy)");
            Assert.AreEqual("BS-90", C("BS2-117").BaseKey, "Super Potion: the old one");
            Assert.AreEqual("BS-94", C("BS2-122").BaseKey, "Potion: the old one");
            Assert.AreEqual("BS-81", C("BS-81").BaseKey);
            Assert.AreEqual(C("JU-64").BaseKey, C("BS2-121").BaseKey, "Poké Ball: Jungle's");
            Assert.AreEqual("BS-71", C("BS2-101").BaseKey, "Computer Search");
            Assert.IsFalse(C("BS2-101").IsAceSpec, "your ruling (v113): Base Set Computer Search is a normal Item");
            Assert.AreEqual(C("BS-73").BaseKey, C("BS2-102").BaseKey, "Imposter Professor Oak");
            Assert.AreEqual(C("BS-85").BaseKey, C("BS2-114").BaseKey, "Pokémon Center");
            Assert.AreEqual(C("BS-96").BaseKey, C("BS2-124").BaseKey, "Double Colorless Energy");
            Assert.AreEqual("MEE-11", C("BS2-130").BaseKey, "Water Energy");
        }

        [Test]
        public void Decks_Of_Base_Set_2_Cards_Play_Without_Errors()
        {
            var prints = Bs2Prints().Where(d => !d.IsEnergy || !d.IsBasicEnergy).ToList();
            var energy = Bs2Prints().Where(d => d.IsEnergy && d.IsBasicEnergy).ToList();
            const int count = 4;
            int per = (prints.Count + count - 1) / count;
            var decks = new List<List<CardDef>>();
            for (int k = 0; k < count; k++)
            {
                var deck = prints.Skip(k * per).Take(per).ToList();
                for (int i = 0; deck.Count < 60; i++) deck.Add(energy[(i + k) % energy.Count]);
                Assert.IsTrue(deck.Any(d => d.IsBasicPokemon), "deck " + k + " has a Basic Pokémon");
                decks.Add(deck);
            }
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
                for (int j = 0; j < decks.Count; j++)
                {
                    if (i == j) continue;
                    var report = MatchRunner.Run(scripts, decks[i], decks[j], 3, 11900 + i * 100 + j * 10, (seat, s) => new RandomAgent(s), true);
                    foreach (var err in report.Errors) errors.Add("BS2 deck " + i + " vs " + j + ": " + err);
                    report = MatchRunner.Run(scripts, decks[i], decks[j], 1, 12900 + i * 100 + j * 10, (seat, s) => new HeuristicAgent(s), true);
                    foreach (var err in report.Errors) errors.Add("BS2 deck " + i + " vs " + j + " (heuristic): " + err);
                }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }
    }
}
