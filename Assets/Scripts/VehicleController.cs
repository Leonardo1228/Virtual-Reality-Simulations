using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 20f;
    public float turnTorque = 8f;

    [Header("Speed Limit")]
    public float maxSpeed = 10f;

    Rigidbody rb;

    float move;
    float turn;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        move = Input.GetAxisRaw("Vertical");   // W/S
        turn = Input.GetAxisRaw("Horizontal"); // A/D
    }

    void FixedUpdate()
    {
        Move();
        Turn();
        LimitSpeed();
    }

    // =========================
    // MOVEMENT (forward/back)
    // =========================
    void Move()
    {
        rb.AddForce(transform.forward * move * moveForce, ForceMode.Force);
    }

    // =========================
    // ROTATION (left/right)
    // =========================
    void Turn()
    {
        rb.AddTorque(Vector3.up * turn * turnTorque, ForceMode.Force);
    }

    // =========================
    // SPEED LIMIT
    // =========================
    void LimitSpeed()
    {
        Vector3 flat = rb.linearVelocity;
        flat.y = 0f;

        if (flat.magnitude > maxSpeed)
        {
            Vector3 limited = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }
}