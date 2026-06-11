using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialWall : MonoBehaviour
{
    [Range(0f, 10f)]
    public float slowForce = 2f;


    void Reset()
    {
        Collider col =
            GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }


    void OnTriggerStay(
        Collider other)
    {
        UnifiedPhysicsBody body =
            other.GetComponentInParent<UnifiedPhysicsBody>();

        if (body == null)
            return;


        float factor =
            Mathf.Clamp01(
                1f -
                slowForce *
                Time.deltaTime
            );


        body.velocity *= factor;
    }


    void OnDrawGizmos()
    {
        Collider col =
            GetComponent<Collider>();

        if (col == null)
            return;


        Gizmos.color =
            Color.yellow;


        if (col is BoxCollider box)
        {
            Matrix4x4 old =
                Gizmos.matrix;


            Gizmos.matrix =
                transform.localToWorldMatrix;


            Gizmos.DrawWireCube(
                box.center,
                box.size
            );


            Gizmos.matrix =
                old;
        }
    }
}