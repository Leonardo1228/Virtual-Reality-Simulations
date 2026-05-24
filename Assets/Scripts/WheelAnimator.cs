using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [Header("Wheel References")]

    public Transform front_wheel;

    public Transform rear_wheel;

    [Header("Settings")]

    public float wheelRadius = 0.6f;

    public float steeringAngle = 30f;

    public float rotationMultiplier = 1f;

    private VehicleController vehicle;

    private float wheelRotation;

    void Awake()
    {
        vehicle = GetComponent<VehicleController>();
    }

    void Update()
    {
        AnimateSteering();

        AnimateRolling();
    }

    void AnimateSteering()
    {
        float steerInput =
            Input.GetAxis("Horizontal");

        float angle =
            steerInput * steeringAngle;

        Vector3 euler =
            front_wheel.localEulerAngles;

        euler.y = angle;

        front_wheel.localEulerAngles =
            euler;
    }

    void AnimateRolling()
    {
        float speed =
            vehicle.velocity.magnitude;

        float rotationSpeed =
            (speed / wheelRadius)
            * Mathf.Rad2Deg
            * rotationMultiplier;

        wheelRotation +=
            rotationSpeed * Time.deltaTime;

        RotateWheel(front_wheel);

        RotateWheel(rear_wheel);
    }

    void RotateWheel(Transform wheel)
    {
        wheel.Rotate(
            Vector3.right,
            wheelRotation * Time.deltaTime,
            Space.Self
        );
    }
}
