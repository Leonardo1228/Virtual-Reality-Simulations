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
        Vector3 localPoint =
            wall.transform.InverseTransformPoint(
                point.position
            );

        Vector3 ext =
            wall.Extents;

        bool inside =
            Mathf.Abs(localPoint.x) <= ext.x
            &&
            Mathf.Abs(localPoint.y) <= ext.y
            &&
            Mathf.Abs(localPoint.z) <= ext.z;

        if (!inside)
            return;

        float dx =
            ext.x - Mathf.Abs(localPoint.x);

        float dy =
            ext.y - Mathf.Abs(localPoint.y);

        float dz =
            ext.z - Mathf.Abs(localPoint.z);

        float minPen =
            Mathf.Min(dx, dy, dz);

        Vector3 localNormal =
            Vector3.zero;

        if (minPen == dx)
        {
            localNormal =
                localPoint.x > 0
                ? Vector3.right
                : Vector3.left;
        }
        else if (minPen == dy)
        {
            localNormal =
                localPoint.y > 0
                ? Vector3.up
                : Vector3.down;
        }
        else
        {
            localNormal =
                localPoint.z > 0
                ? Vector3.forward
                : Vector3.back;
        }

        Vector3 worldNormal =
            wall.transform.TransformDirection(
                localNormal
            );

        float safety =
            0.05f;

        vehicle.transform.position +=
            worldNormal
            * (minPen + safety);

        float vn =
            Vector3.Dot(
                vehicle.velocity,
                worldNormal
            );

        if (vn < 0f)
        {
            vehicle.velocity -=
                worldNormal * vn;
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