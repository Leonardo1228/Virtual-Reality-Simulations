using UnityEngine;
using System.Collections.Generic;

public class VehicleCollision : MonoBehaviour
{
    public static List<BrickWall> allBrickWalls =
        new List<BrickWall>();

    public static List<HeavyWall> allWalls =
        new List<HeavyWall>();

    public static List<TutorialWall> allTutorialWalls =
        new List<TutorialWall>();

    [Header("Collision Points")]

    public Transform frontPoint;

    public Transform centerPoint;

    public Transform rearPoint;

    public float pointRadius = 1.2f;

    public float impactMultiplier = 5f;

    [Header("Vehicle Models")]

    public Transform bodyModel;

    public Transform frontWheelModel;

    public Transform rearWheelModel;

    public LayerMask BrickLayer;

    private VehicleController vehicle;

    void Awake()
    {
        vehicle =
            GetComponent<VehicleController>();
    }

    void Update()
    {
        if (frontWheelModel != null)
        {
            CheckBrickContacts(
                GetModelBounds(
                    frontWheelModel
                )
            );
        }

        if (bodyModel != null)
        {
            CheckBrickContacts(
                GetModelBounds(
                    bodyModel
                )
            );
        }

        if (rearWheelModel != null)
        {
            CheckBrickContacts(
                GetModelBounds(
                    rearWheelModel
                )
            );
        }

        CheckHeavyWallCollisions();

        CheckTutorialWallCollisions();
    }

    Bounds GetModelBounds(
        Transform model)
    {
        Renderer[] renderers =
            model.GetComponentsInChildren<
                Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(
                model.position,
                Vector3.one
            );
        }

        Bounds bounds =
            renderers[0].bounds;

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(
                r.bounds
            );
        }

        return bounds;
    }

    void CheckBrickContacts(
        Bounds vehicleBounds)
    {
        Collider[] hits =
            Physics.OverlapBox(
                vehicleBounds.center,
                vehicleBounds.extents,
                Quaternion.identity,
                BrickLayer
            );

        foreach (Collider hit in hits)
        {
            Rigidbody rb =
                hit.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }

            Vector3 impact =
                vehicle.velocity
                * vehicle.mass
                * 0.02f;

            rb.AddForce(
                impact,
                ForceMode.Impulse
            );
        }
    }

    void CheckHeavyWallCollisions()
    {
        foreach (HeavyWall wall in allWalls)
        {
            if (wall == null)
                continue;

            Vector3 originalVelocity =
                vehicle.velocity;

            bool collision =
                CheckCollision(frontPoint, wall)
                || CheckCollision(centerPoint, wall)
                || CheckCollision(rearPoint, wall);

            if (!collision)
                continue;

            float direction =
                Mathf.Sign(
                    Vector3.Dot(
                        originalVelocity,
                        vehicle.transform.forward
                    )
                );

            Vector3 impact =
                vehicle.transform.forward
                * direction
                * originalVelocity.magnitude
                * vehicle.mass
                * impactMultiplier;

            wall.ReceiveImpact(
                impact,
                vehicle
            );
        }
    }

    void CheckTutorialWallCollisions()
    {
        foreach (TutorialWall wall
            in allTutorialWalls)
        {
            if (wall == null)
                continue;

            CheckTutorialPoint(
                frontPoint,
                wall
            );

            CheckTutorialPoint(
                centerPoint,
                wall
            );

            CheckTutorialPoint(
                rearPoint,
                wall
            );
        }
    }

    void CheckTutorialPoint(
        Transform point,
        TutorialWall wall)
    {
        Bounds bounds =
            wall.GetWorldBounds();

        Vector3 p =
            point.position;

        if (!bounds.Contains(p))
            return;

        Vector3 center =
            bounds.center;

        Vector3 ext =
            bounds.extents;

        Vector3 local =
            p - center;

        float dx =
            ext.x - Mathf.Abs(local.x);

        float dy =
            ext.y - Mathf.Abs(local.y);

        float dz =
            ext.z - Mathf.Abs(local.z);

        float penetration =
            Mathf.Min(dx, dy, dz);

        Vector3 normal;

        if (penetration == dx)
        {
            normal =
                local.x > 0f
                ? Vector3.right
                : Vector3.left;
        }
        else if (penetration == dy)
        {
            normal =
                local.y > 0f
                ? Vector3.up
                : Vector3.down;
        }
        else
        {
            normal =
                local.z > 0f
                ? Vector3.forward
                : Vector3.back;
        }

        vehicle.transform.position +=
            normal
            * (penetration + 0.1f);

        float vn =
            Vector3.Dot(
                vehicle.velocity,
                normal
            );

        if (vn < 0f)
        {
            vehicle.velocity -=
                normal * vn;
        }
    }

    bool CheckCollision(
        Transform point,
        SimulationBody body)
    {
        Vector3 direction =
            point.position
            - body.transform.position;

        float distance =
            direction.magnitude;

        float minDistance =
            pointRadius + body.radius;

        if (distance < minDistance)
        {
            Vector3 normal =
                direction.normalized;

            float penetration =
                minDistance - distance;

            vehicle.transform.position +=
                normal
                * penetration;

            return true;
        }

        return false;
    }

    void DrawBounds(
        Transform model,
        Color color)
    {
        if (model == null)
            return;

        Bounds bounds =
            GetModelBounds(
                model
            );

        Gizmos.color =
            color;

        Gizmos.DrawWireCube(
            bounds.center,
            bounds.size
        );
    }

    void OnDrawGizmos()
    {
        DrawBounds(
            frontWheelModel,
            Color.green
        );

        DrawBounds(
            bodyModel,
            Color.yellow
        );

        DrawBounds(
            rearWheelModel,
            Color.red
        );
    }
}