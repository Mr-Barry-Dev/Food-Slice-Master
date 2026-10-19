using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OtherGameManager2 : MonoBehaviour
{
    // ============================================================
    // FORMATION TYPES
    // ============================================================

    private enum FormationType
    {
        Circle,
        Square,
        Triangle,
        Rectangle
    }

    public enum TestShape
    {
        Circle,
        Square,
        Triangle,
        Rectangle
    }


    // ============================================================
    // CIRCLE SETTINGS
    // ============================================================

    [System.Serializable]
    public class CircleSettings
    {
        [Header("Fruit Count")]
        public int fruitCount = 8;

        [Header("Circle Radius")]
        public float radius = 2f;

        [Header("Fruit Gap")]
        public float fruitGap = 0.2f;
    }


    // ============================================================
    // SQUARE SETTINGS
    // ============================================================

    [System.Serializable]
    public class SquareSettings
    {
        [Header("Fruit Count")]
        public int fruitCount = 9;

        [Header("Fruit Gap")]
        public float fruitGap = 0.5f;
    }


    // ============================================================
    // TRIANGLE SETTINGS
    // ============================================================

    [System.Serializable]
    public class TriangleSettings
    {
        [Header("Fruit Count")]
        public int fruitCount = 6;

        [Header("Fruit Gap")]
        public float fruitGap = 0.5f;
    }


    // ============================================================
    // RECTANGLE SETTINGS
    // ============================================================

    [System.Serializable]
    public class RectangleSettings
    {
        [Header("Fruit Count")]
        public int fruitCount = 6;

        [Header("Fruit Gap")]
        public float fruitGap = 0.5f;
    }


    // ============================================================
    // UI
    // ============================================================

    [Header("===== UI =====")]

    public TMP_Text levelText;

    public Image progressImage;


    // ============================================================
    // GAME MANAGER
    // ============================================================

    [Header("===== GAME STATE MANAGER =====")]

    [Tooltip("Main OtherGameManager which handles Win and Game Over.")]
    public OtherGameManager gameManager;


    // ============================================================
    // FRUIT CENTER
    // ============================================================

    [Header("===== FRUIT CENTER =====")]

    [Tooltip("Fixed center point for every fruit formation.")]
    public Transform fruitSpawnPoint;


    // ============================================================
    // COMMON FRUITS
    // ============================================================

    [Header("===== COMMON FRUITS =====")]

    [Tooltip("Sabhi shapes ke liye common fruit prefabs.")]
    public GameObject[] commonFruitPrefabs;

    [Tooltip("Ek shape me ek hi fruit maximum kitni baar aa sakta hai.")]
    public int maxSameFruitPerShape = 3;


    // ============================================================
    // KNIFE
    // ============================================================

    [Header("===== KNIFE =====")]

    [Tooltip("Knife prefab.")]
    public GameObject knifePrefab;

    [Tooltip("Point where knife becomes ready.")]
    public Transform knifeSpawnPoint;

    [Tooltip("Knife upward movement speed.")]
    public float knifeMoveSpeed = 12f;

    [Tooltip("Knife rotation speed while firing.")]
    public float knifeRotationSpeed = 720f;

    [Tooltip("Knife is destroyed after reaching this Y.")]
    public float knifeDestroyY = 8f;

    [Tooltip("Distance below spawn point where knife starts.")]
    public float knifeStartOffset = 1.5f;

    [Tooltip("Speed at which knife moves to ready position.")]
    public float knifeReadyMoveSpeed = 8f;

    [Tooltip("Only one knife exists at a time.")]
    public bool oneKnifeAtATime = true;


    // ============================================================
    // SOUND
    // ============================================================

    [Header("===== SOUND =====")]

    [Tooltip("Knife fire hone par ye sound bajega.")]
    public AudioClip knifeFireClip;

    [Range(0f, 1f)]
    public float knifeFireVolume = 1f;

    [Tooltip("Optional. Khali chhodo to Main Camera ki position par play hoga.")]
    public AudioSource audioSource;

    // Ek hi knife ke liye sound do baar na baje
    private GameObject lastFiredKnife;


    // ============================================================
    // SHAPE SETTINGS
    // ============================================================

    [Header("===== CIRCLE =====")]

    public CircleSettings circleSettings;


    [Header("===== SQUARE =====")]

    public SquareSettings squareSettings;


    [Header("===== TRIANGLE =====")]

    public TriangleSettings triangleSettings;


    [Header("===== RECTANGLE =====")]

    public RectangleSettings rectangleSettings;


    // ============================================================
    // TEST MODE
    // ============================================================

    [Header("===== TEST MODE =====")]

    public bool testMode = false;

    public TestShape testShape = TestShape.Circle;


    // ============================================================
    // SHAPE ROTATION
    // ============================================================

    [Header("===== SHAPE ROTATION =====")]

    public BossRotation shapeRotation;


    // ============================================================
    // SHAPE ORDER PER LEVEL
    // ============================================================

    [Header("===== SHAPE ORDER PER LEVEL =====")]

    [Tooltip("ON: har level ka shapes ka order alag (random) hoga, lekin us level me hamesha same rahega (retry/revive par bhi). OFF: Circle, Square, Triangle, Rectangle.")]
    public bool randomShapeOrderPerLevel = true;

    [Tooltip("Is level tak (including) 3 shapes aayenge.")]
    public int threeShapesUntilLevel = 5;

    [Tooltip("Is level tak (including) 4 shapes aayenge.")]
    public int fourShapesUntilLevel = 15;

    [Tooltip("Is level tak (including) 5 shapes aayenge. Iske baad 6 shapes.")]
    public int fiveShapesUntilLevel = 25;


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

    [Tooltip("Number ki sorting order (fruits aur knife ke upar dikhne ke liye).")]
    public int hitNumberSortingOrder = 200;

    [Tooltip("Optional: fruits/knife jis Sorting Layer par hain uska naam (khali = Default).")]
    public string hitNumberSortingLayerName = "";

    [Tooltip("Optional: agar prefab UI (TextMeshPro - Text (UI)) hai to ye Canvas use hoga. Khali chhodo to Level Text wala Canvas use hoga.")]
    public Canvas hitNumberCanvas;


    // ============================================================
    // NEXT SHAPE DELAY
    // ============================================================

    [Header("===== NEXT SHAPE DELAY =====")]

    [Tooltip("Current shape complete hone ke baad next shape show hone ka delay.")]
    [Range(1f, 2f)]
    public float nextShapeDelay = 1.5f;


    // ============================================================
    // INTERNAL VARIABLES
    // ============================================================

    private HashSet<GameObject> activeFruits =
        new HashSet<GameObject>();

    private int currentPhase = 0;

    // Is level ke shapes ka order
    private FormationType[] phaseOrder = new FormationType[]
    {
        FormationType.Circle,
        FormationType.Square,
        FormationType.Triangle,
        FormationType.Rectangle
    };

    private int remainingFruits = 0;

    private int totalLevelFruits = 0;

    private int destroyedLevelFruits = 0;

    private int totalTestFruits = 0;

    private int destroyedTestFruits = 0;

    private bool levelCompleted = false;

    private bool testCompleted = false;


    // ============================================================
    // CURRENT KNIFE
    // ============================================================

    private GameObject currentKnife;

    private bool currentKnifeHitFruit = false;

    // Is knife ne jin fruits ko hit kiya unki world positions
    private List<Vector3> currentKnifeHitPositions =
        new List<Vector3>();

    private int currentLevelNumber = 1;


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (fruitSpawnPoint == null)
        {
            Debug.LogError(
                "OtherGameManager2: Fruit Spawn Point is NOT assigned."
            );

            return;
        }


        if (gameManager == null)
        {
            Debug.LogWarning(
                "OtherGameManager2: OtherGameManager is NOT assigned."
            );
        }


        // Level number aur is level ke shapes ka order pehle decide hota hai
        currentLevelNumber = GetLevelNumber();

        BuildShapeOrder();


        if (testMode)
        {
            CalculateTestFruits();
        }
        else
        {
            CalculateTotalLevelFruits();
        }


        destroyedLevelFruits = 0;
        destroyedTestFruits = 0;


        SetProgress(startingProgress);


        StartCurrentPhase();


        CreateReadyKnife();


        SetLevelText("Level " + currentLevelNumber);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        CheckForFireInput();
    }


    // ============================================================
    // CHECK FIRE INPUT
    // ============================================================

    private void CheckForFireInput()
    {
        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        bool tapped = false;


        if (Touchscreen.current != null)
        {
            if (
                Touchscreen.current.primaryTouch.press
                    .wasPressedThisFrame
            )
            {
                tapped = true;
            }
        }


        if (Mouse.current != null)
        {
            if (
                Mouse.current.leftButton
                    .wasPressedThisFrame
            )
            {
                tapped = true;
            }
        }


        if (!tapped)
            return;


        FireReadyKnife();
    }


    // ============================================================
    // CREATE READY KNIFE
    // ============================================================

    private void CreateReadyKnife()
    {
        if (knifePrefab == null)
        {
            Debug.LogWarning(
                "OtherGameManager2: Knife Prefab is NOT assigned."
            );

            return;
        }


        if (knifeSpawnPoint == null)
        {
            Debug.LogWarning(
                "OtherGameManager2: Knife Spawn Point is NOT assigned."
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


        if (oneKnifeAtATime && currentKnife != null)
        {
            return;
        }


        currentKnifeHitFruit = false;

        currentKnifeHitPositions.Clear();


        currentKnife =
            Instantiate(
                knifePrefab,
                knifeSpawnPoint.position,
                knifeSpawnPoint.rotation
            );


        currentKnife.SetActive(true);


        // Prefab par agar original KnifeController laga ho to usse hata do (conflict se bachne ke liye)
        KnifeController originalController =
            currentKnife.GetComponent<KnifeController>();

        if (originalController != null)
        {
            originalController.enabled = false;
            Destroy(originalController);
        }


        SpriteRenderer[] renderers =
            currentKnife.GetComponentsInChildren<SpriteRenderer>(
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
            currentKnife.GetComponentsInChildren<Collider2D>(
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
            currentKnife.GetComponent<Rigidbody2D>();


        if (rb == null)
        {
            rb =
                currentKnife.AddComponent<Rigidbody2D>();
        }


        rb.bodyType =
            RigidbodyType2D.Kinematic;

        rb.gravityScale = 0f;

        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity = 0f;

        rb.simulated = true;


        KnifeController2 controller =
            currentKnife.GetComponent<KnifeController2>();


        if (controller == null)
        {
            controller =
                currentKnife.AddComponent<KnifeController2>();
        }


        controller.enabled = true;


        controller.moveSpeed =
            knifeMoveSpeed;

        controller.rotationSpeed =
            knifeRotationSpeed;

        controller.destroyY =
            knifeDestroyY;

        controller.readyMoveSpeed =
            knifeReadyMoveSpeed;

        controller.startOffset =
            knifeStartOffset;

        controller.gameManager =
            this;


        controller.InitializeReadyKnife(
            knifeSpawnPoint.position
        );
    }


    // ============================================================
    // FIRE READY KNIFE
    // ============================================================

    private void FireReadyKnife()
    {
        if (currentKnife == null)
            return;


        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        KnifeController2 controller =
            currentKnife.GetComponent<KnifeController2>();


        if (controller == null)
            return;


        controller.Fire();


        // Sound sirf pehli baar fire hone par (har knife ke liye ek baar)
        if (lastFiredKnife != currentKnife)
        {
            lastFiredKnife = currentKnife;

            PlayKnifeFireSound();
        }
    }


    // ============================================================
    // KNIFE FIRE SOUND
    // ============================================================

    private void PlayKnifeFireSound()
    {
        if (knifeFireClip == null)
            return;


        // Settings me Sound OFF hai to kuch na bajao
        if (!GameAudio.SoundEnabled)
            return;


        if (audioSource != null)
        {
            audioSource.PlayOneShot(
                knifeFireClip,
                knifeFireVolume
            );
        }
        else if (Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(
                knifeFireClip,
                Camera.main.transform.position,
                knifeFireVolume
            );
        }
    }


    // ============================================================
    // KNIFE FINISHED
    // ============================================================

    public void KnifeFinished(
        KnifeController2 knife)
    {
        if (knife == null)
            return;


        if (
            currentKnife !=
            knife.gameObject
        )
        {
            return;
        }


        bool knifeMissed =
            !currentKnifeHitFruit;


        currentKnife = null;


        if (knifeMissed)
        {
            if (gameManager != null)
            {
                gameManager.ShowGameOver();
            }
            else
            {
                Debug.LogWarning(
                    "OtherGameManager2: Cannot show Game Over because OtherGameManager is not assigned."
                );
            }


            return;
        }


        if (levelCompleted)
            return;


        CreateReadyKnife();
    }


    // ============================================================
    // CALCULATE TOTAL LEVEL FRUITS
    // ============================================================

    private void CalculateTotalLevelFruits()
    {
        totalLevelFruits = 0;


        // Sirf is level ke shapes (repeat bhi count honge)
        for (int i = 0; i < phaseOrder.Length; i++)
        {
            totalLevelFruits +=
                GetFruitCount(phaseOrder[i]);
        }


        if (totalLevelFruits <= 0)
        {
            totalLevelFruits = 1;
        }
    }


    // ============================================================
    // FRUIT COUNT OF A SHAPE
    // ============================================================

    private int GetFruitCount(FormationType formation)
    {
        switch (formation)
        {
            case FormationType.Circle:
                return Mathf.Max(0, circleSettings.fruitCount);

            case FormationType.Square:
                return Mathf.Max(0, squareSettings.fruitCount);

            case FormationType.Triangle:
                return Mathf.Max(0, triangleSettings.fruitCount);

            case FormationType.Rectangle:
                return Mathf.Min(
                    Mathf.Max(0, rectangleSettings.fruitCount),
                    12
                );
        }

        return 0;
    }


    // ============================================================
    // CALCULATE TEST FRUITS
    // ============================================================

    private void CalculateTestFruits()
    {
        switch (testShape)
        {
            case TestShape.Circle:

                totalTestFruits =
                    Mathf.Max(
                        1,
                        circleSettings.fruitCount
                    );

                break;


            case TestShape.Square:

                totalTestFruits =
                    Mathf.Max(
                        1,
                        squareSettings.fruitCount
                    );

                break;


            case TestShape.Triangle:

                totalTestFruits =
                    Mathf.Max(
                        1,
                        triangleSettings.fruitCount
                    );

                break;


            case TestShape.Rectangle:

                totalTestFruits =
                    Mathf.Clamp(
                        rectangleSettings.fruitCount,
                        1,
                        12
                    );

                break;
        }
    }


    // ============================================================
    // FRUIT HIT
    // ============================================================

    public void HitFruit(
        GameObject fruit)
    {
        if (fruit == null)
            return;


        if (!activeFruits.Contains(fruit))
            return;


        currentKnifeHitFruit = true;


        // Flying number ke liye hit position (fruit destroy hone se pehle)
        RegisterKnifeHit(fruit.transform.position);


        activeFruits.Remove(fruit);

        remainingFruits--;


        if (testMode)
        {
            destroyedTestFruits++;

            UpdateTestProgress();
        }
        else
        {
            destroyedLevelFruits++;

            UpdateLevelProgress();
        }


        // Turant destroy nahi: Fruit.Cut() colliders/visible band karke
        // effect + sound chalayega aur delay ke baad khud destroy karega
        Fruit fruitScript =
            fruit.GetComponent<Fruit>();

        if (fruitScript != null)
        {
            fruitScript.Cut();
        }
        else
        {
            Destroy(fruit);
        }


        if (remainingFruits <= 0)
        {
            CompleteCurrentPhase();
        }
    }


    // ============================================================
    // UPDATE NORMAL LEVEL PROGRESS
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
    // UPDATE TEST PROGRESS
    // ============================================================

    private void UpdateTestProgress()
    {
        if (totalTestFruits <= 0)
            return;


        float progress =
            (float)destroyedTestFruits /
            totalTestFruits;


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
    // START CURRENT PHASE
    // ============================================================

    private void StartCurrentPhase()
    {
        if (levelCompleted)
            return;


        ClearExistingFruits();


        if (testMode)
        {
            StartTestShape();

            return;
        }


        if (currentPhase >= phaseOrder.Length)
        {
            CompleteLevel();

            return;
        }


        FormationType formation =
            phaseOrder[currentPhase];


        UpdateLevelStatus(false);
        StartNormalFormation(
            formation
        );
    }


    // ============================================================
    // NORMAL FORMATION
    // ============================================================

    private void StartNormalFormation(FormationType formationType)
    {
        SpawnFormation(formationType);
    }


    // ============================================================
    // TEST SHAPE
    // ============================================================

    private void StartTestShape()
    {
        testCompleted = false;
        SpawnFormation((FormationType)(int)testShape);
    }


    // ============================================================
    // SPAWN FORMATION
    // ============================================================

    private void SpawnFormation(
        FormationType formationType)
    {
        if (fruitSpawnPoint == null)
        {
            Debug.LogError(
                "OtherGameManager2: Fruit Spawn Point is missing."
            );

            return;
        }


        List<Vector3> positions =
            new List<Vector3>();


        if (
            formationType ==
            FormationType.Circle
        )
        {
            positions =
                GenerateCirclePositions(
                    circleSettings.fruitCount,
                    circleSettings.radius,
                    circleSettings.fruitGap
                );


            SpawnFruits(positions);
        }
        else if (
            formationType ==
            FormationType.Square
        )
        {
            positions =
                GenerateSquarePositions(
                    squareSettings.fruitCount,
                    squareSettings.fruitGap
                );


            SpawnFruits(positions);
        }
        else if (
            formationType ==
            FormationType.Triangle
        )
        {
            positions =
                GenerateTrianglePositions(
                    triangleSettings.fruitCount,
                    triangleSettings.fruitGap
                );


            SpawnFruits(positions);
        }
        else if (
            formationType ==
            FormationType.Rectangle
        )
        {
            positions =
                GenerateRectanglePositions(
                    rectangleSettings.fruitCount,
                    rectangleSettings.fruitGap
                );


            SpawnFruits(positions);
        }


        if (shapeRotation != null)
        {
            shapeRotation.SetTarget(
                fruitSpawnPoint
            );
        }
    }


    // ============================================================
    // CIRCLE POSITIONS
    // ============================================================

    private List<Vector3> GenerateCirclePositions(
        int count,
        float radius,
        float fruitGap)
    {
        List<Vector3> positions =
            new List<Vector3>();


        if (count <= 0)
            return positions;


        if (radius <= 0f)
        {
            radius = 1f;
        }


        float circumference =
            Mathf.PI *
            2f *
            radius;


        float minimumSpacing =
            fruitGap + 0.5f;


        float requiredCircumference =
            count *
            minimumSpacing;


        if (
            requiredCircumference >
            circumference
        )
        {
            radius =
                requiredCircumference /
                (Mathf.PI * 2f);
        }


        for (
            int i = 0;
            i < count;
            i++
        )
        {
            float angle =
                (360f / count) *
                i;


            float radians =
                angle *
                Mathf.Deg2Rad;


            float x =
                Mathf.Cos(radians) *
                radius;


            float y =
                Mathf.Sin(radians) *
                radius;


            positions.Add(
                new Vector3(
                    x,
                    y,
                    0f
                )
            );
        }


        return positions;
    }


    // ============================================================
    // SQUARE POSITIONS
    // ============================================================

    private List<Vector3> GenerateSquarePositions(
        int count,
        float fruitGap)
    {
        List<Vector3> positions =
            new List<Vector3>();


        if (count <= 0)
            return positions;


        int side =
            Mathf.CeilToInt(
                Mathf.Sqrt(count)
            );


        for (
            int i = 0;
            i < count;
            i++
        )
        {
            int row =
                i / side;


            int column =
                i % side;


            Vector3 position =
                GridPosition(
                    column,
                    row,
                    side,
                    side,
                    fruitGap
                );


            positions.Add(
                position
            );
        }


        return positions;
    }


    // ============================================================
    // TRIANGLE POSITIONS
    // ============================================================

    private List<Vector3> GenerateTrianglePositions(
        int count,
        float fruitGap)
    {
        List<Vector3> positions =
            new List<Vector3>();


        if (count <= 0)
            return positions;


        int currentRow = 1;


        while (
            positions.Count +
            currentRow <= count
        )
        {
            int rowIndex =
                currentRow - 1;


            for (
                int column = 0;
                column < currentRow;
                column++
            )
            {
                float x =
                    (
                        column -
                        (currentRow - 1) / 2f
                    ) *
                    fruitGap;


                float y =
                    rowIndex *
                    fruitGap;


                positions.Add(
                    new Vector3(
                        x,
                        y,
                        0f
                    )
                );
            }


            currentRow++;
        }


        int remaining =
            count -
            positions.Count;


        if (remaining > 0)
        {
            int rowIndex =
                currentRow - 1;


            for (
                int column = 0;
                column < remaining;
                column++
            )
            {
                float x =
                    (
                        column -
                        (remaining - 1) / 2f
                    ) *
                    fruitGap;


                float y =
                    rowIndex *
                    fruitGap;


                positions.Add(
                    new Vector3(
                        x,
                        y,
                        0f
                    )
                );
            }
        }


        // CENTER TRIANGLE VERTICALLY

        float minY =
            float.MaxValue;


        float maxY =
            float.MinValue;


        foreach (
            Vector3 position
            in positions
        )
        {
            if (position.y < minY)
            {
                minY = position.y;
            }


            if (position.y > maxY)
            {
                maxY = position.y;
            }
        }


        float centerY =
            (minY + maxY) /
            2f;


        for (
            int i = 0;
            i < positions.Count;
            i++
        )
        {
            Vector3 p =
                positions[i];


            p.y -= centerY;


            positions[i] = p;
        }


        return positions;
    }


    // ============================================================
    // RECTANGLE POSITIONS
    // ============================================================

    private List<Vector3> GenerateRectanglePositions(
        int count,
        float gap)
    {
        List<Vector3> positions =
            new List<Vector3>();


        if (count <= 0)
            return positions;


        gap =
            Mathf.Max(
                0.01f,
                gap
            );


        int fruitsToSpawn =
            Mathf.Min(
                count,
                12
            );


        // count ke hisab se columns/rows: 6 -> 3x2, 8 -> 4x2, 12 -> 4x3
        int columns =
            Mathf.CeilToInt(
                Mathf.Sqrt(fruitsToSpawn * 4f / 3f)
            );

        int rows =
            Mathf.CeilToInt(
                (float)fruitsToSpawn / columns
            );


        for (
            int row = 0;
            row < rows;
            row++
        )
        {
            for (
                int column = 0;
                column < columns;
                column++
            )
            {
                if (
                    positions.Count >=
                    fruitsToSpawn
                )
                {
                    break;
                }


                positions.Add(
                    GridPosition(
                        column,
                        row,
                        columns,
                        rows,
                        gap
                    )
                );
            }
        }


        return positions;
    }


    // ============================================================
    // GRID POSITION
    // ============================================================

    private Vector3 GridPosition(
        int column,
        int row,
        int columns,
        int rows,
        float gap)
    {
        float x =
            (
                column -
                (columns - 1) / 2f
            ) *
            gap;


        float y =
            (
                row -
                (rows - 1) / 2f
            ) *
            gap;


        return new Vector3(
            x,
            y,
            0f
        );
    }


    // ============================================================
    // SPAWN FRUITS (common prefabs, ek shape me same fruit max N baar)
    // ============================================================

    private void SpawnFruits(
        List<Vector3> localPositions)
    {
        if (
            commonFruitPrefabs == null ||
            commonFruitPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "OtherGameManager2: Common Fruit Prefabs array is empty."
            );

            return;
        }


        if (fruitSpawnPoint == null)
            return;


        int limit =
            Mathf.Max(1, maxSameFruitPerShape);


        // Sirf valid (null nahi) prefabs ke index
        List<int> validIndexes =
            new List<int>();

        for (int i = 0; i < commonFruitPrefabs.Length; i++)
        {
            if (commonFruitPrefabs[i] != null)
            {
                validIndexes.Add(i);
            }
        }


        if (validIndexes.Count == 0)
        {
            Debug.LogWarning(
                "OtherGameManager2: Common Fruit Prefabs me koi valid prefab nahi hai."
            );

            return;
        }


        // Is shape me har prefab kitni baar use hua
        int[] usedCount =
            new int[commonFruitPrefabs.Length];


        remainingFruits = 0;


        for (
            int i = 0;
            i < localPositions.Count;
            i++
        )
        {
            // Wo prefabs jo abhi limit tak nahi pahunche
            List<int> available =
                new List<int>();

            foreach (int index in validIndexes)
            {
                if (usedCount[index] < limit)
                {
                    available.Add(index);
                }
            }


            int chosenIndex;

            if (available.Count > 0)
            {
                chosenIndex =
                    available[
                        Random.Range(0, available.Count)
                    ];
            }
            else
            {
                // Prefabs kam hain (fruitCount > prefabs * limit)
                // Fallback: sabse kam use hua prefab lo
                Debug.LogWarning(
                    "OtherGameManager2: Prefabs kam hain, limit " +
                    limit +
                    " follow nahi ho sakti. Common Fruit Prefabs badhao."
                );

                chosenIndex = validIndexes[0];

                foreach (int index in validIndexes)
                {
                    if (usedCount[index] < usedCount[chosenIndex])
                    {
                        chosenIndex = index;
                    }
                }
            }


            usedCount[chosenIndex]++;


            GameObject fruit =
                Instantiate(
                    commonFruitPrefabs[chosenIndex],
                    fruitSpawnPoint
                );


            fruit.transform.localPosition =
                localPositions[i];


            fruit.transform.localRotation =
                Quaternion.identity;


            fruit.transform.localScale =
                Vector3.one;


            Collider2D collider =
                fruit.GetComponent<Collider2D>();


            if (collider == null)
            {
                collider =
                    fruit.AddComponent<
                        CircleCollider2D
                    >();
            }


            collider.enabled = true;


            Rigidbody2D rb =
                fruit.GetComponent<Rigidbody2D>();


            if (rb == null)
            {
                rb =
                    fruit.AddComponent<
                        Rigidbody2D
                    >();
            }


            rb.bodyType =
                RigidbodyType2D.Kinematic;


            rb.gravityScale = 0f;


            rb.linearVelocity =
                Vector2.zero;


            rb.angularVelocity = 0f;


            rb.simulated = true;


            activeFruits.Add(
                fruit
            );

            remainingFruits++;
        }
    }


    // ============================================================
    // COMPLETE CURRENT PHASE
    // ============================================================

    private void CompleteCurrentPhase()
    {
        if (levelCompleted)
            return;


        if (testMode)
        {
            CompleteTestShape();

            return;
        }


        currentPhase++;


        if (currentPhase >= phaseOrder.Length)
        {
            CompleteLevel();

            return;
        }


        StartCoroutine(
            StartNextPhaseAfterDelay()
        );
    }


    // ============================================================
    // START NEXT PHASE AFTER DELAY
    // ============================================================

    private IEnumerator StartNextPhaseAfterDelay()
    {
        yield return new WaitForSeconds(
            nextShapeDelay
        );


        if (levelCompleted)
            yield break;


        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            yield break;
        }


        StartCurrentPhase();
    }


    // ============================================================
    // TEST COMPLETE
    // ============================================================

    private void CompleteTestShape()
    {
        if (testCompleted)
            return;


        testCompleted = true;

        levelCompleted = true;


        SetProgress(1f);


        if (levelText != null)
        {
            levelText.text =
                "TEST COMPLETE";
        }


        if (gameManager != null)
        {
            gameManager.ShowWin();
        }
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
    // CLEAR EXISTING FRUITS
    // ============================================================

    private void ClearExistingFruits()
    {
        if (shapeRotation != null)
        {
            shapeRotation.SetTarget(null);
        }


        if (fruitSpawnPoint != null)
        {
            for (
                int i =
                    fruitSpawnPoint.childCount - 1;
                i >= 0;
                i--
            )
            {
                Transform child =
                    fruitSpawnPoint.GetChild(i);


                if (child != null)
                {
                    Destroy(
                        child.gameObject
                    );
                }
            }
        }


        activeFruits.Clear();

        remainingFruits = 0;
    }


    // ============================================================
    // MULTI HIT FLYING NUMBERS
    // ============================================================

    private void RegisterKnifeHit(Vector3 worldPosition)
    {
        currentKnifeHitPositions.Add(worldPosition);

        if (!showMultiHitNumbers)
            return;


        // 1st fruit par +1, 2nd par +2, 3rd par +3 ...
        int hitCount =
            currentKnifeHitPositions.Count;

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
                    "OtherGameManager2: Hit Number Prefab me TextMeshPro nahi mila."
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
                        "OtherGameManager2: Hit Number Prefab UI hai par scene me Canvas nahi mila."
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
        if (currentKnife != null)
        {
            Destroy(currentKnife);
        }
    }


    // ============================================================
    // UPDATE LEVEL STATUS
    // ============================================================

    private void UpdateLevelStatus(bool completed)
    {
        if (gameManager == null)
            return;

        // Real level number gameId ke hisab se, shape index se nahi
        int levelNumber = GetLevelNumber();

        gameManager.UpdateLevelStatusText(
            levelNumber,
            completed
        );
    }


    // ============================================================
    // SHAPE COUNT FOR LEVEL
    // ============================================================

    private int GetShapeCountForLevel(int level)
    {
        if (level <= threeShapesUntilLevel)
            return 3;

        if (level <= fourShapesUntilLevel)
            return 4;

        if (level <= fiveShapesUntilLevel)
            return 5;

        return 6;
    }


    // ============================================================
    // BUILD SHAPE ORDER (level + gameId se fixed random)
    // ============================================================

    private void BuildShapeOrder()
    {
        FormationType[] order =
            GenerateOrder(currentLevelNumber, 0);


        // Pichle level jaisa same order na aaye
        if (currentLevelNumber > 1)
        {
            FormationType[] previous =
                GenerateOrder(currentLevelNumber - 1, 0);

            if (SameOrder(order, previous))
            {
                // Dusra fixed variation (ab bhi same level = same order)
                order =
                    GenerateOrder(currentLevelNumber, 1);
            }
        }


        phaseOrder = order;
    }


    private bool SameOrder(
        FormationType[] a,
        FormationType[] b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }


    private FormationType[] GenerateOrder(
        int level,
        int salt)
    {
        int count =
            GetShapeCountForLevel(level);


        FormationType[] all = new FormationType[]
        {
            FormationType.Circle,
            FormationType.Square,
            FormationType.Triangle,
            FormationType.Rectangle
        };


        if (!randomShapeOrderPerLevel)
        {
            // Fixed order: Circle, Square, Triangle, Rectangle, (repeat)
            FormationType[] fixedOrder =
                new FormationType[count];

            for (int i = 0; i < count; i++)
            {
                fixedOrder[i] = all[i % all.Length];
            }

            return fixedOrder;
        }


        // Fixed seed = gameId + level (+ salt), isliye same level me hamesha same order
        int seed = 17;

        string id =
            gameManager != null
                ? gameManager.gameId
                : "";

        for (int i = 0; i < id.Length; i++)
        {
            seed = unchecked(seed * 31 + id[i]);
        }

        seed = unchecked(seed * 31 + level);
        seed = unchecked(seed * 31 + salt);


        System.Random rng =
            new System.Random(seed);


        // Fisher-Yates shuffle
        for (int i = all.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            FormationType temp = all[i];
            all[i] = all[j];
            all[j] = temp;
        }


        FormationType[] order =
            new FormationType[count];


        for (int i = 0; i < count; i++)
        {
            if (i < all.Length)
            {
                // Pehle 4 tak shapes alag alag
                order[i] = all[i];
            }
            else
            {
                // 5th/6th shape: repeat, par lagatar same shape nahi
                FormationType pick;

                do
                {
                    pick = all[rng.Next(all.Length)];
                }
                while (pick == order[i - 1]);

                order[i] = pick;
            }
        }


        return order;
    }


    // ============================================================
    // LEVEL NUMBER (OtherGameManager ke gameId se)
    // ============================================================

    private int GetLevelNumber()
    {
        if (gameManager != null)
            return gameManager.GetCurrentLevel();

        return 1;
    }


    // ============================================================
    // REVIVE KNIFE
    // ============================================================

    public void ReviveKnife()
    {
        if (levelCompleted)
            return;

        // Agar koi purana knife bacha ho to hata do
        if (currentKnife != null)
        {
            Destroy(currentKnife);
            currentKnife = null;
        }

        CreateReadyKnife();
    }
}