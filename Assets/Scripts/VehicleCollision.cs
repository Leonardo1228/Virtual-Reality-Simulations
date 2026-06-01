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

    public float impactMultiplier = 100f;

    private VehicleController vehicle;

    void Awake()
    {
        vehicle =
            GetComponent<VehicleController>();
    }

    void Update()
    {
        CheckBrickWallCollisions();

        CheckHeavyWallCollisions();

        CheckTutorialWallCollisions();
    }

    void CheckBrickWallCollisions()
    {
        foreach (BrickWall wall in allBrickWalls)
        {
            if (wall == null)
                continue;

            bool collision =
                CheckWallPoint(frontPoint, wall)
                || CheckWallPoint(centerPoint, wall)
                || CheckWallPoint(rearPoint, wall);

            if (!collision)
                continue;

            Vector3 impact =
                vehicle.velocity
                * vehicle.mass
                * impactMultiplier;

            wall.ReceiveImpact(
                impact
            );
        }
    }

    void CheckHeavyWallCollisions()
    {
        foreach (HeavyWall wall in allWalls)
        {
            if (wall == null)
                continue;

            bool collision =
                CheckCollision(frontPoint, wall)
                || CheckCollision(centerPoint, wall)
                || CheckCollision(rearPoint, wall);

            if (!collision)
                continue;

            Vector3 impact =
                vehicle.velocity
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

    bool CheckWallPoint(
        Transform point,
        BrickWall wall)
    {
        float wallRadius =
            Mathf.Max(
                wall.transform.localScale.x,
                wall.transform.localScale.y
            ) * 0.5f;

        float distance =
            Vector3.Distance(
                point.position,
                wall.transform.position
            );

        return distance <
            pointRadius + wallRadius;
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

            vehicle.velocity =
                Vector3.Reflect(
                    vehicle.velocity,
                    normal
                ) * 0.2f;

            return true;
        }

        return false;
    }
}