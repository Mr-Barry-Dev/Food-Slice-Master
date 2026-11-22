// // using UnityEngine;

// // /// <summary>
// // /// 2D Fruit script:
// // /// 1) Fruit ka head (upar) hamesha upar rahega. Parent ghumne se jhukega to
// // ///    smoothly jhulke wapas upar aa jayega, aur kabhi niche nahi jayega.
// // /// 2) Knife lagte hi: colliders OFF -> original fruit destroy -> 2 slice (left/right)
// // ///    alag hoke niche girte hain -> sound -> destroyDelay baad slices destroy.
// // ///
// // /// Setup:
// // /// - Fruit par Rigidbody2D + Collider2D lagao (Rigidbody2D na ho to script khud add kar degi).
// // /// - Knife ka Tag "Knife" rakho aur knife par bhi Collider2D lagao.
// // /// - Inspector me leftSliceSprite aur rightSliceSprite me fruit ke 2 half sprites drag karo.
// // /// </summary>
// // public class Fruit : MonoBehaviour
// // {
// //     [Header("Head Up Settings")]
// //     [Tooltip("Head seedhe (upar) se zyada se zyada kitna jhuk sakta hai (degrees).")]
// //     [Range(0f, 90f)] public float maxTiltAngle = 45f;

// //     [Tooltip("Head kitni tezi se wapas upar aayega. Zyada = jaldi seedha, kam = zyada jhulega.")]
// //     public float returnSpeed = 10f;

// //     [Header("Knife Settings")]
// //     public string knifeTag = "Knife";

// //     [Header("Slice Images")]
// //     [Tooltip("Fruit ka left half sprite")]
// //     public Sprite leftSliceSprite;
// //     [Tooltip("Fruit ka right half sprite")]
// //     public Sprite rightSliceSprite;

// //     [Header("Slice Motion")]
// //     [Tooltip("Dono half ek dusre se kitna bahar ki taraf udenge (side speed).")]
// //     public float sliceSideForce = 1.5f;

// //     [Tooltip("Cut ke time halka sa upar ka jhatka (0 = seedha girenge).")]
// //     public float sliceUpForce = 1f;

// //     [Tooltip("Slice kitni tezi se ghumenge (degrees/sec).")]
// //     public float sliceSpinSpeed = 120f;

// //     [Tooltip("Slice par gravity kitni lage. Zyada = jaldi niche girenge.")]
// //     public float sliceGravityScale = 2f;

// //     [Tooltip("Agar dono half sprites ke pivot apne apne center me hain to thoda offset do " +
// //              "taaki cut ke time dono jud ke ek fruit jaise dikhein. Same pivot ho to 0 rakho.")]
// //     public float sliceStartOffset = 0f;

// //     [Header("Cut Sound")]
// //     public AudioClip cutSound;
// //     [Range(0f, 1f)] public float soundVolume = 1f;

// //     [Tooltip("Cut hone ke baad slices kitni der baad destroy hon")]
// //     public float destroyDelay = 2f;

// //     private Rigidbody2D rb;
// //     private bool isCut = false;

// //     private void Awake()
// //     {
// //         EnsureRigidbody();
// //     }

// //     private bool EnsureRigidbody()
// //     {
// //         if (rb != null) return true;

// //         rb = GetComponent<Rigidbody2D>();
// //         if (rb == null) rb = GetComponentInParent<Rigidbody2D>();

// //         if (rb == null)
// //         {
// //             Debug.LogWarning($"{name}: Rigidbody2D nahi mila, automatically add kar raha hu. Prefab par manually add kar lo.");
// //             rb = gameObject.AddComponent<Rigidbody2D>();
// //         }

// //         return rb != null;
// //     }

// //     // Parent (shape rotation) apna kaam kar le, uske baad head ko seedha karte hain
// //     private void LateUpdate()
// //     {
// //         if (isCut) return;
// //         KeepHeadUp();
// //     }

// //     /// <summary>
// //     /// World rotation (Z) ko 0 (head upar) ki taraf smoothly wapas lata hai
// //     /// aur -maxTiltAngle..+maxTiltAngle ke andar clamp rakhta hai.
// //     /// </summary>
// //     private void KeepHeadUp()
// //     {
// //         // -180..180 range me current world angle (0 = seedha upar)
// //         float angle = Mathf.DeltaAngle(0f, transform.eulerAngles.z);

// //         // Smoothly wapas upar
// //         angle = Mathf.Lerp(angle, 0f, 1f - Mathf.Exp(-returnSpeed * Time.deltaTime));

// //         // Kabhi limit se zyada na jhuke
// //         angle = Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle);

// //         transform.rotation = Quaternion.Euler(0f, 0f, angle);
// //     }

// //     private void OnTriggerEnter2D(Collider2D other)
// //     {
// //         if (other.CompareTag(knifeTag))
// //             Cut();
// //     }

// //     private void OnCollisionEnter2D(Collision2D collision)
// //     {
// //         if (collision.collider.CompareTag(knifeTag))
// //             Cut();
// //     }

// //     public void Cut()
// //     {
// //         if (isCut) return; // ek hi baar chale
// //         isCut = true;
// //         EnsureRigidbody();

// //         // Original fruit ka sprite info (slice banane ke liye) hide/destroy se pehle le lo
// //         SpriteRenderer mainSR = GetComponentInChildren<SpriteRenderer>();

// //         // 1) Pehle saare colliders off
// //         foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
// //             col.enabled = false;

// //         // 2) Original fruit dikhna band
// //         foreach (Renderer r in GetComponentsInChildren<Renderer>())
// //             r.enabled = false;

// //         // Physics rok do
// //         if (rb != null)
// //         {
// //             rb.linearVelocity = Vector2.zero;
// //             rb.angularVelocity = 0f;
// //             rb.simulated = false;
// //         }

// //         // 3) 2 slice spawn karo (left aur right) jo niche girenge
// //         SpawnSlice(leftSliceSprite, -1f, mainSR);
// //         SpawnSlice(rightSliceSprite, 1f, mainSR);

// //         // 4) Sound play (Settings me Sound OFF ho to nahi bajega)
// //         if (cutSound != null && GameAudio.SoundEnabled)
// //             AudioSource.PlayClipAtPoint(cutSound, transform.position, soundVolume);

// //         // 5) Original fruit turant destroy (slices alag objects hain, apne aap destroy honge)
// //         Destroy(gameObject);
// //     }

// //     /// <summary>
// //     /// Ek half slice banata hai. direction: -1 = left, +1 = right
// //     /// </summary>
// //     private void SpawnSlice(Sprite sprite, float direction, SpriteRenderer reference)
// //     {
// //         if (sprite == null) return;

// //         GameObject slice = new GameObject(name + (direction < 0 ? "_SliceLeft" : "_SliceRight"));

// //         // Fruit ki position / rotation / size same rakho
// //         slice.transform.position = transform.position + transform.right * (direction * sliceStartOffset);
// //         slice.transform.rotation = transform.rotation;
// //         slice.transform.localScale = transform.lossyScale;

// //         // Sprite
// //         SpriteRenderer sr = slice.AddComponent<SpriteRenderer>();
// //         sr.sprite = sprite;
// //         if (reference != null)
// //         {
// //             sr.sortingLayerID = reference.sortingLayerID;
// //             sr.sortingOrder = reference.sortingOrder;
// //             sr.color = reference.color;
// //         }

// //         // Physics: collider nahi hai isliye kisi cheez se takrayenge nahi, bas girenge
// //         Rigidbody2D srb = slice.AddComponent<Rigidbody2D>();
// //         srb.gravityScale = sliceGravityScale;
// //         srb.linearVelocity = new Vector2(direction * sliceSideForce, sliceUpForce);
// //         srb.angularVelocity = -direction * sliceSpinSpeed;

// //         Destroy(slice, destroyDelay);
// //     }
// // }



// using UnityEngine;

// /// <summary>
// /// 2D Fruit script:
// /// 1) Fruit ka head (upar) hamesha upar rahega. Parent ghumne se jhukega to
// ///    smoothly jhulke wapas upar aa jayega, aur kabhi niche nahi jayega.
// /// 2) Knife lagte hi: colliders OFF -> original fruit destroy -> 2 slice (left/right)
// ///    alag hoke niche girte hain -> sound -> destroyDelay baad slices destroy.
// ///
// /// Setup:
// /// - Fruit par Rigidbody2D + Collider2D lagao (Rigidbody2D na ho to script khud add kar degi).
// /// - Knife ka Tag "Knife" rakho aur knife par bhi Collider2D lagao.
// /// - Inspector me leftSliceSprite aur rightSliceSprite me fruit ke 2 half sprites drag karo.
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

//     [Header("Slice Images")]
//     [Tooltip("Fruit ka left half sprite")]
//     public Sprite leftSliceSprite;
//     [Tooltip("Fruit ka right half sprite")]
//     public Sprite rightSliceSprite;

//     [Header("Slice Motion")]
//     [Tooltip("Dono half ek dusre se kitna bahar ki taraf udenge (side speed).")]
//     public float sliceSideForce = 1.5f;

//     [Tooltip("Cut ke time halka sa upar ka jhatka (0 = seedha girenge).")]
//     public float sliceUpForce = 1f;

//     [Tooltip("Slice kitni tezi se ghumenge (degrees/sec).")]
//     public float sliceSpinSpeed = 120f;

//     [Tooltip("Slice par gravity kitni lage. Zyada = jaldi niche girenge.")]
//     public float sliceGravityScale = 2f;

//     [Tooltip("Agar dono half sprites ke pivot apne apne center me hain to thoda offset do " +
//              "taaki cut ke time dono jud ke ek fruit jaise dikhein. Same pivot ho to 0 rakho.")]
//     public float sliceStartOffset = 0f;

//     [Header("Juice Dots Effect")]
//     [Tooltip("Cut hone par juice ke dots nikle ya nahi.")]
//     public bool enableJuice = true;

//     [Tooltip("Juice ka color (splash aur dots dono ka). Alpha bhi isi color se set hoga.")]
//     public Color juiceColor = new Color(1f, 0.35f, 0.1f, 0.9f);

//     [Header("Juice Splash (andar se phailta hua juice)")]
//     [Tooltip("Cut ki jagah par bade juice blobs jo andar se phailte hain. OFF = sirf dots.")]
//     public bool enableSplash = true;

//     [Tooltip("Kitne blobs phailenge (min, max).")]
//     public Vector2Int splashBlobCount = new Vector2Int(4, 6);

//     [Tooltip("Blob ka final size (min, max). World units me.")]
//     public Vector2 splashBlobSize = new Vector2(0.5f, 1.1f);

//     [Tooltip("Blobs kitni der me poora phailenge (seconds). Kam = jhatke se, zyada = dheere.")]
//     public float splashSpreadTime = 0.25f;

//     [Tooltip("Phailne ke baad blobs kitni der me fade honge (seconds).")]
//     public float splashFadeTime = 0.7f;

//     [Tooltip("Blobs cut ki jagah se kitni door tak bikhrenge.")]
//     public float splashScatter = 0.3f;

//     [Header("Juice Dots (chote-bade chheente)")]
//     [Tooltip("Dots bahar udte hue kitna slow hote hain. Zyada = pehle tez bahar, phir jaldi ruk ke girte hain.")]
//     public float juiceDrag = 2f;

//     [Tooltip("Kitne dots nikalenge (min, max).")]
//     public Vector2Int juiceDotCount = new Vector2Int(12, 20);

//     [Tooltip("Dot ka size (min = chota, max = bada). World units me.")]
//     public Vector2 juiceDotSize = new Vector2(0.05f, 0.25f);

//     [Tooltip("Dots kitni tezi se bahar udenge (min, max).")]
//     public Vector2 juiceSpeed = new Vector2(1.5f, 5f);

//     [Tooltip("Dots par gravity kitni lage.")]
//     public float juiceGravityScale = 1.5f;

//     [Tooltip("Dots kitni der me gayab honge (min, max seconds).")]
//     public Vector2 juiceLifetime = new Vector2(0.6f, 1.2f);

//     [Header("Cut Sound")]
//     public AudioClip cutSound;
//     [Range(0f, 1f)] public float soundVolume = 1f;

//     [Tooltip("Cut hone ke baad slices kitni der baad destroy hon")]
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

//         // Original fruit ka sprite info (slice banane ke liye) hide/destroy se pehle le lo
//         SpriteRenderer mainSR = GetComponentInChildren<SpriteRenderer>();

//         // 1) Pehle saare colliders off
//         foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
//             col.enabled = false;

//         // 2) Original fruit dikhna band
//         foreach (Renderer r in GetComponentsInChildren<Renderer>())
//             r.enabled = false;

//         // Physics rok do
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector2.zero;
//             rb.angularVelocity = 0f;
//             rb.simulated = false;
//         }

//         // 3) 2 slice spawn karo (left aur right) jo niche girenge
//         SpawnSlice(leftSliceSprite, -1f, mainSR);
//         SpawnSlice(rightSliceSprite, 1f, mainSR);

//         // 3.5) Juice ke chote-bade dots
//         if (enableJuice)
//             SpawnJuice(mainSR);

//         // 4) Sound play (Settings me Sound OFF ho to nahi bajega)
//         if (cutSound != null && GameAudio.SoundEnabled)
//             AudioSource.PlayClipAtPoint(cutSound, transform.position, soundVolume);

//         // 5) Original fruit turant destroy (slices alag objects hain, apne aap destroy honge)
//         Destroy(gameObject);
//     }

//     private static Sprite dotSprite;

//     // Runtime me gol (circle) sprite banata hai, alag image ki zarurat nahi
//     private static Sprite GetDotSprite()
//     {
//         if (dotSprite != null) return dotSprite;

//         const int size = 32;
//         Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
//         tex.filterMode = FilterMode.Bilinear;
//         float r = size / 2f;

//         for (int y = 0; y < size; y++)
//         {
//             for (int x = 0; x < size; x++)
//             {
//                 float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
//                 float a = Mathf.Clamp01(r - d); // soft edge
//                 tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
//             }
//         }
//         tex.Apply();

//         dotSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
//         return dotSprite;
//     }

//     /// <summary>
//     /// Cut ki jagah se juice ke chote-bade dots chaaron taraf udte hain,
//     /// thoda niche girte hain aur fade hoke gayab ho jate hain.
//     /// </summary>
//     private void SpawnJuice(SpriteRenderer reference)
//     {
//         int count = Random.Range(juiceDotCount.x, juiceDotCount.y + 1);

//         // Color (aur alpha) inspector se
//         Color dotColor = juiceColor;

//         // Pehle andar se phailta hua juice (blobs), phir upar se dots
//         if (enableSplash)
//             SpawnSplash(reference);

//         for (int i = 0; i < count; i++)
//         {
//             GameObject dot = new GameObject("JuiceDot");
//             dot.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * 0.1f);

//             float size = Random.Range(juiceDotSize.x, juiceDotSize.y);
//             dot.transform.localScale = Vector3.one * size;

//             SpriteRenderer sr = dot.AddComponent<SpriteRenderer>();
//             sr.sprite = GetDotSprite();
//             sr.color = dotColor;
//             if (reference != null)
//             {
//                 sr.sortingLayerID = reference.sortingLayerID;
//                 sr.sortingOrder = reference.sortingOrder + 1; // slices ke upar dikhe
//             }

//             Rigidbody2D drb = dot.AddComponent<Rigidbody2D>();
//             drb.gravityScale = juiceGravityScale;
//             drb.linearDamping = juiceDrag;

//             // Random direction me bahar (chaaron taraf)
//             Vector2 dir = Random.insideUnitCircle.normalized;
//             drb.linearVelocity = dir * Random.Range(juiceSpeed.x, juiceSpeed.y);

//             float life = Random.Range(juiceLifetime.x, juiceLifetime.y);
//             JuiceDot fade = dot.AddComponent<JuiceDot>();
//             fade.lifetime = life;
//             fade.startScale = size;
//             fade.startAlpha = dotColor.a;
//             fade.sr = sr;
//         }
//     }

//     /// <summary>
//     /// Cut ki jagah par bade juice blobs: chhote se shuru hoke tezi se phailte hain
//     /// (jaise fruit ke andar se juice bahar bikhar raha ho), phir fade ho jate hain.
//     /// Slices ke peeche rehte hain, to slices hatne par juice ka phaila hua dhabba dikhta hai.
//     /// </summary>
//     private void SpawnSplash(SpriteRenderer reference)
//     {
//         int count = Random.Range(splashBlobCount.x, splashBlobCount.y + 1);

//         for (int i = 0; i < count; i++)
//         {
//             GameObject blob = new GameObject("JuiceSplash");
//             blob.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * splashScatter);

//             SpriteRenderer sr = blob.AddComponent<SpriteRenderer>();
//             sr.sprite = GetDotSprite();
//             sr.color = juiceColor;
//             if (reference != null)
//             {
//                 sr.sortingLayerID = reference.sortingLayerID;
//                 sr.sortingOrder = reference.sortingOrder - 1; // slices ke peeche
//             }

//             JuiceSplat splat = blob.AddComponent<JuiceSplat>();
//             splat.sr = sr;
//             splat.endScale = Random.Range(splashBlobSize.x, splashBlobSize.y);
//             splat.spreadTime = splashSpreadTime;
//             splat.fadeTime = splashFadeTime;
//             splat.startAlpha = juiceColor.a;
//         }
//     }

//     /// <summary>
//     /// Ek half slice banata hai. direction: -1 = left, +1 = right
//     /// </summary>
//     private void SpawnSlice(Sprite sprite, float direction, SpriteRenderer reference)
//     {
//         if (sprite == null) return;

//         GameObject slice = new GameObject(name + (direction < 0 ? "_SliceLeft" : "_SliceRight"));

//         // Fruit ki position / rotation / size same rakho
//         slice.transform.position = transform.position + transform.right * (direction * sliceStartOffset);
//         slice.transform.rotation = transform.rotation;
//         slice.transform.localScale = transform.lossyScale;

//         // Sprite
//         SpriteRenderer sr = slice.AddComponent<SpriteRenderer>();
//         sr.sprite = sprite;
//         if (reference != null)
//         {
//             sr.sortingLayerID = reference.sortingLayerID;
//             sr.sortingOrder = reference.sortingOrder;
//             sr.color = reference.color;
//         }

//         // Physics: collider nahi hai isliye kisi cheez se takrayenge nahi, bas girenge
//         Rigidbody2D srb = slice.AddComponent<Rigidbody2D>();
//         srb.gravityScale = sliceGravityScale;
//         srb.linearVelocity = new Vector2(direction * sliceSideForce, sliceUpForce);
//         srb.angularVelocity = -direction * sliceSpinSpeed;

//         Destroy(slice, destroyDelay);
//     }
// }

// /// <summary>
// /// Juice dot ko dheere dheere chota + transparent karke destroy karta hai.
// /// (Fruit.cs ke andar hi rakha hai, alag file ki zarurat nahi.)
// /// </summary>
// public class JuiceDot : MonoBehaviour
// {
//     [HideInInspector] public float lifetime = 1f;
//     [HideInInspector] public float startScale = 0.1f;
//     [HideInInspector] public float startAlpha = 1f;
//     [HideInInspector] public SpriteRenderer sr;

//     private float timer;

//     private void Update()
//     {
//         timer += Time.deltaTime;
//         float t = Mathf.Clamp01(timer / lifetime);

//         transform.localScale = Vector3.one * Mathf.Lerp(startScale, startScale * 0.3f, t);

//         if (sr != null)
//         {
//             Color c = sr.color;
//             c.a = startAlpha * (1f - t);
//             sr.color = c;
//         }

//         if (t >= 1f) Destroy(gameObject);
//     }
// }

// /// <summary>
// /// Juice blob ko chhote se bade tak tezi se phailata hai (ease-out),
// /// phir fade karke destroy karta hai.
// /// </summary>
// public class JuiceSplat : MonoBehaviour
// {
//     [HideInInspector] public SpriteRenderer sr;
//     [HideInInspector] public float endScale = 0.8f;
//     [HideInInspector] public float spreadTime = 0.25f;
//     [HideInInspector] public float fadeTime = 0.7f;
//     [HideInInspector] public float startAlpha = 1f;

//     private float timer;

//     private void Start()
//     {
//         transform.localScale = Vector3.one * (endScale * 0.1f);
//     }

//     private void Update()
//     {
//         timer += Time.deltaTime;

//         // Phailna: jhatke se shuru, dheere dheere ruk jata hai
//         float s = Mathf.Clamp01(timer / Mathf.Max(0.01f, spreadTime));
//         s = 1f - (1f - s) * (1f - s);
//         transform.localScale = Vector3.one * Mathf.Lerp(endScale * 0.1f, endScale, s);

//         // Fade: phailne ke baad shuru hota hai
//         float f = Mathf.Clamp01((timer - spreadTime) / Mathf.Max(0.01f, fadeTime));
//         if (sr != null)
//         {
//             Color c = sr.color;
//             c.a = startAlpha * (1f - f);
//             sr.color = c;
//         }

//         if (f >= 1f) Destroy(gameObject);
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

    [Header("Juice Dots Effect")]
    [Tooltip("Cut hone par juice ke dots nikle ya nahi.")]
    public bool enableJuice = true;

    [Tooltip("Juice dots ka color. Alpha bhi isi color se set hoga.")]
    public Color juiceColor = new Color(1f, 0.35f, 0.1f, 0.9f);

    [Tooltip("Dots fruit ke andar kitne area me se nikalenge (fruit ke center se radius, world units me). " +
             "Zyada = dots poore fruit ke andar se bikhre hue nikalenge, 0 = ek hi point se.")]
    public float juiceSpawnRadius = 0.3f;

    [Tooltip("Dots bahar udte hue kitna slow hote hain. Zyada = pehle tez bahar, phir jaldi ruk ke girte hain.")]
    public float juiceDrag = 2f;

    [Tooltip("Kitne dots nikalenge (min, max).")]
    public Vector2Int juiceDotCount = new Vector2Int(25, 40);

    [Tooltip("Dot ka size (min = chota, max = bada). World units me. Zyada dots chote honge, kuch hi bade.")]
    public Vector2 juiceDotSize = new Vector2(0.04f, 0.22f);

    [Tooltip("Dots kitni tezi se bahar udenge (min, max).")]
    public Vector2 juiceSpeed = new Vector2(1.5f, 5f);

    [Tooltip("Dots par gravity kitni lage.")]
    public float juiceGravityScale = 1.5f;

    [Tooltip("Dots kitni der me gayab honge (min, max seconds).")]
    public Vector2 juiceLifetime = new Vector2(0.6f, 1.2f);

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

        // 3.5) Juice ke chote-bade dots
        if (enableJuice)
            SpawnJuice(mainSR);

        // 4) Sound play (Settings me Sound OFF ho to nahi bajega)
        if (cutSound != null && GameAudio.SoundEnabled)
            AudioSource.PlayClipAtPoint(cutSound, transform.position, soundVolume);

        // 5) Original fruit turant destroy (slices alag objects hain, apne aap destroy honge)
        Destroy(gameObject);
    }

    private static Sprite dotSprite;

    // Runtime me gol (circle) sprite banata hai, alag image ki zarurat nahi
    private static Sprite GetDotSprite()
    {
        if (dotSprite != null) return dotSprite;

        const int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float r = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                float a = Mathf.Clamp01(r - d); // soft edge
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();

        dotSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return dotSprite;
    }

    /// <summary>
    /// Cut ki jagah se juice ke chote-bade dots chaaron taraf udte hain,
    /// thoda niche girte hain aur fade hoke gayab ho jate hain.
    /// </summary>
    private void SpawnJuice(SpriteRenderer reference)
    {
        int count = Random.Range(juiceDotCount.x, juiceDotCount.y + 1);

        // Color (aur alpha) inspector se
        Color dotColor = juiceColor;

        for (int i = 0; i < count; i++)
        {
            GameObject dot = new GameObject("JuiceDot");

            // Dot fruit ke andar kisi random jagah se shuru hota hai
            Vector2 offset = Random.insideUnitCircle * juiceSpawnRadius;
            dot.transform.position = transform.position + (Vector3)offset;

            // Zyada dots chote, kuch hi bade (random^2 se chote ki taraf jhukav)
            float size = Mathf.Lerp(juiceDotSize.x, juiceDotSize.y, Mathf.Pow(Random.value, 2f));
            dot.transform.localScale = Vector3.one * size;

            SpriteRenderer sr = dot.AddComponent<SpriteRenderer>();
            sr.sprite = GetDotSprite();
            sr.color = dotColor;
            if (reference != null)
            {
                sr.sortingLayerID = reference.sortingLayerID;
                sr.sortingOrder = reference.sortingOrder + 1; // slices ke upar dikhe
            }

            Rigidbody2D drb = dot.AddComponent<Rigidbody2D>();
            drb.gravityScale = juiceGravityScale;
            drb.linearDamping = juiceDrag;

            // Fruit ke center se bahar ki taraf (andar se phailta hua), thoda random jhukav ke saath
            Vector2 dir = offset.sqrMagnitude > 0.0001f ? offset.normalized : Random.insideUnitCircle.normalized;
            dir = (dir + Random.insideUnitCircle * 0.5f).normalized;
            drb.linearVelocity = dir * Random.Range(juiceSpeed.x, juiceSpeed.y);

            float life = Random.Range(juiceLifetime.x, juiceLifetime.y);
            JuiceDot fade = dot.AddComponent<JuiceDot>();
            fade.lifetime = life;
            fade.startScale = size;
            fade.startAlpha = dotColor.a;
            fade.sr = sr;
        }
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

/// <summary>
/// Juice dot ko dheere dheere chota + transparent karke destroy karta hai.
/// (Fruit.cs ke andar hi rakha hai, alag file ki zarurat nahi.)
/// </summary>
public class JuiceDot : MonoBehaviour
{
    [HideInInspector] public float lifetime = 1f;
    [HideInInspector] public float startScale = 0.1f;
    [HideInInspector] public float startAlpha = 1f;
    [HideInInspector] public SpriteRenderer sr;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifetime);

        transform.localScale = Vector3.one * Mathf.Lerp(startScale, startScale * 0.3f, t);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = startAlpha * (1f - t);
            sr.color = c;
        }

        if (t >= 1f) Destroy(gameObject);
    }
}