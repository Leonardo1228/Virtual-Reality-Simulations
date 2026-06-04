using UnityEngine;

public class BrickWall : MonoBehaviour
{
    [Header("Brick")]

    public GameObject brickPrefab;

    [Header("Wall")]

    public int width = 8;

    public int height = 5;

    public Vector3 brickSize =
        new Vector3(
            1f,
            0.5f,
            0.5f
        );

    public float spacing = 0.02f;



    [Header("Generation")]

    public bool generateOnStart = true;

    public bool clearFirst = true;

    void Start()
    {
        if (generateOnStart)
        {
            GenerateWall();
        }
    }

#if UNITY_EDITOR

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            transform.localScale =
                Vector3.one;
        }
    }

#endif

    public void GenerateWall()
    {
        if (brickPrefab == null)
        {
            Debug.LogWarning(
                "Brick Prefab missing."
            );

            return;
        }

        if (clearFirst)
        {
            ClearWall();
        }

        float wallWidth =
            width *
            (brickSize.x + spacing);

        Vector3 startPos =
            transform.position
            - new Vector3(
                wallWidth * 0.5f,
                0f,
                0f
            );

        for (
            int y = 0;
            y < height;
            y++
        )
        {
            for (
                int x = 0;
                x < width;
                x++
            )
            {
                float offset =
                    (y % 2 == 0)
                    ? 0f
                    : brickSize.x * 0.5f;

                Vector3 pos =
                    startPos
                    + new Vector3(
                        x *
                        (
                            brickSize.x
                            + spacing
                        )
                        + offset,

                        y *
                        (
                            brickSize.y
                            + spacing
                        ),

                        0f
                    );

                GameObject brick =
                    Instantiate(
                        brickPrefab,
                        pos,
                        Quaternion.identity,
                        transform
                    );

                brick.transform.localScale =
                    brickSize;

                Rigidbody rb =
                    brick.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
    }

    public void BreakWall(
        Vector3 impactPoint,
        Vector3 impactForce,
        float radius = 3f
    )
    {
        Brick[] bricks =
            GetComponentsInChildren<Brick>();

        foreach (Brick brick in bricks)
        {
            float distance =
                Vector3.Distance(
                    brick.transform.position,
                    impactPoint
                );

            if (distance > radius)
                continue;

            Rigidbody rb =
                brick.GetComponent<Rigidbody>();


            if (rb != null)
            {
                rb.useGravity = true;
            }

            if (rb != null)
            {
                rb.isKinematic = false;

                rb.AddForce(
                    impactForce,
                    ForceMode.Impulse
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color =
            Color.yellow;

        float wallWidth =
            width *
            (brickSize.x + spacing);

        float wallHeight =
            height *
            (brickSize.y + spacing);

        Vector3 center =
            transform.position
            + new Vector3(
                0f,
                wallHeight * 0.5f,
                0f
            );

        Vector3 size =
            new Vector3(
                wallWidth,
                wallHeight,
                brickSize.z
            );

        Gizmos.DrawWireCube(
            center,
            size
        );
    }

    public void ClearWall()
    {
        while (
            transform.childCount > 0
        )
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(
                    transform.GetChild(0)
                    .gameObject
                );
            }
            else
#endif
            {
                Destroy(
                    transform.GetChild(0)
                    .gameObject
                );
            }
        }
    }
}
