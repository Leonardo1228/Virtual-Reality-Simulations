using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialWall : MonoBehaviour
{
    public float slowMultiplier = 0.9f;

    void Reset()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        UnifiedPhysicsBody body =
            other.GetComponent<UnifiedPhysicsBody>();

        if (body == null)
            return;

        body.velocity *= slowMultiplier;
    }

    void OnDrawGizmos()
    {
        Collider col =
            GetComponent<Collider>();

        if (col == null)
            return;

        Gizmos.color = Color.yellow;

        if (col is BoxCollider box)
        {
            Matrix4x4 old = Gizmos.matrix;

            Gizmos.matrix =
                transform.localToWorldMatrix;

            Gizmos.DrawWireCube(
                box.center,
                box.size
            );

            Gizmos.matrix = old;
        }
    }
}