using UnityEngine;

public class HeavyWall : SimulationBody
{
    [Header("Wall")]

    public float resistanceForce = 50000f;

    public float bounceDamping = 0.25f;

    public float torqueMultiplier = 0.02f;

    public bool startWithGravity = true;

    void OnEnable()
    {
        VehicleCollision.allWalls.Add(this);
    }

    void OnDisable()
    {
        VehicleCollision.allWalls.Remove(this);
    }

    void Start()
    {
        useGravity = startWithGravity;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ReceiveImpact(
        Vector3 impactForce,
        VehicleController vehicle)
    {
        float impact =
            impactForce.magnitude;

        // Muro demasiado fuerte
        if (impact < resistanceForce)
        {
            Vector3 normal =
                (vehicle.transform.position
                - transform.position)
                .normalized;

            vehicle.Bounce(
                normal,
                bounceDamping
            );

            return;
        }

        // El muro recibe fuerza
        AddForce(
            impactForce
        );

        // Comienza a volcarse
        AddTorque(
            Vector3.Cross(
                Vector3.up,
                impactForce.normalized
            )
            * impact
            * torqueMultiplier
        );
    }
}
