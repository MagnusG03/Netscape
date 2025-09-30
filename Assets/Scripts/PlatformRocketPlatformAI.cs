using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerRocketPlatformAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool rocketing = false;
    public float rocketingTimer = 0f;
    public float rocketingDuration = 2f;
    public float rocketPower = 1f;
    private float width;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        rocketingTimer -= Time.deltaTime;
        if (rocketingTimer <= 0f)
        {
            rocketing = !rocketing;
            animator.SetBool("isFalling", !rocketing);
            rocketingTimer = rocketingDuration;
        }
        if (rocketing == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 3f * rocketPower);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            if (collision.transform.position.x - gameObject.transform.position.x > 0)
            {
                collision.gameObject.transform.position = new Vector2(gameObject.transform.position.x + 0.3f * width, collision.transform.position.y);
            }
            else
            {
                collision.gameObject.transform.position = new Vector2(gameObject.transform.position.x - 0.3f * width, collision.transform.position.y);
            }
        }
    }
}
