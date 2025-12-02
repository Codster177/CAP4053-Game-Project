using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/Generation/OutfitRooms")]
public class OutfitRooms : ScriptableObject
{
    [SerializeField] private List<RoomList> roomLists = new List<RoomList>() { new RoomList("Forest Rooms"), new RoomList("Spawn Rooms") };
    [SerializeField] private List<Tile> floorTiles, wallTiles;

    public void OutfitRoom(GenerateSegments.SpawnNode spawnNode, RoomPrefab prefab, int startRoomIndex, GenerationDirection startDir, GenerateSegments debug)
    {
        Tilemap[] tileMaps = prefab.GetTilemaps();
        if (spawnNode.spawnIndex == startRoomIndex)
        {

            CreateDoor(tileMaps, GenerationManager.GetOppositeDirection(startDir));
            prefab.AddDoorToList(GenerationManager.GetOppositeDirection(startDir));
        }

        for (int i = 0; i < spawnNode.connectedEdges.Count; i++)
        {
            GenerateSegments.Edge curEdge = spawnNode.connectedEdges[i];
            if (!(curEdge.node1.activated && curEdge.node2.activated))
            {
                continue;
            }

            // Debug.Log(debug.LogEdge(curEdge));

            GenerationDirection direction = curEdge.GetDirection();
            if (spawnNode.connectedEdges[i].node2 == spawnNode)
            {
                direction = GenerationManager.GetOppositeDirection(direction);
            }
            // Debug.Log($"Direction = {direction}");
            prefab.AddDoorToList(direction);
            CreateDoor(tileMaps, direction);
        }
    }
    public void CreateNewDoor(RoomPrefab prefab, GenerationDirection direction)
    {
        prefab.AddDoorToList(direction);
        CreateDoor(prefab.GetTilemaps(), direction);
    }
    public void DestroyNewDoor(RoomPrefab prefab, GenerationDirection direction)
    {
        DestroyDoor(prefab.GetTilemaps(), direction);
    }
    private void CreateDoor(Tilemap[] tileMaps, GenerationDirection direction)
    {
        if (direction == GenerationDirection.left)
        {
            tileMaps[1].SetTile(new Vector3Int(-16, -1), null);
            tileMaps[1].SetTile(new Vector3Int(-16, 0), null);
            tileMaps[0].SetTile(new Vector3Int(-16, -1), floorTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(-16, 0), floorTiles[0]);
        }
        if (direction == GenerationDirection.down)
        {
            tileMaps[1].SetTile(new Vector3Int(-1, -9), null);
            tileMaps[1].SetTile(new Vector3Int(0, -9), null);
            tileMaps[0].SetTile(new Vector3Int(-1, -9), floorTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(0, -9), floorTiles[0]);
        }
        if (direction == GenerationDirection.right)
        {
            tileMaps[1].SetTile(new Vector3Int(15, -1), null);
            tileMaps[1].SetTile(new Vector3Int(15, 0), null);
            tileMaps[0].SetTile(new Vector3Int(15, -1), floorTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(15, 0), floorTiles[0]);
        }
        if (direction == GenerationDirection.up)
        {
            tileMaps[1].SetTile(new Vector3Int(-1, 8), null);
            tileMaps[1].SetTile(new Vector3Int(0, 8), null);
            tileMaps[0].SetTile(new Vector3Int(-1, 8), floorTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(0, 8), floorTiles[0]);
        }
    }
    private void DestroyDoor(Tilemap[] tileMaps, GenerationDirection direction)
    {
        if (direction == GenerationDirection.left)
        {
            tileMaps[1].SetTile(new Vector3Int(-16, -1), wallTiles[0]);
            tileMaps[1].SetTile(new Vector3Int(-16, 0), wallTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(-16, -1), null);
            tileMaps[0].SetTile(new Vector3Int(-16, 0), null);
        }
        if (direction == GenerationDirection.down)
        {
            tileMaps[1].SetTile(new Vector3Int(-1, -9), wallTiles[0]);
            tileMaps[1].SetTile(new Vector3Int(0, -9), wallTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(-1, -9), null);
            tileMaps[0].SetTile(new Vector3Int(0, -9), null);
        }
        if (direction == GenerationDirection.right)
        {
            tileMaps[1].SetTile(new Vector3Int(15, -1), wallTiles[0]);
            tileMaps[1].SetTile(new Vector3Int(15, 0), wallTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(15, -1), null);
            tileMaps[0].SetTile(new Vector3Int(15, 0), null);
        }
        if (direction == GenerationDirection.up)
        {
            tileMaps[1].SetTile(new Vector3Int(-1, 8), wallTiles[0]);
            tileMaps[1].SetTile(new Vector3Int(0, 8), wallTiles[0]);
            tileMaps[0].SetTile(new Vector3Int(-1, 8), null);
            tileMaps[0].SetTile(new Vector3Int(0, 8), null);
        }
    }

    // Door Locations:
    // Left: (-16, -1) (-16, 0)
    // Down: (-1, -9) (0, -9)
    // Right: (15, -1) (15, 0)
    // Up: (-1, 8) (0, 8)
    public RoomPrefab GetRoomPrefab(string listName)
    {
        for (int i = 0; i < roomLists.Count; i++)
        {
            if (roomLists[i].listName == listName)
            {
                return roomLists[i].GetRandomPrefab();
            }
        }
        return null;
    }

    [Serializable]
    private class RoomList
    {
        public string listName;
        public List<RoomPrefab> roomPrefabs;

        public RoomList(string listName)
        {
            this.listName = listName;
        }
        public RoomPrefab GetRandomPrefab()
        {
            int randomIndex = UnityEngine.Random.Range(0, roomPrefabs.Count);
            return roomPrefabs[randomIndex];
        }
    }
}
