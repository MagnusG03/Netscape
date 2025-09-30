using System;
using UnityEngine;

public class PlatformerCullScript : MonoBehaviour
{
    public GameObject player;
    private float deactivateDistance = 20f;
    private float activateDistance = 15f;
    private float checkInterval = 2f;
    private float checkTimer = 0f;
    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;
            foreach (Transform child in transform)
            {
                float distance = Vector2.Distance(player.transform.position, child.position);
                if (child.gameObject.activeSelf)
                {
                    if (distance > deactivateDistance)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (distance < activateDistance)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
