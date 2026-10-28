using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Celestial Storm (CES, v83), part 2: Lightning, Psychic and Fighting (CES 49-83). Voltorb, Electrode-GX, Shuppet
    /// and Banette-GX were already in the pool (Hidden Fates Shiny Vault). Manectric's Electric Start lets a player
    /// going second put it face down during setup, and keep a hand whose only Pokémon is Manectric (your ruling, v83). Printed damage
    /// plus "does N damage to 2 of your opponent's Benched Pokémon" hits the Active Pokémon too (your ruling, v83: Chinchou's Spark).
    /// </summary>
    public static class CardsCES2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("CES-49", new BenchSpreadMon("Spark", 10, 2));
            r.RegisterByKey("CES-50", new LanturnCES());
            r.RegisterByKey("CES-52", new ManectricCES());
            r.RegisterByKey("CES-53", new DrawForEverybodyMon());
            r.RegisterByKey("CES-54", new DitchAndDrawMon());
            r.RegisterByKey("CES-55", new ZappyPomPomsMon());
            // Psychic
            r.RegisterByKey("CES-56", new MrMimeGXCES());
            r.RegisterByKey("CES-57", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("CES-58", new ComboMon(new CardsPOR.MemoryLockMon("Amnesia"),
                new CardsFST2.ConditionBonusMon("Swallow Up", 80, c => c.Defending != null && c.RemainingHp(c.Defending) < c.RemainingHp(c.Pokemon))));
            r.RegisterByKey("CES-59", new SwitchSelfMon("Bounce", false));
            r.RegisterByKey("CES-60", new CardsFST2.ConditionBonusMon("Mirror Step", 70, SameNameInPlay));
            r.RegisterByKey("CES-61", new LunatoneCES());
            r.RegisterByKey("CES-62", new SolrockCES());
            r.RegisterByKey("CES-64", new ConditionAttackMon("Perplex", SpecialCondition.Confused));
            r.RegisterByKey("CES-65", new BanetteCES());
            r.RegisterByKey("CES-67", new ComboMon(new CardsMEG.DefendingEnergyBonusMon("Psychic", 20), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            r.RegisterByKey("CES-68", new ComboMon(new CardsASC.SelfGuardMon("Reflect", 40), new CardsFLF1.NoResistanceMon("Psychic Corkscrew")));
            r.RegisterByKey("CES-69", new ComboMon(new SwitchSelfMon("Teleportation Burst", true), new SpearDiveMon()));
            r.RegisterByKey("CES-70", new LunalaCES());
            // Fighting
            r.RegisterByKey("CES-71", new ComboMon(new ScreechMon("Screech", 20), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("CES-72", new EndureMon("Endure"));
            r.RegisterByKey("CES-73", new ComboMon(new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters), new CardsLOT4.RapidSpinMon()));
            r.RegisterByKey("CES-74", new ParalyzeFlipMon("Leer"));
            r.RegisterByKey("CES-75", new DustDevilMon());
            r.RegisterByKey("CES-76", new EndureMon("Bide"));
            r.RegisterByKey("CES-77", new ComboMon(new CardsFST2.ConditionBonusMon("Strike of Enlightenment", 160, c => c.RemainingHp(c.Pokemon) <= 30), new RecoilMon("Spinning Kick", 30)));
            r.RegisterByKey("CES-78", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("CES-79", new ComboMon(new ParalyzeFlipMon("Psy Bolt"), new CardsASR1.TimesMon("Miraculous Spin", c => c.Me.Discard.Count(x => x.Def.Name == "Steven's Resolve"))));
            r.RegisterByKey("CES-80", new ComboMon(new CardsASC.ToolBonusMon("Enhanced Stomp", 20), new CardsSSP2.StompOffMon("Hammer Arm", 1)));
            r.RegisterByKey("CES-81", new ComboMon(new CardsAOR2.StadiumBlowMon("Wreck", 50), new CardsTWM1.SelfDiscardMon("Ground Slash", 1)));
            r.RegisterByKey("CES-82", new ComboMon(new ConditionAttackMon("Eerie Light", SpecialCondition.Confused), new CardsTWM1.HealSelfMon("Absorb Life", 20), new SandyFearMon()));
            r.RegisterByKey("CES-83", new MiniorCES());
        }

        // ================================================================== Shared helpers

        /// <summary>"If 1 of your opponent's Pokémon in play has the same name as 1 of your Pokémon in play" (Mirror Step).</summary>
        internal static bool SameNameInPlay(EffectContext ctx)
        {
            var mine = ctx.MyPokemon().Select(m => m.Def.Name).ToList();
            return ctx.OpponentPokemon().Any(o => mine.Contains(o.Def.Name));
        }

        /// <summary>
        /// Printed damage to the Active Pokémon, and N damage to up to `count` of the opponent's Benched Pokémon (Spark). The printed
        /// number hits the Active Pokémon as well (your ruling, v83).
        /// </summary>
        internal sealed class BenchSpreadMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly int count;

            public BenchSpreadMon(string attack, int amount, int count)
            {
                this.attack = attack;
                this.amount = amount;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(count, bench.Count);
                var targets = bench.Count <= n ? bench : ctx.ChoosePokemonMany(bench, n, n, attack + ": choose " + n + " of your opponent's Benched Pokémon to do " + amount + " damage to");
                foreach (var t in targets) ctx.DamageTarget(t, amount);
                return true;
            }
        }

        /// <summary>"During your next turn, the Defending Pokémon takes N more damage from attacks (after Weakness and Resistance)" (Screech).</summary>
        internal sealed class ScreechMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public ScreechMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var effect = new DamageTakenEffect(defending.Name + " takes " + amount + " more damage from attacks during your next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 2, amount);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>
        /// Endure / Bide: flip a coin; on heads, if this Pokémon would be Knocked Out by damage from an attack during the opponent's
        /// next turn, it isn't, and its remaining HP becomes 10.
        /// </summary>
        internal sealed class EndureMon : Mon
        {
            private readonly string attack;

            public EndureMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                ctx.AddEffect(new EndureEffect(me.Name + " can't be Knocked Out by damage from attacks during your opponent's next turn; its HP would stay at 10 (" + attack + ")",
                    ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class EndureEffect : ContinuousEffect
        {
            public EndureEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Energy Grounding: when 1 of your other Pokémon is Knocked Out by damage from an opponent's attack, you may move a basic
        /// Energy card from it to this Pokémon. Lightning Strike: you may discard all Lightning Energy from this Pokémon for 70 more.
        /// </summary>
        private sealed class LanturnCES : Mon
        {
            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null || mon == null || mon == me || !src.InPlay(me)) return;
                var basics = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return;
                if (!src.AskYesNo("Energy Grounding: move a basic Energy card from " + mon.Name + " to " + me.Name + "?")) return;
                var pick = basics.Count == 1 ? basics : src.ChooseCards(basics, 1, 1, "Energy Grounding: move which basic Energy card to " + me.Name + "?");
                if (pick.Count == 0) return;
                Announce(src, "Energy Grounding");
                src.Engine.MoveEnergy(mon, me, pick[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(x => CardsTEU1.ProvidesType(ctx, me, x, EnergyType.Lightning)).ToList();
                bool more = false;
                if (lightning.Count > 0 && ctx.AskYesNo("Lightning Strike: discard all Lightning Energy from " + me.Name + " for 70 more damage?"))
                {
                    foreach (var e in lightning) ctx.Engine.DiscardEnergy(me, e);
                    more = true;
                }
                ctx.DamageDefending(atk.Damage + (more ? 70 : 0));
                return true;
            }
        }

        /// <summary>
        /// Electric Start: going second, it may be put face down as your Active Pokémon or on your Bench during setup (the engine does
        /// that). Double Charge: you may attach up to 2 basic Energy cards from your hand to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class ManectricCES : Mon
        {
            public override bool StartsFaceDownWhenGoingSecond { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (bench.Count == 0 || energy.Count == 0) return true;
                if (!ctx.AskYesNo("Double Charge: attach up to 2 basic Energy cards from your hand to 1 of your Benched Pokémon?")) return true;
                var target = ctx.ChoosePokemon(bench, "Double Charge: attach the Energy to which Benched Pokémon?");
                if (target == null) return true;
                ctx.AttachCardsAnyWay(energy, 2, (card, mon) => mon == target, "Double Charge: attach basic Energy to " + target.Name);
                return true;
            }
        }

        /// <summary>Draw for Everybody: shuffle your hand into your deck, then draw a card for each Benched Pokémon (both players').</summary>
        private sealed class DrawForEverybodyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draw for Everybody") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Ditch and Draw: you may discard any number of cards from your hand, then draw until you have 5 cards.</summary>
        private sealed class DitchAndDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ditch and Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count > 0)
                    foreach (var c in ctx.ChooseCards(hand, 0, hand.Count, "Ditch and Draw: discard any number of cards from your hand (then draw until you have 5)"))
                        ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DrawUntil(5);
                return true;
            }
        }

        /// <summary>Zappy Pom-Poms: 30 damage to each Pokémon-GX and Pokémon-EX (both yours and your opponent's).</summary>
        private sealed class ZappyPomPomsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zappy Pom-Poms") return false;
                foreach (var t in ctx.OpponentPokemon(m => m.Def.IsGXorEX)) ctx.DamageTarget(t, 30);
                foreach (var t in ctx.MyPokemon(m => m.Def.IsGXorEX)) ctx.DamageTarget(t, 30);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Magic Evens: prevent all damage done to this Pokémon by the opponent's attacks if it's exactly 20, 40, ... 260 (after every
        /// modifier: Choice Band counts first, Compendium 2018-08-02). Breakdown: 1 damage counter on the opponent's Active Pokémon for
        /// each card in their hand. Life Trick-GX: heal all damage from this Pokémon.
        /// </summary>
        private sealed class MrMimeGXCES : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player) return false;
                return damage >= 20 && damage <= 260 && damage % 20 == 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Breakdown")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var defending = ctx.Defending;
                    int n = ctx.Opp.Hand.Count;
                    if (defending != null && n > 0) ctx.PlaceCounters(defending, n);
                    return true;
                }
                if (atk.Name == "Life Trick-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) CardsTEU1.HealAll(ctx, ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sol Shade: if you have Solrock in play, Fire Pokémon in play (both players') have no Abilities, except Pokémon-GX and
        /// Pokémon-EX. The printed type is checked, so type changes from Abilities can't loop back into this check.
        /// </summary>
        private sealed class LunatoneCES : Mon
        {
            private static readonly Mon Psyshock = new ParalyzeFlipMon("Psyshock");

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                if (mon == null || mon.Def.IsGXorEX) return false;
                if (mon.Def.PokemonType != EnergyType.Fire && mon.Def.SecondType != EnergyType.Fire) return false;
                return src.Me.InPlay().Any(m => m.Def.Name == "Solrock");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psyshock.HandleAttack(ctx, atk); }
        }

        /// <summary>Sunbeam: the maximum HP of each of your Lunatone in play is 130. Scorching Light: flip; heads Paralyzed, tails Burned.</summary>
        private sealed class SolrockCES : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && mon.Owner == src.Player && mon.Def.Name == "Lunatone" ? 130 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Light") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, heads ? SpecialCondition.Paralyzed : SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>
        /// Red Eyes: when you play this Pokémon from your hand to evolve, you may put a Basic Pokémon from the opponent's discard pile
        /// onto their Bench. Enemy Show: 1 damage counter for each of the opponent's Pokémon in play, placed on their Pokémon in any way.
        /// </summary>
        private sealed class BanetteCES : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var basics = ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                bool ready = basics.Count > 0 && ctx.Engine.BenchHasRoom(ctx.Opponent);
                if (!TriggerOnce(ctx, "Red Eyes", ready, "Use Red Eyes? Put a Basic Pokémon from your opponent's discard pile onto their Bench.")) return;
                var pick = ctx.ChooseCards(basics, 1, 1, "Red Eyes: put which Basic Pokémon onto your opponent's Bench?");
                if (pick.Count == 0 || !ctx.Opp.Discard.Remove(pick[0])) return;
                ctx.Engine.PutOntoBench(ctx.Opponent, pick[0], false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enemy Show") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.OpponentPokemon().Count;
                ctx.PlaceCountersAnyWay(n, () => ctx.OpponentPokemon(m => ctx.CanAffect(m)));
                return true;
            }
        }

        /// <summary>Spear Dive: 50 damage to 1 of the opponent's Pokémon, not affected by Weakness or Resistance (no printed damage).</summary>
        private sealed class SpearDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spear Dive") return false;
                ctx.DamageAnyOpponent(50, true, false);
                return true;
            }
        }

        /// <summary>Shadow Shield: with any Psychic Energy attached, this Pokémon takes 20 less damage from attacks. Full Moon Ray: 20 more for each Energy on the Defending Pokémon.</summary>
        private sealed class LunalaCES : Mon
        {
            private static readonly Mon Ray = new CardsMEG.DefendingEnergyBonusMon("Full Moon Ray", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null) return damage;
                return src.Engine.HasEnergyOfType(me, EnergyType.Psychic) ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Dust Devil: 20 damage to each Pokémon that isn't a Fighting Pokémon (both yours and your opponent's).</summary>
        private sealed class DustDevilMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dust Devil") return false;
                foreach (var t in ctx.OpponentPokemon(m => !ctx.Engine.IsType(m, EnergyType.Fighting))) ctx.DamageTarget(t, 20);
                foreach (var t in ctx.MyPokemon(m => !ctx.Engine.IsType(m, EnergyType.Fighting))) ctx.DamageTarget(t, 20);
                return true;
            }
        }

        /// <summary>
        /// Sandy Fear-GX: look at the top 13 cards of the opponent's deck and discard any number of Pokémon there; 60 damage for each
        /// card discarded. The opponent shuffles the rest back.
        /// </summary>
        private sealed class SandyFearMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sandy Fear-GX") return false;
                var deck = ctx.Opp.Deck;
                int discarded = 0;
                if (deck.Count > 0 && ctx.Engine.DeckDiscardBlocked(ctx.Opponent))
                    ctx.Log("Cards in " + ctx.Engine.Their(ctx.Opponent) + " deck can't be discarded right now.");
                else if (deck.Count > 0)
                {
                    var top = deck.Take(Math.Min(13, deck.Count)).ToList();
                    ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
                    var mons = top.Where(x => x.Def.IsPokemon).ToList();
                    if (mons.Count == 0)
                        ctx.Engine.ShowCards(ctx.Player, top, "Sandy Fear-GX", "There's no Pokémon in the top " + top.Count + " cards of your opponent's deck", ctx.Source);
                    else
                    {
                        var picks = ctx.ChooseCards(mons, 0, mons.Count, "Sandy Fear-GX: discard any number of these Pokémon (60 damage each)");
                        var gone = new List<CardInstance>();
                        foreach (var c in picks)
                            if (deck.Remove(c)) { ctx.Opp.Discard.Add(c); gone.Add(c); }
                        discarded = gone.Count;
                        if (discarded > 0)
                            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " discarded " + string.Join(", ", gone.Select(x => x.Def.Name).ToArray()) + " from their deck (Sandy Fear-GX).");
                    }
                    ctx.Engine.ShuffleDeck(ctx.Opponent);
                }
                if (discarded > 0) ctx.DamageDefending(atk.Damage * discarded);
                else ctx.Log("Sandy Fear-GX did no damage.");
                return true;
            }
        }

        /// <summary>
        /// Falling Star: once during your turn, if this Pokémon is in your hand and your Bench isn't full, you may move your Active
        /// Pokémon to your Bench and play this Pokémon as your new Active Pokémon. Swift: not affected by Weakness, Resistance or any
        /// effects on the Defending Pokémon.
        /// </summary>
        private sealed class MiniorCES : Mon
        {
            private const string Star = "Falling Star";
            private static readonly Mon Swift = new CardsCRZ1.PiercingMon("Swift");

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && ctx.Me.Active != null && ctx.Engine.BenchHasRoom(ctx.Player)
                       && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def) && !ctx.NamedLimitUsedThisTurn(Star + "#" + ctx.Card.Id);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!CanUseFromHand(ctx)) return;
                ctx.MarkNamedLimit(Star + "#" + ctx.Card.Id);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { ctx.Card }, "played with " + Star);
                ctx.Me.Hand.Remove(ctx.Card);
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                if (mon != null && ctx.Me.Bench.Contains(mon)) ctx.Engine.SwitchActive(ctx.Player, mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swift.HandleAttack(ctx, atk); }
        }
    }
}
