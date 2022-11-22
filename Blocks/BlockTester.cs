using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTester : MonoBehaviour
{
    public int blockType;
    public int[] subBlockTypes;

    void Start()
    {
        GetComponent<Block>().initialise(blockType, subBlockTypes);
        GetComponent<Block>().drawBlock();
    }
}
