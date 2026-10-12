using UnityEngine;

public class KnifeController2 : MonoBehaviour
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

    public OtherGameManager2 gameManager;


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
        readyPosition =
            targetPosition;


        isReady = false;

        isFired = false;


        transform.position =
            readyPosition +
            Vector3.down *
            startOffset;


        transform.rotation =
            Quaternion.identity;
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!isReady && !isFired)
        {
            MoveToReadyPosition();

            return;
        }


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
        if (isFired)
            return;


        if (!isReady)
            return;


        isFired = true;

        isReady = false;
    }


    // ============================================================
    // MOVE UPWARD
    // ============================================================

    private void MoveUpward()
    {
        transform.position +=
            Vector3.up *
            moveSpeed *
            Time.deltaTime;


        transform.Rotate(
            0f,
            0f,
            rotationSpeed *
            Time.deltaTime
        );


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
        if (!isFired)
            return;


        if (other == null)
            return;


        if (gameManager == null)
            return;


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
        if (!isFired)
            return;


        isFired = false;


        if (gameManager != null)
        {
            gameManager.KnifeFinished(
                this
            );
        }


        Destroy(
            gameObject
        );
    }
}