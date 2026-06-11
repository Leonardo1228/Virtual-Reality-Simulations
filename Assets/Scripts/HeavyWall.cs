using UnityEngine;

public class HeavyWall : MonoBehaviour
{
    [Header("Wall")]
    public bool fixedWall = true;

    [Header("Arcade Response")]
    public float breakForceThreshold = 40f;

    [Header("Damping")]
    public float impactDamping = 0.2f;

    private PhysicsBody body;

    void Awake()
    {
        body = GetComponent<PhysicsBody>();
    }

    // llamado por el solver cuando hay impacto (opcional hook)
    public void ApplyImpact(Vector3 velocityBeforeHit, Vector3 normal)
    {
        if (body == null) return;

        float impactForce = velocityBeforeHit.magnitude;

        // pérdida de energía tipo GTA
        body.velocity *= (1f - impactDamping);

        if (!fixedWall && impactForce > breakForceThreshold)
        {
            MakeDynamic(velocityBeforeHit * 0.5f);
        }
    }

    public void MakeDynamic(Vector3 impulse)
    {
        if (body == null) return;

        body.isStatic = false;


        body.velocity += impulse / Mathf.Max(0.001f, body.mass);
    }

    public void Freeze()
    {
        if (body == null) return;

        body.velocity = Vector3.zero;
        body.isStatic = true;

    }
}