using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;

    void Update()
    {
        mainCamera.transform.position = new Vector3(player.position.x + 2f, player.position.y + 1f, mainCamera.transform.position.z);
    }
}
