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
            // FALL OFF (GTA STYLE)
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

            body.force += force;
        }
    }
}