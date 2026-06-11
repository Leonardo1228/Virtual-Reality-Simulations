using UnityEngine;

public class Wind : MonoBehaviour
{
    [Header("Wind")]

    public Vector3 windDirection =
        Vector3.right;

    public float strength =
        3000f;

    public float radius =
        20f;


    [Header("Turbulence")]

    public float turbulenceStrength =
        500f;

    public float turbulenceFrequency =
        1f;


    void Update()
    {
        ApplyWind();
    }


    void ApplyWind()
    {
        foreach (
            UnifiedPhysicsBody body
            in UnifiedPhysicsBody.allBodies)
        {
            if (body == null)
                continue;


            if (body.isStatic)
                continue;


            float distance =
                Vector3.Distance(
                    transform.position,
                    body.transform.position
                );


            if (distance > radius)
                continue;


            Vector3 baseWind =
                windDirection.normalized
                * strength;


            float noiseX =
                Mathf.PerlinNoise(
                    Time.time
                    *
                    turbulenceFrequency,
                    0f
                ) - 0.5f;


            float noiseZ =
                Mathf.PerlinNoise(
                    0f,
                    Time.time
                    *
                    turbulenceFrequency
                ) - 0.5f;


            Vector3 turbulence =
                new Vector3(
                    noiseX,
                    0f,
                    noiseZ
                )
                *
                turbulenceStrength;


            body.AddForce(
                baseWind
                +
                turbulence
            );
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color =
            Color.cyan;


        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );


        Gizmos.color =
            Color.blue;


        Gizmos.DrawRay(
            transform.position,
            windDirection.normalized
            * 5f
        );
    }
}

