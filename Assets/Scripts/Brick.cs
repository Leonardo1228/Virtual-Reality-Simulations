using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Brick")]
    public bool activated = false;

    [Header("Arcade Settings")]
    [Range(0f, 1f)]
    public float bounceLoss = 0.3f;

    private PhysicsBody body;

    void Awake()
    {
        body = GetComponent<PhysicsBody>();
    }

    void Reset()
    {
        if (body == null) body = GetComponent<PhysicsBody>();

        if (body != null)
        {
            body.mass = 4f;
            body.drag = 0f;
            body.restitution = 0.2f;
            body.isStatic = true;
            body.useGravity = false;
        }
    }

    // =========================
    // CALLED BY SOLVER ONLY
    // =========================
    public void ApplyImpact(Vector3 velocityBeforeHit, Vector3 normal)
    {
        float force = velocityBeforeHit.magnitude;

        if (!activated && force > 5f)
        {
            Activate(normal * force);
        }

        if (activated && body != null)
        {
            // pérdida de energía tipo arcade
            body.velocity *= (1f - bounceLoss);
        }
    }

    public void Activate(Vector3 impulse)
    {
        if (activated) return;

        activated = true;

        if (body == null) return;

        body.isStatic = false;
        body.useGravity = true;

        body.velocity += impulse / Mathf.Max(0.001f, body.mass);
    }
}