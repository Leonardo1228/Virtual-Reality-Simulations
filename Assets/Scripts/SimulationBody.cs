using UnityEngine;

public class SimulationBody : MonoBehaviour
{
    [Header("Physics")]

    public float mass = 1000f;

    public float drag = 0.2f;

    public float restitution = 0.2f;

    [HideInInspector]
    public float radius;

    [Header("Gravity")]

    public bool useGravity = true;

    public Vector3 gravity =
        new Vector3(0f, -9.81f, 0f);

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
        float dt = Time.deltaTime;

        // Actualizar radius antes de física
        UpdateRadius();

        Integrate(dt);

        Move(dt);
    }

    void UpdateRadius()
    {
        Vector3 scale =
            transform.localScale;

        // Radius proporcional al tamaño promedio
        radius =
            (
                scale.x
                + scale.y
                + scale.z
            ) / 3f * 0.5f;
    }

    protected virtual void Integrate(float dt)
    {
        // Drag
        Vector3 dragForce =
            -velocity * drag;

        // Gravedad
        Vector3 gravityForce =
            Vector3.zero;

        if (useGravity)
        {
            gravityForce =
                gravity * mass;
        }

        // Aceleración lineal
        acceleration =
            (
                accumulatedForce
                + dragForce
                + gravityForce
            ) / mass;

        velocity += acceleration * dt;

        // Fricción solo en suelo
        if (transform.position.y <= 0.01f)
        {
            ApplyGroundFriction(dt);
        }

        // Angular
        angularAcceleration =
            accumulatedTorque / mass;

        angularVelocity +=
            angularAcceleration * dt;

        angularVelocity *= rotationalDrag;

        transform.Rotate(
            angularVelocity * dt,
            Space.World
        );

        // Limpiar acumuladores
        accumulatedForce = Vector3.zero;

        accumulatedTorque = Vector3.zero;
    }

    void ApplyGroundFriction(float dt)
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

        velocity.x = horizontal.x;
        velocity.z = horizontal.z;
    }

    protected virtual void Move(float dt)
    {
        transform.position += velocity * dt;

        CheckGroundCollision();
    }

    void CheckGroundCollision()
    {
        float groundHeight = 0f;

        Vector3 pos =
            transform.position;

        if (pos.y < groundHeight)
        {
            pos.y = groundHeight;

            transform.position = pos;

            // Rebote vertical
            if (velocity.y < 0f)
            {
                velocity.y *= -restitution;

                // Evitar vibración infinita
                if (Mathf.Abs(velocity.y) < 0.5f)
                {
                    velocity.y = 0f;
                }
            }

            // Fricción horizontal
            velocity.x *= 0.92f;
            velocity.z *= 0.92f;

            // Frenar rotación lentamente
            angularVelocity *= 0.96f;
        }
    }

    public void AddForce(Vector3 force)
    {
        accumulatedForce += force;
    }

    public void AddTorque(Vector3 torque)
    {
        accumulatedTorque += torque;
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
}   

