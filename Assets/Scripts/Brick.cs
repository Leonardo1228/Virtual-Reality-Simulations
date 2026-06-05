using UnityEngine;

public class Brick : SimulationBody
{
    [Header("Brick")]

    public bool activated;

    public float damping = 0.995f;

    void Reset()
    {
        mass = 4f;

        drag = 0.05f;

        restitution = 0.25f;
    }

    void Start()
    {
        useGravity = false;

        if (mass <= 0f)
        {
            mass = 4f;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (activated)
        {
            velocity *= damping;
        }
    }

    public void Activate(
        Vector3 impactForce)
    {
        if (activated)
            return;

        activated = true;

        useGravity = true;

        AddForce(
            impactForce
        );

        AddTorque(
            Random.insideUnitSphere
            * impactForce.magnitude
            * 0.03f
        );
    }
}