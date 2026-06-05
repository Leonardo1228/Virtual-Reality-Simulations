using UnityEngine;

public class VehicleController : SimulationBody
{
    [Header("Vehicle")]

    public float engineForce = 8f;

    public float steeringSpeed = 120f;

    public float maxSpeed = 35f;

    [Header("Collision")]

    public float collisionRecoveryTime = 0.2f;

    private float collisionTimer;

    private ArduinoInput arduinoInput;

    private float moveInput;

    private float steerInput;

    public float MoveInput =>
        moveInput;

    public float SteerInput =>
        steerInput;

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

        ReadInput();

        if (collisionTimer > 0f)
        {
            collisionTimer -=
                Time.deltaTime;
        }

        base.Update();
    }

    public void RegisterCollision()
    {
        collisionTimer =
            collisionRecoveryTime;
    }

    void ReadInput()
    {
        bool arduinoReady =
            arduinoInput != null
            && arduinoInput.IsConnected;

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

        Vector3 targetVelocity =
            transform.forward
            * moveInput
            * maxSpeed;

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
                * accelerationFactor
                * Time.deltaTime
            );

        velocity.x =
            horizontalVelocity.x;

        velocity.z =
            horizontalVelocity.z;

        transform.Rotate(
            Vector3.up,
            steerInput
            * steeringSpeed
            * Time.deltaTime
        );
    }
}
