using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.Tilemaps;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager publicGenerationManager;
    [SerializeField] private GenerateSegments segmentGenerator;
    [SerializeField] private OutfitRooms roomOutfitter;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private GameObject room0;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    void Awake()
    {
        publicGenerationManager = this;
    }

    public void GenerateSegment(Vector3 location, GenerationDirection direction)
    {
        segmentGenerator.GenerateSegment(location, direction);
    }

    public void CloseDoors(RoomPrefab prefab)
    {
        List<GenerationDirection> doorDirections = prefab.GetDoorDirections();
        for (int i = 0; i < doorDirections.Count; i++)
        {
            roomOutfitter.DestroyNewDoor(prefab, doorDirections[i]);
        }
    }
    public void ReopenDoors(RoomPrefab prefab)
    {
        List<GenerationDirection> doorDirections = prefab.GetDoorDirections();
        for (int i = 0; i < doorDirections.Count; i++)
        {
            roomOutfitter.CreateNewDoor(prefab, doorDirections[i]);
        }
    }

    public IEnumerator LoadExitRoom(RoomPrefab exitRoom)
    {
        Debug.Log("Loading Next Section!");
        roomOutfitter.DestroyNewDoor(exitRoom, exitRoom.GetExitRoomDir());
        List<GenerationDirection> newDirections = new List<GenerationDirection>() { GenerationDirection.left, GenerationDirection.down, GenerationDirection.right, GenerationDirection.up };
        newDirections.Remove(exitRoom.GetExitRoomDir());
        GenerationDirection newDirection = newDirections[UnityEngine.Random.Range(0, newDirections.Count)];
        EnemyManager.publicEnemyManager.ClearList();
        EnemyManager.publicEnemyManager.IncreaseEnemyDifficulty();
        navMeshSurface.RemoveData();
        segmentGenerator.ClearCurrentRooms();
        yield return new WaitForSeconds(2f);
        segmentGenerator.GenerateSegment(exitRoom.transform.position, newDirection);
        yield return new WaitForEndOfFrame();
        navMeshSurface.BuildNavMesh();
        segmentGenerator.AddToCurrentlyGenerating(exitRoom);
        roomOutfitter.CreateNewDoor(exitRoom, newDirection);
    }

    public static GenerationDirection GetOppositeDirection(GenerationDirection direction)
    {
        if (direction == GenerationDirection.right)
        {
            return GenerationDirection.left;
        }
        else if (direction == GenerationDirection.left)
        {
            return GenerationDirection.right;
        }
        else if (direction == GenerationDirection.up)
        {
            return GenerationDirection.down;
        }
        else if (direction == GenerationDirection.down)
        {
            return GenerationDirection.up;
        }
        else
        {
            return GenerationDirection.nullType;
        }
    }
    public void ChangeTestModePrefab(int testVal)
    {
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
}

public enum GenerationDirection
{
    nullType,
    up,
    right,
    down,
    left
}