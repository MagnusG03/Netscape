using UnityEngine;

public class PlatformerStomp : MonoBehaviour
{
    public PlatformerGame game;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            game.PlayerInvulnerable(0.2f);
            Destroy(collision.transform.parent.gameObject);

            var rb = GetComponentInParent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 6.5f);
        }
    }
}
