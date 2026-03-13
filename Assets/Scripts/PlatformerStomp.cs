using UnityEngine;

public class PlatformerStomp : MonoBehaviour
{
    public PlatformerGame game;
    private BoxCollider2D stompCollider;
    private Rigidbody2D playerRb;
    private const float BounceVelocity = 6.5f;
    private const float StompTolerance = 0.02f;

    void Awake()
    {
        stompCollider = GetComponent<BoxCollider2D>();
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            TryStomp(collision);
        }
    }

    public bool TryStomp(Collider2D enemyCollider)
    {
        if (!enemyCollider)
        {
            return false;
        }

        Transform enemyRoot = enemyCollider.transform.parent;
        if (!enemyRoot)
        {
            return false;
        }

        Collider2D weakpoint = enemyCollider.CompareTag("Weakpoint")
            ? enemyCollider
            : FindWeakpoint(enemyRoot);

        if (!weakpoint || !IsValidStomp(weakpoint))
        {
            return false;
        }

        game.PlayerInvulnerable(0.2f);
        Destroy(enemyRoot.gameObject);
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, BounceVelocity);
        return true;
    }

    private bool IsValidStomp(Collider2D weakpoint)
    {
        if (!weakpoint || !stompCollider || !playerRb || playerRb.linearVelocity.y > 0f)
        {
            return false;
        }

        Bounds weakpointBounds = weakpoint.bounds;
        Bounds currentBounds = stompCollider.bounds;
        Vector2 stepDelta = playerRb.linearVelocity * Time.fixedDeltaTime;
        float previousBottom = currentBounds.min.y - stepDelta.y;
        float currentBottom = currentBounds.min.y;
        Bounds sweptBounds = currentBounds;
        sweptBounds.Encapsulate(currentBounds.min - (Vector3)stepDelta);
        sweptBounds.Encapsulate(currentBounds.max - (Vector3)stepDelta);

        bool horizontalOverlap = sweptBounds.max.x >= weakpointBounds.min.x && sweptBounds.min.x <= weakpointBounds.max.x;
        if (!horizontalOverlap)
        {
            return false;
        }

        if (currentBounds.Intersects(weakpointBounds))
        {
            return true;
        }

        bool crossedWeakpointTop = previousBottom >= weakpointBounds.max.y - StompTolerance
            && currentBottom <= weakpointBounds.max.y + StompTolerance;
        return crossedWeakpointTop && transform.position.y >= weakpointBounds.center.y;
    }

    private Collider2D FindWeakpoint(Transform enemyRoot)
    {
        foreach (Collider2D enemyCollider in enemyRoot.GetComponentsInChildren<Collider2D>())
        {
            if (enemyCollider.CompareTag("Weakpoint"))
            {
                return enemyCollider;
            }
        }

        return null;
    }
}
