using UnityEngine;
using System.Collections.Generic;

public class PhysicsSolver : MonoBehaviour
{
    public static List<PhysicsBody> bodies = new();

    public float gravity = -9.81f;
    public int substeps = 4;
    public float skin = 0.02f;
    public LayerMask mask;

    public static void Register(PhysicsBody b)
    {
        if (!bodies.Contains(b))
            bodies.Add(b);
    }

    public static void Unregister(PhysicsBody b)
    {
        bodies.Remove(b);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        foreach (var b in bodies)
        {
            if (b == null || b.isStatic) continue;

            Step(b, dt);
        }
    }

    void Step(PhysicsBody b, float dt)
    {
        float step = dt / substeps;

        for (int i = 0; i < substeps; i++)
        {
            ApplyForces(b, step);
            Move(b, step);
            Ground(b);
        }
    }

    void ApplyForces(PhysicsBody b, float dt)
    {
        b.velocity.y += gravity * dt;

        float d = 1f / (1f + b.drag * dt);
        b.velocity *= d;
    }

    void Move(PhysicsBody b, float dt)
    {
        Vector3 move = b.velocity * dt;

        if (move.sqrMagnitude < 0.000001f)
            return;

        Vector3 dir = move.normalized;
        float dist = move.magnitude;

        Vector3 half = b.transform.localScale * 0.5f;

        if (Physics.BoxCast(b.transform.position, half, dir,
            out RaycastHit hit, b.transform.rotation,
            dist + skin, mask))
        {
            float safe = Mathf.Max(hit.distance - skin, 0f);

            b.transform.position += dir * safe;

            b.velocity = Vector3.ProjectOnPlane(b.velocity, hit.normal);
            b.velocity *= b.restitution;
        }
        else
        {
            b.transform.position += move;
        }
    }

    void Ground(PhysicsBody b)
    {
        Vector3 origin = b.transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.3f, mask))
        {
            b.grounded = Vector3.Angle(hit.normal, Vector3.up) < 65f;
            if (b.grounded)
                b.groundNormal = hit.normal;
        }
        else
        {
            b.grounded = false;
        }
    }
}
