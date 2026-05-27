using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WallGenerator : MonoBehaviour
{
    [Header("Brick")]
    public GameObject brickPrefab;

    [Header("Wall Size")]
    public int width = 5;

    public int height = 4;

    [Header("Brick Size")]
    public Vector3 brickSize =
        new Vector3(1f, 0.5f, 0.5f);

    [Header("Spacing")]
    public float spacing = 0.05f;

    [Header("Editor")]
    public bool generateInEditor;

    public bool clearInEditor;

    void Start()
    {
        GenerateWall();
    }

#if UNITY_EDITOR

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (generateInEditor)
            {
                generateInEditor = false;

                GenerateWall();
            }

            if (clearInEditor)
            {
                clearInEditor = false;

                ClearWall();
            }
        }
    }

#endif

    public void GenerateWall()
    {
        ClearWall();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float offset =
                    (y % 2 == 0)
                    ? 0f
                    : brickSize.x * 0.5f;

                Vector3 pos =
                    transform.position +
                    new Vector3(
                        x * (brickSize.x + spacing)
                        + offset,

                        y * (brickSize.y + spacing),

                        0f
                    );

                GameObject brick =
                    Instantiate(
                        brickPrefab,
                        pos + new Vector3(
                        0f,
                        0f,
                        Random.Range(-0.002f, 0.002f)
                        ),
                        Quaternion.identity,
                        transform
                    );

                brick.transform.localScale =
                    brickSize;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Undo.RegisterCreatedObjectUndo(
                        brick,
                        "Generate Wall"
                    );
                }
#endif
            }
        }
    }

    public void ClearWall()
    {
        while (transform.childCount > 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.DestroyObjectImmediate(
                    transform.GetChild(0).gameObject
                );
            }
            else
#endif
            {
                Destroy(
                    transform.GetChild(0).gameObject
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 wallSize =
            new Vector3(
                width * brickSize.x,
                height * brickSize.y,
                brickSize.z
            );

        Gizmos.DrawWireCube(
            transform.position
            + new Vector3(
                wallSize.x * 0.5f,
                wallSize.y * 0.5f,
                0f
            ),

            wallSize
        );
    }
}
