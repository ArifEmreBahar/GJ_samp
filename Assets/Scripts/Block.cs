using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private BlockTypes blockType;
    [SerializeField] private Sprite[] blockModes;
    private SpriteRenderer spriteRenderer;
    private Vector3 blockCoord;
    private bool status = false;

    private void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Block(BlockTypes blockType, Sprite[] blockModes)
    {
        this.blockType = blockType;
        this.blockModes = blockModes;
    }

    public IEnumerator MoveBlockToEmptyPlace(Vector3 emptyPlace, float moveSpeed, bool anim)
    {
        if (anim)
        {
            float elapsedTime = 0;
            Vector3 startPos = transform.position;

            while (transform.position != emptyPlace)
            {
                transform.position = Vector3.Lerp(startPos, emptyPlace, (elapsedTime * moveSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            transform.position = emptyPlace;
            yield return null;
        }
    }

    public void SetBlockImage(int imageNo)
    {

        if (imageNo < blockModes.Length)
        {
            spriteRenderer.sprite = blockModes[imageNo];
        }
        else
        {
            Debug.LogWarning("There is no imageNo: Check your blockModes size or Reduce your imageNo.");
        }
    }

    public void SetPosition(Vector3 position)
    {
        blockCoord = position;
    }

    public void SetStatus(bool status)
    {
        this.status = status;
    }

    public bool GetStatus()
    {
        return status;
    }

    public BlockTypes GetBlockType()
    {
        return blockType;
    }

    public Vector3 GetBlockPosition()
    {
        return blockCoord;
    }
}

public enum BlockTypes
{
    Empty,
    Blue,
    Green,
    Pink,
    Purple,
    Red,
    Yellow,
    Null
}