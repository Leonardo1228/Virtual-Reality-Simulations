using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 windDirection = Vector3.right;
    public float strength = 30f;
    public float radius = 20f;

    public float turbulenceStrength = 10f;
    public float turbulenceFrequency = 1f;

    [Header("Layers")]
    public LayerMask affectedLayers;

    void FixedUpdate()
    {
        ApplyWind();
    }

    void ApplyWind()
    {
        float time = Time.time;

        Vector3 baseWind =
            windDirection.normalized * strength;

        float noiseX =
            Mathf.PerlinNoise(time * turbulenceFrequency, 0f) - 0.5f;

        float noiseZ =
            Mathf.PerlinNoise(0f, time * turbulenceFrequency) - 0.5f;

        Vector3 turbulence =
            new Vector3(noiseX, 0f, noiseZ) * turbulenceStrength;

        Vector3 totalWind =
            baseWind + turbulence;


        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            radius,
            affectedLayers
        );


        foreach (Collider col in targets)
        {
            Rigidbody rb = col.attachedRigidbody;

            if (rb == null)
                continue;

            if (rb.isKinematic)
                continue;


            float distance =
                Vector3.Distance(
                    transform.position,
                    rb.position
                );

            float falloff =
                1f - (distance / radius);

            falloff = Mathf.SmoothStep(
                0f,
                1f,
                falloff
            );


            rb.AddForce(
                totalWind * falloff,
                ForceMode.Force
            );
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            windDirection.normalized * radius
        );
    }
}