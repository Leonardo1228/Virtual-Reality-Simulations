using UnityEngine;

public class Brick : UnifiedPhysicsBody
{
    [Header("Brick")]

    public bool activated = false;


    [Range(0f, 1f)]
    public float damping = 0.995f;


    void Reset()
    {
        mass = 4f;

        drag = 0.05f;

        restitution = 0.25f;


        /*
         Un ladrillo del muro
         comienza inmóvil.
        */
        isStatic = true;


        useGravity = false;
    }


    protected override void Update()
    {
        base.Update();


        /*
         Solo los ladrillos rotos
         pierden velocidad con el tiempo.
        */
        if (activated)
        {
            velocity *= damping;
        }
    }


    public void Activate(
        Vector3 impactForce)
    {
        if (activated)
            return;


        activated = true;


        /*
         Ahora entra a la simulación.
        */
        isStatic = false;


        useGravity = true;


        /*
         El impacto inicial se trata
         como un impulso instantáneo.
        */
        AddImpulse(
            impactForce
        );
    }
}