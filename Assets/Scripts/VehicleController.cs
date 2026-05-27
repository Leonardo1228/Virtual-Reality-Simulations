using UnityEngine;

public class VehicleController : SimulationBody
{
    [Header("Vehicle")]

    public float engineForce = 8f;

    public float steeringSpeed = 120f;

    public float maxSpeed = 35f;

    public ArduinoInput arduinoInput;

    private float moveInput;

    private float steerInput;

    public float MoveInput => moveInput;

    public float SteerInput => steerInput;

    protected override void Update()
    {
        ReadInput();

        base.Update();
    }

    void ReadInput()
    {
        // TECLADO
        float keyboardVertical =
            Input.GetAxis("Vertical");

        float keyboardHorizontal =
            Input.GetAxis("Horizontal");

        // JOYSTICK
        float joystickVertical = 0f;

        float joystickHorizontal = 0f;

        // SOLO SI EXISTE ARDUINO
        if (arduinoInput != null)
        {
            joystickVertical =
                arduinoInput.vertical;

            joystickHorizontal =
                arduinoInput.horizontal;
        }

        // PRIORIDAD:
        // joystick si se mueve,
        // teclado si no

        moveInput =
            Mathf.Abs(joystickVertical) > 0.1f
            ? joystickVertical
            : keyboardVertical;

        steerInput =
            Mathf.Abs(joystickHorizontal) > 0.1f
            ? joystickHorizontal
            : keyboardHorizontal;

        // VELOCIDAD OBJETIVO
        Vector3 forwardVelocity =
    transform.forward
    * moveInput
    * maxSpeed;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        // ACELERACIÓN PROGRESIVA
        horizontalVelocity =
            Vector3.Lerp(
                horizontalVelocity,
                forwardVelocity,
                engineForce * Time.deltaTime
            );

        // APLICAR
        velocity.x =
            horizontalVelocity.x;

        velocity.z =
            horizontalVelocity.z;

        // GIRO
        transform.Rotate(
            Vector3.up,
            steerInput
            * steeringSpeed
            * Time.deltaTime
        );
    }
}