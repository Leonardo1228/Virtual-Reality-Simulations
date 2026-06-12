using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public HybridVehicle vehicle;

    public Wind wind;

    public BrickWall brickWall;

    public HeavyWall heavyWall;

    public static SimulationManager Instance;

    void Awake()
    {
        Instance = this;
    }
}
