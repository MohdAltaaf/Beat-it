using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class AudioLoader : MonoBehaviour
{
    private AudioClip loadedClip;
  
    public RunTiming runTiming;
    public GameState gameState;
    
   
    public string TestFilePath = @"C:\Users\KSF\Downloads\Music\TestSubject.mp3";

    private double songStartDspTime;
    

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
            
            runTiming.duration = loadedClip.length;

            float[] samples = new float [loadedClip.samples * loadedClip.channels];
            loadedClip.GetData(samples, 0);

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
                GetComponent<LevelGenerator>().SpawnBeatBlocks(runTiming, beats, loadedClip);

                var pathXs = GetComponent<LevelGenerator>().SpawnBeatBlocks(runTiming, beats, loadedClip);
                GetComponent<TrafficGenerator>().SpawnTraffic(beats, pathXs);

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

    public bool IsPlaying {get; private set;} = false;
    public IEnumerator TestPlayback(AudioClip clip, List<float> beats)
    {
        AudioSource src = GetComponent<AudioSource>();
        
        src.clip = clip;
        songStartDspTime = AudioSettings.dspTime;
        src.Play();
        IsPlaying = true;

        int nextBeat = 0;
        while(src.isPlaying)
        {
            float elapsed = GetElapsedSongTime();
            if(nextBeat < beats.Count && src.time >= beats[nextBeat])
            {
                Debug.Log($"BEAT at {elapsed: F2}s");
                nextBeat++;
            }
            yield return null;

        }
        if (!gameState.IsGameOver)
            gameState.TriggerWin();
    }

    public float GetElapsedSongTime()
    {
        return (float)(AudioSettings.dspTime - songStartDspTime);
    }
   
  public string fallbackTestPath = @"C:\Users\KSF\Downloads\Music\TestSubject.mp3"; // lets you test this scene directly, without going through the menu each time

    void Start()
    {
        string path = string.IsNullOrEmpty(GameSession.SelectedSongPath) ? fallbackTestPath : GameSession.SelectedSongPath;
        StartCoroutine(loadMp3(path));
    }


   
}



