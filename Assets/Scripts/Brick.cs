using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Brick : MonoBehaviour
{
    [Header("State")]
    public bool activated = false;

    [Header("Activation")]
    public float activationThreshold = 5f;

    [Header("Layers")]
    public LayerMask affectedBy;

    Rigidbody rb;

    public Rigidbody Rigidbody => rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = !activated;
    }


    void OnCollisionEnter(Collision collision)
    {
        // Ya está suelto, PhysX se encarga
        if (activated)
            return;


        // ¿Esta capa puede romper el ladrillo?
        if ((affectedBy.value & (1 << collision.gameObject.layer)) == 0)
            return;


        // ¿El golpe fue suficientemente fuerte?
        if (collision.impulse.magnitude < activationThreshold)
            return;


        Activate();
    }


    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        rb.isKinematic = false;

        rb.WakeUp();
    }
}