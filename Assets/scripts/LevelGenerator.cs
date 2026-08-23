using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{

    public GameObject beatBlocPrefab;
    public MelodyAnalysis melody;
    public BikeController bike;
    public RoadManager road;
    

    public float blockHeight = 0.5f;


   public float pathIntensity = 1f;     // how far the path swings off-center - now directly visible
public float smoothing = 0.5f;       // higher = longer sustained curves, lower = flips more often
public int localAverageBeats = 8;    // how many recent beats define "normal" for this stretch

public List<float> SpawnBeatBlocks(RunTiming timing, List<float> beatTimes, AudioClip clip)
{
    float[] centroids = melody.ComputeCentroids(clip);
    float maxX = road.HalfWidth;

    List<float> pathXs = new List<float>();

    float[] beatCentroids = new float[beatTimes.Count];
    for (int i = 0; i < beatTimes.Count; i++)
        beatCentroids[i] = melody.SampleCentroidAtTime(centroids, beatTimes[i], clip.frequency);

    // how far each note sits from the *recent local* average - not the whole song's range.
    // this is what stops one-sided drift: it's always relative to where the melody has been lately.
    float[] deviation = new float[beatTimes.Count];
    float maxAbsDeviation = 0.0001f;
    for (int i = 0; i < beatTimes.Count; i++)
    {
        int start = Mathf.Max(0, i - localAverageBeats);
        int count = i - start;
        if (count <= 0) { deviation[i] = 0f; continue; }
        float localAvg = 0f;
        for (int k = start; k < i; k++) localAvg += beatCentroids[k];
        localAvg /= count;
        deviation[i] = beatCentroids[i] - localAvg;
        maxAbsDeviation = Mathf.Max(maxAbsDeviation, Mathf.Abs(deviation[i]));
    }

    float smoothedTarget = 0f;
    float prevX = 0f, prevT = 0f;
    bool first = true;

    for (int i = 0; i < beatTimes.Count; i++)
    {
        float t = beatTimes[i];
        float normalizedDev = Mathf.Clamp(deviation[i] / maxAbsDeviation, -1f, 1f);
        smoothedTarget = Mathf.Lerp(normalizedDev, smoothedTarget, smoothing); // carries direction across beats

        float targetX = Mathf.Clamp(smoothedTarget * pathIntensity * maxX, -maxX, maxX);

        float x;
        if (first) { x = 0f; first = false; }
        else
        {
            float maxPhysicalDelta = bike.weavingSpeed * (t - prevT) * 0.75f;
            x = Mathf.Clamp(targetX, prevX - maxPhysicalDelta, prevX + maxPhysicalDelta);
        }

        float z = timing.getZ(t);
        Instantiate(beatBlocPrefab, new Vector3(x, blockHeight, z), Quaternion.identity, transform);

        prevX = x; prevT = t;
        pathXs.Add(x);
    }
    return pathXs;
}


}
