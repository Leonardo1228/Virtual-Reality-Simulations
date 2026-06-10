using UnityEngine;
using System.Collections.Generic;

public class SimulationBody : MonoBehaviour
{
    public static List<Ramp> allRamps =
        new List<Ramp>();


    [Header("Physics")]

    public float mass = 1000f;

    public float drag = 0.2f;

    public float restitution = 0.2f;


    [Header("Gravity")]

    public bool useGravity = true;

    public Vector3 gravity =
        new Vector3(
            0f,
            -9.81f,
            0f
        );


    [Header("Ground")]

    public bool grounded;

    public float groundMargin = 0.02f;

    public Vector3 groundNormal =
        Vector3.up;

    protected float currentGroundHeight;


    [Header("Linear Motion")]

    public Vector3 velocity;

    public Vector3 acceleration;

    protected Vector3 accumulatedForce;


    [Header("Angular Motion")]

    public Vector3 angularVelocity;

    public Vector3 angularAcceleration;

    public float rotationalDrag = 0.96f;

    protected Vector3 accumulatedTorque;


    protected virtual void Update()
    {
        float dt =
            Time.deltaTime;


        UpdateGroundState();


        Integrate(dt);


        Move(dt);
    }

    float GroundHeight()
    {
        return
            transform.localScale.y
            * 0.2f;
    }
    void UpdateGroundState()
    {
        currentGroundHeight =
            GroundHeight();

        groundNormal =
            Vector3.up;


        float highestSurface =
            currentGroundHeight;


        foreach (Ramp ramp in allRamps)
        {
            if (ramp == null)
                continue;


            if (!ramp.ContainsPoint(
                transform.position))
            {
                continue;
            }


            float rampHeight =
                ramp.GetSurfaceHeight(
                    transform.position
                );


            float finalHeight =
                GroundHeight()
                + rampHeight;


            if (finalHeight > highestSurface)
            {
                highestSurface =
                    finalHeight;

                groundNormal =
                    ramp.SurfaceNormal();
            }
        }


        currentGroundHeight =
            highestSurface;


        grounded =
            transform.position.y
            <= currentGroundHeight
            + groundMargin;
    }


    protected virtual void Integrate(
        float dt)
    {
        Vector3 totalForce =
            accumulatedForce;


        if (useGravity && !grounded)
        {
            totalForce +=
                gravity * mass;
        }


        totalForce +=
            -velocity * drag;


        acceleration =
            totalForce / mass;


        velocity +=
            acceleration * dt;


        if (grounded)
        {
            ApplyGroundFriction(
                dt
            );
        }


        angularAcceleration =
            accumulatedTorque / mass;


        angularVelocity +=
            angularAcceleration * dt;


        angularVelocity *=
            rotationalDrag;


        transform.Rotate(
            angularVelocity * dt,
            Space.World
        );


        accumulatedForce =
            Vector3.zero;

        accumulatedTorque =
            Vector3.zero;
    }


    void ApplyGroundFriction(
        float dt)
    {
        Vector3 horizontal =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );


        float friction = 4f;


        horizontal =
            Vector3.Lerp(
                horizontal,
                Vector3.zero,
                friction * dt
            );


        velocity.x =
            horizontal.x;

        velocity.z =
            horizontal.z;
    }


    protected virtual void Move(
        float dt)
    {
        transform.position +=
            velocity * dt;


        CheckGroundCollision();
    }


    void CheckGroundCollision()
    {
        Vector3 pos =
            transform.position;


        if (
            pos.y <=
            currentGroundHeight
            + groundMargin
        )
        {
            pos.y =
                currentGroundHeight;

            transform.position =
                pos;

            grounded = true;


            if (velocity.y < 0f)
            {
                velocity.y = 0f;
            }
        }
        else
        {
            grounded = false;
        }
    }


    public void AddForce(
        Vector3 force)
    {
        accumulatedForce +=
            force;
    }


    public void AddTorque(
        Vector3 torque)
    {
        accumulatedTorque +=
            torque;
    }


    public void Bounce(
        Vector3 normal,
        float damping = 0.2f)
    {
        velocity =
            Vector3.Reflect(
                velocity,
                normal.normalized
            ) * damping;
    }


    public float KineticEnergy()
    {
        return
            0.5f *
            mass *
            velocity.sqrMagnitude;
    }


    public float Momentum()
    {
        return
            mass *
            velocity.magnitude;
    }
}


