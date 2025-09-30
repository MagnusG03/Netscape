using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerSpriteAI : MonoBehaviour
{
    public GameObject player;
    private Rigidbody2D rb;
    private bool facingLeft = true;
    private int moveDirection = -1;
    private float moveSpeed = 1f;
    private float turnGracePeriod = 0.2f;
    private float turnTimer = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        turnTimer -= Time.deltaTime;
        if (rb.linearVelocityX < 0.01f && rb.linearVelocityX > -0.01f)
        {
            FlipDirection();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, player.transform.position.y - gameObject.transform.position.y);
    }

    void FlipDirection()
    {
        if (facingLeft && turnTimer <= 0f)
        {
            facingLeft = false;
            moveDirection = 1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            turnTimer = turnGracePeriod;
        }
        else if (!facingLeft && turnTimer <= 0f)
        {
            facingLeft = true;
            moveDirection = -1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            turnTimer = turnGracePeriod;
        }
    }
}
