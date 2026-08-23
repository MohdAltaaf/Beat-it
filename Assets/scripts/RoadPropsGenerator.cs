using UnityEngine;

public class RoadPropsGenerator : MonoBehaviour
{
    public RoadManager road;
    public RunTiming timing;

    [Header("Fences (continuous, both edges)")]
    public GameObject[] fencePrefabs;
    public float fenceSpacing = 5f;
    public float fenceEdgeOffset = 1f;

    [Header("Clutter (sparse, random - cones etc)")]
    public GameObject[] clutterPrefabs;
    public float clutterSpacing = 15f;
    [Range(0f, 1f)] public float clutterSpawnChance = 0.5f;
    public float clutterEdgeOffsetMin = 1f;
    public float clutterEdgeOffsetMax = 4f;

    public void GenerateProps()
    {
        float totalLength = (timing.startSpeed + timing.maxSpeed) / 2f * timing.duration;
        float halfWidth = road.HalfWidth;

        SpawnFences(totalLength, halfWidth);
        SpawnClutter(totalLength, halfWidth);
    }

    void SpawnFences(float totalLength, float halfWidth)
    {
        if (fencePrefabs.Length == 0) return;
        for (float z = 0; z < totalLength; z += fenceSpacing)
        {
            SpawnOne(fencePrefabs, new Vector3(-halfWidth - fenceEdgeOffset, 0f, z), false);
            SpawnOne(fencePrefabs, new Vector3(halfWidth + fenceEdgeOffset, 0f, z), true);
        }
    }

    void SpawnClutter(float totalLength, float halfWidth)
    {
        if (clutterPrefabs.Length == 0) return;
        for (float z = 0; z < totalLength; z += clutterSpacing)
        {
            if (Random.value > clutterSpawnChance) continue;
            float side = Random.value < 0.5f ? -1f : 1f;
            float offset = Random.Range(clutterEdgeOffsetMin, clutterEdgeOffsetMax);
            SpawnOne(clutterPrefabs, new Vector3(side * (halfWidth + offset), 0f, z), false);
        }
    }

    void SpawnOne(GameObject[] prefabs, Vector3 pos, bool flip)
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        Quaternion rot = flip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        GameObject obj = Instantiate(prefab, pos, rot, transform);
        obj.isStatic = true; // these never move - lets Unity batch them for cheaper rendering
    }
}