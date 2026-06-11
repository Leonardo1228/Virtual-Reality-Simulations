using UnityEngine;
using System.Collections.Generic;

public class PhysicsSolver : MonoBehaviour
{
    public static PhysicsSolver Instance;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float skinWidth = 0.02f;
    public int substeps = 3;
    public float groundDistance = 0.6f;

    private PhysicsBody[] bodies;

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        bodies = FindObjectsOfType<PhysicsBody>();

        float dt = Time.fixedDeltaTime;
        float step = dt / substeps;

        for (int i = 0; i < substeps; i++)
        {
            Simulate(step);
        }
    }

    void Simulate(float dt)
    {
        // =========================
        // 1. APPLY FORCES
        // =========================
        foreach (var b in bodies)
        {
            if (b == null || b.isStatic) continue;

            if (b.useGravity)
                b.velocity += gravity * dt;

            b.velocity += b.force / Mathf.Max(0.001f, b.mass);
            b.force = Vector3.zero;

            b.velocity *= 1f / (1f + b.drag * dt);
        }

        // =========================
        // 2. MOVE
        // =========================
        foreach (var b in bodies)
        {
            if (b == null || b.isStatic) continue;

            Vector3 move = b.velocity * dt;

            if (move.sqrMagnitude < 0.000001f)
                continue;

            ResolveMovement(b, move);
        }

        // =========================
        // 3. GROUND CHECK
        // =========================
        foreach (var b in bodies)
        {
            if (b == null || b.isStatic) continue;

            GroundCheck(b);
        }
    }
    void ResolveMovement(PhysicsBody b, Vector3 move)
    {
        Vector3 dir = move.normalized;
        float dist = move.magnitude;

        if (Physics.BoxCast(
            b.transform.position,
            Vector3.one * 0.5f,
            dir,
            out RaycastHit hit,
            b.transform.rotation,
            dist + skinWidth,
            b.collisionMask))
        {
            // posición corregida (SIN huecos)
            b.transform.position += dir * Mathf.Max(hit.distance - skinWidth, 0f);

            // slide (GTA feel)
            b.velocity = Vector3.ProjectOnPlane(b.velocity, hit.normal) * b.restitution;
        }
        else
        {
            b.transform.position += move;
        }
        if (hit.collider != null)
        {
            var wall = hit.collider.GetComponent<HeavyWall>();

            if (wall != null)
            {
                wall.ApplyImpact(b.velocity, hit.normal);
            }
        }
        var brick = hit.collider.GetComponent<Brick>();

        if (brick != null)
        {
            brick.ApplyImpact(b.velocity, hit.normal);
        }
    }
    void GroundCheck(PhysicsBody b)
    {
        Vector3 origin = b.transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundDistance, b.collisionMask))
        {
            b.grounded = Vector3.Angle(hit.normal, Vector3.up) < 65f;
            b.groundNormal = hit.normal;
        }
        else
        {
            b.grounded = false;
        }
    }
}
