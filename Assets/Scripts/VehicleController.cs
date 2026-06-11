using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 35f;
    public float reverseForce = 25f;
    public float steering = 120f;
    public float maxSpeed = 25f;

    [Header("Grip")]
    public float lateralGrip = 6f;

    [Header("Ground")]
    public LayerMask groundMask;
    public float groundCheckDistance = 1.2f;
    public float groundForce = 25f;

    [Header("Stability")]
    public float downForce = 10f;

    Rigidbody rb;

    float move;
    float steer;

    public bool isPlayerControlled = true;

    public float SteerInput => steer;
    public float MoveInput => move;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!isPlayerControlled)
        {
            move = 0f;
            steer = 0f;
            return;
        }

        move = Input.GetAxisRaw("Vertical");
        steer = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        ApplyEngine();
        ApplySteering();
        ApplyGrip();
        ApplyDownForce();
        StickToGround();
        ClampSpeed();
    }

    // =========================
    // ENGINE
    // =========================
    void ApplyEngine()
    {
        Vector3 forward = transform.forward;

        if (move > 0f)
        {
            rb.AddForce(forward * move * acceleration, ForceMode.Force);
        }
        else if (move < 0f)
        {
            rb.AddForce(-forward * Mathf.Abs(move) * reverseForce, ForceMode.Force);
        }
    }

    // =========================
    // STEERING
    // =========================
    void ApplySteering()
    {
        float speed = rb.linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(speed / 10f);

        float turn = steer * steering * speedFactor;

        rb.AddTorque(Vector3.up * turn, ForceMode.Force);
    }

    // =========================
    // GRIP (anti-drift arcade)
    // =========================
    void ApplyGrip()
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);

        localVel.x = Mathf.Lerp(localVel.x, 0f, lateralGrip * Time.fixedDeltaTime);

        Vector3 corrected = transform.TransformDirection(localVel);

        rb.AddForce(corrected - rb.linearVelocity, ForceMode.VelocityChange);
    }

    // =========================
    // DOWN FORCE (evita vuelo raro)
    // =========================
    void ApplyDownForce()
    {
        rb.AddForce(-transform.up * downForce, ForceMode.Force);
    }

    // =========================
    // SPEED LIMIT
    // =========================
    void ClampSpeed()
    {
        Vector3 flat = rb.linearVelocity;
        flat.y = 0f;

        if (flat.magnitude > maxSpeed)
        {
            Vector3 limited = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    // =========================
    // GROUND STICK (ligero, no invasivo)
    // =========================
    void StickToGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            Vector3 slopeForward = Vector3.ProjectOnPlane(transform.forward, hit.normal);

            rb.AddForce(-hit.normal * groundForce, ForceMode.Force);
            rb.AddForce(slopeForward * move * groundForce, ForceMode.Force);
        }
    }
}
