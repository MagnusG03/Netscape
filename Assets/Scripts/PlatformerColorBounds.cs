using UnityEngine;

/// Screen-color fill when the camera goes above/below certain Y levels.
/// Attach to an empty GameObject (or the camera). For 2D orthographic cameras.
[DefaultExecutionOrder(1100)] // after your camera moves in LateUpdate
public class PlatformerColorBounds : MonoBehaviour
{
    [Header("Camera")]
    public Camera targetCamera;            // if null, uses Camera.main

    [Header("Y thresholds (world units)")]
    public float bottomColorThresholdY = -10f; // below this: show Bottom Color
    public float topColorThresholdY =  20f;    // above this: show Top Color

    [Header("Colors")]
    public Color bottomColor = new Color(0.09f, 0.19f, 0.09f); // dark green
    public Color topColor    = new Color(0.53f, 0.84f, 0.97f); // sky blue

    [Header("Render order")]
    public string sortingLayerName = "Background"; // create if needed
    public int sortingOrder = -1000;               // behind everything

    // Internals
    private SpriteRenderer _bottomFill;
    private SpriteRenderer _topFill;
    private Transform _camT;
    private float _lastOrthoSize;
    private float _lastAspect;

    void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera || !targetCamera.orthographic)
        {
            Debug.LogError("VerticalColorBoundsFill needs an orthographic camera.");
            enabled = false; return;
        }

        _camT = targetCamera.transform;

        // Make 1x1 white sprite programmatically (so you don’t need an asset)
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        _bottomFill = CreateFill("BottomFill", sprite, bottomColor);
        _topFill    = CreateFill("TopFill",    sprite, topColor);

        _lastOrthoSize = targetCamera.orthographicSize;
        _lastAspect    = targetCamera.aspect;

        ResizeToCamera(); // initial size
        UpdateFills(true);
    }

    SpriteRenderer CreateFill(string name, Sprite sprite, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, worldPositionStays: false);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color  = color;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder     = sortingOrder;

        return sr;
    }

    void LateUpdate()
    {
        // Follow camera
        var pos = _camT.position;
        _bottomFill.transform.position = new Vector3(pos.x, pos.y, 0f);
        _topFill.transform.position    = new Vector3(pos.x, pos.y, 0f);

        // Resize if camera zoom/aspect changes
        if (_lastOrthoSize != targetCamera.orthographicSize || _lastAspect != targetCamera.aspect)
        {
            ResizeToCamera();
            _lastOrthoSize = targetCamera.orthographicSize;
            _lastAspect = targetCamera.aspect;
        }

        UpdateFills(false);
    }

    void ResizeToCamera()
    {
        // World-space width/height of the visible screen
        float halfH = targetCamera.orthographicSize;
        float halfW = halfH * targetCamera.aspect;

        // Our sprite’s native size is 1x1; scale it to cover the screen with a small margin
        Vector3 scale = new Vector3(halfW * 2f + 2f, halfH * 2f + 2f, 1f);
        _bottomFill.transform.localScale = scale;
        _topFill.transform.localScale    = scale;
    }

    void UpdateFills(bool force)
    {
        float y = _camT.position.y;

        bool showBottom = y < bottomColorThresholdY;
        bool showTop    = y > topColorThresholdY;

        if (force || _bottomFill.enabled != showBottom)
            _bottomFill.enabled = showBottom;

        if (force || _topFill.enabled != showTop)
            _topFill.enabled = showTop;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (targetCamera && !targetCamera.orthographic)
            Debug.LogWarning("VerticalColorBoundsFill expects an orthographic camera.");
    }
#endif
}
