
using UnityEngine;
using UnityEngine.InputSystem;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BikeController : MonoBehaviour
{
    public float weavingSpeed = 2f;
    public float forwardSpeed = 10f;
    public float tiltSpeed =.5f;
    public float tiltAmount = 1f;
    public GameObject bikeVis;
    public RoadManager road;
    private float weaveInput;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.isKinematic = true;

    }
    void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        weaveInput = moveInput.x;

    }

    // Update is called once per frame
    void Update()
    {
        
        
        transform.Translate(new Vector3(weaveInput*weavingSpeed*Time.deltaTime, 0, forwardSpeed*Time.deltaTime));
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -road.roadWidth*10 +1, road.roadWidth*10 -1);
        transform.position = position;

        float zTilt = -tiltAmount*weaveInput;
        Quaternion targetRotation = Quaternion.Euler(0, 0, zTilt);
        bikeVis.transform.localRotation = Quaternion.Slerp(bikeVis.transform.localRotation, targetRotation, Time.deltaTime*tiltSpeed);
        
        
    }

}
