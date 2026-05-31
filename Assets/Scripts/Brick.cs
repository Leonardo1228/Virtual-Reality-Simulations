using UnityEngine;

public class Brick : SimulationBody
{
    public void ApplyImpact(
        Vector3 impact)
    {
        useGravity = true;

        AddForce(impact);

        AddTorque(
            Random.onUnitSphere * 500f
        );
    }
}
