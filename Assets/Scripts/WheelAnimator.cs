using UnityEngine;

public class WheelAnimator : MonoBehaviour
{

    [Header("References")]

    public VehicleController vehicle;

    public Transform frontWheel;

    public Transform rearWheel;


    [Header("Rolling")]

    public float rotationMultiplier = 800f;


    [Header("Steering")]

    public float maxSteeringAngle = 30f;


    [Header("Axes")]

    public Vector3 rollingAxis =
        Vector3.right;

    public Vector3 steeringAxis =
        Vector3.up;


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
        float speed =
            vehicle.velocity.magnitude;


        float direction =
            Mathf.Sign(
                vehicle.MoveInput
            );


        wheelRotation +=
            speed *
            rotationMultiplier *
            direction *
            Time.deltaTime;
    }


    void UpdateWheelTransforms()
    {
        float steer =
            vehicle.SteerInput *
            maxSteeringAngle;


        Quaternion roll =
            Quaternion.AngleAxis(
                wheelRotation,
                rollingAxis.normalized
            );


        Quaternion steering =
            Quaternion.AngleAxis(
                steer,
                steeringAxis.normalized
            );


        /*
         Ruedas delanteras:
         dirección + giro
        */
        if (frontWheel != null)
        {
            frontWheel.localRotation =
                steering * roll;
        }


        /*
         Ruedas traseras:
         solo giro
        */
        if (rearWheel != null)
        {
            rearWheel.localRotation =
                roll;
        }
    }
}