using UnityEngine;

public class Brick : UnifiedPhysicsBody
{
    [Header("Brick")]
    public bool activated;
    public float damping = 0.995f;

    void Reset()
    {
        mass = 4f;
    }

    void Start()
    {
        useGravity = false;

        if (mass <= 0f)
            mass = 4f;
    }

    void Update()
    {

        if (!activated)
            return;

        velocity *= damping;
    }

    public void Activate(Vector3 impactForce)
    {
        if (activated)
            return;

        activated = true;
        useGravity = true;

        AddForce(impactForce);

        AddForce(
            Vector3.up * impactForce.magnitude * 0.2f
        );
    }
}