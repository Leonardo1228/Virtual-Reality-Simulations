using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public Vector3 velocity;

    public float mass = 1f;
    public float drag = 0.1f;
    public float restitution = 0.2f;

    public bool grounded;
    public Vector3 groundNormal = Vector3.up;

    public bool isStatic;

    void OnEnable() => PhysicsSolver.Register(this);
    void OnDisable() => PhysicsSolver.Unregister(this);

    // =========================
    // ONLY VALID API
    // =========================

    public void AddForce(Vector3 force)
    {
        velocity += force / Mathf.Max(0.001f, mass);
    }

    public void AddImpulse(Vector3 impulse)
    {
        velocity += impulse / Mathf.Max(0.001f, mass);
    }
}
