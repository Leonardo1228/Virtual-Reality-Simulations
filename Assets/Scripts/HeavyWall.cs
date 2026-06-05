using UnityEngine;

public class HeavyWall : SimulationBody
{
    [Header("Wall")]

    public float resistanceForce =
        120000f;

    public float bounceDamping =
        0.25f;

    public float torqueMultiplier =
        0.02f;

    public bool startWithGravity =
        true;

    void Reset()
    {
        mass = 10000f;
        drag = 0.02f;
        restitution = 0.05f;
    }

    void OnEnable()
    {
        VehicleCollision.allWalls.Add(
            this
        );
    }

    void OnDisable()
    {
        VehicleCollision.allWalls.Remove(
            this
        );
    }

    void Start()
    {
        useGravity =
            startWithGravity;

        if (mass <= 0f)
        {
            mass = 10000f;
        }
    }

    public void ReceiveImpact(
        Vector3 impactForce,
        VehicleController vehicle)
    {
        float impact =
            impactForce.magnitude;

        Vector3 normal =
            (
                vehicle.transform.position
                - transform.position
            ).normalized;

        if (
            impact
            < resistanceForce
        )
        {
            vehicle.Bounce(
                normal,
                bounceDamping
            );

            vehicle.RegisterCollision();

            return;
        }

        AddForce(
            impactForce * 0.75f
        );

        AddTorque(
            Vector3.Cross(
                Vector3.up,
                impactForce.normalized
            )
            * impact
            * torqueMultiplier
        );

        vehicle.velocity *=
            0.8f;

        vehicle.RegisterCollision();
    }
}
