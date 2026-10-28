using PokemonTCG.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// One card on the table: a front quad and a back quad on a flat root. The board sets target
    /// position/rotation/scale; the card glides there, so every zone change animates for free.
    /// </summary>
    public sealed class CardView : MonoBehaviour
    {
        public const float Width = 1.4f;
        public const float Height = 1.95f;

        public int Id;
        public CardDef Def;
        public Vector3 TargetPosition;
        public Quaternion TargetRotation = Quaternion.identity;
        public Vector3 TargetScale = Vector3.one;
        public bool Liftable;
        public bool Hovered;
        /// <summary>Being dragged from the hand (drawn at the mouse instead).</summary>
        public bool Dragged;

        private MeshRenderer front;
        /// <summary>v134.5: the face image is still to be loaded (the back shows until then).</summary>
        private bool facePending;
        private float speed = 12f;

        /// <summary>
        /// Imakuni?'s Doduo being thrown into the Lost Zone (Frenzied Escape, v92, your request): instead of gliding, the card
        /// flies there in an arc for ThrowSeconds, spinning wildly the whole way, and stops spinning the moment it lands.
        /// </summary>
        private bool thrown;
        private float throwTime;
        private Vector3 throwFrom;
        private Quaternion throwFromRotation;
        private Vector3 throwFromScale;
        private const float ThrowSeconds = 1.3f;
        /// <summary>Whole turns about each axis during the flight, so the card lands exactly flat with no snap.</summary>
        private const float SpinTurnsY = 7f, SpinTurnsX = 3f, SpinTurnsZ = 2f;

        /// <summary>True while a thrown card is still in the air.</summary>
        public bool InFlight { get { return thrown; } }

        /// <summary>Starts the spinning throw from wherever the card is now to its target (the Lost Zone).</summary>
        public void Throw()
        {
            if (thrown) return;
            thrown = true;
            throwTime = 0f;
            throwFrom = transform.localPosition;
            throwFromRotation = transform.localRotation;
            throwFromScale = transform.localScale;
        }

        /// <summary>One frame of the throw: an eased arc above the table, spinning at a steady rate until it lands.</summary>
        private void UpdateThrow()
        {
            throwTime += Time.deltaTime;
            float u = Mathf.Clamp01(throwTime / ThrowSeconds);
            float eased = u * u * (3f - 2f * u);
            transform.localPosition = Vector3.Lerp(throwFrom, TargetPosition, eased) + new Vector3(0f, 2.4f * Mathf.Sin(Mathf.PI * u), 0f);
            var spin = Quaternion.Euler(360f * SpinTurnsX * u, 360f * SpinTurnsY * u, 360f * SpinTurnsZ * u);
            transform.localRotation = Quaternion.Slerp(throwFromRotation, TargetRotation, eased) * spin;
            transform.localScale = Vector3.Lerp(throwFromScale, TargetScale, eased);
            if (u < 1f) return;
            thrown = false;
            Snap(); // landed: no more spinning
        }

        public static CardView Create(Transform parent, int id, CardDef def)
        {
            var root = new GameObject(def != null ? def.Name + " #" + id : "Hidden card " + id);
            root.transform.SetParent(parent, false);
            var view = root.AddComponent<CardView>();
            view.Id = id;
            view.Def = def;
            var face = CardVisuals.FaceWithinBudget(def);
            view.facePending = face == null;
            view.front = MakeQuad(root.transform, "Front", Quaternion.Euler(90f, 0f, 0f), 0.001f, face ?? CardVisuals.Back());
            MakeQuad(root.transform, "Back", Quaternion.Euler(-90f, 0f, 0f), -0.001f, CardVisuals.Back());
            var col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(Width, 0.03f, Height);
            return view;
        }

        private static MeshRenderer MakeQuad(Transform parent, string name, Quaternion rotation, float yOffset, Texture texture)
        {
            var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            q.name = name;
            var meshCollider = q.GetComponent<Collider>();
            if (meshCollider != null) Destroy(meshCollider);
            q.transform.SetParent(parent, false);
            q.transform.localPosition = new Vector3(0f, yOffset, 0f);
            q.transform.localRotation = rotation;
            q.transform.localScale = new Vector3(Width, Height, 1f);
            var mr = q.GetComponent<MeshRenderer>();
            mr.sharedMaterial = CardVisuals.MaterialFor(texture);
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
            return mr;
        }

        /// <summary>Used when a hidden card becomes known (e.g. the opponent plays it).</summary>
        public void SetDef(CardDef def)
        {
            Def = def;
            name = def != null ? def.Name + " #" + Id : "Hidden card " + Id;
            if (front == null) return;
            var face = CardVisuals.FaceWithinBudget(def);
            facePending = face == null;
            front.sharedMaterial = CardVisuals.MaterialFor(face ?? CardVisuals.Back());
        }

        public void Snap()
        {
            transform.localPosition = TargetPosition;
            transform.localRotation = TargetRotation;
            transform.localScale = TargetScale;
        }

        private void Update()
        {
            if (facePending && front != null)
            {
                var face = CardVisuals.FaceWithinBudget(Def);
                if (face != null)
                {
                    facePending = false;
                    front.sharedMaterial = CardVisuals.MaterialFor(face);
                }
            }
            if (thrown)
            {
                UpdateThrow();
                return;
            }
            float t = 1f - Mathf.Exp(-speed * Time.deltaTime);
            bool lift = Liftable && Hovered && !Dragged;
            Vector3 pos = TargetPosition + (lift ? new Vector3(0f, 0.8f, 0.9f) : Dragged ? new Vector3(0f, -0.4f, -0.9f) : Vector3.zero);
            Vector3 scale = TargetScale * (lift ? 1.25f : Dragged ? 0.6f : 1f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos, t);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, TargetRotation, t);
            transform.localScale = Vector3.Lerp(transform.localScale, scale, t);
        }

        /// <summary>Screen-space rectangle (GUI coordinates, y down) covering this card.</summary>
        public Rect ScreenRect(Camera cam)
        {
            var corners = new[]
            {
                new Vector3(-Width / 2f, 0f, -Height / 2f),
                new Vector3(Width / 2f, 0f, -Height / 2f),
                new Vector3(-Width / 2f, 0f, Height / 2f),
                new Vector3(Width / 2f, 0f, Height / 2f)
            };
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var c in corners)
            {
                // v118: real pixels to GUI units (UiScale).
                var gp = UiScale.FromScreenPixels(cam.WorldToScreenPoint(transform.TransformPoint(c)));
                minX = Mathf.Min(minX, gp.x);
                maxX = Mathf.Max(maxX, gp.x);
                minY = Mathf.Min(minY, gp.y);
                maxY = Mathf.Max(maxY, gp.y);
            }
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
    }
}
