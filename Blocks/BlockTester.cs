using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTester : MonoBehaviour
{
    void Start()
    {
        List<BlockManager.BlockType> bTs = new List<BlockManager.BlockType>();
        bTs.Add(BlockManager.singleton.getBlockType(2));
        bTs.Add(BlockManager.singleton.getBlockType(2));
        GetComponent<Block>().initialise(BlockManager.singleton.getBlockType(2), 0, bTs);
    }
}
