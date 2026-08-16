using UnityEngine;

public class BeatBlock : MonoBehaviour
{
    public AudioClip hitSound;
    private bool isTriggered = false;
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OggerEnter(Collider other)
    {
        Debug.Log($"Trigger hit by: {other.gameObject.name}");
        if(isTriggered || !other.CompareTag("Bike")) return;
        isTriggered = true;
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 2f);     
    }
}
