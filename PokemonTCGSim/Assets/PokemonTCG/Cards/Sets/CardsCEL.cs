using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Celebrations (CEL, v56). CEL 1-25 have regulation mark E (Expanded). The Classic Collection (CC1-CC25) reprints classic
    /// cards: originals older than Black &amp; White are Unlimited only, except Here Comes Team Rocket!, which plays as the
    /// Supporter of its Evolutions reprint and is Expanded (your ruling); Black &amp; White and later are Expanded.
    /// Mew (CEL 11, 25), Zacian V (CEL 16), Professor's Research (CEL 23, 24), Charizard (CC2) and _____'s Pikachu (CC8) were
    /// already in the pool and keep their scripts.
    ///
    /// New rules: Δ Evolution (M Rayquaza-EX ignores the first-turn and same-turn evolution limits), Pokémon ★ (1 per deck),
    /// Imprison markers (no Poké-Powers or Poké-Bodies until the Pokémon evolves, devolves or leaves play; your ruling) and
    /// Prize cards that stay face up for the rest of the game (Here Comes Team Rocket!).
    /// </summary>
    public static class CardsCEL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("CEL-1", new HoOhCEL());
            r.RegisterByKey("CEL-2", new ReshiramCEL());
            r.RegisterByKey("CEL-3", new KyogreCEL());
            r.RegisterByKey("CEL-4", new PalkiaCEL());
            r.RegisterByKey("CEL-5", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("CEL-6", new FlyingPikachuVCEL());
            r.RegisterByKey("CEL-7", new CardsCRI.BasicShieldAttackMon("Max Balloon"));
            r.RegisterByKey("CEL-8", new PlainMon());
            r.RegisterByKey("CEL-9", new CardsMEW1.SpreadBenchMon("Max Surfer", 30));
            r.RegisterByKey("CEL-10", new ZekromCEL());
            r.RegisterByKey("CEL-12", new CardsASR1.EnergyGardenMon("Breath of Life"));
            r.RegisterByKey("CEL-13", new StarProtectionMon());
            r.RegisterByKey("CEL-14", new StarProtectionMon());
            r.RegisterByKey("CEL-15", new LunalaCEL());
            r.RegisterByKey("CEL-17", new GroudonCEL());
            r.RegisterByKey("CEL-18", new ZamazentaVCEL());
            r.RegisterByKey("CEL-19", new YveltalCEL());
            r.RegisterByKey("CEL-20", new DialgaCEL());
            r.RegisterByKey("CEL-21", new SolgaleoCEL());
            r.RegisterByKey("CEL-22", new LugiaCEL());

            r.RegisterByKey("CEL-CC1", new BlastoiseCC());
            r.RegisterByKey("CEL-CC3", new VenusaurCC());
            r.RegisterByKey("CEL-CC4", new ImposterProfessorOak());
            r.RegisterByKey("CEL-CC5", new DarkGyaradosCC());
            r.RegisterByKey("CEL-CC6", new HereComesTeamRocket());
            r.RegisterByKey("CEL-CC7", new RocketsZapdosCC());
            r.RegisterByKey("CEL-CC9", new CardsDAA1.DeepSeaSwirlMon("Eeeeeeek", 7));
            r.RegisterByKey("CEL-CC10", new ShiningMagikarpCC());
            r.RegisterByKey("CEL-CC11", new TeamMagmasGroudonCC());
            r.RegisterByKey("CEL-CC12", new RocketsAdmin());
            r.RegisterByKey("CEL-CC13", new MewExCC());
            r.RegisterByKey("CEL-CC14", new GardevoirExCC());
            r.RegisterByKey("CEL-CC15", new UmbreonStarCC());
            r.RegisterByKey("CEL-CC16", new ClaydolCC());
            r.RegisterByKey("CEL-CC17", new LuxrayGLLvXCC());
            r.RegisterByKey("CEL-CC18", new GarchompCLvXCC());
            r.RegisterByKey("CEL-CC19", new DonphanCC());
            r.RegisterByKey("CEL-CC20", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new CardsASC.SelfDiscardTypeMon("Blue Flare", EnergyType.Fire, 2)));
            r.RegisterByKey("CEL-CC21", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new RecoilMon("Bolt Strike", 40)));
            r.RegisterByKey("CEL-CC22", new MewtwoExCC());
            r.RegisterByKey("CEL-CC23", new XerneasExCC());
            r.RegisterByKey("CEL-CC24", new MRayquazaExCC());
            r.RegisterByKey("CEL-CC25", new TapuLeleGxCC());
        }

        /// <summary>Base Set era Pokémon Powers: "This power can't be used if this Pokémon is Asleep, Confused, or Paralyzed."</summary>
        private static bool NotAsleepConfusedParalyzed(PokemonInPlay mon)
        {
            return mon != null && !mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Confused) && !mon.Has(SpecialCondition.Paralyzed);
        }

        /// <summary>Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        private static bool NoCondition(PokemonInPlay mon)
        {
            return mon != null && mon.Conditions == SpecialCondition.None;
        }

        private static bool BasicOf(CardDef d, EnergyType type)
        {
            return d.IsBasicEnergy && d.EnergyProvides == type;
        }

        private static bool OnMyBench(EffectContext ctx, string name)
        {
            return ctx.Me.Bench.Any(b => b.Name == name);
        }

        /// <summary>The ability at this index is the named one (on the card it's printed on).</summary>
        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        // ================================================================== Celebrations 1-25

        /// <summary>Sacred Fire: 50 damage to 1 of the opponent's Pokémon. Fire Blast: discard an Energy from this Pokémon.</summary>
        private sealed class HoOhCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sacred Fire")
                {
                    ctx.DamageAnyOpponent(50);
                    return true;
                }
                if (atk.Name == "Fire Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Fire Blast: discard an Energy from " + ctx.Pokemon.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Scorching Wind: 20 to each of the opponent's Benched Pokémon. Black Flame: +80 with Zekrom on your Bench.</summary>
        private sealed class ReshiramCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Scorching Wind")
                {
                    foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 20);
                    return true;
                }
                if (atk.Name == "Black Flame")
                {
                    ctx.DamageDefending(atk.Damage + (OnMyBench(ctx, "Zekrom") ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Aqua Storm: discard the top 5 cards of your deck; 50 damage per Energy card discarded to each of 2 of the opponent's Benched Pokémon.</summary>
        private sealed class KyogreCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Storm") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int energy = discarded.Count(c => c.Def.IsEnergy);
                var bench = ctx.OpponentBench();
                if (energy == 0 || bench.Count == 0)
                {
                    ctx.Log("Aqua Storm did no damage (" + energy + " Energy cards discarded).");
                    return true;
                }
                var targets = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Aqua Storm: choose 2 of your opponent's Benched Pokémon (" + (50 * energy) + " damage each)");
                foreach (var target in targets) ctx.DamageTarget(target, 50 * energy);
                return true;
            }
        }

        /// <summary>Marks "this Pokémon's Overdrive Smash does 80 more damage during your next turn" (ends if it moves to the Bench).</summary>
        private sealed class OverdriveSmashEffect : ContinuousEffect
        {
            public OverdriveSmashEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
            }
        }

        /// <summary>Absolute Space: while Active, the opponent can't play Stadium cards from their hand. Overdrive Smash: +80 during your next turn.</summary>
        private sealed class PalkiaCEL : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && player != src.Player && src.PokemonIsActive && card.Def.IsStadium;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overdrive Smash") return false;
                var me = ctx.Pokemon;
                int uid = me.Uid;
                int turn = ctx.Turn;
                bool boosted = ctx.State.Effects.Any(fx => fx is OverdriveSmashEffect && fx.TargetUid == uid && fx.ActiveFromTurn <= turn);
                ctx.DamageDefending(atk.Damage + (boosted ? 80 : 0));
                if (!ctx.InPlay(me)) return true;
                var next = new OverdriveSmashEffect(me.Name + "'s Overdrive Smash does 80 more damage during its next turn", ctx.Player, me.Uid, ctx.Turn + 2);
                next.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(next);
                return true;
            }
        }

        /// <summary>Thunder Shock: heads, Paralyzed. Fly: tails, nothing; heads, 120 and prevent damage and effects of attacks next turn.</summary>
        private sealed class FlyingPikachuVCEL : Mon
        {
            private static readonly Mon thunderShock = new ParalyzeFlipMon("Thunder Shock");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (thunderShock.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Fly") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Field Crush: discard the opponent's Stadium. White Thunder: +80 with Reshiram on your Bench.</summary>
        private sealed class ZekromCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Field Crush")
                {
                    ctx.DamageDefending(atk.Damage);
                    // "If your opponent has a Stadium in play": a Stadium the opponent played.
                    if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name == "White Thunder")
                {
                    ctx.DamageDefending(atk.Damage + (OnMyBench(ctx, "Reshiram") ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Star Protection (Cosmog, Cosmoem): heads, prevent all damage done to this Pokémon by attacks during the opponent's next turn.</summary>
        private sealed class StarProtectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Protection") return false;
                if (!ctx.Flip() || !ctx.InPlay(ctx.Pokemon)) return true;
                ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (Star Protection)",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Lunar Pain: double the damage counters on each of the opponent's Pokémon. Psychic Shot: 30 to 1 of their Benched Pokémon.</summary>
        private sealed class LunalaCEL : Mon
        {
            private static readonly Mon psychicShot = new CardsMEG.SnipeBenchMon("Psychic Shot", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (psychicShot.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Lunar Pain") return false;
                foreach (var target in ctx.OpponentPokemon())
                    if (target.DamageCounters > 0) ctx.PlaceCounters(target, target.DamageCounters);
                return true;
            }
        }

        /// <summary>Magma Volcano: discard the top 5 cards of your deck; 80 damage for each Energy card discarded.</summary>
        private sealed class GroudonCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Volcano") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int energy = discarded.Count(c => c.Def.IsEnergy);
                if (energy == 0)
                {
                    ctx.Log("Magma Volcano discarded no Energy cards, so it did no damage.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage * energy);
                return true;
            }
        }

        /// <summary>Growl of the Shield: your Fighting Pokémon take 20 less damage from the opponent's Pokémon VMAX's attacks (only 1 applies).</summary>
        private sealed class ZamazentaVCEL : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (damage <= 0 || target == null || target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (!info.Attacker.Def.IsVmax || !src.Engine.TypesOf(target).Contains(EnergyType.Fighting)) return damage;
                return CardsCRZ1.IsLeadSource(src, "Growl of the Shield") ? damage - 20 : damage;
            }
        }

        /// <summary>Cry of Destruction: discard up to 3 Special Energy from the opponent's Pokémon.</summary>
        private sealed class YveltalCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cry of Destruction") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.OpponentPokemon())
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var attached in mon.Energy)
                        if (attached.Def.IsAttachedSpecialEnergy) holders[attached] = mon;
                }
                if (holders.Count == 0) return true;
                var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, Math.Min(3, holders.Count), "Cry of Destruction: discard up to 3 Special Energy from your opponent's Pokémon");
                foreach (var chosen in picks) ctx.Engine.DiscardEnergy(holders[chosen], chosen);
                return true;
            }
        }

        /// <summary>Temporal Backflow: a card from your discard pile into your hand. Metal Blast: +20 per Metal Energy on this Pokémon.</summary>
        private sealed class DialgaCEL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Temporal Backflow")
                {
                    var pile = ctx.Me.Discard.ToList();
                    if (pile.Count == 0) return true;
                    var pick = ctx.ChooseCards(pile, 1, 1, "Temporal Backflow: put a card from your discard pile into your hand");
                    ctx.DiscardToHand(pick);
                    return true;
                }
                if (atk.Name == "Metal Blast")
                {
                    ctx.DamageDefending(atk.Damage + 20 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Rush In: once during your turn, if it's on your Bench, switch it with your Active Pokémon. Solar Geyser: up to 2 basic Energy from the discard pile to 1 Benched Pokémon.</summary>
        private sealed class SolgaleoCEL : Mon
        {
            protected override string Activated { get { return "Rush In"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Geyser") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Solar Geyser: attach up to 2 basic Energy cards to which Benched Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Solar Geyser: choose up to 2 basic Energy cards from your discard pile for " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Aero Ball: 20 per Energy on both Active Pokémon. Deep Crush: it can't attack during your next turn.</summary>
        private sealed class LugiaCEL : Mon
        {
            private static readonly Mon aeroBall = new CardsCRZ1.BothActiveEnergyMon("Aero Ball", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (aeroBall.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Deep Crush") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Classic Collection: before Black & White (Unlimited)

        /// <summary>Rain Dance (Pokémon Power): as often as you like, attach a Water Energy card from your hand to 1 of your Water Pokémon. Hydro Pump: +10 per extra Water Energy (up to 2).</summary>
        private sealed class BlastoiseCC : Mon
        {
            private const string RainDance = "Rain Dance";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, RainDance) && ctx.IsMyTurn && NotAsleepConfusedParalyzed(ctx.Pokemon) && RainDanceReady(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                UseRainDance(ctx, RainDance);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Pump") return false;
                // Water Energy not used to pay for [W][W][W]; Water Energy after the 2nd extra one doesn't count.
                int extra = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water) - 3;
                ctx.DamageDefending(atk.Damage + 10 * Math.Max(0, Math.Min(2, extra)));
                return true;
            }
        }

        /// <summary>
        /// Energy Trans (Pokémon Power): as often as you like, move a Grass Energy card from 1 of your Pokémon to another. A Special
        /// Energy that provides Grass while attached (Rainbow Energy) can be moved too (Compendium: Energy Trans + Rainbow / Multi
        /// Energy, 2000 and 2004-2005).
        /// </summary>
        private sealed class VenusaurCC : Mon
        {
            private const string Trans = "Energy Trans";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, Trans) && ctx.IsMyTurn && NotAsleepConfusedParalyzed(ctx.Pokemon) && EnergyTransReady(ctx, EnergyType.Grass);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                UseEnergyTrans(ctx, Trans, EnergyType.Grass);
            }
        }

        // ------------------------------------------------------------------ Shared with HGSS Poké-Powers (CardsHSP, v135)

        /// <summary>
        /// Rain Dance (Blastoise's Pokémon Power, Feraligatr Prime's Poké-Power): as often as you like, attach a Water Energy card from
        /// your hand to 1 of your Water Pokémon. Ready when there's a Water Energy card in the hand and a Water Pokémon to take it.
        /// The caller checks whose turn it is and whether the power can be used (Special Conditions).
        /// </summary>
        internal static bool RainDanceReady(EffectContext ctx)
        {
            return ctx.Me.Hand.Any(c => BasicOf(c.Def, EnergyType.Water)) && WaterPokemon(ctx).Any(m => ctx.Engine.CanAttachFromHand(m));
        }

        internal static void UseRainDance(EffectContext ctx, string ability)
        {
            RepeatableAbility.AttachFromHand(ctx, d => BasicOf(d, EnergyType.Water), () => WaterPokemon(ctx), ability, "a Water Energy card");
        }

        private static List<PokemonInPlay> WaterPokemon(EffectContext ctx)
        {
            return ctx.MyPokemon(m => ctx.Engine.TypesOf(m).Contains(EnergyType.Water));
        }

        /// <summary>
        /// Energy Trans (Venusaur's Pokémon Power) and Leaf Trans (Meganium Prime's Poké-Power): as often as you like, move an Energy
        /// of that type from 1 of your Pokémon to another (one that provides the type while attached counts too). Ready when there's
        /// one to move and another Pokémon to move it to. The caller checks whose turn it is and whether the power can be used.
        /// </summary>
        internal static bool EnergyTransReady(EffectContext ctx, EnergyType type)
        {
            return ctx.MyPokemon().Count > 1 && TransHolders(ctx, type).Count > 0;
        }

        internal static void UseEnergyTrans(EffectContext ctx, string ability, EnergyType type)
        {
            RepeatableAbility.Repeat(ctx, () => EnergyTransReady(ctx, type), offerDone => MoveOneTypedEnergy(ctx, ability, type, offerDone));
        }

        private static List<CardInstance> TypedEnergyOn(EffectContext ctx, PokemonInPlay holder, EnergyType type)
        {
            return holder.Energy.Where(en => CardsDRI1.ProvidesType(ctx, holder, en, type)).ToList();
        }

        private static List<PokemonInPlay> TransHolders(EffectContext ctx, EnergyType type)
        {
            return ctx.MyPokemon(m => TypedEnergyOn(ctx, m, type).Count > 0);
        }

        private static bool MoveOneTypedEnergy(EffectContext ctx, string ability, EnergyType type, bool offerDone)
        {
            var from = RepeatableAbility.ChoosePokemonOrDone(ctx, TransHolders(ctx, type), ability + ": move a " + type + " Energy card from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
            if (from == null) return false;
            var energy = TypedEnergyOn(ctx, from, type);
            if (energy.Count == 0) return false;
            var card = energy.Select(en => en.Def.Name).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, ability + ": move which Energy card?")[0];
            var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), ability + ": move it from " + from.Name + " to which Pokémon?", offerDone);
            if (to == null) return false;
            return ctx.Engine.MoveEnergy(from, to, card);
        }

        /// <summary>Imposter Professor Oak (old Trainer, an Item): your opponent shuffles their hand into their deck and draws 7 cards.</summary>
        private sealed class ImposterProfessorOak : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 7);
            }
        }

        /// <summary>
        /// Final Beam (Pokémon Power): Knocked Out by an opponent's attack, flip a coin; heads, 20 damage per Water Energy on it to the
        /// Pokémon that Knocked it Out, Weakness and Resistance applied (not while Asleep, Confused or Paralyzed). Ice Beam: heads, Paralyzed.
        /// </summary>
        private sealed class DarkGyaradosCC : Mon
        {
            private static readonly Mon iceBeam = new ParalyzeFlipMon("Ice Beam");

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null || !NotAsleepConfusedParalyzed(me)) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                int water = src.Engine.EnergyCountOfType(me, EnergyType.Water);
                if (attacker == null || attacker.Owner == src.Player || water <= 0) return;
                Announce(src, "Final Beam");
                if (!src.Flip()) return;
                int damage = 20 * water;
                // The power's damage is Dark Gyarados' type (Water), with the Knocking Out Pokémon's Weakness and Resistance.
                var types = src.Engine.TypesOf(me);
                var weakness = src.Engine.WeaknessOf(attacker);
                if (weakness.HasValue && types.Contains(weakness.Value))
                    damage = attacker.Def.WeaknessAmount > 0 ? damage + attacker.Def.WeaknessAmount : damage * 2;
                var resistance = src.Engine.ResistanceOf(attacker);
                if (resistance.HasValue && types.Contains(resistance.Value)) damage = Math.Max(0, damage - attacker.Def.ResistanceAmount);
                src.Log("Final Beam does " + damage + " damage to " + attacker.Name + ".");
                if (damage >= 10) src.Engine.PlaceDamageCounters(attacker, damage / 10, "Final Beam");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return iceBeam.HandleAttack(ctx, atk); }
        }

        /// <summary>Here Comes Team Rocket! (a Supporter, your ruling): each player's Prize cards are turned face up for the rest of the game.</summary>
        private sealed class HereComesTeamRocket : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    foreach (var prize in p.Prizes)
                        if (!p.FaceUpPrizeIds.Contains(prize.Id)) p.FaceUpPrizeIds.Add(prize.Id);
                    if (p.Prizes.Count > 0) ctx.Engine.Reveal(player, p.Prizes.ToList(), "turned " + ctx.Engine.Their(player) + " Prize cards face up for the rest of the game");
                }
            }
        }

        /// <summary>Plasma: attach a Lightning Energy card from your discard pile to it. Electroburn: 10 damage to itself per Lightning Energy on it.</summary>
        private sealed class RocketsZapdosCC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Plasma")
                {
                    ctx.DamageDefending(atk.Damage);
                    var lightning = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
                    if (lightning.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                    var card = lightning[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                    return true;
                }
                if (atk.Name == "Electroburn")
                {
                    int lightningEnergy = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Lightning);
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * lightningEnergy);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Gold Scale: your opponent may draw 2 cards, then you may draw 2. Dragon Bond: search for a Gyarados, Dark Gyarados or Shining Gyarados.</summary>
        private sealed class ShiningMagikarpCC : Mon
        {
            private static bool GyaradosCard(CardDef d)
            {
                return d.Name == "Gyarados" || d.Name == "Dark Gyarados" || d.Name == "Shining Gyarados";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gold Scale")
                {
                    if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Gold Scale: draw 2 cards?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 2);
                    if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Gold Scale: draw 2 cards?")) ctx.Draw(2);
                    return true;
                }
                if (atk.Name == "Dragon Bond")
                {
                    ctx.SearchToHand(GyaradosCard, 1, "Dragon Bond: search your deck for a Gyarados, Dark Gyarados or Shining Gyarados");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Power Saver (Poké-Body): while there are 3 or fewer Pokémon in play (both players') with Team Magma in their name, it can't
        /// attack. Linear Attack: 20 damage to 1 of the opponent's Pokémon. Pulverize: +20 if the Defending Pokémon has 2+ damage counters.
        /// </summary>
        private sealed class TeamMagmasGroudonCC : Mon
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon) return false;
                int magma = 0;
                for (int side = 0; side < 2; side++)
                    foreach (var other in src.State.Players[side].InPlay())
                        if (other.Name.Contains("Team Magma")) magma++;
                return magma <= 3;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Linear Attack")
                {
                    ctx.DamageAnyOpponent(20);
                    return true;
                }
                if (atk.Name == "Pulverize")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage + (defending != null && defending.DamageCounters >= 2 ? 20 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Rocket's Admin.: each player shuffles their hand into their deck, then draws up to as many cards as their remaining Prize cards (you first).</summary>
        private sealed class RocketsAdmin : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent }) ctx.Engine.ShuffleHandIntoDeck(player);
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    int max = Math.Min(p.Prizes.Count, p.Deck.Count);
                    if (max <= 0 || ctx.Engine.SupporterEffectsPreventedOn(player)) continue;
                    int n = ctx.Engine.AskNumber(player, 0, max, "Rocket's Admin.: draw how many cards (up to " + p.Prizes.Count + ")?", ctx.Source);
                    ctx.Engine.DrawCards(player, n);
                }
            }
        }

        /// <summary>
        /// Versatile (Poké-Body): it can use the attacks of every other Pokémon in play, both players' (it still needs the Energy).
        /// Power Move: search for an Energy card and attach it to this Pokémon; then it may switch with a Benched Pokémon.
        /// </summary>
        private sealed class MewExCC : Mon
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                var seen = new HashSet<string>();
                for (int side = 0; side < 2; side++)
                {
                    foreach (var other in src.State.Players[side].InPlay())
                    {
                        if (other == src.Pokemon) continue;
                        var defs = new List<CardDef> { other.Def };
                        // A LV.X or BREAK also has the attacks of the Pokémon under it.
                        if ((other.Def.IsLevelUp || other.Def.IsBreak) && other.Stack.Count >= 2) defs.Add(other.Stack[other.Stack.Count - 2].Def);
                        foreach (var def in defs)
                        {
                            if (!seen.Add(def.BaseKey)) continue;
                            for (int i = 0; i < def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(def, i));
                        }
                    }
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Move") return false;
                var self = ctx.Pokemon;
                if (ctx.Me.Deck.Count > 0)
                {
                    var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Power Move: search your deck for an Energy card to attach to " + self.Name);
                    foreach (var card in found)
                    {
                        if (ctx.InPlay(self)) ctx.Engine.AttachEnergyCard(card, self);
                        else ctx.Me.Deck.Add(card);
                    }
                    ctx.ShuffleMyDeck();
                }
                if (ctx.InPlay(self) && ctx.PokemonIsActive && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Power Move: switch " + self.Name + " with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>
        /// Imprison (Poké-Power): once during your turn, while Active, put an Imprison marker on 1 of the opponent's Pokémon: it can't
        /// use Poké-Powers or Poké-Bodies until it evolves, devolves or leaves play (your ruling). Flame Ball: may move a Fire Energy
        /// card from it to a Benched Pokémon.
        /// </summary>
        private sealed class GardevoirExCC : Mon
        {
            protected override string Activated { get { return "Imprison"; } }

            private static List<PokemonInPlay> Unmarked(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => !GameEngine.Imprisoned(m));
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.PokemonIsActive && Unmarked(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Unmarked(ctx), "Imprison: put an Imprison marker on which of your opponent's Pokémon?");
                if (target == null) return;
                target.Markers[PokemonInPlay.ImprisonMarker] = 1;
                ctx.Log(target.Name + " got an Imprison marker: it can't use Poké-Powers or Poké-Bodies until it evolves, devolves or leaves play.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Ball") return false;
                ctx.DamageDefending(atk.Damage);
                var self = ctx.Pokemon;
                if (!ctx.InPlay(self)) return true;
                var bench = ctx.MyBench().Where(b => b != self).ToList();
                var fire = self.Energy.Where(en => BasicOf(en.Def, EnergyType.Fire)).ToList();
                if (bench.Count == 0 || fire.Count == 0) return true;
                if (!ctx.AskYesNo("Flame Ball: move a Fire Energy card from " + self.Name + " to 1 of your Benched Pokémon?")) return true;
                var to = ctx.ChoosePokemon(bench, "Flame Ball: move the Fire Energy card to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(self, to, fire[0]);
                return true;
            }
        }

        /// <summary>Dark Ray (Poké-Power): benched from the hand, discard a card from the opponent's hand without looking. Feint Attack: 30 to any, nothing changes it.</summary>
        private sealed class UmbreonStarCC : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Dark Ray", ctx.Opp.Hand.Count > 0, "Dark Ray: discard a card from your opponent's hand without looking?"))
                    CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Feint Attack") return false;
                ctx.DamageAnyOpponent(30, true, true);
                return true;
            }
        }

        /// <summary>Cosmic Power (Poké-Power): put up to 2 cards from your hand on the bottom of your deck; if you do, draw until you have 6.</summary>
        private sealed class ClaydolCC : Mon
        {
            protected override string Activated { get { return "Cosmic Power"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var picks = ctx.ChooseCards(hand, 1, Math.Min(2, hand.Count), "Cosmic Power: put 1 or 2 cards from your hand on the bottom of your deck");
                if (picks.Count == 0) return;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.Log("Cosmic Power: " + picks.Count + (picks.Count == 1 ? " card went" : " cards went") + " to the bottom of the deck.");
                ctx.DrawUntil(6);
            }
        }

        /// <summary>Bright Look (Poké-Power): put onto your Active Luxray GL from the hand, switch in 1 of the opponent's Benched Pokémon. Flash Impact: 30 to 1 of your Pokémon.</summary>
        private sealed class LuxrayGLLvXCC : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.PokemonIsActive) return;
                if (TriggerOnce(ctx, "Bright Look", ctx.GustTargets().Count > 0, "Bright Look: switch the Defending Pokémon with 1 of your opponent's Benched Pokémon?"))
                    ctx.GustOpponent("Bright Look: choose the Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Flash Impact: do 30 damage to which of your Pokémon?");
                ctx.DamageWith(target, 30, false, false, false);
                return true;
            }
        }

        /// <summary>Healing Breath (Poké-Power): put onto your Active Garchomp C from the hand, heal all damage from your Pokémon SP. Dragon Rush: discard 2 Energy, 80 to any.</summary>
        private sealed class GarchompCLvXCC : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.PokemonIsActive) return;
                var hurt = ctx.MyPokemon(x => x.Def.IsPokemonSP && x.DamageCounters > 0);
                if (!TriggerOnce(ctx, "Healing Breath", hurt.Count > 0, "Healing Breath: remove all damage counters from each of your Pokémon SP?")) return;
                foreach (var sp in hurt) ctx.Heal(sp, sp.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Rush") return false;
                var self = ctx.Pokemon;
                ctx.DiscardEnergyFrom(self, 2, "Dragon Rush: discard 2 Energy attached to " + self.Name);
                ctx.DamageAnyOpponent(80);
                ctx.SelfCantUseAttackNextTurn("Dragon Rush");
                return true;
            }
        }

        /// <summary>Exoskeleton (Poké-Body): damage done to it by attacks is reduced by 20 after Weakness and Resistance. Earthquake: 10 to each of your Benched Pokémon.</summary>
        private sealed class DonphanCC : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earthquake") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.MyBench()) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        // ================================================================== Classic Collection: Black & White and later (Expanded)

        /// <summary>X Ball: 20 per Energy on both Active Pokémon. Psydrive: discard an Energy from this Pokémon.</summary>
        private sealed class MewtwoExCC : Mon
        {
            private static readonly Mon xBall = new CardsCRZ1.BothActiveEnergyMon("X Ball", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (xBall.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Psydrive") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Psydrive: discard an Energy from " + ctx.Pokemon.Name);
                return true;
            }
        }

        /// <summary>Break Through: also 30 to 1 Benched Pokémon. X Blast: it can't use X Blast during your next turn.</summary>
        private sealed class XerneasExCC : Mon
        {
            private static readonly Mon breakThrough = new CardsMEG.SnipeBenchMon("Break Through", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (breakThrough.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "X Blast") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn("X Blast");
                return true;
            }
        }

        /// <summary>Δ Evolution (Ancient Trait): no first-turn or same-turn limit when played from the hand. Emerald Break: 30 per Benched Pokémon you have.</summary>
        private sealed class MRayquazaExCC : Mon
        {
            public override bool EvolvesWithoutTimingLimits { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Emerald Break") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Bench.Count);
                return true;
            }
        }

        /// <summary>Wonder Tag: benched from the hand during your turn, search for a Supporter. Energy Drive: 20 per Energy on both Active, no Weakness or Resistance. Tapu Cure-GX.</summary>
        private sealed class TapuLeleGxCC : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Wonder Tag", ctx.Me.Deck.Count > 0, "Wonder Tag: search your deck for a Supporter card?"))
                    ctx.SearchToHand(d => d.IsSupporter, 1, "Wonder Tag: choose a Supporter card to put into your hand");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Drive")
                {
                    int n = (ctx.Me.Active == null ? 0 : ctx.EnergyCount(ctx.Me.Active)) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                    ctx.DamageWith(ctx.Defending, atk.Damage * n, false, false, false);
                    return true;
                }
                if (atk.Name == "Tapu Cure-GX")
                {
                    var hurt = ctx.MyBench().Where(b => b.DamageCounters > 0).ToList();
                    if (hurt.Count == 0) return true;
                    var picks = hurt.Count <= 2 ? hurt : ctx.ChoosePokemonMany(hurt, 2, 2, "Tapu Cure-GX: heal all damage from which 2 of your Benched Pokémon?");
                    foreach (var healed in picks) ctx.Heal(healed, healed.DamageCounters * 10);
                    return true;
                }
                return false;
            }
        }
    }
}
