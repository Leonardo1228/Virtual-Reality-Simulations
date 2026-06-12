using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeavyWall : MonoBehaviour
{
    [Header("Wall")]
    public bool fixedWall = true;

    public float breakImpulseThreshold = 40f;

    [Header("Layers")]
    public LayerMask validImpactLayers;

    Rigidbody rb;

    bool broken;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (fixedWall)
        {
            rb.isKinematic = true;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (broken)
            return;


        if ((validImpactLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;


        float impact =
            collision.impulse.magnitude;


        if (impact >= breakImpulseThreshold)
        {
            Break(collision);
        }
    }


    public void Break(Collision collision)
    {
        broken = true;

        rb.isKinematic = false;

        rb.WakeUp();


        // Transferimos la fuerza del golpe
        rb.AddForce(
            collision.impulse,
            ForceMode.Impulse
        );
    }


    public void Break()
    {
        broken = true;

        rb.isKinematic = false;

        rb.WakeUp();
    }
}