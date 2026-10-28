using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ruby &amp; Sapphire Rare Pokémon and Pokémon-ex (RS 1-24, 96-103; v149). Unlimited only. Delcatty 5 is 30th Celebration's
    /// CC8. The cards were printed for 2-on-2 games too: here a game is 1-on-1, so "each Defending Pokémon" and "1 of the
    /// Defending Pokémon" are the Defending Pokémon, and Seaking's Fast Stream (only with 2 Defending Pokémon) just does its damage.
    /// Mega Throw counts the Pokémon-ex of this era only (rule 145).
    /// </summary>
    public static class CardsRS
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RS-1", new ComboMon(new RetaliateFlipMon(), new CoinsMon("Double Lariat", 2)));
            r.RegisterByKey("RS-2", new BeautiflyRS());
            r.RegisterByKey("RS-3", new BlazikenRS());
            r.RegisterByKey("RS-4", new ComboMon(new LavaBurnMon(), new DiscardBasicOrNothingMon("Fire Spin", 2, false)));
            r.RegisterByKey("RS-6", new DustoxRS());
            r.RegisterByKey("RS-7", new GardevoirRS());
            r.RegisterByKey("RS-8", new ComboMon(new TextDamageMon("Super Slap Push", 20),
                new CardsFST2.ConditionBonusMon("Mega Throw", 40, c => c.Defending != null && c.Defending.Def.IsOldEx)));
            r.RegisterByKey("RS-9", new ComboMon(new AttractCurrentMon(), new CardsFST1.FlipRecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("RS-10", new MightyenaRS());
            r.RegisterByKey("RS-11", new LizardPoisonMon());
            r.RegisterByKey("RS-12", new SlakingRS());
            r.RegisterByKey("RS-13", new SwampertWaterCall());
            r.RegisterByKey("RS-14", new RecoilMon("Take Down", 20));
            r.RegisterByKey("RS-15", new ComboMon(new NoRetreatMon("Clutch"), new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("RS-16", new CardsASR1.PlusMon("Battle Blast", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting)));
            r.RegisterByKey("RS-17", new ComboMon(new CardsFST2.ConditionBonusMon("Rend", 20, c => c.Defending != null && c.Defending.DamageCounters > 0),
                new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("RS-18", new ComboMon(new InvisibleHandMon(), new RepulsionMon()));
            r.RegisterByKey("RS-19", new StockpileMon());
            r.RegisterByKey("RS-20", new SceptileEnergyTrans());
            r.RegisterByKey("RS-21", new WaterArrowMon());
            r.RegisterByKey("RS-22", new CardsASC.CounterattackMon("Rough Skin", 2, new DarkSlashMon()));
            r.RegisterByKey("RS-23", new SwampertNaturalRemedy());
            r.RegisterByKey("RS-24", new ComboMon(new ConditionAttackMon("Confusion Gas", SpecialCondition.Confused), new PoisonSmogMon()));

            r.RegisterByKey("RS-96", new ComboMon(new HealingEggMon(), new RecoilMon("Double-edge", 60)));
            r.RegisterByKey("RS-97", new ComboMon(new ParalyzeFlipMon("Thundershock"), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("RS-98", new ComboMon(new FlipBonusMon("Steady Punch", 10), new CardsFLF1.NoResistanceMon("Sky Uppercut")));
            r.RegisterByKey("RS-99", new ComboMon(new ExtraEnergyMon("Energy Ball", 2), new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("RS-100", new ComboMon(new CardsUPR3.SmokescreenMon("Smokescreen", 1), new ConditionAttackMon("Super Singe", SpecialCondition.Burned)));
            r.RegisterByKey("RS-101", new EnergyAbsorptionMon());
            r.RegisterByKey("RS-102", new DodgeMon("Agility", false));
            r.RegisterByKey("RS-103", new ComboMon(new CoinsMon("Double Scratch", 2), new BeatUpMon()));
        }

        // ================================================================== Shared

        /// <summary>A basic Energy card of this type (in the hand, deck or discard pile, where Special Energy has no type).</summary>
        internal static bool BasicOf(CardDef d, EnergyType type) { return d != null && d.IsBasicEnergy && d.EnergyProvides == type; }

        /// <summary>Attacks with no printed damage that do it in their text ("Does 20 damage to each Defending Pokémon").</summary>
        internal sealed class TextDamageMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TextDamageMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(amount);
                return true;
            }
        }

        /// <summary>"Discard N basic Energy cards attached to this Pokémon or this attack does nothing" (Fire Spin; Critical Move also can't attack next turn).</summary>
        internal sealed class DiscardBasicOrNothingMon : Mon
        {
            private readonly string attack;
            private readonly int count;
            private readonly bool lockNextTurn;

            public DiscardBasicOrNothingMon(string attack, int count, bool lockNextTurn)
            {
                this.attack = attack;
                this.count = count;
                this.lockNextTurn = lockNextTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    if (basics.Count < count) return Nothing(ctx);
                    var picks = basics.Count == count ? basics : ctx.ChooseCards(basics, count, count, attack + ": discard " + count + " basic Energy card" + (count == 1 ? "" : "s"));
                    int paid = 0;
                    foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) paid++;
                    if (paid < count) return Nothing(ctx);
                }
                ctx.DamageDefending(atk.Damage);
                if (lockNextTurn) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>"N more damage for each Energy attached to this Pokémon but not used to pay for this attack's Energy cost" (up to a cap: Energy Ball, Cannonball).</summary>
        internal sealed class ExtraEnergyMon : Mon
        {
            private readonly string attack;
            private readonly int maxExtra;

            public ExtraEnergyMon(string attack, int maxExtra)
            {
                this.attack = attack;
                this.maxExtra = maxExtra;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int extra = Math.Max(0, ctx.EnergyCount(ctx.Pokemon) - atk.Cost.Length);
                if (maxExtra >= 0) extra = Math.Min(maxExtra, extra);
                ctx.DamageDefending(atk.Damage + 10 * extra);
                return true;
            }
        }

        /// <summary>"Remove all Special Conditions from this Pokémon when you attach a [type] Energy card from your hand to it" (Natural Cure, Poké-Body).</summary>
        internal sealed class NaturalCureMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            public NaturalCureMon(EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !BasicOf(energy.Def, type) || me.Conditions == SpecialCondition.None) return;
                Announce(src, "Natural Cure");
                src.Engine.RemoveAllConditions(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Hard Cocoon (Poké-Body): during the opponent's turn, when the opponent's attack would damage this Pokémon (after Weakness and
        /// Resistance), flip a coin; heads, 30 less.
        /// </summary>
        internal sealed class HardCocoonMon : Mon
        {
            private readonly Mon attacks;

            public HardCocoonMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || damage <= 0) return damage;
                if (src.State.CurrentPlayer == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                Announce(src, "Hard Cocoon");
                return src.Flip() ? Math.Max(0, damage - 30) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Flip a coin. If heads, choose 1 card from your opponent's hand without looking and discard it" (Shakedown, Knock Off).</summary>
        internal sealed class FlipHandDiscardMon : Mon
        {
            private readonly string attack;

            public FlipHandDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon. This attack does 20 damage to that Pokémon" (Weakness and Resistance for the Active one only).</summary>
        internal sealed class WaterArrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Arrow") return false;
                ctx.DamageAnyOpponent(20);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Retaliate: flip; heads, 10 damage for each damage counter on this Pokémon.</summary>
        internal sealed class RetaliateFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Retaliate") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>
        /// Withering Dust (Poké-Body): while Beautifly is in play (Active or Benched), Resistance isn't applied to any Active Pokémon.
        /// Stun Spore: heads, Paralyzed. Parallel Gain: then 1 damage counter off each of your Pokémon, Beautifly included.
        /// </summary>
        private sealed class BeautiflyRS : Mon
        {
            private static readonly Mon StunSpore = new ParalyzeFlipMon("Stun Spore");

            public override bool IgnoresResistance(EffectContext src, DamageInfo info) { return info != null && info.TargetIsActive; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (StunSpore.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Parallel Gain") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Firestarter (Poké-Power): once during your turn, attach a Fire Energy card from your discard pile to 1 of your Benched
        /// Pokémon. Fire Stream: discard a Fire Energy card attached to Blaziken; if you do, 10 to each of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class BlazikenRS : Mon
        {
            private const string Power = "Firestarter";

            protected override string Activated { get { return Power; } }

            private static List<CardInstance> Fire(EffectContext ctx) { return ctx.Me.Discard.Where(c => BasicOf(c.Def, EnergyType.Fire)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && Fire(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fire = Fire(ctx);
                if (fire.Count == 0 || ctx.Me.Bench.Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Firestarter: attach a Fire Energy card from your discard pile to which Benched Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(fire[0])) return;
                ctx.Engine.AttachEnergyCard(fire[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Stream") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                bool discarded = ctx.InPlay(me)
                    && ctx.DiscardEnergyFrom(me, 1, "Fire Stream: discard a Fire Energy card from " + me.Name, c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Fire)) > 0;
                if (discarded)
                    foreach (var mon in ctx.OpponentBench())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Lava Burn: 20 to the Defending Pokémon and 10 to 1 of the opponent's Benched Pokémon (no Weakness or Resistance).</summary>
        private sealed class LavaBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Burn") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>
        /// Protective Dust (Poké-Body): no effects of attacks, other than damage, reach Dustox. Toxic: Poisoned, 2 damage counters
        /// between turns.
        /// </summary>
        private sealed class DustoxRS : Mon
        {
            private static readonly Mon Toxic = new CardsBS1.ToxicMon("Toxic", 2);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Toxic.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Psy Shadow (Poké-Power): once during your turn, a Psychic Energy card from your deck onto 1 of your Pokémon, then 2 damage
        /// counters on that Pokémon. Energy Burst: 10 for each Energy on Gardevoir and the Defending Pokémon.
        /// </summary>
        private sealed class GardevoirRS : Mon
        {
            private const string Power = "Psy Shadow";
            private static readonly Mon Burst = new CardsCRZ1.BothActiveEnergyMon("Energy Burst", 10);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => BasicOf(d, EnergyType.Psychic), 1, "Psy Shadow: choose a Psychic Energy card");
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Psy Shadow: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null) { ctx.Me.Deck.Add(card); continue; }
                    if (ctx.Engine.AttachEnergyCard(card, target)) ctx.PlaceCounters(target, 2);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burst.HandleAttack(ctx, atk); }
        }

        /// <summary>Attract Current: a Lightning Energy card from your deck onto 1 of your Pokémon.</summary>
        private sealed class AttractCurrentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attract Current") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => BasicOf(d, EnergyType.Lightning), 1, "Attract Current: choose a Lightning Energy card");
                foreach (var card in found)
                {
                    var mine = ctx.MyPokemon();
                    var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Attract Current: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null) { ctx.Me.Deck.Add(card); continue; }
                    ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Intimidating Fang (Poké-Body): while Mightyena is your Active Pokémon, damage the opponent's attacks do to your Pokémon is 10
        /// less, before Weakness and Resistance. Shakedown: heads, a random card from the opponent's hand is discarded.
        /// </summary>
        private sealed class MightyenaRS : Mon
        {
            private static readonly Mon Shakedown = new FlipHandDiscardMon("Shakedown");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                // Swift-like attacks ignore effects on the Defending Pokémon.
                if (info.IgnoreDefenderEffects && info.TargetIsActive) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shakedown.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Lizard Poison: by the Energy attached to Sceptile: 1 Asleep; 2 Poisoned; 3 Asleep and Poisoned; 4 or more Asleep, Burned and
        /// Poisoned.
        /// </summary>
        private sealed class LizardPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lizard Poison") return false;
                var defending = ctx.Defending;
                int energy = ctx.EnergyCount(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage);
                if (energy <= 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (energy == 1 || energy == 3 || energy >= 4) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (energy >= 4) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                if (energy >= 2) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Lazy (Poké-Body): while Slaking is your Active Pokémon, your opponent's Pokémon can't use Poké-Powers. Critical Move: discard
        /// a basic Energy card attached to Slaking or the attack does nothing; Slaking can't attack during your next turn.
        /// </summary>
        private sealed class SlakingRS : Mon
        {
            private static readonly Mon Critical = new DiscardBasicOrNothingMon("Critical Move", 1, true);

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !src.IsActive(me)) return false;
                return !src.Engine.AbilityBlocked(me, "Lazy");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Critical.HandleAttack(ctx, atk); }
        }

        /// <summary>Water Call (Poké-Power): once during your turn, attach a Water Energy card from your hand to your Active Pokémon. Hypno Splash: Asleep.</summary>
        private sealed class SwampertWaterCall : Mon
        {
            private const string Power = "Water Call";
            private static readonly Mon Splash = new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return CardsE1.NoCondition(ctx.Pokemon) && active != null && ctx.Engine.CanAttachFromHand(active)
                       && ctx.Me.Hand.Any(c => BasicOf(c.Def, EnergyType.Water));
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var water = ctx.Me.Hand.Where(c => BasicOf(c.Def, EnergyType.Water)).ToList();
                if (active == null || water.Count == 0) return;
                var card = water.All(c => c.Def == water[0].Def) ? water[0] : ctx.ChooseCards(water, 1, 1, "Water Call: attach which Water Energy card to " + active.Name + "?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.AttachEnergyCard(card, active, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>Invisible Hand: if the opponent's Active Pokémon is an Evolved Pokémon, any 1 card from your deck into your hand.</summary>
        private sealed class InvisibleHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Invisible Hand") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0 || !CardsE2.IsEvolved(ctx.Opp.Active)) return true;
                var found = ctx.SearchMyDeck(d => true, 1, "Invisible Hand: choose any 1 card", false);
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Repulsion: does nothing if the opponent has no Benched Pokémon; heads, they put the Defending Pokémon and everything attached into their hand.</summary>
        private sealed class RepulsionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Repulsion") return false;
                var defending = ctx.Defending;
                if (ctx.Opp.Bench.Count == 0 || defending == null) return Nothing(ctx);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>
        /// Stockpile: during your next turn, Spit Up's base damage is 70 (not 30) and Swallow's is 60 (not 20). Swallow: then remove
        /// damage counters from Pelipper equal to the damage it did.
        /// </summary>
        private sealed class StockpileMon : Mon
        {
            private static int Boost(EffectContext ctx, string attack)
            {
                int uid = ctx.Pokemon.Uid;
                return ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(e => e.TargetUid == uid && e.AttackName == attack).Sum(e => e.Bonus);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                switch (atk.Name)
                {
                    case "Stockpile":
                        if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                        if (!ctx.InPlay(me)) return true;
                        foreach (var boosted in new[] { "Spit Up", "Swallow" })
                        {
                            var boost = new AttackBoostEffect(me.Name + "'s " + boosted + " has 40 more base damage during its next turn (Stockpile)", ctx.Player, me.Uid, ctx.Turn + 2, boosted, 40);
                            boost.ActiveFromTurn = ctx.Turn + 2;
                            ctx.AddEffect(boost);
                        }
                        return true;
                    case "Spit Up":
                        ctx.DamageDefending(atk.Damage + Boost(ctx, "Spit Up"));
                        return true;
                    case "Swallow":
                        int done = ctx.DamageDefending(atk.Damage + Boost(ctx, "Swallow"));
                        if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, done);
                        return true;
                }
                return false;
            }
        }

        /// <summary>Energy Trans (Poké-Power): as often as you like, move a Grass Energy card from 1 of your Pokémon to another. Tail Rap: 2 coins.</summary>
        private sealed class SceptileEnergyTrans : Mon
        {
            private const string Trans = "Energy Trans";
            private static readonly Mon TailRap = new CoinsMon("Tail Rap", 2);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Trans) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && CardsCEL.EnergyTransReady(ctx, EnergyType.Grass);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { CardsCEL.UseEnergyTrans(ctx, Trans, EnergyType.Grass); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TailRap.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Slash: you may discard a Darkness Energy card attached to Sharpedo; if you do, 30 more.</summary>
        private sealed class DarkSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Slash") return false;
                var me = ctx.Pokemon;
                var dark = me.Energy.Where(c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Darkness)).ToList();
                bool bonus = false;
                if (dark.Count > 0 && !CardsBS1.CopyingWithMetronome && ctx.AskYesNo("Dark Slash: discard a Darkness Energy card for 30 more damage?"))
                    bonus = ctx.DiscardEnergyFrom(me, 1, "Dark Slash: discard which Darkness Energy card?", c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Darkness)) > 0;
                ctx.DamageDefending(atk.Damage + (bonus ? 30 : 0));
                return true;
            }
        }

        /// <summary>
        /// Natural Remedy (Poké-Body): once during your turn, attaching a Water Energy card from your hand to Swampert removes 1 damage
        /// counter from it (like Pokémon Park, only an attachment that heals uses it up). Water Arrow: 20 to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class SwampertNaturalRemedy : Mon
        {
            private const string Body = "Natural Remedy";
            private static readonly Mon Arrow = new WaterArrowMon();

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || energy == null || !BasicOf(energy.Def, EnergyType.Water)) return;
                if (src.State.CurrentPlayer != src.Player || me.DamageCounters == 0 || src.AbilityUsedThisTurn(Body)) return;
                src.MarkAbilityUsed(Body);
                Announce(src, Body);
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Arrow.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Smog: the Defending Pokémon is now Poisoned; 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class PoisonSmogMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Smog") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Healing Egg: 2 damage counters (1 if it has only 1) off each of your Pokémon except Chansey ex.</summary>
        private sealed class HealingEggMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Egg") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon != ctx.Pokemon && mon.DamageCounters > 0) ctx.Heal(mon, 20);
                return true;
            }
        }

        /// <summary>Energy Absorption: attach up to 2 Energy cards from your discard pile to Mewtwo ex.</summary>
        private sealed class EnergyAbsorptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Absorption") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0 && ctx.InPlay(me))
                    ctx.AttachCardsAnyWay(energy, 2, (c, m) => m == me, "Energy Absorption: attach an Energy card from your discard pile to " + me.Name);
                return true;
            }
        }

        /// <summary>Beat Up: a coin for each of your Pokémon in play; 20 for each heads.</summary>
        private sealed class BeatUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Beat Up") return false;
                int coins = ctx.MyPokemon().Count;
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }
    }
}
