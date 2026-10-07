// using UnityEngine;
// using System.Collections;

// public class BossRotation : MonoBehaviour
// {
//     [Header("BOSS SPRITE")]
//     public SpriteRenderer targetRenderer;
//     public Sprite bossSprite;

//     [Header("ROTATION")]
//     public float minSpeed = 150f;
//     public float maxSpeed = 400f;

//     [Range(0f, 1f)]
//     public float stopChance = 0.4f;

//     public float stopDuration = 0.5f;
//     public float minChangeTime = 0.5f;
//     public float maxChangeTime = 2f;

//     private float currentSpeed;
//     private Coroutine rotationRoutine;

//     void OnEnable()
//     {
//         if (targetRenderer != null && bossSprite != null)
//             targetRenderer.sprite = bossSprite;

//         rotationRoutine = StartCoroutine(RandomRotationRoutine());
//     }

//     void OnDisable()
//     {
//         if (rotationRoutine != null)
//             StopCoroutine(rotationRoutine);

//         currentSpeed = 0f;
//     }

//     void Update()
//     {
//         if (targetRenderer != null)
//         {
//             targetRenderer.transform.Rotate(
//                 0f,
//                 0f,
//                 currentSpeed * Time.deltaTime);
//         }
//     }

//     IEnumerator RandomRotationRoutine()
//     {
//         while (true)
//         {
//             int direction =
//                 Random.Range(0, 2) == 0 ? -1 : 1;

//             currentSpeed =
//                 Random.Range(minSpeed, maxSpeed) * direction;

//             yield return new WaitForSeconds(
//                 Random.Range(minChangeTime, maxChangeTime));

//             if (Random.value <= stopChance)
//             {
//                 currentSpeed = 0f;

//                 yield return new WaitForSeconds(stopDuration);
//             }
//         }
//     }
// }




using UnityEngine;
using System.Collections;

public class BossRotation : MonoBehaviour
{
    [Header("===== ROTATION =====")]
    public float minSpeed = 150f;
    public float maxSpeed = 400f;

    [Range(0f, 1f)]
    public float stopChance = 0.4f;

    public float stopDuration = 0.5f;

    [Header("===== SPEED CHANGE TIME =====")]
    public float minChangeTime = 0.5f;
    public float maxChangeTime = 2f;

    private Transform target;
    private float currentSpeed;
    private Coroutine rotationRoutine;

    private void OnEnable()
    {
        StartRotation();
    }

    private void OnDisable()
    {
        StopRotation();
    }

    // GameManager current formation yahan bhejega
    public void SetTarget(Transform newTarget)
    {
        // Purane target ki rotation reset
        if (target != null)
        {
            target.localRotation = Quaternion.identity;
        }

        target = newTarget;

        // New formation ko starting rotation par rakho
        if (target != null)
        {
            target.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        if (target == null)
            return;

        target.Rotate(
            0f,
            0f,
            currentSpeed * Time.deltaTime,
            Space.Self
        );
    }

    private void StartRotation()
    {
        if (rotationRoutine != null)
        {
            StopCoroutine(rotationRoutine);
        }

        rotationRoutine = StartCoroutine(RandomRotationRoutine());
    }

    private void StopRotation()
    {
        if (rotationRoutine != null)
        {
            StopCoroutine(rotationRoutine);
            rotationRoutine = null;
        }

        currentSpeed = 0f;
    }

    private IEnumerator RandomRotationRoutine()
    {
        while (true)
        {
            // Random direction
            int direction =
                Random.Range(0, 2) == 0 ? -1 : 1;

            // Random speed
            currentSpeed =
                Random.Range(minSpeed, maxSpeed) * direction;

            // Kitni der current speed chalegi
            yield return new WaitForSeconds(
                Random.Range(minChangeTime, maxChangeTime)
            );

            // Random chance se rukna
            if (Random.value <= stopChance)
            {
                currentSpeed = 0f;

                yield return new WaitForSeconds(stopDuration);
            }
        }
    }
}