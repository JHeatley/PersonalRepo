using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Silver Tempest (SIT 1-150, 2022): the Pokémon that weren't already in the pool (SIT 5 is CRZ 7 Sunkern, SIT 118 is
    /// CRZ 90 Metang). Regulation mark F: Expanded only. The full arts and secret rares (170-211, 214, 215) and the Trainer
    /// Gallery's reprints are second prints of the main cards. Pokémon whose attacks only do their printed damage need no
    /// script: Venonat, Spinarak, Petilil, Foongus, Growlithe, Fletchinder, Dewpider, Chinchou, Solosis, Elgyem, Espurr,
    /// Swirlix, Drakloak, Zubat, Golbat, Croagunk, Sandile, Ferroseed, Ferrothorn, Noibat, Archen and Fletchling.
    /// The Trainer Gallery Pokémon, Trainers and Energy are in CardsSIT2.
    /// </summary>
    public static class CardsSIT1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SIT-2", new MiraclePowderMon());
            r.RegisterByKey("SIT-4", new AriadosSIT());
            r.RegisterByKey("SIT-6", new BrightBeamMon());
            r.RegisterByKey("SIT-7", new SerperiorV());
            r.RegisterByKey("SIT-8", new SerperiorVstar());
            r.RegisterByKey("SIT-10", new HisuianLilligantSIT());
            r.RegisterByKey("SIT-12", new AmoongussSIT());
            r.RegisterByKey("SIT-13", new SearchAttackMon("Energy Digging", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("SIT-14", new ComboMon(new FourAsOneMon(70, "Cobalion", "Terrakion", "Keldeo"), new CardsTWM1.HealSelfMon("Leaf Drain", 20)));
            r.RegisterByKey("SIT-15", new CardsASC.CounterattackMon("Needle Line", 3, new CardsTWM1.HealSelfMon("Touchdown", 30)));
            r.RegisterByKey("SIT-16", new RadiantTsareena());
            r.RegisterByKey("SIT-17", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("SIT-18", new ComboMon(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep), new LockAfterAttackMon("Scorching Breath", false)));
            r.RegisterByKey("SIT-20", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire), new CardsCRZ1.VBonusMon("Fighting Tackle", 100)));
            r.RegisterByKey("SIT-21", new RecoilMon("Take Down", 10));
            r.RegisterByKey("SIT-22", new RapidashSIT());
            r.RegisterByKey("SIT-23", new AssistingFlameMon());
            r.RegisterByKey("SIT-24", new ComboMon(new SparklingWingMon(), new CardsSVI1.FlipTailsLockMon("White Blaze")));
            r.RegisterByKey("SIT-25", new SearchAttackMon("Lead", d => d.IsSupporter, 1));
            r.RegisterByKey("SIT-26", new FlareParadeMon());
            r.RegisterByKey("SIT-27", new ComboMon(new FlareParadeMon(), new EnergyCrushMon()));
            r.RegisterByKey("SIT-29", new ComboMon(new CardsMEG.SnipeMon("Quick Dive", 50), new CardsPFL.DamagedBonusMon("Merciless Strike", 80)));
            r.RegisterByKey("SIT-30", new CardsBLK.FlailMon("Reprisal"));
            r.RegisterByKey("SIT-31", new GrittyClawsMon());
            r.RegisterByKey("SIT-32", new ComboMon(new SecretAttackMon(), new CardsTWM1.SelfDiscardMon("Flare Shot", 99)));
            r.RegisterByKey("SIT-33", new CardsCRZ1.VBonusMon("White Drop", 50));
            r.RegisterByKey("SIT-34", new AlolanVulpixVstar());
            r.RegisterByKey("SIT-35", new OmastarV());
            r.RegisterByKey("SIT-36", new CardsBRS2.WildShockMon("Wild Freeze", 50));
            r.RegisterByKey("SIT-37", new CardsDAA1.DrainMon("Gulp Down"));
            r.RegisterByKey("SIT-38", new WailordSIT());
            r.RegisterByKey("SIT-39", new AscensionMon());
            r.RegisterByKey("SIT-40", new ComboMon(new CardsMEG.SnipeMon("Water Arrow", 50), new ConditionAttackMon("Mellow Wave", SpecialCondition.Asleep)));
            r.RegisterByKey("SIT-41", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SIT-42", new CandiceBonusMon("Big Mouth", 130, false));
            r.RegisterByKey("SIT-43", new CandiceBonusMon("Frosty Jail", 90, true));
            r.RegisterByKey("SIT-44", new ComboMon(new FossilFindingMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("SIT-45", new SeaFeastMon());
            r.RegisterByKey("SIT-46", new FourAsOneMon(170, "Cobalion", "Terrakion", "Virizion"));
            r.RegisterByKey("SIT-48", new DrowningBallMon());
            r.RegisterByKey("SIT-49", new PikaStrikeMon());
            r.RegisterByKey("SIT-50", new AmbushingSparkMon());
            r.RegisterByKey("SIT-52", new CardsTWM1.SelfDiscardMon("Strong Volt", 2));
            r.RegisterByKey("SIT-53", new CardsDAA1.DeepSeaSwirlMon("Overhaul", 6));
            r.RegisterByKey("SIT-54", new SearchAttackMon("Lucky Find", d => d.IsItem, 1));
            r.RegisterByKey("SIT-55", new MysteryBoltMon());
            r.RegisterByKey("SIT-56", new CardsPAL1.EvolutionBonusMon("Battle Claw", 30));
            r.RegisterByKey("SIT-57", new ComboMon(new SwitchSelfMon("Switching Bolt", false), new CardsASC.SelfGuardMon("Lightning Wall", 100)));
            r.RegisterByKey("SIT-58", new RegielekiVmax());
            r.RegisterByKey("SIT-59", new RadiantAlakazam());
            r.RegisterByKey("SIT-60", new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep));
            r.RegisterByKey("SIT-61", new PsyCallMon());
            r.RegisterByKey("SIT-62", new JynxSIT());
            r.RegisterByKey("SIT-63", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("SIT-64", new MismagiusSIT());
            r.RegisterByKey("SIT-65", new UnownV());
            r.RegisterByKey("SIT-66", new UnownVstar());
            r.RegisterByKey("SIT-67", new CardsPOR.MemoryLockMon("Memory Skip"));
            r.RegisterByKey("SIT-68", new RefinementMon());
            r.RegisterByKey("SIT-69", new RefinementMon());
            r.RegisterByKey("SIT-70", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Pouty Slap"), new CardsASC.RandomDiscardMon("Chomp Down")));
            r.RegisterByKey("SIT-71", new MawileVstar());
            r.RegisterByKey("SIT-72", new MedititeSIT());
            r.RegisterByKey("SIT-73", new MedichamSIT());
            r.RegisterByKey("SIT-74", new DreamingToneMon());
            r.RegisterByKey("SIT-75", new WarningMon());
            r.RegisterByKey("SIT-77", new CellSpearMon());
            r.RegisterByKey("SIT-78", new ReuniclusSIT());
            r.RegisterByKey("SIT-80", new PsychicArrowMon());
            r.RegisterByKey("SIT-82", new MeowsticSIT());
            r.RegisterByKey("SIT-84", new CardsTWM1.HealSelfMon("Draining Kiss", 30));
            r.RegisterByKey("SIT-85", new DedenneSIT());
            r.RegisterByKey("SIT-86", new IndeedeeSIT());
            r.RegisterByKey("SIT-87", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SIT-89", new DragapultSIT());
            r.RegisterByKey("SIT-90", new HisuianArcanineV());
            r.RegisterByKey("SIT-91", new FlipBonusMon("Knock Away", 20));
            r.RegisterByKey("SIT-92", new OverspinMon());
            r.RegisterByKey("SIT-93", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("SIT-94", new ClaydolSIT());
            r.RegisterByKey("SIT-95", new FossilDropMon());
            r.RegisterByKey("SIT-96", new ReapingClawMon());
            r.RegisterByKey("SIT-97", new CavernTackleMon());
            r.RegisterByKey("SIT-98", new ComboMon(new DrawAttackMon("Allure", 2), new CardsWHT.FlipMoreMon("Acrobatics", 2, 20)));
            r.RegisterByKey("SIT-99", new NoRetreatMon("Corner"));
            r.RegisterByKey("SIT-100", new CardsCEC3.SpreadAmountMon("Sandpot Trap", 30));
            r.RegisterByKey("SIT-101", new MysteryPressMon());
            r.RegisterByKey("SIT-102", new UrsalunaV());
            r.RegisterByKey("SIT-105", new ComboMon(new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned), new CriticalBiteMon()));
            r.RegisterByKey("SIT-106", new FlockMon());
            r.RegisterByKey("SIT-107", new ComboMon(new DrawAttackMon("Triple Draw", 3), new CallousWingsMon()));
            r.RegisterByKey("SIT-108", new ComboMon(new PursuitBlastMon(), new CardsCES1.ConditionsMon("Shrieking Poison", SpecialCondition.Confused, SpecialCondition.Poisoned)));
            r.RegisterByKey("SIT-110", new DivingUppercutMon());
            r.RegisterByKey("SIT-112", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("SIT-113", new KrookodileSIT());
            r.RegisterByKey("SIT-114", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("SIT-115", new CardsBLK.VenoshockMon(120));
            r.RegisterByKey("SIT-116", new PekoBlasterMon());
            r.RegisterByKey("SIT-117", new MagneticLiftMon());
            r.RegisterByKey("SIT-119", new MetagrossSIT());
            r.RegisterByKey("SIT-120", new RadiantJirachi());
            r.RegisterByKey("SIT-123", new SearchAttackMon("Call Sign", d => d.IsPokemon, 1));
            r.RegisterByKey("SIT-124", new CoinsMon("Gear Grind", 2));
            r.RegisterByKey("SIT-125", new KlinklangSIT());
            r.RegisterByKey("SIT-126", new CobalionSIT());
            r.RegisterByKey("SIT-127", new TogeDashMon());
            r.RegisterByKey("SIT-128", new ComboMon(new CardsMEG.SnipeMon("Gear Throw", 30), new SpecialLaserMon("Special Laser", 120)));
            r.RegisterByKey("SIT-129", new CoinsMon("Slam", 2));
            r.RegisterByKey("SIT-130", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("SIT-131", new EnergyHurricaneMon());
            r.RegisterByKey("SIT-133", new ComboMon(new RadiantHuntMon(), new CardsBCR1.DrawUntilAttackMon("Seventh Echo", 7)));
            r.RegisterByKey("SIT-134", new ShoutOfPowerMon());
            r.RegisterByKey("SIT-135", new ComboMon(new CelestialRoarMon(), new Cards30CC.BenchHitMon("Dragon Laser", 30)));
            r.RegisterByKey("SIT-136", new RegidragoVstar());
            r.RegisterByKey("SIT-137", new ColorfulPaletteMon());
            r.RegisterByKey("SIT-138", new ComboMon(new CardsCRZ1.CycleDrawMon("Read the Wind", 3), new MayDiscardStadiumMon("Aero Dive")));
            r.RegisterByKey("SIT-139", new LugiaVstar());
            r.RegisterByKey("SIT-140", new HoOhV());
            r.RegisterByKey("SIT-141", new DizzyingSpinMon());
            r.RegisterByKey("SIT-142", new CardsPFL.FlipUntilTailsTimesMon("Continuous Spin"));
            r.RegisterByKey("SIT-143", new MagicalEchoMon());
            r.RegisterByKey("SIT-144", new TailsNothingMon("Try Bouncing"));
            r.RegisterByKey("SIT-145", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("SIT-147", new ArcheopsSIT());
            r.RegisterByKey("SIT-148", new FlipBonusMon("Jump On", 30));
            r.RegisterByKey("SIT-149", new ComboMon(new EerieCryMon(), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
        }

        // ================================================================== Shared helpers

        /// <summary>"If you have no Supporter cards in your discard pile" (Mystery Bolt, Mystery Charge, Mystery Press).</summary>
        internal static bool NoSupporterInDiscard(EffectContext ctx)
        {
            return !ctx.Me.Discard.Any(c => c.Def.IsSupporter);
        }

        /// <summary>
        /// "If 1 of your [name] used [attack] during your last turn": this player's last attack was made on their previous turn
        /// by a Pokémon with that name (Dedenne's Dede-Short, Togedemaru's Toge Dash, Terrakion's Cavern Tackle).
        /// </summary>
        internal static bool UsedLastTurn(EffectContext ctx, string pokemonName, string attack)
        {
            var me = ctx.Me;
            return me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackName == attack
                   && (pokemonName == null || (me.LastAttackerDef != null && me.LastAttackerDef.Name == pokemonName));
        }

        /// <summary>Search the deck for up to max cards and put them onto the Bench, whatever their Stage (Psy Call, Primal Guidance).</summary>
        internal static int SearchAnyStageToBench(EffectContext ctx, Func<CardDef, bool> filter, int max, string prompt)
        {
            int n = Math.Min(max, ctx.BenchSpace);
            if (n <= 0) return 0;
            var found = ctx.SearchMyDeck(d => filter(d) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n, prompt);
            foreach (var c in found) ctx.Engine.PutOntoBench(ctx.Player, c, false);
            ctx.ShuffleMyDeck();
            return found.Count;
        }

        /// <summary>Move any amount of Energy between your Pokémon in any way you like (Regal Blender).</summary>
        internal static void MoveEnergyAnyWay(EffectContext ctx, string name)
        {
            for (int guard = 0; guard < 40; guard++)
            {
                var mine = ctx.MyPokemon();
                var withEnergy = mine.Where(m => m.Energy.Count > 0).ToList();
                if (withEnergy.Count == 0 || mine.Count < 2) break;
                if (!ctx.AskYesNo(name + ": move an Energy between your Pokémon?")) break;
                var from = ctx.ChoosePokemon(withEnergy, name + ": move an Energy from which Pokémon?");
                var energy = ctx.ChooseCards(from.Energy.ToList(), 1, 1, name + ": choose the Energy to move")[0];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), name + ": move " + energy.Def.Name + " to which Pokémon?");
                ctx.Engine.MoveEnergy(from, to, energy);
            }
        }

        // ================================================================== Reusable attacks

        /// <summary>Four as One: N more damage if the three other Swords of Justice are on your Bench.</summary>
        internal sealed class FourAsOneMon : Mon
        {
            private readonly int bonus;
            private readonly string[] names;

            public FourAsOneMon(int bonus, params string[] names)
            {
                this.bonus = bonus;
                this.names = names;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Four as One") return false;
                bool all = names.All(n => ctx.Me.Bench.Any(m => m.Name == n));
                ctx.DamageDefending(atk.Damage + (all ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If this Pokémon has any Special Energy attached, this attack does N more damage."</summary>
        internal sealed class SpecialLaserMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public SpecialLaserMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) ? bonus : 0));
                return true;
            }
        }

        /// <summary>"You may discard a Stadium in play" after the damage (Aero Dive, Tempest Dive).</summary>
        internal sealed class MayDiscardStadiumMon : Mon
        {
            private readonly string attack;

            public MayDiscardStadiumMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo(attack + ": discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>"If you played Candice from your hand during this turn, this attack does N more damage" (Frosty Jail also Paralyzes).</summary>
        internal sealed class CandiceBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly bool paralyze;

            public CandiceBonusMon(string attack, int bonus, bool paralyze)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.paralyze = paralyze;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool candice = ctx.Me.SupporterNameThisTurn == "Candice";
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (candice ? bonus : 0));
                if (candice && paralyze && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Flare Parade: N damage for each Serena card in your discard pile.</summary>
        internal sealed class FlareParadeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Parade") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => c.Def.Name == "Serena"));
                return true;
            }
        }

        /// <summary>Refinement: discard a card from your hand to draw 2 cards, once during your turn (Kirlia, Gardevoir).</summary>
        internal sealed class RefinementMon : Mon
        {
            protected override string Activated { get { return "Refinement"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Refinement: discard a card from your hand (then draw 2 cards)");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(2);
            }
        }

        /// <summary>Ascension: search your deck for a card that evolves from this Pokémon and evolve it.</summary>
        internal sealed class AscensionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ascension") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                string name = ctx.Pokemon.Name;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Ascension: choose a card that evolves from " + name, false);
                if (found.Count > 0) ctx.Engine.EvolvePokemon(ctx.Pokemon, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Colorful Palette: look at the top 5, attach any number of basic Energy there to 1 of your Pokémon, shuffle the rest back.</summary>
        internal sealed class ColorfulPaletteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Palette") return false;
                var top = ctx.LookAtTop(5);
                if (top.Count == 0) return true;
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                var picks = ctx.ChooseCards(energy, 0, energy.Count, "Colorful Palette: choose any number of basic Energy cards to attach to 1 of your Pokémon");
                if (picks.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Colorful Palette: attach them to which Pokémon?");
                    foreach (var card in picks)
                    {
                        ctx.Me.Deck.Remove(card);
                        ctx.Engine.AttachEnergyCard(card, target);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Magical Echo: move all damage counters from 1 of your Benched Pokémon to your opponent's Active Pokémon.</summary>
        internal sealed class MagicalEchoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Echo") return false;
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0 || ctx.Defending == null) return true;
                var from = ctx.ChoosePokemon(hurt, "Magical Echo: move all damage counters from which of your Benched Pokémon?");
                ctx.Engine.MoveDamageCounters(from, ctx.Defending, from.DamageCounters, "Magical Echo");
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Miracle Powder: flip a coin; on heads, choose a Special Condition for the opponent's Active Pokémon.</summary>
        private sealed class MiraclePowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miracle Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                var conditions = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                int pick = ctx.ChooseOption("Miracle Powder: choose a Special Condition for " + defending.Name, "Asleep", "Burned", "Confused", "Paralyzed", "Poisoned");
                ctx.ApplyCondition(defending, conditions[Math.Max(0, Math.Min(4, pick))]);
                return true;
            }
        }

        /// <summary>Hidden Threads: the opponent's Pokémon VSTAR's attacks cost [C] more (doesn't stack).</summary>
        private sealed class AriadosSIT : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon.Owner == src.Player || !mon.Def.IsVstar) return;
                if (!CardsCRZ1.IsLeadSource(src, "Hidden Threads")) return;
                cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Bright Beam: discard up to 3 Energy cards from your hand, 70 more damage for each.</summary>
        private sealed class BrightBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bright Beam") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                int n = 0;
                if (energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, Math.Min(3, energy.Count), "Bright Beam: discard up to 3 Energy cards from your hand (70 more damage for each)");
                    foreach (var card in picks)
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 70 * n);
                return true;
            }
        }

        /// <summary>Noble Light: heal 30 damage from each Pokémon (both players').</summary>
        private sealed class SerperiorV : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Noble Light") return false;
                foreach (var mon in ctx.MyPokemon()) ctx.Heal(mon, 30);
                foreach (var mon in ctx.OpponentPokemon()) ctx.Heal(mon, 30);
                return true;
            }
        }

        /// <summary>Regal Blender: then move any amount of Energy between your Pokémon. Star Winder: 60 for each Energy on this Pokémon, then switch it with a Benched Pokémon.</summary>
        private sealed class SerperiorVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Regal Blender")
                {
                    ctx.DamageDefending(atk.Damage);
                    MoveEnergyAnyWay(ctx, "Regal Blender");
                    return true;
                }
                if (atk.Name != "Star Winder") return false;
                ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Swelling Scent: search for up to 2 basic Grass Energy and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class HisuianLilligantSIT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swelling Scent") return false;
                if (ctx.Me.Deck.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 2, "Swelling Scent: choose up to 2 Grass Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, m) => !ctx.IsActive(m), "Swelling Scent");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Surprise Spores: during the opponent's turn, if this card is discarded from your hand by an effect of their
        /// Pokémon's attack or Ability or of their Item or Supporter card, discard their hand. Hypno Hammer: Asleep.
        /// </summary>
        private sealed class AmoongussSIT : Mon
        {
            private static readonly Mon hypnoHammer = new ConditionAttackMon("Hypno Hammer", SpecialCondition.Asleep);

            public override bool WatchesHandDiscardByOpponent { get { return true; } }

            public override void OnDiscardedFromHandByOpponent(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count == 0) return;
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Amoonguss used its Ability Surprise Spores.", ctx.Card.Id);
                ctx.Engine.DiscardHand(ctx.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return hypnoHammer.HandleAttack(ctx, atk); }
        }

        /// <summary>Elegant Heal: once during your turn, heal 20 damage from each of your Pokémon. Aroma Shot: this Pokémon recovers from all Special Conditions.</summary>
        private sealed class RadiantTsareena : Mon
        {
            protected override string Activated { get { return "Elegant Heal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.InPlay().Any(m => m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon()) ctx.Heal(mon, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aroma Shot") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Heat Boost: discard a Fire Energy card from your hand; this turn your Fire Pokémon's attacks do 30 more to the opponent's Active Pokémon.</summary>
        private sealed class RapidashSIT : Mon
        {
            protected override string Activated { get { return "Heat Boost"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return;
                var pick = ctx.ChooseCards(fire, 1, 1, "Heat Boost: discard a Fire Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.AddEffect(new ConditionalDamageBonusEffect("Heat Boost: your Fire Pokémon's attacks do 30 more damage to the opponent's Active Pokémon this turn",
                    ctx.Player, ctx.Turn, 30, (engine, attacker, target) => engine.IsType(attacker, EnergyType.Fire)));
            }
        }

        /// <summary>Assisting Flame: attach up to 2 Fire Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class AssistingFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assisting Flame") return false;
                ctx.DamageDefending(atk.Damage);
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                ctx.AttachCardsAnyWay(fire, 2, (card, m) => true, "Assisting Flame");
                return true;
            }
        }

        /// <summary>Sparkling Wing: search for up to 2 basic Energy and attach them to 1 of your Pokémon.</summary>
        private sealed class SparklingWingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sparkling Wing") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Sparkling Wing: choose up to 2 basic Energy cards");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Sparkling Wing: attach them to which Pokémon?");
                    foreach (var c in found) ctx.Engine.AttachEnergyCard(c, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Energy Crush: 50 damage for each Energy attached to all of your opponent's Pokémon.</summary>
        private sealed class EnergyCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Crush") return false;
                int n = ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Gritty Claws: during the opponent's next turn, if this Pokémon has full HP and would be Knocked Out by attack damage, it keeps 10 HP.</summary>
        private sealed class GrittyClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gritty Claws") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new GrittyClawsEffect(ctx.Pokemon.Name + " can't be Knocked Out from full HP during the opponent's next turn (Gritty Claws)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        public sealed class GrittyClawsEffect : ContinuousEffect
        {
            public GrittyClawsEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromFullHp(GameEngine engine, PokemonInPlay mon) { return mon.Uid == TargetUid; }
        }

        /// <summary>Secret Attack: choose an attack from 1 of this Pokémon's previous Evolutions and use it as this attack.</summary>
        private sealed class SecretAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Secret Attack") return false;
                var me = ctx.Pokemon;
                var opts = new List<DecisionOption>();
                var seen = new HashSet<string>();
                for (int k = 0; k < me.Stack.Count - 1; k++)
                {
                    var d = me.Stack[k].Def;
                    if (!d.IsPokemon || !seen.Add(d.BaseKey)) continue;
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        if (a.VstarPower && ctx.Me.VstarUsed) continue;
                        if (ctx.Engine.AttackNamedPrevented(me, a.Name)) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                }
                if (opts.Count == 0) return Nothing(ctx);
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Secret Attack: choose an attack of a previous Evolution to use", opts, ctx.Source);
                UseCopiedAttack(ctx, opts[pick].Card, opts[pick].Index);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Snow Mirage: not affected by effects on the Defending Pokémon; next turn, prevent all damage from attacks of Pokémon
        /// that have an Ability. Silvery Snow Star: 70 for each of the opponent's Pokémon V, no Weakness or Resistance.
        /// </summary>
        private sealed class AlolanVulpixVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snow Mirage")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new CardsCRI.AbilityShieldEffect(ctx.Pokemon.Name + " takes no damage from attacks by Pokémon with an Ability during the opponent's next turn (Snow Mirage)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name != "Silvery Snow Star") return false;
                int n = ctx.OpponentPokemon(m => m.Def.IsPokemonV).Count;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage * n, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Primal Guidance: put up to 2 Pokémon that evolve from an Item card with "Fossil" in its name from your deck onto your
        /// Bench. Tentacle Lock: if the Defending Pokémon is an Evolution Pokémon, it can't attack during the opponent's next turn.
        /// </summary>
        private sealed class OmastarV : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Primal Guidance")
                {
                    if (ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0)
                        SearchAnyStageToBench(ctx, d => d.IsPokemon && !string.IsNullOrEmpty(d.EvolvesFrom) && d.EvolvesFrom.Contains("Fossil"), 2,
                            "Primal Guidance: choose up to 2 Pokémon that evolve from a Fossil to put onto your Bench");
                    return true;
                }
                if (atk.Name != "Tentacle Lock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsEvolution)
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Tentacle Lock)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Jumbo-Sized: this Pokémon takes 30 less damage from attacks. Special Wave: 120 more with a Special Energy attached.</summary>
        private sealed class WailordSIT : Mon
        {
            private static readonly Mon specialWave = new SpecialLaserMon("Special Wave", 120);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return specialWave.HandleAttack(ctx, atk); }
        }

        /// <summary>Fossil Finding: shuffle up to 4 Unidentified Fossil and Rare Fossil cards from your discard pile into your deck.</summary>
        private sealed class FossilFindingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Finding") return false;
                var fossils = ctx.Me.Discard.Where(c => c.Def.Name == "Unidentified Fossil" || c.Def.Name == "Rare Fossil").ToList();
                if (fossils.Count == 0) return true;
                var picks = ctx.ChooseCards(fossils, 0, Math.Min(4, fossils.Count), "Fossil Finding: shuffle up to 4 Unidentified Fossil and Rare Fossil cards into your deck");
                if (picks.Count > 0)
                {
                    ctx.DiscardToDeck(picks);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Sea Feast: put up to 3 Basic Water Pokémon from your deck onto your Bench.</summary>
        private sealed class SeaFeastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sea Feast") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.HasType(EnergyType.Water), 3, "Sea Feast: choose up to 3 Basic Water Pokémon to put onto your Bench");
                return true;
            }
        }

        /// <summary>Drowning Ball: flip a coin; on heads, the opponent's Active Pokémon is Paralyzed and you discard an Energy from it.</summary>
        private sealed class DrowningBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drowning Ball") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Drowning Ball: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Pika Strike: 180 more damage if 1 of your Dedenne used Dede-Short during your last turn.</summary>
        private sealed class PikaStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pika Strike") return false;
                ctx.DamageDefending(atk.Damage + (UsedLastTurn(ctx, "Dedenne", "Dede-Short") ? 180 : 0));
                return true;
            }
        }

        /// <summary>Ambushing Spark: 100 more damage if your opponent has used their VSTAR Power this game.</summary>
        private sealed class AmbushingSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ambushing Spark") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.VstarUsed ? 100 : 0));
                return true;
            }
        }

        /// <summary>Mystery Bolt: only with no Supporter cards in your discard pile; 100 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class MysteryBoltMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Mystery Bolt" || NoSupporterInDiscard(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mystery Bolt") return false;
                ctx.DamageAnyOpponent(100);
                return true;
            }
        }

        /// <summary>Transistor: your Basic Lightning Pokémon's attacks do 30 more damage to the opponent's Active Pokémon. Max Thunder and Lightning: can't attack next turn.</summary>
        private sealed class RegielekiVmax : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive) return damage;
                if (!attacker.IsBasic || !src.Engine.IsType(attacker, EnergyType.Lightning)) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Thunder and Lightning") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Painful Spoons: once during your turn, move up to 2 damage counters from 1 of the opponent's Pokémon to another of theirs. Mind Ruler: 20 for each card in their hand.</summary>
        private sealed class RadiantAlakazam : Mon
        {
            protected override string Activated { get { return "Painful Spoons"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.InPlay().Count() >= 2 && ctx.Opp.InPlay().Any(m => m.DamageCounters > 0) && !ctx.Engine.CounterMovementBlocked();
            }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var from = ctx.ChoosePokemon(hurt, "Painful Spoons: move damage counters from which of your opponent's Pokémon?");
                int most = Math.Min(2, from.DamageCounters);
                int n = most == 1 ? 1 : ctx.AskNumber(1, most, "Painful Spoons: move how many damage counters (up to 2)?");
                var to = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != from), "Painful Spoons: move them to which of your opponent's Pokémon?");
                ctx.Engine.MoveDamageCounters(from, to, n, "Painful Spoons");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Ruler") return false;
                ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Psy Call: put up to 2 Stage 1 Pokémon from your deck onto your Bench.</summary>
        private sealed class PsyCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psy Call") return false;
                if (ctx.Me.Deck.Count > 0)
                    SearchAnyStageToBench(ctx, d => d.IsPokemon && d.Stage == Stage.Stage1 && !d.IsVmax && !d.IsVstar, 2, "Psy Call: choose up to 2 Stage 1 Pokémon to put onto your Bench");
                return true;
            }
        }

        /// <summary>Selfish Lips: no Prize cards for the opponent if this Pokémon is Knocked Out by attack damage from their Pokémon V.</summary>
        private sealed class JynxSIT : Mon
        {
            private static readonly Mon psychicAssault = new CardsMEW1.CounterBonusMon("Psychic Assault", 10);

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut != src.Pokemon || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                return knockedOut.HitByAttackerDef != null && knockedOut.HitByAttackerDef.IsPokemonV ? 0 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return psychicAssault.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiteful Magic: Knocked Out from full HP by an opponent's attack damage: 8 damage counters on the Attacking Pokémon. Eerie Voice: 2 counters on each opposing Pokémon.</summary>
        private sealed class MismagiusSIT : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null) return;
                // It had full HP: every damage counter on it came from attack damage this turn.
                bool fullBefore = me.AttackDamageTurn == src.Turn && me.DamageCounters * 10 <= me.AttackDamageThatTurn;
                if (!fullBefore) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                if (attacker == null) return;
                Announce(src, "Spiteful Magic");
                src.Engine.PlaceDamageCounters(attacker, 8, "Spiteful Magic");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Voice") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 2);
                return true;
            }
        }

        /// <summary>Shady Stamp: Confused. Victory Symbol: if you have exactly 1 Prize card left, you win the game.</summary>
        private sealed class UnownV : Mon
        {
            private static readonly Mon shadyStamp = new ConditionAttackMon("Shady Stamp", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Victory Symbol") return shadyStamp.HandleAttack(ctx, atk);
                if (ctx.Me.Prizes.Count == 1) ctx.Engine.EndGame(ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " used Victory Symbol with 1 Prize card left.");
                else ctx.Log("Victory Symbol: " + ctx.Engine.PlayerName(ctx.Player) + " doesn't have exactly 1 Prize card left, so nothing happens.");
                return true;
            }
        }

        /// <summary>Tri Power: 70 for each heads of 3 coins. Star Cipher: this Pokémon gains an Ability that makes the Weakness of each opposing Pokémon Psychic.</summary>
        private sealed class UnownVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tri Power")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                    return true;
                }
                if (atk.Name != "Star Cipher") return false;
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.Engine.AddEffect(new StarCipherEffect(ctx.Pokemon.Name + " gained an Ability: the Weakness of each of the opponent's Pokémon is Psychic (Star Cipher)", ctx.Player, ctx.Pokemon.Uid));
                return true;
            }
        }

        /// <summary>
        /// Star Cipher's gained Ability, until that Unown leaves play: "The Weakness of each of your opponent's Pokémon in play
        /// is now [P]. (The amount of Weakness doesn't change.)" A Pokémon with no Weakness gets Psychic Weakness ×2 (your
        /// ruling, v50). It's an Ability, so Ability blockers switch it off and Lustrous Body keeps it off Corviknight VMAX.
        /// </summary>
        public sealed class StarCipherEffect : ContinuousEffect
        {
            public StarCipherEffect(string name, int owner, int unownUid) : base(name, owner, unownUid, int.MaxValue, false) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                if (mon.Owner == Owner) return weakness;
                var unown = engine.State.FindPokemon(TargetUid);
                if (unown == null || unown.Owner != Owner) return weakness;
                if (engine.AbilityBlocked(unown, "Star Cipher")) return weakness;
                if (engine.EffectsPrevented(mon, EffectSourceKind.Ability, Owner, false)) return weakness;
                return EnergyType.Psychic;
            }
        }

        /// <summary>Star Rondo: from the Bench, switch with your Active Pokémon, then switch in 1 of the opponent's Benched Pokémon. Sudden Eater: 90 more if it moved to the Active Spot this turn.</summary>
        private sealed class MawileVstar : Mon
        {
            protected override string Activated { get { return "Star Rondo"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Pokemon != null && !ctx.PokemonIsActive && ctx.Me.Active != null;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon)) return;
                if (ctx.Opp.Bench.Count > 0) ctx.GustOpponent("Star Rondo: choose 1 of your opponent's Benched Pokémon to switch with their Active Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Eater") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? 90 : 0));
                return true;
            }
        }

        /// <summary>Yoga Guard: while on the Bench, prevent all damage done to this Pokémon by attacks (yours too).</summary>
        private sealed class MedititeSIT : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && !src.PokemonIsActive ? 0 : damage;
            }
        }

        /// <summary>Chakra Awakening: with exactly 4 cards in your hand, this Pokémon's attacks cost [C][C][C] less. Vigorous Kick: 90 more if the opponent has a Pokémon VMAX in play.</summary>
        private sealed class MedichamSIT : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon || src.Me.Hand.Count != 4) return;
                for (int i = 0; i < 3; i++) if (!cost.Remove(EnergyType.Colorless)) break;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vigorous Kick") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.InPlay().Any(m => m.Def.IsVmax) ? 90 : 0));
                return true;
            }
        }

        /// <summary>Dreaming Tone: during the opponent's next turn, the Defending Pokémon falls Asleep if they attach an Energy card from their hand to it.</summary>
        private sealed class DreamingToneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dreaming Tone") return false;
                var defending = ctx.Defending;
                if (defending != null)
                    ctx.AddEffect(new DreamingToneEffect(defending.Name + " falls Asleep if an Energy card is attached to it from the hand during its next turn (Dreaming Tone)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        public sealed class DreamingToneEffect : ContinuousEffect
        {
            public DreamingToneEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target.Uid != TargetUid || target.Owner == Owner || engine.State.CurrentPlayer != target.Owner) return;
                engine.ApplyCondition(target, SpecialCondition.Asleep);
            }
        }

        /// <summary>Warning: put a Basic Pokémon from your deck onto your Bench, or up to 5 if the opponent's Active Pokémon is a Pokémon V.</summary>
        private sealed class WarningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Warning") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                int max = ctx.Defending != null && ctx.Defending.Def.IsPokemonV ? 5 : 1;
                ctx.SearchToBench(d => true, max, "Warning: choose " + (max == 1 ? "a Basic Pokémon" : "up to 5 Basic Pokémon") + " to put onto your Bench");
                return true;
            }
        }

        /// <summary>Cell Spear: 2 damage counters on 1 of the opponent's Benched Pokémon.</summary>
        private sealed class CellSpearMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cell Spear") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count > 0) ctx.PlaceCounters(ctx.ChoosePokemon(bench, "Cell Spear: put 2 damage counters on which Benched Pokémon?"), 2);
                return true;
            }
        }

        /// <summary>Persistent Cells: Knocked Out by an opponent's attack damage, it goes to your hand (attached cards are discarded). Cell Fork: 3 counters on 2 Benched Pokémon.</summary>
        private sealed class ReuniclusSIT : Mon
        {
            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return mon == src.Pokemon && byOpponentsAttack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cell Fork") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var picks = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Cell Fork: choose 2 of your opponent's Benched Pokémon (3 damage counters each)");
                foreach (var mon in picks) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        /// <summary>Psychic Arrow: 60 damage to 1 of the opponent's Pokémon, applying Weakness and Resistance on the Bench too.</summary>
        private sealed class PsychicArrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Arrow") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Psychic Arrow: choose 1 of your opponent's Pokémon to damage");
                ctx.Engine.DealDamage(new DamageInfo
                {
                    Attacker = ctx.Pokemon,
                    Target = target,
                    TargetIsActive = ctx.IsActive(target),
                    ApplyWeakness = true,
                    ApplyResistance = true,
                    WeaknessAndResistanceOnBench = true
                }, 60);
                return true;
            }
        }

        /// <summary>Inviting Ears: when played from the hand to evolve, search your deck for up to 2 Supporter cards.</summary>
        private sealed class MeowsticSIT : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Inviting Ears", ctx.Me.Deck.Count > 0, "Use Inviting Ears to search your deck for up to 2 Supporter cards?")) return;
                ctx.SearchToHand(d => d.IsSupporter, 2, "Inviting Ears: choose up to 2 Supporter cards");
            }
        }

        /// <summary>Dede-Short: only if 1 of your Togedemaru used Toge Dash during your last turn; the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class DedenneSIT : Mon
        {
            private static readonly Mon dedeShort = new ConditionAttackMon("Dede-Short", SpecialCondition.Paralyzed);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Dede-Short" || UsedLastTurn(ctx, "Togedemaru", "Toge Dash");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return dedeShort.HandleAttack(ctx, atk); }
        }

        /// <summary>Smart Service: usable on your first turn if you go first; search your deck for a card.</summary>
        private sealed class IndeedeeSIT : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Smart Service"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smart Service") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Smart Service: choose a card to put into your hand", false, 1);
                return true;
            }
        }

        /// <summary>Dragon Launcher: discard Benched Dreepy (up to the number of opposing Pokémon); 100 damage to a different opposing Pokémon for each, no Weakness or Resistance.</summary>
        private sealed class DragapultSIT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Launcher") return false;
                var dreepy = ctx.MyBench().Where(m => m.Name == "Dreepy").ToList();
                int most = Math.Min(dreepy.Count, ctx.Opp.InPlay().Count());
                if (most == 0) return true;
                var discards = ctx.ChoosePokemonMany(dreepy, 0, most, "Dragon Launcher: discard up to " + most + " of your Benched Dreepy (100 damage to a different opposing Pokémon for each)");
                foreach (var d in discards) ctx.Engine.DiscardPokemonFromPlay(d);
                var hit = new List<PokemonInPlay>();
                for (int i = 0; i < discards.Count; i++)
                {
                    var left = ctx.OpponentPokemon(m => !hit.Contains(m));
                    if (left.Count == 0) break;
                    var target = ctx.ChoosePokemon(left, "Dragon Launcher: do 100 damage to which of your opponent's Pokémon? (" + (i + 1) + " of " + discards.Count + ")");
                    hit.Add(target);
                    ctx.DamageWith(target, 100, false, false, false);
                }
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Irresistible Force: as often as you like during your turn, move a Fighting Energy from your other Pokémon to this Pokémon. Rock Bullet: 30 more for each Fighting Energy.</summary>
        private sealed class HisuianArcanineV : Mon
        {
            private static readonly Mon rockBullet = new CardsMEG.EnergyBonusMon("Rock Bullet", EnergyType.Fighting, 30);

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fighting)));
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return abilityIndex == 0 && ctx.IsMyTurn && ctx.Pokemon != null && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var givers = Givers(ctx);
                if (givers.Count == 0) return;
                var from = ctx.ChoosePokemon(givers, "Irresistible Force: move a Fighting Energy from which Pokémon?");
                var energy = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Fighting)).ToList();
                var pick = ctx.ChooseCards(energy, 1, 1, "Irresistible Force: choose the Energy to move")[0];
                ctx.Engine.MoveEnergy(from, ctx.Pokemon, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return rockBullet.HandleAttack(ctx, atk); }
        }

        /// <summary>Overspin: does nothing if this Pokémon evolved during this turn.</summary>
        private sealed class OverspinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overspin") return false;
                if (ctx.Pokemon.TurnEvolved == ctx.Turn) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Mystery Charge: with no Supporter cards in your discard pile, once during your turn attach a basic Fighting Energy from your discard pile to 1 of your Pokémon.</summary>
        private sealed class ClaydolSIT : Mon
        {
            protected override string Activated { get { return "Mystery Charge"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoSupporterInDiscard(ctx) && ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var fighting = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                ctx.AttachCardsAnyWay(fighting, 1, (card, m) => true, "Mystery Charge", false);
            }
        }

        /// <summary>Fossil Drop: you may discard an Unidentified Fossil from your hand for 120 more damage.</summary>
        private sealed class FossilDropMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Drop") return false;
                var fossil = ctx.Me.Hand.FirstOrDefault(c => c.Def.Name == "Unidentified Fossil");
                bool used = fossil != null && ctx.AskYesNo("Fossil Drop: discard an Unidentified Fossil from your hand for 120 more damage?");
                if (used) ctx.Engine.DiscardFromHand(ctx.Player, fossil);
                ctx.DamageDefending(atk.Damage + (used ? 120 : 0));
                return true;
            }
        }

        /// <summary>Reaping Claw: if the opponent's Active Pokémon has 100 HP or less remaining, it is Knocked Out.</summary>
        private sealed class ReapingClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reaping Claw") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.RemainingHp(defending) <= 100) ctx.Engine.KnockOut(defending, "Reaping Claw");
                else ctx.Log("Reaping Claw: " + (defending == null ? "no Defending Pokémon" : defending.Name + " has more than 100 HP left") + ".");
                return true;
            }
        }

        /// <summary>Cavern Tackle: can't be used if 1 of your Pokémon used it during your last turn; prevent all damage done to this Pokémon by attacks next turn.</summary>
        private sealed class CavernTackleMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Cavern Tackle" || !UsedLastTurn(ctx, null, "Cavern Tackle");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cavern Tackle") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new PreventDamageEffect("Prevent all damage done to " + ctx.Pokemon.Name + " by attacks during the opponent's next turn (Cavern Tackle)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Mystery Press: 130 more damage if you have no Supporter cards in your discard pile.</summary>
        private sealed class MysteryPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mystery Press") return false;
                ctx.DamageDefending(atk.Damage + (NoSupporterInDiscard(ctx) ? 130 : 0));
                return true;
            }
        }

        /// <summary>Hard Coat: takes 30 less damage from attacks. Peat Shoulder: 10 less damage for each damage counter on this Pokémon.</summary>
        private sealed class UrsalunaV : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peat Shoulder") return false;
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Critical Bite: 30 damage to 1 of the opponent's Pokémon; if it's Knocked Out by this damage, take 2 more Prize cards.</summary>
        private sealed class CriticalBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Critical Bite") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Critical Bite: choose 1 of your opponent's Pokémon to damage");
                int done = ctx.DamageTarget(target, 30);
                if (done > 0 && ctx.Engine.IsKnockedOut(target))
                    ctx.State.Effects.Add(new KnockOutPrizeBonusEffect("Critical Bite: take 2 more Prize cards for " + target.Name, ctx.Player, target.Uid, ctx.Turn, 2));
                return true;
            }
        }

        /// <summary>Flock: put up to 2 Murkrow from your deck onto your Bench.</summary>
        private sealed class FlockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flock") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.Name == "Murkrow", 2, "Flock: choose up to 2 Murkrow to put onto your Bench");
                return true;
            }
        }

        /// <summary>Callous Wings: you may switch this Pokémon with 1 of your Benched Murkrow.</summary>
        private sealed class CallousWingsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Callous Wings") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || !ctx.PokemonIsActive) return true;
                var murkrow = ctx.MyBench().Where(m => m.Name == "Murkrow").ToList();
                if (murkrow.Count == 0 || !ctx.AskYesNo("Callous Wings: switch " + ctx.Pokemon.Name + " with 1 of your Benched Murkrow?")) return true;
                var target = ctx.ChoosePokemon(murkrow, "Callous Wings: switch with which Murkrow?");
                ctx.Engine.SwitchActive(ctx.Player, target);
                return true;
            }
        }

        /// <summary>Pursuit Blast: 30 damage to 1 of the opponent's Benched Pokémon, or 120 if it retreated during their last turn.</summary>
        private sealed class PursuitBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pursuit Blast") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Pursuit Blast: choose 1 of your opponent's Benched Pokémon (120 damage if it retreated during their last turn)");
                ctx.DamageTarget(target, target.RetreatedOnTurn == ctx.OpponentsLastTurn ? 120 : 30);
                return true;
            }
        }

        /// <summary>Diving Uppercut: during the opponent's next turn, this Pokémon takes 50 more damage from attacks.</summary>
        private sealed class DivingUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diving Uppercut") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new DamageTakenEffect(ctx.Pokemon.Name + " takes 50 more damage from attacks during the opponent's next turn (Diving Uppercut)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 50));
                return true;
            }
        }

        /// <summary>Bully of the Sands: when played to evolve, or when Knocked Out in the Active Spot by an opponent's attack damage, you may discard a random card from the opponent's hand.</summary>
        private sealed class KrookodileSIT : Mon
        {
            private static readonly Mon doubleEdge = new RecoilMon("Double-Edge", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Bully of the Sands", ctx.Opp.Hand.Count > 0, "Use Bully of the Sands to discard a random card from your opponent's hand?")) return;
                CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
            }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Opp.Hand.Count == 0) return;
                if (!src.AskYesNo("Bully of the Sands: discard a random card from your opponent's hand?")) return;
                Announce(src, "Bully of the Sands");
                CardsASC.DiscardRandomFromOpponentHand(src, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return doubleEdge.HandleAttack(ctx, atk); }
        }

        /// <summary>Peko Blaster: only if 1 of your Dedenne used Dede-Short during your last turn; 60 damage to each of the opponent's Pokémon.</summary>
        private sealed class PekoBlasterMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Peko Blaster" || UsedLastTurn(ctx, "Dedenne", "Dede-Short");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peko Blaster") return false;
                CardsCRZ1.DamageEachOpponent(ctx, 60);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Magnetic Lift: search your deck for a card, shuffle, then put it on top.</summary>
        private sealed class MagneticLiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Lift") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => true, 1, "Magnetic Lift: choose a card to put on top of your deck", false, 1);
                ctx.ShuffleMyDeck();
                if (found.Count > 0) ctx.Me.Deck.Insert(0, found[0]);
                return true;
            }
        }

        /// <summary>Emergency Entry: drawn for the turn with room on the Bench, put it onto your Bench and draw 3 cards. Meteor Mash: 100 more next turn.</summary>
        private sealed class MetagrossSIT : Mon
        {
            private static readonly Mon meteorMash = new CardsASC.NextTurnBoostMon("Meteor Mash", 100);

            public override void OnDrawnForTurn(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0 || ctx.NamedLimitUsedThisTurn("Emergency Entry")) return;
                if (!ctx.AskYesNo("Emergency Entry: you drew Metagross. Put it onto your Bench and draw 3 cards?")) return;
                if (!ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit("Emergency Entry");
                ctx.Engine.PushSource(EffectSourceKind.Ability, ctx.Player, null);
                try
                {
                    var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                    ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Metagross used its Ability Emergency Entry.", ctx.Card.Id, mon.Uid);
                    ctx.Draw(3);
                }
                finally { ctx.Engine.PopSource(); }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return meteorMash.HandleAttack(ctx, atk); }
        }

        /// <summary>Entrusted Wishes: Knocked Out in the Active Spot by an opponent's attack damage, search your deck for up to 3 cards. Astral Misfortune: 2 heads Knocks Out the Defending Pokémon.</summary>
        private sealed class RadiantJirachi : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Me.Deck.Count == 0) return;
                Announce(src, "Entrusted Wishes");
                src.SearchToHand(d => true, 3, "Entrusted Wishes: choose up to 3 cards to put into your hand", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Astral Misfortune") return false;
                var defending = ctx.Defending;
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, "Astral Misfortune");
                return true;
            }
        }

        /// <summary>Triple Gears: when played to evolve, search for up to 3 basic Energy and attach them to your Pokémon in any way you like.</summary>
        private sealed class KlinklangSIT : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Triple Gears", ctx.Me.Deck.Count > 0, "Use Triple Gears to search your deck for up to 3 basic Energy and attach them to your Pokémon?")) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Triple Gears: choose up to 3 basic Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, m) => true, "Triple Gears");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Justified Law: your Basic Pokémon's attacks do 30 more damage to the opponent's Active Darkness Pokémon. Follow-Up: a basic Energy from the deck to each of up to 2 Benched Pokémon.</summary>
        private sealed class CobalionSIT : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || !attacker.IsBasic) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Darkness) ? damage + 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Follow-Up") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(bench, 0, Math.Min(2, bench.Count), "Follow-Up: choose up to 2 of your Benched Pokémon (a basic Energy from your deck for each)");
                foreach (var mon in picks)
                {
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Follow-Up: choose a basic Energy card to attach to " + mon.Name);
                    foreach (var c in found) ctx.Engine.AttachEnergyCard(c, mon);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Toge Dash: flip a coin; on heads, the opponent takes no Prize cards if this Pokémon is Knocked Out during their next turn.</summary>
        private sealed class TogeDashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toge Dash") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new NoPrizeEffect("No Prize cards for " + ctx.Pokemon.Name + " if it's Knocked Out during the opponent's next turn (Toge Dash)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        public sealed class NoPrizeEffect : ContinuousEffect
        {
            public NoPrizeEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return knockedOut.Uid == TargetUid && taker != Owner ? 0 : prizes;
            }
        }

        // ================================================================== Dragon

        /// <summary>Energy Hurricane: search for up to 3 basic Energy and attach them to your Pokémon in any way you like.</summary>
        private sealed class EnergyHurricaneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Hurricane") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Energy Hurricane: choose up to 3 basic Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, m) => true, "Energy Hurricane");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Radiant Hunt: Knock Out 1 of the opponent's Radiant Pokémon.</summary>
        private sealed class RadiantHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Radiant Hunt") return false;
                var radiant = ctx.OpponentPokemon(m => m.Def.IsRadiant);
                if (radiant.Count == 0) return true;
                var target = ctx.ChoosePokemon(radiant, "Radiant Hunt: Knock Out which Radiant Pokémon?");
                ctx.Engine.KnockOut(target, "Radiant Hunt");
                return true;
            }
        }

        /// <summary>Shout of Power: attach a basic Energy from your discard pile to 1 of your Benched Pokémon.</summary>
        internal sealed class ShoutOfPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shout of Power") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0) ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 1, (card, m) => !ctx.IsActive(m), "Shout of Power", false);
                return true;
            }
        }

        /// <summary>Celestial Roar: discard the top 3 cards of your deck and attach any Energy cards among them to this Pokémon.</summary>
        private sealed class CelestialRoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Celestial Roar") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                foreach (var c in milled)
                {
                    if (!c.Def.IsEnergy || !ctx.InPlay(ctx.Pokemon) || !ctx.Me.Discard.Remove(c)) continue;
                    ctx.Engine.AttachEnergyCard(c, ctx.Pokemon);
                }
                return true;
            }
        }

        /// <summary>Legacy Star: discard the top 7 cards of your deck, then put up to 2 cards from your discard pile into your hand. Apex Dragon: use an attack of a Dragon Pokémon in your discard pile.</summary>
        private sealed class RegidragoVstar : Mon
        {
            protected override string Activated { get { return "Legacy Star"; } }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 7);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(2, pile.Count), "Legacy Star: put up to 2 cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Apex Dragon") return false;
                var opts = new List<DecisionOption>();
                var seen = new HashSet<string>();
                foreach (var c in ctx.Me.Discard)
                {
                    var d = c.Def;
                    if (!d.IsPokemon || !d.HasType(EnergyType.Dragon) || !seen.Add(d.BaseKey)) continue;
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        if (a.VstarPower && ctx.Me.VstarUsed) continue;
                        if (ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                }
                if (opts.Count == 0) return Nothing(ctx);
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Apex Dragon: choose an attack of a Dragon Pokémon in your discard pile", opts, ctx.Source);
                UseCopiedAttack(ctx, opts[pick].Card, opts[pick].Index);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Summoning Star: put up to 2 Colorless Pokémon without a Rule Box from your discard pile onto your Bench. Tempest Dive: you may discard a Stadium.</summary>
        private sealed class LugiaVstar : Mon
        {
            private static readonly Mon tempestDive = new MayDiscardStadiumMon("Tempest Dive");

            private static List<CardInstance> Targets(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon && !c.Def.HasRuleBox && c.Def.HasType(EnergyType.Colorless)).ToList();
            }

            protected override string Activated { get { return "Summoning Star"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.BenchSpace > 0 && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pile = Targets(ctx);
                int n = Math.Min(2, Math.Min(ctx.BenchSpace, pile.Count));
                if (n <= 0) return;
                var picks = ctx.ChooseCards(pile, 0, n, "Summoning Star: put up to 2 Colorless Pokémon without a Rule Box from your discard pile onto your Bench");
                foreach (var c in picks)
                {
                    if (ctx.BenchSpace <= 0 || !ctx.Me.Discard.Remove(c)) continue;
                    ctx.Engine.PutOntoBench(ctx.Player, c, false);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return tempestDive.HandleAttack(ctx, atk); }
        }

        /// <summary>Reviving Flame: once during your turn, from your discard pile onto your Bench with up to 4 basic Energy from the discard pile; your turn ends. Rainbow Burn: 30 more for each type of basic Energy attached.</summary>
        private sealed class HoOhV : Mon
        {
            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.BenchSpace > 0 && !ctx.NamedLimitUsedThisTurn("Reviving Flame#" + ctx.Card.Id);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit("Reviving Flame#" + ctx.Card.Id);
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Ho-Oh V used its Ability Reviving Flame.", ctx.Card.Id, mon.Uid);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, Math.Min(4, energy.Count), "Reviving Flame: attach up to 4 basic Energy cards from your discard pile to Ho-Oh V");
                    foreach (var c in picks)
                        if (ctx.Me.Discard.Remove(c)) ctx.Engine.AttachEnergyCard(c, mon);
                }
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Burn") return false;
                var types = new HashSet<EnergyType>();
                foreach (var e in ctx.Pokemon.Energy) if (e.Def.IsBasicEnergy) types.Add(e.Def.EnergyProvides);
                ctx.DamageDefending(atk.Damage + 30 * types.Count);
                return true;
            }
        }

        /// <summary>Dizzying Spin: 10 damage to each of the opponent's Pokémon, and their Active Pokémon is Confused.</summary>
        private sealed class DizzyingSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dizzying Spin") return false;
                var defending = ctx.Defending;
                CardsCRZ1.DamageEachOpponent(ctx, 10);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Primal Turbo: once during your turn, search for up to 2 Special Energy cards and attach them to 1 of your Pokémon.</summary>
        private sealed class ArcheopsSIT : Mon
        {
            protected override string Activated { get { return "Primal Turbo"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsSpecialEnergy, 2, "Primal Turbo: choose up to 2 Special Energy cards");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Primal Turbo: attach them to which Pokémon?");
                    foreach (var c in found) ctx.Engine.AttachEnergyCard(c, target);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Eerie Cry: 3 damage counters on each of the opponent's Pokémon that has any damage counters on it.</summary>
        private sealed class EerieCryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Cry") return false;
                foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0)) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }
    }
}
