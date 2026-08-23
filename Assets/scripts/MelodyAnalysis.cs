using UnityEngine;
using System.Collections.Generic;

public class MelodyAnalysis : MonoBehaviour
{
    [Header("Band edges (Hz), low to high - melody/vocal range")]
    public float[] bandEdges = { 200f, 500f, 1000f, 2000f, 4000f };
    public int windowSize = 1024;

    public float[] ComputeCentroids(AudioClip clip)
    {
        int channels = clip.channels;
        int totalSamples = clip.samples;
        float[] raw = new float[totalSamples * channels];
        clip.GetData(raw, 0);

        float[] mono = new float[totalSamples];
        for (int i = 0; i < totalSamples; i++)
        {
            float sum = 0f;
            for (int c = 0; c < channels; c++) sum += raw[i * channels + c];
            mono[i] = sum / channels;
        }

        int bandCount = bandEdges.Length;
        float[][] lowPassed = new float[bandCount][];
        for (int b = 0; b < bandCount; b++)
            lowPassed[b] = LowPassFilter(mono, clip.frequency, bandEdges[b]);

        int numWindows = totalSamples / windowSize;
        float[] centroids = new float[numWindows];

        for (int w = 0; w < numWindows; w++)
        {
            int start = w * windowSize;
            float weightedSum = 0f;
            float totalEnergy = 0f;

            for (int b = 0; b < bandCount - 1; b++)
            {
                float bandEnergy = 0f;
                for (int i = start; i < start + windowSize; i++)
                {
                    float bandSample = lowPassed[b + 1][i] - lowPassed[b][i]; // band-pass = diff of two low-pass
                    bandEnergy += bandSample * bandSample;
                }
                float bandCenter = (bandEdges[b] + bandEdges[b + 1]) / 2f;
                weightedSum += bandCenter * bandEnergy;
                totalEnergy += bandEnergy;
            }

            centroids[w] = totalEnergy > 0.00001f ? weightedSum / totalEnergy : bandEdges[0];
        }

        return centroids;
    }

    public float SampleCentroidAtTime(float[] centroids, float time, int sampleRate)
    {
        int w = Mathf.Clamp(Mathf.FloorToInt(time * sampleRate / windowSize), 0, centroids.Length - 1);
        return centroids[w];
    }

    float[] LowPassFilter(float[] input, float sampleRate, float cutoffHz)
    {
        float[] output = new float[input.Length];
        float rc = 1f / (2f * Mathf.PI * cutoffHz);
        float dt = 1f / sampleRate;
        float alpha = dt / (rc + dt);
        output[0] = input[0];
        for (int i = 1; i < input.Length; i++)
            output[i] = output[i - 1] + alpha * (input[i] - output[i - 1]);
        return output;
    }
}