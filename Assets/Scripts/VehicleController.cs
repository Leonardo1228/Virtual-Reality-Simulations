using UnityEngine;

public class VehicleController : SimulationBody
{
    [Header("Vehicle")]

    public float engineForce = 8f;

    public float steeringSpeed = 120f;

    public float maxSpeed = 35f;

    private ArduinoInput arduinoInput;

    private float moveInput;

    private float steerInput;



    public float MoveInput => moveInput;

    public float SteerInput => steerInput;

    protected override void Update()
    {
        if (arduinoInput == null)
        {
            arduinoInput =
                ArduinoInput.Instance;
        }

        ReadInput();

        base.Update();
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
                Input.GetAxis("Vertical");

            steerInput =
                Input.GetAxis("Horizontal");
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

        horizontalVelocity =
            Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                engineForce
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
