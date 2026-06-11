using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Brick : MonoBehaviour
{
    [Header("Brick")]
    public bool activated = false;

    [Header("Arcade Settings")]
    [Range(0f, 1f)]
    public float bounceLoss = 0.3f;

    [Header("Activation")]
    public float activationThreshold = 5f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!activated)
        {
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float impact = collision.impulse.magnitude;

        if (!activated && impact > activationThreshold)
        {
            Activate(collision);
        }

        if (activated)
        {
            ApplyDamageResponse();
        }
    }

    void Activate(Collision collision)
    {
        activated = true;

        rb.isKinematic = false;

        rb.AddForce(
            collision.impulse,
            ForceMode.Impulse
        );
    }

    void ApplyDamageResponse()
    {
        rb.linearVelocity *= (1f - bounceLoss);
    }
}