using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager singleton = null;


    public readonly static string ANY = "AY";
    public readonly static string EMPTY = "EY";
    public readonly static string USING = "UG";

    public readonly static string ACCESS_MODIFIER = "AM"; // public, private, etc.
    public readonly static string TYPE = "TP"; // int, string, etc.
    public readonly static string VARIABLE = "VR"; // @TP *name*
    public readonly static string BOOLEAN_EXPRESSION = "BE"; // true, i == 1, etc.
    public readonly static string NAMESPACE = "NS"; // System, UnityEngine, etc.
    public readonly static string BODY = "BY"; // Class, if statements, etc.


    public static GameObject blockFab;
    [SerializeField] private GameObject blockPrefab;


    [SerializeField] private bool safeMode = true;


    [SerializeField] private List<BlockVariant> blockVariants;
    [System.Serializable]
    public class BlockVariant
    {
        [SerializeField] private string name;

        [SerializeField] private string blockType;

        private string[] subBlockTypes;

        [SerializeField] private Color color;


        // DEFAULT VALUES, WILL BE CHANGED IN INSTANCE OF BLOCK
        // an array containing the text for each line in the block. '@BT' marks an input field
        [SerializeField] private string[] lines;

        // width and height in letters/lines
        private int width;
        private int height;
        // sub block positions in letters/lines
        // each row = { line, pos } / { y, x }
        private int[,] subBlockPositions;


        public string getName()
        {
            return name;
        }

        public string getBlockType()
        {
            return blockType;
        }

        public string[] getSubBlockTypes()
        {
            string[] sBTs = new string[subBlockTypes.Length];
            for (int i = 0; i < sBTs.Length; i++) sBTs[i] = subBlockTypes[i];
            return sBTs;
        }

        public Color getColor()
        {
            return new Color(color.r, color.g, color.b, color.a);
        }

        // DEFAULT VALUES GET METHODS
        public string[] getLines()
        {
            string[] ls = new string[lines.Length];
            for (int i = 0; i < ls.Length; i++) ls[i] = lines[i];
            return ls;
        }

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

            // calculate subBlockPositions
            subBlockPositions = BlockManager.getSubBlockPositions(lines);

            // get subBlockTypes
            subBlockTypes = new string[subBlockPositions.GetLength(0)];
            for (int i = 0; i < subBlockPositions.GetLength(0); i++)
            {
                string line = lines[subBlockPositions[i, 0]];
                int pos = subBlockPositions[i, 1];
                string newType;
                try
                {
                    newType = "" + line[pos + 1] + line[pos + 2];
                }
                catch
                {
                    Debug.Log("Incorrect block line format.");
                    newType = "AY";
                }
                subBlockTypes[i] = newType;
            }
        }
    }
    public BlockVariant getBlockVariant(int index)
    {
        if (index < 0 || index >= blockVariants.Count)
        {
            Debug.Log("Block variant index out of range.");
            return null;
        }
        else return blockVariants[index];
    }



    // TODO: on load, get custom block types first (e.g., variables)
    // then spawn the top level block for each window and it will cascade



    // called when spawning and deleting (/spawning empty block) block
    public static void spawnBlock(int blockVariant, Block toReplace)
    {
        // get info from emptyBlock
        Block parent = toReplace.getParent();
        int subBlockIndex = parent.getSubBlockIndex(toReplace);

        // if replacing an empty block, check it is correct block type for parent
        if (singleton.safeMode && toReplace.getBlockVariant().getBlockType().Equals(EMPTY))
        {
            string newBlockType = singleton.getBlockVariant(blockVariant).getBlockType();
            string[] sBTs = parent.getBlockVariant().getSubBlockTypes();

            if (sBTs[subBlockIndex] != ANY && sBTs[subBlockIndex] != newBlockType)
            {
                Debug.Log("This is not the correct block type for this area.");
                return;
            }
        }

        // configure new block
        Block newBlock = Instantiate(blockFab, parent.transform).GetComponent<Block>();
        newBlock.transform.position = toReplace.transform.position;
        newBlock.initialise(blockVariant);

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

        foreach (BlockVariant bT in blockVariants)
        {
            bT.calculateInstanceVariables();
        }
    }
}
