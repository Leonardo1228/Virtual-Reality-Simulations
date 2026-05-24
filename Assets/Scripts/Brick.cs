using UnityEngine;

public class Brick : SimulationBody
{
    public float breakForce = 5000f;

    void OnEnable()
    {
        VehicleCollision.allBricks.Add(this);
    }

    void OnDisable()
    {
        VehicleCollision.allBricks.Remove(this);
    }

    void Start()
    {
        useGravity = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ApplyImpact(Vector3 impact)
    {
        AddForce(impact);

        AddTorque(
            Random.onUnitSphere * 500f
        );
    }

    void LateUpdate()
    {
        foreach (Brick other in VehicleCollision.allBricks)
        {
            if (other == this)
                continue;

            if (other == null)
                continue;

            BodyCollisionSolver.Resolve(
                this,
                other
            );
        }
    }
}