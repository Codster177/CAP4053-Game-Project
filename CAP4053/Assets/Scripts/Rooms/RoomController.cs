using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEditor;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private RoomPrefab roomPrefab;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private RoomTrigger roomTrigger;
    [SerializeField] private bool safeRoom;
    [SerializeField] private static GameObject room0;
    private CameraManager cameraManager;
    void Start()
    {
        cameraManager = CameraManager.publicCameraManager;
        roomTrigger.SetRoomController(this);
    }
    void Update()
    {
        if (safeRoom && roomTrigger.GetInsideRoom())
        {
            ExitRoomCheck();
        }
        if ((!safeRoom) && (GameManager.publicGameManager.testMode == 1))
        {
            CloseDoorTest();
        }
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
        if (enemySpawner == null)
        {
            return;
        }
        if ((!safeRoom) && (!RoomProgressionManager.Instance.HasVisitedRoom(this)))
        {
            enemySpawner.SpawnEnemies();
        }
        if (!RoomProgressionManager.Instance.HasVisitedRoom(this))
        {
            RoomProgressionManager.Instance.RegisterRoomEntry(this);
        }
    }

    private void CloseDoorTest()
    {
        // Debug.Log($"Enemies Left: {EnemyManager.publicEnemyManager.GetEnemyList().Count}");

        if (EnemyManager.publicEnemyManager.GetEnemyList().Count > 0)
        {
            float distanceFromCenter = Vector3.Distance(GameManager.publicGameManager.GetPlayerLocation().position, transform.position);
            if (distanceFromCenter < 6f)
            {
                roomPrefab.OpenOrCloseObjects(true);
                GenerationManager.publicGenerationManager.CloseDoors(roomPrefab);
            }
        }
        else
        {
            roomPrefab.OpenOrCloseObjects(false);
            GenerationManager.publicGenerationManager.ReopenDoors(roomPrefab);
        }
    }

    private void ExitRoomCheck()
    {
        float distanceFromCenter = Vector3.Distance(GameManager.publicGameManager.GetPlayerLocation().position, transform.position);
        if (distanceFromCenter < 4f)
        {
            safeRoom = false;
            StartCoroutine(GenerationManager.publicGenerationManager.LoadExitRoom(roomPrefab));
        }
    }
    void OawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 6f);
    }
}
