using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [Header("References")]

    public VehicleController vehicle;

    public Transform front_wheel;

    public Transform rear_wheel;

    [Header("Settings")]

    public float wheelRotationSpeed = 800f;

    public float steeringAngle = 30f;

    private float wheelRotation;

    void Update()
    {
        if (vehicle == null)
            return;

        AnimateRolling();

        AnimateSteering();
    }

    void AnimateRolling()
    {
        float speed =
            vehicle.velocity.magnitude;

        float direction =
            Mathf.Sign(vehicle.MoveInput);

        wheelRotation +=
            speed
            * wheelRotationSpeed
            * direction
            * Time.deltaTime;

        // Cambia eje si tu modelo usa otro
        front_wheel.localRotation =
            Quaternion.Euler(
                wheelRotation,
                front_wheel.localEulerAngles.y,
                front_wheel.localEulerAngles.z
            );

        rear_wheel.localRotation =
            Quaternion.Euler(
                wheelRotation,
                rear_wheel.localEulerAngles.y,
                rear_wheel.localEulerAngles.z
            );
    }

    void AnimateSteering()
    {
        float steer =
            vehicle.SteerInput;

        Vector3 euler =
            front_wheel.localEulerAngles;

        // Cambia eje si hace falta
        euler.y =
            steer * steeringAngle;

        front_wheel.localEulerAngles =
            euler;
    }
}