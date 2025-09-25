using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class PlatformerCamera : MonoBehaviour
{
    public Transform player;

    // Dead-zone size as FRACTION of the visible screen (0..1)
    private float deadZoneWidth = 0.2f;
    private float deadZoneHeight = 0.5f;

    // Shift the dead-zone relative to screen size (fraction of screen)
    // -0.25 = shift left by 25% of screen width (i.e., “look ahead” to the right)
    private float horizontalScreenOffset = -0.175f;
    private float verticalScreenOffset = 0f;

    // Optional smoothing
    private bool useSmoothing = true;
    private float smoothTime = 0.08f;
    private Vector3 velocity;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("PlatformerCamera: Use an orthographic camera for 2D.");
    }

    private void LateUpdate()
    {
        if (player == null || cam == null) return;

        Vector3 camPos = transform.position;

        // Screen size in WORLD units
        float halfScreenH = cam.orthographicSize;
        float halfScreenW = halfScreenH * cam.aspect;

        // Dead-zone size in WORLD units
        float dzHalfW = Mathf.Clamp01(deadZoneWidth) * halfScreenW;
        float dzHalfH = Mathf.Clamp01(deadZoneHeight) * halfScreenH;

        // Dead-zone center (shifted by screen fraction)
        Vector3 dzCenter = camPos;
        dzCenter.x += horizontalScreenOffset * (halfScreenW * 2f);
        dzCenter.y += verticalScreenOffset   * (halfScreenH * 2f);

        float left   = dzCenter.x - dzHalfW;
        float right  = dzCenter.x + dzHalfW;
        float bottom = dzCenter.y - dzHalfH;
        float top    = dzCenter.y + dzHalfH;

        // Move camera only enough to bring player back inside the box
        Vector3 target = camPos;
        Vector3 p = player.position;

        if (p.x > right)      target.x += p.x - right;
        else if (p.x < left)  target.x += p.x - left;

        if (p.y > top)        target.y += p.y - top;
        else if (p.y < bottom)target.y += p.y - bottom;

        target.z = camPos.z;

        transform.position = useSmoothing
            ? Vector3.SmoothDamp(camPos, target, ref velocity, smoothTime)
            : target;
    }

    // Visualize the dead-zone in the Scene view
    private void OnDrawGizmosSelected()
    {
        var c = GetComponent<Camera>();
        if (c == null || !c.orthographic) return;

        float halfScreenH = c.orthographicSize;
        float halfScreenW = halfScreenH * c.aspect;

        float dzHalfW = Mathf.Clamp01(deadZoneWidth) * halfScreenW;
        float dzHalfH = Mathf.Clamp01(deadZoneHeight) * halfScreenH;

        Vector3 dzCenter = transform.position;
        dzCenter.x += horizontalScreenOffset * (halfScreenW * 2f);
        dzCenter.y += verticalScreenOffset   * (halfScreenH * 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(dzCenter, new Vector3(dzHalfW * 2f, dzHalfH * 2f, 0f));

        Gizmos.color = new Color(1,1,1,0.25f);
        Gizmos.DrawWireCube(transform.position, new Vector3(halfScreenW * 2f, halfScreenH * 2f, 0f));
    }
}
