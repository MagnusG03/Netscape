using UnityEngine;

public class PlatformerStomp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            Destroy(collision.transform.parent.gameObject);

            var rb = GetComponentInParent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
        }
    }
}
