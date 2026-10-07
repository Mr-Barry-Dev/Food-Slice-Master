using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager2 : MonoBehaviour
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
        [Header("Fruit Prefabs")]
        public GameObject[] fruitPrefabs;

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
        [Header("Fruit Prefabs")]
        public GameObject[] fruitPrefabs;

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
        [Header("Fruit Prefabs")]
        public GameObject[] fruitPrefabs;

        [Header("Fruit Count")]
        public int fruitCount = 10;

        [Header("Fruit Gap")]
        public float fruitGap = 0.5f;
    }


    // ============================================================
    // RECTANGLE SETTINGS
    // ============================================================

    [System.Serializable]
    public class RectangleSettings
    {
        [Header("Fruit Prefabs")]
        public GameObject[] fruitPrefabs;

        [Header("Fruit Count")]
        public int fruitCount = 12;

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

    [Tooltip("Main GameManager which handles Win and Game Over.")]
    public GameManager gameManager;


    // ============================================================
    // FRUIT CENTER
    // ============================================================

    [Header("===== FRUIT CENTER =====")]

    [Tooltip("Fixed center point for every fruit formation.")]
    public Transform fruitSpawnPoint;


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
    // PROGRESS SETTINGS
    // ============================================================

    [Header("===== PROGRESS =====")]

    [Range(0f, 1f)]
    public float startingProgress = 0.10f;


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


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        // --------------------------------------------------------
        // CHECK FRUIT CENTER
        // --------------------------------------------------------

        if (fruitSpawnPoint == null)
        {
            Debug.LogError(
                "GameManager2: Fruit Spawn Point is NOT assigned."
            );

            return;
        }


        // --------------------------------------------------------
        // CHECK GAME MANAGER
        // --------------------------------------------------------

        if (gameManager == null)
        {
            Debug.LogWarning(
                "GameManager2: GameManager is NOT assigned."
            );
        }


        // --------------------------------------------------------
        // CALCULATE FRUITS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // INITIAL PROGRESS
        // --------------------------------------------------------

        SetProgress(startingProgress);


        // --------------------------------------------------------
        // START FIRST FORMATION
        // --------------------------------------------------------

        StartCurrentPhase();


        // --------------------------------------------------------
        // CREATE FIRST READY KNIFE
        // --------------------------------------------------------

        CreateReadyKnife();
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
        // --------------------------------------------------------
        // GAME ENDED
        // --------------------------------------------------------

        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        bool tapped = false;


        // --------------------------------------------------------
        // TOUCH
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // MOUSE
        // --------------------------------------------------------

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
                "GameManager2: Knife Prefab is NOT assigned."
            );

            return;
        }


        if (knifeSpawnPoint == null)
        {
            Debug.LogWarning(
                "GameManager2: Knife Spawn Point is NOT assigned."
            );

            return;
        }


        // --------------------------------------------------------
        // GAME ENDED
        // --------------------------------------------------------

        if (
            gameManager != null &&
            gameManager.IsGameEnded()
        )
        {
            return;
        }


        // --------------------------------------------------------
        // ONLY ONE KNIFE
        // --------------------------------------------------------

        if (oneKnifeAtATime && currentKnife != null)
        {
            return;
        }


        // --------------------------------------------------------
        // RESET HIT STATE
        // --------------------------------------------------------

        currentKnifeHitFruit = false;


        // --------------------------------------------------------
        // CREATE KNIFE
        // --------------------------------------------------------

        currentKnife =
            Instantiate(
                knifePrefab,
                knifeSpawnPoint.position,
                knifeSpawnPoint.rotation
            );


        currentKnife.SetActive(true);


        // --------------------------------------------------------
        // FORCE SPRITE VISIBLE
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // COLLIDERS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // RIGIDBODY
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // GET CONTROLLER
        // --------------------------------------------------------

        KnifeController controller =
            currentKnife.GetComponent<KnifeController>();


        if (controller == null)
        {
            controller =
                currentKnife.AddComponent<KnifeController>();
        }


        controller.enabled = true;


        // --------------------------------------------------------
        // SEND SETTINGS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // START READY ANIMATION
        // --------------------------------------------------------

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


        KnifeController controller =
            currentKnife.GetComponent<KnifeController>();


        if (controller == null)
            return;


        controller.Fire();
    }


    // ============================================================
    // KNIFE FINISHED
    // ============================================================

    public void KnifeFinished(
        KnifeController knife)
    {
        if (knife == null)
            return;


        // --------------------------------------------------------
        // ONLY ACCEPT CURRENT KNIFE
        // --------------------------------------------------------

        if (
            currentKnife !=
            knife.gameObject
        )
        {
            return;
        }


        // --------------------------------------------------------
        // SAVE HIT STATE
        // --------------------------------------------------------

        bool knifeMissed =
            !currentKnifeHitFruit;


        // --------------------------------------------------------
        // CLEAR CURRENT KNIFE
        // --------------------------------------------------------

        currentKnife = null;


        // --------------------------------------------------------
        // GAME OVER IF NO FRUIT WAS HIT
        // --------------------------------------------------------

        if (knifeMissed)
        {
            if (gameManager != null)
            {
                gameManager.ShowGameOver();
            }
            else
            {
                Debug.LogWarning(
                    "GameManager2: Cannot show Game Over because GameManager is not assigned."
                );
            }


            return;
        }


        // --------------------------------------------------------
        // IF GAME IS COMPLETE
        // --------------------------------------------------------

        if (levelCompleted)
            return;


        // --------------------------------------------------------
        // CREATE NEXT READY KNIFE
        // --------------------------------------------------------

        CreateReadyKnife();
    }


    // ============================================================
    // CALCULATE TOTAL LEVEL FRUITS
    // ============================================================

    private void CalculateTotalLevelFruits()
    {
        int circleFruits =
            Mathf.Max(
                0,
                circleSettings.fruitCount
            );


        int squareFruits =
            Mathf.Max(
                0,
                squareSettings.fruitCount
            );


        int triangleFruits =
            Mathf.Max(
                0,
                triangleSettings.fruitCount
            );


        int rectangleFruits =
            Mathf.Min(
                Mathf.Max(
                    0,
                    rectangleSettings.fruitCount
                ),
                12
            );


        totalLevelFruits =
            circleFruits +
            squareFruits +
            triangleFruits +
            rectangleFruits;


        if (totalLevelFruits <= 0)
        {
            totalLevelFruits = 1;
        }
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


        // --------------------------------------------------------
        // ONLY TRACKED FRUITS CAN BE DESTROYED
        // --------------------------------------------------------

        if (!activeFruits.Contains(fruit))
            return;


        // --------------------------------------------------------
        // THIS KNIFE SUCCESSFULLY HIT FRUIT
        // --------------------------------------------------------

        currentKnifeHitFruit = true;


        // --------------------------------------------------------
        // REMOVE FROM ACTIVE LIST
        // --------------------------------------------------------

        activeFruits.Remove(fruit);

        remainingFruits--;


        // --------------------------------------------------------
        // UPDATE PROGRESS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // DESTROY FRUIT
        // --------------------------------------------------------

        Destroy(fruit);


        // --------------------------------------------------------
        // CHECK PHASE
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // TEST MODE
        // --------------------------------------------------------

        if (testMode)
        {
            StartTestShape();

            return;
        }


        // --------------------------------------------------------
        // NORMAL LEVEL
        // --------------------------------------------------------

        if (currentPhase >= 4)
        {
            CompleteLevel();

            return;
        }


        FormationType formation =
            (FormationType)currentPhase;


        StartNormalFormation(
            formation
        );
    }


    // ============================================================
    // NORMAL FORMATION
    // ============================================================

    private void StartNormalFormation(
        FormationType formationType)
    {
        switch (formationType)
        {
            case FormationType.Circle:

                SetLevelText("Level 1");

                SpawnFormation(
                    FormationType.Circle
                );

                break;


            case FormationType.Square:

                SetLevelText("Level 2");

                SpawnFormation(
                    FormationType.Square
                );

                break;


            case FormationType.Triangle:

                SetLevelText("Level 3");

                SpawnFormation(
                    FormationType.Triangle
                );

                break;


            case FormationType.Rectangle:

                SetLevelText("Level 4");

                SpawnFormation(
                    FormationType.Rectangle
                );

                break;
        }
    }


    // ============================================================
    // TEST SHAPE
    // ============================================================

    private void StartTestShape()
    {
        testCompleted = false;


        switch (testShape)
        {
            case TestShape.Circle:

                SetLevelText(
                    "TEST - CIRCLE"
                );

                SpawnFormation(
                    FormationType.Circle
                );

                break;


            case TestShape.Square:

                SetLevelText(
                    "TEST - SQUARE"
                );

                SpawnFormation(
                    FormationType.Square
                );

                break;


            case TestShape.Triangle:

                SetLevelText(
                    "TEST - TRIANGLE"
                );

                SpawnFormation(
                    FormationType.Triangle
                );

                break;


            case TestShape.Rectangle:

                SetLevelText(
                    "TEST - RECTANGLE"
                );

                SpawnFormation(
                    FormationType.Rectangle
                );

                break;
        }
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
                "GameManager2: Fruit Spawn Point is missing."
            );

            return;
        }


        List<Vector3> positions =
            new List<Vector3>();


        // ========================================================
        // CIRCLE
        // ========================================================

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


            SpawnFruits(
                circleSettings.fruitPrefabs,
                positions
            );
        }


        // ========================================================
        // SQUARE
        // ========================================================

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


            SpawnFruits(
                squareSettings.fruitPrefabs,
                positions
            );
        }


        // ========================================================
        // TRIANGLE
        // ========================================================

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


            SpawnFruits(
                triangleSettings.fruitPrefabs,
                positions
            );
        }


        // ========================================================
        // RECTANGLE
        // ========================================================

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


            SpawnFruits(
                rectangleSettings.fruitPrefabs,
                positions
            );
        }


        // ========================================================
        // SET ROTATION TARGET
        // ========================================================

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


        // --------------------------------------------------------
        // CENTER TRIANGLE VERTICALLY
        // --------------------------------------------------------

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


        int columns = 4;

        int rows = 3;


        int totalPositions =
            columns * rows;


        int fruitsToSpawn =
            Mathf.Min(
                count,
                totalPositions
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
    // SPAWN FRUITS
    // ============================================================

    private void SpawnFruits(
        GameObject[] fruitPrefabs,
        List<Vector3> localPositions)
    {
        if (
            fruitPrefabs == null ||
            fruitPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "Fruit Prefabs array is empty."
            );

            return;
        }


        if (fruitSpawnPoint == null)
            return;


        remainingFruits = 0;


        for (
            int i = 0;
            i < localPositions.Count;
            i++
        )
        {
            GameObject prefab =
                fruitPrefabs[
                    Random.Range(
                        0,
                        fruitPrefabs.Length
                    )
                ];


            if (prefab == null)
                continue;


            // ----------------------------------------------------
            // CREATE FRUIT
            // ----------------------------------------------------

            GameObject fruit =
                Instantiate(
                    prefab,
                    fruitSpawnPoint
                );


            // ----------------------------------------------------
            // POSITION
            // ----------------------------------------------------

            fruit.transform.localPosition =
                localPositions[i];


            // ----------------------------------------------------
            // ROTATION
            // ----------------------------------------------------

            fruit.transform.localRotation =
                Quaternion.identity;


            // ----------------------------------------------------
            // SCALE
            // ----------------------------------------------------

            fruit.transform.localScale =
                Vector3.one;


            // ----------------------------------------------------
            // COLLIDER
            // ----------------------------------------------------

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


            // ----------------------------------------------------
            // RIGIDBODY
            // ----------------------------------------------------

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


            // ----------------------------------------------------
            // TRACK FRUIT
            // ----------------------------------------------------

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


        // --------------------------------------------------------
        // TEST MODE
        // --------------------------------------------------------

        if (testMode)
        {
            CompleteTestShape();

            return;
        }


        // --------------------------------------------------------
        // NEXT PHASE
        // --------------------------------------------------------

        currentPhase++;


        // --------------------------------------------------------
        // ALL 4 SHAPES COMPLETE
        // --------------------------------------------------------

        if (currentPhase >= 4)
        {
            CompleteLevel();

            return;
        }


        // --------------------------------------------------------
        // WAIT BEFORE NEXT SHAPE
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // SHOW WIN
        // --------------------------------------------------------

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


        SetProgress(1f);


        if (levelText != null)
        {
            levelText.text =
                "LEVEL COMPLETE";
        }


        // --------------------------------------------------------
        // SHOW WIN
        // --------------------------------------------------------

        if (gameManager != null)
        {
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
        // --------------------------------------------------------
        // REMOVE ROTATION TARGET
        // --------------------------------------------------------

        if (shapeRotation != null)
        {
            shapeRotation.SetTarget(null);
        }


        // --------------------------------------------------------
        // DESTROY ALL FRUITS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // CLEAR TRACKING
        // --------------------------------------------------------

        activeFruits.Clear();

        remainingFruits = 0;
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
}
