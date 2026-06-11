using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 windDirection = Vector3.right;
    public float strength = 30f;
    public float radius = 20f;

    public float turbulenceStrength = 10f;
    public float turbulenceFrequency = 1f;

    void FixedUpdate()
    {
        ApplyWind();
    }

    void ApplyWind()
    {
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();

        Vector3 baseWind = windDirection.normalized * strength;

        float time = Time.time;

        float noiseX = Mathf.PerlinNoise(time * turbulenceFrequency, 0f) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(0f, time * turbulenceFrequency) - 0.5f;

        Vector3 turbulence = new Vector3(noiseX, 0f, noiseZ) * turbulenceStrength;

        Vector3 totalWind = baseWind + turbulence;

        foreach (var rb in bodies)
        {
            if (rb == null || rb.isKinematic)
                continue;

            float dist = Vector3.Distance(transform.position, rb.position);
            if (dist > radius) continue;

            float t = 1f - (dist / radius);
            t = Mathf.SmoothStep(0f, 1f, t);

            rb.AddForce(totalWind * t, ForceMode.Force);
        }
    }

    void OnDrawGizmos()
    {
        // área de influencia
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);

        // dirección del viento
        Gizmos.color = Color.blue;
        Vector3 dir = windDirection.normalized;

        Vector3 center = transform.position;
        Vector3 end = center + dir * radius;

        Gizmos.DrawLine(center, end);

        // flecha
        Vector3 right = Vector3.Cross(Vector3.up, dir) * 0.3f;

        Gizmos.DrawLine(end, end - dir * 1f + right);
        Gizmos.DrawLine(end, end - dir * 1f - right);

        // turbulencia visual (opcional)
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(center, Vector3.one * 0.5f);
    }
}