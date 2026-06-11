using System.Collections.Generic;
using UnityEngine;

public interface IForceGenerator
{
    void ApplyForces(float dt);
}

public class ForceManager : MonoBehaviour
{
    public static List<IForceGenerator> generators = new();

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        foreach (var g in generators)
        {
            g.ApplyForces(dt);
        }
    }

    public static void Register(IForceGenerator g)
    {
        if (!generators.Contains(g))
            generators.Add(g);
    }

    public static void Unregister(IForceGenerator g)
    {
        generators.Remove(g);
    }
}
