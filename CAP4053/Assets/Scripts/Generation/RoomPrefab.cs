using System;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : MonoBehaviour
{
    public GameObject roomPrefab;
    [SerializeField] private NavMeshModifier[] navMeshModifiers;
    [SerializeField] private Grid roomGrid;
    [SerializeField] private Tilemap floormap;
    [SerializeField] private Tilemap doormap;
    [SerializeField] private Tilemap wallmap;
    [SerializeField] private List<DirectionTrashList> removeObjects = new List<DirectionTrashList>() { new DirectionTrashList(GenerationDirection.up), new DirectionTrashList(GenerationDirection.down), new DirectionTrashList(GenerationDirection.right), new DirectionTrashList(GenerationDirection.left) };
    private List<GenerationDirection> doorDirections = new List<GenerationDirection>();
    private GenerationDirection exitRoomDir = GenerationDirection.nullType;

    public Grid GetGrid()
    {
        return roomGrid;
    }
    public Tilemap[] GetTilemaps()
    {
        return new Tilemap[2] { floormap, wallmap };
    }
    public NavMeshModifier[] GetNavMeshModifiers()
    {
        return navMeshModifiers;
    }
    public void SetExitRoomDir(GenerationDirection direction)
    {
        exitRoomDir = direction;
        OpenOrCloseObjects(direction, false);
    }
    public GenerationDirection GetExitRoomDir()
    {
        return exitRoomDir;
    }
    public void AddDoorToList(GenerationDirection direction)
    {
        if (!doorDirections.Contains(direction))
        {
            doorDirections.Add(direction);
            OpenOrCloseObjects(direction, false);
        }
    }
    public List<GenerationDirection> GetDoorDirections()
    {
        return doorDirections;
    }
    public void OpenOrCloseObjects(GenerationDirection directionToOpen, bool enabled)
    {
        for (int i = 0; i < removeObjects.Count; i++)
        {
            if (removeObjects[i].direction == directionToOpen)
            {
                removeObjects[i].EnableObjects(enabled);
            }
        }
    }
    public void OpenOrCloseObjects(bool enabled)
    {
        for (int i = 0; i < doorDirections.Count; i++)
        {
            for (int j = 0; j < removeObjects.Count; j++)
            {
                if (removeObjects[j].direction == doorDirections[i])
                {
                    removeObjects[j].EnableObjects(enabled);
                }
            }
        }
    }
    [Serializable]
    private class DirectionTrashList
    {
        public GenerationDirection direction;
        public List<GameObject> deletables;
        public DirectionTrashList(GenerationDirection direction)
        {
            this.direction = direction;
        }
        public void EnableObjects(bool enabled)
        {
            for (int i = 0; i < deletables.Count; i++)
            {
                deletables[i].SetActive(enabled);
            }
        }
    }
}
