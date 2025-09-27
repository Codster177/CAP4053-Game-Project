using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : MonoBehaviour
{
    public GameObject roomPrefab;
    [SerializeField] private Grid roomGrid;
    [SerializeField] private Tilemap floormap;
    [SerializeField] private Tilemap wallmap;

    public Grid GetGrid()
    {
        return roomGrid;
    }
    public List<Tilemap> GetTilemaps()
    {
        return new List<Tilemap>() { floormap, wallmap };
    }
}
