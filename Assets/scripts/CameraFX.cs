using UnityEngine;

public class CameraFX : MonoBehaviour
{
    public Camera cam;
    public float punchAmount = 8f;
    public float recoverySpeed = 4f; // higher = snaps back to normal faster

    private float baseFOV;
    private float targetFOV;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        baseFOV = cam.fieldOfView;
        targetFOV = baseFOV;
    }

    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, 1f - Mathf.Exp(-recoverySpeed * Time.deltaTime));
    }

    public void Punch()
    {
        cam.fieldOfView = baseFOV + punchAmount; // instant jump up
        targetFOV = baseFOV;                     // eases back down every frame after
    }
}