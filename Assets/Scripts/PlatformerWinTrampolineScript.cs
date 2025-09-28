using UnityEngine;

public class PlatformerWinTrampolineScript : MonoBehaviour
{
    public PlatformerGame game;
    private float timer = 0f;
    private bool startTimer = false;
    private Rigidbody2D playerRb;

    void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 40f);
            if (timer >= 1.5f)
            {
                game.Win();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlatformerMovement>())
        {
            startTimer = true;
            playerRb = collision.GetComponentInParent<Rigidbody2D>();
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 40f);
        }
        else
        {
            var rb = collision.GetComponentInParent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 40f);
        }
    }
}
