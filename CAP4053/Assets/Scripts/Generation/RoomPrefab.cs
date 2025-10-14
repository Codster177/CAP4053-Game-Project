using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : MonoBehaviour
{
    public GameObject roomPrefab;
    [SerializeField] private NavMeshModifier[] navMeshModifiers;
    [SerializeField] private Grid roomGrid;
    [SerializeField] private Tilemap floormap;
    [SerializeField] private Tilemap wallmap;
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
    }
    public GenerationDirection GetExitRoomDir()
    {
        return exitRoomDir;
    }
}
