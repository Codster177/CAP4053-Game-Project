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

    public static void ChangeTestModePrefab(int testVal)
    {
        GameObject room0 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Rooms/Room 0.prefab");
        if (room0 == null)
        {
            return;
        }
        RoomPrefab prefab = room0.GetComponent<RoomPrefab>();
        NavMeshModifier[] navMeshModifiers = prefab.GetNavMeshModifiers();
        if ((testVal == 1) || (testVal == 2))
        {
            navMeshModifiers[0].ignoreFromBuild = false;
            navMeshModifiers[1].ignoreFromBuild = true;
            navMeshModifiers[2].ignoreFromBuild = true;
        }
        else if (testVal == 3)
        {
            navMeshModifiers[0].ignoreFromBuild = true;
            navMeshModifiers[1].ignoreFromBuild = false;
            navMeshModifiers[2].ignoreFromBuild = false;
        }
        else
        {
            Debug.LogError("RoomController: ChangeTestModePrefab: TestVal != 1, 2, or 3.");
        }
    }

    public void ExitRoomCheck()
    {
        float distanceFromCenter = Vector3.Distance(GameManager.publicGameManager.GetPlayerLocation().position, transform.position);
        if (distanceFromCenter < 6f)
        {
            safeRoom = false;
            StartCoroutine(GenerationManager.publicGenerationManager.LoadExitRoom(roomPrefab));
        }
    }
}
