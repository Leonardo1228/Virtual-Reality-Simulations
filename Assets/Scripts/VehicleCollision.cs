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

    [Header("Collision Boxes")]

    public CollisionBox frontWheel =
        new CollisionBox();

    public CollisionBox body =
        new CollisionBox();

    public CollisionBox rearWheel =
        new CollisionBox();

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
        CheckBrickBox(frontWheel);
        CheckBrickBox(body);
        CheckBrickBox(rearWheel);

        CheckHeavyWalls();

        CheckTutorialWalls();
    }

    void CheckBrickBox(
        CollisionBox box)
    {
        if (!box.enabled)
            return;

        if (box.anchor == null)
            return;

        if (
            vehicle.velocity.magnitude
            < minimumBreakSpeed
        )
        {
            return;
        }

        Collider[] hits =
            Physics.OverlapBox(
                box.WorldCenter(),
                box.size * 0.5f,
                box.WorldRotation(),
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

            Vector3 impact =
                vehicle.velocity
                * vehicle.mass
                * impactMultiplier;

            brick.Activate(
                impact
            );

            vehicle.velocity *=
                brickEnergyLoss;
        }
    }

    void CheckHeavyWalls()
    {
        foreach (
            HeavyWall wall
            in allWalls
        )
        {
            if (wall == null)
                continue;

            bool collision =
                CheckHeavyBox(
                    frontWheel,
                    wall
                )
                || CheckHeavyBox(
                    body,
                    wall
                )
                || CheckHeavyBox(
                    rearWheel,
                    wall
                );

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

    bool CheckHeavyBox(
        CollisionBox box,
        HeavyWall wall)
    {
        if (!box.enabled)
            return false;

        if (box.anchor == null)
            return false;

        BoxCollider wallCollider =
            wall.GetComponent<BoxCollider>();

        if (wallCollider == null)
            return false;

        Vector3 center =
            box.WorldCenter();

        Vector3 closest =
            wallCollider.ClosestPoint(
                center
            );

        bool inside =
            Vector3.Distance(
                center,
                closest
            ) < 0.01f;

        if (!inside)
            return false;

        vehicle.RegisterCollision();

        return true;
    }

    void CheckTutorialWalls()
    {
        foreach (
            TutorialWall wall
            in allTutorialWalls
        )
        {
            if (wall == null)
                continue;

            CheckTutorialBox(
                frontWheel,
                wall
            );

            CheckTutorialBox(
                body,
                wall
            );

            CheckTutorialBox(
                rearWheel,
                wall
            );
        }
    }

    void CheckTutorialBox(
        CollisionBox box,
        TutorialWall wall)
    {
        if (!box.enabled)
            return;

        if (box.anchor == null)
            return;

        Bounds bounds =
            wall.GetWorldBounds();

        if (
            bounds.Contains(
                box.WorldCenter()
            )
        )
        {
            vehicle.velocity *=
                0.9f;
        }
    }

    void DrawBox(
        CollisionBox box,
        Color color)
    {
        if (!box.enabled)
            return;

        if (box.anchor == null)
            return;

        Gizmos.color =
            color;

        Matrix4x4 old =
            Gizmos.matrix;

        Gizmos.matrix =
            Matrix4x4.TRS(
                box.WorldCenter(),
                box.WorldRotation(),
                Vector3.one
            );

        Gizmos.DrawWireCube(
            Vector3.zero,
            box.size
        );

        Gizmos.matrix =
            old;
    }

    void OnDrawGizmos()
    {
        DrawBox(
            frontWheel,
            Color.green
        );

        DrawBox(
            body,
            Color.yellow
        );

        DrawBox(
            rearWheel,
            Color.red
        );
    }
}