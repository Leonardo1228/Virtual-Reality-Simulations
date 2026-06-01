using UnityEngine;

[ExecuteAlways]
public class TutorialWall : MonoBehaviour
{
    private Renderer cachedRenderer;

    void Awake()
    {
        UpdateRenderer();
    }

    void OnEnable()
    {
        VehicleCollision.allTutorialWalls.Add(this);

        UpdateRenderer();
    }

    void OnDisable()
    {
        VehicleCollision.allTutorialWalls.Remove(this);
    }

    void UpdateRenderer()
    {
        cachedRenderer =
            GetComponentInChildren<Renderer>();
    }

    public Bounds GetWorldBounds()
    {
        if (cachedRenderer == null)
            UpdateRenderer();

        return cachedRenderer.bounds;
    }

    public Vector3 GetHalfExtents()
    {
        Bounds b =
            GetWorldBounds();

        return b.extents;
    }

    public Vector3 GetCenter()
    {
        Bounds b =
            GetWorldBounds();

        return b.center;
    }

    void OnDrawGizmos()
    {
        if (cachedRenderer == null)
            UpdateRenderer();

        if (cachedRenderer == null)
            return;

        Bounds b =
            cachedRenderer.bounds;

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawWireCube(
            b.center,
            b.size
        );
    }
}
