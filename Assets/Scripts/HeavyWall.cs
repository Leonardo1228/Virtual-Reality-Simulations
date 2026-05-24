using UnityEngine;

public class HeavyWall : SimulationBody
{
    [Header("Wall")]

    public float resistanceForce = 30000f;

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
        float force =
            impactForce.magnitude;

        // Rebote del carro
        if (force < resistanceForce)
        {
            Vector3 normal =
                (vehicle.transform.position
                - transform.position)
                .normalized;

            vehicle.Bounce(
                normal,
                0.15f
            );

            return;
        }

        AddForce(impactForce);

        AddTorque(
            Random.onUnitSphere * 2000f
        );
    }
}
