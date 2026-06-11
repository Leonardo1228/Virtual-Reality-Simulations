using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public PhysicsBody body;

    public float acceleration = 20f;
    public float brake = 25f;
    public float maxSpeed = 35f;
    public float steering = 140f;

    float moveInput;
    float steerInput;

    public float MoveInput => moveInput;
    public float SteerInput => steerInput;

    float yaw;

    void Start()
    {
        if (body == null)
            body = GetComponent<PhysicsBody>();

        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        Drive();
        Rotate();
    }

    void Drive()
    {
        float dt = Time.deltaTime;

        Vector3 forward =
            Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

        forward.y = 0f;
        forward.Normalize();

        // =========================
        // ACCELERATION / BRAKE
        // =========================

        if (moveInput > 0f)
        {
            body.velocity += forward * moveInput * acceleration * dt;
        }
        else if (moveInput < 0f)
        {
            body.velocity -= forward * brake * dt;
        }

        // =========================
        // SPEED LIMIT
        // =========================

        Vector3 flat = new Vector3(body.velocity.x, 0f, body.velocity.z);

        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;

            body.velocity.x = flat.x;
            body.velocity.z = flat.z;
        }

        // =========================
        // STEERING
        // =========================

        float control = body.grounded ? 1f : 0.3f;

        yaw += steerInput * steering * control * dt;
    }

    void Rotate()
    {
        Vector3 up = body.grounded ? body.groundNormal : Vector3.up;

        Vector3 forward =
            Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

        forward = Vector3.ProjectOnPlane(forward, up).normalized;

        if (forward.sqrMagnitude < 0.001f)
            forward = transform.forward;

        Quaternion target =
            Quaternion.LookRotation(forward, up);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                target,
                10f * Time.deltaTime
            );
    }
}