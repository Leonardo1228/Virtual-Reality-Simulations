using UnityEngine;

[ExecuteAlways]
public class Ramp : MonoBehaviour
{
    private Renderer cachedRenderer;


    void Awake()
    {
        UpdateRenderer();
    }


    void OnEnable()
    {
        SimulationBody.allRamps.Add(this);

        UpdateRenderer();
    }


    void OnDisable()
    {
        SimulationBody.allRamps.Remove(this);
    }


    void UpdateRenderer()
    {
        cachedRenderer =
            GetComponentInChildren<Renderer>();
    }


    public Bounds GetBounds()
    {
        if (cachedRenderer == null)
        {
            UpdateRenderer();
        }

        return cachedRenderer.bounds;
    }


    public Vector3 SurfaceNormal()
    {
        // La cara superior del cubo
        return transform.up;
    }


    public bool ContainsPoint(
        Vector3 worldPoint)
    {
        Vector3 local =
            transform.InverseTransformPoint(
                worldPoint
            );


        Vector3 half =
            transform.localScale * 0.5f;


        return
            Mathf.Abs(local.x) <= half.x
            &&
            Mathf.Abs(local.z) <= half.z;
    }


    public float GetSurfaceHeight(
        Vector3 worldPoint)
    {
        /*
         Convertimos el punto a espacio local.

         En un cubo:
         y = +0.5 es la cara superior.
        */

        Vector3 local =
            transform.InverseTransformPoint(
                worldPoint
            );


        local.y = 0.5f;


        Vector3 surfacePoint =
            transform.TransformPoint(local);


        return surfacePoint.y;
    }


    void OnDrawGizmos()
    {
        if (cachedRenderer == null)
        {
            UpdateRenderer();
        }


        if (cachedRenderer == null)
        {
            return;
        }


        Gizmos.color =
            Color.cyan;


        Matrix4x4 old =
            Gizmos.matrix;


        Gizmos.matrix =
            transform.localToWorldMatrix;


        Gizmos.DrawWireCube(
            Vector3.zero,
            Vector3.one
        );


        Gizmos.color =
            Color.blue;


        Vector3 center =
            transform.position
            +
            transform.up * 0.5f;


        Gizmos.DrawRay(
            center,
            transform.up
        );


        Gizmos.matrix =
            old;
    }
}