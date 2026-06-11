using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public PhysicsBody body;

    public float acceleration = 20f;
    public float brake = 25f;
    public float maxSpeed = 35f;
    public float steering = 140f;

    float move;
    float steer;
    private float moveInput;
    private float steerInput;
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
        move = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");

        Drive();
        Rotate();
    }

    void Drive()
    {
        Vector3 forward = transform.forward;

        if (move > 0)
            body.velocity += forward * move * acceleration * Time.deltaTime;

        if (move < 0)
            body.velocity -= forward * brake * Time.deltaTime;

        Vector3 flat = new Vector3(body.velocity.x, 0, body.velocity.z);

        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            body.velocity.x = flat.x;
            body.velocity.z = flat.z;
        }

        float control = body.grounded ? 1f : 0.3f;
        yaw += steer * steering * control * Time.deltaTime;
    }

    void Rotate()
    {
        Vector3 up = body.grounded ? body.groundNormal : Vector3.up;

        Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up),
            10f * Time.deltaTime
        );
    }
}