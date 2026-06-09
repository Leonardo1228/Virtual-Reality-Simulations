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

    public static List<VehicleController> allVehicles =
    new List<VehicleController>();

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

    [Header("Vehicle Collision")]

    public float vehicleBounce =
    0.35f;

    public float vehicleEnergyLoss =
        0.8f;

    public float vehicleSeparation =
        0.2f;

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


        CheckVehicleCollisions();
    }

    void CheckVehicleCollisions()
    {
        foreach (
            VehicleController other
            in allVehicles
        )
        {
            if (other == null)
                continue;


            if (other == vehicle)
                continue;


            VehicleCollision otherCollision =
                other.GetComponent<VehicleCollision>();

            if (otherCollision == null)
                continue;


            bool collision =
                CheckVehicleBoxes(
                    frontWheel,
                    otherCollision
                )

                ||

                CheckVehicleBoxes(
                    body,
                    otherCollision
                )

                ||

                CheckVehicleBoxes(
                    rearWheel,
                    otherCollision
                );


            if (collision)
            {
                ResolveVehicleCollision(
                    other
                );
            }
        }
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

        Collider[] hits =
            Physics.OverlapBox(
                box.WorldCenter(),
                box.size * 0.5f,
                box.WorldRotation()
            );

        foreach (Collider hit in hits)
        {
            if (hit == wallCollider)
            {
                Vector3 pushDirection =
                    (
                        vehicle.transform.position
                        - wall.transform.position
                    ).normalized;

                vehicle.transform.position +=
                    pushDirection * 0.2f;

                return true;
            }
        }

        return false;
    }

    bool CheckVehicleBoxes(
    CollisionBox myBox,
    VehicleCollision other)
    {
        if (!myBox.enabled)
            return false;

        if (myBox.anchor == null)
            return false;

        return
            BoxOverlap(
                myBox,
                other.frontWheel
            )

            ||

            BoxOverlap(
                myBox,
                other.body
            )

            ||

            BoxOverlap(
                myBox,
                other.rearWheel
            );
    }

    bool BoxOverlap(
    CollisionBox a,
    CollisionBox b)
    {
        if (!b.enabled)
            return false;

        if (b.anchor == null)
            return false;


        Vector3 aCenter =
            a.WorldCenter();

        Vector3 bCenter =
            b.WorldCenter();


        Vector3 aHalf =
            a.size * 0.5f;

        Vector3 bHalf =
            b.size * 0.5f;


        return
            Mathf.Abs(
                aCenter.x - bCenter.x
            )
            <=
            aHalf.x + bHalf.x

            &&

            Mathf.Abs(
                aCenter.y - bCenter.y
            )
            <=
            aHalf.y + bHalf.y

            &&

            Mathf.Abs(
                aCenter.z - bCenter.z
            )
            <=
            aHalf.z + bHalf.z;
    }

    void ResolveVehicleCollision(
    VehicleController other)
    {
        Vector3 normal =
            (
                other.transform.position
                - transform.position
            ).normalized;


        Vector3 relativeVelocity =
            vehicle.velocity
            - other.velocity;


        float impactSpeed =
            Vector3.Dot(
                relativeVelocity,
                normal
            );


        if (impactSpeed <= 0f)
            return;


        float totalMass =
            vehicle.mass
            + other.mass;


        float impulse =
            impactSpeed
            * vehicleBounce;


        Vector3 force =
            normal
            * impulse;


        vehicle.velocity -=
            force
            * (
                other.mass
                / totalMass
            );


        other.velocity +=
            force
            * (
                vehicle.mass
                / totalMass
            );


        vehicle.velocity *=
            vehicleEnergyLoss;


        other.velocity *=
            vehicleEnergyLoss;


        Vector3 separation =
            normal
            * vehicleSeparation;


        transform.position -=
            separation * 0.5f;


        other.transform.position +=
            separation * 0.5f;


        vehicle.RegisterCollision();

        other.RegisterCollision();
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