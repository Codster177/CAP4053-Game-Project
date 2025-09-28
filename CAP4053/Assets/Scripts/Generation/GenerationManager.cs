using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.Tilemaps;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager publicGenerationManager;
    [SerializeField] private Grid globalGrid;
    [SerializeField] private List<RoomPrefab> roomPrefabs, currentlyGenerating;
    [SerializeField] private float xOffsetConst, yOffsetConst;
    [SerializeField] private GenerationDirection testGenerationDirection;
    [SerializeField] private int squareBounds;
    [SerializeField] private int[] minimumRoomRange;

    void Awake()
    {
        publicGenerationManager = this;
        if (minimumRoomRange.Length != 2)
        {
            Debug.LogError("GenerationManager: minimumRoomRange length is not equal to 2.");
            Destroy(this);
        }
    }

    void Start()
    {
        GenerateSegment(new Vector2(0f, 0f), minimumRoomRange, testGenerationDirection);
    }

    // Segment is a square grid going from (0,0) to (squareBounds, squareBounds).
    // First, determing the connecting room's grid position. For example, if the direction's up, it might be (0,2).
    // When generating, loop from (0,0) to (squareBounds, squareBounds), and to find the room's vector location, use
    // the CalculateVectorPos function to find the room's vector position.  
    public void GenerateSegment(Vector2 startingPosition, int[] minimumRoomRange, GenerationDirection direction)
    {
        // Finds the position of the connecting room to the new segment.
        int[] connectingGridInfo = DetermineLocalOffset(direction);

        // Calculates the offset that the position is multiplied by.
        int[] localOffset = new int[2] { 0 - connectingGridInfo[0], 0 - connectingGridInfo[1] };

        // Debug.
        Debug.Log("Starting Room Grid Position: " + connectingGridInfo[0] + " " + connectingGridInfo[1] + ", Index Val = " + connectingGridInfo[2]);
        Debug.Log("Grid Offset: " + localOffset[0] + " " + localOffset[1]);

        // Randomizes how many rooms will spawn in a segment. 
        int segmentMinimumCount = UnityEngine.Random.Range(minimumRoomRange[0], minimumRoomRange[1] + 1);
        Debug.Log(segmentMinimumCount);

        // Establishes two lists that are meant for the 
        List<int> roomIndexList = new List<int>();
        List<int> spawnedIndexList = new List<int>();

        for (int i = 0; i < (squareBounds * squareBounds); i++)
        {
            roomIndexList.Add(i);
        }

        roomIndexList.Remove(connectingGridInfo[2]);
        spawnedIndexList.Add(connectingGridInfo[2]);


        for (int i = 1; i < segmentMinimumCount; i++)
        {
            int roomIndex = roomIndexList[UnityEngine.Random.Range(0, roomIndexList.Count)];
            roomIndexList.Remove(roomIndex);
            spawnedIndexList.Add(roomIndex);
        }

        for (int i = 0; i < spawnedIndexList.Count; i++)
        {
            int[] gridCoords = ConvertGridIndex(spawnedIndexList[i]);

            Vector2 roomVectorPos = CalculateVectorPos(startingPosition, localOffset, gridCoords);
            RoomPrefab newRoom = GenerateRoom(roomVectorPos, roomPrefabs[1]);
            currentlyGenerating.Add(newRoom);
        }

        ConnectSegment(spawnedIndexList, roomIndexList);
    }
    private RoomPrefab GenerateRoom(Vector2 position, RoomPrefab prefab)
    {
        GameObject newRoom = Instantiate(prefab.roomPrefab, position, Quaternion.identity);
        return newRoom.GetComponent<RoomPrefab>();
    }

    private int[] ConvertGridIndex(int index)
    {
        int[] gridCoords = new int[2];
        gridCoords[0] = index / squareBounds;
        gridCoords[1] = index % squareBounds;
        return gridCoords;
    }
    private int ConvertGridCoords(int[] cords)
    {
        int index = 0;
        index = (cords[0] * squareBounds) + cords[1];
        return index;
    }
    private int[] DetermineLocalOffset(GenerationDirection direction)
    {
        int[] connectingGridPos = new int[3];
        int xGrid, yGrid, indexVal;
        if (direction == GenerationDirection.up)
        {
            xGrid = UnityEngine.Random.Range(0, squareBounds);
            yGrid = -1;
            indexVal = squareBounds * xGrid;
        }
        else if (direction == GenerationDirection.right)
        {
            xGrid = -1;
            yGrid = UnityEngine.Random.Range(0, squareBounds);
            indexVal = yGrid;
        }
        else if (direction == GenerationDirection.down)
        {
            xGrid = UnityEngine.Random.Range(0, squareBounds);
            yGrid = squareBounds;
            indexVal = (squareBounds * xGrid) + (squareBounds - 1);
        }
        else
        {
            xGrid = squareBounds;
            yGrid = UnityEngine.Random.Range(0, squareBounds);
            indexVal = squareBounds * (squareBounds - 1) + yGrid;
        }
        connectingGridPos[0] = xGrid;
        connectingGridPos[1] = yGrid;
        connectingGridPos[2] = indexVal;
        return connectingGridPos;
    }
    private Vector2 CalculateVectorPos(Vector2 startingPosition, int[] localOffset, int[] gridPos)
    {
        return new Vector2(startingPosition.x + (xOffsetConst * (gridPos[0] + localOffset[0])),
        startingPosition.y + (yOffsetConst * (gridPos[1] + localOffset[1])));
    }
    private void ConnectSegment(List<int> spawnedRoomIndices, List<int> notSpawnedIndices)
    {
        List<SpawnNode> nodeList = new List<SpawnNode>();

        for (int i = 0; i < spawnedRoomIndices.Count; i++)
        {
            nodeList.Add(new SpawnNode(spawnedRoomIndices[i], true));
        }
        for (int i = 0; i < notSpawnedIndices.Count; i++)
        {
            nodeList.Add(new SpawnNode(notSpawnedIndices[i], false));
        }
        nodeList.Sort();
        // for (int i = 0; i < nodeList.Count; i++)
        // {
        //     Debug.Log("nodeList[" + i + "]: " + nodeList[i].spawnIndex);
        // }
        List<Edge> edgeList = MakeEdges(nodeList);
        for (int i = 0; i < edgeList.Count; i++)
        {
            int[] node1Coords = ConvertGridIndex(edgeList[i].node1.spawnIndex);
            int[] node2Coords = ConvertGridIndex(edgeList[i].node2.spawnIndex);
            Debug.Log("edgeList[" + i + "]: Node 1 = " + node1Coords[0] + " " + node1Coords[1] + ", Node 2 = " + node2Coords[0] + " " + node2Coords[1] + ", Weight = " + edgeList[i].weight);
        }
    }
    private List<Edge> MakeEdges(List<SpawnNode> nodeList)
    {
        List<Edge> edgeList = new List<Edge>();
        for (int i = 0; i < nodeList.Count; i++)
        {
            int[] gridCoords = ConvertGridIndex(nodeList[i].spawnIndex);
            SpawnNode newNode;
            if (gridCoords[0] != (squareBounds - 1))
            {
                newNode = nodeList[ConvertGridCoords(new int[] { gridCoords[0] + 1, gridCoords[1] })];

                int weight = -1;
                if (nodeList[i].Activated && newNode.Activated)
                {
                    weight = 1;
                }
                else if (nodeList[i].Activated || newNode.Activated)
                {
                    weight = 2;
                }
                else
                {
                    weight = 3;
                }
                edgeList.Add(new Edge(nodeList[i], newNode, weight));
            }
            if (gridCoords[1] != (squareBounds - 1))
            {
                newNode = nodeList[ConvertGridCoords(new int[] { gridCoords[0], gridCoords[1] + 1 })];

                int weight = -1;
                if (nodeList[i].Activated && newNode.Activated)
                {
                    weight = 1;
                }
                else if (nodeList[i].Activated || newNode.Activated)
                {
                    weight = 2;
                }
                else
                {
                    weight = 3;
                }
                edgeList.Add(new Edge(nodeList[i], newNode, weight));
            }
        }
        return edgeList;
    }

    private class SpawnNode : IComparable
    {
        public int spawnIndex;
        public bool Activated;
        public SpawnNode(int index, bool setActivated)
        {
            spawnIndex = index;
            Activated = setActivated;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            SpawnNode otherNode = obj as SpawnNode;
            if (otherNode != null)
            {
                return this.spawnIndex.CompareTo(otherNode.spawnIndex);
            }
            else
            {
                throw new ArgumentException("GenerationManager: SpawnNode: Comparing object to something that is not a SpawnNode.");
            }
        }
    }
    private class Edge
    {
        public SpawnNode node1, node2;
        public int weight;
        public Edge(SpawnNode newNode1, SpawnNode newNode2, int newWeight)
        {
            node1 = newNode1;
            node2 = newNode2;
            weight = newWeight;
        }
    }
}

public enum GenerationDirection
{
    up,
    right,
    down,
    left
}