using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Origin (LOR) part 2: the Trainer Gallery Pokémon that weren't already in the pool (TG3 Charizard, TG7 Banette,
    /// TG11 Castform, TG12-TG17 Orbeetle, Centiskorch and Pikachu V/VMAX, TG20-TG22 Crobat V and Eternatus V/VMAX), the
    /// Trainers and Gift Energy. Pokémon Tools are registered by name with the other Trainers. Your rulings (v51): Mr. Mime's
    /// Rock-Paper-Scissors is real Rock-Paper-Scissors against a bot throwing at random; Ancient Star and the Abilities that
    /// shield Pokémon from it: whichever came first wins.
    /// </summary>
    public static class CardsLOR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LOR-TG3", new CharizardLOR());
            r.RegisterByKey("LOR-TG7", new ComboMon(new ResoluteSpiteMon(), new ConditionAttackMon("Eerie Light", SpecialCondition.Confused)));
            r.RegisterByKey("LOR-TG11", new CastformLOR());
            r.RegisterByKey("LOR-TG12", new ComboMon(new SwitchSelfMon("Strafe", true), new CardsMEG.DefendingEnergyBonusMon("Mysterious Wave", 30)));
            r.RegisterByKey("LOR-TG13", new OrbeetleVmax());
            r.RegisterByKey("LOR-TG14", new RadiatingHeatMon());
            r.RegisterByKey("LOR-TG15", new CentifernoMon());
            r.RegisterByKey("LOR-TG16", new ComboMon(new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 2), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("LOR-TG17", new VoltTackleMon());
            r.RegisterByKey("LOR-TG20", new CrobatV());
            r.RegisterByKey("LOR-TG21", new ComboMon(new PowerAcceleratorMon(), new CardsCRZ1.VmaxBonusMon("Dynamax Cannon", 120)));
            r.RegisterByKey("LOR-TG22", new EternatusVmax());

            // Supporters
            r.RegisterByName("Arezu", new Arezu());
            r.RegisterByName("Fantina", new Fantina());
            r.RegisterByName("Iscan", new Iscan());
            r.RegisterByName("Lady", new Lady());
            r.RegisterByName("Miss Fortune Sisters", new MissFortuneSisters());
            r.RegisterByName("Riley", new Riley());
            r.RegisterByName("Thorton", new Thorton());
            r.RegisterByName("Adventurer's Discovery", new AdventurersDiscovery());
            r.RegisterByName("Kabu", new Kabu());
            r.RegisterByName("Opal", new Opal());
            // Items
            r.RegisterByName("Arc Phone", new ArcPhone());
            r.RegisterByName("Damage Pump", new DamagePump());
            r.RegisterByName("Mirage Gate", new MirageGate());
            r.RegisterByName("Tool Box", new ToolBox());
            r.RegisterByName("Dark Patch", new DarkPatch());
            // Pokémon Tools
            r.RegisterByName("Box of Disaster", new BoxOfDisaster());
            r.RegisterByName("Panic Mask", new PanicMask());
            r.RegisterByName("Windup Arm", new WindupArm());
            // Stadiums
            r.RegisterByName("Lake Acuity", new LakeAcuity());
            r.RegisterByName("Lost City", new LostCity());
            r.RegisterByName("Collapsed Stadium", new CollapsedStadium());
            // Special Energy
            r.RegisterByName("Gift Energy", new GiftEnergy());
        }

        // ================================================================== Trainer Gallery Pokémon

        /// <summary>"Leon card": a card named Leon (the Supporter), or a card of Leon's.</summary>
        private static bool IsLeonCard(CardDef d)
        {
            return d.Name == "Leon" || d.Name.StartsWith("Leon's ");
        }

        /// <summary>Battle Sense: once during your turn, look at the top 3 cards, put 1 into your hand and discard the others. Royal Blaze: 50 more for each Leon card in your discard pile.</summary>
        private sealed class CharizardLOR : Mon
        {
            protected override string Activated { get { return "Battle Sense"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return;
                var pick = ctx.ChooseCards(top, 1, 1, "Battle Sense: put 1 of these cards into your hand (the others are discarded)")[0];
                ctx.DeckToHand(new List<CardInstance> { pick });
                var rest = new List<CardInstance>();
                foreach (var card in top)
                {
                    if (card == pick || !ctx.Me.Deck.Remove(card)) continue;
                    ctx.Me.Discard.Add(card);
                    rest.Add(card);
                }
                if (rest.Count > 0)
                    ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", rest.Select(c => c.Def.Name).ToArray()) + " (Battle Sense).", rest);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Royal Blaze") return false;
                int n = ctx.Me.Discard.Count(c => IsLeonCard(c.Def));
                ctx.DamageDefending(atk.Damage + 50 * n);
                return true;
            }
        }

        /// <summary>
        /// Resolute Spite: put up to 7 damage counters on this Pokémon; 20 damage for each one placed. A bot doesn't put on so many
        /// that it Knocks itself Out.
        /// </summary>
        private sealed class ResoluteSpiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Resolute Spite") return false;
                var me = ctx.Pokemon;
                int max = 7;
                if (!ctx.Engine.IsInteractive(ctx.Player)) max = Math.Min(7, Math.Max(0, (ctx.RemainingHp(me) - 10) / 10));
                int n = max <= 0 ? 0 : ctx.AskNumber(0, max, "Resolute Spite: put how many damage counters on this Pokémon? (20 damage for each)");
                int placed = n > 0 ? ctx.Engine.PlaceDamageCounters(me, n, "Resolute Spite") : 0;
                if (placed > 0) ctx.DamageDefending(20 * placed);
                else ctx.Log(me.Name + "'s Resolute Spite did no damage.");
                return true;
            }
        }

        /// <summary>Weather Reading: with 8 or more Stadium cards in your discard pile, its attacks cost no Energy. Weather Force: draw until you have 6 cards.</summary>
        private sealed class CastformLOR : Mon
        {
            private static readonly Mon force = new CardsBCR1.DrawUntilAttackMon("Weather Force", 6);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon != null && mon == src.Pokemon && src.Me.Discard.Count(c => c.Def.IsStadiumCard) >= 8) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return force.HandleAttack(ctx, atk); }
        }

        /// <summary>Eerie Beam: once during your turn, if this Pokémon is Active, 1 damage counter on each of the opponent's Pokémon. G-Max Wave: 50 more per Energy on their Active.</summary>
        private sealed class OrbeetleVmax : Mon
        {
            private static readonly Mon wave = new CardsMEG.DefendingEnergyBonusMon("G-Max Wave", 50);

            protected override string Activated { get { return "Eerie Beam"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return wave.HandleAttack(ctx, atk); }
        }

        /// <summary>Radiating Heat: 20; you may discard an Energy from this Pokémon, and if you do, discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class RadiatingHeatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Radiating Heat") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Radiating Heat: discard an Energy from this Pokémon to discard an Energy from your opponent's Active Pokémon?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Radiating Heat: discard an Energy from this Pokémon") == 0) return true;
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Radiating Heat: discard an Energy from your opponent's Active Pokémon");
                return true;
            }
        }

        /// <summary>G-Max Centiferno: 40 more per Fire Energy on this Pokémon; if it did damage, you may attach a Fire Energy card from your discard pile to it.</summary>
        private sealed class CentifernoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Centiferno") return false;
                var me = ctx.Pokemon;
                int n = CardsCRZ1.EnergyOfType(ctx, me, EnergyType.Fire).Count;
                int dealt = ctx.DamageDefending(atk.Damage + 40 * n);
                if (dealt <= 0 || !ctx.InPlay(me)) return true;
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count == 0 || !ctx.AskYesNo("G-Max Centiferno: attach a Fire Energy card from your discard pile to this Pokémon?")) return true;
                var pick = ctx.ChooseCards(fire, 1, 1, "G-Max Centiferno: choose a Fire Energy card")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, me);
                return true;
            }
        }

        /// <summary>G-Max Volt Tackle: you may discard all Energy from this Pokémon for 150 more damage.</summary>
        private sealed class VoltTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Volt Tackle") return false;
                var me = ctx.Pokemon;
                bool more = me.Energy.Count > 0 && ctx.AskYesNo("G-Max Volt Tackle: discard all Energy from this Pokémon for 150 more damage?")
                            && ctx.Engine.DiscardAllEnergy(me) > 0;
                ctx.DamageDefending(atk.Damage + (more ? 150 : 0));
                return true;
            }
        }

        /// <summary>Dark Asset: when played from your hand onto your Bench, you may draw until you have 6 cards (1 Dark Asset per turn). Venomous Fang: Poisoned.</summary>
        private sealed class CrobatV : Mon
        {
            private const string Asset = "Dark Asset";
            private static readonly Mon fang = new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned);

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!ctx.IsMyTurn || ctx.NamedLimitUsedThisTurn(Asset) || ctx.Me.Hand.Count >= 6 || ctx.Me.Deck.Count == 0) return;
                if (!ctx.AskYesNo("Use Dark Asset to draw cards until you have 6 cards in your hand?")) return;
                ctx.MarkNamedLimit(Asset);
                ctx.MarkAbilityUsed(Asset);
                Announce(ctx, Asset);
                ctx.DrawUntil(6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return fang.HandleAttack(ctx, atk); }
        }

        /// <summary>Power Accelerator: 30; you may attach a Darkness Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class PowerAcceleratorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Accelerator") return false;
                ctx.DamageDefending(atk.Damage);
                CardsCRZ1.AttachEnergyFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Darkness), 1, m => !ctx.IsActive(m), "Power Accelerator");
                return true;
            }
        }

        /// <summary>
        /// Eternal Zone: if all your Pokémon in play are Darkness type, you can have up to 8 Benched Pokémon and can't put
        /// non-Darkness Pokémon into play by any means (dual types with Darkness are fine). When it stops working the Bench is cut
        /// back to 5 before the next Active Pokémon is chosen (Compendium). Dread End: 30 for each of your Darkness Pokémon.
        /// </summary>
        private sealed class EternatusVmax : Mon
        {
            private static bool AllDarkness(EffectContext src)
            {
                return src.Me.InPlay().All(m => src.Engine.IsType(m, EnergyType.Darkness));
            }

            public override int ModifyMaxBench(EffectContext src, int player, int max)
            {
                return src.Pokemon != null && player == src.Player && AllDarkness(src) ? Math.Max(max, 8) : max;
            }

            public override bool BlocksPlayingPokemon(EffectContext src, int player, CardDef card)
            {
                return src.Pokemon != null && player == src.Player && !card.HasType(EnergyType.Darkness) && AllDarkness(src);
            }

            public override bool BlocksPuttingIntoPlay(EffectContext src, int player, CardDef card)
            {
                return BlocksPlayingPokemon(src, player, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dread End") return false;
                int n = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Darkness)).Count;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        // ================================================================== Supporters

        /// <summary>Arezu: search your deck for up to 3 Evolution Pokémon that don't have a Rule Box.</summary>
        private sealed class Arezu : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution && !d.HasRuleBox, 3, "Arezu: choose up to 3 Evolution Pokémon without a Rule Box");
            }
        }

        /// <summary>Fantina: only with 10 or more cards in your Lost Zone; during the opponent's next turn your Pokémon take 120 less damage from their Pokémon V.</summary>
        private sealed class Fantina : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.LostZone.Count >= 10; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new CardsCRZ1.TeamShieldEffect("Fantina: your Pokémon take 120 less damage from the opponent's Pokémon V during their next turn",
                    ctx.Player, ctx.Turn + 1, 120, attacker => attacker.Def.IsPokemonV));
            }
        }

        /// <summary>Iscan: draw 2 cards, 2 more if your Active Pokémon has "Hisuian" in its name.</summary>
        private sealed class Iscan : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool hisuian = ctx.Me.Active != null && ctx.Me.Active.Name.Contains("Hisuian");
                ctx.Draw(hisuian ? 4 : 2);
            }
        }

        /// <summary>Lady: search your deck for up to 4 basic Energy cards.</summary>
        private sealed class Lady : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy, 4, "Lady: choose up to 4 basic Energy cards");
            }
        }

        /// <summary>Miss Fortune Sisters: look at the top 5 cards of the opponent's deck, discard any Item cards there, and they shuffle the rest back.</summary>
        private sealed class MissFortuneSisters : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Opp.Deck.Take(Math.Min(5, ctx.Opp.Deck.Count)).ToList();
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
                var items = top.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0 && !ctx.Engine.DeckDiscardBlocked(ctx.Opponent))
                {
                    var picks = ctx.ChooseCards(items, 0, items.Count, "Miss Fortune Sisters: discard any number of Item cards from the top of your opponent's deck");
                    foreach (var card in picks)
                    {
                        if (!ctx.Opp.Deck.Remove(card)) continue;
                        ctx.Opp.Discard.Add(card);
                    }
                    if (picks.Count > 0)
                        ctx.Engine.EmitCards(GameEventType.Discard, ctx.Opponent, "Miss Fortune Sisters discarded " + string.Join(", ", picks.Select(x => x.Def.Name).ToArray()) + " from the top of " + ctx.Engine.Their(ctx.Opponent) + " deck.", picks);
                }
                // The deck is still shuffled when nothing was discarded (Compendium, 2024-04-18).
                ctx.Engine.ShuffleDeck(ctx.Opponent);
            }
        }

        /// <summary>Riley: reveal the top 5 cards of your deck; your opponent chooses 2 of them to discard and the rest go into your hand.</summary>
        private sealed class Riley : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Me.Deck.Take(Math.Min(5, ctx.Me.Deck.Count)).ToList();
                ctx.Engine.Reveal(ctx.Player, top, "revealed the top of " + ctx.Engine.Their(ctx.Player) + " deck");
                int n = Math.Min(2, top.Count);
                // Milotic's Dew Guard: the opponent can't be made to choose, so you keep all of them (Compendium, 2022-09-29).
                var chosen = ctx.Engine.SupporterEffectsPreventedOn(ctx.Opponent) ? new List<CardInstance>()
                    : ctx.Engine.ChooseCards(ctx.Opponent, top, n, n, "Riley: choose " + n + " of these cards for your opponent to discard (they keep the rest)", ctx.Source);
                foreach (var card in top)
                {
                    if (!ctx.Me.Deck.Remove(card)) continue;
                    if (chosen.Contains(card)) ctx.Me.Discard.Add(card);
                    else ctx.Me.Hand.Add(card);
                }
                if (chosen.Count > 0)
                    ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", chosen.Select(x => x.Def.Name).ToArray()) + " (Riley).", chosen);
            }
        }

        /// <summary>
        /// Thorton: switch a Basic Pokémon in your discard pile with 1 of your Basic Pokémon in play. Attached cards, damage
        /// counters, Special Conditions, turns in play and other effects stay on the new Pokémon (Compendium: it can be Knocked
        /// Out at once by the damage it keeps).
        /// </summary>
        private sealed class Thorton : CardScript
        {
            private static List<CardInstance> InDiscard(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            private static List<PokemonInPlay> InPlay(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count == 1 && m.Def.IsBasicPokemon);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return InDiscard(ctx).Count > 0 && InPlay(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = InDiscard(ctx);
                var targets = InPlay(ctx);
                if (pile.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(pile, 1, 1, "Thorton: choose a Basic Pokémon in your discard pile")[0];
                var mon = ctx.ChoosePokemon(targets, "Thorton: switch " + card.Def.Name + " with which of your Basic Pokémon in play?");
                if (mon == null || !ctx.Me.Discard.Remove(card)) return;
                var old = mon.Stack[0];
                mon.Stack[0] = card;
                ctx.Me.Discard.Add(old);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " in play with " + card.Def.Name + " from " + ctx.Engine.Their(ctx.Player) + " discard pile (Thorton).", card.Id, mon.Uid);
            }
        }

        /// <summary>Adventurer's Discovery: search your deck for up to 3 Pokémon V.</summary>
        private sealed class AdventurersDiscovery : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemonV, 3, "Adventurer's Discovery: choose up to 3 Pokémon V");
            }
        }

        /// <summary>Kabu: shuffle your hand into your deck and draw 4 cards, or 8 if your Active Pokémon is your only Pokémon in play.</summary>
        private sealed class Kabu : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool alone = ctx.Me.Active != null && ctx.Me.Bench.Count == 0;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(alone ? 8 : 4);
            }
        }

        /// <summary>Opal: flip 2 coins; search your deck for up to that many cards.</summary>
        private sealed class Opal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.SearchToHand(d => true, heads, "Opal: choose up to " + heads + (heads == 1 ? " card" : " cards") + " to put into your hand", false);
                else ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== Items

        /// <summary>Arc Phone: look at the top card of your deck; you may switch it with 1 of your face-down Prize cards (they stay face down).</summary>
        private sealed class ArcPhone : CardScript
        {
            public const string KeepLabel = "Don't switch";

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var top = me.Deck[0];
                ctx.Engine.NoteLookedAt(new List<CardInstance> { top }, "Top card of your deck", ctx.Player);
                var slots = me.PrizeSlotList();
                var faceDown = new List<int>();
                for (int i = 0; i < me.Prizes.Count; i++)
                    if (!me.FaceUpPrizeIds.Contains(me.Prizes[i].Id)) faceDown.Add(i);
                faceDown.Sort((a, b) => slots[a].CompareTo(slots[b]));
                var labels = new List<string> { KeepLabel };
                foreach (int idx in faceDown) labels.Add("Switch it with Prize card " + (slots[idx] + 1));
                int pick = ctx.ChooseOption("Arc Phone: the top card of your deck is " + top.Def.Name + ". Switch it with 1 of your face-down Prize cards?", labels.ToArray());
                if (pick <= 0 || pick > faceDown.Count) return;
                int index = faceDown[pick - 1];
                var prize = me.Prizes[index];
                int slot = slots[index];
                me.Prizes[index] = top;
                me.Deck[0] = prize;
                me.PrizeSlotByCard.Remove(prize.Id);
                me.PrizeSlotByCard[top.Id] = slot;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched the top card of " + ctx.Engine.Their(ctx.Player) + " deck with a face-down Prize card (Arc Phone).");
            }
        }

        /// <summary>Damage Pump: move up to 2 damage counters from 1 of your Pokémon to your other Pokémon in any way you like.</summary>
        private sealed class DamagePump : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon().Count > 1 && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var from = ctx.ChoosePokemon(hurt, "Damage Pump: move damage counters from which of your Pokémon?");
                if (from == null) return;
                for (int i = 0; i < 2 && from.DamageCounters > 0; i++)
                {
                    var others = ctx.MyPokemon(x => x != from);
                    if (others.Count == 0) break;
                    if (i > 0 && !ctx.AskYesNo("Damage Pump: move another damage counter from " + from.Name + "?")) break;
                    var to = ctx.ChoosePokemon(others, "Damage Pump: move damage counter " + (i + 1) + " of up to 2 from " + from.Name + " to which Pokémon?");
                    if (to == null) break;
                    ctx.Engine.MoveDamageCounters(from, to, 1, "Damage Pump");
                }
            }
        }

        /// <summary>Mirage Gate: only with 7 or more cards in your Lost Zone; search for up to 2 basic Energy of different types and attach them to your Pokémon in any way you like.</summary>
        private sealed class MirageGate : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.LostZone.Count >= 7 && ctx.Me.Deck.Count > 0 && ctx.Me.HasPokemonInPlay;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Mirage Gate: choose a basic Energy card (up to 2, of different types)");
                if (found.Count > 0)
                {
                    var first = found[0].Def.EnergyProvides;
                    found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides != first, 1, "Mirage Gate: choose a basic Energy card of a different type than " + found[0].Def.Name));
                }
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, mon) => true, "Mirage Gate");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Tool Box: look at the top 7 cards, put any number of Pokémon Tool cards among them into your hand, shuffle the rest back.</summary>
        private sealed class ToolBox : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var tools = top.Where(c => c.Def.IsTool).ToList();
                var picks = ctx.ChooseCards(tools, 0, tools.Count, "Tool Box: put any number of Pokémon Tool cards into your hand");
                if (picks.Count > 0)
                {
                    ctx.DeckToHand(picks);
                    ctx.Engine.Reveal(ctx.Player, picks, "revealed");
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Dark Patch: attach a basic Darkness Energy card from your discard pile to 1 of your Benched Darkness Pokémon.</summary>
        private sealed class DarkPatch : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Darkness)).ToList();
            }

            private static List<CardInstance> Energy(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Darkness);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 && Energy(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = Energy(ctx);
                var targets = Targets(ctx);
                if (energy.Count == 0 || targets.Count == 0) return;
                var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0]
                    : ctx.ChooseCards(energy, 1, 1, "Dark Patch: choose a basic Darkness Energy card from your discard pile")[0];
                var target = ctx.ChoosePokemon(targets, "Dark Patch: attach " + card.Def.Name + " to which Benched Darkness Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Box of Disaster: if the Pokémon V holding it is Knocked Out from full HP by the opponent's attack damage, 8 damage counters on the Attacking Pokémon.</summary>
        private sealed class BoxOfDisaster : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || me == null || !me.Def.IsPokemonV) return;
                // It had full HP: every damage counter on it came from attack damage this turn.
                bool fullBefore = me.AttackDamageTurn == src.Turn && me.DamageCounters * 10 <= me.AttackDamageThatTurn;
                if (!fullBefore) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                if (attacker == null) return;
                src.Engine.PlaceDamageCounters(attacker, 8, "Box of Disaster");
            }
        }

        /// <summary>Panic Mask: prevent all damage done to its Pokémon by attacks from the opponent's Pokémon that have 40 HP or less remaining.</summary>
        private sealed class PanicMask : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent) return damage;
                return src.Engine.RemainingHp(info.Attacker) <= 40 ? 0 : damage;
            }
        }

        /// <summary>Windup Arm: the Pokémon holding it can attack even if it's Asleep or Paralyzed.</summary>
        private sealed class WindupArm : CardScript
        {
            public override bool AllowsAttackWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon;
            }
        }

        // ================================================================== Stadiums

        /// <summary>Lake Acuity: Pokémon with any Water or Fighting Energy attached (both players') take 20 less damage from the opponent's attacks.</summary>
        private sealed class LakeAcuity : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (!info.AgainstOpponent) return damage;
                var target = info.Target;
                foreach (var e in target.Energy)
                    if (CardsDRI1.ProvidesType(src, target, e, EnergyType.Water) || CardsDRI1.ProvidesType(src, target, e, EnergyType.Fighting)) return damage - 20;
                return damage;
            }
        }

        /// <summary>
        /// Lost City: a Knocked Out Pokémon (either player's) goes to the Lost Zone, its whole evolution stack included; attached
        /// cards are discarded (Compendium, 2022-10-13). Lunatone's New Moon keeps its owner's Pokémon out of it.
        /// </summary>
        private sealed class LostCity : CardScript
        {
            public override int LostZoneOnKnockOut(EffectContext src, PokemonInPlay mon)
            {
                return src.Engine.StadiumShielded(mon) ? 0 : 1;
            }
        }

        /// <summary>
        /// Collapsed Stadium: no player can have more than 4 Benched Pokémon (the smaller number wins over effects that raise it);
        /// when it's played, its owner discards down to 4 first, then the opponent.
        /// </summary>
        private sealed class CollapsedStadium : CardScript
        {
            public override int CapMaxBench(EffectContext src, int player) { return 4; }

            public override void OnStadiumPlayed(EffectContext ctx)
            {
                ctx.Engine.EnforceBenchLimits(ctx.Player);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Gift Energy: Colorless. If its Pokémon is Knocked Out by the opponent's attack damage, its owner draws until they have 7 cards.</summary>
        private sealed class GiftEnergy : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Me.Hand.Count >= 7) return;
                src.DrawUntil(7);
            }
        }
    }
}
