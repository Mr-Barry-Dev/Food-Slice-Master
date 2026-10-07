// using UnityEngine;

// public class KnifeController : MonoBehaviour
// {
//     [Header("===== KNIFE MOVEMENT =====")]

//     public float moveSpeed = 12f;

//     public float destroyY = 8f;


//     [Header("===== KNIFE ROTATION =====")]

//     public float rotationSpeed = 720f;


//     [Header("===== GAME MANAGER =====")]

//     public GameManager2 gameManager;


//     private void Update()
//     {
//         // Move upward
//         transform.position +=
//             Vector3.up *
//             moveSpeed *
//             Time.deltaTime;


//         // Rotate continuously
//         transform.Rotate(
//             0f,
//             0f,
//             rotationSpeed *
//             Time.deltaTime
//         );


//         // Destroy only after reaching Y 8
//         if (transform.position.y >= destroyY)
//         {
//             Destroy(gameObject);
//         }
//     }


//     private void OnTriggerEnter2D(
//         Collider2D other)
//     {
//         if (gameManager == null)
//             return;

//         gameManager.HitFruit(
//             other.gameObject
//         );

//         // Knife DOES NOT get destroyed.
//     }


//     private void OnCollisionEnter2D(
//         Collision2D collision)
//     {
//         if (gameManager == null)
//             return;

//         gameManager.HitFruit(
//             collision.gameObject
//         );

//         // Knife DOES NOT get destroyed.
//     }
// }


using UnityEngine;

public class KnifeController : MonoBehaviour
{
    // ============================================================
    // KNIFE MOVEMENT
    // ============================================================

    [Header("===== KNIFE MOVEMENT =====")]

    [Tooltip("Knife upward firing speed.")]
    public float moveSpeed = 12f;

    [Tooltip("Knife destroy hone ki Y position.")]
    public float destroyY = 8f;


    // ============================================================
    // READY ANIMATION
    // ============================================================

    [Header("===== READY ANIMATION =====")]

    [Tooltip("Knife ready position tak kitni speed se aayega.")]
    public float readyMoveSpeed = 8f;

    [Tooltip("Spawn point se kitna neeche se start hoga.")]
    public float startOffset = 1.5f;

    [Tooltip("Ready position par pahunchne ka small distance.")]
    public float readyStopDistance = 0.02f;


    // ============================================================
    // ROTATION
    // ============================================================

    [Header("===== KNIFE ROTATION =====")]

    [Tooltip("Knife firing ke time rotation speed.")]
    public float rotationSpeed = 720f;


    // ============================================================
    // GAME MANAGER
    // ============================================================

    [Header("===== GAME MANAGER =====")]

    public GameManager2 gameManager;


    // ============================================================
    // INTERNAL
    // ============================================================

    private Vector3 readyPosition;

    private bool isReady = false;

    private bool isFired = false;


    // ============================================================
    // READY STATE
    // ============================================================

    public bool IsReady
    {
        get
        {
            return isReady && !isFired;
        }
    }


    // ============================================================
    // INITIALIZE READY KNIFE
    // ============================================================

    public void InitializeReadyKnife(
        Vector3 targetPosition)
    {
        // --------------------------------------------------------
        // SAVE READY POSITION
        // --------------------------------------------------------

        readyPosition =
            targetPosition;


        // --------------------------------------------------------
        // RESET STATES
        // --------------------------------------------------------

        isReady = false;

        isFired = false;


        // --------------------------------------------------------
        // START BELOW SPAWN POINT
        // --------------------------------------------------------

        transform.position =
            readyPosition +
            Vector3.down *
            startOffset;


        // --------------------------------------------------------
        // RESET ROTATION
        // --------------------------------------------------------

        transform.rotation =
            Quaternion.identity;
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        // --------------------------------------------------------
        // MOVE TO READY POSITION
        // --------------------------------------------------------

        if (!isReady && !isFired)
        {
            MoveToReadyPosition();

            return;
        }


        // --------------------------------------------------------
        // FIRE MOVEMENT
        // --------------------------------------------------------

        if (isFired)
        {
            MoveUpward();
        }
    }


    // ============================================================
    // MOVE TO READY POSITION
    // ============================================================

    private void MoveToReadyPosition()
    {
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                readyPosition,
                readyMoveSpeed *
                Time.deltaTime
            );


        // --------------------------------------------------------
        // CHECK READY
        // --------------------------------------------------------

        if (
            Vector3.Distance(
                transform.position,
                readyPosition
            )
            <=
            readyStopDistance
        )
        {
            transform.position =
                readyPosition;


            isReady = true;
        }
    }


    // ============================================================
    // FIRE
    // ============================================================

    public void Fire()
    {
        // --------------------------------------------------------
        // ALREADY FIRED
        // --------------------------------------------------------

        if (isFired)
            return;


        // --------------------------------------------------------
        // NOT READY YET
        // --------------------------------------------------------

        if (!isReady)
            return;


        // --------------------------------------------------------
        // FIRE
        // --------------------------------------------------------

        isFired = true;

        isReady = false;
    }


    // ============================================================
    // MOVE UPWARD
    // ============================================================

    private void MoveUpward()
    {
        // --------------------------------------------------------
        // MOVE UP
        // --------------------------------------------------------

        transform.position +=
            Vector3.up *
            moveSpeed *
            Time.deltaTime;


        // --------------------------------------------------------
        // ROTATE
        // --------------------------------------------------------

        transform.Rotate(
            0f,
            0f,
            rotationSpeed *
            Time.deltaTime
        );


        // --------------------------------------------------------
        // DESTROY ONLY AT Y LIMIT
        // --------------------------------------------------------

        if (
            transform.position.y >=
            destroyY
        )
        {
            FinishKnife();
        }
    }


    // ============================================================
    // TRIGGER COLLISION
    // ============================================================

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        HandleHit(
            other.gameObject
        );
    }


    // ============================================================
    // NORMAL COLLISION
    // ============================================================

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        HandleHit(
            collision.gameObject
        );
    }


    // ============================================================
    // HANDLE HIT
    // ============================================================

    private void HandleHit(
        GameObject other)
    {
        // --------------------------------------------------------
        // KNIFE SHOULD ONLY HIT WHILE FIRING
        // --------------------------------------------------------

        if (!isFired)
            return;


        if (other == null)
            return;


        if (gameManager == null)
            return;


        // --------------------------------------------------------
        // SEND HIT TO GAME MANAGER
        // --------------------------------------------------------

        gameManager.HitFruit(
            other
        );


        // IMPORTANT:
        // Knife is NOT destroyed here.
        // Knife continues moving upward.
    }


    // ============================================================
    // FINISH KNIFE
    // ============================================================

    private void FinishKnife()
    {
        // --------------------------------------------------------
        // PREVENT DOUBLE CALL
        // --------------------------------------------------------

        if (!isFired)
            return;


        isFired = false;


        // --------------------------------------------------------
        // TELL GAME MANAGER
        // --------------------------------------------------------

        if (gameManager != null)
        {
            gameManager.KnifeFinished(
                this
            );
        }


        // --------------------------------------------------------
        // DESTROY THIS KNIFE
        // --------------------------------------------------------

        Destroy(
            gameObject
        );
    }
}