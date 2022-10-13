using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager singleton = null;

    [SerializeField] private List<BlockType> blockTypes;
    [System.Serializable]
    public class BlockType
    {
        [SerializeField] private string name;
        public string getName()
        {
            return name;
        }

        // an array containing the text for each line in the block. "{}" marks an input field
        [SerializeField] private string[] lines;
        public string[] getLines()
        {
            return lines;
        }
    }
    public BlockType getBlockType(int b)
    {
        return blockTypes[b];
    }

    void Awake()
    {
        if (singleton == null) singleton = this;
        else Debug.LogError("Two BlockManager singletons.");
    }
}
