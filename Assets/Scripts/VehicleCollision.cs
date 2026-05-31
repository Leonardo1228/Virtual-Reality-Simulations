using UnityEngine;
using System.Collections.Generic;

public class VehicleCollision : MonoBehaviour
{
    public static List<BrickWall> allBrickWalls =
        new List<BrickWall>();

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
    }

    void CheckBrickWallCollisions()
    {
        foreach (BrickWall wall in allBrickWalls)
        {
            if (wall == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    wall.transform.position
                );

            float wallRadius =
                Mathf.Max(
                    wall.transform.localScale.x,
                    wall.transform.localScale.y
                );

            if (distance >
                pointRadius + wallRadius)
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
            // Normal
            Vector3 normal =
                direction.normalized;

            // Penetración
            float penetration =
                minDistance - distance;

            // Empujar vehículo hacia afuera
            vehicle.transform.position +=
                normal
                * penetration;

            // Rebote ligero
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