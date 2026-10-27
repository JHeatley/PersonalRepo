using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PokemonTCG
{
    /// <summary>One card on screen: art (or a text frame if the art can't load), plus
    /// damage, energy, tool and conditions when it's a Pokémon in play.</summary>
    public class CardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public CardData Card;
        public MonData Mon;
        public string Ref;  // board slot ("you:0"...) or "hand"

        public Action<CardView> Clicked;
        public Action<CardView> HoverStart;
        public Action<CardView> HoverEnd;

        Image glow;
        Image frame;
        RawImage art;
        Text title;
        Text info;
        RectTransform energyRow;
        Text badge;
        string loadingUrl;

        public static CardView Create(Transform parent, float width, ImageCache images)
        {
            float height = width * 88f / 63f;
            var root = Ui.Rect("Card", parent);
            Ui.Size(root, width, height);
            var view = root.gameObject.AddComponent<CardView>();

            view.glow = root.gameObject.AddComponent<Image>();
            view.glow.color = new Color(0, 0, 0, 0);  // also the click target

            view.frame = Ui.Panelled("Frame", root, Ui.PanelLight);
            Ui.Fill(view.frame.rectTransform, 4);
            view.frame.raycastTarget = false;

            view.title = Ui.Label(view.frame.transform, "", Mathf.RoundToInt(width / 9f), TextAnchor.UpperCenter,
                                  Color.white, FontStyle.Bold);
            Ui.Fill(view.title.rectTransform, 6);

            var artRt = Ui.Fill(Ui.Rect("Art", view.frame.transform));
            view.art = artRt.gameObject.AddComponent<RawImage>();
            view.art.raycastTarget = false;
            view.art.enabled = false;

            // Bottom strip for Pokémon in play: HP and energy
            var strip = Ui.Panelled("Strip", view.frame.transform, new Color(0, 0, 0, 0.72f));
            Ui.Place(strip.rectTransform, 0, 0, 1, 0.24f);
            strip.raycastTarget = false;
            view.info = Ui.Label(strip.transform, "", Mathf.RoundToInt(width / 8.5f), TextAnchor.UpperCenter, Color.white,
                                 FontStyle.Bold);
            Ui.Place(view.info.rectTransform, 0, 0.45f, 1, 1);
            view.energyRow = Ui.Place(Ui.Rect("Energy", strip.transform), 0.04f, 0.04f, 0.96f, 0.48f);
            Ui.Row(view.energyRow, 2, TextAnchor.MiddleCenter);
            view.badge = Ui.Label(view.frame.transform, "", Mathf.RoundToInt(width / 10f), TextAnchor.UpperRight,
                                  Ui.Accent, FontStyle.Bold);
            Ui.Place(view.badge.rectTransform, 0, 0.6f, 0.97f, 0.98f);
            view.images = images;
            return view;
        }

        ImageCache images;

        public void SetCard(CardData card)
        {
            Card = card;
            Mon = null;
            frame.color = Ui.KindColor(card);
            title.text = card != null ? card.name : "";
            info.transform.parent.gameObject.SetActive(false);
            badge.text = "";
            LoadArt(card);
        }

        public void SetMon(MonData mon)
        {
            SetCard(mon.card);
            Mon = mon;
            info.transform.parent.gameObject.SetActive(true);
            int left = Mathf.Max(0, mon.hp - mon.damage);
            info.text = left + "/" + mon.hp;
            info.color = mon.damage == 0 ? Color.white : left <= mon.hp / 3 ? new Color(1f, 0.4f, 0.35f) : Ui.Accent;
            Ui.Clear(energyRow);
            foreach (var e in mon.energy)
            {
                var dot = Ui.Panelled("E", energyRow, Ui.EnergyColor(e));
                Ui.Size(dot, 14, 14);
                var t = Ui.Label(dot.transform, e, 11, TextAnchor.MiddleCenter, Color.black, FontStyle.Bold);
                Ui.Fill(t.rectTransform);
            }
            string extra = "";
            if (mon.tool != null && mon.tool.Length > 0) extra += "Tool ";
            if (mon.conditions != null)
                foreach (var c in mon.conditions)
                    extra += c.Substring(0, 1).ToUpper() + c.Substring(1, Math.Min(3, c.Length - 1)) + " ";
            badge.text = extra.Trim();
        }

        public void SetHighlight(bool on)
        {
            glow.color = on ? Ui.Accent : new Color(0, 0, 0, 0);
        }

        void LoadArt(CardData card)
        {
            art.enabled = false;
            title.enabled = true;
            loadingUrl = card != null ? card.image : null;
            if (string.IsNullOrEmpty(loadingUrl) || images == null)
                return;
            string url = loadingUrl;
            images.Get(url, tex =>
            {
                if (this == null || loadingUrl != url || tex == null)
                    return;
                art.texture = tex;
                art.enabled = true;
                title.enabled = false;
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Clicked != null) Clicked(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HoverStart != null) HoverStart(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (HoverEnd != null) HoverEnd(this);
        }
    }
}
