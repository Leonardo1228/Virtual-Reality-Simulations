using UnityEngine;

public class TutorialWall : MonoBehaviour
{
    [Header("Wall")]

    public bool autoUseScale = true;

    public Vector3 halfExtents =
        new Vector3(
            5f,
            2f,
            0.5f
        );

    public Vector3 Extents
    {
        get
        {
            if (autoUseScale)
            {
                return
                    transform.localScale
                    * 0.5f;
            }

            return halfExtents;
        }
    }

    void OnEnable()
    {
        VehicleCollision.allTutorialWalls.Add(this);
    }

    void OnDisable()
    {
        VehicleCollision.allTutorialWalls.Remove(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Matrix4x4 old =
            Gizmos.matrix;

        Gizmos.matrix =
            Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                Vector3.one
            );

        Gizmos.DrawWireCube(
            Vector3.zero,
            Extents * 2f
        );

        Gizmos.matrix = old;
    }
}
