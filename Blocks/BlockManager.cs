using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager singleton = null;

    public static GameObject blockFab;
    [SerializeField] private GameObject blockPrefab;

    public Material emptyMat;

    [SerializeField] private List<BlockType> blockTypes;
    [System.Serializable]
    public class BlockType
    {
        [SerializeField] private string name;
        public string getName()
        {
            return name;
        }


        // DEFAULT VALUES
        // an array containing the text for each line in the block. '@' marks an input field
        [SerializeField] private string[] lines;
        public string[] getLines()
        {
            string[] ls = new string[lines.Length];
            for (int i = 0; i < ls.Length; i++) ls[i] = lines[i];
            return ls;
        }
        // width and height in letters/lines
        private int width;
        private int height;
        // sub block positions in letters/lines
        // each row = { line, pos } / { y, x }
        private int[,] subBlockPositions;


        // DEFAULT VALUES GET METHODS
        public int getWidth() { return width; }

        public int getHeight() { return height; }

        public int[,] getSubBlockPositions()
        {
            int[,] sBPs = new int[subBlockPositions.GetLength(0), 2];
            for (int i = 0; i < sBPs.GetLength(0); i++)
            {
                sBPs[i, 0] = subBlockPositions[i, 0];
                sBPs[i, 1] = subBlockPositions[i, 1];
            }
            return sBPs;
        }

        public int getSubBlockCount()
        {
            return subBlockPositions.GetLength(0);
        }


        public void calculateInstanceVariables()
        {
            // calculate all the default values for a new block using lines
            width = getMaxLineLength(lines);
            height = lines.Length;

            subBlockPositions = BlockManager.getSubBlockPositions(lines);
        }
    }
    public BlockManager.BlockType getBlockType(int index)
    {
        return blockTypes[index];
    }



    // TODO: on load, get custom block types first (e.g., variables)
    // then spawn the top level block for each window and it will cascade



    // called when spawning and deleting (/spawning empty block) block
    public static void spawnBlock(int blockType, Block toReplace)
    {
        // get info from emptyBlock
        Block parent = toReplace.getParent();
        int subBlockIndex = parent.getSubBlockIndex(toReplace);

        // configure new block
        Block newBlock = Instantiate(blockFab, parent.transform).GetComponent<Block>();
        newBlock.transform.position = toReplace.transform.position;
        newBlock.initialise(blockType);

        // reconfigure parent
        parent.replaceSubBlock(newBlock, subBlockIndex);

        // destroy old block
        Destroy(toReplace.gameObject);

        // draw the blocks
        parent.getWindow2D().drawBlock();
    }



    public static int getMaxLineLength(string[] lines)
    {
        int longest = -1;
        foreach (string line in lines)
        {
            if (line.Length > longest)
                longest = line.Length;
        }

        return longest;
    }

    public static int[,] getSubBlockPositions(string[] lines)
    {
        List<int[]> positions = new List<int[]>();
        for (int i = 0; i < lines.Length; i++)
        {
            int currentPos = lines[i].IndexOf('@');
            while (currentPos >= 0)
            {
                positions.Add(new int[] { i, currentPos });
                currentPos = lines[i].IndexOf('@', currentPos + 1);
            }
        }


        // list -> 2D array
        int[,] subBlockPositions = new int[positions.Count, 2];
        for (int i = 0; i < positions.Count; i++)
        {
            subBlockPositions[i, 0] = positions[i][0];
            subBlockPositions[i, 1] = positions[i][1];
        }

        return subBlockPositions;
    }



    void Awake()
    {
        if (singleton == null) singleton = this;
        else Debug.LogError("Two BlockManager singletons.");

        blockFab = blockPrefab;

        foreach (BlockType bT in blockTypes)
        {
            bT.calculateInstanceVariables();
        }
    }
}
