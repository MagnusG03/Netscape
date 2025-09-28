using UnityEngine;

public class PlatformerTrampolineScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var rb = collision.GetComponentInParent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
    }
}
