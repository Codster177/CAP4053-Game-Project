using UnityEngine;

public class RoomController : MonoBehaviour
{

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private RoomTrigger roomTrigger;
    private CameraManager cameraManager;
    void Start()
    {
        cameraManager = CameraManager.publicCameraManager;
        roomTrigger.SetRoomController(this);
    }
    public void QueueRoom(Transform cameraPos)
    {
        cameraManager.AddToQueue(cameraPos);
    }
    public void DequeueRoom(Transform cameraPos)
    {
        cameraManager.DequeueRoom(cameraPos);
    }
    public void SpawnEnemies()
    {
        enemySpawner.SpawnEnemies();
    }
}
