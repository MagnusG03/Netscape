using UnityEngine;

public class PlatformerDamage : MonoBehaviour
{
    private Vector2 bumpVector;
    private PlatformerMovement move;
    private float stunDuration = 0.3f;
    private float stunTimer = 0f;
    public PlatformerGame game;

    void Awake()
    {
        move = GetComponent<PlatformerMovement>();
    }

    void Update()
    {
        if (move && Time.time >= stunTimer)
        {
            move.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Strongpoint"))
        {
            bumpVector = transform.position - collision.transform.gameObject.transform.position;
            bumpVector.Normalize();
            var rb = GetComponentInParent<Rigidbody2D>();

            move.enabled = false;
            stunTimer = Time.time + stunDuration;
            rb.linearVelocity = new Vector2(bumpVector.x * Random.Range(4f, 8f), 3f);
            game.DamagePlayer(1);
        }
    }
}
