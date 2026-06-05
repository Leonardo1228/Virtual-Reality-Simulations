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

    [Header("Vehicle Sensors")]

    public CollisionSensor frontWheel =
        new CollisionSensor();

    public CollisionSensor body =
        new CollisionSensor();

    public CollisionSensor rearWheel =
        new CollisionSensor();

    [Header("Brick Settings")]

    public float minimumBreakSpeed =
        3f;

    public float brickEnergyLoss =
        0.995f;

    public float impactMultiplier =
        5f;

    public LayerMask BrickLayer;

    private VehicleController vehicle;

    void Awake()
    {
        vehicle =
            GetComponent<VehicleController>();
    }

    void Update()
    {
        CheckBrickSensor(
            frontWheel
        );

        CheckBrickSensor(
            body
        );

        CheckBrickSensor(
            rearWheel
        );

        CheckHeavyWallCollisions();

        CheckTutorialWallCollisions();
    }

    void CheckBrickSensor(
        CollisionSensor sensor)
    {
        if (!sensor.enabled)
            return;

        if (sensor.anchor == null)
            return;

        if (
            vehicle.velocity.magnitude
            < minimumBreakSpeed
        )
        {
            return;
        }

        Vector3 center =
            sensor.WorldPosition();

        Collider[] hits =
            Physics.OverlapSphere(
                center,
                sensor.radius,
                BrickLayer
            );

        foreach (Collider hit in hits)
        {
            Brick brick =
                hit.GetComponent<Brick>();

            if (brick == null)
                continue;

            if (brick.activated)
                continue;

            float distance =
                Vector3.Distance(
                    center,
                    brick.transform.position
                );

            float strength =
                1f -
                Mathf.Clamp01(
                    distance /
                    sensor.radius
                );

            Vector3 impact =
                vehicle.velocity
                * vehicle.mass
                * impactMultiplier
                * strength;

            brick.Activate(
                impact
            );

            vehicle.velocity *=
                brickEnergyLoss;
        }
    }

    void CheckHeavyWallCollisions()
    {
        foreach (
            HeavyWall wall
            in allWalls
        )
        {
            if (wall == null)
                continue;

            Vector3 originalVelocity =
                vehicle.velocity;

            bool collision =
                CheckWallSensor(
                    frontWheel,
                    wall
                )
                || CheckWallSensor(
                    body,
                    wall
                )
                || CheckWallSensor(
                    rearWheel,
                    wall
                );

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

    bool CheckWallSensor(
        CollisionSensor sensor,
        SimulationBody wall)
    {
        if (!sensor.enabled)
            return false;

        if (sensor.anchor == null)
            return false;

        Vector3 center =
            sensor.WorldPosition();

        Vector3 direction =
            center
            - wall.transform.position;

        float distance =
            direction.magnitude;

        float minDistance =
            sensor.radius
            + wall.radius;

        if (distance >= minDistance)
            return false;

        Vector3 normal =
            direction.normalized;

        float penetration =
            minDistance
            - distance;

        vehicle.transform.position +=
            normal
            * penetration;

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

        return true;
    }

    void CheckTutorialWallCollisions()
    {
        foreach (
            TutorialWall wall
            in allTutorialWalls
        )
        {
            if (wall == null)
                continue;

            CheckTutorialSensor(
                frontWheel,
                wall
            );

            CheckTutorialSensor(
                body,
                wall
            );

            CheckTutorialSensor(
                rearWheel,
                wall
            );
        }
    }

    void CheckTutorialSensor(
        CollisionSensor sensor,
        TutorialWall wall)
    {
        if (!sensor.enabled)
            return;

        if (sensor.anchor == null)
            return;

        Bounds bounds =
            wall.GetWorldBounds();

        Vector3 center =
            sensor.WorldPosition();

        Vector3 closest =
            bounds.ClosestPoint(
                center
            );

        float distance =
            Vector3.Distance(
                center,
                closest
            );

        if (
            distance >
            sensor.radius
        )
        {
            return;
        }

        Vector3 normal =
            (
                center
                - closest
            ).normalized;

        if (
            normal.sqrMagnitude
            < 0.001f
        )
        {
            normal =
                -vehicle.transform.forward;
        }

        float penetration =
            sensor.radius
            - distance;

        vehicle.transform.position +=
            normal
            * (
                penetration
                + 0.05f
            );

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

    void DrawSensor(
        CollisionSensor sensor,
        Color color)
    {
        if (
            sensor == null
            || !sensor.enabled
            || sensor.anchor == null
        )
        {
            return;
        }

        Gizmos.color =
            color;

        Gizmos.DrawWireSphere(
            sensor.WorldPosition(),
            sensor.radius
        );
    }

    void OnDrawGizmos()
    {
        DrawSensor(
            frontWheel,
            Color.green
        );

        DrawSensor(
            body,
            Color.yellow
        );

        DrawSensor(
            rearWheel,
            Color.red
        );
    }
}