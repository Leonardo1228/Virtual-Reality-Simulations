using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [Header("References")]
    public PhysicsBody vehicle;

    public Transform frontWheel;
    public Transform rearWheel;

    [Header("Rolling")]
    public float rotationMultiplier = 1f;

    [Header("Steering")]
    public float maxSteeringAngle = 30f;

    [Header("Axes")]
    public Vector3 rollingAxis = Vector3.right;
    public Vector3 steeringAxis = Vector3.up;

    float wheelRotation;

    void Update()
    {
        if (vehicle == null)
            return;

        UpdateWheelRotation();
        UpdateWheelTransforms();
    }

    void UpdateWheelRotation()
    {
        Vector3 vel = vehicle.velocity;
        vel.y = 0f;

        float speed = vel.magnitude;

        // dirección estable basada en velocity real (NO transform.forward)
        Vector3 forward =
            speed > 0.1f
            ? vel.normalized
            : vehicle.transform.forward;

        float forwardSpeed = Vector3.Dot(vel, forward);

        wheelRotation += forwardSpeed * rotationMultiplier * Time.deltaTime;
    }

    void UpdateWheelTransforms()
    {
        float steer = 0f;

        var controller = vehicle.GetComponent<VehicleController>();
        if (controller != null)
            steer = controller.MoveInput * maxSteeringAngle;

        Quaternion roll =
            Quaternion.AngleAxis(wheelRotation, rollingAxis.normalized);

        Quaternion steering =
            Quaternion.AngleAxis(steer, steeringAxis.normalized);

        if (frontWheel != null)
            frontWheel.localRotation = steering * roll;

        if (rearWheel != null)
            rearWheel.localRotation = roll;
    }
}