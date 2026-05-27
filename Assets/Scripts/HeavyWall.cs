using UnityEngine;

public class HeavyWall : SimulationBody
{
    [Header("Wall")]

    public float resistanceForce = 35000f;

    public float bounceForce = 0.15f;

    public float collapseTorque = 2000f;

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

        Vector3 normal =
            (
                vehicle.transform.position
                - transform.position
            ).normalized;

        // MURO RESISTE
        if (force < resistanceForce)
        {
            vehicle.Bounce(
                normal,
                bounceForce
            );

            return;
        }

        // MURO CEDE
        AddForce(impactForce);

        AddTorque(
            Random.onUnitSphere
            * collapseTorque
        );
    }
}
