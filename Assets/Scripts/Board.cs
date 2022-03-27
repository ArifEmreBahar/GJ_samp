using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board
{
    private int width;
    private int height;
    private float cellSize;
    private float spawnHeight;

    private List<Vector3> connectedCells = new List<Vector3>();
    private Dictionary<int, Vector3[]> connectedAllBoard = new Dictionary<int, Vector3[]>();

    public BoardCell[,] boardCells;

    //Board Structure
    public Board(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        spawnHeight = height;

        SetUpBoard();
    }
    //Board Structure with spawnHeight
    public Board(int width, int height, float cellSize, float spawnHeight)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.spawnHeight = spawnHeight;

        if (spawnHeight < height)
        {
            spawnHeight = height;
            Debug.LogWarning("Spawn Height can't be smaller than Board Height: Set to Board Height.");
        }

        SetUpBoard();
    }

    //Makes Board ready to use
    private void SetUpBoard()
    {
        boardCells = new BoardCell[width, height];

        for (int x = 0; x < boardCells.GetLength(0); x++)
        {
            for (int y = 0; y < boardCells.GetLength(1); y++)
            {
                boardCells[x, y].position = CoordToCell(new Vector3(x, y));
                boardCells[x, y].blockType = BlockTypes.Empty;
                DrawCellEdges(x, y, Color.red, Mathf.Infinity);
            }
        }
    }

    //Retruns spawn position with looking its current position
    public Vector3 GetSpawnPosition(Vector3 currentPosition)
    {
        return currentPosition + new Vector3(0, spawnHeight * cellSize);
    }

    //Returns all connected cells
    public Dictionary<int,Vector3[]> GetConnectedCells()
    {
        int connectID = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < connectedAllBoard.Count; i++)
                {
                    for (int j = 0; j < connectedAllBoard.Values.Count; j++)
                    {
                        if (!connectedAllBoard.ElementAt(i).Value.Contains(new Vector3(x, y)))
                        {
                            FindConnectedCells(new Vector3(x, y));
                            connectedAllBoard.Add(connectID, connectedCells.ToArray());
                            connectID++;
                            Debug.Log(connectID);
                            connectedCells.Clear();
                        }
                    }
                }  
            }
        }
        return connectedAllBoard;
    }

    //Returns connected cells around click area
    public Vector3[] GetConnectedCells(Vector3 coordinate)
    {
        FindConnectedCells(coordinate);
        Vector3[] connectedCellsTemp = connectedCells.ToArray();
        connectedCells.Clear();
        return connectedCellsTemp;
    }

    //Finds connected cells
    public void FindConnectedCells(Vector3 coordinate)
    {
        if (!InBoardCoord(coordinate)) return;

        connectedCells.Add(coordinate);

        BlockTypes blockType = boardCells[(int)coordinate.x, (int)coordinate.y].blockType;

        BlockTypes upBlockType = (int)coordinate.y < height - 1 ? boardCells[(int)coordinate.x, (int)coordinate.y + 1].blockType : BlockTypes.Null;  
        if (blockType == upBlockType) {
            Vector3 nextCell = new Vector3((int)coordinate.x, (int)coordinate.y + 1);

            if (!connectedCells.Contains(nextCell))
            {
                FindConnectedCells(nextCell);
            }
        }

        BlockTypes rightBlockType = (int)coordinate.x < width - 1 ? boardCells[(int)coordinate.x + 1, (int)coordinate.y].blockType : BlockTypes.Null;
        if (blockType == rightBlockType) {
            Vector3 nextCell = new Vector3((int)coordinate.x + 1, (int)coordinate.y);

            if (!connectedCells.Contains(nextCell))
            {
                FindConnectedCells(nextCell);
            }
        }

        BlockTypes downBlockType = (int)coordinate.y > 0 ? boardCells[(int)coordinate.x, (int)coordinate.y - 1].blockType : BlockTypes.Null;
        if (blockType == downBlockType) {
            Vector3 nextCell = new Vector3((int)coordinate.x, (int)coordinate.y - 1);

            if (!connectedCells.Contains(nextCell))
            {
                FindConnectedCells(nextCell);
            }
        }

        BlockTypes leftBlockType = (int)coordinate.x > 0 ? boardCells[(int)coordinate.x - 1, (int)coordinate.y].blockType : BlockTypes.Null;
        if (blockType == leftBlockType) {
            Vector3 nextCell = new Vector3((int)coordinate.x - 1, (int)coordinate.y);

            if (!connectedCells.Contains(nextCell))
            {
                FindConnectedCells(nextCell);
            }
        }
    }

    //Clears all boards
    public void ClearAllBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                boardCells[x, y].blockType = BlockTypes.Empty;
            }
        }
    }
    
    //Checks are there any movement to play
    public bool IsAllAlone()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!IsAlone(new Vector3(x,y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    //Checks the cell is alone
    public bool IsAlone(Vector3 coordinate)
    {
        if (!InBoardCoord(coordinate)) return false;

        BlockTypes blockType = boardCells[(int)coordinate.x, (int)coordinate.y].blockType;

        BlockTypes upBlockType = (int)coordinate.y < height - 1 ? boardCells[(int)coordinate.x, (int)coordinate.y + 1].blockType : BlockTypes.Null;
        BlockTypes leftBlockType = (int)coordinate.x > 0 ? boardCells[(int)coordinate.x - 1, (int)coordinate.y].blockType : BlockTypes.Null;
        BlockTypes downBlockType = (int)coordinate.y > 0 ? boardCells[(int)coordinate.x, (int)coordinate.y - 1].blockType : BlockTypes.Null;
        BlockTypes rightBlockType = (int)coordinate.x < width - 1 ? boardCells[(int)coordinate.x + 1, (int)coordinate.y].blockType : BlockTypes.Null;

        if (blockType != upBlockType &&
            blockType != leftBlockType &&
            blockType != downBlockType &&
            blockType != rightBlockType)
        {
            return true;
        }
        return false;
    }

    //Sets member type on board
    public void SetBoardMemberType(Vector3 coordinate, BlockTypes blockType)
    {
        if (InBoardCoord(coordinate))
        {
            boardCells[(int)coordinate.x, (int)coordinate.y].blockType = blockType;
        }
    }

    //Makes cell empty
    public void MakeEmpty(Vector3 coordinate)
    {
        if (InBoardCoord(coordinate))
        {
            boardCells[(int)coordinate.x, (int)coordinate.y].blockType = BlockTypes.Empty;
        }
    }

    //Makes multiple cell empty
    public void MakeEmpty(Vector3[] coordinate)
    {
        for (int i = 0; i < coordinate.Length; i++)
        {
            if (InBoardCoord(coordinate[i]))
            {
                boardCells[(int)coordinate[i].x, (int)coordinate[i].y].blockType = BlockTypes.Empty;
                DrawCellEdges((int)coordinate[i].x, (int)coordinate[i].y, Color.blue, 4f);
            }
        }
    }

    //Checks is the coordinate on board
    public bool InBoardCoord(Vector3 coordinate)
    {
        if (coordinate.x < width && coordinate.y < height && coordinate.x >= 0 && coordinate.y >= 0)
        {
            return true;
        }
        else
        {
            Debug.LogWarning("The coordinate you enter is outside of board: Enter a value in the board.");
            return false;
        }
    }

    //Checks is the world position in boards cell area
    public bool InBoardCell(Vector3 worldPosition)
    {
        if (CellToCoord(worldPosition) == new Vector3 (-1,-1,-1))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Changes cell position to coordinate 
    public Vector3 CellToCoord(Vector3 worldPosition)
    {
        Vector3 coord = worldPosition / cellSize;
        
        if (width % 2 == 0 && height % 2 == 0)
        {
            coord = coord - new Vector3(0.5f, 0.5f);
        }
        else if (width % 2 == 0 && height % 2 == 1)
        {
            coord = coord - new Vector3(0.5f, 0);
        }
        else if (width % 2 == 1 && height % 2 == 0)
        {
            coord = coord - new Vector3(0, 0.5f);
        }

        coord = coord + new Vector3(width / 2, height / 2);

        coord = new Vector3(RoundCoordToInt(coord.x), RoundCoordToInt(coord.y));

        if (coord.x >= 0 && coord.y >= 0 && coord.x < width && coord.y < height)
        {
            return new Vector3(RoundCoordToInt(coord.x), RoundCoordToInt(coord.y));
        }
        else
        {
            return new Vector3(-1,-1,-1);
        }
    }

    //Rounds coordinate float to int 
    private int RoundCoordToInt(float coordVal)
    {
        if (coordVal % 1 >= 0.5f)
        {
            if (coordVal >= 0)
            {
                return (int)coordVal + 1;
            }
            else
            {
                return (int)coordVal - 1;
            }
        }
        else
        {
            if (coordVal >= 0)
            {
                return (int)coordVal;
            }
            else
            {
                return (int)coordVal;
            }
        }
    }

    //Changes coordinate to cell position
    public Vector3 CoordToCell(Vector3 coordinate)
    {   
        if (width % 2 == 0 && height % 2 == 0)
        {
            return new Vector3(coordinate.x - width / 2 + 0.5f, coordinate.y - height / 2 + 0.5f) * cellSize;
        }
        else if (width % 2 == 0 && height % 2 == 1)
        {
            return new Vector3(coordinate.x - width / 2 + 0.5f, coordinate.y - height / 2) * cellSize;
           
        }
        else if (width % 2 == 1 && height % 2 == 0)
        {
            return new Vector3(coordinate.x - width / 2, coordinate.y - height / 2 + 0.5f) * cellSize;
        }
        else
        {
            return new Vector3(coordinate.x - width / 2, coordinate.y - height / 2) * cellSize;
        }      
    }

    //Draw Gizmos
    private void DrawCellEdges(int x, int y, Color color, float duration)
    {
        Debug.DrawLine(CoordToCell(new Vector3(x, y)) - new Vector3(cellSize / 2, cellSize / 2), CoordToCell(new Vector3(x, y)) + new Vector3(cellSize / 2, cellSize / 2), color, duration);
        Debug.DrawLine(CoordToCell(new Vector3(x, y)) - new Vector3(cellSize / 2, -cellSize / 2), CoordToCell(new Vector3(x, y)) + new Vector3(cellSize / 2, -cellSize / 2), color, duration);
    }
}

public struct BoardCell
{
    public Vector3 position;
    public BlockTypes blockType;
}


