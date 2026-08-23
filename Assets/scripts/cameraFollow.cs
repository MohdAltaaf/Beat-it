
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject Bike;
    public float distance = 3;
    public float height = 2f;
    public float followSpeed = 5f; 

    
    void Start()
    {
        
    }

    // Update is called once per frame
   public bool followEnabled = true;

    void LateUpdate()
    {
        if (!followEnabled) return;
        Vector3 targetPosition = Bike.transform.position + new Vector3(0, height, -distance + 5f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
        transform.LookAt(Bike.transform.position);
    }
}
