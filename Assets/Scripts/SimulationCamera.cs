using UnityEngine;

public class SimulationCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset =
        new Vector3(0f, 5f, -10f);

    [Header("Smooth")]
    public float followSpeed = 5f;

    [Header("Look")]
    public bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            target.position +
            target.TransformDirection(offset);

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                Time.deltaTime * followSpeed
            );

        if (lookAtTarget)
        {
            transform.LookAt(
                target.position + Vector3.up * 2f
            );
        }
    }
}
