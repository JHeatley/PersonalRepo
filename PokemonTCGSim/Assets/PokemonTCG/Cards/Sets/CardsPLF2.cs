using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Freeze (PLF, v110): Fighting to Colorless Pokémon (58-98), Empoleon (117) and the Trainers. Float Stone, Superior Energy
    /// Retrieval, Plasma Energy, Professor Juniper, Max Potion, Ultra Ball and the Sigilyph, Garbodor and Garchomp secret rares were
    /// already in the pool.
    /// </summary>
    public static class CardsPLF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLF-58", new CardsASR1.PlusMon("Lovestrike", 40, c => c.Me.Bench.Count(m => m.Name == "Nidoqueen")));
            r.RegisterByKey("PLF-60", new ComboMon(new CoinsMon("Fury Swipes", 3), new CardsBS1.KarateChopMon()));
            r.RegisterByKey("PLF-61", new CoinsMon("Swing Around", 2));
            r.RegisterByKey("PLF-62", new CardsASR1.TimesMon("Continuous Slap", c => c.FlipUntilTails()));
            r.RegisterByKey("PLF-63", new ComboMon(new ParalyzeFlipMon("Fake Out"), new PivotThrowMon()));
            r.RegisterByKey("PLF-64", new UmbreonDarkShade());
            r.RegisterByKey("PLF-65", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("PLF-66", new ComboMon(new CardsLTR1.SpreadMon("Hail", 10), new VilifyMon()));
            r.RegisterByKey("PLF-67", new ComboMon(new CardsASR1.PlusMon("Mind Jack", 20, c => c.Opp.Bench.Count), new CardsASR1.RevealHandAttackMon("Fearsome Shadow")));
            r.RegisterByKey("PLF-68", new FlipBonusMon("Gentle Slap", 20));
            r.RegisterByKey("PLF-70", new PistonHeadbuttMon());
            r.RegisterByKey("PLF-73", new CardsFST2.ConditionBonusMon("Dragon Slayer", 40, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Dragon)));
            r.RegisterByKey("PLF-74", new CardsASC.FlipDiscardDefendingEnergyMon("Cut Down"));
            r.RegisterByKey("PLF-75", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("PLF-76", new CoinsMon("Slam", 2));
            r.RegisterByKey("PLF-77", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("PLF-78", new ComboMon(new CardsGRI2.TractorbeamMon(40), new CardsLOT4.PeckOffMon("Obsidian Fang")));
            r.RegisterByKey("PLF-79", new MetalDefenderMon());
            r.RegisterByKey("PLF-80", new ComboMon(new AstonishMon(), new BigOlBiteMon()));
            r.RegisterByKey("PLF-81", new CardsTWM1.HealSelfMon("Shed Skin", 20));
            r.RegisterByKey("PLF-82", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("PLF-83", new ComboMon(new CardsDAA1.ItemLockAttackMon("Deafen"), new CardsTWM1.HealSelfMon("Healwing", 30)));
            r.RegisterByKey("PLF-84", new ComboMon(new DragonVortexMon(), new TriBulletMon()));
            r.RegisterByKey("PLF-85", new LatiasBrightDown());
            r.RegisterByKey("PLF-86", new ComboMon(new NoRetreatMon("Mach Flight"), new CardsTWM1.SelfDiscardMon("Luster Purge", 99)));
            r.RegisterByKey("PLF-87", new TailsNothingMon("Lunge"));
            r.RegisterByKey("PLF-88", new TransferJunkMon());
            r.RegisterByKey("PLF-89", new ComboMon(new CardsMEG.WeakenDefendingMon("Growl", 20), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("PLF-90", new SignsOfEvolutionMon());
            r.RegisterByKey("PLF-91", new CardsSSH1.TeatimeMon("Dual Draw", 2, 2));
            r.RegisterByKey("PLF-92", new ComboMon(new CardsASR1.TimesMon("Powerful Vision", c => c.Opp.Hand.Count), new DodgeMon("Fly", true)));
            r.RegisterByKey("PLF-93", new MaxMilkMon());
            r.RegisterByKey("PLF-94", new KecleonColorChange());
            r.RegisterByKey("PLF-96", new CardsFST1.FlipRecoilMon("Take Down", 10));
            r.RegisterByKey("PLF-97", new CardsBRS1.StrongBreezeMon());
            r.RegisterByKey("PLF-98", new TornadusExMon());
            r.RegisterByKey("PLF-117", new EmpoleonDivingDraw());

            r.RegisterByName("Frozen City", new FrozenCity());
            r.RegisterByName("Ghetsis", new Ghetsis());
            r.RegisterByName("Shadow Triad", new ShadowTriad());
            r.RegisterByName("Team Plasma Badge", new TeamPlasmaBadge());
            r.RegisterByName("Team Plasma Ball", new TeamPlasmaBall());
            r.RegisterByName("Life Dew", new LifeDew());
            r.RegisterByName("Rock Guard", new RockGuard());
        }

        // ================================================================== Fighting, Darkness

        /// <summary>Pivot Throw: 90, and this Pokémon takes 20 more damage from attacks during the opponent's next turn.</summary>
        private sealed class PivotThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pivot Throw") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new CardsUNM3.TakeMoreEffect(me.Name + " takes 20 more damage from attacks (Pivot Throw)", ctx.Player, me.Uid, ctx.Turn + 1, 20));
                return true;
            }
        }

        /// <summary>Dark Shade: each of your Team Plasma Pokémon in play gets +20 HP.</summary>
        private sealed class UmbreonDarkShade : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.Engine.IsTeamPlasma(mon)) return hp;
                return hp + 20;
            }
        }

        /// <summary>Vilify: discard as many Pokémon as you like from your hand; 30 damage for each one.</summary>
        private sealed class VilifyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vilify") return false;
                var pokemon = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                int n = 0;
                if (pokemon.Count > 0)
                {
                    var picks = ctx.ChooseCards(pokemon, 0, pokemon.Count, "Vilify: discard any number of Pokémon from your hand (30 damage each)");
                    foreach (var card in picks)
                    {
                        if (!ctx.Me.Hand.Contains(card)) continue;
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>
        /// Piston Headbutt: 30, then move an Energy from the Defending Pokémon to 1 of the opponent's Benched Pokémon. Moved onto a
        /// Pokémon protected from the attack (Silver Mirror), the Energy is discarded instead.
        /// </summary>
        private sealed class PistonHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Piston Headbutt") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Piston Headbutt: move which Energy from " + defending.Name + "?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Piston Headbutt: move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (to == null) return true;
                // Energy moved onto a Pokémon the attack can't affect (Silver Mirror) is discarded instead (2013-09/10 ruling).
                if (!ctx.CanAffect(to)) ctx.Engine.DiscardEnergy(defending, card);
                else ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Metal Defender: 50, and this Pokémon has no Weakness during the opponent's next turn.</summary>
        private sealed class MetalDefenderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Defender") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new NoWeaknessEffect(me.Name + " has no Weakness during your opponent's next turn (Metal Defender)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class NoWeaknessEffect : ContinuousEffect
        {
            public NoWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon != null && mon.Uid == TargetUid ? (EnergyType?)null : weakness;
            }
        }

        /// <summary>Astonish: flip a coin; heads, a random card from the opponent's hand is revealed and shuffled into their deck.</summary>
        internal sealed class AstonishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Astonish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0 || !ctx.Flip()) return true;
                var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "revealed a random card from the hand (Astonish)");
                ctx.Opp.Hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Big Ol' Bite: 30, heal 30 from this Pokémon; the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class BigOlBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Ol' Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Dragon Vortex: 20 for each Water and Lightning Energy card in your discard pile, then shuffle them into your deck.</summary>
        private sealed class DragonVortexMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Vortex") return false;
                var cards = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water) || CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
                ctx.DamageDefending(atk.Damage * cards.Count);
                if (cards.Count > 0)
                {
                    ctx.DiscardToDeck(cards);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Tri Bullet: 30 damage to 3 of the opponent's Pokémon (no Weakness or Resistance on the Bench).</summary>
        private sealed class TriBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tri Bullet") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                int n = Math.Min(3, all.Count);
                var picks = n == all.Count ? all : ctx.ChoosePokemonMany(all, n, n, "Tri Bullet: choose 3 of your opponent's Pokémon (30 damage each)");
                foreach (var target in picks) ctx.DamageTarget(target, 30);
                return true;
            }
        }

        /// <summary>Bright Down: prevent all effects of attacks, damage included, done to this Pokémon by the opponent's Pokémon that have Abilities. Barrier Break.</summary>
        private sealed class LatiasBrightDown : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !src.Engine.HasAbility(a)) return damage;
                return 0;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && src.Engine.HasAbility(attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Barrier Break") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Transfer Junk: put a Team Plasma Pokémon, a Team Plasma Trainer and a Team Plasma Energy card from your discard pile into your hand.</summary>
        private sealed class TransferJunkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Transfer Junk") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var picks = new List<CardInstance>();
                Pick(ctx, picks, d => d.IsPokemon, "Pokémon");
                Pick(ctx, picks, d => d.IsTrainer, "Trainer card");
                Pick(ctx, picks, d => d.IsEnergy, "Energy card");
                if (picks.Count > 0) ctx.DiscardToHand(picks);
                return true;
            }

            private static void Pick(EffectContext ctx, List<CardInstance> picks, Func<CardDef, bool> kind, string what)
            {
                var pile = ctx.Me.Discard.Where(c => c.Def.IsTeamPlasma && kind(c.Def)).ToList();
                if (pile.Count == 0) return;
                picks.Add(ctx.ChooseCards(pile, 1, 1, "Transfer Junk: choose a Team Plasma " + what)[0]);
            }
        }

        /// <summary>Signs of Evolution: search your deck for up to 3 Pokémon of different types that evolve from Eevee.</summary>
        private sealed class SignsOfEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Signs of Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var types = new List<EnergyType>();
                for (int i = 0; i < 3; i++)
                {
                    var taken = types.ToList();
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == "Eevee" && !taken.Contains(d.PokemonType), 1,
                        "Signs of Evolution: choose a Pokémon that evolves from Eevee (" + (i + 1) + " of up to 3, each a different type)");
                    if (found.Count == 0) break;
                    types.Add(found[0].Def.PokemonType);
                    ctx.Me.Hand.AddRange(found);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Max Milk: heal all damage from 1 of your Pokémon, then discard all Energy attached to this Pokémon.</summary>
        private sealed class MaxMilkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Milk") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count > 0)
                {
                    var target = ctx.ChoosePokemon(hurt, "Max Milk: heal all damage from which of your Pokémon?");
                    if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                }
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0) ctx.Engine.DiscardAllEnergy(me);
                return true;
            }
        }

        /// <summary>
        /// Color Change: while Active, Kecleon is the same type as the opponent's Active Pokémon (the types printed on that card).
        /// Imittack: choose 1 of the Defending Pokémon's attacks and use it if Kecleon has the Energy for it.
        /// </summary>
        private sealed class KecleonColorChange : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || !src.IsActive(me)) return;
                var theirs = src.Opp.Active;
                if (theirs == null) return;
                types.Clear();
                types.Add(theirs.Def.PokemonType);
                if (theirs.Def.SecondType.HasValue && theirs.Def.SecondType.Value != theirs.Def.PokemonType) types.Add(theirs.Def.SecondType.Value);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Imittack") return false;
                var defending = ctx.Defending;
                CardDef source;
                int index;
                if (defending == null || !ChooseCopiedAttack(ctx, defending, "Imittack: choose 1 of the Defending Pokémon's attacks", out source, out index)) return Nothing(ctx);
                if (!ctx.Engine.CanPayAttack(ctx.Pokemon, source.Attacks[index]))
                {
                    ctx.Log(ctx.Pokemon.Name + " doesn't have the Energy for " + source.Attacks[index].Name + ", so Imittack did nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Windfall: shuffle your hand into your deck and draw 6 cards. Jet Blast: 30 more per Plasma Energy attached.</summary>
        internal sealed class TornadusExMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Windfall")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(6);
                    return true;
                }
                if (atk.Name == "Jet Blast")
                {
                    ctx.DamageDefending(atk.Damage + 30 * CardsPLB1.PlasmaEnergy(ctx.Pokemon));
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Diving Draw: once during your turn, you may discard a card from your hand; then draw 2 (old "discard, then draw" wording:
        /// usable with an empty deck, Compendium 2012-10-04). Attack Command: 10 for each Pokémon in play.
        /// </summary>
        private sealed class EmpoleonDivingDraw : Mon
        {
            protected override string Activated { get { return "Diving Draw"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                var pick = ctx.ChooseCards(hand, 1, 1, "Diving Draw: discard which card from your hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attack Command") return false;
                int n = ctx.MyPokemon().Count + ctx.OpponentPokemon().Count;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Frozen City: whenever a player attaches an Energy from their hand to 1 of their Pokémon that isn't a Team Plasma Pokémon, put 2 damage counters on it.</summary>
        private sealed class FrozenCity : CardScript
        {
            /// <summary>Before Eevee's Energy Evolution (Compendium 2014-08-28).</summary>
            public override bool ReactsFirstToAttachFromHand { get { return true; } }

            private static void Chill(EffectContext src, PokemonInPlay target)
            {
                if (target == null || src.Engine.IsTeamPlasma(target)) return;
                src.Engine.PlaceDamageCounters(target, 2, "Frozen City");
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Chill(src, target); }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Chill(src, target); }
        }

        /// <summary>Ghetsis: the opponent reveals their hand and shuffles all Item cards from it into their deck; draw a card for each.</summary>
        private sealed class Ghetsis : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                var items = ctx.Opp.Hand.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return;
                foreach (var card in items)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Ghetsis: " + ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + items.Count + (items.Count == 1 ? " Item card" : " Item cards") + " into their deck.");
                ctx.Draw(items.Count);
            }
        }

        /// <summary>Shadow Triad: put a Team Plasma card from your discard pile into your hand.</summary>
        private sealed class ShadowTriad : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsTeamPlasma).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                var pick = ctx.ChooseCards(pile, 1, 1, "Shadow Triad: put which Team Plasma card into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
            }
        }

        /// <summary>Team Plasma Badge: the Pokémon it's attached to is a Team Plasma Pokémon.</summary>
        private sealed class TeamPlasmaBadge : CardScript
        {
            public override bool MakesHolderTeamPlasma(EffectContext src) { return src.Pokemon != null; }
        }

        /// <summary>Team Plasma Ball: search your deck for a Team Plasma Pokémon.</summary>
        private sealed class TeamPlasmaBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.IsTeamPlasma, 1, "Team Plasma Ball: choose a Team Plasma Pokémon");
            }
        }

        /// <summary>Life Dew (ACE SPEC): if the Pokémon it's attached to is Knocked Out, the opponent takes 1 fewer Prize card.</summary>
        private sealed class LifeDew : CardScript
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return src.Pokemon != null && knockedOut == src.Pokemon ? Math.Max(0, prizes - 1) : prizes;
            }
        }

        /// <summary>Rock Guard (ACE SPEC): when the Active holder is damaged by an opponent's attack, put 6 damage counters on the attacker.</summary>
        private sealed class RockGuard : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 6, "Rock Guard");
            }
        }
    }
}
