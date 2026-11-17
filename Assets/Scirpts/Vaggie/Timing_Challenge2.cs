// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;

// // ============================================================
// // BOOMERANG (sirf Timing_Challenge2 ke liye, isi file me)
// // Runtime par AddComponent se lagta hai, alag script ki zarurat nahi
// //
// // Flow: ready position par ruka rehta hai -> tap par tap ki taraf udta hai
// // (range tak) -> wapas ready position par aata hai. Raste me jo fruit
// // touch hota hai wo kat jata hai.
// // ============================================================

// public class TimingBoomerang : MonoBehaviour
// {
//     [HideInInspector] public float moveSpeed = 14f;
//     [HideInInspector] public float spinSpeed = 900f;
//     [HideInInspector] public float range = 8f;
//     [HideInInspector] public float readyMoveSpeed = 8f;
//     [HideInInspector] public float startOffset = 1.5f;
//     [HideInInspector] public Timing_Challenge2 gameManager;

//     private Vector3 readyPosition;
//     private Vector3 fireOrigin;
//     private Vector3 fireDirection;

//     private bool isFired = false;
//     private bool isReturning = false;
//     private bool isFinished = false;


//     public void InitializeReady(Vector3 position)
//     {
//         readyPosition = position;
//         isFired = false;
//         isReturning = false;
//         isFinished = false;

//         // Ready point ke neeche se start, phir upar ready position tak aayega
//         transform.position =
//             position + Vector3.down * startOffset;
//     }


//     // true = fire ho gaya, false = pehle se fire ho chuka tha
//     public bool Fire(Vector2 direction)
//     {
//         if (isFired || isFinished)
//             return false;

//         if (direction.sqrMagnitude < 0.0001f)
//         {
//             direction = Vector2.up;
//         }

//         fireDirection =
//             ((Vector3)direction).normalized;

//         fireOrigin = transform.position;

//         isFired = true;
//         isReturning = false;

//         return true;
//     }


//     private void Update()
//     {
//         if (isFinished)
//             return;


//         if (!isFired)
//         {
//             transform.position =
//                 Vector3.MoveTowards(
//                     transform.position,
//                     readyPosition,
//                     readyMoveSpeed * Time.deltaTime
//                 );

//             return;
//         }


//         transform.Rotate(
//             0f,
//             0f,
//             spinSpeed * Time.deltaTime
//         );


//         if (!isReturning)
//         {
//             // Tap ki direction me seedha jao, range tak
//             transform.position +=
//                 fireDirection * moveSpeed * Time.deltaTime;

//             float traveled =
//                 Vector3.Distance(
//                     fireOrigin,
//                     transform.position
//                 );

//             if (traveled >= range)
//             {
//                 isReturning = true;
//             }
//         }
//         else
//         {
//             // Wapas ready position par
//             transform.position =
//                 Vector3.MoveTowards(
//                     transform.position,
//                     readyPosition,
//                     moveSpeed * Time.deltaTime
//                 );

//             if (
//                 (transform.position - readyPosition)
//                     .sqrMagnitude < 0.0001f
//             )
//             {
//                 Finish();
//             }
//         }
//     }


//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!isFired || isFinished)
//             return;

//         if (gameManager == null)
//             return;


//         Fruit fruit =
//             other.GetComponentInParent<Fruit>();

//         GameObject target =
//             fruit != null
//                 ? fruit.gameObject
//                 : other.gameObject;

//         gameManager.HitFruit(target);
//     }


//     private void Finish()
//     {
//         if (isFinished)
//             return;

//         isFinished = true;

//         if (gameManager != null)
//         {
//             gameManager.BoomerangFinished(this);
//         }

//         Destroy(gameObject);
//     }
// }


// public class Timing_Challenge2 : MonoBehaviour
// {
//     // ============================================================
//     // UI
//     // ============================================================

//     [Header("===== UI =====")]

//     public TMP_Text levelText;

//     public Image progressImage;

//     [Tooltip("Countdown timer ka TMP Text.")]
//     public TMP_Text timerText;


//     // ============================================================
//     // TIMER
//     // ============================================================

//     [Header("===== TIMER =====")]

//     [Tooltip("Poora level clear karne ke liye total seconds. Ye khatam hone se pehle sab stages clear karne hain, warna Game Over.")]
//     public float levelTime = 30f;

//     [Tooltip("Countdown itne (ya kam) seconds par aate hi timer text red ho jayega.")]
//     public float redTimeThreshold = 5f;

//     [Tooltip("Timer text ka red color.")]
//     public Color timerWarningColor = Color.red;

//     [Tooltip("Revive par (agar time khatam ho chuka tha) itne seconds wapas milenge.")]
//     public float reviveExtraTime = 10f;


//     // ============================================================
//     // GAME MANAGER
//     // ============================================================

//     [Header("===== GAME STATE MANAGER =====")]

//     [Tooltip("Main Timing_Challenge which handles Win and Game Over.")]
//     public Timing_Challenge gameManager;


//     // ============================================================
//     // FRUITS
//     // ============================================================

//     [Header("===== FRUITS =====")]

//     [Tooltip("Optional. Assign karoge to spawn hue fruits is object ke andar (parent) aayenge. Position screen ke hisab se hi hogi.")]
//     public Transform fruitSpawnPoint;

//     [Tooltip("Fruit prefabs. Har baar inme se random ek fruit aayega.")]
//     public GameObject[] commonFruitPrefabs;

//     [Tooltip("Ek level me kitne stages hain. Har stage ke saare fruits kaatne par agla stage aata hai.")]
//     public int stagesPerLevel = 3;

//     [Tooltip("Pehle stage me ek saath kitne fruits aayenge.")]
//     public int startFruitsPerStage = 3;

//     [Tooltip("Har agle stage me fruits kitne badhenge.")]
//     public int fruitsIncreasePerStage = 1;

//     [Tooltip("Ek stage me ek saath maximum kitne fruits.")]
//     public int maxFruitsPerStage = 6;

//     [Tooltip("Stage clear hone ke baad agla stage kitni der baad aayega.")]
//     public float nextStageDelay = 0.8f;

//     [Tooltip("ON: ek level me fruits ka spawn pattern fixed rahega (retry/reload par wahi). OFF: har baar naya random.")]
//     public bool keepFruitPatternPerLevel = true;

//     [Tooltip("Fruit ki minimum speed.")]
//     public float fruitSpeedMin = 2.5f;

//     [Tooltip("Fruit ki maximum speed.")]
//     public float fruitSpeedMax = 4f;

//     [Tooltip("Fruit spawn hone ki minimum height (Y, world units).")]
//     public float spawnYMin = -1f;

//     [Tooltip("Fruit spawn hone ki maximum height (Y, world units).")]
//     public float spawnYMax = 3f;

//     [Tooltip("Screen ke left/right kinare se kitna andar fruit ghoomega aur wapas mudega (world units).")]
//     public float screenPadding = 0.7f;


//     // ============================================================
//     // BOOMERANG
//     // ============================================================

//     [Header("===== BOOMERANG =====")]

//     [FormerlySerializedAs("knifePrefab")]
//     [Tooltip("Boomerang prefab.")]
//     public GameObject boomerangPrefab;

//     [FormerlySerializedAs("knifeSpawnPoint")]
//     [Tooltip("Point jahan boomerang ready rehta hai aur jahan wapas aata hai.")]
//     public Transform boomerangSpawnPoint;

//     [FormerlySerializedAs("knifeMoveSpeed")]
//     [Tooltip("Boomerang ki udne ki speed.")]
//     public float boomerangSpeed = 14f;

//     [FormerlySerializedAs("knifeRotationSpeed")]
//     [Tooltip("Udte waqt boomerang ghoomne ki speed.")]
//     public float boomerangSpinSpeed = 900f;

//     [Tooltip("Tap ki direction me boomerang kitni door tak jayega, phir wapas aayega.")]
//     public float boomerangRange = 8f;

//     [FormerlySerializedAs("knifeStartOffset")]
//     [Tooltip("Ready point ke kitna neeche se boomerang start hoga.")]
//     public float boomerangStartOffset = 1.5f;

//     [FormerlySerializedAs("knifeReadyMoveSpeed")]
//     [Tooltip("Ready position tak aane ki speed.")]
//     public float boomerangReadyMoveSpeed = 8f;

//     [FormerlySerializedAs("oneKnifeAtATime")]
//     [Tooltip("Ek time par sirf ek boomerang.")]
//     public bool oneBoomerangAtATime = true;

//     [Tooltip("ON: jab screen par koi fruit na ho tab tap se fire nahi hoga (khali jaane se game over na ho).")]
//     public bool fireOnlyWhenFruitPresent = true;


//     // ============================================================
//     // SOUND
//     // ============================================================

//     [Header("===== SOUND =====")]

//     [FormerlySerializedAs("knifeFireClip")]
//     [Tooltip("Boomerang fire hone par ye sound bajega.")]
//     public AudioClip fireClip;

//     [FormerlySerializedAs("knifeFireVolume")]
//     [Range(0f, 1f)]
//     public float fireVolume = 1f;

//     [Tooltip("Optional. Khali chhodo to Main Camera ki position par play hoga.")]
//     public AudioSource audioSource;


//     // ============================================================
//     // PROGRESS SETTINGS
//     // ============================================================

//     [Header("===== PROGRESS =====")]

//     [Range(0f, 1f)]
//     public float startingProgress = 0.10f;


//     // ============================================================
//     // MULTI HIT FLYING NUMBERS
//     // ============================================================

//     [Header("===== MULTI HIT NUMBERS =====")]

//     [Tooltip("ON: har fruit slice hone par +1, +2, +3... flying numbers dikhenge (1 fruit par bhi +1).")]
//     public bool showMultiHitNumbers = true;

//     [Tooltip("Optional: apna TextMeshPro (3D) prefab. Khali chhodo to number code se ban jayega.")]
//     public GameObject hitNumberPrefab;

//     [Tooltip("Number kitni der (seconds) dikhega.")]
//     public float hitNumberDuration = 1f;

//     [Tooltip("Number kitna upar udega (world units).")]
//     public float hitNumberFlyDistance = 1f;

//     [Tooltip("Number ka font size (sirf code se bane number ke liye).")]
//     public float hitNumberFontSize = 8f;

//     [Tooltip("Number ka color (sirf code se bane number ke liye).")]
//     public Color hitNumberColor = Color.yellow;

//     [Tooltip("Number ki sorting order (fruits aur boomerang ke upar dikhne ke liye).")]
//     public int hitNumberSortingOrder = 200;

//     [Tooltip("Optional: fruits/boomerang jis Sorting Layer par hain uska naam (khali = Default).")]
//     public string hitNumberSortingLayerName = "";

//     [Tooltip("Optional: agar prefab UI (TextMeshPro - Text (UI)) hai to ye Canvas use hoga. Khali chhodo to Level Text wala Canvas use hoga.")]
//     public Canvas hitNumberCanvas;


//     // ============================================================
//     // INTERNAL VARIABLES
//     // ============================================================

//     // Screen par is stage ke saare fruits
//     private class FruitData
//     {
//         public GameObject obj;
//         public Vector2 direction;
//         public float speed;
//     }

//     private List<FruitData> activeFruits =
//         new List<FruitData>();

//     private int currentStage = 0;

//     private bool stageTransitioning = false;

//     private float timeLeft = 0f;

//     private bool timeUp = false;

//     private Color timerNormalColor = Color.white;

//     private System.Random rng;

//     private int totalLevelFruits = 0;

//     private int destroyedLevelFruits = 0;

//     private bool levelCompleted = false;

//     private int currentLevelNumber = 1;


//     // ============================================================
//     // CURRENT BOOMERANG
//     // ============================================================

//     private GameObject currentBoomerang;

//     private bool currentBoomerangHitFruit = false;

//     // Is throw me jin fruits ko hit kiya unki world positions
//     private List<Vector3> currentHitPositions =
//         new List<Vector3>();


//     // ============================================================
//     // START
//     // ============================================================

//     private void Start()
//     {
//         if (gameManager == null)
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Timing_Challenge is NOT assigned."
//             );
//         }


//         currentLevelNumber = GetLevelNumber();

//         stagesPerLevel = Mathf.Max(1, stagesPerLevel);

//         totalLevelFruits = 0;

//         for (int s = 0; s < stagesPerLevel; s++)
//         {
//             totalLevelFruits += GetFruitsForStage(s);
//         }

//         totalLevelFruits = Mathf.Max(1, totalLevelFruits);

//         destroyedLevelFruits = 0;

//         rng = CreateRandom();


//         // Timer
//         timeLeft = Mathf.Max(1f, levelTime);

//         timeUp = false;

//         if (timerText != null)
//         {
//             timerNormalColor = timerText.color;
//         }

//         UpdateTimerText();


//         SetProgress(startingProgress);

//         UpdateLevelStatus(false);


//         currentStage = 0;

//         SpawnStage();

//         CreateReadyBoomerang();


//         SetLevelText("Level " + currentLevelNumber);
//     }


//     // ============================================================
//     // UPDATE
//     // ============================================================

//     private void Update()
//     {
//         UpdateTimer();

//         CheckForFireInput();

//         MoveFruits();
//     }


//     // ============================================================
//     // TIMER
//     // Game over / win ke time ruka rehta hai, revive par wahin se chalta hai
//     // ============================================================

//     private void UpdateTimer()
//     {
//         if (levelCompleted || timeUp)
//             return;


//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         timeLeft -= Time.deltaTime;

//         if (timeLeft <= 0f)
//         {
//             timeLeft = 0f;

//             UpdateTimerText();

//             TimeUp();

//             return;
//         }

//         UpdateTimerText();
//     }


//     private void UpdateTimerText()
//     {
//         if (timerText == null)
//             return;


//         int seconds =
//             Mathf.CeilToInt(timeLeft);

//         timerText.text =
//             seconds.ToString();


//         // Countdown red threshold par aate hi red
//         timerText.color =
//             timeLeft <= redTimeThreshold
//                 ? timerWarningColor
//                 : timerNormalColor;
//     }


//     // Time khatam: level clear nahi hua to Game Over
//     private void TimeUp()
//     {
//         if (timeUp || levelCompleted)
//             return;


//         timeUp = true;


//         // Udta hua boomerang hata do (revive par naya aayega)
//         if (currentBoomerang != null)
//         {
//             Destroy(currentBoomerang);
//             currentBoomerang = null;
//         }


//         if (gameManager != null)
//         {
//             gameManager.ShowGameOver();
//         }
//         else
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Cannot show Game Over because Timing_Challenge is not assigned."
//             );
//         }
//     }


//     // ============================================================
//     // SCREEN BOUNDS (world units)
//     // ============================================================

//     private bool GetScreenBounds(
//         out Vector2 min,
//         out Vector2 max)
//     {
//         min = Vector2.zero;
//         max = Vector2.zero;

//         Camera cam = Camera.main;

//         if (cam == null)
//             return false;

//         float z = Mathf.Abs(cam.transform.position.z);

//         Vector3 bottomLeft =
//             cam.ViewportToWorldPoint(new Vector3(0f, 0f, z));

//         Vector3 topRight =
//             cam.ViewportToWorldPoint(new Vector3(1f, 1f, z));

//         min = bottomLeft;
//         max = topRight;

//         return true;
//     }


//     // ============================================================
//     // FIXED RANDOM (gameId + level)
//     // Same level = hamesha same spawn pattern (retry / reload par bhi)
//     // ============================================================

//     private System.Random CreateRandom()
//     {
//         if (!keepFruitPatternPerLevel)
//         {
//             return new System.Random(
//                 System.Environment.TickCount
//             );
//         }


//         int seed = 91;

//         string id =
//             gameManager != null
//                 ? gameManager.gameId
//                 : "";

//         for (int i = 0; i < id.Length; i++)
//         {
//             seed = unchecked(seed * 31 + id[i]);
//         }

//         seed = unchecked(seed * 31 + currentLevelNumber);

//         return new System.Random(seed);
//     }


//     private float RandomRange(float min, float max)
//     {
//         if (max < min)
//         {
//             float temp = min;
//             min = max;
//             max = temp;
//         }

//         return min +
//             (float)rng.NextDouble() * (max - min);
//     }


//     // ============================================================
//     // FRUITS PER STAGE
//     // Stage 0 me startFruitsPerStage, har agle stage me fruitsIncreasePerStage
//     // badhte jaate hain (maxFruitsPerStage tak)
//     // ============================================================

//     private int GetFruitsForStage(int stage)
//     {
//         int count =
//             startFruitsPerStage +
//             stage * fruitsIncreasePerStage;

//         return Mathf.Clamp(
//             count,
//             1,
//             Mathf.Max(1, maxFruitsPerStage)
//         );
//     }


//     // ============================================================
//     // SPAWN STAGE (ek stage ke saare fruits ek saath)
//     //
//     // Sirf horizontal movement: left <-> right (ping-pong).
//     // Y spawnYMin..spawnYMax ke beech (har fruit apni alag height band me,
//     // taaki overlap na ho), X screen ke andar kahin bhi.
//     // Fruit destroy nahi hota, kinare se wapas mudta hai aur har baar
//     // screen ki center line cross karta hai, jab tak boomerang se kat na jaye.
//     // ============================================================

//     private void SpawnStage()
//     {
//         if (levelCompleted)
//             return;

//         if (activeFruits.Count > 0)
//             return;


//         if (
//             commonFruitPrefabs == null ||
//             commonFruitPrefabs.Length == 0
//         )
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Common Fruit Prefabs array is empty."
//             );

//             return;
//         }


//         // Sirf valid (null nahi) prefabs
//         List<GameObject> valid =
//             new List<GameObject>();

//         for (int i = 0; i < commonFruitPrefabs.Length; i++)
//         {
//             if (commonFruitPrefabs[i] != null)
//             {
//                 valid.Add(commonFruitPrefabs[i]);
//             }
//         }


//         if (valid.Count == 0)
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Common Fruit Prefabs me koi valid prefab nahi hai."
//             );

//             return;
//         }


//         Vector2 min;
//         Vector2 max;

//         if (!GetScreenBounds(out min, out max))
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Main Camera nahi mila, fruit spawn nahi ho sakta."
//             );

//             return;
//         }


//         float left = min.x + screenPadding;
//         float right = max.x - screenPadding;

//         int count =
//             GetFruitsForStage(currentStage);

//         float yLow =
//             Mathf.Min(spawnYMin, spawnYMax);

//         float yHigh =
//             Mathf.Max(spawnYMin, spawnYMax);

//         float band =
//             (yHigh - yLow) / count;

//         float z =
//             fruitSpawnPoint != null
//                 ? fruitSpawnPoint.position.z
//                 : 0f;


//         for (int i = 0; i < count; i++)
//         {
//             // Y: is fruit ki band ke andar random
//             float y =
//                 RandomRange(
//                     yLow + band * i,
//                     yLow + band * (i + 1)
//                 );

//             // X: screen ke andar kahin bhi
//             float x =
//                 RandomRange(left, right);

//             GameObject prefab =
//                 valid[rng.Next(valid.Count)];

//             GameObject fruit =
//                 Instantiate(
//                     prefab,
//                     new Vector3(x, y, z),
//                     Quaternion.identity,
//                     fruitSpawnPoint
//                 );


//             Collider2D collider =
//                 fruit.GetComponent<Collider2D>();

//             if (collider == null)
//             {
//                 collider =
//                     fruit.AddComponent<CircleCollider2D>();
//             }

//             collider.enabled = true;


//             Rigidbody2D rb =
//                 fruit.GetComponent<Rigidbody2D>();

//             if (rb == null)
//             {
//                 rb =
//                     fruit.AddComponent<Rigidbody2D>();
//             }

//             rb.bodyType =
//                 RigidbodyType2D.Kinematic;

//             rb.gravityScale = 0f;

//             rb.linearVelocity =
//                 Vector2.zero;

//             rb.angularVelocity = 0f;

//             rb.simulated = true;


//             FruitData data = new FruitData();

//             data.obj = fruit;

//             // Shuru me left ya right, random
//             data.direction =
//                 rng.Next(2) == 0
//                     ? Vector2.left
//                     : Vector2.right;

//             data.speed =
//                 RandomRange(fruitSpeedMin, fruitSpeedMax);

//             activeFruits.Add(data);
//         }
//     }


//     // ============================================================
//     // MOVE FRUITS
//     // ============================================================

//     private void MoveFruits()
//     {
//         if (activeFruits.Count == 0)
//             return;

//         if (levelCompleted)
//             return;

//         // Game over / win ke time fruits ruke rahe (revive par wahin se continue)
//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         Vector2 min;
//         Vector2 max;

//         if (!GetScreenBounds(out min, out max))
//             return;


//         float leftLimit = min.x + screenPadding;
//         float rightLimit = max.x - screenPadding;


//         for (int i = 0; i < activeFruits.Count; i++)
//         {
//             FruitData data = activeFruits[i];

//             if (data.obj == null)
//                 continue;


//             Transform t = data.obj.transform;

//             t.position +=
//                 (Vector3)(
//                     data.direction *
//                     data.speed *
//                     Time.deltaTime
//                 );


//             // Fruit destroy nahi hota: kinare par pahunchte hi wapas mud jata hai
//             // (left -> right aur right -> left, jab tak kat na jaye)
//             Vector3 pos = t.position;

//             if (pos.x <= leftLimit)
//             {
//                 pos.x = leftLimit;
//                 data.direction = Vector2.right;
//             }
//             else if (pos.x >= rightLimit)
//             {
//                 pos.x = rightLimit;
//                 data.direction = Vector2.left;
//             }

//             t.position = pos;
//         }
//     }


//     // ============================================================
//     // STAGE CLEAR -> NEXT STAGE
//     // ============================================================

//     private IEnumerator NextStageRoutine()
//     {
//         stageTransitioning = true;

//         if (nextStageDelay > 0f)
//         {
//             yield return new WaitForSeconds(nextStageDelay);
//         }

//         // Game over ho to revive tak ruko, win ho to band
//         while (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             if (levelCompleted || gameManager.IsLevelWon())
//             {
//                 stageTransitioning = false;
//                 yield break;
//             }

//             yield return null;
//         }

//         stageTransitioning = false;

//         currentStage++;

//         SpawnStage();
//     }


//     // ============================================================
//     // CHECK FIRE INPUT
//     // ============================================================

//     private bool TryGetTapPosition(out Vector2 screenPosition)
//     {
//         screenPosition = Vector2.zero;


//         if (Touchscreen.current != null)
//         {
//             if (
//                 Touchscreen.current.primaryTouch.press
//                     .wasPressedThisFrame
//             )
//             {
//                 screenPosition =
//                     Touchscreen.current.primaryTouch.position
//                         .ReadValue();

//                 return true;
//             }
//         }


//         if (Mouse.current != null)
//         {
//             if (
//                 Mouse.current.leftButton
//                     .wasPressedThisFrame
//             )
//             {
//                 screenPosition =
//                     Mouse.current.position.ReadValue();

//                 return true;
//             }
//         }


//         return false;
//     }


//     private void CheckForFireInput()
//     {
//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         Vector2 tapScreenPosition;

//         if (!TryGetTapPosition(out tapScreenPosition))
//             return;


//         FireReadyBoomerang(tapScreenPosition);
//     }


//     // ============================================================
//     // CREATE READY BOOMERANG
//     // ============================================================

//     private void CreateReadyBoomerang()
//     {
//         if (boomerangPrefab == null)
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Boomerang Prefab is NOT assigned."
//             );

//             return;
//         }


//         if (boomerangSpawnPoint == null)
//         {
//             Debug.LogWarning(
//                 "Timing_Challenge2: Boomerang Spawn Point is NOT assigned."
//             );

//             return;
//         }


//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         if (oneBoomerangAtATime && currentBoomerang != null)
//         {
//             return;
//         }


//         currentBoomerangHitFruit = false;

//         currentHitPositions.Clear();


//         currentBoomerang =
//             Instantiate(
//                 boomerangPrefab,
//                 boomerangSpawnPoint.position,
//                 boomerangSpawnPoint.rotation
//             );


//         currentBoomerang.SetActive(true);


//         // Prefab par purana KnifeController2 ho to hata do (conflict se bachne ke liye)
//         KnifeController2 oldKnife2 =
//             currentBoomerang.GetComponent<KnifeController2>();

//         if (oldKnife2 != null)
//         {
//             oldKnife2.enabled = false;
//             Destroy(oldKnife2);
//         }


//         // Prefab par agar original KnifeController laga ho to usse hata do (conflict se bachne ke liye)
//         KnifeController originalController =
//             currentBoomerang.GetComponent<KnifeController>();

//         if (originalController != null)
//         {
//             originalController.enabled = false;
//             Destroy(originalController);
//         }


//         SpriteRenderer[] renderers =
//             currentBoomerang.GetComponentsInChildren<SpriteRenderer>(
//                 true
//             );


//         foreach (
//             SpriteRenderer renderer
//             in renderers
//         )
//         {
//             renderer.enabled = true;
//             renderer.sortingOrder = 100;
//         }


//         Collider2D[] colliders =
//             currentBoomerang.GetComponentsInChildren<Collider2D>(
//                 true
//             );


//         foreach (
//             Collider2D collider
//             in colliders
//         )
//         {
//             collider.enabled = true;

//             collider.isTrigger = true;
//         }


//         Rigidbody2D rb =
//             currentBoomerang.GetComponent<Rigidbody2D>();


//         if (rb == null)
//         {
//             rb =
//                 currentBoomerang.AddComponent<Rigidbody2D>();
//         }


//         rb.bodyType =
//             RigidbodyType2D.Kinematic;

//         rb.gravityScale = 0f;

//         rb.linearVelocity =
//             Vector2.zero;

//         rb.angularVelocity = 0f;

//         rb.simulated = true;


//         TimingBoomerang controller =
//             currentBoomerang.GetComponent<TimingBoomerang>();


//         if (controller == null)
//         {
//             controller =
//                 currentBoomerang.AddComponent<TimingBoomerang>();
//         }


//         controller.enabled = true;


//         controller.moveSpeed =
//             boomerangSpeed;

//         controller.spinSpeed =
//             boomerangSpinSpeed;

//         controller.range =
//             boomerangRange;

//         controller.readyMoveSpeed =
//             boomerangReadyMoveSpeed;

//         controller.startOffset =
//             boomerangStartOffset;

//         controller.gameManager =
//             this;


//         controller.InitializeReady(
//             boomerangSpawnPoint.position
//         );
//     }


//     // ============================================================
//     // FIRE READY BOOMERANG (tap kahin bhi, seedha upar)
//     // ============================================================

//     private void FireReadyBoomerang(Vector2 tapScreenPosition)
//     {
//         if (currentBoomerang == null)
//             return;


//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         // Khali screen par fire karke game over na ho
//         if (fireOnlyWhenFruitPresent && activeFruits.Count == 0)
//             return;


//         TimingBoomerang controller =
//             currentBoomerang.GetComponent<TimingBoomerang>();


//         if (controller == null)
//             return;


//         // Tap kahin bhi ho, boomerang hamesha seedha upar jayega
//         if (controller.Fire(Vector2.up))
//         {
//             PlayFireSound();
//         }
//     }


//     // ============================================================
//     // FIRE SOUND
//     // ============================================================

//     private void PlayFireSound()
//     {
//         if (fireClip == null)
//             return;


//         // Settings me Sound OFF hai to kuch na bajao
//         if (!GameAudio.SoundEnabled)
//             return;


//         if (audioSource != null)
//         {
//             audioSource.PlayOneShot(
//                 fireClip,
//                 fireVolume
//             );
//         }
//         else if (Camera.main != null)
//         {
//             AudioSource.PlayClipAtPoint(
//                 fireClip,
//                 Camera.main.transform.position,
//                 fireVolume
//             );
//         }
//     }


//     // ============================================================
//     // BOOMERANG FINISHED (wapas ready position par aa gaya)
//     // ============================================================

//     public void BoomerangFinished(
//         TimingBoomerang boomerang)
//     {
//         if (boomerang == null)
//             return;


//         if (
//             currentBoomerang !=
//             boomerang.gameObject
//         )
//         {
//             return;
//         }


//         bool missed =
//             !currentBoomerangHitFruit;


//         currentBoomerang = null;


//         if (missed)
//         {
//             if (gameManager != null)
//             {
//                 gameManager.ShowGameOver();
//             }
//             else
//             {
//                 Debug.LogWarning(
//                     "Timing_Challenge2: Cannot show Game Over because Timing_Challenge is not assigned."
//                 );
//             }


//             return;
//         }


//         if (levelCompleted)
//             return;


//         CreateReadyBoomerang();
//     }


//     // ============================================================
//     // FRUIT HIT
//     // ============================================================

//     public void HitFruit(
//         GameObject fruit)
//     {
//         if (fruit == null || levelCompleted || timeUp)
//             return;


//         // Time khatam / game over ke baad koi hit count nahi hoga
//         if (
//             gameManager != null &&
//             gameManager.IsGameEnded()
//         )
//         {
//             return;
//         }


//         // Collider child par ho sakta hai, isliye child bhi chalega
//         FruitData hit = null;

//         for (int i = 0; i < activeFruits.Count; i++)
//         {
//             GameObject o = activeFruits[i].obj;

//             if (
//                 o != null &&
//                 (fruit == o || fruit.transform.IsChildOf(o.transform))
//             )
//             {
//                 hit = activeFruits[i];
//                 break;
//             }
//         }

//         if (hit == null)
//             return;


//         GameObject cutFruit = hit.obj;

//         activeFruits.Remove(hit);


//         currentBoomerangHitFruit = true;


//         // Flying number ke liye hit position (fruit destroy hone se pehle)
//         RegisterHit(cutFruit.transform.position);


//         destroyedLevelFruits++;

//         UpdateLevelProgress();


//         // Turant destroy nahi: Fruit.Cut() colliders/visible band karke
//         // effect + sound chalayega aur delay ke baad khud destroy karega
//         Fruit fruitScript =
//             cutFruit.GetComponent<Fruit>();

//         if (fruitScript != null)
//         {
//             fruitScript.Cut();
//         }
//         else
//         {
//             Destroy(cutFruit);
//         }


//         // Is stage ke saare fruits kat gaye
//         if (activeFruits.Count == 0)
//         {
//             if (currentStage + 1 >= stagesPerLevel)
//             {
//                 // Aakhri stage clear = level clear (timer khatam hone se pehle)
//                 CompleteLevel();
//             }
//             else if (!stageTransitioning)
//             {
//                 StartCoroutine(NextStageRoutine());
//             }
//         }
//     }


//     // ============================================================
//     // UPDATE LEVEL PROGRESS
//     // ============================================================

//     private void UpdateLevelProgress()
//     {
//         if (totalLevelFruits <= 0)
//             return;


//         float progress =
//             (float)destroyedLevelFruits /
//             totalLevelFruits;


//         float finalProgress =
//             Mathf.Lerp(
//                 startingProgress,
//                 1f,
//                 progress
//             );


//         SetProgress(finalProgress);
//     }


//     // ============================================================
//     // SET PROGRESS
//     // ============================================================

//     private void SetProgress(
//         float value)
//     {
//         if (progressImage == null)
//             return;


//         progressImage.fillAmount =
//             Mathf.Clamp01(value);
//     }


//     // ============================================================
//     // LEVEL COMPLETE
//     // ============================================================

//     private void CompleteLevel()
//     {
//         if (levelCompleted)
//             return;


//         levelCompleted = true;
//         UpdateLevelStatus(true);


//         SetProgress(1f);


//         if (gameManager != null)
//         {
//             gameManager.GiveLevelCompleteRewards();
//             gameManager.ShowWin();
//         }
//     }


//     // ============================================================
//     // LEVEL TEXT
//     // ============================================================

//     private void SetLevelText(
//         string text)
//     {
//         if (levelText != null)
//         {
//             levelText.text = text;
//         }
//     }


//     // ============================================================
//     // MULTI HIT FLYING NUMBERS
//     // ============================================================

//     private void RegisterHit(Vector3 worldPosition)
//     {
//         currentHitPositions.Add(worldPosition);

//         if (!showMultiHitNumbers)
//             return;


//         // 1st fruit par +1, 2nd par +2, 3rd par +3 ...
//         int hitCount =
//             currentHitPositions.Count;

//         SpawnHitNumber(worldPosition, hitCount);
//     }


//     private void SpawnHitNumber(
//         Vector3 worldPosition,
//         int number)
//     {
//         GameObject obj;

//         TMP_Text text;

//         Canvas uiCanvas = null;


//         if (hitNumberPrefab != null)
//         {
//             obj =
//                 Instantiate(
//                     hitNumberPrefab,
//                     worldPosition,
//                     Quaternion.identity
//                 );

//             // Prefab inactive ho to bhi dikhe
//             obj.SetActive(true);

//             text =
//                 obj.GetComponentInChildren<TMP_Text>(true);


//             if (text == null)
//             {
//                 Debug.LogWarning(
//                     "Timing_Challenge2: Hit Number Prefab me TextMeshPro nahi mila."
//                 );

//                 Destroy(obj);

//                 return;
//             }


//             if (text is TextMeshProUGUI)
//             {
//                 // UI text bina Canvas ke screen par nahi dikhta,
//                 // isliye isse Canvas ke andar daalte hain.
//                 uiCanvas = GetHitNumberCanvas();

//                 if (uiCanvas == null)
//                 {
//                     Debug.LogWarning(
//                         "Timing_Challenge2: Hit Number Prefab UI hai par scene me Canvas nahi mila."
//                     );

//                     Destroy(obj);

//                     return;
//                 }

//                 obj.transform.SetParent(
//                     uiCanvas.transform,
//                     false
//                 );

//                 obj.transform.SetAsLastSibling();

//                 SetUIPositionFromWorld(
//                     obj.transform as RectTransform,
//                     uiCanvas,
//                     worldPosition
//                 );
//             }
//             else
//             {
//                 // World text: fruits ke upar dikhe
//                 TextMeshPro worldPrefabText =
//                     text as TextMeshPro;

//                 if (worldPrefabText != null)
//                 {
//                     ApplyHitNumberSorting(worldPrefabText);
//                 }
//             }
//         }
//         else
//         {
//             obj = new GameObject("HitNumber");

//             obj.transform.position = worldPosition;

//             TextMeshPro worldText =
//                 obj.AddComponent<TextMeshPro>();

//             worldText.fontSize = hitNumberFontSize;
//             worldText.fontStyle = FontStyles.Bold;
//             worldText.alignment = TextAlignmentOptions.Center;
//             worldText.color = hitNumberColor;

//             ApplyHitNumberSorting(worldText);

//             text = worldText;
//         }


//         // Prefab ka alpha 0 ho to bhi number dikhe
//         Color startColor = text.color;

//         if (startColor.a < 0.01f)
//         {
//             startColor.a = 1f;
//             text.color = startColor;
//         }


//         text.text = "+" + number;


//         StartCoroutine(
//             FlyHitNumber(
//                 obj,
//                 text,
//                 worldPosition,
//                 uiCanvas
//             )
//         );
//     }


//     private void ApplyHitNumberSorting(TextMeshPro worldText)
//     {
//         worldText.sortingOrder = hitNumberSortingOrder;

//         if (!string.IsNullOrEmpty(hitNumberSortingLayerName))
//         {
//             worldText.sortingLayerID =
//                 SortingLayer.NameToID(hitNumberSortingLayerName);
//         }
//     }


//     private Canvas GetHitNumberCanvas()
//     {
//         Canvas canvas = hitNumberCanvas;

//         if (canvas == null && levelText != null)
//         {
//             canvas = levelText.canvas;
//         }

//         if (canvas == null)
//         {
//             canvas = FindFirstObjectByType<Canvas>();
//         }

//         if (canvas == null)
//             return null;

//         return canvas.rootCanvas;
//     }


//     // World position ko Canvas ke andar UI position me badalta hai
//     private void SetUIPositionFromWorld(
//         RectTransform rect,
//         Canvas canvas,
//         Vector3 worldPosition)
//     {
//         if (rect == null || canvas == null)
//             return;


//         Camera worldCamera = Camera.main;

//         if (worldCamera == null)
//             worldCamera = canvas.worldCamera;

//         if (worldCamera == null)
//             return;


//         Vector2 screenPoint =
//             worldCamera.WorldToScreenPoint(worldPosition);


//         if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
//         {
//             rect.position = screenPoint;
//         }
//         else
//         {
//             Vector3 canvasPoint;

//             if (
//                 RectTransformUtility.ScreenPointToWorldPointInRectangle(
//                     canvas.transform as RectTransform,
//                     screenPoint,
//                     canvas.worldCamera,
//                     out canvasPoint
//                 )
//             )
//             {
//                 rect.position = canvasPoint;
//             }
//         }
//     }


//     private IEnumerator FlyHitNumber(
//         GameObject obj,
//         TMP_Text text,
//         Vector3 startWorld,
//         Canvas uiCanvas)
//     {
//         float duration =
//             Mathf.Max(0.05f, hitNumberDuration);

//         Vector3 endWorld =
//             startWorld +
//             Vector3.up * hitNumberFlyDistance;

//         Color baseColor =
//             text.color;

//         float timer = 0f;


//         while (timer < duration)
//         {
//             if (obj == null)
//                 yield break;


//             timer += Time.deltaTime;

//             float t =
//                 Mathf.Clamp01(timer / duration);


//             // Upar ud ta hua (ease out)
//             float eased =
//                 1f - Mathf.Pow(1f - t, 2f);

//             Vector3 currentWorld =
//                 Vector3.Lerp(
//                     startWorld,
//                     endWorld,
//                     eased
//                 );


//             if (uiCanvas != null)
//             {
//                 SetUIPositionFromWorld(
//                     obj.transform as RectTransform,
//                     uiCanvas,
//                     currentWorld
//                 );
//             }
//             else
//             {
//                 obj.transform.position = currentWorld;
//             }


//             // Aakhri half me fade out
//             float alpha =
//                 t < 0.5f
//                     ? 1f
//                     : 1f - ((t - 0.5f) / 0.5f);

//             text.color =
//                 new Color(
//                     baseColor.r,
//                     baseColor.g,
//                     baseColor.b,
//                     baseColor.a * alpha
//                 );


//             yield return null;
//         }


//         if (obj != null)
//         {
//             Destroy(obj);
//         }
//     }


//     // ============================================================
//     // ON DESTROY
//     // ============================================================

//     private void OnDestroy()
//     {
//         if (currentBoomerang != null)
//         {
//             Destroy(currentBoomerang);
//         }
//     }


//     // ============================================================
//     // UPDATE LEVEL STATUS
//     // ============================================================

//     private void UpdateLevelStatus(bool completed)
//     {
//         if (gameManager == null)
//             return;

//         gameManager.UpdateLevelStatusText(
//             GetLevelNumber(),
//             completed
//         );
//     }


//     // ============================================================
//     // LEVEL NUMBER (Timing_Challenge ke gameId se)
//     // ============================================================

//     private int GetLevelNumber()
//     {
//         if (gameManager != null)
//             return gameManager.GetCurrentLevel();

//         return 1;
//     }


//     // ============================================================
//     // REVIVE (naam ReviveKnife hi rakha hai, Timing_Challenge isi ko call karta hai)
//     // Same stage, same fruits, bas naya boomerang ready
//     // Time khatam hone par game over hua tha to reviveExtraTime wapas milega
//     // ============================================================

//     public void ReviveKnife()
//     {
//         if (levelCompleted)
//             return;


//         if (timeUp)
//         {
//             timeUp = false;

//             timeLeft = Mathf.Max(1f, reviveExtraTime);

//             UpdateTimerText();
//         }

//         // Agar koi purana boomerang bacha ho to hata do
//         if (currentBoomerang != null)
//         {
//             Destroy(currentBoomerang);
//             currentBoomerang = null;
//         }

//         CreateReadyBoomerang();
//     }
// }



using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

// ============================================================
// BOOMERANG (sirf Timing_Challenge2 ke liye, isi file me)
// Runtime par AddComponent se lagta hai, alag script ki zarurat nahi
//
// Flow: ready position par ruka rehta hai -> tap par tap ki taraf udta hai
// (range tak) -> wapas ready position par aata hai. Raste me jo fruit
// touch hota hai wo kat jata hai.
// ============================================================

public class TimingBoomerang : MonoBehaviour
{
    [HideInInspector] public float moveSpeed = 14f;
    [HideInInspector] public float spinSpeed = 900f;
    [HideInInspector] public float range = 8f;
    [HideInInspector] public float readyMoveSpeed = 8f;
    [HideInInspector] public float startOffset = 1.5f;
    [HideInInspector] public Timing_Challenge2 gameManager;

    private Vector3 readyPosition;
    private Vector3 fireOrigin;
    private Vector3 fireDirection;

    private bool isFired = false;
    private bool isReturning = false;
    private bool isFinished = false;


    public void InitializeReady(Vector3 position)
    {
        readyPosition = position;
        isFired = false;
        isReturning = false;
        isFinished = false;

        // Ready point ke neeche se start, phir upar ready position tak aayega
        transform.position =
            position + Vector3.down * startOffset;
    }


    // true = fire ho gaya, false = pehle se fire ho chuka tha
    public bool Fire(Vector2 direction)
    {
        if (isFired || isFinished)
            return false;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.up;
        }

        fireDirection =
            ((Vector3)direction).normalized;

        fireOrigin = transform.position;

        isFired = true;
        isReturning = false;

        return true;
    }


    private void Update()
    {
        if (isFinished)
            return;


        if (!isFired)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    readyPosition,
                    readyMoveSpeed * Time.deltaTime
                );

            return;
        }


        transform.Rotate(
            0f,
            0f,
            spinSpeed * Time.deltaTime
        );


        if (!isReturning)
        {
            // Tap ki direction me seedha jao, range tak
            transform.position +=
                fireDirection * moveSpeed * Time.deltaTime;

            float traveled =
                Vector3.Distance(
                    fireOrigin,
                    transform.position
                );

            if (traveled >= range)
            {
                isReturning = true;
            }
        }
        else
        {
            // Wapas ready position par
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    readyPosition,
                    moveSpeed * Time.deltaTime
                );

            if (
                (transform.position - readyPosition)
                    .sqrMagnitude < 0.0001f
            )
            {
                Finish();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFired || isFinished)
            return;

        if (gameManager == null)
            return;


        Fruit fruit =
            other.GetComponentInParent<Fruit>();

        GameObject target =
            fruit != null
                ? fruit.gameObject
                : other.gameObject;

        gameManager.HitFruit(target);
    }


    private void Finish()
    {
        if (isFinished)
            return;

        isFinished = true;

        if (gameManager != null)
        {
            gameManager.BoomerangFinished(this);
        }

        Destroy(gameObject);
    }
}


public class Timing_Challenge2 : MonoBehaviour
{
    // ============================================================
    // UI
    // ============================================================

    [Header("===== UI =====")]

    public TMP_Text levelText;

    public Image progressImage;

    [Tooltip("Countdown timer ka TMP Text.")]
    public TMP_Text timerText;


    // ============================================================
    // TIMER
    // ============================================================

    [Header("===== TIMER =====")]

    [Tooltip("Poora level clear karne ke liye total seconds. Ye khatam hone se pehle sab stages clear karne hain, warna Game Over.")]
    public float levelTime = 30f;

    [Tooltip("Countdown itne (ya kam) seconds par aate hi timer text red ho jayega.")]
    public float redTimeThreshold = 5f;

    [Tooltip("Timer text ka red color.")]
    public Color timerWarningColor = Color.red;

    [Tooltip("Revive par (agar time khatam ho chuka tha) itne seconds wapas milenge.")]
    public float reviveExtraTime = 10f;


    // ============================================================
    // GAME OVER REASONS (game over reason text me dikhenge)
    // Code me fixed hain (Inspector me nahi), taaki purani saved value na aaye
    // ============================================================

    private const string TimeUpReason = "Clear It Before {0} Seconds";

    private const string FruitMissReason = " You Failed To Hit Fruits";


    // ============================================================
    // GAME MANAGER
    // ============================================================

    [Header("===== GAME STATE MANAGER =====")]

    [Tooltip("Main Timing_Challenge which handles Win and Game Over.")]
    public Timing_Challenge gameManager;


    // ============================================================
    // FRUITS
    // ============================================================

    [Header("===== FRUITS =====")]

    [Tooltip("Optional. Assign karoge to spawn hue fruits is object ke andar (parent) aayenge. Position screen ke hisab se hi hogi.")]
    public Transform fruitSpawnPoint;

    [Tooltip("Fruit prefabs. Har baar inme se random ek fruit aayega.")]
    public GameObject[] commonFruitPrefabs;

    [Tooltip("Ek level me kitne stages hain. Har stage ke saare fruits kaatne par agla stage aata hai.")]
    public int stagesPerLevel = 3;

    [Tooltip("Pehle stage me ek saath kitne fruits aayenge.")]
    public int startFruitsPerStage = 3;

    [Tooltip("Har agle stage me fruits kitne badhenge.")]
    public int fruitsIncreasePerStage = 1;

    [Tooltip("Ek stage me ek saath maximum kitne fruits.")]
    public int maxFruitsPerStage = 6;

    [Tooltip("Stage clear hone ke baad agla stage kitni der baad aayega.")]
    public float nextStageDelay = 0.8f;

    [Tooltip("ON: ek level me fruits ka spawn pattern fixed rahega (retry/reload par wahi). OFF: har baar naya random.")]
    public bool keepFruitPatternPerLevel = true;

    [Tooltip("Fruit ki minimum speed.")]
    public float fruitSpeedMin = 2.5f;

    [Tooltip("Fruit ki maximum speed.")]
    public float fruitSpeedMax = 4f;

    [Tooltip("Fruit spawn hone ki minimum height (Y, world units).")]
    public float spawnYMin = -1f;

    [Tooltip("Fruit spawn hone ki maximum height (Y, world units).")]
    public float spawnYMax = 3f;

    [Tooltip("Screen ke left/right kinare se kitna andar fruit ghoomega aur wapas mudega (world units).")]
    public float screenPadding = 0.7f;


    // ============================================================
    // BOOMERANG
    // ============================================================

    [Header("===== BOOMERANG =====")]

    [FormerlySerializedAs("knifePrefab")]
    [Tooltip("Boomerang prefab.")]
    public GameObject boomerangPrefab;

    [FormerlySerializedAs("knifeSpawnPoint")]
    [Tooltip("Point jahan boomerang ready rehta hai aur jahan wapas aata hai.")]
    public Transform boomerangSpawnPoint;

    [FormerlySerializedAs("knifeMoveSpeed")]
    [Tooltip("Boomerang ki udne ki speed.")]
    public float boomerangSpeed = 14f;

    [FormerlySerializedAs("knifeRotationSpeed")]
    [Tooltip("Udte waqt boomerang ghoomne ki speed.")]
    public float boomerangSpinSpeed = 900f;

    [Tooltip("Tap ki direction me boomerang kitni door tak jayega, phir wapas aayega.")]
    public float boomerangRange = 8f;

    [FormerlySerializedAs("knifeStartOffset")]
    [Tooltip("Ready point ke kitna neeche se boomerang start hoga.")]
    public float boomerangStartOffset = 1.5f;

    [FormerlySerializedAs("knifeReadyMoveSpeed")]
    [Tooltip("Ready position tak aane ki speed.")]
    public float boomerangReadyMoveSpeed = 8f;

    [FormerlySerializedAs("oneKnifeAtATime")]
    [Tooltip("Ek time par sirf ek boomerang.")]
    public bool oneBoomerangAtATime = true;

    [Tooltip("ON: jab screen par koi fruit na ho tab tap se fire nahi hoga (khali jaane se game over na ho).")]
    public bool fireOnlyWhenFruitPresent = true;

    [Tooltip("ON: UI Button par tap karne se boomerang fire nahi hoga (background image/panel par tap se fire hoga). Agar fire nahi ho raha to isse OFF karke dekho.")]
    public bool ignoreTapsOnUI = true;


    // ============================================================
    // SOUND
    // ============================================================

    [Header("===== SOUND =====")]

    [FormerlySerializedAs("knifeFireClip")]
    [Tooltip("Boomerang fire hone par ye sound bajega.")]
    public AudioClip fireClip;

    [FormerlySerializedAs("knifeFireVolume")]
    [Range(0f, 1f)]
    public float fireVolume = 1f;

    [Tooltip("Optional. Khali chhodo to Main Camera ki position par play hoga.")]
    public AudioSource audioSource;


    // ============================================================
    // PROGRESS SETTINGS
    // ============================================================

    [Header("===== PROGRESS =====")]

    [Range(0f, 1f)]
    public float startingProgress = 0.10f;


    // ============================================================
    // MULTI HIT FLYING NUMBERS
    // ============================================================

    [Header("===== MULTI HIT NUMBERS =====")]

    [Tooltip("ON: har fruit slice hone par +1, +2, +3... flying numbers dikhenge (1 fruit par bhi +1).")]
    public bool showMultiHitNumbers = true;

    [Tooltip("Optional: apna TextMeshPro (3D) prefab. Khali chhodo to number code se ban jayega.")]
    public GameObject hitNumberPrefab;

    [Tooltip("Number kitni der (seconds) dikhega.")]
    public float hitNumberDuration = 1f;

    [Tooltip("Number kitna upar udega (world units).")]
    public float hitNumberFlyDistance = 1f;

    [Tooltip("Number ka font size (sirf code se bane number ke liye).")]
    public float hitNumberFontSize = 8f;

    [Tooltip("Number ka color (sirf code se bane number ke liye).")]
    public Color hitNumberColor = Color.yellow;

    [Tooltip("Number ki sorting order (fruits aur boomerang ke upar dikhne ke liye).")]
    public int hitNumberSortingOrder = 200;

    [Tooltip("Optional: fruits/boomerang jis Sorting Layer par hain uska naam (khali = Default).")]
    public string hitNumberSortingLayerName = "";

    [Tooltip("Optional: agar prefab UI (TextMeshPro - Text (UI)) hai to ye Canvas use hoga. Khali chhodo to Level Text wala Canvas use hoga.")]
    public Canvas hitNumberCanvas;


    // ============================================================
    // INTERNAL VARIABLES
    // ============================================================

    // Screen par is stage ke saare fruits
    private class FruitData
    {
        public GameObject obj;
        public Vector2 direction;
        public float speed;
    }

    private List<FruitData> activeFruits =
        new List<FruitData>();

    private int currentStage = 0;

    private bool stageTransitioning = false;

    private float timeLeft = 0f;

    private bool timeUp = false;

    private Color timerNormalColor = Color.white;

    private System.Random rng;

    private int totalLevelFruits = 0;

    private int destroyedLevelFruits = 0;

    private bool levelCompleted = false;

    private int currentLevelNumber = 1;


    // ============================================================
    // CURRENT BOOMERANG
    // ============================================================

    private GameObject currentBoomerang;

    private bool currentBoomerangHitFruit = false;

    // Is throw me jin fruits ko hit kiya unki world positions
    private List<Vector3> currentHitPositions =
        new List<Vector3>();


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (gameManager == null)
        {
            Debug.LogWarning(
                "Timing_Challenge2: Timing_Challenge is NOT assigned."
            );
        }


        currentLevelNumber = GetLevelNumber();

        stagesPerLevel = Mathf.Max(1, stagesPerLevel);

        totalLevelFruits = 0;

        for (int s = 0; s < stagesPerLevel; s++)
        {
            totalLevelFruits += GetFruitsForStage(s);
        }

        totalLevelFruits = Mathf.Max(1, totalLevelFruits);

        destroyedLevelFruits = 0;

        rng = CreateRandom();


        // Timer
        timeLeft = Mathf.Max(1f, levelTime);

        timeUp = false;

        if (timerText != null)
        {
            timerNormalColor = timerText.color;
        }

        UpdateTimerText();


        SetProgress(startingProgress);

        UpdateLevelStatus(false);


        currentStage = 0;

        SpawnStage();

        CreateReadyBoomerang();


        SetLevelText("Level " + currentLevelNumber);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        UpdateTimer();

        CheckForFireInput();

        MoveFruits();
    }


    // ============================================================
    // TIMER
    // Game over / win ke time ruka rehta hai, revive par wahin se chalta hai
    // ============================================================

    private void UpdateTimer()
    {
        if (levelCompleted || timeUp)
            return;


        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;

            UpdateTimerText();

            TimeUp();

            return;
        }

        UpdateTimerText();
    }


    private void UpdateTimerText()
    {
        if (timerText == null)
            return;


        int seconds =
            Mathf.CeilToInt(timeLeft);

        timerText.text =
            seconds.ToString();


        // Countdown red threshold par aate hi red
        timerText.color =
            timeLeft <= redTimeThreshold
                ? timerWarningColor
                : timerNormalColor;
    }


    // Time khatam: level clear nahi hua to Game Over
    private void TimeUp()
    {
        if (timeUp || levelCompleted)
            return;


        timeUp = true;


        // Udta hua boomerang hata do (revive par naya aayega)
        if (currentBoomerang != null)
        {
            Destroy(currentBoomerang);
            currentBoomerang = null;
        }


        if (gameManager != null)
        {
            gameManager.ShowGameOver(TimeUpReason);
        }
        else
        {
            Debug.LogWarning(
                "Timing_Challenge2: Cannot show Game Over because Timing_Challenge is not assigned."
            );
        }
    }


    // ============================================================
    // SCREEN BOUNDS (world units)
    // ============================================================

    private bool GetScreenBounds(
        out Vector2 min,
        out Vector2 max)
    {
        min = Vector2.zero;
        max = Vector2.zero;

        Camera cam = Camera.main;

        if (cam == null)
            return false;

        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 bottomLeft =
            cam.ViewportToWorldPoint(new Vector3(0f, 0f, z));

        Vector3 topRight =
            cam.ViewportToWorldPoint(new Vector3(1f, 1f, z));

        min = bottomLeft;
        max = topRight;

        return true;
    }


    // ============================================================
    // FIXED RANDOM (gameId + level)
    // Same level = hamesha same spawn pattern (retry / reload par bhi)
    // ============================================================

    private System.Random CreateRandom()
    {
        if (!keepFruitPatternPerLevel)
        {
            return new System.Random(
                System.Environment.TickCount
            );
        }


        int seed = 91;

        string id =
            gameManager != null
                ? gameManager.gameId
                : "";

        for (int i = 0; i < id.Length; i++)
        {
            seed = unchecked(seed * 31 + id[i]);
        }

        seed = unchecked(seed * 31 + currentLevelNumber);

        return new System.Random(seed);
    }


    private float RandomRange(float min, float max)
    {
        if (max < min)
        {
            float temp = min;
            min = max;
            max = temp;
        }

        return min +
            (float)rng.NextDouble() * (max - min);
    }


    // ============================================================
    // FRUITS PER STAGE
    // Stage 0 me startFruitsPerStage, har agle stage me fruitsIncreasePerStage
    // badhte jaate hain (maxFruitsPerStage tak)
    // ============================================================

    private int GetFruitsForStage(int stage)
    {
        int count =
            startFruitsPerStage +
            stage * fruitsIncreasePerStage;

        return Mathf.Clamp(
            count,
            1,
            Mathf.Max(1, maxFruitsPerStage)
        );
    }


    // ============================================================
    // SPAWN STAGE (ek stage ke saare fruits ek saath)
    //
    // Sirf horizontal movement: left <-> right (ping-pong).
    // Y spawnYMin..spawnYMax ke beech (har fruit apni alag height band me,
    // taaki overlap na ho), X screen ke andar kahin bhi.
    // Fruit destroy nahi hota, kinare se wapas mudta hai aur har baar
    // screen ki center line cross karta hai, jab tak boomerang se kat na jaye.
    // ============================================================

    private void SpawnStage()
    {
        if (levelCompleted)
            return;

        if (activeFruits.Count > 0)
            return;


        if (
            commonFruitPrefabs == null ||
            commonFruitPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "Timing_Challenge2: Common Fruit Prefabs array is empty."
            );

            return;
        }


        // Sirf valid (null nahi) prefabs
        List<GameObject> valid =
            new List<GameObject>();

        for (int i = 0; i < commonFruitPrefabs.Length; i++)
        {
            if (commonFruitPrefabs[i] != null)
            {
                valid.Add(commonFruitPrefabs[i]);
            }
        }


        if (valid.Count == 0)
        {
            Debug.LogWarning(
                "Timing_Challenge2: Common Fruit Prefabs me koi valid prefab nahi hai."
            );

            return;
        }


        Vector2 min;
        Vector2 max;

        if (!GetScreenBounds(out min, out max))
        {
            Debug.LogWarning(
                "Timing_Challenge2: Main Camera nahi mila, fruit spawn nahi ho sakta."
            );

            return;
        }


        float left = min.x + screenPadding;
        float right = max.x - screenPadding;

        int count =
            GetFruitsForStage(currentStage);

        float yLow =
            Mathf.Min(spawnYMin, spawnYMax);

        float yHigh =
            Mathf.Max(spawnYMin, spawnYMax);

        float band =
            (yHigh - yLow) / count;

        float z =
            fruitSpawnPoint != null
                ? fruitSpawnPoint.position.z
                : 0f;


        for (int i = 0; i < count; i++)
        {
            // Y: is fruit ki band ke andar random
            float y =
                RandomRange(
                    yLow + band * i,
                    yLow + band * (i + 1)
                );

            // X: screen ke andar kahin bhi
            float x =
                RandomRange(left, right);

            GameObject prefab =
                valid[rng.Next(valid.Count)];

            GameObject fruit =
                Instantiate(
                    prefab,
                    new Vector3(x, y, z),
                    Quaternion.identity,
                    fruitSpawnPoint
                );


            Collider2D collider =
                fruit.GetComponent<Collider2D>();

            if (collider == null)
            {
                collider =
                    fruit.AddComponent<CircleCollider2D>();
            }

            collider.enabled = true;


            Rigidbody2D rb =
                fruit.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                rb =
                    fruit.AddComponent<Rigidbody2D>();
            }

            rb.bodyType =
                RigidbodyType2D.Kinematic;

            rb.gravityScale = 0f;

            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;

            rb.simulated = true;


            FruitData data = new FruitData();

            data.obj = fruit;

            // Shuru me left ya right, random
            data.direction =
                rng.Next(2) == 0
                    ? Vector2.left
                    : Vector2.right;

            data.speed =
                RandomRange(fruitSpeedMin, fruitSpeedMax);

            activeFruits.Add(data);
        }
    }


    // ============================================================
    // MOVE FRUITS
    // ============================================================

    private void MoveFruits()
    {
        if (activeFruits.Count == 0)
            return;

        if (levelCompleted)
            return;

        // Game over / win ke time fruits ruke rahe (revive par wahin se continue)
        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        Vector2 min;
        Vector2 max;

        if (!GetScreenBounds(out min, out max))
            return;


        float leftLimit = min.x + screenPadding;
        float rightLimit = max.x - screenPadding;


        for (int i = 0; i < activeFruits.Count; i++)
        {
            FruitData data = activeFruits[i];

            if (data.obj == null)
                continue;


            Transform t = data.obj.transform;

            t.position +=
                (Vector3)(
                    data.direction *
                    data.speed *
                    Time.deltaTime
                );


            // Fruit destroy nahi hota: kinare par pahunchte hi wapas mud jata hai
            // (left -> right aur right -> left, jab tak kat na jaye)
            Vector3 pos = t.position;

            if (pos.x <= leftLimit)
            {
                pos.x = leftLimit;
                data.direction = Vector2.right;
            }
            else if (pos.x >= rightLimit)
            {
                pos.x = rightLimit;
                data.direction = Vector2.left;
            }

            t.position = pos;
        }
    }


    // ============================================================
    // STAGE CLEAR -> NEXT STAGE
    // ============================================================

    private IEnumerator NextStageRoutine()
    {
        stageTransitioning = true;

        if (nextStageDelay > 0f)
        {
            yield return new WaitForSeconds(nextStageDelay);
        }

        // Game over ho to revive tak ruko, win ho to band
        while (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            if (levelCompleted || gameManager.IsLevelWon())
            {
                stageTransitioning = false;
                yield break;
            }

            yield return null;
        }

        stageTransitioning = false;

        currentStage++;

        SpawnStage();
    }


    // ============================================================
    // CHECK FIRE INPUT
    // ============================================================

    private bool TryGetTapPosition(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;


        if (Touchscreen.current != null)
        {
            if (
                Touchscreen.current.primaryTouch.press
                    .wasPressedThisFrame
            )
            {
                screenPosition =
                    Touchscreen.current.primaryTouch.position
                        .ReadValue();

                return true;
            }
        }


        if (Mouse.current != null)
        {
            if (
                Mouse.current.leftButton
                    .wasPressedThisFrame
            )
            {
                screenPosition =
                    Mouse.current.position.ReadValue();

                return true;
            }
        }


        return false;
    }


    // Sirf asli UI button (Selectable) par tap ho to true.
    // Background image / panel par tap hone se fire block nahi hota.
    private bool IsPointerOverUIButton(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;


        PointerEventData pointer =
            new PointerEventData(EventSystem.current);

        pointer.position = screenPosition;


        List<RaycastResult> results =
            new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, results);


        for (int i = 0; i < results.Count; i++)
        {
            if (
                results[i].gameObject != null &&
                results[i].gameObject
                    .GetComponentInParent<Selectable>() != null
            )
            {
                return true;
            }
        }


        return false;
    }


    private void CheckForFireInput()
    {
        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        Vector2 tapScreenPosition;

        if (!TryGetTapPosition(out tapScreenPosition))
            return;


        // UI button (menu, settings, etc.) dabane par boomerang fire na ho
        if (ignoreTapsOnUI && IsPointerOverUIButton(tapScreenPosition))
            return;


        FireReadyBoomerang(tapScreenPosition);
    }


    // ============================================================
    // CREATE READY BOOMERANG
    // ============================================================

    private void CreateReadyBoomerang()
    {
        if (boomerangPrefab == null)
        {
            Debug.LogWarning(
                "Timing_Challenge2: Boomerang Prefab is NOT assigned."
            );

            return;
        }


        if (boomerangSpawnPoint == null)
        {
            Debug.LogWarning(
                "Timing_Challenge2: Boomerang Spawn Point is NOT assigned."
            );

            return;
        }


        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        if (oneBoomerangAtATime && currentBoomerang != null)
        {
            return;
        }


        currentBoomerangHitFruit = false;

        currentHitPositions.Clear();


        currentBoomerang =
            Instantiate(
                boomerangPrefab,
                boomerangSpawnPoint.position,
                boomerangSpawnPoint.rotation
            );


        currentBoomerang.SetActive(true);


        // Prefab par purana KnifeController2 ho to hata do (conflict se bachne ke liye)
        KnifeController2 oldKnife2 =
            currentBoomerang.GetComponent<KnifeController2>();

        if (oldKnife2 != null)
        {
            oldKnife2.enabled = false;
            Destroy(oldKnife2);
        }


        // Prefab par agar original KnifeController laga ho to usse hata do (conflict se bachne ke liye)
        KnifeController originalController =
            currentBoomerang.GetComponent<KnifeController>();

        if (originalController != null)
        {
            originalController.enabled = false;
            Destroy(originalController);
        }


        SpriteRenderer[] renderers =
            currentBoomerang.GetComponentsInChildren<SpriteRenderer>(
                true
            );


        foreach (
            SpriteRenderer renderer
            in renderers
        )
        {
            renderer.enabled = true;
            renderer.sortingOrder = 100;
        }


        Collider2D[] colliders =
            currentBoomerang.GetComponentsInChildren<Collider2D>(
                true
            );


        foreach (
            Collider2D collider
            in colliders
        )
        {
            collider.enabled = true;

            collider.isTrigger = true;
        }


        Rigidbody2D rb =
            currentBoomerang.GetComponent<Rigidbody2D>();


        if (rb == null)
        {
            rb =
                currentBoomerang.AddComponent<Rigidbody2D>();
        }


        rb.bodyType =
            RigidbodyType2D.Kinematic;

        rb.gravityScale = 0f;

        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity = 0f;

        rb.simulated = true;


        TimingBoomerang controller =
            currentBoomerang.GetComponent<TimingBoomerang>();


        if (controller == null)
        {
            controller =
                currentBoomerang.AddComponent<TimingBoomerang>();
        }


        controller.enabled = true;


        controller.moveSpeed =
            boomerangSpeed;

        controller.spinSpeed =
            boomerangSpinSpeed;

        controller.range =
            boomerangRange;

        controller.readyMoveSpeed =
            boomerangReadyMoveSpeed;

        controller.startOffset =
            boomerangStartOffset;

        controller.gameManager =
            this;


        controller.InitializeReady(
            boomerangSpawnPoint.position
        );
    }


    // ============================================================
    // FIRE READY BOOMERANG (tap kahin bhi, seedha upar)
    // ============================================================

    private void FireReadyBoomerang(Vector2 tapScreenPosition)
    {
        if (currentBoomerang == null)
            return;


        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        // Khali screen par fire karke game over na ho
        if (fireOnlyWhenFruitPresent && activeFruits.Count == 0)
            return;


        TimingBoomerang controller =
            currentBoomerang.GetComponent<TimingBoomerang>();


        if (controller == null)
            return;


        // Tap kahin bhi ho, boomerang hamesha seedha upar jayega
        if (controller.Fire(Vector2.up))
        {
            PlayFireSound();
        }
    }


    // ============================================================
    // FIRE SOUND
    // ============================================================

    private void PlayFireSound()
    {
        if (fireClip == null)
            return;


        // Settings me Sound OFF hai to kuch na bajao
        if (!GameAudio.SoundEnabled)
            return;


        if (audioSource != null)
        {
            audioSource.PlayOneShot(
                fireClip,
                fireVolume
            );
        }
        else if (Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(
                fireClip,
                Camera.main.transform.position,
                fireVolume
            );
        }
    }


    // ============================================================
    // BOOMERANG FINISHED (wapas ready position par aa gaya)
    // ============================================================

    public void BoomerangFinished(
        TimingBoomerang boomerang)
    {
        if (boomerang == null)
            return;


        if (
            currentBoomerang !=
            boomerang.gameObject
        )
        {
            return;
        }


        bool missed =
            !currentBoomerangHitFruit;


        currentBoomerang = null;


        if (missed)
        {
            if (gameManager != null)
            {
                gameManager.ShowGameOver(FruitMissReason);
            }
            else
            {
                Debug.LogWarning(
                    "Timing_Challenge2: Cannot show Game Over because Timing_Challenge is not assigned."
                );
            }


            return;
        }


        if (levelCompleted)
            return;


        CreateReadyBoomerang();
    }


    // ============================================================
    // FRUIT HIT
    // ============================================================

    public void HitFruit(
        GameObject fruit)
    {
        if (fruit == null || levelCompleted || timeUp)
            return;


        // Time khatam / game over ke baad koi hit count nahi hoga
        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        // Collider child par ho sakta hai, isliye child bhi chalega
        FruitData hit = null;

        for (int i = 0; i < activeFruits.Count; i++)
        {
            GameObject o = activeFruits[i].obj;

            if (
                o != null &&
                (fruit == o || fruit.transform.IsChildOf(o.transform))
            )
            {
                hit = activeFruits[i];
                break;
            }
        }

        if (hit == null)
            return;


        GameObject cutFruit = hit.obj;

        activeFruits.Remove(hit);


        currentBoomerangHitFruit = true;


        // Flying number ke liye hit position (fruit destroy hone se pehle)
        RegisterHit(cutFruit.transform.position);


        destroyedLevelFruits++;

        UpdateLevelProgress();


        // Turant destroy nahi: Fruit.Cut() colliders/visible band karke
        // effect + sound chalayega aur delay ke baad khud destroy karega
        Fruit fruitScript =
            cutFruit.GetComponent<Fruit>();

        if (fruitScript != null)
        {
            fruitScript.Cut();
        }
        else
        {
            Destroy(cutFruit);
        }


        // Is stage ke saare fruits kat gaye
        if (activeFruits.Count == 0)
        {
            if (currentStage + 1 >= stagesPerLevel)
            {
                // Aakhri stage clear = level clear (timer khatam hone se pehle)
                CompleteLevel();
            }
            else if (!stageTransitioning)
            {
                StartCoroutine(NextStageRoutine());
            }
        }
    }


    // ============================================================
    // UPDATE LEVEL PROGRESS
    // ============================================================

    private void UpdateLevelProgress()
    {
        if (totalLevelFruits <= 0)
            return;


        float progress =
            (float)destroyedLevelFruits /
            totalLevelFruits;


        float finalProgress =
            Mathf.Lerp(
                startingProgress,
                1f,
                progress
            );


        SetProgress(finalProgress);
    }


    // ============================================================
    // SET PROGRESS
    // ============================================================

    private void SetProgress(
        float value)
    {
        if (progressImage == null)
            return;


        progressImage.fillAmount =
            Mathf.Clamp01(value);
    }


    // ============================================================
    // LEVEL COMPLETE
    // ============================================================

    private void CompleteLevel()
    {
        if (levelCompleted)
            return;


        levelCompleted = true;
        UpdateLevelStatus(true);


        SetProgress(1f);


        if (gameManager != null)
        {
            gameManager.GiveLevelCompleteRewards();
            gameManager.ShowWin();
        }
    }


    // ============================================================
    // LEVEL TEXT
    // ============================================================

    private void SetLevelText(
        string text)
    {
        if (levelText != null)
        {
            levelText.text = text;
        }
    }


    // ============================================================
    // MULTI HIT FLYING NUMBERS
    // ============================================================

    private void RegisterHit(Vector3 worldPosition)
    {
        currentHitPositions.Add(worldPosition);

        if (!showMultiHitNumbers)
            return;


        // 1st fruit par +1, 2nd par +2, 3rd par +3 ...
        int hitCount =
            currentHitPositions.Count;

        SpawnHitNumber(worldPosition, hitCount);
    }


    private void SpawnHitNumber(
        Vector3 worldPosition,
        int number)
    {
        GameObject obj;

        TMP_Text text;

        Canvas uiCanvas = null;


        if (hitNumberPrefab != null)
        {
            obj =
                Instantiate(
                    hitNumberPrefab,
                    worldPosition,
                    Quaternion.identity
                );

            // Prefab inactive ho to bhi dikhe
            obj.SetActive(true);

            text =
                obj.GetComponentInChildren<TMP_Text>(true);


            if (text == null)
            {
                Debug.LogWarning(
                    "Timing_Challenge2: Hit Number Prefab me TextMeshPro nahi mila."
                );

                Destroy(obj);

                return;
            }


            if (text is TextMeshProUGUI)
            {
                // UI text bina Canvas ke screen par nahi dikhta,
                // isliye isse Canvas ke andar daalte hain.
                uiCanvas = GetHitNumberCanvas();

                if (uiCanvas == null)
                {
                    Debug.LogWarning(
                        "Timing_Challenge2: Hit Number Prefab UI hai par scene me Canvas nahi mila."
                    );

                    Destroy(obj);

                    return;
                }

                obj.transform.SetParent(
                    uiCanvas.transform,
                    false
                );

                obj.transform.SetAsLastSibling();

                SetUIPositionFromWorld(
                    obj.transform as RectTransform,
                    uiCanvas,
                    worldPosition
                );
            }
            else
            {
                // World text: fruits ke upar dikhe
                TextMeshPro worldPrefabText =
                    text as TextMeshPro;

                if (worldPrefabText != null)
                {
                    ApplyHitNumberSorting(worldPrefabText);
                }
            }
        }
        else
        {
            obj = new GameObject("HitNumber");

            obj.transform.position = worldPosition;

            TextMeshPro worldText =
                obj.AddComponent<TextMeshPro>();

            worldText.fontSize = hitNumberFontSize;
            worldText.fontStyle = FontStyles.Bold;
            worldText.alignment = TextAlignmentOptions.Center;
            worldText.color = hitNumberColor;

            ApplyHitNumberSorting(worldText);

            text = worldText;
        }


        // Prefab ka alpha 0 ho to bhi number dikhe
        Color startColor = text.color;

        if (startColor.a < 0.01f)
        {
            startColor.a = 1f;
            text.color = startColor;
        }


        text.text = "+" + number;


        StartCoroutine(
            FlyHitNumber(
                obj,
                text,
                worldPosition,
                uiCanvas
            )
        );
    }


    private void ApplyHitNumberSorting(TextMeshPro worldText)
    {
        worldText.sortingOrder = hitNumberSortingOrder;

        if (!string.IsNullOrEmpty(hitNumberSortingLayerName))
        {
            worldText.sortingLayerID =
                SortingLayer.NameToID(hitNumberSortingLayerName);
        }
    }


    private Canvas GetHitNumberCanvas()
    {
        Canvas canvas = hitNumberCanvas;

        if (canvas == null && levelText != null)
        {
            canvas = levelText.canvas;
        }

        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
            return null;

        return canvas.rootCanvas;
    }


    // World position ko Canvas ke andar UI position me badalta hai
    private void SetUIPositionFromWorld(
        RectTransform rect,
        Canvas canvas,
        Vector3 worldPosition)
    {
        if (rect == null || canvas == null)
            return;


        Camera worldCamera = Camera.main;

        if (worldCamera == null)
            worldCamera = canvas.worldCamera;

        if (worldCamera == null)
            return;


        Vector2 screenPoint =
            worldCamera.WorldToScreenPoint(worldPosition);


        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rect.position = screenPoint;
        }
        else
        {
            Vector3 canvasPoint;

            if (
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPoint,
                    canvas.worldCamera,
                    out canvasPoint
                )
            )
            {
                rect.position = canvasPoint;
            }
        }
    }


    private IEnumerator FlyHitNumber(
        GameObject obj,
        TMP_Text text,
        Vector3 startWorld,
        Canvas uiCanvas)
    {
        float duration =
            Mathf.Max(0.05f, hitNumberDuration);

        Vector3 endWorld =
            startWorld +
            Vector3.up * hitNumberFlyDistance;

        Color baseColor =
            text.color;

        float timer = 0f;


        while (timer < duration)
        {
            if (obj == null)
                yield break;


            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / duration);


            // Upar ud ta hua (ease out)
            float eased =
                1f - Mathf.Pow(1f - t, 2f);

            Vector3 currentWorld =
                Vector3.Lerp(
                    startWorld,
                    endWorld,
                    eased
                );


            if (uiCanvas != null)
            {
                SetUIPositionFromWorld(
                    obj.transform as RectTransform,
                    uiCanvas,
                    currentWorld
                );
            }
            else
            {
                obj.transform.position = currentWorld;
            }


            // Aakhri half me fade out
            float alpha =
                t < 0.5f
                    ? 1f
                    : 1f - ((t - 0.5f) / 0.5f);

            text.color =
                new Color(
                    baseColor.r,
                    baseColor.g,
                    baseColor.b,
                    baseColor.a * alpha
                );


            yield return null;
        }


        if (obj != null)
        {
            Destroy(obj);
        }
    }


    // ============================================================
    // ON DESTROY
    // ============================================================

    private void OnDestroy()
    {
        if (currentBoomerang != null)
        {
            Destroy(currentBoomerang);
        }
    }


    // ============================================================
    // UPDATE LEVEL STATUS
    // ============================================================

    private void UpdateLevelStatus(bool completed)
    {
        if (gameManager == null)
            return;

        gameManager.UpdateLevelStatusText(
            GetLevelNumber(),
            completed
        );
    }


    // ============================================================
    // LEVEL NUMBER (Timing_Challenge ke gameId se)
    // ============================================================

    private int GetLevelNumber()
    {
        if (gameManager != null)
            return gameManager.GetCurrentLevel();

        return 1;
    }


    // ============================================================
    // REVIVE (naam ReviveKnife hi rakha hai, Timing_Challenge isi ko call karta hai)
    // Same stage, same fruits, bas naya boomerang ready
    // Time khatam hone par game over hua tha to reviveExtraTime wapas milega
    // ============================================================

    public void ReviveKnife()
    {
        if (levelCompleted)
            return;


        if (timeUp)
        {
            timeUp = false;

            timeLeft = Mathf.Max(1f, reviveExtraTime);

            UpdateTimerText();
        }

        // Agar koi purana boomerang bacha ho to hata do
        if (currentBoomerang != null)
        {
            Destroy(currentBoomerang);
            currentBoomerang = null;
        }

        CreateReadyBoomerang();
    }
}