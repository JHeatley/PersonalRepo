using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Delta Species Uncommon and Common Pokémon (DS 19-88, 114; v169). Unlimited only. The Holon's Pokémon (21, 22, 70, 71) can be
    /// attached from the hand as Energy (CardsDS.HolonMon). The Castform cards change into each other (Temperamental Weather) and the
    /// Ditto into each other (Duplicate). 1-on-1: "each Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsDSB
    {
        private static Mon Ditto(Mon attacks) { return new CardsDX.FormChangeMon("Ditto", attacks, "Duplicate"); }

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DS-19", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("DS-20", new CardsSS.BabyEvolutionMon("Marill", new TypeMatchMon()));
            r.RegisterByKey("DS-21", new CardsDS.HolonMon(true, new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused)));
            r.RegisterByKey("DS-22", new CardsDS.HolonMon(true, CardsSS.ConditionBonus("Extra Ball", 20, d => d.Def.IsOldEx)));
            r.RegisterByKey("DS-23", new HypnoBindingAura());
            r.RegisterByKey("DS-24", new ComboMon(new DeltaFangMon(),
                new CardsASR1.TimesMon("Gang Up", c => c.MyPokemon(m => c.Engine.IsType(m, EnergyType.Darkness) || c.Engine.IsType(m, EnergyType.Metal)).Count)));
            r.RegisterByKey("DS-25", new Porygon2Backup());
            r.RegisterByKey("DS-26", new CardsHL.CastformMon("Rain Castform",
                new CardsASR1.PlusMon("Holon Splash", 10, c => CardsDS.HolonEnergyCount(c.Pokemon))));
            r.RegisterByKey("DS-27", new SandslashDeltaStorm());
            r.RegisterByKey("DS-28", new SlowkingPrizeShift());
            r.RegisterByKey("DS-29", new CardsHL.CastformMon("Snow-cloud Castform", new HolonBlizzardMon()));
            r.RegisterByKey("DS-30", new CardsMA.SwiftMon("Swift"));
            r.RegisterByKey("DS-31", new CardsHL.CastformMon("Sunny Castform", new CardsDS.HolonSearchMon()));
            r.RegisterByKey("DS-32", new SteepDiveMon());
            r.RegisterByKey("DS-33", new WeezingBodyOdor());
            r.RegisterByKey("DS-34", new CardsHL.CastformMon("Castform", new HolonDrawMon()));
            r.RegisterByKey("DS-35", Ditto(new CardsTRR.EnergyLinkMon("Energy Link")));
            r.RegisterByKey("DS-36", Ditto(new CardsBS1.ToxicMon("Toxic", 2)));
            r.RegisterByKey("DS-37", Ditto(new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("DS-38", Ditto(new CardsE2.CopyMon()));
            r.RegisterByKey("DS-39", Ditto(new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("DS-40", Ditto(new SwitchSelfMon("Smash Turn", true)));
            r.RegisterByKey("DS-41", new CoinsMon("Slam", 2));
            r.RegisterByKey("DS-42", new CardsNP.TwisterMon());
            r.RegisterByKey("DS-43", new ComboMon(new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned), new SonicSignalMon()));
            r.RegisterByKey("DS-44", new HariyamaReversalAura());
            r.RegisterByKey("DS-45", new PartnerProtectionMon("Beacon Protection", "Volbeat", a => CardsTRR.Dark(a.Name),
                new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new CardsEM.EvolvedConditionMon("Strange Rasping", SpecialCondition.Confused))));
            r.RegisterByKey("DS-46", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 1));
            r.RegisterByKey("DS-47", new ComboMon(new CardsTWM1.HealSelfMon("Calm Mind", 30),
                new CardsASR1.PlusMon("Psychic Boom", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("DS-48", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("DS-49", new FlipBonusMon("Sharp Claws", 10));
            r.RegisterByKey("DS-50", new PersianProwl());
            r.RegisterByKey("DS-51", new ComboMon(new ParalyzeFlipMon("Paralyzing Gaze"), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("DS-52", new ComboMon(new ConditionAttackMon("Super Singe", SpecialCondition.Burned), new FlipBonusMon("Stomp", 10)));
            r.RegisterByKey("DS-53", new ParalyzeFlipMon("Tight Jaw"));
            r.RegisterByKey("DS-54", new CardsFST1.TakeLessMon(10, null));
            r.RegisterByKey("DS-55", new CardsLM.ShiningHornMon(new ComboMon(
                new SearchAttackMon("Cry for Help", d => d.IsPokemon && d.HasType(EnergyType.Metal) && !d.IsOldEx, 1),
                new ShieldAttackMon("Steel Wing", 20))));
            r.RegisterByKey("DS-56", new PartnerProtectionMon("Extra Protection", "Illumise", a => a.Def.IsOldEx, new CardsMA.SwiftMon("Swift")));
            r.RegisterByKey("DS-57", new PlainMon());
            r.RegisterByKey("DS-58", new CardsSUM1.DiscardTypeFromSelfMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("DS-59", new BeldumConductiveBody());
            r.RegisterByKey("DS-60", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("DS-61", Ditto(new CardsGEN1.FlipTailsDiscardTypeMon("Fireworks", EnergyType.Fire, 1)));
            r.RegisterByKey("DS-62", Ditto(new CardsSSB.AnyOpponentTextMon("Linear Attack", 20)));
            r.RegisterByKey("DS-63", Ditto(new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("DS-64", Ditto(new CardsTWM1.HealSelfMon("Spiral Drain", 30)));
            r.RegisterByKey("DS-65", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("DS-66", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("DS-67", new CardsTEU1.AmnesiaMon("Disable"));
            r.RegisterByKey("DS-68", new CardsOBF1.AttachFromDiscardToSelfMon("Metal Scoop", EnergyType.Metal));
            r.RegisterByKey("DS-69", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("DS-70", new CardsDS.HolonMon(false, new CardsSSB.AnyOpponentTextMon("Linear Attack", 10)));
            r.RegisterByKey("DS-71", new CardsDS.HolonMon(false, new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("DS-72", new ConditionAttackMon("Confusion Gas", SpecialCondition.Confused));
            r.RegisterByKey("DS-73", new FlipBonusMon("Rising Lunge", 10));
            r.RegisterByKey("DS-74", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("DS-75", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("DS-76", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("DS-77", new PickupPowerMon());
            r.RegisterByKey("DS-78", new PlainMon());
            r.RegisterByKey("DS-79", new ComboMon(new CardsE3B.OpponentSwitchesMon("Roar"), new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("DS-80", new PlainMon());
            r.RegisterByKey("DS-81", new ComboMon(new DrawAttackMon("Collect", 1), new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("DS-82", new CardsE2.AnyOpponentMon("Dig Under", 10));
            r.RegisterByKey("DS-83", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("DS-84", new PlainMon());
            r.RegisterByKey("DS-85", new ComboMon(new TailsNothingMon("Surprise Attack"), new RecoverMon()));
            r.RegisterByKey("DS-86", new CardsMEG.SnipeMon("Skill Dive", 10));
            r.RegisterByKey("DS-87", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("DS-88", new CardsPAR2.FlipConditionMon("Poison Fang", SpecialCondition.Poisoned));
            r.RegisterByKey("DS-114", new AzumarillThickFat());
        }

        /// <summary>
        /// Type Match: show a basic Energy card from your hand, then a Basic Pokémon or Evolution card of that type from your deck into
        /// your hand (a δ Pokémon of 2 types counts as both).
        /// </summary>
        private sealed class TypeMatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Type Match") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var shown = ctx.ChooseCards(energy, 1, 1, "Type Match: show which basic Energy card from your hand?")[0];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { shown }, "showed for Type Match");
                var type = shown.Def.EnergyProvides;
                ctx.SearchToHand(d => d.IsPokemon && d.HasType(type), 1, "Type Match: choose a " + type + " Basic Pokémon or Evolution card");
                return true;
            }
        }

        /// <summary>
        /// Binding Aura (Poké-Body): the opponent can't play Basic Pokémon or Evolution cards from their hand to evolve their Active
        /// Pokémon, and can't attach Energy cards from their hand to an Asleep Pokémon. Sleep Inducer. Psyshot.
        /// </summary>
        private sealed class HypnoBindingAura : Mon
        {
            private static readonly Mon Inducer = new CardsE2.SleepInducerMon(false);

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner != src.Player && src.IsActive(target);
            }

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner != src.Player && target.Has(SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Inducer.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Fang: if the Defending Pokémon is Pokémon-ex, it can't attack during the opponent's next turn.</summary>
        private sealed class DeltaFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsOldEx && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Delta Fang)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Backup (Poké-Power): once during your turn, with less than 6 cards in your hand, draw until you have 6. Not with a Special
        /// Condition. Machine Burst: with a Technical Machine attached (one discarded after the attack still counts: Compendium), the
        /// Defending Pokémon is Asleep and Burned.
        /// </summary>
        private sealed class Porygon2Backup : Mon
        {
            private const string Power = "Backup";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(6); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Machine Burst") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool tm = me.Tools.Any(t => t.Def.Name.Contains("Technical Machine"));
                ctx.DamageDefending(atk.Damage);
                if (tm && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                }
                return true;
            }
        }

        /// <summary>Delta Storm (Poké-Body): while Sandslash is your Active Pokémon, 1 damage counter on each of the opponent's Pokémon-ex between turns. Rend.</summary>
        private sealed class SandslashDeltaStorm : Mon
        {
            private static readonly Mon Rend = CardsSS.ConditionBonus("Rend", 20, d => d.DamageCounters > 0);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var targets = src.OpponentPokemon(m => m.Def.IsOldEx);
                if (targets.Count == 0) return;
                Announce(src, "Delta Storm");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Delta Storm");
            }

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? 1 - src.Player : -2; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rend.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Prize Shift (Poké-Power): once during your turn, put a card from your hand as a Prize card face up, then put 1 of your
        /// face-down Prize cards (chosen without looking) into your hand. Not with a Special Condition, nor when all your Prize cards
        /// are face up. Psychic Option: 20 more with a Water Energy card attached; with a Psychic Energy card attached, a Special
        /// Energy card attached to the Defending Pokémon is discarded.
        /// </summary>
        private sealed class SlowkingPrizeShift : Mon
        {
            private const string Power = "Prize Shift";

            private static List<CardInstance> FaceDown(EffectContext ctx) { return ctx.Me.Prizes.Where(c => !ctx.Me.FaceUpPrizeIds.Contains(c.Id)).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count > 0 && FaceDown(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, Power + ": put which card from your hand as a face-up Prize card?")[0];
                var faceDown = FaceDown(ctx);
                if (faceDown.Count == 0 || !ctx.Me.Hand.Remove(card)) return;
                var labels = faceDown.Select((c, i) => "Face-down Prize card " + (i + 1)).ToArray();
                int pick = ctx.ChooseOneOption(Power + ": take which face-down Prize card (without looking)?", labels);
                var taken = faceDown[pick];
                int at = ctx.Me.Prizes.IndexOf(taken);
                ctx.Me.Prizes[at] = card;
                ctx.Me.FaceUpPrizeIds.Add(card.Id);
                ctx.Me.Hand.Add(taken);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "put this card as a face-up Prize card (Prize Shift)");
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a face-down Prize card into their hand (Prize Shift).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Option") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool water = me.Energy.Any(e => CardsDS.BasicOf(e.Def, EnergyType.Water));
                bool psychic = me.Energy.Any(e => CardsDS.BasicOf(e.Def, EnergyType.Psychic));
                ctx.DamageDefending(atk.Damage + (water ? 20 : 0));
                if (psychic && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Psychic Option: discard which Special Energy card from " + defending.Name + "?", e => e.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Holon Blizzard: with a Holon Energy card attached, 10 to each of the opponent's Benched Pokémon too.</summary>
        private sealed class HolonBlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Holon Blizzard") return false;
                bool holon = CardsDS.HolonEnergyCount(ctx.Pokemon) > 0;
                ctx.DamageDefending(atk.Damage);
                if (holon)
                    foreach (var mon in ctx.OpponentBench())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Holon Draw: draw a card; with a Holon Energy card attached, 2 more.</summary>
        private sealed class HolonDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Holon Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(CardsDS.HolonEnergyCount(ctx.Pokemon) > 0 ? 3 : 1);
                return true;
            }
        }

        /// <summary>Steep Dive: heads, prevent all effects of attacks, damage included, on Swellow during the opponent's next turn; tails, Glide's base damage is 100 during your next turn.</summary>
        private sealed class SteepDiveMon : Mon
        {
            private const string Marker = "Steep Dive";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Glide")
                {
                    int turn;
                    bool boosted = me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn;
                    ctx.DamageDefending(boosted ? 100 : atk.Damage);
                    return true;
                }
                if (atk.Name != "Steep Dive") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.ProtectSelfNextTurn();
                else if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                return true;
            }
        }

        /// <summary>Body Odor (Poké-Body): while Weezing is the Active Pokémon, 1 damage counter on each of the opponent's Pokémon that has a Poké-Body, between turns. Mist Attack: 10 to each of the opponent's Pokémon.</summary>
        private sealed class WeezingBodyOdor : Mon
        {
            private static readonly Mon Mist = new CardsSS.EachOpponentMon("Mist Attack", 10);

            private static bool HasBody(PokemonInPlay mon) { return mon.Def.Abilities.Any(a => a.Kind == AbilityDef.PokeBody); }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var targets = src.OpponentPokemon(HasBody);
                if (targets.Count == 0) return;
                Announce(src, "Body Odor");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Body Odor");
            }

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? 1 - src.Player : -2; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Mist.HandleAttack(ctx, atk); }
        }

        /// <summary>Sonic Signal: 10 to each of 2 of the opponent's Pokémon (no W/R for Benched).</summary>
        private sealed class SonicSignalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Signal") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                int n = Math.Min(2, all.Count);
                foreach (var mon in ctx.ChoosePokemonMany(all, n, n, "Sonic Signal: choose 2 of your opponent's Pokémon (10 damage each)"))
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Reversal Aura (Poké-Body): while you have more Prize cards left than the opponent, Hariyama's attacks do 20 more to the
        /// Active Pokémon (before W/R) and damage done to Hariyama by the opponent's Active Pokémon is 20 less (after W/R). Brick
        /// Smash: not affected by Resistance or effects on the Defending Pokémon.
        /// </summary>
        private sealed class HariyamaReversalAura : Mon
        {
            private static readonly Mon Smash = new CardsEM.PiercingMon("Brick Smash");

            private static bool Behind(EffectContext src) { return src.Me.Prizes.Count > src.Opp.Prizes.Count; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Behind(src) ? damage + 20 : damage;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !src.IsActive(a) || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Behind(src) ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smash.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Beacon Protection / Extra Protection (Poké-Body): with the partner (Volbeat / Illumise) in play, prevent all effects of attacks,
        /// damage included, done to this Pokémon by the opponent's Pokémon that match (Dark in the name / Pokémon-ex).
        /// </summary>
        internal sealed class PartnerProtectionMon : Mon
        {
            private readonly string body;
            private readonly string partner;
            private readonly Func<PokemonInPlay, bool> attacker;
            private readonly Mon attacks;

            public PartnerProtectionMon(string body, string partner, Func<PokemonInPlay, bool> attacker, Mon attacks)
            {
                this.body = body;
                this.partner = partner;
                this.attacker = attacker;
                this.attacks = attacks;
            }

            private bool Applies(EffectContext src, PokemonInPlay target, PokemonInPlay a)
            {
                return target != null && target == src.Pokemon && a != null && a.Owner != src.Player && attacker(a) && src.Me.InPlay().Any(m => m.Name == partner);
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Applies(src, target, src.Engine.CurrentSourcePokemon);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Applies(src, info.Target, info.Attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Prowl (Poké-Power): once during your turn, when you evolve into Persian from your hand, any 1 card from your deck into your hand. Snap Tail: 30 to 1 of the opponent's Pokémon.</summary>
        private sealed class PersianProwl : Mon
        {
            private const string Power = "Prowl";
            private static readonly Mon Tail = new CardsSSB.AnyOpponentTextMon("Snap Tail", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, ctx.Me.Deck.Count > 0, Power + ": search your deck for any card?")) return;
                ctx.SearchToHand(d => true, 1, Power + ": choose any card", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tail.HandleAttack(ctx, atk); }
        }

        /// <summary>Conductive Body (Poké-Body): while Beldum is your Active Pokémon, its Retreat Cost is [C] less for each Beldum on your Bench. Take Down.</summary>
        private sealed class BeldumConductiveBody : Mon
        {
            private static readonly Mon TakeDown = new RecoilMon("Take Down", 10);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Pokemon || !src.IsActive(mon)) return cost;
                return cost - src.Me.Bench.Count(m => m.Name == "Beldum");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TakeDown.HandleAttack(ctx, atk); }
        }

        /// <summary>Pickup Power: an Energy card from your discard pile into your hand.</summary>
        private sealed class PickupPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pickup Power") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(cards, 1, 1, "Pickup Power: put which Energy card into your hand?"));
                return true;
            }
        }

        /// <summary>Recover: discard a Water Energy card attached to Staryu, then remove 4 damage counters from it (all if fewer).</summary>
        private sealed class RecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Recover: discard which Water Energy card?", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Water)) == 0) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 10 * Math.Min(4, me.DamageCounters));
                return true;
            }
        }

        /// <summary>Thick Fat (Poké-Body): damage done to Azumarill by attacks from Fire and Water Pokémon is 30 less (after W/R). Bubblebeam.</summary>
        private sealed class AzumarillThickFat : Mon
        {
            private static readonly Mon Bubblebeam = new ParalyzeFlipMon("Bubblebeam");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(a, EnergyType.Fire) || src.Engine.IsType(a, EnergyType.Water) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bubblebeam.HandleAttack(ctx, atk); }
        }
    }
}
