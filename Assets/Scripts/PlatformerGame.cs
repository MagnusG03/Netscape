using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerGame : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public PlatformerCanvas canvas;
    private Color playerColor;
    private bool gameOver = false;
    private int playerHealth = 3;
    private float damageInvulnerabilityTimer = 0f;
    private float damageInvulnerabilityDuration = 1f;
    private float invulnerabilityTimer = 0f;
    private float continueTimer = 5;
    private bool continueTimerActive = false;

    void Start()
    {
        playerColor = playerSprite.color;
    }

    void Update()
    {
        if (!gameOver)
        {
            Time.timeScale = 1f;
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
        else if (gameOver && Time.timeScale != 0f)
        {
            GameOver();
        }
        if (continueTimerActive)
        {
            if (continueTimer > 0)
            {
                continueTimer -= Time.deltaTime;
            }
            canvas.roundWinPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = "Continuing in " + Math.Ceiling(continueTimer);
            if (continueTimer <= 0)
            {
                RestartGame();
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

    private void GameOver()
    {
        canvas.gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Win()
    {
        canvas.roundWinPanel.SetActive(true);
        continueTimerActive = true;
    }
}
