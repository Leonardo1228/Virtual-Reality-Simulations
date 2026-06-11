using UnityEngine;

public class Wind : MonoBehaviour
{
    [Header("Wind")]
    public Vector3 windDirection = Vector3.right;
    public float strength = 30f;
    public float radius = 20f;

    [Header("Turbulence")]
    public float turbulenceStrength = 10f;
    public float turbulenceFrequency = 1f;

    void Update()
    {
        ApplyWind();
    }

    void ApplyWind()
    {
        PhysicsBody[] bodies = FindObjectsOfType<PhysicsBody>();

        foreach (PhysicsBody body in bodies)
        {
            if (body == null || body.isStatic)
                continue;

            Vector3 offset = body.transform.position - transform.position;
            float distance = offset.magnitude;

            if (distance > radius)
                continue;

            // =========================
            // FALL OFF 
            // =========================
            float t = 1f - (distance / radius);
            t = Mathf.SmoothStep(0f, 1f, t);

            // =========================
            // BASE WIND
            // =========================
            Vector3 baseWind = windDirection.normalized * strength;

            // =========================
            // TURBULENCE
            // =========================
            float noiseX = Mathf.PerlinNoise(Time.time * turbulenceFrequency, 0f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(0f, Time.time * turbulenceFrequency) - 0.5f;

            Vector3 turbulence =
                new Vector3(noiseX, 0f, noiseZ) * turbulenceStrength;

            // =========================
            // MASS AFFECT
            // =========================
            float massFactor = 1f / Mathf.Max(1f, body.mass);

            // =========================
            // FINAL FORCE (NO dt scaling)
            // =========================
            Vector3 force =
                (baseWind + turbulence) *
                t *
                massFactor;

        }
    }
    void OnDrawGizmos()
    {
        if (!enabled) return;

        // esfera de influencia
        Gizmos.color = new Color(0f, 1f, 1f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, radius);

        // dirección del viento (más visible)
        Gizmos.color = Color.blue;
        Vector3 dir = windDirection.normalized;

        Gizmos.DrawLine(transform.position, transform.position + dir * radius);

        // flecha simple (punta visual)
        Vector3 right = Vector3.Cross(Vector3.up, dir) * 0.2f;
        Gizmos.DrawLine(transform.position + dir * radius, transform.position + dir * radius - dir * 0.5f + right);
        Gizmos.DrawLine(transform.position + dir * radius, transform.position + dir * radius - dir * 0.5f - right);
    }
}