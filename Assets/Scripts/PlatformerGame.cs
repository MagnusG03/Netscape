using UnityEngine;

public class PlatformerGame : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    private Color playerColor;
    private bool gameOver = false;
    private int playerHealth = 3;
    private float damageInvulnerabilityTimer = 0f;
    private float damageInvulnerabilityDuration = 1f;
    private float invulnerabilityTimer = 0f;

    void Start()
    {
        playerColor = playerSprite.color;
    }

    void Update()
    {
        if (!gameOver)
        {
            damageInvulnerabilityTimer -= Time.deltaTime;
            invulnerabilityTimer -= Time.deltaTime;
            if (damageInvulnerabilityTimer > 0f)
            {
                playerSprite.color = Color.Lerp(playerColor, Color.red, (1f - Mathf.Cos(2f * Mathf.PI * 3 * (1f - Mathf.Clamp01(damageInvulnerabilityTimer / damageInvulnerabilityDuration)))) * 0.5f);
            }
            else
            {
                playerSprite.color = playerColor;
            }
            if (playerHealth <= 0)
            {
                gameOver = true;
                Debug.Log("Game Over!");
            }
        }
    }

    public void DamagePlayer(int damageAmount)
    {
        if (damageInvulnerabilityTimer <= 0f && invulnerabilityTimer <= 0f)
        {
            playerHealth -= damageAmount;
            damageInvulnerabilityTimer = damageInvulnerabilityDuration;
            Debug.Log("Player Health: " + playerHealth);
        }
    }

    public void PlayerInvulnerable(float invulnerabilityDuration)
    {
        invulnerabilityTimer = invulnerabilityDuration;
    }
}
