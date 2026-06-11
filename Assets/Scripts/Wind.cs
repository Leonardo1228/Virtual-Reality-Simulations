using UnityEngine;

public class Wind : MonoBehaviour, IForceGenerator
{
    public Vector3 windDirection = Vector3.right;
    public float strength = 30f;
    public float radius = 20f;

    Rigidbody[] bodies;

    void Start()
    {
        ForceManager.Register(this);
        bodies = FindObjectsOfType<Rigidbody>();
    }

    void OnDestroy()
    {
        ForceManager.Unregister(this);
    }

    public void ApplyForces(float dt)
    {
        Vector3 wind = windDirection.normalized * strength;

        foreach (var rb in bodies)
        {
            if (rb == null || rb.isKinematic)
                continue;

            float dist = Vector3.Distance(transform.position, rb.position);

            if (dist > radius)
                continue;

            float t = 1f - (dist / radius);

            rb.AddForce(wind * t, ForceMode.Force);
        }
    }
}