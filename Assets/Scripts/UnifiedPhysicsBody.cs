using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class CollisionBox
{
    public bool enabled = true;


    [Header("Local Transform")]

    public Vector3 center;


    public Vector3 rotation;


    [Header("Size")]

    public Vector3 size =
        Vector3.one;


    public Vector3 WorldCenter(
        Transform root)
    {
        return root.TransformPoint(
            center
        );
    }


    public Quaternion WorldRotation(
        Transform root)
    {
        return root.rotation *
               Quaternion.Euler(
                   rotation
               );
    }


    public Vector3 HalfExtents()
    {
        return size * 0.5f;
    }
}




public class UnifiedPhysicsBody :
    MonoBehaviour
{

    /*
    ==========================
        GLOBAL REGISTRY
    ==========================
    */


    public static readonly
    List<UnifiedPhysicsBody>
    allBodies =
        new List<UnifiedPhysicsBody>();


    protected virtual void OnEnable()
    {
        if (
            !allBodies.Contains(
                this
            )
        )
        {
            allBodies.Add(
                this
            );
        }
    }


    protected virtual void OnDisable()
    {
        allBodies.Remove(
            this
        );
    }


    /*
    ==========================
           PHYSICS
    ==========================
    */


    [Header("Physics")]


    public bool isStatic =
        false;


    [Min(0.001f)]
    public float mass =
        1000f;


    [Range(0f, 1f)]
    public float restitution =
        0.2f;


    [Range(0f, 10f)]
    public float drag =
        0.1f;


    [Tooltip(
        "Extra damping after collisions"
    )]
    [Range(0f, 1f)]
    public float collisionDamping =
        0.98f;


    /*
    ==========================
           GRAVITY
    ==========================
    */


    [Header("Gravity")]


    public bool useGravity =
        true;


    public Vector3 gravity =
        new Vector3(
            0f,
            -9.81f,
            0f
        );


    /*
    ==========================
            MOTION
    ==========================
    */


    [Header("Motion")]


    public Vector3 velocity;


    public Vector3 acceleration;


    protected Vector3 accumulatedForce;


    /*
    ==========================
          COLLISION
    ==========================
    */


    [Header("Collision")]


    public CollisionBox[] collisionBoxes;


    public LayerMask collisionMask =
        -1;


    [Min(0.001f)]
    public float skinWidth =
        0.02f;


    [Tooltip(
        "Maximum slope angle considered ground"
    )]
    [Range(0f, 90f)]
    public float maxGroundAngle =
        65f;


    public bool grounded;


    public Vector3 groundNormal =
        Vector3.up;


    /*
    ==========================
       STABILITY SETTINGS
    ==========================
    */


    [Header("Stability")]


    [Tooltip(
        "Prevents fast objects from tunneling"
    )]
    [Range(1, 10)]
    public int movementSubsteps =
        3;


    [Tooltip(
        "Small separation after impact"
    )]
    public float separationOffset =
        0.01f;


    /*
    ==========================
             DEBUG
    ==========================
    */


    [Header("Debug")]


    public float speed;


    public float kineticEnergy;


    public float momentum;


    public int contactCount;


    /*
    ==========================
           UPDATE LOOP
    ==========================
    */


    protected virtual void Update()
    {
        float dt =
            Time.deltaTime;


        if (!isStatic)
        {
            Simulate(
                dt
            );
        }


        UpdateDebug();
    }


    /*
    ==========================
          SIMULATION
    ==========================
    */


    protected virtual void Simulate(
        float dt)
    {
        ApplyForces(
            dt
        );


        float stepTime =
            dt /
            movementSubsteps;


        for (
            int i = 0;
            i < movementSubsteps;
            i++
        )
        {
            MoveStep(
                stepTime
            );
        }


        accumulatedForce =
            Vector3.zero;
    }


    /*
    ==========================
        FORCE INTEGRATION
    ==========================
    */


    protected virtual void ApplyForces(
        float dt)
    {
        Vector3 totalForce =
            accumulatedForce;


        if (useGravity)
        {
            totalForce +=
                gravity *
                mass;
        }


        acceleration =
            totalForce /
            mass;


        velocity +=
            acceleration *
            dt;


        float damping =
            Mathf.Clamp01(
                1f -
                drag * dt
            );


        velocity *=
            damping;
    }

    /*
==========================
      MOVEMENT STEP
==========================
*/

    protected virtual void MoveStep(
        float dt)
    {
        Vector3 movement =
            velocity *
            dt;


        float distance =
            movement.magnitude;


        if (distance <= 0.00001f)
        {
            return;
        }


        Vector3 direction =
            movement.normalized;


        RaycastHit hit;


        CollisionBox sourceBox;


        bool collided =
            CheckCollision(
                direction,
                distance,
                out hit,
                out sourceBox
            );


        if (!collided)
        {
            transform.position +=
                movement;


            grounded =
                false;


            return;
        }


        ResolveCollision(
            hit,
            direction
        );
    }


    /*
    ==========================
       MULTI BOX DETECTION
    ==========================
    */


    bool CheckCollision(
        Vector3 direction,
        float distance,
        out RaycastHit nearestHit,
        out CollisionBox hitBox)
    {
        nearestHit =
            new RaycastHit();


        hitBox =
            null;


        float closest =
            float.MaxValue;


        bool found =
            false;


        contactCount =
            0;


        foreach (
            CollisionBox box
            in collisionBoxes)
        {
            if (box == null)
                continue;


            if (!box.enabled)
                continue;


            RaycastHit hit;


            bool hasHit =
                Physics.BoxCast(
                    box.WorldCenter(
                        transform
                    ),

                    box.HalfExtents(),

                    direction,

                    out hit,

                    box.WorldRotation(
                        transform
                    ),

                    distance +
                    skinWidth,

                    collisionMask
                );


            if (!hasHit)
                continue;


            contactCount++;


            if (hit.distance <
                closest)
            {
                closest =
                    hit.distance;


                nearestHit =
                    hit;


                hitBox =
                    box;


                found =
                    true;
            }
        }


        return found;
    }


    /*
    ==========================
         COLLISION RESPONSE
    ==========================
    */


    void ResolveCollision(
        RaycastHit hit,
        Vector3 direction)
    {

        /*
         Separar el cuerpo
         ligeramente de la superficie
        */

        transform.position +=
            direction *
            Mathf.Max(
                hit.distance
                -
                skinWidth,
                0f
            );


        transform.position +=
            hit.normal *
            separationOffset;


        groundNormal =
            hit.normal;


        float angle =
            Vector3.Angle(
                hit.normal,
                Vector3.up
            );


        grounded =
            angle <=
            maxGroundAngle;


        UnifiedPhysicsBody other =
            hit.collider
            .GetComponentInParent
            <UnifiedPhysicsBody>();


        if (
            other != null
            &&
            other != this
        )
        {
            ResolveBodyCollision(
                other,
                hit.normal
            );
        }
        else
        {
            ResolveStaticCollision(
                hit.normal
            );
        }
    }


    /*
    ==========================
      STATIC COLLISION
    ==========================
    */


    void ResolveStaticCollision(
        Vector3 normal)
    {
        float incoming =
            Vector3.Dot(
                velocity,
                normal
            );


        /*
         Si nos estamos alejando,
         no hacer nada.
        */

        if (incoming >= 0f)
            return;


        /*
         Rebote
        */

        Vector3 bounce =
            -(1f +
            restitution)
            *
            incoming
            *
            normal;


        velocity +=
            bounce;


        /*
         Deslizamiento en superficies
         inclinadas.
        */

        velocity =
            Vector3.ProjectOnPlane(
                velocity,
                normal
            );


        /*
         Pérdida de energía
         para evitar vibraciones.
        */

        velocity *=
            collisionDamping;
    }
    /*
==========================
  BODY COLLISION
==========================
*/

    void ResolveBodyCollision(
        UnifiedPhysicsBody other,
        Vector3 normal)
    {
        if (other.isStatic)
        {
            ResolveStaticCollision(
                normal
            );
            return;
        }


        Vector3 relativeVelocity =
            velocity -
            other.velocity;


        float speed =
            Vector3.Dot(
                relativeVelocity,
                normal
            );


        /*
        Si se están separando,
        no aplicamos impulso.
        */

        if (speed >= 0f)
            return;


        float e =
            Mathf.Min(
                restitution,
                other.restitution
            );


        float impulseStrength =
            -(1f + e)
            * speed;


        impulseStrength /=
            (1f / mass)
            +
            (1f / other.mass);


        Vector3 impulse =
            normal *
            impulseStrength;


        /*
        Intercambio de momento
        corregido.
        */

        velocity +=
            impulse / mass;


        other.velocity -=
            impulse / other.mass;


        velocity *=
            collisionDamping;


        other.velocity *=
            other.collisionDamping;
    }



    /*
    ==========================
        EXTERNAL FORCES
    ==========================
    */


    public void AddForce(
        Vector3 force)
    {
        if (isStatic)
            return;


        accumulatedForce +=
            force;
    }


    public void AddImpulse(
        Vector3 impulse)
    {
        if (isStatic)
            return;


        velocity +=
            impulse / mass;
    }


    public void Stop()
    {
        velocity =
            Vector3.zero;


        acceleration =
            Vector3.zero;


        accumulatedForce =
            Vector3.zero;
    }



    /*
    ==========================
          DEBUG DATA
    ==========================
    */


    void UpdateDebug()
    {
        speed =
            velocity.magnitude;


        kineticEnergy =
            0.5f
            *
            mass
            *
            velocity.sqrMagnitude;


        momentum =
            mass
            *
            speed;
    }



    /*
    ==========================
            GIZMOS
    ==========================
    */


    void OnDrawGizmosSelected()
    {
        if (collisionBoxes == null)
            return;


        foreach (
            CollisionBox box
            in collisionBoxes)
        {
            if (box == null)
                continue;


            if (!box.enabled)
                continue;


            Matrix4x4 old =
                Gizmos.matrix;


            Gizmos.matrix =
                Matrix4x4.TRS(
                    box.WorldCenter(
                        transform
                    ),
                    box.WorldRotation(
                        transform
                    ),
                    Vector3.one
                );


            Gizmos.color =
                grounded
                ? Color.green
                : Color.cyan;


            Gizmos.DrawWireCube(
                Vector3.zero,
                box.size
            );


            Gizmos.matrix =
                old;
        }


        Gizmos.color =
            Color.red;


        Gizmos.DrawRay(
            transform.position,
            groundNormal
            * 2f
        );
    }
}



