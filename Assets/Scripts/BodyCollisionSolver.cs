using UnityEngine;

public static class BodyCollisionSolver
{
    public static void Resolve(
        SimulationBody a,
        SimulationBody b)
    {
        Vector3 delta =
            b.transform.position
            - a.transform.position;

        float distance =
            delta.magnitude;

        float minDistance =
            a.radius + b.radius;

        // No colisión
        if (distance >= minDistance)
            return;

        // Evitar división por cero
        if (distance <= 0.0001f)
        {
            delta = Random.onUnitSphere * 0.01f;
            distance = delta.magnitude;
        }

        Vector3 normal =
            delta / distance;

        float penetration =
            minDistance - distance;

        // Separación
        Vector3 separation =
            normal * penetration * 0.5f;

        a.transform.position -= separation;
        b.transform.position += separation;

        // Velocidad relativa
        Vector3 relativeVelocity =
            b.velocity - a.velocity;

        float velocityAlongNormal =
            Vector3.Dot(
                relativeVelocity,
                normal
            );

        // Ya se separan
        if (velocityAlongNormal > 0f)
            return;

        float restitution =
            Mathf.Min(
                a.restitution,
                b.restitution
            );

        float impulseMagnitude =
            -(1f + restitution)
            * velocityAlongNormal;

        impulseMagnitude /=
            (1f / a.mass)
            + (1f / b.mass);

        Vector3 impulse =
            impulseMagnitude * normal;

        a.velocity -= impulse / a.mass;
        b.velocity += impulse / b.mass;
    }
}
