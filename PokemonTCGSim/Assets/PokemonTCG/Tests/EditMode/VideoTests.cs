using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v77: learning from video. Reading what the boxes show, names (English and Japanese), card art, the timeline of events, and studying it.</summary>
    public class VideoTests : ScenarioTestBase
    {
        // ------------------------------------------------------------------ Text the OCR reads

        [TestCase("310/310", 310, 310)]
        [TestCase("90 /110", 90, 110)]
        [TestCase("90110", 90, 110)]
        [TestCase("7O/7O", 70, 70)]
        [TestCase("８０", 80, -1)]
        public void Hp_Is_Read_In_Its_Usual_Forms(string text, int hp, int max)
        {
            int h, m;
            Assert.IsTrue(VideoText.ParseHp(text, out h, out m), text);
            Assert.AreEqual(hp, h);
            Assert.AreEqual(max, m);
        }

        [Test]
        public void Numbers_Clocks_And_Empty_Places_Are_Read()
        {
            Assert.AreEqual(5, VideoText.ParseInt("5 TURN"));
            Assert.AreEqual(5, VideoText.ParseInt("TURN 5"), "the letters of a word aren't read as digits");
            Assert.AreEqual(-1, VideoText.ParseInt("TURN"));
            Assert.AreEqual(19 * 60 + 21, VideoText.ParseClock("19:21"));
            Assert.AreEqual(9 * 60 + 24, VideoText.ParseClock("Main 09:24"));
            Assert.IsTrue(VideoText.IsEmptySlot("E M P T Y"));
            Assert.IsTrue(VideoText.IsEmptySlot(""));
            Assert.IsFalse(VideoText.IsEmptySlot("ドロンチ"));
            Assert.AreEqual("活力の森", VideoText.AfterLabel("スタジアム:活力の森"));
            Assert.AreEqual("EX", VideoText.Normalize("ＥＸ"));
            Assert.AreEqual(VideoText.KatakanaKey("ドロンチ"), VideoText.KatakanaKey("ど口んち"), "hiragana and look-alike kanji read as katakana");
            Assert.IsFalse(VideoText.ParseHp("ENERGY", out int _, out int _));
        }

        // ------------------------------------------------------------------ Names

        private CardNameReader Names()
        {
            var names = CardNameReader.ForDatabase(db);
            names.AddSpecies("ドラパルト", "Dragapult");
            names.AddSpecies("ドロンチ", "Drakloak");
            names.AddSpecies("ドラメシヤ", "Dreepy");
            names.AddSpecies("ルカリオ", "Lucario");
            names.AddSpecies("オーガポン", "Ogerpon");
            names.AddSpecies("ミュウツー", "Mewtwo");
            names.AddSpecies("フェザンディピティ", "Fezandipiti");
            return names;
        }

        [TestCase("Makuhita", "Makuhita")]
        [TestCase("Dragapult ex 320", "Dragapult ex")]
        [TestCase("Teal Mask Ogerpn ex", "Teal Mask Ogerpon ex")]
        [TestCase("ドラパルトex", "Dragapult ex")]
        [TestCase("ド口ンチ", "Drakloak")]
        [TestCase("ドラパルトＥＸ", "Dragapult ex")]
        [TestCase("メガルカリオex", "Mega Lucario ex")]
        [TestCase("オーガポン みどりのめんex", "Teal Mask Ogerpon ex")]
        [TestCase("ロケット団のミュウツーex", "Team Rocket's Mewtwo ex")]
        [TestCase("活力の森", "Forest of Vitality")]
        public void Names_Are_Matched_To_Cards(string shown, string card)
        {
            var match = Names().Read(shown);
            Assert.AreEqual(card, match.Name, shown + " was read as " + match.English);
        }

        [Test]
        public void Unknown_Names_Stay_Unknown_And_A_Fix_Is_Remembered()
        {
            var names = Names();
            Assert.IsNull(names.Read("キチキギスex").Name, "a species the table doesn't have");
            Assert.IsNull(names.Read("EMPTY").Name);
            names.AddName("キチキギスex", "Fezandipiti ex");
            Assert.AreEqual("Fezandipiti ex", names.Read("キチキギスex").Name);
        }

        [Test]
        public void PokeApi_Species_Names_Are_Read_Whatever_Its_Language_Numbers()
        {
            const string csv = "pokemon_species_id,local_language_id,name,genus\n"
                               + "1,1,フシギダネ,たねポケモン\n1,9,Bulbasaur,\"Seed Pokémon, the first\"\n1,11,フシギダネ,たねポケモン\n"
                               + "887,1,ドラパルト,ステルスポケモン\n887,9,Dragapult,Stealth Pokémon\n"
                               + "83,1,カモネギ,かるがもポケモン\n83,9,Farfetch’d,Wild Duck Pokémon\n";
            var names = new CardNameReader(new[] { "Dragapult ex", "Farfetch'd", "Bulbasaur" });
            Assert.AreEqual(4, names.LoadPokeApiSpecies(csv), "Bulbasaur twice (both Japanese spellings), Dragapult, Farfetch'd");
            Assert.AreEqual("Dragapult ex", names.Read("ドラパルトex").Name);
            Assert.AreEqual("Farfetch'd", names.Read("カモネギ").Name);
        }

        // ------------------------------------------------------------------ Layouts, pictures and the OCR's answers

        [Test]
        public void A_Layout_Is_Saved_And_Read_Back()
        {
            var cl = VideoLayout.ChampionsLeague();
            Assert.AreEqual(5, cl.BenchSlots(0));
            Assert.AreEqual(5, cl.BenchSlots(1));
            Assert.AreEqual("ja", cl.Language);
            var back = VideoLayout.Parse(cl.Serialize());
            Assert.AreEqual(cl.Name, back.Name);
            Assert.AreEqual(cl.Boxes.Count, back.Boxes.Count);
            var prizes = back.Find(VideoRoles.Prizes(1));
            Assert.AreEqual(VideoBoxKind.Icons, prizes.Kind);
            Assert.AreEqual(6, prizes.Slots);
            Assert.AreEqual(cl.Find(VideoRoles.ActiveHp(0)).X, back.Find(VideoRoles.ActiveHp(0)).X, 1e-4);
            var live = VideoLayout.TcgLive();
            Assert.AreEqual(VideoBoxKind.CardArt, live.Find(VideoRoles.ActiveArt(1)).Kind);
            Assert.AreEqual("YOUR TURN", live.TurnText[0]);
            StringAssert.Contains("Bench 2 HP", VideoRoles.Describe(VideoRoles.BenchHp(1, 2), null));
        }

        [Test]
        public void Prize_Card_Icons_Are_Counted()
        {
            var row = new VideoImage(300, 50);
            row.Fill(0, 0, 300, 50, 200, 200, 200);
            for (int s = 0; s < 6; s++)
            {
                if (s == 0) row.Fill(s * 50 + 5, 5, 40, 40, 120, 120, 125); // a taken Prize card: grey
                else row.Fill(s * 50 + 5, 5, 40, 40, 220, 30, 40);
            }
            Assert.AreEqual(5, row.CountFilledIcons(6));
            var none = new VideoImage(300, 50);
            none.Fill(0, 0, 300, 50, 90, 90, 90);
            Assert.AreEqual(0, none.CountFilledIcons(6));
        }

        [Test]
        public void Pictures_Are_Prepared_For_The_Ocr_And_Written_As_Bmp()
        {
            var dark = new VideoImage(60, 20);
            dark.Fill(0, 0, 60, 20, 30, 30, 30);
            dark.Fill(20, 5, 20, 10, 250, 250, 250);
            var ready = VideoOcr.Prepare(dark);
            Assert.GreaterOrEqual(ready.Height, 64);
            Assert.Greater(ready.Luma(ready.Width - 20, ready.Height / 2), 200, "the dark background becomes light");
            Assert.Less(ready.Luma(ready.Width / 2, ready.Height / 2), 60, "the light text becomes dark");
            var bmp = ready.ToBmp();
            Assert.AreEqual((byte)'B', bmp[0]);
            Assert.AreEqual(54 + ((ready.Width * 3 + 3) / 4 * 4) * ready.Height, bmp.Length);
            Assert.Less(VideoImage.SignatureDifference(dark.Signature(), dark.Signature()), 0.001);
        }

        [Test]
        public void The_Ocr_Scripts_Answers_Are_Read()
        {
            string error;
            var results = VideoOcr.ParseResults("C:\\t\\000001.bmp\tス ピ ア ー ex\nC:\\t\\000002.bmp\t\nC:\\t\\000003.bmp\t#failed nope\n", out error);
            Assert.IsNull(error);
            Assert.AreEqual("ス ピ ア ー ex", results["C:\\t\\000001.bmp"]);
            Assert.AreEqual("", results["C:\\t\\000002.bmp"], "nothing read");
            Assert.IsNull(results["C:\\t\\000003.bmp"], "couldn't be read at all");
            VideoOcr.ParseResults("#error\tlanguage\ten-US\n", out error);
            StringAssert.Contains("en-US", error);
            StringAssert.Contains("Windows.Media.Ocr", VideoOcr.Script);
        }

        // ------------------------------------------------------------------ Card art

        private static VideoImage FakeCard(int seed)
        {
            var rng = new Random(seed);
            var card = new VideoImage(150, 210);
            card.Fill(0, 0, 150, 210, 230, 220, 120);
            for (int i = 0; i < 14; i++)
                card.Fill(rng.Next(10, 130), rng.Next(20, 95), rng.Next(10, 40), rng.Next(8, 30), rng.Next(256), rng.Next(256), rng.Next(256));
            return card;
        }

        [Test]
        public void Cards_Are_Recognised_By_Their_Art_Even_A_Little_Off()
        {
            var index = new CardArtIndex();
            for (int i = 0; i < 20; i++) index.Add("K-" + i, "Card " + i, CardArtIndex.Fingerprint(FakeCard(i)));
            // Card 7 on a "table", the box drawn a few pixels off.
            var table = new VideoImage(400, 300);
            table.Fill(0, 0, 400, 300, 60, 60, 70);
            var card = FakeCard(7);
            for (int y = 0; y < card.Height; y++)
                for (int x = 0; x < card.Width; x++)
                    table.Set(100 + x, 40 + y, card.R(x, y), card.G(x, y), card.B(x, y));
            var match = index.Match(table, 104 / 400.0, 36 / 300.0, 150 / 400.0, 210 / 300.0, null);
            Assert.AreEqual("Card 7", match.Name);
            Assert.IsTrue(match.Sure, "score " + match.Score + ", margin " + match.Margin);
            var only = index.Match(table, 104 / 400.0, 36 / 300.0, 150 / 400.0, 210 / 300.0, new HashSet<string> { "Card 3", "Card 4" });
            Assert.AreNotEqual("Card 7", only.Name, "only the decks' cards are looked at");
            // Saved and read back (and a changed image file is read again).
            var back = new CardArtIndex();
            Assert.AreEqual(20, back.Load(index.Serialize(k => "stamp"), k => k == "K-3" ? "changed" : "stamp") + 1);
            Assert.IsFalse(back.Contains("K-3"));
            Assert.AreEqual("Card 7", back.Match(table, 104 / 400.0, 36 / 300.0, 150 / 400.0, 210 / 300.0, null).Name);
        }

        // ------------------------------------------------------------------ From readings to events

        private static MonReading M(string name, int hp, int max)
        {
            return new MonReading { Name = name, Hp = hp, MaxHp = max };
        }

        private static MonReading Empty()
        {
            return new MonReading { Empty = true };
        }

        private static VideoSnapshot Snap(double t, int turns0, int turns1, int prizes0, int prizes1, MonReading a0, MonReading[] b0, MonReading a1, MonReading[] b1)
        {
            var s = new VideoSnapshot { Time = t, Stadium = "" };
            s.Sides[0].Turns = turns0;
            s.Sides[1].Turns = turns1;
            s.Sides[0].Prizes = prizes0;
            s.Sides[1].Prizes = prizes1;
            s.Sides[0].Active = a0;
            s.Sides[1].Active = a1;
            s.Sides[0].Bench = b0;
            s.Sides[1].Bench = b1;
            return s;
        }

        /// <summary>
        /// An overlay video: left (went first) Dragapult ex with Drakloak; right Fezandipiti ex with Team Rocket's Mewtwo ex.
        /// Left's turn 5: Phantom Dive for 200 on Fezandipiti ex, 6 counters on Mewtwo. Right's turn 6: Mewtwo switched in,
        /// Erasure Ball for 160. Left's turn 7: Phantom Dive for 200 on Mewtwo, and the counters Knock Out Fezandipiti ex on
        /// the Bench (2 Prize cards). One moment is misread.
        /// </summary>
        private static List<VideoSnapshot> OverlayGame()
        {
            var snaps = new List<VideoSnapshot>();
            for (int t = 0; t <= 60; t++)
            {
                var a0 = M("Dragapult ex", 320, 320);
                var b0 = new[] { M("Drakloak", 90, 90), Empty(), Empty() };
                var a1 = M("Fezandipiti ex", 210, 210);
                var b1 = new[] { Empty(), M("Team Rocket's Mewtwo ex", 280, 280), Empty() };
                int c0 = 3, c1 = 2, p0 = 6, p1 = 6;
                if (t >= 10)
                {
                    a1 = M("Fezandipiti ex", 10, 210);
                    b1[1] = M("Team Rocket's Mewtwo ex", 220, 280);
                }
                if (t >= 20) c1 = 3;
                if (t >= 25)
                {
                    a1 = M("Team Rocket's Mewtwo ex", 220, 280);
                    b1[1] = M("Fezandipiti ex", 10, 210);
                }
                if (t >= 30) a0 = M("Dragapult ex", 160, 320);
                if (t >= 40) c0 = 4;
                if (t >= 45)
                {
                    a1 = M("Team Rocket's Mewtwo ex", 20, 280);
                    b1[1] = Empty();
                    p0 = 4;
                }
                if (t == 15) a0 = M("Dragonite", 320, 320); // one misread moment
                snaps.Add(Snap(t, c0, c1, p0, p1, a0, b0, a1, b1));
            }
            return snaps;
        }

        [Test]
        public void An_Overlay_Video_Becomes_Turns_Attacks_Knock_Outs_And_Prizes()
        {
            int first;
            var events = VideoTimeline.Build(OverlayGame(), db, new VideoTimelineOptions(), out first);
            string all = string.Join("\n", events.Select(e => e.Describe(new[] { "Aki", "Bo" })).ToArray());
            Assert.AreEqual(0, first, "the left player's turn counter is ahead: they went first");
            CollectionAssert.AreEqual(new[] { 5, 6, 7 }, events.Where(e => e.Kind == VideoEventKind.Turn).Select(e => e.Amount).ToArray(), all);
            Assert.IsTrue(events.Where(e => e.Setup).Any(e => e.Kind == VideoEventKind.Bench && e.Card == "Drakloak" && e.Player == 0), all);
            var attacks = events.Where(e => e.Kind == VideoEventKind.Attack).ToList();
            Assert.AreEqual(3, attacks.Count, all);
            Assert.AreEqual("Phantom Dive", attacks[0].Attack);
            Assert.IsTrue(attacks[0].Sure, all);
            Assert.AreEqual(200, attacks[0].Amount);
            Assert.AreEqual("Fezandipiti ex", attacks[0].Card2);
            Assert.AreEqual("Erasure Ball", attacks[1].Attack, all);
            Assert.AreEqual(1, attacks[1].Player);
            Assert.AreEqual("Team Rocket's Mewtwo ex", attacks[2].Card2);
            Assert.IsTrue(events.Any(e => e.Kind == VideoEventKind.Counters && e.Target == 1 && e.Card2 == "Team Rocket's Mewtwo ex" && e.Amount == 6), all);
            Assert.IsTrue(events.Any(e => e.Kind == VideoEventKind.Active && e.Player == 1 && e.Card == "Team Rocket's Mewtwo ex"), all);
            var ko = events.Single(e => e.Kind == VideoEventKind.KnockOut);
            Assert.AreEqual("Fezandipiti ex", ko.Card);
            Assert.IsTrue(ko.Sure, "Prize cards were taken");
            Assert.AreEqual(2, events.Single(e => e.Kind == VideoEventKind.Prizes).Amount);
            Assert.IsFalse(all.Contains("Dragonite"), "a single misread changes nothing");
        }

        [Test]
        public void An_Attack_That_Several_Attacks_Could_Be_Asks_Which()
        {
            var e = new VideoEvent { Kind = VideoEventKind.Attack };
            var mewtwo = db.Cards.Where(d => d.Name == "Team Rocket's Mewtwo ex").ToList();
            var dragapult = db.Cards.Where(d => d.Name == "Dragapult ex").ToList();
            // 70 damage on a Pokémon that was Knocked Out with 60 HP left: both attacks could have done it.
            VideoTimeline.GuessAttack(dragapult, mewtwo, 60, true, e);
            Assert.IsFalse(e.Sure);
            StringAssert.Contains("Phantom Dive", e.Choices);
            StringAssert.Contains("Jet Headbutt", e.Choices);
            Assert.IsTrue(e.IsUnidentifiedAttack);
            // Unidentified, it's written as damage counters (no decision is studied from it).
            e.Player = 0;
            e.Target = 1;
            e.Card = "Dragapult ex";
            e.Card2 = "Team Rocket's Mewtwo ex";
            e.Amount = 60;
            StringAssert.Contains("put 6 damage counters on Bo's Team Rocket's Mewtwo ex", e.Lines(new[] { "Aki", "Bo" })[0]);
        }

        [Test]
        public void An_Overlay_Is_Read_Into_A_Snapshot()
        {
            var layout = VideoLayout.ChampionsLeague();
            var r = new FrameReading { Time = 3 };
            r.Text[VideoRoles.Active(0)] = "ドラパルトex";
            r.Text[VideoRoles.ActiveHp(0)] = "320 /320";
            r.Text[VideoRoles.Bench(0, 1)] = "EMPTY";
            r.Text[VideoRoles.Bench(0, 2)] = "ド口ンチ";
            r.Text[VideoRoles.BenchHp(0, 2)] = "90/90";
            r.Text[VideoRoles.Bench(0, 3)] = "";
            r.Text[VideoRoles.BenchHp(0, 3)] = "70/70";
            r.Text[VideoRoles.Turns(0)] = "5";
            r.Counts[VideoRoles.Prizes(0)] = 5;
            r.Text[VideoRoles.Stadium] = "スタジアム:活力の森";
            r.Text[VideoRoles.Active(1)] = "キチキギスex";
            var snap = SnapshotReader.Read(r, layout, Names());
            var left = snap.Sides[0];
            Assert.AreEqual("Dragapult ex", left.Active.Name);
            Assert.AreEqual(320, left.Active.Hp);
            Assert.AreEqual(320, left.Active.MaxHp);
            Assert.IsTrue(left.Bench[0].Empty);
            Assert.AreEqual("Drakloak", left.Bench[1].Name);
            Assert.IsFalse(left.Bench[2].Empty, "a name missed with an HP showing isn't an empty place");
            Assert.IsNull(left.Bench[2].Name);
            Assert.AreEqual(5, left.Turns);
            Assert.AreEqual(5, left.Prizes);
            Assert.AreEqual("Forest of Vitality", snap.Stadium);
            Assert.IsNull(snap.Sides[1].Active.Name);
            CollectionAssert.Contains(snap.UnknownNames, "キチキギスex");
        }

        [Test]
        public void A_Tcg_Live_Recording_Gets_Its_Turns_From_The_Clocks()
        {
            var layout = VideoLayout.TcgLive();
            var snaps = new List<VideoSnapshot>();
            int clock0 = 1200, clock1 = 1200;
            for (int t = 0; t < 50; t++)
            {
                if (t < 20) clock1--;
                else clock0--;
                var r = new FrameReading { Time = t };
                r.Art[VideoRoles.ActiveArt(0)] = "Dragapult ex";
                r.Text[VideoRoles.ActiveHp(0)] = "320";
                r.Art[VideoRoles.ActiveArt(1)] = "Fezandipiti ex";
                r.Text[VideoRoles.ActiveHp(1)] = t >= 30 ? "10" : "210";
                r.Art[VideoRoles.BenchArt(1, 1)] = t >= 12 ? "Team Rocket's Mewtwo ex" : "";
                r.Text[VideoRoles.Clock(0)] = (clock0 / 60).ToString("00") + ":" + (clock0 % 60).ToString("00");
                r.Text[VideoRoles.Clock(1)] = (clock1 / 60).ToString("00") + ":" + (clock1 % 60).ToString("00");
                r.Text[VideoRoles.Prizes(0)] = "6";
                r.Text[VideoRoles.Prizes(1)] = "6";
                snaps.Add(SnapshotReader.Read(r, layout, Names()));
            }
            int first;
            var events = VideoTimeline.Build(snaps, db, new VideoTimelineOptions(), out first);
            string all = string.Join("\n", events.Select(e => e.Describe(null)).ToArray());
            var turns = events.Where(e => e.Kind == VideoEventKind.Turn).Select(e => e.Player).ToArray();
            CollectionAssert.AreEqual(new[] { 1, 0 }, turns, all);
            Assert.IsTrue(events.Any(e => e.Kind == VideoEventKind.Bench && !e.Setup && e.Card == "Team Rocket's Mewtwo ex" && e.Player == 1), all);
            var attack = events.Single(e => e.Kind == VideoEventKind.Attack);
            Assert.AreEqual("Phantom Dive", attack.Attack, all);
            Assert.AreEqual(0, attack.Player);
        }

        // ------------------------------------------------------------------ YouTube links

        [Test]
        public void A_YouTube_Link_Is_Recognised_In_Its_Usual_Forms()
        {
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("https://www.youtube.com/watch?v=dQw4w9WgXcQ"));
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("  youtube.com/watch?feature=share&v=dQw4w9WgXcQ&t=42s "));
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("https://youtu.be/dQw4w9WgXcQ?si=abc"));
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("https://m.youtube.com/shorts/dQw4w9WgXcQ"));
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("https://www.youtube.com/live/dQw4w9WgXcQ?feature=shared"));
            Assert.AreEqual("dQw4w9WgXcQ", VideoDownload.YouTubeId("https://www.youtube.com/watch?v=dQw4w9WgXcQ&list=PL123&index=2"));
            Assert.IsFalse(VideoDownload.IsYouTubeLink("C:\\videos\\CL final.mp4"), "a file is opened as a file");
            Assert.IsFalse(VideoDownload.IsYouTubeLink("https://www.youtube.com/@PokemonTCG"));
            Assert.IsFalse(VideoDownload.IsYouTubeLink("https://youtu.be/short"));
            Assert.IsFalse(VideoDownload.IsYouTubeLink(""));
        }

        [Test]
        public void YouTube_Downloads_Ask_For_A_Playable_Mp4_And_Show_Their_Progress()
        {
            string args = VideoDownload.Arguments("dQw4w9WgXcQ", "C:\\AI\\Videos\\Downloads");
            StringAssert.Contains("https://www.youtube.com/watch?v=dQw4w9WgXcQ", args);
            StringAssert.Contains("--no-playlist", args);
            StringAssert.Contains("vcodec^=avc1", args, "H.264, which Unity's VideoPlayer plays");
            StringAssert.Contains("height<=1080", args);
            StringAssert.Contains("[%(id)s].%(ext)s", args, "the id in the file name, to find it again");
            Assert.AreEqual(42.5, VideoDownload.ParseProgress("[download]  42.5% of ~ 120.00MiB at  5.00MiB/s ETA 00:20"), 1e-9);
            Assert.AreEqual(100, VideoDownload.ParseProgress("[download] 100% of 120.00MiB in 00:24"), 1e-9);
            Assert.AreEqual(-1, VideoDownload.ParseProgress("[youtube] dQw4w9WgXcQ: Downloading webpage"));
            StringAssert.Contains("H.264", VideoDownload.Explain("[youtube] dQw4w9WgXcQ: Requested format is not available."));
            StringAssert.Contains("private", VideoDownload.Explain("[youtube] x: Private video. Sign in if you've been granted access"));

            string folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ptcg video " + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(folder);
            try
            {
                Assert.IsNull(VideoDownload.FindDownloaded(folder, "dQw4w9WgXcQ"));
                string saved = System.IO.Path.Combine(folder, "Regionals Top 8 [dQw4w9WgXcQ].mp4");
                System.IO.File.WriteAllText(saved, "x");
                Assert.AreEqual(saved, VideoDownload.FindDownloaded(folder, "dQw4w9WgXcQ"), "downloaded once, opened from the folder after that");
                Assert.IsNull(VideoDownload.FindDownloaded(folder, "aaaaaaaaaaa"));
                Assert.IsNull(VideoDownload.FindTool(folder));
            }
            finally
            {
                System.IO.Directory.Delete(folder, true);
            }
        }

        // ------------------------------------------------------------------ Studying a video

        [Test]
        public void A_Video_Is_Studied_As_A_Battle_Log()
        {
            var session = new VideoSession { VideoPath = "C:\\videos\\CL final.mp4", Names = new[] { "Aki", "Bo" } };
            int first;
            session.Events.AddRange(VideoTimeline.Build(OverlayGame(), db, new VideoTimelineOptions(), out first));
            session.FirstPlayer = first;
            session.Events.Add(new VideoEvent { Time = 22, Kind = VideoEventKind.Trainer, Player = 1, Card = "Switch", Manual = true });
            session.SortEvents(); // as the app does after adding an event by hand
            string log = session.BuildLog();
            StringAssert.StartsWith(VideoSession.LogHeader, log);
            StringAssert.Contains("Aki's Turn 5", log);
            StringAssert.Contains("Aki's Dragapult ex used Phantom Dive on Bo's Fezandipiti ex for 200 damage.", log);
            var parsed = TcgLiveLog.Parse(log);
            Assert.IsTrue(parsed.FromVideo);
            Assert.AreEqual("Aki", parsed.FirstPlayer);
            Assert.AreEqual(5, parsed.Turns.First(t => t.Number > 0).Number, "the video's own turn numbers are kept");

            var brain = new LearningBrain();
            var report = LogStudy.Study(log, session.Title, db, StandardCards.CreateRegistry(), brain);
            Assert.IsNull(report.Error, report.Error);
            Assert.AreEqual("video", report.Kind);
            Assert.AreEqual(0, report.UnreadLines, string.Join("\n", report.Unread.ToArray()));
            // Dragapult ex's 2 attacks line up once their Energy is attached. Team Rocket's Mewtwo ex's can't: its Power Saver
            // needs 4 Team Rocket's Pokémon in play, and this game has 1.
            Assert.GreaterOrEqual(report.Matched, 2, "the attacks line up once their Energy is attached: " + report.Summary());
            Assert.Greater(report.TotalStudied, 0, report.Summary());
            Assert.AreEqual(1, brain.LogsStudied);
        }

        [Test]
        public void A_Video_Study_Is_Saved_And_Read_Back()
        {
            var session = new VideoSession { VideoPath = "D:\\v\\game.mp4", LayoutName = "Champions League overlay (Japanese)", Interval = 0.5, FirstPlayer = 0, Winner = 1 };
            session.Names = new[] { "Aki", "Bo" };
            var r = new FrameReading { Time = 1.5 };
            r.Text[VideoRoles.Active(0)] = "ドラパルトex";
            r.Counts[VideoRoles.Prizes(0)] = 4;
            r.Art[VideoRoles.ActiveArt(1)] = "Fezandipiti ex";
            session.Readings.Add(r);
            session.Events.Add(new VideoEvent { Time = 2, Kind = VideoEventKind.Attack, Player = 0, Target = 1, Card = "Dragapult ex", Card2 = "Fezandipiti ex", Amount = 200, Attack = "", Choices = "Jet Headbutt, Phantom Dive", Sure = false });
            session.Events.Add(new VideoEvent { Time = 3, Kind = VideoEventKind.Line, Text = "Aki ended their turn.", Manual = true });
            var back = VideoSession.Parse(session.Serialize());
            Assert.AreEqual(session.VideoPath, back.VideoPath);
            Assert.AreEqual(0.5, back.Interval, 1e-9);
            Assert.AreEqual(1, back.Winner);
            Assert.AreEqual("ドラパルトex", back.Readings[0].Text[VideoRoles.Active(0)]);
            Assert.AreEqual(4, back.Readings[0].Counts[VideoRoles.Prizes(0)]);
            Assert.AreEqual("Fezandipiti ex", back.Readings[0].Art[VideoRoles.ActiveArt(1)]);
            Assert.AreEqual(2, back.Events.Count);
            Assert.IsTrue(back.Events[0].IsUnidentifiedAttack);
            Assert.AreEqual("Jet Headbutt, Phantom Dive", back.Events[0].Choices);
            Assert.IsTrue(back.Events[1].Manual);
            StringAssert.Contains("Bo wins.", back.BuildLog());
        }
    }
}
