using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialWall : MonoBehaviour
{
    [Range(0f, 10f)]
    public float slowForce = 2f;

    void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null || rb.isKinematic)
            return;

        Vector3 slowdown = -rb.linearVelocity * slowForce;

        rb.AddForce(slowdown, ForceMode.Force);
    }

    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = Color.yellow;

        if (col is BoxCollider box)
        {
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(box.center, box.size);

            Gizmos.matrix = old;
        }
    }
}