using UnityEngine;

public class VehicleController : UnifiedPhysicsBody
{
    [Header("Engine")]

    public float engineForce =
        8f;

    public float maxSpeed =
        35f;


    [Header("Steering")]

    public float steeringSpeed =
        120f;


    public bool allowAirSteering =
        true;


    public float airSteeringMultiplier =
        0.3f;


    [Header("Alignment")]

    public float alignmentSpeed =
        8f;


    [Header("Control")]

    public bool playerControlled =
        true;


    private ArduinoInput arduinoInput;


    private float moveInput;


    private float steerInput;

    public float MoveInput => moveInput;

    public float SteerInput => steerInput;


    private float yawAngle;



    void Start()
    {
        yawAngle =
            transform.eulerAngles.y;
    }



    protected override void Update()
    {
        base.Update();


        ReadInput();


        UpdateOrientation();
    }



    void ReadInput()
    {
        if (!playerControlled)
        {
            moveInput = 0f;
            steerInput = 0f;
            return;
        }


        if (arduinoInput == null)
        {
            arduinoInput =
                ArduinoInput.Instance;
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


        /*
         Giro.
        */

        float steeringMultiplier =
            grounded
            ? 1f
            : airSteeringMultiplier;


        if (grounded || allowAirSteering)
        {
            yawAngle +=
                steerInput *
                steeringSpeed *
                steeringMultiplier *
                dt;
        }


        /*
         En el aire no hay tracción.
        */

        if (!grounded)
            return;


        /*
         Dirección del vehículo.
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
         Velocidad objetivo.
        */

        Vector3 targetVelocity =
            forward *
            moveInput *
            maxSpeed;


        /*
         Solo modificamos la velocidad
         horizontal, dejando que la
         gravedad maneje Y.
        */

        Vector3 horizontal =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );


        horizontal =
            Vector3.Lerp(
                horizontal,
                targetVelocity,
                engineForce * dt
            );


        velocity.x =
            horizontal.x;

        velocity.z =
            horizontal.z;
    }



    void UpdateOrientation()
    {
        /*
         Dirección según volante.
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
         Si está en el suelo,
         usamos la normal de la
         superficie para inclinarlo.
        */

        Vector3 up =
            grounded
            ? groundNormal
            : transform.up;


        /*
         Proyectamos la dirección
         sobre la superficie.
        */

        Vector3 projectedForward =
            Vector3.ProjectOnPlane(
                forward,
                up
            ).normalized;


        /*
         Evita errores si la pendiente
         es demasiado extrema.
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
                up
            );


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                alignmentSpeed *
                Time.deltaTime
            );
    }
}