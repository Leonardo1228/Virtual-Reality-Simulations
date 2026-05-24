using UnityEngine;
using System.Collections.Generic;

public class VehicleCollision : MonoBehaviour
{
    public static List<Brick> allBricks =
        new List<Brick>();

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
        CheckBrickCollisions();

        CheckHeavyWallCollisions();
    }

    void CheckBrickCollisions()
    {
        foreach (Brick brick in allBricks)
        {
            if (brick == null)
                continue;

            bool collision =
                CheckCollision(frontPoint, brick)
                || CheckCollision(centerPoint, brick)
                || CheckCollision(rearPoint, brick);

            if (!collision)
                continue;

            Vector3 impact =
                vehicle.velocity
                * vehicle.mass
                * impactMultiplier;

            brick.ApplyImpact(impact);
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

    bool CheckCollision(
        Transform point,
        SimulationBody body)
    {
        float distance =
            Vector3.Distance(
                point.position,
                body.transform.position
            );

        return distance <
            pointRadius + body.radius;
    }
}