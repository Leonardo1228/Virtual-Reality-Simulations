using UnityEngine;

public class HeavyWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public bool fixedWall = true;
    public float breakImpulseThreshold = 40f;

    [Header("Damage")]
    public float damageMultiplier = 1f;

    [Header("Layers")]
    public LayerMask validImpactLayers;

    Rigidbody rb;
    bool broken;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (fixedWall && rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (broken) return;

        if ((validImpactLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        float impact = collision.impulse.magnitude;

        if (impact < 0.1f)
            return;

        HandleImpact(collision, impact);
    }

    void HandleImpact(Collision collision, float impact)
    {
        // daño opcional (puedes expandir esto después)
        float damage = impact * damageMultiplier;

        if (impact > breakImpulseThreshold)
        {
            Break(collision);
        }
    }

    void Break(Collision collision)
    {
        broken = true;

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // empuje físico real del choque
        Vector3 force = collision.relativeVelocity * rb.mass;
        rb.AddForce(force, ForceMode.Impulse);
    }
}