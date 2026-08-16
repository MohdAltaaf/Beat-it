using UnityEngine;

public class RunTiming : MonoBehaviour
{
    public float startSpeed = 10f;
    public float maxSpeed = 30f; 
    [HideInInspector] public float duration; //set once clip loads

    public float GetAcceleration() => (maxSpeed - startSpeed)/duration;

    public float getZ(float t) => startSpeed * t + 0.5f * GetAcceleration() *t*t;


}
