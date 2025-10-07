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
    public Tilemap[] GetTilemaps()
    {
        return new Tilemap[2] { floormap, wallmap };
    }
}
