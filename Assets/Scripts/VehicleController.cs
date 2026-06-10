using UnityEngine;

public class VehicleController : UnifiedPhysicsBody
{
    [Header("Vehicle")]
    public float engineForce = 8000f;
    public float steeringSpeed = 120f;
    public float maxSpeed = 35f;

    [Header("Control")]
    public bool playerControlled = true;

    [Header("Air Control")]
    public bool allowAirSteering = true;
    public float airSteeringMultiplier = 0.3f;

    [Header("Ground Alignment")]
    public float alignmentSpeed = 8f;

    [Header("Collision Response")]
    public float collisionRecoveryTime = 0.2f;
    private float collisionTimer;

    private ArduinoInput arduinoInput;

    private float moveInput;
    private float steerInput;
    private float yawAngle;

    public float MoveInput => moveInput;
    public float SteerInput => steerInput;

    void OnEnable()
    {
        // si quieres mantener tracking externo
    }

    void Start()
    {
        yawAngle = transform.eulerAngles.y;
    }

    void Update()
    {

        if (arduinoInput == null)
            arduinoInput = ArduinoInput.Instance;

        ReadInput();

        if (collisionTimer > 0f)
            collisionTimer -= Time.deltaTime;

        ApplyVehicleControl();
        UpdateOrientation();
    }

    public void RegisterCollision()
    {
        collisionTimer = collisionRecoveryTime;
    }

    void ReadInput()
    {
        if (!playerControlled)
        {
            moveInput = 0f;
            steerInput = 0f;
            return;
        }

        bool arduinoReady =
            arduinoInput != null && arduinoInput.IsConnected;

        if (arduinoReady)
        {
            moveInput = arduinoInput.vertical;
            steerInput = arduinoInput.horizontal;
        }
        else
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }

        moveInput = Mathf.Clamp(moveInput, -1f, 1f);
        steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    void ApplyVehicleControl()
    {
        float dt = Time.deltaTime;

        float steeringMultiplier =
            grounded ? 1f : airSteeringMultiplier;

        if (grounded || allowAirSteering)
        {
            yawAngle += steerInput * steeringSpeed * steeringMultiplier * dt;
        }

        Vector3 forward =
            Quaternion.Euler(0f, yawAngle, 0f) * Vector3.forward;

        // target speed
        Vector3 targetVelocity =
            forward * moveInput * maxSpeed;

        // horizontal velocity (preserve Y physics)
        Vector3 horizontalVelocity =
            new Vector3(velocity.x, 0f, velocity.z);

        float accel =
            (collisionTimer > 0f) ? 0.25f : 1f;

        horizontalVelocity =
            Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                engineForce * accel * dt
            );

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        // clamp speed
        Vector3 flat = new Vector3(velocity.x, 0f, velocity.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            velocity.x = flat.x;
            velocity.z = flat.z;
        }
    }

    void UpdateOrientation()
    {
        Vector3 forward =
            Quaternion.Euler(0f, yawAngle, 0f) * Vector3.forward;

        Vector3 targetUp =
            grounded ? groundNormal : Vector3.up;

        Vector3 projectedForward =
            Vector3.ProjectOnPlane(forward, targetUp);

        if (projectedForward.sqrMagnitude < 0.001f)
            projectedForward = transform.forward;

        Quaternion targetRot =
            Quaternion.LookRotation(projectedForward.normalized, targetUp);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRot,
                alignmentSpeed * Time.deltaTime
            );
    }
}