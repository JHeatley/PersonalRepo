using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Wizards Black Star Promos (WP 1-53, 1999-2002). Every card came out before Black &amp; White, so they're Unlimited only.
    /// WP 9 and WP 14 are second prints of WP 8 (Mew) and WP 3 (Mewtwo).
    ///
    /// Old mechanics handled here or by the engine (v48):
    /// - Pokémon Powers work like Abilities but aren't Abilities (RULES_NOTES 144); Suicune's Pure Body is a Poké-Body.
    /// - Baby Pokémon (Pichu, Cleffa, Igglybuff): the Baby Rule coin flip, and evolving into any card of their Basic's name
    ///   (engine: GameEngine.PerformAttack and CanEvolveInto; your rulings).
    /// - Lightning Rod markers and Coloring counters stay until the Pokémon evolves, devolves or leaves play (your ruling).
    /// - "Benching either Pokémon ends this effect" (Growl, Snivel, Leer): the effect ends when this Pokémon or the Defending
    ///   Pokémon moves to the Bench.
    /// </summary>
    public static class CardsWP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("WP-1", new GrowlPikachu());
            r.RegisterByKey("WP-2", new LightScreenElectabuzz());
            r.RegisterByKey("WP-3", new EnergyAbsorptionMewtwo());
            r.RegisterByKey("WP-4", new RechargePikachu());
            r.RegisterByKey("WP-5", new DragoniteWP());
            r.RegisterByKey("WP-6", new ArcanineWP());
            r.RegisterByKey("WP-7", new JigglypuffWP());
            r.RegisterByKey("WP-8", new DevolutionBeamMew());
            r.RegisterByKey("WP-10", new MeowthWP());
            r.RegisterByKey("WP-11", new EeveeWP());
            r.RegisterByKey("WP-12", new EnergyControlMewtwo());
            r.RegisterByKey("WP-13", new VenusaurWP());
            r.RegisterByKey("WP-15", new CoolPorygonWP());
            r.RegisterByKey("WP-16", new ComputerError());
            r.RegisterByKey("WP-17", new DarkPersianWP());
            r.RegisterByKey("WP-18", new TeamRocketsMeowthWP());
            r.RegisterByKey("WP-19", new SabrinasAbraWP());
            r.RegisterByKey("WP-20", new PsyduckWP());
            r.RegisterByKey("WP-21", new MoltresWP());
            r.RegisterByKey("WP-22", new ArticunoWP());
            r.RegisterByKey("WP-23", new ZapdosWP());
            r.RegisterByKey("WP-24", new BirthdayPikachu());
            r.RegisterByKey("WP-25", new FlyingPikachuWP());
            r.RegisterByKey("WP-26", new ScratchPikachu());
            r.RegisterByKey("WP-27", new AgilityPikachu());
            r.RegisterByKey("WP-28", new PlainMon()); // only plain attacks
            r.RegisterByKey("WP-29", new MarillWP());
            r.RegisterByKey("WP-30", new TogepiWP());
            r.RegisterByKey("WP-31", new CleffaWP());
            r.RegisterByKey("WP-32", new SmeargleWP());
            r.RegisterByKey("WP-33", new ScizorWP());
            r.RegisterByKey("WP-34", new EnteiWP());
            r.RegisterByKey("WP-35", new PichuWP());
            r.RegisterByKey("WP-36", new IgglybuffWP());
            r.RegisterByKey("WP-37", new HitmontopWP());
            r.RegisterByKey("WP-38", new UnownJWP());
            r.RegisterByKey("WP-39", new MisdreavusWP());
            r.RegisterByKey("WP-40", new PokemonCenterWP());
            r.RegisterByKey("WP-41", new LuckyStadium());
            r.RegisterByKey("WP-42", new PokemonTower());
            r.RegisterByKey("WP-43", new MachampWP());
            r.RegisterByKey("WP-44", new MagmarWP());
            r.RegisterByKey("WP-45", new ScytherWP());
            r.RegisterByKey("WP-46", new LightningRodElectabuzz());
            r.RegisterByKey("WP-47", new NeutralShieldMew());
            r.RegisterByKey("WP-48", new AuroraVeilArticuno());
            r.RegisterByKey("WP-49", new SnorlaxWP());
            r.RegisterByKey("WP-50", new CelebiWP());
            r.RegisterByKey("WP-51", new RapidashWP());
            r.RegisterByKey("WP-52", new HoOhWP());
            r.RegisterByKey("WP-53", new SuicuneWP());
        }

        // ================================================================== Shared helpers

        /// <summary>Pokémon Powers of this era: "This power can't be used if this Pokémon is Asleep, Confused, or Paralyzed."</summary>
        private static bool NotAsleepConfusedOrParalyzed(PokemonInPlay mon)
        {
            return mon != null && !mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Confused) && !mon.Has(SpecialCondition.Paralyzed);
        }

        /// <summary>"Evolved Pokémon": something is on top of its Basic card (Antique Fossils and LEGEND halves aren't evolved).</summary>
        private static bool IsEvolvedPokemon(PokemonInPlay mon)
        {
            return mon != null && mon.Stack.Count > 1 && !mon.Def.IsFossil && !mon.Def.IsMultiCard;
        }

        /// <summary>Damage, then "Flip a coin. If heads, the Defending Pokémon is now [condition]."</summary>
        private static void DamageAndFlipCondition(EffectContext ctx, int damage, SpecialCondition condition)
        {
            var defending = ctx.Defending;
            bool heads = ctx.Flip();
            ctx.DamageDefending(damage);
            if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
        }

        /// <summary>Quick Attack: "Flip a coin. If heads, this attack does 10 damage plus 20 more damage."</summary>
        private static void QuickAttack(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 20 : 0));
        }

        /// <summary>Agility, Fly: "If heads, prevent all effects of attacks, including damage, done to this Pokémon during your opponent's next turn."</summary>
        private static void DamageAndMaybeProtect(EffectContext ctx, int damage, bool protect)
        {
            ctx.DamageDefending(damage);
            if (protect) ctx.ProtectSelfNextTurn();
        }

        /// <summary>Growl (after Weakness and Resistance) and Snivel (before): less damage from the Defending Pokémon's attacks next turn.</summary>
        private static void DefendingDoesLessToMe(EffectContext ctx, int amount, bool beforeWeakness, string attackName)
        {
            var defending = ctx.Defending;
            var self = ctx.Pokemon;
            if (defending == null || !ctx.InPlay(self)) return;
            ctx.AddEffect(new DefenderDamageEffect(self.Name + " takes " + amount + " less damage from " + defending.Name + "'s attacks during the opponent's next turn (" + attackName + ")",
                ctx.Player, self.Uid, ctx.Turn + 1, defending.Uid, ctx.Turn, amount, beforeWeakness));
        }

        private static readonly EnergyType[] ChoosableTypes =
        {
            EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic,
            EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Dragon, EnergyType.Fairy
        };

        /// <summary>"A type of your choice other than [C]".</summary>
        private static EnergyType ChooseType(EffectContext ctx, string prompt)
        {
            var labels = ChoosableTypes.Select(t => t.ToString()).ToArray();
            return ChoosableTypes[ctx.ChooseOption(prompt, labels)];
        }

        /// <summary>
        /// The Defending Pokémon is still the same Pokémon in the Active Spot, and hasn't been moved to the Bench since the
        /// effect began ("Benching either Pokémon ends this effect").
        /// </summary>
        private static bool StillTheDefender(GameEngine engine, PokemonInPlay mon, int defenderUid, int sinceTurn)
        {
            return mon != null && mon.Uid == defenderUid && engine.State.IsActive(mon) && mon.MovedToActiveOnTurn <= sinceTurn;
        }

        // ================================================================== Timed effects

        /// <summary>Growl / Snivel: damage from the Defending Pokémon's attacks to this Pokémon is reduced during the opponent's next turn.</summary>
        private sealed class DefenderDamageEffect : ContinuousEffect
        {
            private readonly int defenderUid;
            private readonly int sinceTurn;
            private readonly int amount;
            private readonly bool beforeWeakness;

            public DefenderDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn, int defenderUid, int sinceTurn, int amount, bool beforeWeakness)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.defenderUid = defenderUid;
                this.sinceTurn = sinceTurn;
                this.amount = amount;
                this.beforeWeakness = beforeWeakness;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (!beforeWeakness || target == null || target.Uid != TargetUid || !StillTheDefender(engine, attacker, defenderUid, sinceTurn)) return damage;
                return damage - amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (beforeWeakness || info.Target.Uid != TargetUid || !StillTheDefender(engine, info.Attacker, defenderUid, sinceTurn)) return damage;
                return damage - amount;
            }
        }

        /// <summary>Leer: the Defending Pokémon can't attack this Pokémon during the opponent's next turn (while both stay Active).</summary>
        private sealed class LeerEffect : ContinuousEffect
        {
            private readonly int defenderUid;
            private readonly int sinceTurn;

            public LeerEffect(string name, int owner, int targetUid, int expiresAfterTurn, int defenderUid, int sinceTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.defenderUid = defenderUid;
                this.sinceTurn = sinceTurn;
            }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                var scizor = engine.State.FindPokemon(TargetUid);
                return scizor != null && engine.State.IsActive(scizor) && StillTheDefender(engine, mon, defenderUid, sinceTurn);
            }
        }

        /// <summary>Light Screen: attack damage to this Pokémon (after Weakness and Resistance) is halved, rounded down to the nearest 10.</summary>
        private sealed class HalveDamageEffect : ContinuousEffect
        {
            public HalveDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || !info.AgainstOpponent || damage <= 0) return damage;
                return damage / 2 / 10 * 10;
            }
        }

        /// <summary>Texture Magic: Cool Porygon's Resistance is another type until it goes to the Bench or evolves.</summary>
        private sealed class ResistanceChangeEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public ResistanceChangeEffect(string name, int owner, int targetUid, EnergyType type)
                : base(name, owner, targetUid, int.MaxValue, true)
            {
                this.type = type;
            }

            public override EnergyType? ModifyResistance(GameEngine engine, PokemonInPlay mon, EnergyType? resistance)
            {
                return mon.Uid == TargetUid ? type : resistance;
            }
        }

        /// <summary>Texture Magic: the Defending Pokémon's Weakness is another type until it goes to the Bench or evolves.</summary>
        private sealed class WeaknessChangeEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public WeaknessChangeEffect(string name, int owner, int targetUid, EnergyType type)
                : base(name, owner, targetUid, int.MaxValue, true)
            {
                this.type = type;
            }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon.Uid == TargetUid && weakness.HasValue ? type : weakness;
            }
        }

        /// <summary>Headache: "Your opponent can't play Trainer cards during his or her next turn."</summary>
        private sealed class NoTrainersEffect : ContinuousEffect
        {
            private readonly int affected;

            public NoTrainersEffect(string name, int owner, int affected, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.affected = affected;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && engine.State.CurrentPlayer == affected && card != null && card.Def.IsTrainer;
            }
        }

        /// <summary>Protective Flame: prevent all effects of attacks, including damage, done to the owner's Benched Pokémon.</summary>
        private sealed class BenchShieldEffect : ContinuousEffect
        {
            public BenchShieldEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Owner == Owner && !info.TargetIsActive && info.AgainstOpponent ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return kind == EffectSourceKind.Attack && target.Owner == Owner && !engine.State.IsActive(target);
            }
        }

        // ================================================================== Pokémon

        /// <summary>Growl: the Defending Pokémon does 10 less to it next turn (after W/R). Thundershock: 20, flip to Paralyze.</summary>
        private sealed class GrowlPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Growl")
                {
                    DefendingDoesLessToMe(ctx, 10, false, "Growl");
                    return true;
                }
                if (atk.Name == "Thundershock")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Light Screen: attack damage to it is halved next turn. Quick Attack: 10, or 30 on heads.</summary>
        private sealed class LightScreenElectabuzz : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Light Screen")
                {
                    var self = ctx.Pokemon;
                    if (ctx.InPlay(self))
                        ctx.AddEffect(new HalveDamageEffect(self.Name + " takes half damage from attacks during the opponent's next turn (Light Screen)", ctx.Player, self.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Quick Attack")
                {
                    QuickAttack(ctx, atk);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Energy Absorption: attach up to 2 Energy cards from your discard pile to it. Psyburn: plain.</summary>
        private sealed class EnergyAbsorptionMewtwo : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Absorption") return false;
                var self = ctx.Pokemon;
                var inDiscard = ctx.Me.Discard.Where(card => card.Def.IsEnergy).ToList();
                if (inDiscard.Count == 0) return true;
                var picks = ctx.ChooseCards(inDiscard, 0, 2, "Energy Absorption: attach up to 2 Energy cards from your discard pile to " + self.Name);
                foreach (var e in picks)
                {
                    if (!ctx.InPlay(self) || !ctx.Me.Discard.Remove(e)) continue;
                    ctx.Engine.AttachEnergyCard(e, self);
                }
                return true;
            }
        }

        /// <summary>Recharge: a Lightning Energy card from the deck onto it. Thunderbolt: discard all its Energy to do 50.</summary>
        private sealed class RechargePikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var self = ctx.Pokemon;
                if (atk.Name == "Recharge")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning, 1, "Recharge: choose a Lightning Energy card to attach to " + self.Name);
                    foreach (var e in found)
                    {
                        if (ctx.InPlay(self)) ctx.Engine.AttachEnergyCard(e, self);
                        else ctx.Me.Deck.Add(e);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Thunderbolt")
                {
                    // "Discard all Energy cards attached to Pikachu in order to use this attack": the discard comes first.
                    ctx.Engine.DiscardAllEnergy(self);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Special Delivery (Pokémon Power): draw a card, then put a card from your hand on top of your deck. Supersonic Flight: tails, nothing.</summary>
        private sealed class DragoniteWP : Mon
        {
            protected override string Activated { get { return "Special Delivery"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NotAsleepConfusedOrParalyzed(ctx.Pokemon) && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Draw(1) == 0 || ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Special Delivery: put a card from your hand on top of your deck")[0];
                if (!ctx.Me.Hand.Remove(pick)) return;
                ctx.Me.Deck.Insert(0, pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card from " + ctx.Engine.Their(ctx.Player) + " hand on top of " + ctx.Engine.Their(ctx.Player) + " deck (Special Delivery).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supersonic Flight") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Quick Attack: 10, or 30 on heads. Flames of Rage: discard 2 Fire Energy cards; 40 + 10 per damage counter on it.</summary>
        private sealed class ArcanineWP : Mon
        {
            private static bool FireCard(CardInstance energy) { return energy.Def.EnergyProvides == EnergyType.Fire; }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Flames of Rage") return true;
                return ctx.Pokemon != null && ctx.Pokemon.Energy.Count(FireCard) >= 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Quick Attack")
                {
                    QuickAttack(ctx, atk);
                    return true;
                }
                if (atk.Name == "Flames of Rage")
                {
                    var self = ctx.Pokemon;
                    ctx.DiscardEnergyFrom(self, 2, "Flames of Rage: discard 2 Fire Energy cards attached to " + self.Name, FireCard);
                    ctx.DamageDefending(atk.Damage + 10 * self.DamageCounters);
                    return true;
                }
                return false;
            }
        }

        /// <summary>First Aid: remove 1 damage counter from it. Double-edge: 40, and 20 to itself.</summary>
        private sealed class JigglypuffWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "First Aid")
                {
                    ctx.Heal(ctx.Pokemon, 10);
                    return true;
                }
                if (atk.Name == "Double-edge")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(20);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Psywave: 10 per Energy card on the Defending Pokémon. Devolution Beam: devolve any evolved Pokémon by 1 Stage.</summary>
        private sealed class DevolutionBeamMew : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Psywave")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage * (defending == null ? 0 : defending.Energy.Count));
                    return true;
                }
                if (atk.Name == "Devolution Beam")
                {
                    var evolved = ctx.MyPokemon(IsEvolvedPokemon);
                    evolved.AddRange(ctx.OpponentPokemon(IsEvolvedPokemon));
                    if (evolved.Count == 0) return true;
                    var target = ctx.ChoosePokemon(evolved, "Devolution Beam: devolve which evolved Pokémon (yours or your opponent's)?");
                    // Devolve puts the top Evolution card into its owner's hand and clears Special Conditions, attack effects
                    // and markers, as if the Pokémon had evolved.
                    if (target != null) ctx.Engine.Devolve(target, 1);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Cat Punch: heads, 20 to the Defending Pokémon; tails, 20 to a Benched Pokémon your opponent chooses.</summary>
        private sealed class MeowthWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cat Punch") return false;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(20);
                    return true;
                }
                if (ctx.Opp.Bench.Count == 0) return true;
                var target = ctx.Engine.ChoosePokemon(ctx.Opponent, ctx.Opp.Bench.ToList(), "Cat Punch: choose 1 of your Benched Pokémon to take 20 damage", ctx.Source);
                ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>
        /// Chain Reaction (Pokémon Power): whenever any Pokémon evolves, in any way, you may evolve this Eevee from your deck,
        /// with no timing limits (your ruling). An Eevee evolved this way sets off your other Eevee.
        /// </summary>
        private sealed class EeveeWP : Mon
        {
            public override void OnAnyPokemonEvolved(EffectContext src, PokemonInPlay evolved)
            {
                var eevee = src.Pokemon;
                if (eevee == null || evolved == null || eevee == evolved || !src.InPlay(eevee) || eevee.Name != "Eevee") return;
                if (!NotAsleepConfusedOrParalyzed(eevee) || src.Me.Deck.Count == 0) return;
                if (!src.AskYesNo("Chain Reaction: " + src.Engine.OwnedName(evolved) + " evolved. Look through your deck for a card that evolves from Eevee and evolve your " + eevee.Name + " (" + (src.State.IsActive(eevee) ? "Active" : "Bench") + ")?")) return;
                Announce(src, "Chain Reaction");
                var found = src.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == "Eevee", 1, "Chain Reaction: choose a card that evolves from Eevee");
                src.ShuffleMyDeck();
                if (found.Count == 0) return;
                if (!src.InPlay(eevee) || eevee.Name != "Eevee")
                {
                    src.Me.Deck.AddRange(found);
                    src.ShuffleMyDeck();
                    return;
                }
                src.Engine.EvolvePokemon(eevee, found[0], false);
            }
        }

        /// <summary>Energy Control: heads, move a basic Energy card between your opponent's Pokémon. Telekinesis: 30 to any of their Pokémon, no W/R.</summary>
        private sealed class EnergyControlMewtwo : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Control")
                {
                    if (!ctx.Flip()) return true;
                    var theirs = ctx.OpponentPokemon(ctx.CanAffect);
                    var holders = theirs.Where(m => m.Energy.Any(card => card.Def.IsBasicEnergy)).ToList();
                    if (holders.Count == 0 || theirs.Count < 2) return true;
                    var giver = ctx.ChoosePokemon(holders, "Energy Control: move a basic Energy card from which of your opponent's Pokémon?");
                    var basics = giver.Energy.Where(card => card.Def.IsBasicEnergy).ToList();
                    var energy = ctx.ChooseCards(basics, 1, 1, "Energy Control: choose the basic Energy card to move")[0];
                    var taker = ctx.ChoosePokemon(theirs.Where(m => m != giver).ToList(), "Energy Control: attach " + energy.Def.Name + " to which of your opponent's Pokémon?");
                    ctx.Engine.MoveEnergy(giver, taker, energy);
                    return true;
                }
                if (atk.Name == "Telekinesis")
                {
                    ctx.DamageAnyOpponent(30, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Solar Power (Pokémon Power): both Active Pokémon recover from Asleep, Confused, Paralyzed and Poisoned. Mega Drain: heal half the damage done.</summary>
        private sealed class VenusaurWP : Mon
        {
            private const SpecialCondition Cured = SpecialCondition.Asleep | SpecialCondition.Confused | SpecialCondition.Paralyzed | SpecialCondition.Poisoned;

            private static bool HasCurable(PokemonInPlay mon) { return mon != null && (mon.Conditions & Cured) != 0; }

            protected override string Activated { get { return "Solar Power"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NotAsleepConfusedOrParalyzed(ctx.Pokemon) && (HasCurable(ctx.Me.Active) || HasCurable(ctx.Opp.Active));
            }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (HasCurable(mon)) ctx.Engine.RemoveCondition(mon, mon.Conditions & Cured);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Drain") return false;
                int done = ctx.DamageDefending(atk.Damage);
                // Half the damage done (after Weakness and Resistance), rounded up to the nearest 10.
                int heal = (done / 2 + 9) / 10 * 10;
                if (heal > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, heal);
                return true;
            }
        }

        /// <summary>Texture Magic: may change its Resistance and the Defending Pokémon's Weakness. 3-D Attack: 3 coins, 20 each.</summary>
        private sealed class CoolPorygonWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Texture Magic")
                {
                    var self = ctx.Pokemon;
                    var defending = ctx.Defending;
                    if (ctx.InPlay(self) && ctx.AskYesNo("Texture Magic: change " + self.Name + "'s Resistance?"))
                    {
                        var resistType = ChooseType(ctx, "Texture Magic: " + self.Name + "'s Resistance becomes which type?");
                        ctx.AddEffect(new ResistanceChangeEffect(self.Name + "'s Resistance is " + resistType + " (Texture Magic)", ctx.Player, self.Uid, resistType));
                    }
                    if (defending != null && ctx.InPlay(defending) && ctx.Engine.WeaknessOf(defending).HasValue && ctx.CanAffect(defending)
                        && ctx.AskYesNo("Texture Magic: change " + defending.Name + "'s Weakness?"))
                    {
                        var weakType = ChooseType(ctx, "Texture Magic: " + defending.Name + "'s Weakness becomes which type?");
                        ctx.AddEffect(new WeaknessChangeEffect(defending.Name + "'s Weakness is " + weakType + " (Texture Magic)", ctx.Player, defending.Uid, weakType));
                    }
                    return true;
                }
                if (atk.Name == "3-D Attack")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Tempt: if they have a Bench, flip; heads, switch 1 of their Benched Pokémon with the Defending Pokémon. Poison Claws: flip to Poison.</summary>
        private sealed class DarkPersianWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tempt")
                {
                    if (ctx.Opp.Bench.Count == 0) return true;
                    if (ctx.Flip()) ctx.GustOpponent("Tempt: choose 1 of your opponent's Benched Pokémon to switch with the Defending Pokémon");
                    return true;
                }
                if (atk.Name == "Poison Claws")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Poisoned);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Miraculous Comeback: a coin per Pokémon in play; 10 per heads to the Defending Pokémon, then 10 per tails to itself.</summary>
        private sealed class TeamRocketsMeowthWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Comeback") return false;
                int coins = ctx.MyPokemon().Count + ctx.OpponentPokemon().Count;
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(10 * heads);
                if (coins > heads) ctx.DamageSelf(10 * (coins - heads));
                return true;
            }
        }

        /// <summary>Synchronize: only if it and the Defending Pokémon have the same number of Energy cards attached.</summary>
        private sealed class SabrinasAbraWP : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Synchronize") return true;
                var defending = ctx.Defending;
                return ctx.Pokemon != null && defending != null && ctx.Pokemon.Energy.Count == defending.Energy.Count;
            }
        }

        /// <summary>Headache: your opponent can't play Trainer cards next turn. Fury Swipes: 3 coins, 10 each.</summary>
        private sealed class PsyduckWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Headache")
                {
                    ctx.AddEffect(new NoTrainersEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during " + ctx.Engine.Their(ctx.Opponent) + " next turn (Headache)",
                        ctx.Player, ctx.Opponent, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Fury Swipes")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hyper Flame: heads, discard a Fire Energy card; tails, discard all Energy; nothing to discard, nothing happens.</summary>
        private sealed class MoltresWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Flame") return false;
                var self = ctx.Pokemon;
                int discarded = ctx.Flip()
                    ? ctx.DiscardEnergyFrom(self, 1, "Hyper Flame: discard a Fire Energy card attached to " + self.Name, x => x.Def.EnergyProvides == EnergyType.Fire)
                    : ctx.Engine.DiscardAllEnergy(self);
                if (discarded == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Diamond Dust: 20; heads, the Defending Pokémon is Paralyzed and 10 to each of their Benched Pokémon.</summary>
        private sealed class ArticunoWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Dust") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) return true;
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, 10);
                return true;
            }
        }

        /// <summary>Lightning Burn: 30; heads, 30 to 1 of their Benched Pokémon too; tails, 30 to itself.</summary>
        private sealed class ZapdosWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Burn") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads) ctx.DamageOneBenched(30);
                else ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Birthday Surprise: 30; on your birthday (set on the main menu), flip: heads, 50 more.</summary>
        private sealed class BirthdayPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Birthday Surprise") return false;
                int damage = atk.Damage;
                if (!ctx.Me.BirthdayToday) ctx.Log("It isn't " + ctx.Engine.Their(ctx.Player) + " birthday, so Birthday Surprise does " + damage + " damage.");
                else if (ctx.Flip()) damage += 50;
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Thundershock: 10, flip to Paralyze. Fly: tails, nothing; heads, 30 and protected next turn.</summary>
        private sealed class FlyingPikachuWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thundershock")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Fly")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    DamageAndMaybeProtect(ctx, atk.Damage, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Thunderbolt: 40, then discard all Energy cards attached to it.</summary>
        private sealed class ScratchPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderbolt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Thundershock: 10, flip to Paralyze. Agility: 20; heads, protected next turn.</summary>
        private sealed class AgilityPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thundershock")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Agility")
                {
                    bool heads = ctx.Flip();
                    DamageAndMaybeProtect(ctx, atk.Damage, heads);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Water Gun: 20 + 10 per Energy beyond the attack's cost, up to 20 more.</summary>
        private sealed class MarillWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Gun") return false;
                int extra = ExtraEnergy(ctx, atk, 2) ? 2 : ExtraEnergy(ctx, atk, 1) ? 1 : 0;
                ctx.DamageDefending(atk.Damage + 10 * extra);
                return true;
            }
        }

        /// <summary>Snivel: the Defending Pokémon does 20 less to it next turn (before W/R). Mini-Metronome: heads, use 1 of the Defending Pokémon's attacks.</summary>
        private sealed class TogepiWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snivel")
                {
                    DefendingDoesLessToMe(ctx, 20, true, "Snivel");
                    return true;
                }
                if (atk.Name == "Mini-Metronome")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    CardDef source;
                    int index;
                    if (!ChooseCopiedAttack(ctx, ctx.Defending, "Mini-Metronome: choose 1 of the Defending Pokémon's attacks", out source, out index))
                    {
                        ctx.Log("Mini-Metronome: the Defending Pokémon has no attacks to copy.");
                        return true;
                    }
                    UseCopiedAttack(ctx, source, index);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Eek: draw 2 cards. (Baby Rule and evolving into Clefairy: engine.)</summary>
        private sealed class CleffaWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eek") return false;
                ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Paint: heads, a Coloring counter on the Defending Pokémon makes it the type you choose (until it evolves or leaves play).</summary>
        private sealed class SmeargleWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paint") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                var type = ChooseType(ctx, "Paint: " + defending.Name + " becomes which type?");
                defending.Markers[PokemonInPlay.ColoringMarker] = (int)type;
                ctx.Log("Paint: a Coloring counter was put on " + ctx.Engine.OwnedName(defending) + "; it's " + type + " type now.");
                return true;
            }
        }

        /// <summary>Leer: heads, the Defending Pokémon can't attack it next turn. Metal Pincer: flip until tails, 30 + 10 per heads.</summary>
        private sealed class ScizorWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Leer")
                {
                    var self = ctx.Pokemon;
                    var defending = ctx.Defending;
                    if (!ctx.Flip() || defending == null || !ctx.InPlay(self)) return true;
                    ctx.AddEffect(new LeerEffect(defending.Name + " can't attack " + self.Name + " during its next turn (Leer)", ctx.Player, self.Uid, ctx.Turn + 1, defending.Uid, ctx.Turn));
                    return true;
                }
                if (atk.Name == "Metal Pincer")
                {
                    ctx.DamageDefending(atk.Damage + 10 * ctx.FlipUntilTails());
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Bolt (Pokémon Power): damaged by the opponent's attack and not Knocked Out, flip; heads, it and its cards are shuffled into
        /// the deck (not if it was Asleep, Confused or Paralyzed when damaged). Protective Flame: 50; your Bench is protected next turn.
        /// </summary>
        private sealed class EnteiWP : Mon
        {
            /// <summary>Turn on which it was last damaged by the opponent's attack while Asleep, Confused or Paralyzed.</summary>
            private const string HitWhileDisabled = "Bolt: damaged while Asleep, Confused or Paralyzed";

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var self = src.Pokemon;
                if (self == null || info.Target != self || !info.AgainstOpponent) return damage;
                if (NotAsleepConfusedOrParalyzed(self)) self.AbilityUsedOnTurn.Remove(HitWhileDisabled);
                else self.AbilityUsedOnTurn[HitWhileDisabled] = src.Turn;
                return damage;
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var self = ctx.Pokemon;
                if (self == null || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(self) || damage <= 0) return;
                if (ctx.Engine.IsKnockedOut(self)) return;
                int turn;
                if (self.AbilityUsedOnTurn.TryGetValue(HitWhileDisabled, out turn) && turn == ctx.Turn) return;
                Announce(ctx, "Bolt");
                if (ctx.Flip()) ctx.Engine.ShufflePokemonIntoDeck(self);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Protective Flame") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new BenchShieldEffect(ctx.Engine.Possessive(ctx.Player) + " Benched Pokémon are protected from attacks during the opponent's next turn (Protective Flame)",
                    ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Let's Play!: put a Baby Pokémon from your deck onto your Bench (not with a full Bench).</summary>
        private sealed class PichuWP : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Let's Play!" || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Let's Play!") return false;
                ctx.SearchToBench(d => d.IsBaby, 1, "Let's Play!: choose a Baby Pokémon to put onto your Bench");
                return true;
            }
        }

        /// <summary>Good Night Song: the Defending Pokémon is now Asleep.</summary>
        private sealed class IgglybuffWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Good Night Song") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Repeating Kick: flip until tails, 20 per heads. Rapid Spin: 30, then they switch, then you switch.</summary>
        internal sealed class HitmontopWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Repeating Kick")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                    return true;
                }
                if (atk.Name == "Rapid Spin")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.OpponentSwitchesActive();
                    if (ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Rapid Spin: choose a Benched Pokémon to switch with your Active Pokémon");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Join (Pokémon Power): with Unown J, O, I and N on your Bench, search your deck for a Pokémon card. Neo Discovery's Unown
        /// [O], [I] and [N] count as those letters (v139).
        /// </summary>
        private sealed class UnownJWP : Mon
        {
            protected override string Activated { get { return "Join"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0) return false;
                return CardsN2.UnownOnBench(ctx, "J", "O", "I", "N");
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon, 1, "Join: choose a Basic Pokémon or Evolution card to put into your hand");
            }

            /// <summary>Hidden Power: Unown [A]'s [Anger] (v139) and Unown [X]'s [XXXXX] (v142) add to it too.</summary>
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CardsN2.HiddenPower(ctx, atk); }
        }

        /// <summary>Pain Split: 1 damage counter on the Defending Pokémon per counter on it. Confuse Ray: 10, flip to Confuse.</summary>
        private sealed class MisdreavusWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pain Split")
                {
                    int counters = ctx.Pokemon.DamageCounters;
                    if (counters > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, counters);
                    return true;
                }
                if (atk.Name == "Confuse Ray")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Seething Anger: a coin per damage counter on it, 20 + 10 per heads. Fling: 50, then they switch the Defending Pokémon.</summary>
        private sealed class MachampWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Seething Anger")
                {
                    int coins = ctx.Pokemon.DamageCounters;
                    ctx.DamageDefending(atk.Damage + 10 * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                    return true;
                }
                if (atk.Name == "Fling")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.OpponentSwitchesActive();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Burning Fire: discard any Fire Energy cards from your Pokémon, 10 more damage each.</summary>
        private sealed class MagmarWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Fire") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var m in ctx.MyPokemon())
                    foreach (var card in m.Energy)
                        if (card.Def.EnergyProvides == EnergyType.Fire) holders[card] = m;
                int discarded = 0;
                if (holders.Count > 0)
                {
                    var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Burning Fire: discard any Fire Energy cards attached to your Pokémon (10 more damage each)");
                    foreach (var pick in picks)
                        if (ctx.Engine.DiscardEnergy(holders[pick], pick)) discarded++;
                }
                ctx.DamageDefending(atk.Damage + 10 * discarded);
                return true;
            }
        }

        /// <summary>Slashing Strike: 40; it can't use Slashing Strike during your next turn.</summary>
        private sealed class ScytherWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slashing Strike") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn("Slashing Strike");
                return true;
            }
        }

        /// <summary>
        /// Lightning Rod: a Lightning Rod marker on 1 of the opponent's Pokémon. Lightning Bolt: 10 to the Defending Pokémon and 20
        /// to every Pokémon with a marker, yours included (your ruling), Weakness and Resistance applied even on the Bench; a marked
        /// Defending Pokémon takes 30 (your ruling).
        /// </summary>
        private sealed class LightningRodElectabuzz : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lightning Rod")
                {
                    var targets = ctx.OpponentPokemon(ctx.CanAffect);
                    if (targets.Count == 0) return true;
                    var target = ctx.ChoosePokemon(targets, "Lightning Rod: put a Lightning Rod marker on which of your opponent's Pokémon?");
                    if (target == null) return true;
                    if (target.Markers.ContainsKey(PokemonInPlay.LightningRodMarker))
                    {
                        ctx.Log(ctx.Engine.OwnedName(target) + " already has a Lightning Rod marker (only 1 at a time).");
                        return true;
                    }
                    target.Markers[PokemonInPlay.LightningRodMarker] = 1;
                    ctx.Log("Lightning Rod: a Lightning Rod marker was put on " + ctx.Engine.OwnedName(target) + ".");
                    return true;
                }
                if (atk.Name == "Lightning Bolt")
                {
                    var defending = ctx.Defending;
                    bool defendingMarked = defending != null && defending.Markers.ContainsKey(PokemonInPlay.LightningRodMarker);
                    var marked = ctx.MyPokemon(m => m.Markers.ContainsKey(PokemonInPlay.LightningRodMarker));
                    marked.AddRange(ctx.OpponentPokemon(m => m != defending && m.Markers.ContainsKey(PokemonInPlay.LightningRodMarker)));
                    ctx.DamageDefending(atk.Damage + (defendingMarked ? 20 : 0));
                    foreach (var mon in marked)
                    {
                        if (!ctx.InPlay(mon)) continue;
                        ctx.Engine.DealDamage(new DamageInfo
                        {
                            Attacker = ctx.Pokemon,
                            Target = mon,
                            ApplyWeakness = true,
                            ApplyResistance = true,
                            WeaknessAndResistanceOnBench = true
                        }, 20);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>Neutral Shield (Pokémon Power): prevent all effects of attacks, damage included, done to it by Evolved Pokémon. Psyshock: flip to Confuse.</summary>
        private sealed class NeutralShieldMew : Mon
        {
            private static bool ShieldUp(EffectContext src) { return NotAsleepConfusedOrParalyzed(src.Pokemon); }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && ShieldUp(src) && IsEvolvedPokemon(src.Engine.CurrentSourcePokemon);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && src.Engine.CurrentSourceKind == EffectSourceKind.Attack
                       && ShieldUp(src) && IsEvolvedPokemon(info.Attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyshock") return false;
                DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Aurora Veil (Pokémon Power): while it's your Active Pokémon with no Special Condition, your Benched Pokémon take no damage
        /// from attacks (any attack, yours too: your ruling) and aren't affected by them. Ice Beam: flip to Paralyze.
        /// </summary>
        private sealed class AuroraVeilArticuno : Mon
        {
            private static bool VeilUp(EffectContext src)
            {
                var self = src.Pokemon;
                return self != null && src.State.IsActive(self) && self.Conditions == SpecialCondition.None;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && target.Owner == src.Player && !src.State.IsActive(target) && VeilUp(src);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Attacker != null && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && info.Target.Owner == src.Player
                       && !info.TargetIsActive && VeilUp(src);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Beam") return false;
                DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Guard (Pokémon Power): while it's your Active Pokémon with no Special Condition, the Defending Pokémon can't retreat. Roll Over: it falls Asleep; heads, so does the Defending Pokémon.</summary>
        private sealed class SnorlaxWP : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                var self = src.Pokemon;
                return self != null && mon != null && mon.Owner != src.Player && src.State.IsActive(mon) && src.State.IsActive(self)
                       && self.Conditions == SpecialCondition.None;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roll Over") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Leaf Slice: 2 coins, 20 each.</summary>
        private sealed class CelebiWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Slice") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>Jump Over: choose 1 of their Benched Pokémon, flip; heads, 20 to it. Super Singe: flip to Burn.</summary>
        private sealed class RapidashWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Jump Over")
                {
                    if (ctx.Opp.Bench.Count == 0) return true;
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Jump Over: choose 1 of your opponent's Benched Pokémon");
                    if (ctx.Flip() && target != null) ctx.DamageTarget(target, 20);
                    return true;
                }
                if (atk.Name == "Super Singe")
                {
                    DamageAndFlipCondition(ctx, atk.Damage, SpecialCondition.Burned);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sacred Wing: tails, the base damage is 20 instead of 60.</summary>
        private sealed class HoOhWP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sacred Wing") return false;
                ctx.DamageDefending(ctx.Flip() ? atk.Damage : 20);
                return true;
            }
        }

        /// <summary>Pure Body (Poké-Body): attaching a Water Energy card from your hand to it discards an Energy card from it. Hypno Wave: heads 50; tails 30 and Asleep.</summary>
        private sealed class SuicuneWP : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || !energy.Def.IsBasicEnergy || energy.Def.EnergyProvides != EnergyType.Water) return;
                if (!src.InPlay(target) || target.Energy.Count == 0) return;
                src.Log("Pure Body: an Energy card attached to " + target.Name + " has to be discarded.");
                src.DiscardEnergyFrom(target, 1, "Pure Body: discard an Energy card attached to " + target.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypno Wave") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Computer Error: you may draw up to 5, then your opponent may draw up to 5; your turn ends.</summary>
        private sealed class ComputerError : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int mine = Math.Min(5, ctx.Me.Deck.Count);
                if (mine > 0) ctx.Draw(ctx.AskNumber(0, mine, "Computer Error: draw how many cards (up to 5)?"));
                int theirs = Math.Min(5, ctx.Opp.Deck.Count);
                if (theirs > 0)
                {
                    int n = ctx.Engine.AskNumber(ctx.Opponent, 0, theirs, "Computer Error: draw how many cards (up to 5)?", ctx.Source);
                    ctx.Engine.DrawCards(ctx.Opponent, n);
                }
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Pokémon Center: heal all your damaged Pokémon fully, then discard every Energy card attached to them.</summary>
        private sealed class PokemonCenterWP : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0))
                {
                    ctx.Heal(mon, mon.DamageCounters * 10);
                    ctx.Engine.DiscardAllEnergy(mon);
                }
            }
        }

        /// <summary>Lucky Stadium: once during each player's turn (before attacking), flip; heads, draw a card.</summary>
        private sealed class LuckyStadium : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.Draw(1);
                return false;
            }
        }

        /// <summary>Pokémon Tower: cards stay in the discard pile instead of going to the hand (the engine checks what's resolving).</summary>
        private sealed class PokemonTower : CardScript
        {
            public override bool KeepsDiscardPilesOutOfHands(EffectContext src) { return true; }
        }
    }
}
