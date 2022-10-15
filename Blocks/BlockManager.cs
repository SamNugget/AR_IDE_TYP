using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager singleton = null;

    public static GameObject blockFab;
    public GameObject blockPrefab;

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

        [SerializeField] private int lineCount;
        public int getLineCount()
        {
            return lineCount;
        }

        [SerializeField] private int maxLettersPerLine;
        public int getMaxLettersPerLine()
        {
            return maxLettersPerLine;
        }

        public void initialise()
        {
            lineCount = lines.Length;


            int longest = 0;
            foreach (string line in lines)
                if (line.Length > longest) longest = line.Length;
            maxLettersPerLine = longest;
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

        blockFab = blockPrefab;

        foreach (BlockType bT in blockTypes)
            bT.initialise();
    }
}
