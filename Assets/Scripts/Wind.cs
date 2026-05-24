using UnityEngine;

public class Wind : MonoBehaviour, IForceGenerator
{
    public Vector3 windDirection = Vector3.right;

    public float strength = 3000f;

    public float radius = 20f;

    public float turbulenceStrength = 500f;

    public float turbulenceFrequency = 1f;

    void Update()
    {
        ApplyForces(Time.deltaTime);
    }

    public void ApplyForces(float dt)
    {
        SimulationBody[] bodies =
            FindObjectsOfType<SimulationBody>();

        foreach (SimulationBody body in bodies)
        {
            float distance =
                Vector3.Distance(transform.position,
                                 body.transform.position);

            if (distance > radius)
                continue;

            Vector3 dir = windDirection.normalized;

            float noiseX =
                Mathf.PerlinNoise(Time.time * turbulenceFrequency, 0f) - 0.5f;

            float noiseZ =
                Mathf.PerlinNoise(0f, Time.time * turbulenceFrequency) - 0.5f;

            Vector3 turbulence =
                new Vector3(noiseX, 0f, noiseZ)
                * turbulenceStrength;

            Vector3 totalForce =
                dir * strength + turbulence;

            body.AddForce(totalForce);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;

        Gizmos.DrawRay(
            transform.position,
            windDirection.normalized * 5f
        );
    }
}

