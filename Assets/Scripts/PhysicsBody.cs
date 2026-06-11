using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 force;

    public float mass = 1f;
    public float drag = 0.05f;
    public float restitution = 0.2f;

    public bool useGravity = true;
    public bool isStatic = false;

    [HideInInspector] public bool grounded;
    [HideInInspector] public Vector3 groundNormal = Vector3.up;

    public LayerMask collisionMask;
}
