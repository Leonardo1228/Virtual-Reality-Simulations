using UnityEngine;

[CreateAssetMenu(
    fileName = "PhysicsProfile",
    menuName = "Simulation/Physics Profile"
)]
public class PhysicsProfile : ScriptableObject
{
    [Header("Linear")]
    public float mass = 1000f;

    public float drag = 0.2f;

    public float restitution = 0.2f;

    public float radius = 1f;

    [Header("Gravity")]
    public bool useGravity = true;

    public Vector3 gravity =
        new Vector3(0f, -9.81f, 0f);

    [Header("Angular")]
    public float rotationalDrag = 0.98f;
}
