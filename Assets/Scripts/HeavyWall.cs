using UnityEngine;

public class HeavyWall : UnifiedPhysicsBody
{
    [Header("Wall")]

    public bool fixedWall = true;


    [Tooltip(
        "Extra energy loss when hitting this wall"
    )]
    [Range(0f, 1f)]
    public float impactDamping = 0.8f;


    void Reset()
    {
        /*
         Un muro pesado tiene mucha masa.
        */
        mass = 10000f;


        /*
         Casi no rebota.
        */
        restitution = 0.05f;


        /*
         Poco arrastre.
        */
        drag = 0.02f;


        /*
         Normalmente no cae.
        */
        useGravity = false;


        /*
         Un muro fijo no se mueve.
        */
        isStatic = fixedWall;
    }


    void Start()
    {
        isStatic = fixedWall;
    }


    protected override void Update()
    {
        base.Update();


        /*
         Si el muro es móvil,
         reducimos un poco su energía
         después de los impactos.
        */
        if (!isStatic)
        {
            velocity *= impactDamping;
        }
    }


    public void MakeDynamic()
    {
        isStatic = false;
        useGravity = true;
    }


    public void Freeze()
    {
        Stop();

        isStatic = true;

        useGravity = false;
    }
}
