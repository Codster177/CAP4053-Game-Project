using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    private CameraManager cameraManager;

    void Start()
    {
        cameraManager = CameraManager.publicCameraManager;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(cameraManager);
        if (collision.tag == "Player")
        {
            cameraManager.AddToQueue(transform);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            cameraManager.DequeueRoom(transform);
        }
    }
}
