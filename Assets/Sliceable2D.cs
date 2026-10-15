using UnityEngine;

public enum SliceType2D { Fruit, Liquid }

/// <summary>
/// 2D object (mango, apple, juice bottle...) par lagao. Collider2D zaroori hai.
/// Type = Fruit  -> 2 halves me cut hoke neeche girta hai.
/// Type = Liquid -> juice splash + wall par juice splat (aur drip).
/// </summary>
public class Sliceable2D : MonoBehaviour
{
    [Header("Type")]
    public SliceType2D type = SliceType2D.Fruit;

    [Header("FRUIT - 2 slice")]
    [Tooltip("Fruit ka RIGHT wala aadha hissa (Rigidbody2D + Collider2D ke saath prefab)")]
    public GameObject halfA;
    [Tooltip("Fruit ka LEFT wala aadha hissa")]
    public GameObject halfB;
    [Tooltip("Cut par juice spray (optional particle prefab)")]
    public GameObject fruitJuiceFX;
    [Tooltip("Halves ko knife ke cut ki direction me ghumao")]
    public bool alignHalvesToCut = true;
    public float splitForce = 2.5f;
    public float spinSpeed = 180f;          // degrees/sec
    public float halvesLifetime = 4f;

    [Header("LIQUID - juice")]
    public Color liquidColor = new Color(1f, 0.55f, 0f);
    [Tooltip("Hit point par juice particle burst (optional)")]
    public GameObject juiceSplashFX;
    [Tooltip("Wall par chipakne wala splat sprite prefab (optional WallDrip2D ke saath)")]
    public GameObject wallSplatPrefab;
    [Tooltip("Wall/background object ka Tag. Us par Collider2D hona chahiye. Khali chhodo to splat hit point par hi banega.")]
    public string wallTag = "Wall";
    public Vector2 splatScaleRange = new Vector2(0.8f, 1.4f);
    [Tooltip("Splat ko wall ke sprites ke upar, par bottle ke peeche dikhane ke liye")]
    public int splatSortingOrder = -5;
    [Tooltip("Bottle toot kar jo tukde bane (optional)")]
    public GameObject brokenContainerPrefab;

    [Header("Common")]
    public AudioClip sliceSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    bool sliced;

    public void Slice(Vector2 hitPoint, Vector2 knifeDirection)
    {
        if (sliced) return;
        sliced = true;

        if (sliceSound) AudioSource.PlayClipAtPoint(sliceSound, hitPoint, soundVolume);

        if (type == SliceType2D.Fruit) SliceFruit(hitPoint, knifeDirection);
        else SliceLiquid(hitPoint);

        Destroy(gameObject);
    }

    // ---------------- FRUIT ----------------
    void SliceFruit(Vector2 hitPoint, Vector2 knifeDir)
    {
        if (knifeDir.sqrMagnitude < 0.0001f) knifeDir = Vector2.right;
        knifeDir.Normalize();

        // cut line knife ki direction me hai, normal uske perpendicular
        Vector2 normal = new Vector2(-knifeDir.y, knifeDir.x);

        Quaternion rot = transform.rotation;
        if (alignHalvesToCut)
        {
            float cur = Mathf.Atan2(transform.right.y, transform.right.x);
            float want = Mathf.Atan2(normal.y, normal.x);
            rot = Quaternion.Euler(0f, 0f, (want - cur) * Mathf.Rad2Deg) * rot;
        }

        Vector2 inherited = Vector2.zero;
        var myRb = GetComponent<Rigidbody2D>();
        if (myRb) inherited = GetVelocity(myRb);

        SpawnHalf(halfA, normal, rot, inherited);
        SpawnHalf(halfB, -normal, rot, inherited);

        if (fruitJuiceFX)
        {
            float a = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            var fx = Instantiate(fruitJuiceFX, hitPoint, Quaternion.Euler(0f, 0f, a));
            Destroy(fx, 3f);
        }
    }

    void SpawnHalf(GameObject prefab, Vector2 dir, Quaternion rot, Vector2 inherited)
    {
        if (!prefab) return;
        var h = Instantiate(prefab, (Vector2)transform.position + dir * 0.02f, rot);
        h.transform.localScale = transform.localScale;

        var rb = h.GetComponent<Rigidbody2D>();
        if (!rb) rb = h.AddComponent<Rigidbody2D>();
        SetVelocity(rb, inherited + dir * splitForce);
        rb.angularVelocity = Random.Range(-spinSpeed, spinSpeed);

        Destroy(h, halvesLifetime);
    }

    // ---------------- LIQUID ----------------
    void SliceLiquid(Vector2 hitPoint)
    {
        if (juiceSplashFX)
        {
            var fx = Instantiate(juiceSplashFX, hitPoint, Quaternion.identity);
            foreach (var ps in fx.GetComponentsInChildren<ParticleSystem>())
            {
                var main = ps.main;
                main.startColor = liquidColor;
            }
            Destroy(fx, 4f);
        }

        if (wallSplatPrefab)
        {
            Vector2 p = hitPoint;

            // Wall mila to splat hamesha wall ke andar hi banega
            if (!string.IsNullOrEmpty(wallTag))
            {
                GameObject wall = null;
                try { wall = GameObject.FindWithTag(wallTag); } catch { /* tag defined nahi hai */ }
                if (wall)
                {
                    var wc = wall.GetComponent<Collider2D>();
                    if (wc && !wc.OverlapPoint(p)) p = wc.ClosestPoint(p);
                }
            }

            var splat = Instantiate(wallSplatPrefab, new Vector3(p.x, p.y, transform.position.z),
                                    Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            splat.transform.localScale *= Random.Range(splatScaleRange.x, splatScaleRange.y);

            foreach (var sr in splat.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = liquidColor;
                sr.sortingOrder = splatSortingOrder;
            }
        }

        if (brokenContainerPrefab)
            Instantiate(brokenContainerPrefab, transform.position, transform.rotation);
    }

    // Unity 6 (linearVelocity) aur purane Unity (velocity) dono ke liye
    static Vector2 GetVelocity(Rigidbody2D rb)
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    static void SetVelocity(Rigidbody2D rb, Vector2 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }
}