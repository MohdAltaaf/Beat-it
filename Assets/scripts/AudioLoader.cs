using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class AudioLoader : MonoBehaviour
{
    private AudioClip loadedClip;
    
   
    public string TestFilePath = @"C:\Users\KSF\Downloads\Music\TestSubject.mp3";
    

    public IEnumerator loadMp3(string filePath)
    {
        
        string url = "file://" +filePath;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log ("Failed to Load MP3: " + www.error );
                yield break;
            }

            loadedClip = DownloadHandlerAudioClip.GetContent(www);
            Debug.Log($"loaded: {loadedClip.length}s, {loadedClip.frequency}Hz, {loadedClip.channels}ch");
            float[] samples = new float [loadedClip.samples * loadedClip.channels];
            loadedClip.GetData(samples, 0);

            Debug.Log ($"first 10 samples: {string.Join(", ", samples[..10])}");

            float threshold = 0.001f;
            int firstNonZero = -1;
            for(int i =0; i < samples.Length; i++)
            {
                if (Mathf.Abs(samples[i])>threshold)
                {
                    firstNonZero = i;
                    break;
                }
                
            }
            if(firstNonZero >= 0 )
            {
                float timeSeconds = (firstNonZero/loadedClip.channels)/(float)loadedClip.frequency;
                Debug.Log($"First moment of sound is at array index: {firstNonZero}, ~{timeSeconds:F2}s in.");

                var beats = GetComponent<audioAnalysis>().DetectBeats(loadedClip);
                Debug.Log($"Found {beats.Count} beats.");
                //Got beats ready to start playbackTest.
                StartCoroutine(TestPlayback(loadedClip, beats));
            }
            else
            {
                Debug.Log("Whole song is just 0s , somethins wrong. ");
            }
        }
    }
    public IEnumerator TestPlayback(AudioClip clip, List<float> beats)
    {
        AudioSource src = GetComponent<AudioSource>();
        src.clip = clip;
        src.Play();

        int nextBeat = 0;
        while(src.isPlaying)
        {
            if(nextBeat < beats.Count && src.time >= beats[nextBeat])
            {
                Debug.Log($"BEAT at {src.time: F2}s");
                nextBeat++;
            }
            yield return null;

        }
    }
   
    void Start()
    {
        StartCoroutine(loadMp3(TestFilePath));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
