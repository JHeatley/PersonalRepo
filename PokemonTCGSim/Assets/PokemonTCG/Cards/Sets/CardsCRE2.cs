using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Chilling Reign (CRE, 2021): Pokémon 98-128 (Darkness, Metal, Colorless), the secret-rare Electrode and Snorlax (Vivid
    /// Voltage reprints, regulation mark D), the Trainers (with the gold Fan of Waves and Urn of Vitality from Battle Styles)
    /// and the Special Energy. Regulation mark E: Expanded only.
    /// Pokémon whose attacks only do their printed damage need no script (Aron, Lairon, Porygon).
    /// Your rulings (v59): Urn of Vitality only takes cards named Single Strike Energy; Greedent's Brazen Tail doesn't stop the
    /// opponent's Pokémon Tools (Rugged Helmet still works), only their Items played from the hand and Supporters.
    /// </summary>
    public static class CardsCRE2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Darkness
            r.RegisterByKey("CRE-98", new GalarianSlowkingCRE());
            r.RegisterByKey("CRE-99", new ComboMon(new ConcoctionMon(), new WordOfRuinMon()));
            r.RegisterByKey("CRE-100", new CardsBS1.ToxicMon("Max Toxify", 12));
            r.RegisterByKey("CRE-101", new QwilfishCRE());
            r.RegisterByKey("CRE-102", new CardsFST2.ConditionBonusMon("Strong Tail", 90, PlayedSingleStrikeSupporter));
            r.RegisterByKey("CRE-103", new GhostlyCriesMon());
            r.RegisterByKey("CRE-104", new LiepardV());
            r.RegisterByKey("CRE-105", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("CRE-106", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("CRE-107", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new CardsBLK.VenoshockMon(120)));
            r.RegisterByKey("CRE-108", new ComboMon(new FieldCrushMon(), new CardsASC.SelfDamagedBonusMon("Fists of Strife", 100)));
            // Metal
            r.RegisterByKey("CRE-111", new ComboMon(new ShieldAttackMon("Guard Press", 30), new SeismicRiftMon()));
            r.RegisterByKey("CRE-112", new ComboMon(new CardsWHT.FlipMoreMon("Bullet Punch", 2, 20), new CardsFST2.ConditionBonusMon("Synchro Hammer", 90, SameEnergyAsDefending)));
            r.RegisterByKey("CRE-113", new ComboMon(new SearchAttackMon("Zap Traction", CardsEVS1.AnyCard, 2), new CardsASC.NextTurnBoostMon("Max Rush", 150)));
            r.RegisterByKey("CRE-114", new LockAfterAttackMon("Metal Slash", false));
            // Colorless
            r.RegisterByKey("CRE-115", new ComboMon(new RagingBullMon(), new RecoilMon("Take Down", 30)));
            r.RegisterByKey("CRE-117", new CoinsMon("Tri Attack", 3));
            r.RegisterByKey("CRE-118", new PorygonZCRE());
            r.RegisterByKey("CRE-120", new GaleClawsMon());
            r.RegisterByKey("CRE-122", new KecleonCRE());
            r.RegisterByKey("CRE-123", new ComboMon(new CardsMEG.DrawUntilSixMon("Return"), new CardsPAF1.ReturnSelfMon("Sky Return")));
            r.RegisterByKey("CRE-124", new ComboMon(new CardsFST2.ConditionBonusMon("Blow Through", 20, StadiumInPlay), new CardsTWM1.SelfDiscardMon("Blasting Hammer", 1)));
            r.RegisterByKey("CRE-125", new MaxWindSpiritMon());
            r.RegisterByKey("CRE-126", new ComboMon(new SearchAttackMon("Find a Friend", IsPokemonCard, 1), new ShieldAttackMon("Fur Attack", 20)));
            r.RegisterByKey("CRE-127", new ScroungeMon());
            r.RegisterByKey("CRE-128", new GreedentCRE());
            // Secret rares new to the pool
            r.RegisterByKey("CRE-222", new ElectrodeCRE());
            r.RegisterByKey("CRE-224", new SnorlaxCRE());

            // Trainers
            r.RegisterByName("Agatha", new Agatha());
            r.RegisterByName("Avery", new Avery());
            r.RegisterByName("Brawly", new Brawly());
            r.RegisterByName("Caitlin", new Caitlin());
            r.RegisterByName("Crushing Gloves", new CardsBRS2.TypeGloves(EnergyType.Metal));
            r.RegisterByName("Doctor", new Doctor());
            r.RegisterByName("Dyna Tree Hill", new DynaTreeHill());
            r.RegisterByName("Echoing Horn", new EchoingHorn());
            r.RegisterByName("Expedition Uniform", new ExpeditionUniform());
            r.RegisterByName("Fan of Waves", new FanOfWaves());
            r.RegisterByName("Fire-Resistant Gloves", new CardsBRS2.TypeGloves(EnergyType.Fire));
            r.RegisterByName("Flannery", new Flannery());
            r.RegisterByName("Fog Crystal", new FogCrystal());
            r.RegisterByName("Galarian Chestplate", new GalarianChestplate());
            r.RegisterByName("Honey", new Honey());
            r.RegisterByName("Justified Gloves", new CardsBRS2.TypeGloves(EnergyType.Darkness));
            r.RegisterByName("Karen's Conviction", new KarensConviction());
            r.RegisterByName("Klara", new Klara());
            r.RegisterByName("Old Cemetery", new OldCemetery());
            r.RegisterByName("Peonia", new Peonia());
            r.RegisterByName("Peony", new Peony());
            r.RegisterByName("Rapid Strike Scroll of the Skies", new CreScroll(false));
            r.RegisterByName("Rugged Helmet", new RuggedHelmet());
            r.RegisterByName("Siebold", new Siebold());
            r.RegisterByName("Single Strike Scroll of Piercing", new CreScroll(true));
            r.RegisterByName("Urn of Vitality", new UrnOfVitality());
            r.RegisterByName("Weeding Gloves", new CardsBRS2.TypeGloves(EnergyType.Grass));
            r.RegisterByName("Welcoming Lantern", new WelcomingLantern());
            // Energy
            r.RegisterByName("Impact Energy", new StrikeEnergy(true));
            r.RegisterByName("Lucky Energy", new LuckyEnergy());
            r.RegisterByName("Spiral Energy", new StrikeEnergy(false));
        }

        // ================================================================== Filters

        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static bool StadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null; }

        /// <summary>"Single Strike cards": cards with Single Strike on them.</summary>
        internal static bool IsSingleStrikeCard(CardDef d) { return d.IsSingleStrike || d.Name.IndexOf("Single Strike", StringComparison.Ordinal) >= 0; }

        private static bool PlayedSingleStrikeSupporter(EffectContext ctx)
        {
            var sup = ctx.Me.SupporterThisTurn;
            return sup != null && IsSingleStrikeCard(sup);
        }

        private static bool SameEnergyAsDefending(EffectContext ctx)
        {
            return ctx.Defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(ctx.Defending);
        }

        // ================================================================== Darkness

        /// <summary>Mysterious Potion: once during your turn, choose 1 of your Pokémon and flip a coin; heads heal 90 damage from it, tails put 3 damage counters on it.</summary>
        private sealed class GalarianSlowkingCRE : Mon
        {
            protected override string Activated { get { return "Mysterious Potion"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Mysterious Potion: choose 1 of your Pokémon (heads: heal 90, tails: 3 damage counters)");
                if (target == null) return;
                if (ctx.Flip()) ctx.Heal(target, 90);
                else ctx.PlaceCounters(target, 3);
            }
        }

        /// <summary>Concoction: discard a card from your hand; if you do, draw 3 cards.</summary>
        private sealed class ConcoctionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Concoction") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var pick = ctx.ChooseCards(hand, 1, 1, "Concoction: discard a card from your hand, then draw 3 cards");
                if (pick.Count == 0) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Word of Ruin: at the end of your opponent's next turn, the Defending Pokémon is Knocked Out.</summary>
        private sealed class WordOfRuinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Word of Ruin") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of your opponent's next turn (Word of Ruin)",
                    ctx.Player, defending.Uid, ctx.Turn, "Word of Ruin", 0));
                return true;
            }
        }

        /// <summary>
        /// Bursting Needles: if this Pokémon is in the Active Spot and is Knocked Out by damage from an attack from the
        /// opponent's Pokémon, put 6 damage counters on the Attacking Pokémon. Poison Jab: Poisoned.
        /// </summary>
        private sealed class QwilfishCRE : Mon
        {
            private static readonly Mon PoisonJab = new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player) return;
                Announce(src, "Bursting Needles");
                src.Engine.PlaceDamageCounters(attacker, 6, "Bursting Needles");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PoisonJab.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Ghostly Cries: 1 damage counter on the opponent's Pokémon (in any way you like) for each Pokémon in their discard
        /// pile; if any were placed, they shuffle all Pokémon from their discard pile into their deck.
        /// </summary>
        private sealed class GhostlyCriesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ghostly Cries") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int count = ctx.Opp.Discard.Count(c => c.Def.IsPokemon);
                if (count == 0) return true;
                int before = ctx.OpponentPokemon().Sum(m => m.DamageCounters);
                ctx.PlaceCountersAnyWay(count, () => ctx.OpponentPokemon());
                int after = ctx.OpponentPokemon().Sum(m => m.DamageCounters);
                if (after <= before) return true;
                var opp = ctx.Opp;
                var mons = opp.Discard.Where(c => c.Def.IsPokemon).ToList();
                foreach (var card in mons)
                {
                    opp.Discard.Remove(card);
                    opp.Deck.Add(card);
                }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Ghostly Cries: " + ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + mons.Count + " Pokémon from their discard pile into their deck.");
                return true;
            }
        }

        /// <summary>
        /// Hidden Claw: when you play this Pokémon from your hand onto your Bench during your turn, you may discard a Pokémon
        /// Tool from a Pokémon (yours or your opponent's). Shadow Ripper: you may put this Pokémon and all attached cards into
        /// your hand.
        /// </summary>
        private sealed class LiepardV : Mon
        {
            private static List<PokemonInPlay> ToolHolders(EffectContext ctx)
            {
                var list = ctx.MyPokemon(m => m.Tools.Count > 0);
                list.AddRange(ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m)));
                return list;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Hidden Claw", ToolHolders(ctx).Count > 0, "Use Hidden Claw? Discard a Pokémon Tool from a Pokémon (yours or your opponent's).")) return;
                var holders = ToolHolders(ctx);
                if (holders.Count == 0) return;
                var holder = ctx.ChoosePokemon(holders, "Hidden Claw: discard a Pokémon Tool from which Pokémon?");
                if (holder == null) return;
                var tool = holder.Tools.Count == 1 ? holder.Tools[0] : ctx.ChooseCards(holder.Tools.ToList(), 1, 1, "Hidden Claw: discard which Pokémon Tool?")[0];
                ctx.Engine.DiscardTool(holder, tool);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Ripper") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.AskYesNo("Shadow Ripper: put " + me.Name + " and all attached cards into your hand?"))
                    ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Field Crush: if your opponent has a Stadium in play, discard it.</summary>
        internal sealed class FieldCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Field Crush") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Seismic Rift: also 30 damage to each of your Pokémon, Aggron included (your ruling, v59).</summary>
        private sealed class SeismicRiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seismic Rift") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DamageSelf(30);
                foreach (var mon in ctx.MyBench()) if (mon != me) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Raging Bull: 20 more damage for each damage counter on this Pokémon; this Pokémon is now Confused.</summary>
        private sealed class RagingBullMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Bull") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 20 * me.DamageCounters);
                if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.Engine.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Bug Transmission: when you attach an Energy card from your hand to this Pokémon during your turn, you may make the
        /// opponent's Active Pokémon Confused. Superbeam: discard 2 Energy from this Pokémon.
        /// </summary>
        private sealed class PorygonZCRE : Mon
        {
            private static readonly Mon Superbeam = new CardsTWM1.SelfDiscardMon("Superbeam", 2);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target != src.Pokemon || !src.IsMyTurn) return;
                var theirs = src.Opp.Active;
                if (theirs == null || !src.CanAffect(theirs)) return;
                if (!src.AskYesNo("Bug Transmission: make " + theirs.Name + " Confused?")) return;
                Announce(src, "Bug Transmission");
                src.ApplyCondition(theirs, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Superbeam.HandleAttack(ctx, atk); }
        }

        /// <summary>Gale Claws: if you played a Rapid Strike Supporter card from your hand this turn, also 50 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class GaleClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gale Claws") return false;
                ctx.DamageDefending(atk.Damage);
                var sup = ctx.Me.SupporterThisTurn;
                if (sup == null || !CardsEVS1.IsRapidStrikeCard(sup)) return true;
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, "Gale Claws: choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(target, 50);
                return true;
            }
        }

        /// <summary>Chromashift: this Pokémon is the type of each basic Energy attached to it (several types with several kinds).</summary>
        private sealed class KecleonCRE : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                var kinds = mon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().ToList();
                if (kinds.Count == 0) return;
                types.Clear();
                types.AddRange(kinds);
            }
        }

        /// <summary>Max Wind Spirit: if a Stadium is in play, 120 more damage; then discard that Stadium.</summary>
        private sealed class MaxWindSpiritMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Wind Spirit") return false;
                var stadium = ctx.State.Stadium;
                ctx.DamageDefending(atk.Damage + (stadium != null ? 120 : 0));
                if (stadium != null && ctx.State.Stadium == stadium) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Scrounge: put a Pokémon Tool card from your discard pile into your hand.</summary>
        private sealed class ScroungeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrounge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
                if (tools.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(tools, 1, 1, "Scrounge: put a Pokémon Tool card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>
        /// Brazen Tail: Energy attached to your Pokémon can't be put into a hand, a deck or the discard pile by effects of your
        /// opponent's Item or Supporter cards.
        /// </summary>
        private sealed class GreedentCRE : Mon
        {
            public override bool ProtectsEnergyFromTrainerRemoval(EffectContext src, PokemonInPlay holder, CardInstance energy)
            {
                return src.Pokemon != null && holder != null && holder.Owner == src.Player;
            }
        }

        /// <summary>
        /// Buzzap Generator: once during your turn, if this Pokémon is on your Bench, search your deck for up to 2 Lightning
        /// Energy cards and attach them to your Lightning Pokémon in any way you like; then this Pokémon is Knocked Out.
        /// </summary>
        private sealed class ElectrodeCRE : Mon
        {
            protected override string Activated { get { return "Buzzap Generator"; } }
            protected override bool AbilityReady(EffectContext ctx) { return !ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }
            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Buzzap Generator"; }

            private static bool IsLightningEnergy(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Lightning); }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(IsLightningEnergy, 2, "Buzzap Generator: search your deck for up to 2 Lightning Energy cards");
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, LightningTarget(ctx), "Buzzap Generator", false);
                foreach (var card in found)
                    if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card)) && !ctx.Me.Deck.Contains(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                ctx.Engine.KnockOut(ctx.Pokemon, "Buzzap Generator");
            }

            private static Func<CardInstance, PokemonInPlay, bool> LightningTarget(EffectContext ctx)
            {
                return (card, mon) => ctx.Engine.IsType(mon, EnergyType.Lightning);
            }
        }

        /// <summary>Gormandize: once during your turn, if this Pokémon is Active, draw until you have 7 cards; your turn ends. Body Slam: flip for Paralysis.</summary>
        private sealed class SnorlaxCRE : Mon
        {
            private static readonly Mon BodySlam = new ParalyzeFlipMon("Body Slam");

            protected override string Activated { get { return "Gormandize"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Hand.Count < 7 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.DrawUntil(7);
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BodySlam.HandleAttack(ctx, atk); }
        }

        // ================================================================== Supporters

        /// <summary>Agatha: move up to 3 damage counters from your Active Pokémon to your opponent's Active Pokémon.</summary>
        private sealed class Agatha : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0 && ctx.Opp.Active != null;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mine = ctx.Me.Active;
                var theirs = ctx.Opp.Active;
                if (mine == null || theirs == null || mine.DamageCounters == 0) return;
                int max = Math.Min(3, mine.DamageCounters);
                int n = max == 1 ? 1 : ctx.AskNumber(1, max, "Agatha: move how many damage counters from " + mine.Name + " to " + theirs.Name + "?");
                ctx.Engine.MoveDamageCounters(mine, theirs, n, "Agatha");
            }
        }

        /// <summary>Avery: draw 3 cards; if you drew any, your opponent discards Benched Pokémon (their choice) until they have 3.</summary>
        private sealed class Avery : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Draw(3) == 0) return;
                // Milotic's Dew Guard stops the Bench discard (Compendium 2021, v63).
                if (TefPokemon.OpponentShielded(ctx)) return;
                var opp = ctx.Opp;
                for (int guard = 0; opp.Bench.Count > 3 && guard < 10; guard++)
                {
                    var mon = ctx.Engine.ChoosePokemon(ctx.Opponent, opp.Bench.ToList(), "Avery: discard a Benched Pokémon (down to 3)", ctx.Source);
                    if (mon == null) break;
                    ctx.Engine.DiscardPokemonFromPlay(mon);
                }
            }
        }

        /// <summary>Brawly: search your deck for up to 3 Basic Rapid Strike Pokémon and put them onto your Bench.</summary>
        private sealed class Brawly : CardScript
        {
            private static bool IsRapid(CardDef d) { return d.IsRapidStrike; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToBench(IsRapid, 3, "Brawly: up to 3 Basic Rapid Strike Pokémon for your Bench");
            }
        }

        /// <summary>Caitlin: put any number of cards from your hand on the bottom of your deck in any order, then draw that many.</summary>
        private sealed class Caitlin : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, hand.Count, "Caitlin: put any number of cards from your hand on the bottom of your deck (in the order you pick them)");
                foreach (var card in pick)
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Count + (pick.Count == 1 ? " card" : " cards") + " on the bottom of their deck (Caitlin).");
                ctx.Draw(pick.Count);
            }
        }

        /// <summary>Doctor: draw 2 cards; 2 more if the opponent's Active Pokémon is a Pokémon VMAX.</summary>
        private sealed class Doctor : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool vmax = ctx.Opp.Active != null && ctx.Opp.Active.Def.IsVmax;
                ctx.Draw(vmax ? 4 : 2);
            }
        }

        /// <summary>Flannery: discard a Special Energy from 1 of your opponent's Pokémon, and discard a Stadium in play.</summary>
        private sealed class Flannery : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 || ctx.State.Stadium != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count > 0)
                {
                    var holder = ctx.ChoosePokemon(targets, "Flannery: discard a Special Energy from which of your opponent's Pokémon?");
                    var specials = holder.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                    var energy = ctx.ChooseCards(specials, 1, 1, "Flannery: discard which Special Energy?")[0];
                    ctx.Engine.DiscardEnergy(holder, energy);
                }
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
            }
        }

        /// <summary>Honey: draw a card for each of your opponent's Benched Pokémon V.</summary>
        private sealed class Honey : CardScript
        {
            private static int Count(EffectContext ctx) { return ctx.Opp.Bench.Count(m => m.Def.IsPokemonV); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Count(ctx) > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(Count(ctx)); }
        }

        /// <summary>
        /// Karen's Conviction: this turn, your Single Strike Pokémon's attacks do 20 more damage to the opponent's Active Pokémon
        /// for each Prize card the opponent has taken (before Weakness and Resistance).
        /// </summary>
        private sealed class KarensConviction : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int bonus = 20 * ctx.OpponentPrizesTaken;
                if (bonus <= 0)
                {
                    ctx.Log("Karen's Conviction: your opponent hasn't taken any Prize cards yet.");
                    return;
                }
                ctx.AddEffect(new ConditionalDamageBonusEffect("Karen's Conviction: Single Strike Pokémon's attacks do " + bonus + " more damage this turn",
                    ctx.Player, ctx.Turn, bonus, SingleStrikeAttacker));
            }

            private static bool SingleStrikeAttacker(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target)
            {
                return attacker != null && attacker.Def.IsSingleStrike;
            }
        }

        /// <summary>Klara: choose 1 or both: up to 2 Pokémon, and/or up to 2 basic Energy cards, from your discard pile into your hand.</summary>
        private sealed class Klara : CardScript
        {
            private static List<CardInstance> Pokemon(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pokemon(ctx).Count > 0 || ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = Pokemon(ctx);
                var energy = ctx.BasicEnergyInDiscard();
                bool takeMons = mons.Count > 0;
                bool takeEnergy = energy.Count > 0;
                if (takeMons && takeEnergy)
                {
                    int choice = ctx.ChooseOption("Klara: choose 1 or both", "Both", "Up to 2 Pokémon", "Up to 2 basic Energy cards");
                    takeMons = choice != 2;
                    takeEnergy = choice != 1;
                }
                var picked = new List<CardInstance>();
                if (takeMons) picked.AddRange(ctx.ChooseCards(mons, 1, Math.Min(2, mons.Count), "Klara: put up to 2 Pokémon from your discard pile into your hand"));
                if (takeEnergy) picked.AddRange(ctx.ChooseCards(energy, 1, Math.Min(2, energy.Count), "Klara: put up to 2 basic Energy cards from your discard pile into your hand"));
                ctx.DiscardToHand(picked);
            }
        }

        /// <summary>
        /// Peonia: put up to 3 of your Prize cards into your hand (taken face down, so at random), then put a card from your
        /// hand face down as a Prize card for each one.
        /// </summary>
        private sealed class Peonia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Prizes.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                int max = Math.Min(3, me.Prizes.Count);
                if (max == 0) return;
                int n = max == 1 ? 1 : ctx.AskNumber(1, max, "Peonia: put how many Prize cards into your hand?");
                var slots = me.PrizeSlotList();
                var freed = new List<int>();
                var taken = new List<CardInstance>();
                for (int k = 0; k < n && me.Prizes.Count > 0; k++)
                {
                    int index = ctx.State.Rng.Next(me.Prizes.Count);
                    var card = me.Prizes[index];
                    freed.Add(slots[index]);
                    slots.RemoveAt(index);
                    me.Prizes.RemoveAt(index);
                    me.FaceUpPrizeIds.Remove(card.Id);
                    me.PrizeSlotByCard.Remove(card.Id);
                    me.Hand.Add(card);
                    taken.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + taken.Count + (taken.Count == 1 ? " Prize card" : " Prize cards") + " into their hand (Peonia).");
                int back = Math.Min(taken.Count, me.Hand.Count);
                if (back == 0) return;
                var pick = ctx.ChooseCards(me.Hand.ToList(), back, back, "Peonia: put " + back + (back == 1 ? " card" : " cards") + " from your hand face down as Prize cards");
                for (int k = 0; k < pick.Count; k++)
                {
                    var card = pick[k];
                    me.Hand.Remove(card);
                    me.Prizes.Add(card);
                    if (k < freed.Count) me.PrizeSlotByCard[card.Id] = freed[k];
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Count + (pick.Count == 1 ? " card" : " cards") + " face down as Prize cards (Peonia).");
            }
        }

        /// <summary>Peony: discard your hand, then search your deck for up to 2 Trainer cards.</summary>
        private sealed class Peony : CardScript
        {
            private static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.SearchToHand(IsTrainerCard, 2, "Peony: search your deck for up to 2 Trainer cards");
            }
        }

        /// <summary>Siebold: heal 60 damage from each of up to 2 of your Rapid Strike Pokémon.</summary>
        private sealed class Siebold : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsRapidStrike && m.DamageCounters > 0); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var pick = targets.Count == 1 ? targets : ctx.ChoosePokemonMany(targets, 1, 2, "Siebold: heal 60 damage from up to 2 of your Rapid Strike Pokémon");
                foreach (var mon in pick) ctx.Heal(mon, 60);
            }
        }

        // ================================================================== Items

        /// <summary>Echoing Horn: put a Basic Pokémon from your opponent's discard pile onto their Bench.</summary>
        private sealed class EchoingHorn : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Basics(ctx).Count > 0 && ctx.Engine.BenchSpace(ctx.Opponent) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return;
                var card = ctx.ChooseCards(basics, 1, 1, "Echoing Horn: put which Basic Pokémon from your opponent's discard pile onto their Bench?")[0];
                ctx.Opp.Discard.Remove(card);
                ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
            }
        }

        /// <summary>Expedition Uniform: look at the bottom 3 cards of your deck and put them on top of your deck in any order.</summary>
        private sealed class ExpeditionUniform : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int count = Math.Min(3, deck.Count);
                if (count == 0) return;
                var bottom = deck.Skip(deck.Count - count).ToList();
                ctx.Engine.NoteLookedAt(bottom, "Bottom " + count + (count == 1 ? " card" : " cards") + " of your deck", ctx.Player);
                foreach (var card in bottom) deck.Remove(card);
                var ordered = new List<CardInstance>();
                var left = bottom.ToList();
                while (left.Count > 1)
                {
                    var pick = ctx.ChooseCards(left, 1, 1, "Expedition Uniform: choose the card to put " + (ordered.Count == 0 ? "on top of your deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                    ordered.Add(pick);
                    left.Remove(pick);
                }
                ordered.AddRange(left);
                deck.InsertRange(0, ordered);
                ctx.Engine.ForgetLookedAt();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the bottom " + ordered.Count + (ordered.Count == 1 ? " card" : " cards") + " of their deck on top (Expedition Uniform).");
            }
        }

        /// <summary>Fan of Waves: put a Special Energy attached to 1 of your opponent's Pokémon on the bottom of their deck.</summary>
        private sealed class FanOfWaves : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var holder = ctx.ChoosePokemon(targets, "Fan of Waves: which of your opponent's Pokémon?");
                var specials = holder.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                var energy = ctx.ChooseCards(specials, 1, 1, "Fan of Waves: put which Special Energy on the bottom of their deck?")[0];
                if (ctx.Engine.EnergyRemovalProtected(holder, energy))
                {
                    ctx.Log(energy.Def.Name + " on " + holder.Name + " can't be put into the deck by Fan of Waves.");
                    return;
                }
                holder.Energy.Remove(energy);
                ctx.State.Players[energy.Owner].Deck.Add(energy);
                ctx.Log(energy.Def.Name + " from " + holder.Name + " was put on the bottom of " + ctx.Engine.PlayerName(energy.Owner) + "'s deck (Fan of Waves).");
            }
        }

        /// <summary>Fog Crystal: search your deck for a Psychic Energy card or a Basic Psychic Pokémon.</summary>
        private sealed class FogCrystal : CardScript
        {
            private static bool Fits(CardDef d)
            {
                return CardsEVS1.IsEnergyOf(d, EnergyType.Psychic) || (d.IsBasicPokemon && d.HasType(EnergyType.Psychic));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(Fits, 1, "Fog Crystal: a Psychic Energy card or a Basic Psychic Pokémon");
            }
        }

        /// <summary>Urn of Vitality: shuffle up to 2 Single Strike Energy cards from your discard pile into your deck (only that card, your ruling).</summary>
        private sealed class UrnOfVitality : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.Name == "Single Strike Energy").ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                var pick = ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Urn of Vitality: shuffle up to 2 Single Strike Energy cards into your deck");
                ctx.DiscardToDeck(pick);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Welcoming Lantern: put a Single Strike Supporter card from your discard pile into your hand.</summary>
        private sealed class WelcomingLantern : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsSupporter && IsSingleStrikeCard(c.Def)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Welcoming Lantern: put a Single Strike Supporter card into your hand"));
            }
        }

        // ================================================================== Tools

        /// <summary>Galarian Chestplate: a Pokémon with "Galarian" in its name takes 30 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class GalarianChestplate : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || !holder.Name.Contains("Galarian")) return damage;
                return damage - 30;
            }
        }

        /// <summary>
        /// Rugged Helmet: if its Pokémon is in the Active Spot and is damaged by an attack from the opponent's Pokémon (even if
        /// it's Knocked Out), put an Energy attached to the Attacking Pokémon into the opponent's hand.
        /// </summary>
        private sealed class RuggedHelmet : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || attacker.Energy.Count == 0) return;
                if (!ctx.InPlay(attacker)) return;
                var energy = attacker.Energy.Count == 1 ? attacker.Energy[0] : ctx.ChooseCards(attacker.Energy.ToList(), 1, 1, "Rugged Helmet: put which Energy on " + attacker.Name + " into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(attacker, energy);
            }
        }

        /// <summary>
        /// Rapid Strike Scroll of the Skies / Single Strike Scroll of Piercing: a Rapid Strike (Single Strike) Pokémon with it
        /// can use its attack. Gravdrop: 50 more for each Energy attached to the opponent's Active Pokémon. Bullet
        /// Breakthrough: not affected by Weakness, Resistance or effects on the opponent's Active Pokémon.
        /// </summary>
        private sealed class CreScroll : CardScript
        {
            private readonly bool single;

            public CreScroll(bool single) { this.single = single; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                bool fits = single ? mon.Def.IsSingleStrike : mon.Def.IsRapidStrike;
                if (!fits) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                var defending = ctx.Defending;
                if (defending == null) return;
                if (single)
                {
                    ctx.DamageWith(defending, atk.Damage, false, false, true);
                    return;
                }
                ctx.DamageDefending(atk.Damage + 50 * ctx.EnergyCount(defending));
            }
        }

        // ================================================================== Stadiums

        /// <summary>Dyna Tree Hill: Pokémon (both players') can't be healed.</summary>
        private sealed class DynaTreeHill : CardScript
        {
            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon) { return true; }
        }

        /// <summary>
        /// Old Cemetery: whenever either player attaches an Energy card from their hand to 1 of their non-Psychic Pokémon, put
        /// 2 damage counters on that Pokémon.
        /// </summary>
        private sealed class OldCemetery : CardScript
        {
            private static void Haunt(EffectContext src, PokemonInPlay target)
            {
                if (target == null || src.Engine.StadiumShielded(target) || src.Engine.IsType(target, EnergyType.Psychic)) return;
                if (src.State.FindPokemon(target.Uid) != target) return;
                src.Engine.PlaceDamageCounters(target, 2, "Old Cemetery");
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Haunt(src, target); }
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Haunt(src, target); }
        }

        // ================================================================== Energy

        /// <summary>
        /// Impact Energy / Spiral Energy: only on a Single Strike (Rapid Strike) Pokémon, discarded from anything else; provides
        /// every type, 1 Energy at a time; its Pokémon can't be Poisoned (Paralyzed) and recovers from it.
        /// </summary>
        private sealed class StrikeEnergy : CardScript
        {
            private readonly bool single;
            private readonly SpecialCondition shield;

            public StrikeEnergy(bool single)
            {
                this.single = single;
                shield = single ? SpecialCondition.Poisoned : SpecialCondition.Paralyzed;
            }

            private bool Fits(PokemonInPlay mon) { return single ? mon.Def.IsSingleStrike : mon.Def.IsRapidStrike; }

            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && Fits(target); }
            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !Fits(ctx.Pokemon); }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & shield) != 0;
            }

            private void Recover(EffectContext src)
            {
                var mon = src.Pokemon;
                if (mon != null && mon.Has(shield)) src.Engine.RemoveCondition(mon, shield);
            }

            public override void OnAttachedFromHand(EffectContext ctx) { Recover(ctx); }
            public override void OnPokemonCheckup(EffectContext src) { Recover(src); }
            public override void OnEndOfTurn(EffectContext src) { Recover(src); }
        }

        /// <summary>Lucky Energy: provides [C]; if its Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), draw a card.</summary>
        private sealed class LuckyEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || ctx.Me.Deck.Count == 0) return;
                ctx.Log("Lucky Energy: " + ctx.Engine.PlayerName(ctx.Player) + " draws a card.");
                ctx.Draw(1);
            }
        }
    }
}
