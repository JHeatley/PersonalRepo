using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Brilliant Stars (BRS, 2022): the Trainer Gallery Pokémon, the Trainers and Magma Basin. The Trainer Gallery reprints
    /// Pokémon from Vivid Voltage, Battle Styles, Evolving Skies and Darkness Ablaze. Flareon, Vaporeon and Jolteon need a
    /// Memory Capsule (Vivid Voltage, not in the pool), and Houndoom's Single Strike Roar a Single Strike Energy (Battle Styles,
    /// not in the pool): their scripts follow the text, so they work if those cards are added.
    /// Your rulings (v54): Single Strike Style Mustard draws 5 whenever you search, even if you find nothing (with a full Bench
    /// it can't be played); Blunder Policy still draws 3 at the end of the turn if its Pokémon or the Tool is gone by then;
    /// Spectral Breach doesn't reach a Pokémon shielded from the opponent's Abilities.
    /// </summary>
    public static class CardsBRS2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Trainer Gallery Pokémon
            r.RegisterByKey("BRS-TG1", new AwakeningMon(EnergyType.Grass));
            r.RegisterByKey("BRS-TG2", new AwakeningMon(EnergyType.Fire));
            r.RegisterByKey("BRS-TG3", new OctilleryTG());
            r.RegisterByKey("BRS-TG4", new AwakeningMon(EnergyType.Water));
            r.RegisterByKey("BRS-TG5", new WildShockMon("Wild Shock", 60));
            r.RegisterByKey("BRS-TG6", new DusknoirTG());
            r.RegisterByKey("BRS-TG9", new AriadosTG());
            r.RegisterByKey("BRS-TG10", new HoundoomTG());
            r.RegisterByKey("BRS-TG11", new SearchAttackMon("Vee-Search", IsPokemonVCard, 3));
            r.RegisterByKey("BRS-TG12", new OranguruTG());
            r.RegisterByKey("BRS-TG13", new ComboMon(new ElectrifyMon(), new CardsASR1.PlusMon("Bolt Storm", 30, LightningEnergyInPlay)));
            r.RegisterByKey("BRS-TG14", new SylveonV());
            r.RegisterByKey("BRS-TG15", new ComboMon(new PreciousTouchMon(), new CardsASR1.PlusMon("Max Harmony", 30, BenchTypes)));
            r.RegisterByKey("BRS-TG18", new ComboMon(new CardsMEG.ChargeSelfMon("Laser Focus", EnergyType.Fighting, 2), new LockAfterAttackMon("Impact Blow", true)));
            r.RegisterByKey("BRS-TG19", new OneBlowMon());
            r.RegisterByKey("BRS-TG20", new SwitchSelfMon("Strafe", true));
            r.RegisterByKey("BRS-TG21", new RapidStrikeUrshifuVmax());
            r.RegisterByKey("BRS-TG22", new ComboMon(new NoRetreatMon("Mean Look"), new CardsASC.SelfDamagedBonusMon("Moonlight Blade", 80)));
            r.RegisterByKey("BRS-TG23", new UmbreonVmaxTG());

            // Trainers
            r.RegisterByName("Acerola's Premonition", new AcerolasPremonition());
            r.RegisterByName("Barry", new Barry());
            r.RegisterByName("Blunder Policy", new BlunderPolicy());
            r.RegisterByName("Café Master", new CafeMaster());
            r.RegisterByName("Cleansing Gloves", new TypeGloves(EnergyType.Psychic));
            r.RegisterByName("Fresh Water Set", new FreshWaterSet());
            r.RegisterByName("Gloria", new Gloria());
            r.RegisterByName("Hunting Gloves", new TypeGloves(EnergyType.Dragon));
            r.RegisterByName("Kindler", new Kindler());
            r.RegisterByName("Magma Basin", new MagmaBasin());
            r.RegisterByName("Marnie's Pride", new MarniesPride());
            r.RegisterByName("Pot Helmet", new PotHelmet());
            r.RegisterByName("Roseanne's Backup", new RoseannesBackup());
            r.RegisterByName("Team Yell's Cheer", new TeamYellsCheer());
            r.RegisterByName("Rapid Strike Style Mustard", new RapidStrikeStyleMustard());
            r.RegisterByName("Single Strike Style Mustard", new SingleStrikeStyleMustard());
        }

        // ================================================================== Filters and counters

        private static bool IsPokemonVCard(CardDef d) { return d.IsPokemon && d.IsPokemonV; }
        private static bool IsItemCard(CardDef d) { return d.IsItem; }
        private static bool IsLightningEnergy(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Lightning); }

        /// <summary>Bolt Storm: the Lightning Energy attached to all of your Pokémon.</summary>
        private static int LightningEnergyInPlay(EffectContext ctx)
        {
            int n = 0;
            foreach (var mon in ctx.Me.InPlay()) n += ctx.Engine.EnergyCountOfType(mon, EnergyType.Lightning);
            return n;
        }

        /// <summary>Max Harmony: the different types of Pokémon on your Bench.</summary>
        private static int BenchTypes(EffectContext ctx)
        {
            var types = new HashSet<EnergyType>();
            foreach (var mon in ctx.Me.Bench)
                foreach (var type in ctx.Engine.TypesOf(mon)) types.Add(type);
            return types.Count;
        }

        /// <summary>Alcremie's Additional Order: while it's your Active Pokémon, Café Master doesn't end your turn.</summary>
        internal static bool AdditionalOrder(EffectContext ctx)
        {
            var active = ctx.Me.Active;
            return active != null && ctx.Engine.HasAbilityNamed(active, "Additional Order");
        }

        // ================================================================== Trainer Gallery Pokémon

        /// <summary>
        /// Incandescent / Torrential / Thunderous Awakening: if this Pokémon has a Memory Capsule attached, [type] Pokémon in play
        /// (both players') have no Abilities. The printed type is checked, plus the type the Pokémon's own Ability would give it:
        /// Kecleon whose Chromashift would make it that type loses Chromashift and stays Colorless (Compendium 2022-06-02).
        /// Neither check asks Abilities again.
        /// </summary>
        private sealed class AwakeningMon : Mon
        {
            private readonly EnergyType blocked;

            public AwakeningMon(EnergyType blocked) { this.blocked = blocked; }

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }
            public override bool LockDependsOnTool { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || !me.HasToolNamed("Memory Capsule") || src.Engine.ToolsDisabled()) return false;
                if (mon.Def.HasType(blocked)) return true;
                var withOwnAbility = new List<EnergyType> { mon.Def.PokemonType };
                if (mon.Def.SecondType.HasValue) withOwnAbility.Add(mon.Def.SecondType.Value);
                src.Engine.Scripts.For(mon.Def).ModifyTypes(new EffectContext(src.Engine, mon.Owner, mon.Top, mon), mon, withOwnAbility);
                return withOwnAbility.Contains(blocked);
            }
        }

        /// <summary>Rapid Strike Search: once during your turn, search your deck for a Rapid Strike card (only 1 Rapid Strike Search each turn).</summary>
        private sealed class OctilleryTG : Mon
        {
            protected override string Activated { get { return "Rapid Strike Search"; } }
            protected override bool OnePerPlayer { get { return true; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(IsRapidStrikeCard, 1, "Rapid Strike Search: choose a Rapid Strike card to put into your hand");
            }

            private static bool IsRapidStrikeCard(CardDef d) { return d.IsRapidStrike; }
        }

        /// <summary>Wild Shock: this Pokémon also does 60 damage to itself; the opponent's Active Pokémon is Paralyzed. Used for Wild Shock, Wild Freeze (the attack name and the numbers are passed in).</summary>
        internal sealed class WildShockMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public WildShockMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(amount);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Spectral Breach: all Special Energy attached to Pokémon (both players') provide [C] and have no other effect. The engine
        /// asks this of every Pokémon in play and counts it while the Ability works (like Temple of Sinnoh).
        /// </summary>
        private sealed class DusknoirTG : Mon
        {
            public override bool DisablesSpecialEnergy(EffectContext src) { return src.Pokemon != null; }
        }

        /// <summary>Spider Net: when you play this Pokémon to evolve, you may switch 1 of the opponent's Benched Evolution Pokémon into the Active Spot. Poison Sting: Poisoned.</summary>
        private sealed class AriadosTG : Mon
        {
            private static readonly Mon PoisonSting = new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned);

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.GustTargets().Where(m => m.Def.IsEvolution).ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Opp.Active != null && Targets(ctx).Count > 0;
                if (!TriggerOnce(ctx, "Spider Net", ready, "Spider Net: switch 1 of your opponent's Benched Evolution Pokémon with their Active Pokémon?")) return;
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var pick = ctx.ChoosePokemon(targets, "Spider Net: choose 1 of your opponent's Benched Evolution Pokémon to switch in");
                ctx.Engine.SwitchActive(ctx.Opponent, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PoisonSting.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Single Strike Roar: once during your turn, a Single Strike Energy from your deck onto 1 of your Single Strike Pokémon, then
        /// 2 damage counters on it. Not usable when all 4 of your Single Strike Energy are in known places (hand, discard pile, Lost
        /// Zone or in play: Compendium, 2021-03-25).
        /// </summary>
        private sealed class HoundoomTG : Mon
        {
            protected override string Activated { get { return "Single Strike Roar"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => m.Def.IsSingleStrike).Count > 0 && KnownSingleStrikeEnergy(ctx) < 4;
            }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(IsSingleStrikeEnergy, 1, "Single Strike Roar: search your deck for a Single Strike Energy card");
                PokemonInPlay target = null;
                if (found.Count > 0)
                {
                    var strikers = ctx.MyPokemon(m => m.Def.IsSingleStrike);
                    target = ctx.ChoosePokemon(strikers, "Single Strike Roar: attach it to which of your Single Strike Pokémon?");
                    ctx.Engine.AttachEnergyCard(found[0], target);
                }
                ctx.ShuffleMyDeck();
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            private static bool IsSingleStrikeEnergy(CardDef d) { return d.IsEnergy && d.Name == "Single Strike Energy"; }

            /// <summary>Your Single Strike Energy cards in the places both players can see (not the deck or the Prize cards).</summary>
            private static int KnownSingleStrikeEnergy(EffectContext ctx)
            {
                var me = ctx.Me;
                return me.Hand.Count(c => IsSingleStrikeEnergy(c.Def)) + me.Discard.Count(c => IsSingleStrikeEnergy(c.Def))
                    + me.LostZone.Count(c => IsSingleStrikeEnergy(c.Def)) + me.InPlay().Sum(m => m.Energy.Count(c => IsSingleStrikeEnergy(c.Def)));
            }
        }

        /// <summary>Primate Wisdom: once during your turn, switch a card from your hand with the top card of your deck.</summary>
        private sealed class OranguruTG : Mon
        {
            protected override string Activated { get { return "Primate Wisdom"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Primate Wisdom: choose a card from your hand to switch with the top card of your deck");
                if (pick.Count == 0 || ctx.Me.Deck.Count == 0) return;
                var card = pick[0];
                var top = ctx.Me.Deck[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Me.Deck.RemoveAt(0);
                ctx.Me.Deck.Insert(0, card);
                ctx.Me.Hand.Add(top);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched a card from their hand with the top card of their deck.");
            }
        }

        /// <summary>Electrify: search your deck for up to 2 Lightning Energy cards and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class ElectrifyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electrify") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsBRS1.SearchAttachAnyWay(ctx, IsLightningEnergy, 2, mon => ctx.Me.Bench.Contains(mon), "Electrify", "Lightning Energy cards for your Benched Pokémon");
                return true;
            }
        }

        /// <summary>Dream Gift: once during your turn, search your deck for an Item card; then your turn ends.</summary>
        private sealed class SylveonV : Mon
        {
            protected override string Activated { get { return "Dream Gift"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(IsItemCard, 1, "Dream Gift: choose an Item card to put into your hand");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Precious Touch: attach an Energy card from your hand to 1 of your Benched Pokémon; if you do, heal 120 damage from it.</summary>
        private sealed class PreciousTouchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Precious Touch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var bench = ctx.MyBench().Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
                if (energy.Count == 0 || bench.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Precious Touch: attach which Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(bench, "Precious Touch: attach " + card.Def.Name + " to which Benched Pokémon?");
                if (!ctx.Me.Hand.Remove(card)) return true;
                ctx.Engine.AttachEnergyCard(card, target, true);
                ctx.Heal(target, 120);
                return true;
            }
        }

        /// <summary>G-Max One Blow: not affected by effects on the opponent's Active Pokémon; discard all Energy from this Pokémon.</summary>
        private sealed class OneBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max One Blow") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Gale Thrust: 120 more damage if this Pokémon moved from your Bench to the Active Spot this turn. G-Max Rapid Flow:
        /// discard all Energy from this Pokémon; 120 damage to 2 of your opponent's Pokémon.
        /// </summary>
        private sealed class RapidStrikeUrshifuVmax : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gale Thrust")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? 120 : 0));
                    return true;
                }
                if (atk.Name != "G-Max Rapid Flow") return false;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var targets = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, "G-Max Rapid Flow: choose 2 of your opponent's Pokémon (120 damage each)");
                foreach (var target in targets) ctx.DamageTarget(target, atk.Damage);
                return true;
            }
        }

        /// <summary>Dark Signal: when you play this Pokémon to evolve, you may switch 1 of the opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class UmbreonVmaxTG : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Opp.Active != null && ctx.GustTargets().Count > 0;
                if (!TriggerOnce(ctx, "Dark Signal", ready, "Dark Signal: switch 1 of your opponent's Benched Pokémon with their Active Pokémon?")) return;
                ctx.GustOpponent("Dark Signal: choose 1 of your opponent's Benched Pokémon to switch in");
            }
        }

        // ================================================================== Items

        /// <summary>Fresh Water Set: heal 20 damage from each of your Pokémon.</summary>
        private sealed class FreshWaterSet : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 20);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Blunder Policy: if its Pokémon flips any tails for the damage or effects of its attack, draw 3 cards at the end of your turn.</summary>
        private sealed class BlunderPolicy : CardScript
        {
            public override void OnHolderAttackFlippedTails(EffectContext src)
            {
                src.AddEffect(new EndOfTurnDrawEffect("Blunder Policy: " + src.Engine.PlayerName(src.Player) + " will draw 3 cards at the end of the turn",
                    src.Player, src.Turn, 3));
            }
        }

        /// <summary>"Draw N cards at the end of your turn" (Blunder Policy). It's on the player, so it doesn't need the card any more.</summary>
        internal sealed class EndOfTurnDrawEffect : ContinuousEffect
        {
            private readonly int cards;

            public EndOfTurnDrawEffect(string name, int owner, int turn, int cards)
                : base(name, owner, -1, turn, false)
            {
                this.cards = cards;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn || engine.State.CurrentPlayer != Owner) return;
                engine.DrawCards(Owner, cards);
            }
        }

        /// <summary>Cleansing Gloves / Hunting Gloves: the holder's attacks do 30 more damage to the opponent's Active [Psychic / Dragon] Pokémon.</summary>
        internal sealed class TypeGloves : CardScript
        {
            private readonly EnergyType type;

            public TypeGloves(EnergyType type) { this.type = type; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, type) ? damage + 30 : damage;
            }
        }

        /// <summary>Pot Helmet: if its Pokémon doesn't have a Rule Box, it takes 30 less damage from the opponent's attacks.</summary>
        private sealed class PotHelmet : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent || info.Target.Def.HasRuleBox) return damage;
                return damage - 30;
            }
        }

        // ================================================================== Supporters

        /// <summary>Acerola's Premonition: your opponent reveals their hand; draw a card for each Trainer card there.</summary>
        private sealed class AcerolasPremonition : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                int n = ctx.Opp.Hand.Count(c => c.Def.IsTrainer);
                if (n > 0) ctx.Draw(n);
            }
        }

        /// <summary>Barry: draw 3 cards.</summary>
        private sealed class Barry : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>
        /// Café Master: choose up to 3 of your Benched Pokémon; for each, a basic Energy of a different type from your deck onto it.
        /// Your turn ends, unless Alcremie's Additional Order is working in your Active Spot.
        /// </summary>
        private sealed class CafeMaster : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var bench = ctx.MyBench();
                if (bench.Count > 0 && ctx.Me.Deck.Count > 0)
                {
                    var chosen = ctx.ChoosePokemonMany(bench, 0, Math.Min(3, bench.Count), "Café Master: choose up to 3 of your Benched Pokémon");
                    var used = new List<EnergyType>();
                    foreach (var mon in chosen)
                    {
                        var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && !used.Contains(d.EnergyProvides), 1,
                            "Café Master: search your deck for a basic Energy (a type not chosen yet) for " + mon.Name);
                        if (found.Count == 0) continue;
                        used.Add(found[0].Def.EnergyProvides);
                        ctx.Engine.AttachEnergyCard(found[0], mon);
                    }
                    ctx.ShuffleMyDeck();
                }
                if (AdditionalOrder(ctx)) ctx.Log("Additional Order: Café Master doesn't end " + ctx.Engine.Their(ctx.Player) + " turn.");
                else ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Gloria: search your deck for up to 3 Basic Pokémon that don't have a Rule Box and put them onto your Bench.</summary>
        private sealed class Gloria : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToBench(NoRuleBox, 3, "Gloria: choose up to 3 Basic Pokémon that don't have a Rule Box");
            }

            private static bool NoRuleBox(CardDef d) { return !d.HasRuleBox; }
        }

        /// <summary>Kindler: discard a Fire Energy card from your hand (a cost); look at the top 7 cards and put up to 2 into your hand.</summary>
        private sealed class Kindler : CardScript
        {
            private static List<CardInstance> FireCards(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Fire)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && FireCards(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = FireCards(ctx);
                if (fire.Count == 0) return;
                var cost = fire.Select(c => c.Def.Name).Distinct().Count() == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Kindler: discard a Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);
                var top = ctx.LookAtTop(7);
                if (top.Count == 0) return;
                var picks = ctx.ChooseCards(top, 0, Math.Min(2, top.Count), "Kindler: put up to 2 of these cards into your hand");
                ctx.DeckToHand(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Marnie's Pride: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class MarniesPride : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = ctx.BasicEnergyInDiscard();
                var bench = ctx.MyBench();
                if (energy.Count == 0 || bench.Count == 0) return;
                var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Marnie's Pride: attach which basic Energy card from your discard pile?")[0];
                var target = ctx.ChoosePokemon(bench, "Marnie's Pride: attach " + card.Def.Name + " to which Benched Pokémon?");
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>
        /// Roseanne's Backup: choose 1 or more: shuffle a Pokémon, a Pokémon Tool card, a Stadium card and/or an Energy card from
        /// your discard pile into your deck.
        /// </summary>
        private sealed class RoseannesBackup : CardScript
        {
            private static readonly string[] Kinds = { "a Pokémon", "a Pokémon Tool card", "a Stadium card", "an Energy card" };

            private static bool Matches(CardDef d, int kind)
            {
                switch (kind)
                {
                    case 0: return d.IsPokemon;
                    case 1: return d.IsTool;
                    case 2: return d.IsStadiumCard;
                    default: return d.IsEnergy;
                }
            }

            private static List<int> Available(EffectContext ctx)
            {
                var list = new List<int>();
                for (int kind = 0; kind < Kinds.Length; kind++)
                {
                    int k = kind;
                    if (ctx.Me.Discard.Any(c => Matches(c.Def, k))) list.Add(kind);
                }
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Available(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var kinds = Available(ctx);
                if (kinds.Count == 0) return;
                var opts = kinds.Select(k => new DecisionOption { Label = "Shuffle " + Kinds[k] + " from your discard pile into your deck", Index = k }).ToList();
                var chosen = ctx.Engine.AskMany(ctx.Player, DecisionKind.ChooseOption, "Roseanne's Backup: choose 1 or more", opts, 1, opts.Count, ctx.Source);
                var moved = new List<CardInstance>();
                foreach (int pick in chosen)
                {
                    int kind = opts[pick].Index;
                    var pool = ctx.Me.Discard.Where(c => Matches(c.Def, kind)).ToList();
                    if (pool.Count == 0) continue;
                    var card = ctx.ChooseCards(pool, 1, 1, "Roseanne's Backup: choose " + Kinds[kind] + " to shuffle into your deck")[0];
                    moved.AddRange(ctx.DiscardToDeck(new List<CardInstance> { card }));
                }
                if (moved.Count > 0) ctx.Engine.Reveal(ctx.Player, moved, "shuffled these cards from their discard pile into their deck");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Team Yell's Cheer: shuffle up to 3 Pokémon and Supporter cards (not Team Yell's Cheer) from your discard pile into your deck.</summary>
        private sealed class TeamYellsCheer : CardScript
        {
            private static List<CardInstance> Pool(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon || (c.Def.IsSupporter && c.Def.Name != "Team Yell's Cheer")).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pool(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pool = Pool(ctx);
                if (pool.Count == 0) return;
                var picks = ctx.ChooseCards(pool, 1, Math.Min(3, pool.Count), "Team Yell's Cheer: shuffle up to 3 Pokémon and Supporter cards from your discard pile into your deck");
                var moved = ctx.DiscardToDeck(picks);
                if (moved.Count > 0) ctx.Engine.Reveal(ctx.Player, moved, "shuffled these cards from their discard pile into their deck");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Rapid Strike Style Mustard: only as the last card in your hand; a Rapid Strike Pokémon from your discard pile onto your Bench, and if you do, draw 5.</summary>
        private sealed class RapidStrikeStyleMustard : CardScript
        {
            private static List<CardInstance> Pool(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && c.Def.IsRapidStrike && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card && ctx.BenchSpace > 0 && Pool(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pool = Pool(ctx);
                if (pool.Count == 0 || ctx.BenchSpace <= 0) return;
                var card = ctx.ChooseCards(pool, 1, 1, "Rapid Strike Style Mustard: put which Rapid Strike Pokémon onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.Draw(5);
            }
        }

        /// <summary>
        /// Single Strike Style Mustard: only as the last card in your hand; search your deck for a Single Strike Pokémon and put it
        /// onto your Bench; if you searched, draw 5, even if you found nothing (your ruling). With a full Bench you can't search, so
        /// it can't be played then.
        /// </summary>
        private sealed class SingleStrikeStyleMustard : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0 || ctx.Me.Deck.Count == 0) return;
                ctx.SearchToBench(IsSingleStrike, 1, "Single Strike Style Mustard: choose a Single Strike Pokémon to put onto your Bench");
                ctx.Draw(5);
            }

            private static bool IsSingleStrike(CardDef d) { return d.IsSingleStrike; }
        }

        // ================================================================== Stadium

        /// <summary>
        /// Magma Basin: once during each player's turn, that player may attach a Fire Energy card from their discard pile to 1 of
        /// their Benched Fire Pokémon, then put 2 damage counters on that Pokémon.
        /// </summary>
        private sealed class MagmaBasin : StadiumAction
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Fire)).ToList();
            }

            private static List<CardInstance> FireInDiscard(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Fire)).ToList();
            }

            protected override bool Ready(EffectContext ctx) { return FireInDiscard(ctx).Count > 0 && Targets(ctx).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var fire = FireInDiscard(ctx);
                var targets = Targets(ctx);
                if (fire.Count == 0 || targets.Count == 0) return false;
                var card = fire.Select(c => c.Def.Name).Distinct().Count() == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Magma Basin: attach which Fire Energy card from your discard pile?")[0];
                var target = ctx.ChoosePokemon(targets, "Magma Basin: attach " + card.Def.Name + " to which Benched Fire Pokémon?");
                if (!ctx.Me.Discard.Remove(card)) return false;
                ctx.Engine.AttachEnergyCard(card, target);
                ctx.PlaceCounters(target, 2);
                return false;
            }
        }
    }
}
