using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTester : MonoBehaviour
{
    void Start()
    {
        List<BlockManager.BlockType> bTs = new List<BlockManager.BlockType>();
        bTs.Add(BlockManager.singleton.getBlockType(3));
        GetComponent<Block>().initialise(null, 0, bTs);
    }
}
