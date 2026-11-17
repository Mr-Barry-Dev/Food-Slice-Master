// using UnityEngine;

// /// <summary>
// /// 2D Fruit script:
// /// 1) Fruit ka head (upar) hamesha upar rahega. Parent ghumne se jhukega to
// ///    smoothly jhulke wapas upar aa jayega, aur kabhi niche nahi jayega.
// /// 2) Knife lagte hi: colliders OFF -> fruit visible band -> effect + sound -> destroyDelay baad destroy.
// ///
// /// Setup:
// /// - Fruit par Rigidbody2D + Collider2D lagao (Rigidbody2D na ho to script khud add kar degi).
// /// - Knife ka Tag "Knife" rakho aur knife par bhi Collider2D lagao.
// /// </summary>
// public class Fruit : MonoBehaviour
// {
//     [Header("Head Up Settings")]
//     [Tooltip("Head seedhe (upar) se zyada se zyada kitna jhuk sakta hai (degrees).")]
//     [Range(0f, 90f)] public float maxTiltAngle = 45f;

//     [Tooltip("Head kitni tezi se wapas upar aayega. Zyada = jaldi seedha, kam = zyada jhulega.")]
//     public float returnSpeed = 10f;

//     [Header("Knife Settings")]
//     public string knifeTag = "Knife";

//     [Header("Cut Effects")]
//     [Tooltip("Particle / splash effect prefab")]
//     public GameObject cutEffectPrefab;
//     public AudioClip cutSound;
//     [Range(0f, 1f)] public float soundVolume = 1f;

//     [Tooltip("Cut hone ke baad kitni der baad fruit destroy ho")]
//     public float destroyDelay = 2f;

//     private Rigidbody2D rb;
//     private bool isCut = false;

//     private void Awake()
//     {
//         EnsureRigidbody();
//     }

//     private bool EnsureRigidbody()
//     {
//         if (rb != null) return true;

//         rb = GetComponent<Rigidbody2D>();
//         if (rb == null) rb = GetComponentInParent<Rigidbody2D>();

//         if (rb == null)
//         {
//             Debug.LogWarning($"{name}: Rigidbody2D nahi mila, automatically add kar raha hu. Prefab par manually add kar lo.");
//             rb = gameObject.AddComponent<Rigidbody2D>();
//         }

//         return rb != null;
//     }

//     // Parent (shape rotation) apna kaam kar le, uske baad head ko seedha karte hain
//     private void LateUpdate()
//     {
//         if (isCut) return;
//         KeepHeadUp();
//     }

//     /// <summary>
//     /// World rotation (Z) ko 0 (head upar) ki taraf smoothly wapas lata hai
//     /// aur -maxTiltAngle..+maxTiltAngle ke andar clamp rakhta hai.
//     /// </summary>
//     private void KeepHeadUp()
//     {
//         // -180..180 range me current world angle (0 = seedha upar)
//         float angle = Mathf.DeltaAngle(0f, transform.eulerAngles.z);

//         // Smoothly wapas upar
//         angle = Mathf.Lerp(angle, 0f, 1f - Mathf.Exp(-returnSpeed * Time.deltaTime));

//         // Kabhi limit se zyada na jhuke
//         angle = Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle);

//         transform.rotation = Quaternion.Euler(0f, 0f, angle);
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag(knifeTag))
//             Cut();
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.collider.CompareTag(knifeTag))
//             Cut();
//     }

//     public void Cut()
//     {
//         if (isCut) return; // ek hi baar chale
//         isCut = true;
//         EnsureRigidbody();

//         // 1) Pehle saare colliders off
//         foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
//             col.enabled = false;

//         // 2) Fruit dikhna band
//         foreach (Renderer r in GetComponentsInChildren<Renderer>())
//             r.enabled = false;

//         // Physics rok do
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector2.zero;
//             rb.angularVelocity = 0f;
//             rb.simulated = false;
//         }

//         // 3) Effect play
//         if (cutEffectPrefab != null)
//         {
//             GameObject fx = Instantiate(cutEffectPrefab, transform.position, Quaternion.identity);
//             Destroy(fx, destroyDelay + 1f);
//         }

//         // 4) Sound play (Settings me Sound OFF ho to nahi bajega)
//         if (cutSound != null && GameAudio.SoundEnabled)
//             AudioSource.PlayClipAtPoint(cutSound, transform.position, soundVolume);

//         // 5) destroyDelay (2 sec) baad destroy
//         Destroy(gameObject, destroyDelay);
//     }
// }




using UnityEngine;

/// <summary>
/// 2D Fruit script:
/// 1) Fruit ka head (upar) hamesha upar rahega. Parent ghumne se jhukega to
///    smoothly jhulke wapas upar aa jayega, aur kabhi niche nahi jayega.
/// 2) Knife lagte hi: colliders OFF -> original fruit destroy -> 2 slice (left/right)
///    alag hoke niche girte hain -> sound -> destroyDelay baad slices destroy.
///
/// Setup:
/// - Fruit par Rigidbody2D + Collider2D lagao (Rigidbody2D na ho to script khud add kar degi).
/// - Knife ka Tag "Knife" rakho aur knife par bhi Collider2D lagao.
/// - Inspector me leftSliceSprite aur rightSliceSprite me fruit ke 2 half sprites drag karo.
/// </summary>
public class Fruit : MonoBehaviour
{
    [Header("Head Up Settings")]
    [Tooltip("Head seedhe (upar) se zyada se zyada kitna jhuk sakta hai (degrees).")]
    [Range(0f, 90f)] public float maxTiltAngle = 45f;

    [Tooltip("Head kitni tezi se wapas upar aayega. Zyada = jaldi seedha, kam = zyada jhulega.")]
    public float returnSpeed = 10f;

    [Header("Knife Settings")]
    public string knifeTag = "Knife";

    [Header("Slice Images")]
    [Tooltip("Fruit ka left half sprite")]
    public Sprite leftSliceSprite;
    [Tooltip("Fruit ka right half sprite")]
    public Sprite rightSliceSprite;

    [Header("Slice Motion")]
    [Tooltip("Dono half ek dusre se kitna bahar ki taraf udenge (side speed).")]
    public float sliceSideForce = 1.5f;

    [Tooltip("Cut ke time halka sa upar ka jhatka (0 = seedha girenge).")]
    public float sliceUpForce = 1f;

    [Tooltip("Slice kitni tezi se ghumenge (degrees/sec).")]
    public float sliceSpinSpeed = 120f;

    [Tooltip("Slice par gravity kitni lage. Zyada = jaldi niche girenge.")]
    public float sliceGravityScale = 2f;

    [Tooltip("Agar dono half sprites ke pivot apne apne center me hain to thoda offset do " +
             "taaki cut ke time dono jud ke ek fruit jaise dikhein. Same pivot ho to 0 rakho.")]
    public float sliceStartOffset = 0f;

    [Header("Cut Sound")]
    public AudioClip cutSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Tooltip("Cut hone ke baad slices kitni der baad destroy hon")]
    public float destroyDelay = 2f;

    private Rigidbody2D rb;
    private bool isCut = false;

    private void Awake()
    {
        EnsureRigidbody();
    }

    private bool EnsureRigidbody()
    {
        if (rb != null) return true;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogWarning($"{name}: Rigidbody2D nahi mila, automatically add kar raha hu. Prefab par manually add kar lo.");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        return rb != null;
    }

    // Parent (shape rotation) apna kaam kar le, uske baad head ko seedha karte hain
    private void LateUpdate()
    {
        if (isCut) return;
        KeepHeadUp();
    }

    /// <summary>
    /// World rotation (Z) ko 0 (head upar) ki taraf smoothly wapas lata hai
    /// aur -maxTiltAngle..+maxTiltAngle ke andar clamp rakhta hai.
    /// </summary>
    private void KeepHeadUp()
    {
        // -180..180 range me current world angle (0 = seedha upar)
        float angle = Mathf.DeltaAngle(0f, transform.eulerAngles.z);

        // Smoothly wapas upar
        angle = Mathf.Lerp(angle, 0f, 1f - Mathf.Exp(-returnSpeed * Time.deltaTime));

        // Kabhi limit se zyada na jhuke
        angle = Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(knifeTag))
            Cut();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(knifeTag))
            Cut();
    }

    public void Cut()
    {
        if (isCut) return; // ek hi baar chale
        isCut = true;
        EnsureRigidbody();

        // Original fruit ka sprite info (slice banane ke liye) hide/destroy se pehle le lo
        SpriteRenderer mainSR = GetComponentInChildren<SpriteRenderer>();

        // 1) Pehle saare colliders off
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // 2) Original fruit dikhna band
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Physics rok do
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // 3) 2 slice spawn karo (left aur right) jo niche girenge
        SpawnSlice(leftSliceSprite, -1f, mainSR);
        SpawnSlice(rightSliceSprite, 1f, mainSR);

        // 4) Sound play (Settings me Sound OFF ho to nahi bajega)
        if (cutSound != null && GameAudio.SoundEnabled)
            AudioSource.PlayClipAtPoint(cutSound, transform.position, soundVolume);

        // 5) Original fruit turant destroy (slices alag objects hain, apne aap destroy honge)
        Destroy(gameObject);
    }

    /// <summary>
    /// Ek half slice banata hai. direction: -1 = left, +1 = right
    /// </summary>
    private void SpawnSlice(Sprite sprite, float direction, SpriteRenderer reference)
    {
        if (sprite == null) return;

        GameObject slice = new GameObject(name + (direction < 0 ? "_SliceLeft" : "_SliceRight"));

        // Fruit ki position / rotation / size same rakho
        slice.transform.position = transform.position + transform.right * (direction * sliceStartOffset);
        slice.transform.rotation = transform.rotation;
        slice.transform.localScale = transform.lossyScale;

        // Sprite
        SpriteRenderer sr = slice.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        if (reference != null)
        {
            sr.sortingLayerID = reference.sortingLayerID;
            sr.sortingOrder = reference.sortingOrder;
            sr.color = reference.color;
        }

        // Physics: collider nahi hai isliye kisi cheez se takrayenge nahi, bas girenge
        Rigidbody2D srb = slice.AddComponent<Rigidbody2D>();
        srb.gravityScale = sliceGravityScale;
        srb.linearVelocity = new Vector2(direction * sliceSideForce, sliceUpForce);
        srb.angularVelocity = -direction * sliceSpinSpeed;

        Destroy(slice, destroyDelay);
    }
}