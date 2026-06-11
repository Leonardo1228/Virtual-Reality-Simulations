using UnityEngine;
using System.Collections.Generic;


public enum CollisionBoxPart
{
    Body,
    FrontWheel,
    RearWheel,
    Other
}


[System.Serializable]
public class CollisionBox
{
    public bool enabled = true;


    [Header("Part")]

    public CollisionBoxPart part =
        CollisionBoxPart.Body;


    [Header("Local Transform")]

    public Vector3 center;


    public Vector3 rotation;


    [Header("Size")]

    public Vector3 size =
        Vector3.one;


    [Header("Collision Response")]

    [Min(0f)]
    public float separationMultiplier =
        1f;


    [Min(0f)]
    public float penetrationMultiplier =
        1f;


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


    public Vector3 WorldSize(
        Transform root)
    {
        Vector3 scale =
            root.lossyScale;


        scale =
            new Vector3(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z)
            );


        return Vector3.Scale(
            size,
            scale
        );
    }


    public Vector3 WorldHalfExtents(
        Transform root)
    {
        return WorldSize(
            root
        ) * 0.5f;
    }
}




public class UnifiedPhysicsBody :
    MonoBehaviour
{
    static BoxCollider penetrationProbe;


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
        EnsureCollisionBoxes();


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


    protected virtual void Reset()
    {
        EnsureCollisionBoxes();
    }


    void EnsureCollisionBoxes()
    {
        if (
            collisionBoxes != null
            &&
            collisionBoxes.Length > 0
        )
        {
            return;
        }


        BoxCollider boxCollider =
            GetComponent<BoxCollider>();


        CollisionBox box =
            new CollisionBox();


        if (boxCollider != null)
        {
            box.center =
                boxCollider.center;


            box.size =
                boxCollider.size;
        }


        collisionBoxes =
            new CollisionBox[]
            {
                box
            };
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


    [Header("Vehicle Part Response")]

    public bool useVehiclePartResponse =
        true;


    [Min(0f)]
    public float wheelSeparationMultiplier =
        2f;


    [Min(0f)]
    public float wheelPenetrationMultiplier =
        2f;


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
       GROUND SYSTEM
==========================
*/

    [Header("Ground Detection")]

    [Tooltip(
        "Distance below collision boxes to search for ground"
    )]
    [Min(0.001f)]
    public float groundCheckDistance =
        0.1f;


    [Tooltip(
        "Snap object to ground when close"
    )]
    public bool snapToGround =
        true;


    [Tooltip(
        "Extra distance to keep object stable"
    )]
    [Min(0f)]
    public float groundStickOffset =
        0.01f;


    [Tooltip(
        "Prevent small bouncing while resting"
    )]
    public bool cancelDownwardVelocity =
        true;


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

    [Tooltip(
    "Fix objects starting inside colliders"
)]
    public bool solvePenetration = true;


    [Min(0f)]
    public float penetrationPadding = 0.005f;


    [Range(1, 5)]
    public int penetrationIterations = 2;


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
        /*
         Comprobar si estamos apoyados
         antes de movernos.
        */
        GroundCheck();


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


        /*
         Corrección final por si terminamos
         cerca del suelo.
        */
        GroundCheck();


        if (solvePenetration)
        {
            SolvePenetration();
        }


        accumulatedForce =
            Vector3.zero;
    }


    /*
==========================
    PENETRATION FIX
==========================
*/

    void SolvePenetration()
    {
        BoxCollider probe =
            GetPenetrationProbe();


        if (probe == null)
        {
            return;
        }


        for (
            int iteration = 0;
            iteration < penetrationIterations;
            iteration++
        )
        {
            bool fixedSomething =
                false;


            foreach (
                CollisionBox box
                in collisionBoxes
            )
            {
                if (box == null)
                    continue;


                if (!box.enabled)
                    continue;


                Collider[] overlaps =
                    Physics.OverlapBox(
                        box.WorldCenter(
                            transform
                        ),

                        box.WorldHalfExtents(
                            transform
                        ),

                        box.WorldRotation(
                            transform
                        ),

                        collisionMask,

                        QueryTriggerInteraction.Ignore
                    );


                foreach (
                    Collider col
                    in overlaps
                )
                {
                    if (IsOwnCollider(col))
                    {
                        continue;
                    }


                    Vector3 direction;

                    float distance;


                    probe.size =
                        box.WorldSize(
                            transform
                        );


                    probe.center =
                        Vector3.zero;


                    bool penetrated =
                        Physics.ComputePenetration(
                            probe,
                            box.WorldCenter(
                                transform
                            ),
                            box.WorldRotation(
                                transform
                            ),

                            col,
                            col.transform.position,
                            col.transform.rotation,

                            out direction,
                            out distance
                        );


                    if (!penetrated)
                        continue;


                    transform.position +=
                        direction *
                        (
                            distance
                            +
                            GetPenetrationPadding(
                                box
                            )
                        );


                    fixedSomething =
                        true;
                }
            }


            if (!fixedSomething)
            {
                break;
            }
        }
    }


    static BoxCollider GetPenetrationProbe()
    {
        if (penetrationProbe != null)
        {
            return penetrationProbe;
        }


        GameObject probeObject =
            new GameObject(
                "UnifiedPhysicsBody Box Penetration Probe"
            );


        probeObject.hideFlags =
            HideFlags.HideAndDontSave;


        penetrationProbe =
            probeObject.AddComponent
            <BoxCollider>();


        penetrationProbe.enabled =
            false;


        return penetrationProbe;
    }


    bool IsOwnCollider(
        Collider col)
    {
        if (col == null)
        {
            return true;
        }


        return
            col.transform == transform
            ||
            col.GetComponentInParent
            <UnifiedPhysicsBody>() == this;
    }


    float GetSeparationOffset(
        CollisionBox box)
    {
        float multiplier =
            GetPartMultiplier(
                box,
                box != null
                ? box.separationMultiplier
                : 1f,
                wheelSeparationMultiplier
            );


        return separationOffset *
               multiplier;
    }


    float GetPenetrationPadding(
        CollisionBox box)
    {
        float multiplier =
            GetPartMultiplier(
                box,
                box != null
                ? box.penetrationMultiplier
                : 1f,
                wheelPenetrationMultiplier
            );


        return penetrationPadding *
               multiplier;
    }


    float GetPartMultiplier(
        CollisionBox box,
        float boxMultiplier,
        float wheelMultiplier)
    {
        if (box == null)
        {
            return 1f;
        }


        float multiplier =
            boxMultiplier <= 0f
            ? 1f
            : boxMultiplier;


        if (
            useVehiclePartResponse
            &&
            IsWheelPart(
                box.part
            )
        )
        {
            multiplier *=
                wheelMultiplier <= 0f
                ? 1f
                : wheelMultiplier;
        }


        return multiplier;
    }


    bool IsWheelPart(
        CollisionBoxPart part)
    {
        return
            part == CollisionBoxPart.FrontWheel
            ||
            part == CollisionBoxPart.RearWheel;
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


        if (
            useGravity &&
            !grounded
        )
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
        GROUND CHECK
==========================
*/

    void GroundCheck()
    {
        grounded = false;

        groundNormal =
            Vector3.up;


        float bestDistance =
            float.MaxValue;


        RaycastHit bestHit =
            new RaycastHit();


        foreach (
            CollisionBox box
            in collisionBoxes
        )
        {
            if (box == null)
                continue;


            if (!box.enabled)
                continue;


            Vector3 origin =
                box.WorldCenter(
                    transform
                );


            RaycastHit[] hits =
                Physics.BoxCastAll(
                    origin,

                    box.WorldHalfExtents(
                        transform
                    ),

                    Vector3.down,

                    box.WorldRotation(
                        transform
                    ),

                    groundCheckDistance
                    +
                    skinWidth,

                    collisionMask,

                    QueryTriggerInteraction.Ignore
                );


            foreach (
                RaycastHit hit
                in hits
            )
            {
                if (IsOwnCollider(hit.collider))
                    continue;


                float angle =
                    Vector3.Angle(
                        hit.normal,
                        Vector3.up
                    );


                /*
                Solo superficies caminables.
                */
                if (
                    angle >
                    maxGroundAngle
                )
                {
                    continue;
                }


                if (
                    hit.distance <
                    bestDistance
                )
                {
                    bestDistance =
                        hit.distance;


                    bestHit =
                        hit;
                }
            }
        }


        /*
        No encontramos suelo.
        */
        if (
            bestDistance ==
            float.MaxValue
        )
        {
            return;
        }


        grounded = true;


        groundNormal =
            bestHit.normal;


        /*
        Mantener el cuerpo pegado
        al terreno.
        */
        if (
            snapToGround
            &&
            bestDistance >
            0f
        )
        {
            float correction =
                bestDistance
                -
                groundStickOffset;


            if (
                correction > 0f
            )
            {
                transform.position -=
                    Vector3.up
                    *
                    correction;
            }
        }


        /*
        Eliminar caída residual.
        */
        if (
            cancelDownwardVelocity
            &&
            velocity.y < 0f
        )
        {
            velocity.y = 0f;
        }
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
            velocity * dt;


        float distance =
            movement.magnitude;


        /*
        Movimientos demasiado pequeños
        no necesitan comprobación.
        */
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


        /*
        No hubo impacto.
        */
        if (!collided)
        {
            transform.position +=
                movement;

            return;
        }


        /*
        Nos movemos hasta el punto
        anterior al impacto.
        */
        float safeDistance =
            Mathf.Max(
                hit.distance -
                skinWidth,
                0f
            );


        transform.position +=
            direction *
            safeDistance;


        /*
        Separación mínima para evitar
        quedar pegado dentro del collider.
        */
        transform.position +=
            hit.normal *
            GetSeparationOffset(
                sourceBox
            );


        /*
        Resolver el tipo de impacto.
        */
        HandleCollision(
            hit,
            sourceBox
        );
    }


    /*
==========================
       HANDLE COLLISION
==========================
*/

    void HandleCollision(
        RaycastHit hit,
        CollisionBox sourceBox)
    {
        float angle =
            Vector3.Angle(
                hit.normal,
                Vector3.up
            );


        /*
        Guardamos normal del suelo.
        */
        if (angle <= maxGroundAngle)
        {
            grounded = true;

            groundNormal =
                hit.normal;
        }


        UnifiedPhysicsBody other =
            hit.collider
            .GetComponentInParent
            <UnifiedPhysicsBody>();


        /*
        Colisión contra otro cuerpo.
        */
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

            return;
        }


        /*
        Mundo estático:
        piso, muro, rampa, modelo.
        */
        ResolveStaticCollision(
            hit.normal
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


            RaycastHit[] hits =
                Physics.BoxCastAll(
                    box.WorldCenter(
                        transform
                    ),

                    box.WorldHalfExtents(
                        transform
                    ),

                    direction,

                    box.WorldRotation(
                        transform
                    ),

                    distance +
                    skinWidth,

                    collisionMask,

                    QueryTriggerInteraction.Ignore
                );


            foreach (
                RaycastHit hit
                in hits
            )
            {
                if (IsOwnCollider(hit.collider))
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
        float angle =
            Vector3.Angle(
                normal,
                Vector3.up
            );


        /*
        ==========================
                GROUND
        ==========================
        */
        if (angle <= maxGroundAngle)
        {
            /*
            Quitamos únicamente
            la velocidad que empuja
            hacia el suelo.
            */

            float downward =
                Vector3.Dot(
                    velocity,
                    -normal
                );


            if (downward > 0f)
            {
                velocity +=
                    normal *
                    downward;
            }


            /*
            Dejamos el movimiento
            paralelo a la superficie.
            */

            velocity =
                Vector3.ProjectOnPlane(
                    velocity,
                    normal
                );


            /*
            Fricción por contacto.
            */

            velocity *=
                collisionDamping;


            return;
        }


        /*
        ==========================
                  CEILING
        ==========================
        */
        if (angle > 150f)
        {
            float upward =
                Vector3.Dot(
                    velocity,
                    normal
                );


            if (upward < 0f)
            {
                velocity -=
                    normal *
                    upward;
            }


            return;
        }


        /*
        ==========================
                  WALL
        ==========================
        */

        float impact =
            Vector3.Dot(
                velocity,
                normal
            );


        /*
        Si se aleja de la pared
        no hacemos nada.
        */
        if (impact >= 0f)
        {
            return;
        }


        /*
        Rebote usando restitution.
        */

        velocity -=
            (1f + restitution)
            *
            impact
            *
            normal;


        /*
        Reducimos energía del choque.
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
