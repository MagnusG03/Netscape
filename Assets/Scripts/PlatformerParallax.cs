using System.Collections.Generic;
using UnityEngine;

/// Parallax + infinite horizontal tiling for a SpriteRenderer layer.
/// Drop this on a GameObject that has a SpriteRenderer with a seamless sprite.
/// It will auto-spawn left/right tiles and scroll them as the camera moves.
[RequireComponent(typeof(SpriteRenderer))]
[DefaultExecutionOrder(1000)] // run after camera LateUpdate
public class PlatformerParallax : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;                 // If null, uses Camera.main

    [Header("Parallax (fraction of camera movement)")]
    [Range(-1f, 2f)] public float parallaxX = 0.5f;
    [Range(-1f, 2f)] public float parallaxY = 0.0f;

    [Header("Repeating")]
    public bool repeatX = true;                 // Horizontal infinite repeat
    public bool repeatY = false;                // (Optional) vertical repeat if your art tiles vertically
    [Tooltip("How many tiles to keep loaded in each direction (min 1). 1 = three tiles total.")]
    [Min(1)] public int tilesPerSide = 1;

    [Header("Z / Sorting")]
    public bool lockZToOriginal = true;         // Keeps Z from drifting

    // Internal
    private Transform _camT;
    private Vector3 _lastCamPos;
    private SpriteRenderer _src;
    private Vector2 _tileSizeWorld;             // world width/height of the sprite (after scale)
    private readonly List<Transform> _tiles = new List<Transform>(); // grid of spawned tiles (center + clones)
    private Vector3 _baseZ;

    void Awake()
    {
        _src = GetComponent<SpriteRenderer>();
        if (!_src || !_src.sprite)
        {
            Debug.LogError($"[{name}] ParallaxRepeatingLayer needs a SpriteRenderer with a sprite.");
            enabled = false;
            return;
        }

        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera) { Debug.LogError("No camera assigned and Camera.main not found."); enabled = false; return; }
        _camT = targetCamera.transform;

        // Cache world size of the sprite (includes this transform's scale)
        // bounds.size already accounts for lossyScale.
        _tileSizeWorld = _src.bounds.size;

        _lastCamPos = _camT.position;
        _baseZ = new Vector3(0, 0, transform.position.z);

        BuildTiles();
    }

    void BuildTiles()
    {
        // Clear previous (if any)
        foreach (var t in _tiles)
            if (t && t != transform) Destroy(t.gameObject);
        _tiles.Clear();

        // Ensure center tile is first
        _tiles.Add(transform);

        // Spawn a ring of tiles around the center (only X by default)
        for (int x = -tilesPerSide; x <= tilesPerSide; x++)
        {
            for (int y = - (repeatY ? tilesPerSide : 0); y <= (repeatY ? tilesPerSide : 0); y++)
            {
                if (x == 0 && y == 0) continue;
                var t = SpawnClone(new Vector3(x * _tileSizeWorld.x, y * _tileSizeWorld.y, 0f));
                _tiles.Add(t);
            }
        }
    }

    Transform SpawnClone(Vector3 localOffset)
    {
        var go = new GameObject($"{name}_tile");
        go.transform.SetParent(transform, worldPositionStays: false);
        go.transform.localPosition = localOffset;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale    = Vector3.one; // inherits parent scale

        var dst = go.AddComponent<SpriteRenderer>();
        CopyRenderer(_src, dst);
        return go.transform;
    }

    static void CopyRenderer(SpriteRenderer src, SpriteRenderer dst)
    {
        dst.sprite = src.sprite;
        dst.color = src.color;
        dst.flipX = src.flipX;
        dst.flipY = src.flipY;
        dst.drawMode = src.drawMode;
        dst.size = src.size;
        dst.maskInteraction = src.maskInteraction;
        dst.sortingLayerID = src.sortingLayerID;
        dst.sortingOrder = src.sortingOrder;
        dst.sharedMaterial = src.sharedMaterial;
        dst.material = src.material;
        dst.enabled = src.enabled;
    }

    void LateUpdate()
    {
        if (!_camT) return;

        // 1) Parallax shift by camera movement delta
        Vector3 camPos = _camT.position;
        Vector3 delta = camPos - _lastCamPos;

        transform.position += new Vector3(delta.x * parallaxX, delta.y * parallaxY, 0f);
        if (lockZToOriginal)
            transform.position = new Vector3(transform.position.x, transform.position.y, _baseZ.z);

        _lastCamPos = camPos;

        // 2) Recycle tiles to create endless scrolling
        if (repeatX) RecycleX();
        if (repeatY) RecycleY();
    }

    void RecycleX()
    {
        // Find leftmost & rightmost tiles in world X
        float width = _tileSizeWorld.x;
        Transform leftmost = null, rightmost = null;
        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;

        foreach (var t in _tiles)
        {
            float x = t.position.x;
            if (x < minX) { minX = x; leftmost = t; }
            if (x > maxX) { maxX = x; rightmost = t; }
        }

        // When the camera passes beyond a tile by more than half a width,
        // hop the tile from one side to the other.
        float camX = _camT.position.x;

        // Move leftmost to the right
        while (camX - minX > width * 1.5f)
        {
            leftmost.position += new Vector3((Mathf.Ceil((_camT.position.x - minX) / width) + 1) * width, 0f, 0f);
            minX = float.PositiveInfinity; maxX = float.NegativeInfinity;
            foreach (var t in _tiles)
            {
                float x = t.position.x;
                if (x < minX) { minX = x; leftmost = t; }
                if (x > maxX) { maxX = x; rightmost = t; }
            }
        }

        // Move rightmost to the left
        while (maxX - camX > width * 1.5f)
        {
            rightmost.position -= new Vector3((Mathf.Ceil((maxX - _camT.position.x) / width) + 1) * width, 0f, 0f);
            minX = float.PositiveInfinity; maxX = float.NegativeInfinity;
            foreach (var t in _tiles)
            {
                float x = t.position.x;
                if (x < minX) { minX = x; leftmost = t; }
                if (x > maxX) { maxX = x; rightmost = t; }
            }
        }
    }

    void RecycleY()
    {
        float height = _tileSizeWorld.y;
        Transform bottom = null, top = null;
        float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;

        foreach (var t in _tiles)
        {
            float y = t.position.y;
            if (y < minY) { minY = y; bottom = t; }
            if (y > maxY) { maxY = y; top = t; }
        }

        float camY = _camT.position.y;

        while (camY - minY > height * 1.5f)
        {
            bottom.position += new Vector3(0f, (Mathf.Ceil((_camT.position.y - minY) / height) + 1) * height, 0f);
            minY = float.PositiveInfinity; maxY = float.NegativeInfinity;
            foreach (var t in _tiles)
            {
                float y = t.position.y;
                if (y < minY) { minY = y; bottom = t; }
                if (y > maxY) { maxY = y; top = t; }
            }
        }

        while (maxY - camY > height * 1.5f)
        {
            top.position -= new Vector3(0f, (Mathf.Ceil((maxY - _camT.position.y) / height) + 1) * height, 0f);
            minY = float.PositiveInfinity; maxY = float.NegativeInfinity;
            foreach (var t in _tiles)
            {
                float y = t.position.y;
                if (y < minY) { minY = y; bottom = t; }
                if (y > maxY) { maxY = y; top = t; }
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (tilesPerSide < 1) tilesPerSide = 1;
    }
#endif
}
