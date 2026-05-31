using UnityEngine;
using System.Collections.Generic;

public class VehicleCollision : MonoBehaviour
{
    public static List<BrickWall> allBrickWalls =
        new List<BrickWall>();

    public static List<HeavyWall> allWalls =
        new List<HeavyWall>();

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