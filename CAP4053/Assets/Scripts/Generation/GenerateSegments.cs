using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(menuName = "Scriptable Objects/Generation/GenerateSegments")]
public class GenerateSegments : ScriptableObject
{
    [SerializeField] private List<RoomPrefab> currentlyGenerating = new List<RoomPrefab>();
    [SerializeField] private float xOffsetConst = 32, yOffsetConst = 18;
    [SerializeField] private int squareBounds = 5;
    [SerializeField] private int[] minimumRoomRange;
    [SerializeField] private OutfitRooms roomOutfitter;
    private int startRoomIndex;
    void Awake()
    {
        if (minimumRoomRange.Length != 2)
        {
            Debug.LogError("GenerationManager: minimumRoomRange length is not equal to 2.");
            Destroy(this);
        }
    }
    public void SetParameters(float xOffset, float yOffset, int squareBounds, int[] minimumRoomRange)
    {
        this.xOffsetConst = xOffset;
        this.yOffsetConst = yOffset;
        this.squareBounds = squareBounds;
        this.minimumRoomRange = minimumRoomRange;
    }
    public void AddToCurrentlyGenerating(RoomPrefab roomPrefab)
    {
        currentlyGenerating.Add(roomPrefab);
    }
    public void GenerateSegment(Vector2 startingPosition, GenerationDirection direction)
    {
        // Finds the position of the connecting room to the new segment.
        int[] connectingGridInfo = DetermineLocalOffset(direction);

        // Calculates the offset that the position is multiplied by.
        int[] localOffset = new int[2] { 0 - connectingGridInfo[0], 0 - connectingGridInfo[1] };

        startRoomIndex = connectingGridInfo[2];

        // Debug.
        // Debug.Log("Starting Room Grid Position: " + connectingGridInfo[0] + " " + connectingGridInfo[1] + ", Index Val = " + connectingGridInfo[2]);
        // Debug.Log("Grid Offset: " + localOffset[0] + " " + localOffset[1]);

        // Randomizes how many rooms will spawn in a segment. 
        if (minimumRoomRange[1] > (squareBounds * squareBounds))
        {
            Debug.LogError("GenerationManager: GenerateSegment: minimumRoomRange exceeds the bounds set to generate in.");
            return;
        }
        int segmentMinimumCount = UnityEngine.Random.Range(minimumRoomRange[0], minimumRoomRange[1] + 1);

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

        // Debug.Log($"Stopwatch time before ConnectSegment(): {stopwatch.ElapsedMilliseconds} ms.");

        List<SpawnNode> nodeList = ConnectSegment(spawnedIndexList, roomIndexList);

        for (int i = 0; i < spawnedIndexList.Count; i++)
        {
            int[] gridCoords = ConvertGridIndex(spawnedIndexList[i]);

            Vector2 roomVectorPos = CalculateVectorPos(startingPosition, localOffset, gridCoords);
            RoomPrefab newRoom = GenerateRoom(roomVectorPos, roomOutfitter.GetRoomPrefab(0));
            roomOutfitter.OutfitRoom(nodeList[spawnedIndexList[i]], newRoom, startRoomIndex, direction, this);
            nodeList[spawnedIndexList[i]].roomPrefab = newRoom;
            currentlyGenerating.Add(newRoom);
        }
        CreateExitRoom(nodeList, spawnedIndexList, startingPosition, localOffset);
    }
    public void ClearCurrentRooms()
    {
        if (currentlyGenerating == null)
        {
            currentlyGenerating = new List<RoomPrefab>();
            return;
        }
        for (int i = 0; i < currentlyGenerating.Count; i++)
        {
            if (currentlyGenerating[i] == null)
            {
                continue;
            }
            if (currentlyGenerating[i].gameObject != null)
            {
                Destroy(currentlyGenerating[i].gameObject);
            }
        }
        System.GC.Collect();
        currentlyGenerating = new List<RoomPrefab>();
    }
    private RoomPrefab GenerateRoom(Vector2 position, RoomPrefab prefab)
    {
        GameObject newRoom = Instantiate(prefab.roomPrefab, position, Quaternion.identity);
        return newRoom.GetComponent<RoomPrefab>();
    }
    private RoomPrefab GenerateRoomInDirection(Vector2 position, RoomPrefab prefab, GenerationDirection direction)
    {
        Vector2 offset = new Vector2(0f, 0f);
        if (direction == GenerationDirection.left)
        {
            offset.x = -1f;
        }
        else if (direction == GenerationDirection.right)
        {
            offset.x = 1f;
        }
        else if (direction == GenerationDirection.down)
        {
            offset.y = -1f;
        }
        else if (direction == GenerationDirection.up)
        {
            offset.y = 1f;
        }
        Vector2 newPos = new Vector2(position.x + (xOffsetConst * offset.x), position.y + (yOffsetConst * offset.y));
        return GenerateRoom(newPos, prefab);
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
    private List<SpawnNode> ConnectSegment(List<int> spawnedRoomIndices, List<int> notSpawnedIndices)
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
        MakeEdges(nodeList);
        FindPaths(nodeList, spawnedRoomIndices, notSpawnedIndices);
        return nodeList;
    }
    private void MakeEdges(List<SpawnNode> nodeList)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            SpawnNode left, down, right, up;
            left = GetNodeInDirection(nodeList, nodeList[i], GenerationDirection.left);
            down = GetNodeInDirection(nodeList, nodeList[i], GenerationDirection.down);
            right = GetNodeInDirection(nodeList, nodeList[i], GenerationDirection.right);
            up = GetNodeInDirection(nodeList, nodeList[i], GenerationDirection.up);

            if (left != null)
            {
                nodeList[i].SetEdge(left, GenerationDirection.left);
            }
            if (down != null)
            {
                nodeList[i].SetEdge(down, GenerationDirection.down);
            }
            if (right != null)
            {
                nodeList[i].SetEdge(right, GenerationDirection.right);
            }
            if (up != null)
            {
                nodeList[i].SetEdge(up, GenerationDirection.up);
            }
        }
    }
    private void FindPaths(List<SpawnNode> nodeList, List<int> spawnedRoomIndices, List<int> notSpawnedIndices)
    {
        // Debug.Log($"Starting Room Coords: {ConvertGridIndex(startRoomIndex)[0]}, {ConvertGridIndex(startRoomIndex)[1]}");
        int[] startNodeCoords = ConvertGridIndex(startRoomIndex);

        List<int> spawnedCopy = new List<int>(spawnedRoomIndices);
        List<int> randomSpawnedIndicies = new List<int>();
        while (spawnedCopy.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, spawnedCopy.Count);
            randomSpawnedIndicies.Add(spawnedCopy[randomIndex]);
            spawnedCopy.RemoveAt(randomIndex);
        }
        // Debug.Log(spawnedRoomIndices.Count);
        for (int i = 0; i < randomSpawnedIndicies.Count; i++)
        {
            SpawnNode curNode = nodeList[randomSpawnedIndicies[i]];
            SpawnNode travelNode = curNode;

            int[] curCoords = ConvertGridIndex(curNode.spawnIndex);

            // Debug.Log($"Finding Path! Current Node: {curCoords[0]}, {curCoords[1]}");
            while (!curNode.connected)
            {
                int[] travelCoords = ConvertGridIndex(travelNode.spawnIndex);
                // Debug.Log($"| - Travel Node: {travelCoords[0]}, {travelCoords[1]}, Activated = {travelNode.activated}");
                if ((travelNode.spawnIndex == startRoomIndex) || (travelNode.connected))
                {
                    // Debug.Log($"| --- Connected to Spawn Room! ---");
                    curNode.connected = true;
                    break;
                }
                // GenerationDirection[] startDirs = DirectionToCoords(travelNode, ConvertGridIndex(curNode.spawnIndex));
                GenerationDirection[] targetDirs = DirectionToCoords(travelNode, startNodeCoords);
                Edge[] nextEdge = new Edge[2];
                if (targetDirs[0] != GenerationDirection.nullType)
                {
                    nextEdge[0] = travelNode.GetNodesEdge(GetNodeInDirection(nodeList, travelNode, targetDirs[0]));
                }
                if (targetDirs[1] != GenerationDirection.nullType)
                {
                    nextEdge[1] = travelNode.GetNodesEdge(GetNodeInDirection(nodeList, travelNode, targetDirs[1]));
                }

                if ((nextEdge[0] == null) && (nextEdge[1] == null))
                {
                    Debug.LogError("GenerationManager: FindPaths: No directions to follow.");
                    return;
                }
                if (nextEdge[0] == null)
                {
                    travelNode = nextEdge[1].GetOtherNode(travelNode);
                }
                else if (nextEdge[1] == null)
                {
                    travelNode = nextEdge[0].GetOtherNode(travelNode);
                }
                else
                {
                    if (nextEdge[0].weight < nextEdge[1].weight)
                    {
                        travelNode = nextEdge[0].GetOtherNode(travelNode);
                    }
                    else if (nextEdge[0].weight > nextEdge[1].weight)
                    {
                        travelNode = nextEdge[1].GetOtherNode(travelNode);
                    }
                    else
                    {
                        int randomIndex = UnityEngine.Random.Range(0, 2);
                        travelNode = nextEdge[randomIndex].GetOtherNode(travelNode);
                    }
                }
                if (!travelNode.activated)
                {
                    // Debug.Log("Activating Travel Node");
                    travelNode.SetActivated();
                    notSpawnedIndices.Remove(travelNode.spawnIndex);
                    spawnedRoomIndices.Add(travelNode.spawnIndex);
                }
            }
        }
        for (int i = 0; i < spawnedRoomIndices.Count; i++)
        {
            int[] spawningRoomCoords = ConvertGridIndex(spawnedRoomIndices[i]);
            // Debug.Log($"Room to spawn: {spawningRoomCoords[0]}, {spawningRoomCoords[1]}");
        }
    }
    private void CreateExitRoom(List<SpawnNode> nodeList, List<int> spawnedRoomIndices, Vector2 startingPos, int[] localOffset)
    {
        bool roomSpawned = false;
        while (!roomSpawned)
        {
            int exitRoomVal = UnityEngine.Random.Range(0, spawnedRoomIndices.Count);
            SpawnNode node = nodeList[spawnedRoomIndices[exitRoomVal]];
            int[] nodeCoords = ConvertGridIndex(node.spawnIndex);
            List<GenerationDirection> generationDirections = new List<GenerationDirection>() { GenerationDirection.left, GenerationDirection.down, GenerationDirection.right, GenerationDirection.up };
            GenerationDirection direction = GenerationDirection.nullType;
            for (int i = 0; i < node.connectedEdges.Count; i++)
            {
                Edge edge = node.connectedEdges[i];
                Debug.Log(LogEdge(edge));
                if (edge.node1 == node)
                {
                    if (edge.node2.activated)
                    {
                        generationDirections.Remove(edge.GetDirection());
                    }
                }
                else
                {
                    if (edge.node1.activated)
                    {
                        generationDirections.Remove(GenerationManager.GetOppositeDirection(edge.GetDirection()));
                    }
                }
            }
            if (generationDirections.Count == 0)
            {
                continue;
            }
            else
            {
                for (int j = 0; j < generationDirections.Count; j++)
                {
                    Debug.Log($"Generation Directions [{j}]: {generationDirections[j]}");
                }
                int directionVal = UnityEngine.Random.Range(0, generationDirections.Count);
                direction = generationDirections[directionVal];
                Debug.Log($"Chosen Direction: {direction}");
                Vector2 connectingRoomPos = CalculateVectorPos(startingPos, localOffset, nodeCoords);
                Debug.Log($"Connection room position: {connectingRoomPos}");
                RoomPrefab connectingRoom = node.roomPrefab;
                RoomPrefab exitRoom = GenerateRoomInDirection(connectingRoomPos, roomOutfitter.GetRoomPrefab(1), direction);
                exitRoom.SetExitRoomDir(GenerationManager.GetOppositeDirection(direction));
                roomOutfitter.CreateNewDoor(connectingRoom, direction);
                roomOutfitter.CreateNewDoor(exitRoom, GenerationManager.GetOppositeDirection(direction));
                roomSpawned = true;
            }
        }
    }
    private SpawnNode GetNodeByGridCoords(List<SpawnNode> nodeList, int[] coords)
    {
        int gridIndex = ConvertGridCoords(coords);
        try
        {
            return nodeList[gridIndex];
        }
        catch (System.Exception)
        {
            Debug.LogError("GenerationManager: GetNodeByGridCoords: gridIndex out of range.");
            throw;
        }
    }
    private SpawnNode GetNodeInDirection(List<SpawnNode> nodeList, SpawnNode node, GenerationDirection direction)
    {
        int[] nodeCoords = ConvertGridIndex(node.spawnIndex);
        if (direction == GenerationDirection.left && nodeCoords[0] > 0)
        {
            return GetNodeByGridCoords(nodeList, new int[2] { (nodeCoords[0] - 1), nodeCoords[1] });
        }
        else if (direction == GenerationDirection.down && nodeCoords[1] > 0)
        {
            return GetNodeByGridCoords(nodeList, new int[2] { nodeCoords[0], (nodeCoords[1] - 1) });
        }
        else if (direction == GenerationDirection.right && nodeCoords[0] < (squareBounds - 1))
        {
            return GetNodeByGridCoords(nodeList, new int[2] { (nodeCoords[0] + 1), nodeCoords[1] });
        }
        else if (direction == GenerationDirection.up && nodeCoords[1] < (squareBounds - 1))
        {
            return GetNodeByGridCoords(nodeList, new int[2] { nodeCoords[0], (nodeCoords[1] + 1) });
        }
        else
        {
            // Debug.LogError("GenerationManager: GetNodeInDirection: Given direction asks for node outside of squareBounds.");
            return null;
        }
    }
    private GenerationDirection[] DirectionToCoords(SpawnNode node, int[] destCoords)
    {
        GenerationDirection[] returnDirs = new GenerationDirection[2];
        int[] nodeCoords = ConvertGridIndex(node.spawnIndex);
        int xDir = destCoords[0] - nodeCoords[0];
        int yDir = destCoords[1] - nodeCoords[1];
        List<SpawnNode> nextNodeChoices = new List<SpawnNode>();
        if (xDir < 0)
        {
            returnDirs[0] = GenerationDirection.left;
        }
        else if (xDir > 0)
        {
            returnDirs[0] = GenerationDirection.right;
        }
        else
        {
            returnDirs[0] = GenerationDirection.nullType;
        }
        if (yDir < 0)
        {
            returnDirs[1] = GenerationDirection.down;
        }
        else if (yDir > 0)
        {
            returnDirs[1] = GenerationDirection.up;
        }
        else
        {
            returnDirs[1] = GenerationDirection.nullType;
        }
        return returnDirs;
    }

    public class SpawnNode : IComparable
    {
        public int spawnIndex;
        public bool activated, connected;
        public List<Edge> connectedEdges;
        public RoomPrefab roomPrefab;
        public SpawnNode(int index, bool setActivated)
        {
            spawnIndex = index;
            activated = setActivated;
            connectedEdges = new List<Edge>();
        }
        public void SetActivated()
        {
            if (activated)
            {
                return;
            }
            for (int i = 0; i < connectedEdges.Count; i++)
            {
                connectedEdges[i].weight -= 1;
            }
            activated = true;
        }
        public void SetEdge(SpawnNode otherNode, GenerationDirection direction)
        {
            if (this.GetNodesEdge(otherNode) != null)
            {
                return;
            }
            Edge existingEdge = otherNode.GetNodesEdge(this);
            if (existingEdge != null)
            {
                connectedEdges.Add(existingEdge);
            }
            else
            {
                int weight = 3;
                if (this.activated)
                {
                    weight -= 1;
                }
                if (otherNode.activated)
                {
                    weight -= 1;
                }
                connectedEdges.Add(new Edge(this, otherNode, weight, direction));
            }
        }
        public Edge GetNodesEdge(SpawnNode otherNode)
        {
            for (int i = 0; i < connectedEdges.Count; i++)
            {
                if (connectedEdges[i].node1 == otherNode || connectedEdges[i].node2 == otherNode)
                {
                    return connectedEdges[i];
                }
            }
            return null;
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
    public String LogNodeCoords(SpawnNode node)
    {
        int[] coords = ConvertGridIndex(node.spawnIndex);
        return $"({coords[0]}, {coords[1]})";
    }
    // Edge weight rules:
    // Weight = 3: Two non-activated nodes.
    // Weight = 2: One activated node and one unactivated node.
    // Weight = 1: Two activated nodes.
    public class Edge
    {
        public SpawnNode node1, node2;
        public int weight;
        private GenerationDirection direction;
        public Edge(SpawnNode newNode1, SpawnNode newNode2, int newWeight, GenerationDirection direction)
        {
            node1 = newNode1;
            node2 = newNode2;
            weight = newWeight;
            this.direction = direction;
        }
        public SpawnNode GetOtherNode(SpawnNode startNode)
        {
            if (node1 == startNode)
            {
                return node2;
            }
            else
            {
                return node1;
            }
        }
        public GenerationDirection GetDirection()
        {
            return direction;
        }

    }
    public String LogEdge(Edge edge)
    {
        return $"Edge - Node1 = {LogNodeCoords(edge.node1)}, Node 2 = {LogNodeCoords(edge.node2)}, weight = {edge.weight}, direction = {edge.GetDirection()}";
    }
}
