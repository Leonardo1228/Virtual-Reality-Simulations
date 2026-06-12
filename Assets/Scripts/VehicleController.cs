using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HybridVehicle : MonoBehaviour
{
    [Header("Control")]
    public bool isPlayerControlled = true;

    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 25f;

    [Header("Turning")]
    public float turnSpeed = 90f;

    Rigidbody rb;

    float moveInput;
    float turnInput;

    public float MoveInput => moveInput;
    public float TurnInput => turnInput;

    public float CurrentSpeed =>
    rb.linearVelocity.magnitude;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = 1000f;
        rb.linearDamping = 0.2f;
        rb.angularDamping = 2f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // El coche no se vuelca, pero sí se mueve libremente
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!isPlayerControlled)
        {
            moveInput = 0f;
            turnInput = 0f;
            return;
        }

        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        ApplyEngine();
        ApplySteering();
    }

    void ApplyEngine()
    {
        // Velocidad actual en la dirección del coche
        float currentForwardSpeed =
            Vector3.Dot(
                rb.linearVelocity,
                transform.forward
            );

        // A dónde queremos llegar
        float targetSpeed =
            moveInput * maxSpeed;

        float speedDifference =
            targetSpeed - currentForwardSpeed;

        // Fuerza para acercarse a esa velocidad
        Vector3 force =
            transform.forward *
            speedDifference *
            acceleration;

        rb.AddForce(
            force,
            ForceMode.Force
        );
    }

    void ApplySteering()
    {
        // No gira si está completamente quieto
        if (rb.linearVelocity.magnitude < 0.5f)
            return;

        float rotation =
            turnInput *
            turnSpeed *
            Time.fixedDeltaTime;

        Quaternion turn =
            Quaternion.Euler(
                0f,
                rotation,
                0f
            );

        rb.MoveRotation(
            rb.rotation * turn
        );
    }
}