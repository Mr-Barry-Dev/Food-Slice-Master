using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 2D knife par lagao. Collider2D chahiye (script usse Is Trigger bana deti hai),
/// Rigidbody2D (Kinematic) khud add kar leti hai.
/// followPointer ON ho to knife mouse/touch ke saath chalta hai.
/// Tez swipe me bhi cut hoga (beech ka raasta Linecast se check hota hai).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SliceKnife2D : MonoBehaviour
{
    [Header("Slice")]
    [Tooltip("Is speed (units/sec) se tez chalega tabhi cut hoga")]
    public float minSliceSpeed = 3f;

    [Header("Pointer control (optional)")]
    public bool followPointer = true;
    public bool needPressHeld = true;
    [Range(0.1f, 1f)] public float followSmooth = 0.8f;

    [Header("Trail (optional)")]
    public TrailRenderer trail;

    Rigidbody2D rb;
    Camera cam;
    Vector2 target, lastPos, velocity;

    void Awake()
    {
        cam = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;

        GetComponent<Collider2D>().isTrigger = true;

        lastPos = transform.position;
        target = lastPos;
        if (trail) trail.emitting = !needPressHeld;
    }

    void Update()
    {
        if (!followPointer || !cam) return;

        bool pressed = IsPressed();
        if (trail) trail.emitting = !needPressHeld || pressed;

        if (needPressHeld && !pressed) { target = transform.position; return; }

        Vector3 w = cam.ScreenToWorldPoint(PointerPosition());
        target = new Vector2(w.x, w.y);
    }

    void FixedUpdate()
    {
        Vector2 p = transform.position;
        velocity = (p - lastPos) / Time.fixedDeltaTime;

        // Tez swipe me bich ke objects na chhoot jayein
        if (velocity.magnitude >= minSliceSpeed)
        {
            var hits = Physics2D.LinecastAll(lastPos, p);
            foreach (var h in hits) TrySlice(h.collider, h.point);
        }
        lastPos = p;

        if (followPointer)
            rb.MovePosition(Vector2.Lerp(p, target, followSmooth));
    }

    void OnTriggerEnter2D(Collider2D other) { TrySlice(other, other.ClosestPoint(transform.position)); }
    void OnTriggerStay2D(Collider2D other)  { TrySlice(other, other.ClosestPoint(transform.position)); }

    void TrySlice(Collider2D other, Vector2 hitPoint)
    {
        if (velocity.magnitude < minSliceSpeed) return;

        var s = other.GetComponentInParent<Sliceable2D>();
        if (!s) return;

        s.Slice(hitPoint, velocity.normalized);
    }

    // ---- Input (legacy Input Manager aur naya Input System dono chalte hain) ----
    static bool IsPressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Pointer.current != null && Pointer.current.press.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }

    static Vector3 PointerPosition()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Pointer.current != null ? (Vector3)Pointer.current.position.ReadValue() : Vector3.zero;
#else
        return Input.mousePosition;
#endif
    }
}