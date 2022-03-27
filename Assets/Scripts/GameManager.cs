using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float boardCellSize = 2.5f;
    private float plusSpawnHeight = 3f;
    private float blockMovementSpeed = 3f;
    private int[] imageRange = { 5, 8, 10 };
    private bool isAnim = true;
    [SerializeField] [Range(2, 10)] private int boardWidth;
    [SerializeField] [Range(2, 10)] private int boardHeight;
    [SerializeField] [Range(1, 6)] private int colorVariant;
    [SerializeField] private List<Block> blockPrefabs;

    private Board board;

    private List<Block> blockList;

    private void Start()
    {
        InputManager.OnClicked += OnClickedBlock;
    }

    public void StartGame(int boardWidth, int boardHeight, int colorVariant)
    {
        this.boardWidth = boardWidth;
        this.boardHeight = boardHeight;
        this.colorVariant = colorVariant;

        board = new Board(boardWidth, boardHeight, boardCellSize, boardHeight + plusSpawnHeight);
        blockList = new List<Block>();

        CreateBlocks();
        ShuffleBlockList();
        FillEmptyPlaces();
        NoMoveCheck();
    }

    private void OnClickedBlock(Vector3 clickPosition)
    {
        Vector3 coord = board.CellToCoord(clickPosition);

        if (coord != -Vector3.one)
        {
            OnClickedCell(coord);
        }
    }

    private void OnClickedCell(Vector3 coordinate)
    {
        if (!board.IsAlone(coordinate))
        {
            Vector3[] connectedCells = board.GetConnectedCells(coordinate);

            board.MakeEmpty(connectedCells);

            DestroyBlocks(connectedCells);

            RollDownToEmptyCells(connectedCells);

            FillEmptyPlaces();

            NoMoveCheck();

            SetAllBlockImages();
        }
    }

    private void RollDownToEmptyCells(Vector3[] cellsToFill)
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {   
                Block foundBlock = FindBlock(new Vector3(x, y));

                if (foundBlock != null)
                {                                 
                    int emptyBelow = 0;
                    for (int e = y; e >= 0; e--)
                    {
                        if (e - 1 >= 0)
                        {
                            if (board.boardCells[x, e - 1].blockType == BlockTypes.Empty)
                            {
                                emptyBelow++;
                            }
                        }
                    }
                    if (emptyBelow != 0)
                    {    
                        board.boardCells[x, y].blockType = BlockTypes.Empty;//şimdilik
                        board.SetBoardMemberType(new Vector3(x, y - emptyBelow), foundBlock.GetBlockType());
                        SetStatusToBlock(foundBlock, true);
                        foundBlock.SetPosition(new Vector3(x, y - emptyBelow));
                        StartCoroutine(foundBlock.MoveBlockToEmptyPlace(board.boardCells[x, y - emptyBelow].position, blockMovementSpeed * 2, isAnim));
                    }                   
                }
            }
        }
    }

    private void SetAllBlockImages()
    {
        Dictionary<int, Vector3[]> connecteds = board.GetConnectedCells();

        foreach (var item in connecteds)
        {   
            SetBlockImages(item.Value);
        }
    }

    private void SetBlockImages(Vector3[] coordinates)
    {
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (coordinates.Length < imageRange[0])
            {
                FindBlock(coordinates[i]).SetBlockImage(0);
            }
            else if (coordinates.Length >= imageRange[0] && coordinates.Length < imageRange[1])
            {
                FindBlock(coordinates[i]).SetBlockImage(1);
            }
            else if (coordinates.Length >= imageRange[1] && coordinates.Length < imageRange[2])
            {
                FindBlock(coordinates[i]).SetBlockImage(2);
            }
            else if(coordinates.Length >= imageRange[2])
            {
                FindBlock(coordinates[i]).SetBlockImage(3);
            }
        }
    }

    private void NoMoveCheck()
    {
        while (board.IsAllAlone())
        {
            ShuffleBlockList();
            board.ClearAllBoard();
            DisableAllBlocks();
            FillEmptyPlaces();
        }
    }

    private void DisableAllBlocks()
    {
        foreach (Block block in blockList)
        {
            block.SetPosition(-Vector3.one);
            SetStatusToBlock(block, false);
        }
    }

    private Block FindBlock(Vector3 coord)
    {
        foreach (Block block in blockList)
        {
            if (block.GetStatus())
            {
                if (block.GetBlockPosition() == coord)
                {
                return block;
                }
            }
        }
        return null;
    }

    private void DestroyBlocks(Vector3[] destroyPositions)
    {
        for (int i = 0; i < destroyPositions.Length; i++)
        {
            Block block = FindBlock(destroyPositions[i]);
            block.SetPosition(-Vector3.one);
            block.transform.position = -Vector3.one;
            SetStatusToBlock(block, false);
        }
    }

    private void CreateBlocks()
    {   
        for (int i = 0; i < boardWidth * boardHeight * 2; i++)
        {
            Block instetiatedBlock = Instantiate(blockPrefabs[i % colorVariant], transform);
            instetiatedBlock.transform.position = -Vector3.one;
            SetStatusToBlock(instetiatedBlock, false);
            blockList.Add(instetiatedBlock);
        }
    }

    private void FillEmptyPlaces()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight ; y++)
            {
                if (board.boardCells[x, y].blockType == BlockTypes.Empty)
                {
                    Block nextBlock = GetNextBlock();
                    board.SetBoardMemberType(new Vector3(x, y), nextBlock.GetBlockType());
                    nextBlock.transform.position = board.GetSpawnPosition(board.boardCells[x, y].position);
                    SetStatusToBlock(nextBlock, true);
                    nextBlock.SetPosition(new Vector3(x, y));
                    StartCoroutine(nextBlock.MoveBlockToEmptyPlace(board.boardCells[x, y].position, blockMovementSpeed, isAnim));
                }
            }
        }
    }

    public void SetAnim(bool anim)
    {
        isAnim = anim;
        Debug.Log(anim);
    }

    private void SetStatusToBlock(Block block, bool status)
    {
        block.gameObject.SetActive(status);
        block.SetStatus(status);
    }

    private Block GetNextBlock()
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            Block blockToReturn = blockList[i];

            if (blockToReturn.GetStatus())
            {
                blockList.Remove(blockToReturn);
                blockList.Add(blockToReturn);
            }
            else
            {
                return blockToReturn;
            }
        }
        Debug.LogWarning("All Block's status is false: There is something wrong!");
        return null;
    }

    private void ShuffleBlockList()
    {
        for (int i = 0; i < blockList.Count - 1; i++)
        {
            Block temp = blockList[i];
            int randomIndex = Random.Range(i, blockList.Count);
            blockList[i] = blockList[randomIndex];
            blockList[randomIndex] = temp;
        }
    }
}
