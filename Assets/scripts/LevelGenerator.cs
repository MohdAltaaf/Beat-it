using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{

    public GameObject beatBlocPrefab;
   
    public float blockHeight = 0.5f;

    public void spawnBeatBlocks(RunTiming timing, List<float> beatTimes)
    {
        foreach(float t in beatTimes)
        {
            float z = timing.getZ(t);
            Vector3 spawnPos = new Vector3(0, blockHeight, z);
            Instantiate(beatBlocPrefab, spawnPos, Quaternion.identity, transform);
        }
    }


}
