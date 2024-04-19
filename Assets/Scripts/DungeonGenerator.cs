using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;
using Unity.AI.Navigation; //"Editor" not "Engine"

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2 SpecialRadiusMin;
        public Vector2 SpecialRadiusMax; // defines the square radius in which special rooms CANNOT spawn. If both values are the same, this is ignored.
        public float percentageSpawn;
        public bool SpecialRoom; // true means there can only be one, but there MUST be one of these rooms

        public int ProbabilityOfSpawning(int x, int y, bool z)
        {
            if (SpecialRoom){
                if (z) return 0; // cannot spawn anymore, there already is one
                else if ((x >= SpecialRadiusMin.x && y >= SpecialRadiusMin.y) && (x <= SpecialRadiusMax.x && y <= SpecialRadiusMax.y)){
                    if ((SpecialRadiusMin.x == 0) && (SpecialRadiusMin.y == 0) && (SpecialRadiusMax.x == 0) && (SpecialRadiusMax.y == 0)) return 2; // if they are all zero then this must spawn at the beginning.
                    else if ((SpecialRadiusMin.x == SpecialRadiusMax.x) && (SpecialRadiusMin.y == SpecialRadiusMax.y)) return 1; // if the values are the same this is ignored
                    else return 0; // cannot spawn inside the radius.
                }
                else return 2; // must spawn at least one
            }
            
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn
            else{
                if (percentageSpawn >= 100.0f){
                    return 2;
                }
                int chance = (int)UnityEngine.Random.Range(0.0f, 100.0f);
                if (chance <= percentageSpawn){
                    return 1;
                }
                else{
                    return 0;
                }
            }
        }

    }
    public Transform Player;
    public NewEnemy agent; // boss
    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    private List<GameObject[,]> allRooms = new List<GameObject[,]>();
    private List<bool> isRoomSpecial = new List<bool>(), SpecialBuilt = new List<bool>(); // keeps track of which rooms are special, then which special rooms have been built.
    public Vector2 offset;

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshSurface nm = FindObjectOfType<NavMeshSurface>();
        for (int i = 0; i < rooms.Length; i++){
            if (rooms[i].SpecialRoom){
                isRoomSpecial.Add(true);
            }
            else{
                isRoomSpecial.Add(false);
            }
            SpecialBuilt.Add(false);
        }
        MazeGenerator();
        nm.BuildNavMesh();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j, SpecialBuilt[k]);

                        if(p == 2)
                        {
                            randomRoom = k;
                            if (isRoomSpecial[randomRoom]){
                                SpecialBuilt[randomRoom] = true;
                            }
                            break;
                        } else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                        else if ((i == j) && (j == 0)){
                            randomRoom = 0;
                        }
                    }

                    if(randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];
                            if (isRoomSpecial[randomRoom]){
                                SpecialBuilt[randomRoom] = true;
                            }
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.Player = Player;
                    newRoom.agent = agent;
                    if (randomRoom == 2){ // specifically the boss room, you can choose to change this if you wish for more flexibility.
                        newRoom.isEnemyRoom = true;
                    }
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }

    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k<1000)
        {
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
        
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
