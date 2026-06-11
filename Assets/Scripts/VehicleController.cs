using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    public float acceleration = 35f;
    public float brakeForce = 45f;
    public float steering = 120f;
    public float maxSpeed = 30f;

    public float traction = 8f; // control lateral

    Rigidbody rb;

    float move;
    float steer;

    public float SteerInput => steer;
    public float MoveInput => move;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        move = Input.GetAxisRaw("Vertical");   // W/S
        steer = Input.GetAxisRaw("Horizontal"); // A/D
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
        ApplySideFriction();
        ClampSpeed();
        StickToGround();
    }

    #region ENGINE
    void ApplyEngineForce()
    {
        Vector3 forward = transform.forward;

        if (move > 0f)
        {
            rb.AddForce(forward * acceleration * move, ForceMode.Force);
        }
        else if (move < 0f)
        {
            rb.AddForce(-rb.linearVelocity.normalized * brakeForce, ForceMode.Force);
        }
    }
    #endregion

    #region STEERING
    void ApplySteering()
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 10f);

        float turn = steer * steering * speedFactor;

        rb.AddTorque(Vector3.up * turn, ForceMode.Force);
    }
    #endregion

    #region STABILITY
    void ApplySideFriction()
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);

        localVel.x = Mathf.Lerp(localVel.x, 0, traction * Time.fixedDeltaTime);

        rb.linearVelocity = transform.TransformDirection(localVel);
    }
    #endregion

    #region SPEED LIMIT
    void ClampSpeed()
    {
        Vector3 flat = rb.linearVelocity;
        flat.y = 0;

        if (flat.magnitude > maxSpeed)
        {
            Vector3 limited = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }
    #endregion
    void StickToGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal);
            rb.AddForce(slopeDir * 20f, ForceMode.Force);
        }
    }
}