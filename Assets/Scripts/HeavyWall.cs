using UnityEngine;

public class HeavyWall : UnifiedPhysicsBody
{
    [Header("Wall Settings")]
    public float startMass = 10000f;
    public bool startWithGravity = true;

    void Reset()
    {
        mass = startMass;
    }

    void Start()
    {
        mass = startMass;
        useGravity = startWithGravity;
    }
}
