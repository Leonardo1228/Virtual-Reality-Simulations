using UnityEngine;
using System.Collections.Generic;

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

    private readonly List<Brick> bricks =
        new List<Brick>();

    void OnEnable()
    {
        if (!VehicleCollision
            .allBrickWalls
            .Contains(this))
        {
            VehicleCollision
                .allBrickWalls
                .Add(this);
        }
    }

    void OnDisable()
    {
        VehicleCollision
            .allBrickWalls
            .Remove(this);
    }

    void Start()
    {
        if (generateOnStart)
        {
            GenerateWall();
        }
    }

    public void GenerateWall()
    {
        if (brickPrefab == null)
            return;

        if (clearFirst)
        {
            ClearWall();
        }

        bricks.Clear();

        float wallWidth =
            width *
            (brickSize.x + spacing);

        Vector3 startPos =
            transform.position
            - new Vector3(
                wallWidth * 0.5f,
                0f,
                0f
            )
            + new Vector3(
                0f,
                brickSize.y * 0.5f,
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

                GameObject obj =
                    Instantiate(
                        brickPrefab,
                        pos,
                        transform.rotation,
                        transform
                    );

                obj.transform.localScale =
                    brickSize;

                Brick brick =
                    obj.GetComponent<Brick>();

                if (brick != null)
                {
                    brick.useGravity =
                        false;

                    bricks.Add(
                        brick
                    );
                }
            }
        }
    }

    public void BreakWall(
        Vector3 impactPoint,
        Vector3 impactForce,
        float radius)
    {
        foreach (
            Brick brick
            in bricks
        )
        {
            if (brick == null)
                continue;

            if (brick.activated)
                continue;

            float distance =
                Vector3.Distance(
                    brick.transform.position,
                    impactPoint
                );

            if (distance > radius)
                continue;

            float strength =
                1f -
                Mathf.Clamp01(
                    distance / radius
                );

            brick.Activate(
                impactForce
                * strength
            );
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color =
            Color.yellow;

        float wallWidth =
            width *
            (
                brickSize.x
                + spacing
            );

        float wallHeight =
            height *
            (
                brickSize.y
                + spacing
            );

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
                    transform
                    .GetChild(0)
                    .gameObject
                );
            }
            else
#endif
            {
                Destroy(
                    transform
                    .GetChild(0)
                    .gameObject
                );
            }
        }
    }
}