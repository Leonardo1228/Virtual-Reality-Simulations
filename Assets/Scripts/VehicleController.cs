using UnityEngine;

public class VehicleController : SimulationBody
{
    [Header("Vehicle")]
    public float engineForce = 12000f;

    public float steeringSpeed = 45f;

    public float maxSpeed = 25f;

    public float accelerationResponse = 4f;

    private float moveInput;

    private float steerInput;

    protected override void Update()
    {
        ReadInput();

        base.Update();
    }

    void ReadInput()
    {
        moveInput =
            ArduinoInput.Instance.vertical;

        steerInput =
            ArduinoInput.Instance.horizontal;

        ApplyEngine();

        ApplySteering();
    }

    void ApplyEngine()
    {
        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        float currentSpeed =
            horizontalVelocity.magnitude;

        // Factor de reducción cerca de velocidad máxima
        float speedFactor =
            1f - Mathf.Clamp01(
                currentSpeed / maxSpeed
            );

        float finalForce =
            engineForce
            * speedFactor
            * accelerationResponse;

        Vector3 force =
            transform.forward
            * moveInput
            * finalForce;

        AddForce(force);
    }

    void ApplySteering()
    {
        float speed =
            velocity.magnitude;

        if (speed < 0.5f)
            return;

        float steeringFactor =
            Mathf.Clamp01(speed / maxSpeed);

        transform.Rotate(
            Vector3.up,
            steerInput
            * steeringSpeed
            * steeringFactor
            * Time.deltaTime
        );
    }
}