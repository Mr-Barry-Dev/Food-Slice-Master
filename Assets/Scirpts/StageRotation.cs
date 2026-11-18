using UnityEngine;

/// <summary>
/// Ek hi script me 10 tarah ke rotation patterns.
/// Drop-in replacement for BossRotation: SetTarget(Transform) same hai.
/// ConfigureForStage(seed, startAngle) se har stage ka pattern + speed + start angle
/// fixed (saved) ho jata hai - same seed = hamesha same rotation.
/// </summary>
public class StageRotation : MonoBehaviour
{
    public enum RotationPattern
    {
        ClockwiseConstant,       // 0: seedha clockwise
        AntiClockwiseConstant,   // 1: seedha anti-clockwise
        PingPong,                // 2: idhar se udhar jhoolna
        ClockwiseAccelerating,   // 3: clockwise, slow se fast se slow
        SpeedPulseAntiClockwise, // 4: anti-clockwise, speed pulse karti hai
        StopAndGoClockwise,      // 5: clockwise ghoom ke ruk ke phir ghoomna
        StopAndGoAntiClockwise,  // 6: anti-clockwise ghoom ke ruk ke phir ghoomna
        ZigZagFlip,              // 7: har kuch second me direction badal jata hai
        WobbleDrift,             // 8: dheere ghoomna + upar se hilna
        StepSnap                 // 9: jhatke se fixed angle steps me ghoomna
    }


    // ============================================================
    // PATTERN SELECTION
    // ============================================================

    [Header("===== PATTERN =====")]

    [Tooltip("ON: har stage ke seed se pattern chuna jayega (fixed per stage). OFF: neeche wala Pattern use hoga.")]
    public bool randomPatternPerStage = true;

    [Tooltip("Randomly chunne ke liye allowed patterns. Khali = sabhi 10.")]
    public RotationPattern[] allowedPatterns;

    [Tooltip("randomPatternPerStage OFF ho to ye pattern chalega.")]
    public RotationPattern pattern = RotationPattern.ClockwiseConstant;


    // ============================================================
    // SPEED SETTINGS
    // ============================================================

    [Header("===== SPEED (degrees / second) =====")]

    public float minSpeed = 60f;

    public float maxSpeed = 140f;

    [Tooltip("Level badhne par speed kitni badhe (per level). 0 = no change.")]
    public float speedIncreasePerLevel = 0f;


    // ============================================================
    // PATTERN DETAILS
    // ============================================================

    [Header("===== PINGPONG / WOBBLE =====")]

    public float minSwingAngle = 60f;

    public float maxSwingAngle = 120f;

    public float swingPeriod = 3f;

    [Header("===== ACCELERATE / PULSE =====")]

    public float speedCyclePeriod = 4f;

    [Header("===== STOP AND GO =====")]

    public float rotateTime = 1.5f;

    public float pauseTime = 0.7f;

    [Header("===== ZIGZAG =====")]

    public float flipInterval = 2f;

    [Header("===== STEP SNAP =====")]

    [Tooltip("Ek step kitne degree ka.")]
    public float stepAngle = 45f;

    [Tooltip("Kitne second baad agla step.")]
    public float stepInterval = 0.8f;

    [Tooltip("Step tak pahunchne ki speed (degrees/second).")]
    public float stepSnapSpeed = 400f;


    // ============================================================
    // RUNTIME
    // ============================================================

    private Transform target;

    private RotationPattern activePattern;

    private float speed;
    private float swingAngle;
    private float startAngle;

    private float angle;        // start angle ke upar kitna ghooma
    private float drift;        // WobbleDrift ke liye
    private float time;

    private float stepTarget;
    private float stepTimer;
    private float stepDirection = -1f;

    public bool paused = false;


    // ============================================================
    // PUBLIC API
    // ============================================================

    /// <summary>BossRotation jaisa hi. null do to rotation ruk jati hai.</summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        ResetState();
    }


    /// <summary>
    /// Stage ka rotation fix karo. Same seed + same startAngle = same rotation.
    /// SetTarget ke BAAD call karo.
    /// </summary>
    public void ConfigureForStage(int seed, float stageStartAngle, int level = 1)
    {
        System.Random rng = new System.Random(seed);

        // Pattern
        if (randomPatternPerStage)
        {
            if (allowedPatterns != null && allowedPatterns.Length > 0)
            {
                activePattern =
                    allowedPatterns[rng.Next(allowedPatterns.Length)];
            }
            else
            {
                activePattern =
                    (RotationPattern)rng.Next(10);
            }
        }
        else
        {
            activePattern = pattern;

            rng.Next(); // random sequence same rakhne ke liye
        }


        // Speed
        speed =
            Mathf.Lerp(
                minSpeed,
                maxSpeed,
                (float)rng.NextDouble()
            ) +
            Mathf.Max(0, level - 1) * speedIncreasePerLevel;


        // Swing angle
        swingAngle =
            Mathf.Lerp(
                minSwingAngle,
                maxSwingAngle,
                (float)rng.NextDouble()
            );


        // Step direction (StepSnap ke liye)
        stepDirection = rng.Next(2) == 0 ? -1f : 1f;


        startAngle = stageStartAngle;

        ResetState();

        Apply();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (target == null || paused)
            return;


        float dt = Time.deltaTime;

        time += dt;


        switch (activePattern)
        {
            case RotationPattern.ClockwiseConstant:

                angle -= speed * dt;

                break;


            case RotationPattern.AntiClockwiseConstant:

                angle += speed * dt;

                break;


            case RotationPattern.PingPong:

                angle =
                    Mathf.Sin(
                        time * Mathf.PI * 2f /
                        Mathf.Max(0.1f, swingPeriod)
                    ) * swingAngle;

                break;


            case RotationPattern.ClockwiseAccelerating:
            {
                float t =
                    Mathf.PingPong(
                        time / Mathf.Max(0.1f, speedCyclePeriod),
                        1f
                    );

                float current =
                    Mathf.Lerp(speed * 0.4f, speed * 1.8f, t);

                angle -= current * dt;

                break;
            }


            case RotationPattern.SpeedPulseAntiClockwise:
            {
                float pulse =
                    1f +
                    0.8f *
                    Mathf.Sin(
                        time * Mathf.PI * 2f /
                        Mathf.Max(0.1f, speedCyclePeriod)
                    );

                angle += speed * pulse * dt;

                break;
            }


            case RotationPattern.StopAndGoClockwise:

                if (IsRotatePhase())
                    angle -= speed * dt;

                break;


            case RotationPattern.StopAndGoAntiClockwise:

                if (IsRotatePhase())
                    angle += speed * dt;

                break;


            case RotationPattern.ZigZagFlip:
            {
                int step =
                    Mathf.FloorToInt(
                        time / Mathf.Max(0.1f, flipInterval)
                    );

                float dir = (step % 2 == 0) ? -1f : 1f;

                angle += dir * speed * dt;

                break;
            }


            case RotationPattern.WobbleDrift:

                drift -= speed * 0.5f * dt;

                angle =
                    drift +
                    Mathf.Sin(
                        time * Mathf.PI * 2f /
                        Mathf.Max(0.1f, swingPeriod)
                    ) * 30f;

                break;


            case RotationPattern.StepSnap:

                stepTimer += dt;

                if (stepTimer >= stepInterval)
                {
                    stepTimer = 0f;

                    stepTarget += stepAngle * stepDirection;
                }

                angle =
                    Mathf.MoveTowards(
                        angle,
                        stepTarget,
                        stepSnapSpeed * dt
                    );

                break;
        }


        Apply();
    }


    // ============================================================
    // HELPERS
    // ============================================================

    private bool IsRotatePhase()
    {
        float cycle =
            Mathf.Max(0.1f, rotateTime + pauseTime);

        return (time % cycle) < rotateTime;
    }


    private void ResetState()
    {
        angle = 0f;
        drift = 0f;
        time = 0f;
        stepTarget = 0f;
        stepTimer = 0f;
    }


    private void Apply()
    {
        if (target == null)
            return;

        target.rotation =
            Quaternion.Euler(0f, 0f, startAngle + angle);
    }
}