using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public float roadWidth = 5;
    public float roadLength = 100;
    

    public float HalfWidth => roadWidth * 10f - 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale= new Vector3(2*roadWidth, 1, roadLength);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }
}
