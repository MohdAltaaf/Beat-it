using UnityEngine;
using System.Collections.Generic;

public class audioAnalysis : MonoBehaviour
{
    [Header("Detection Settings")]
    public int windowSize = 1024;
    [Range(1f, 5f)] public float sensitivity = 1.5f;
    public float minGapSeconds = 0.2f;
    [Range(20f, 500f)] public float lowPassCutoffHz = 150f;

    public List<float> DetectBeats(AudioClip clip)
    {
        int channels = clip.channels;
        int totalSamples = clip.samples;
        float[] raw = new float[totalSamples * channels];
        clip.GetData(raw, 0);

        float[] mono = new float[totalSamples];
        for (int i = 0; i < totalSamples; i++)
        {
            float sum = 0f;
            for (int c = 0; c < channels; c++)
                sum += raw[i * channels + c];
            mono[i] = sum / channels;
        }

        float[] bass = LowPassFilter(mono, clip.frequency, lowPassCutoffHz);

        int numWindows = totalSamples / windowSize;
        float[] energies = new float[numWindows];
        for (int w = 0; w < numWindows; w++)
        {
            float sumSquares = 0f;
            for (int i = 0; i < windowSize; i++)
            {
                float s = bass[w * windowSize + i];
                sumSquares += s * s;
            }
            energies[w] = sumSquares / windowSize;
        }

        int historySize = Mathf.RoundToInt(clip.frequency / (float)windowSize);
        List<float> beatTimes = new List<float>();
        float lastBeatTime = -minGapSeconds;

        for (int w = historySize; w < numWindows; w++)
        {
            float localAverage = 0f;
            for (int k = w - historySize; k < w; k++)
                localAverage += energies[k];
            localAverage /= historySize;

            float windowTime = (w * windowSize) / (float)clip.frequency;

            if (energies[w] > localAverage * sensitivity && windowTime - lastBeatTime >= minGapSeconds)
            {
                int windowStart = w * windowSize;
                int peakIndex = windowStart;
                float peakValue = 0f;
                for (int i = windowStart; i < windowStart + windowSize; i++)
                {
                    float abs = Mathf.Abs(bass[i]);
                    if (abs > peakValue)
                    {
                        peakValue = abs;
                        peakIndex = i;
                    }
                }

                float preciseTime = peakIndex / (float)clip.frequency;
                beatTimes.Add(preciseTime);
                lastBeatTime = preciseTime;
            }
        }

        return beatTimes;
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