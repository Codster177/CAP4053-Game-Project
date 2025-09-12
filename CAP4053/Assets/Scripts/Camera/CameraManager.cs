using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager publicCameraManager;
    [SerializeField]
    private CinemachineCamera mainCamera;
    private List<Transform> cameraQueue = new List<Transform>();
    private Transform currentCamera;


    void Awake()
    {
        publicCameraManager = this;
    }

    public void AddToQueue(Transform cameraPos)
    {
        for (int i = 0; i < cameraQueue.Count; i++)
        {
            if (cameraQueue[i] == cameraPos)
            {
                return;
            }
        }
        cameraQueue.Add(cameraPos);
        CheckForFront();
    }
    public void DequeueRoom(Transform cameraPos)
    {
        cameraQueue.Remove(cameraPos);
        CheckForFront();
    }
    private void CheckForFront()
    {
        if (cameraQueue.Count <= 0)
        {
            return;
        }
        if (currentCamera != cameraQueue[0])
        {
            currentCamera = cameraQueue[0];
            mainCamera.Follow = currentCamera;
            mainCamera.LookAt = currentCamera;
        }
    }

}
