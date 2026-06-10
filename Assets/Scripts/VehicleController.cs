using UnityEngine;

public class VehicleController : SimulationBody
{
    [Header("Vehicle")]

    public float engineForce = 8f;

    public float steeringSpeed = 120f;

    public float maxSpeed = 35f;


    [Header("Control")]

    public bool playerControlled = true;


    [Header("Air Control")]

    public bool allowAirSteering = true;

    public float airSteeringMultiplier = 0.3f;


    [Header("Ground Alignment")]

    public float alignmentSpeed = 8f;


    [Header("Collision")]

    public float collisionRecoveryTime = 0.2f;

    private float collisionTimer;


    private ArduinoInput arduinoInput;


    private float moveInput;

    private float steerInput;


    private float yawAngle;


    public float MoveInput =>
        moveInput;


    public float SteerInput =>
        steerInput;


    void OnEnable()
    {
        VehicleCollision.allVehicles.Add(this);
    }


    void OnDisable()
    {
        VehicleCollision.allVehicles.Remove(this);
    }


    void Start()
    {
        yawAngle =
            transform.eulerAngles.y;
    }


    void Reset()
    {
        mass = 1200f;

        drag = 0.1f;

        restitution = 0.05f;
    }


    protected override void Update()
    {
        if (arduinoInput == null)
        {
            arduinoInput =
                ArduinoInput.Instance;
        }


        // Primero física y detección de suelo
        base.Update();


        // Luego control
        ReadInput();


        if (collisionTimer > 0f)
        {
            collisionTimer -=
                Time.deltaTime;
        }


        // Finalmente ajustamos la inclinación
        UpdateOrientation();
    }


    public void RegisterCollision()
    {
        collisionTimer =
            collisionRecoveryTime;
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
            arduinoInput != null
            &&
            arduinoInput.IsConnected;


        if (arduinoReady)
        {
            moveInput =
                arduinoInput.vertical;

            steerInput =
                arduinoInput.horizontal;
        }
        else
        {
            moveInput =
                Input.GetAxis(
                    "Vertical"
                );


            steerInput =
                Input.GetAxis(
                    "Horizontal"
                );
        }


        moveInput =
            Mathf.Clamp(
                moveInput,
                -1f,
                1f
            );


        steerInput =
            Mathf.Clamp(
                steerInput,
                -1f,
                1f
            );


        HandleMovement();
    }

    void HandleMovement()
    {
        float dt =
            Time.deltaTime;


        float steeringMultiplier =
            grounded
            ? 1f
            : airSteeringMultiplier;


        if (
            grounded
            ||
            allowAirSteering
        )
        {
            yawAngle +=
                steerInput
                *
                steeringSpeed
                *
                steeringMultiplier
                *
                dt;
        }


        if (!grounded)
        {
            return;
        }


        Vector3 forward =
            Quaternion.Euler(
                0f,
                yawAngle,
                0f
            )
            * Vector3.forward;


        Vector3 targetVelocity =
            forward
            *
            moveInput
            *
            maxSpeed;


        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );


        float accelerationFactor =
            collisionTimer > 0f
            ? 0.25f
            : 1f;


        horizontalVelocity =
            Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                engineForce
                *
                accelerationFactor
                *
                dt
            );


        velocity.x =
            horizontalVelocity.x;


        velocity.z =
            horizontalVelocity.z;
    }


    void UpdateOrientation()
    {
        /*
         Dirección del vehículo basada
         únicamente en el volante.
        */

        Vector3 forward =
            Quaternion.Euler(
                0f,
                yawAngle,
                0f
            )
            *
            Vector3.forward;


        /*
         Si estamos en una superficie,
         alineamos el vehículo con
         la normal del suelo.
        */

        Vector3 targetUp =
            grounded
            ? groundNormal
            : transform.up;


        /*
         Proyectamos la dirección
         sobre el plano del suelo.
        */

        Vector3 projectedForward =
            Vector3.ProjectOnPlane(
                forward,
                targetUp
            ).normalized;


        /*
         Protección por si la proyección
         queda demasiado pequeña.
        */

        if (
            projectedForward.sqrMagnitude
            < 0.001f
        )
        {
            projectedForward =
                transform.forward;
        }


        Quaternion targetRotation =
            Quaternion.LookRotation(
                projectedForward,
                targetUp
            );


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                alignmentSpeed
                *
                Time.deltaTime
            );
    }
}