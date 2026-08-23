using UnityEngine;
using System.Collections.Generic;

public class TrafficGenerator : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public RunTiming timing;
    public RoadManager road;
    public BikeController bike;
    public float carHeight = 0.5f;

    [Header("On-path (forced dodge) cars")]
    [Range(0f, 1f)] public float spawnChance = 0.4f;
    [Range(0.3f, 0.9f)] public float carSpeedFraction = 0.6f;
    public float minDetourMargin = 2f;
    public float onPathDespawnBuffer = 1.5f; // seconds after its encounter before an on-path car disappears

    [Header("Ambient off-path traffic")]
    public float ambientSpacingSeconds = 3f;
    [Range(0f, 1f)] public float ambientSpawnChance = 0.5f;
    public float ambientDespawnBuffer = 3f; // seconds after spawn before an ambient car disappears

    public void SpawnTraffic(List<float> beatTimes, List<float> pathXs)
    {
        SpawnOnPathCars(beatTimes, pathXs);
        SpawnAmbientCars(timing.duration);
    }

    public float straightThreshold = 0.5f; // beats closer than this in X count as "straight" - skip spawning

    void SpawnOnPathCars(List<float> beatTimes, List<float> pathXs)
    {
        for (int i = 0; i < beatTimes.Count - 1; i++)
        {
            float tA = beatTimes[i];
            float tB = beatTimes[i + 1];
            float gap = tB - tA;

            float xDiff = Mathf.Abs(pathXs[i + 1] - pathXs[i]);
            if (xDiff < straightThreshold) continue; // straight run - leave it alone, per your fix

            float maxPhysicalDelta = bike.weavingSpeed * gap * 0.75f;
            float slack = maxPhysicalDelta - xDiff;
            if (slack < minDetourMargin) continue; // curve already eats most of the dodge room - still skip

            if (Random.value > spawnChance) continue;

            float tEncounter = (tA + tB) / 2f;
            float xEncounter = Mathf.Lerp(pathXs[i], pathXs[i + 1], 0.5f);
            float zEncounter = timing.getZ(tEncounter);

            float bikeSpeedAtEncounter = timing.startSpeed + timing.GetAcceleration() * tEncounter;
            float carSpeed = bikeSpeedAtEncounter * carSpeedFraction;
            float spawnZ = zEncounter - carSpeed * tEncounter;

            SpawnCar(xEncounter, spawnZ, carSpeed, tEncounter + onPathDespawnBuffer);
        }
    }

    void SpawnAmbientCars(float songDuration)
    {
        float maxX = road.HalfWidth;
        for (float t = 0f; t < songDuration; t += ambientSpacingSeconds)
        {
            if (Random.value > ambientSpawnChance) continue;

            float z = timing.getZ(t);
            float side = Random.value < 0.5f ? -1f : 1f;
            float x = side * Random.Range(maxX * 0.6f, maxX * 0.9f);

            float carSpeed = (timing.startSpeed + timing.GetAcceleration() * t) * carSpeedFraction;
            float spawnZ = z - carSpeed * t;

            SpawnCar(x, spawnZ, carSpeed, t + ambientDespawnBuffer); // despawn time added
        }
    }

    void SpawnCar(float x, float spawnZ, float carSpeed, float despawnTime) // new param
    {
        GameObject prefab = PickRandomPrefab();
        if (prefab == null) return;

        GameObject car = Instantiate(prefab, new Vector3(x, carHeight, spawnZ), Quaternion.identity, transform);
        car.GetComponent<TrafficCar>().Init(carSpeed, spawnZ, x, carHeight, despawnTime); // passed through
    }

    GameObject PickRandomPrefab()
    {
        List<GameObject> valid = new List<GameObject>();
        foreach (var p in carPrefabs)
            if (p != null) valid.Add(p);

        return valid.Count == 0 ? null : valid[Random.Range(0, valid.Count)];
    }
}