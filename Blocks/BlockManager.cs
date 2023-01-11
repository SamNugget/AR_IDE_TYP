using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager singleton = null;


    public static Block lastMaster;
    public static Window3D getLastEditWindow()
    {
        return lastMaster.GetComponentInParent<Window3D>();
    }


    // special
    public readonly static string EMPTY = "EY";
    public readonly static string ANY = "AY";
    public readonly static string SPLITTER = "SR";
    public readonly static string INSERT_LINE = "IL";
    public readonly static string PLACE_FIELD = "PF";
    public readonly static string PLACE_METHOD = "PM";

    public readonly static string USING = "UG";

    // struct, class, interface, enum, and record constructs
    public readonly static string CONSTRUCT = "CT";
    public readonly static string CONSTRUCT_BODY = "CB"; // field or method
    public readonly static string FIELD = "FD"; // @AM @TP @NM {}
    public readonly static string METHOD = "MD"; // @AM @TP @NM() {}

    // namespace, class name, method name or variable name
    public readonly static string NEW_NAME = "NN"; // can be copied but not deleted
    public readonly static string NAME = "NM";
    public readonly static string VARIABLE_NAME = "VN";

    // for fields and methods
    public readonly static string ACCESS_MODIFIER = "AM"; // public, private, etc.
    public readonly static string TYPE = "TP"; // int, string, etc.

    // for inside methods and constructors
    public readonly static string BODY = "BY"; // if statements, variable declaration, etc.
    public readonly static string VARIABLE_DECLARATION = "VD"; // @TP @VN

    // for if statements, while loops
    public readonly static string BOOLEAN_EXPRESSION = "BE"; // true, i == 1, etc.
    public readonly static string TRUE_FALSE = "TF";

    // TODO: namespaces, properties, delegates, and events

    private static readonly string[] cycleable = {
        CONSTRUCT, ACCESS_MODIFIER, TRUE_FALSE
    };
    public static bool isCycleable(string blockType)
    {
        for (int i = 0; i < cycleable.Length; i++)
            if (blockType == cycleable[i]) return true;
        return false;
    }
    public static int cycleBlockVariantIndex(BlockVariant variant)
    {
        string blockType = variant.getBlockType();

        // try get next ahead in list
        for (int i = getBlockVariantIndex(variant) + 1; i < singleton.blockVariants.Count; i++)
            if (singleton.blockVariants[i].getBlockType() == blockType) return i;

        // try get first from beginning of list
        return getFirstVariantOfType(blockType);
    }
    public static int getFirstVariantOfType(string blockType)
    {
        for (int i = 0; i < singleton.blockVariants.Count; i++)
            if (singleton.blockVariants[i].getBlockType() == blockType) return i;
        return -1;
    }



    public static GameObject blockFab;
    [SerializeField] private GameObject blockPrefab;


    //[SerializeField] private bool safeMode = true;


    [SerializeField] private List<BlockVariant> blockVariants;
    [System.Serializable]
    public class BlockVariant
    {
        [SerializeField] private string name;

        [SerializeField] private string blockType;

        private string[] subBlockTypes;

        [SerializeField] private Color color;

        [SerializeField] private bool splittable;


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

        public bool getSplittable() { return splittable; }

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



        public BlockVariant(string name, string blockType, Color color, bool splittable, string[] lines)
        {
            this.name = name;
            this.blockType = blockType;
            this.color = color;
            this.splittable = splittable;
            this.lines = lines;

            calculateInstanceVariables();
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
    public static BlockVariant getBlockVariant(int index)
    {
        if (index < 0 || index >= singleton.blockVariants.Count)
        {
            Debug.Log("Block variant index out of range.");
            return null;
        }
        else return singleton.blockVariants[index];
    }
    public static BlockVariant getBlockVariant(string name)
    {
        foreach (BlockVariant bV in singleton.blockVariants)
            if (bV.getName().Equals(name)) return bV;
        return null;
    }
    public static int getBlockVariantIndex(string name)
    {
        BlockVariant bV = getBlockVariant(name);
        return getBlockVariantIndex(bV);
    }
    public static int getBlockVariantIndex(BlockVariant bV)
    {
        return singleton.blockVariants.IndexOf(bV);
    }
    public static int getNoOfBlockVariants()
    {
        return singleton.blockVariants.Count;
    }



    // TODO: on load, get custom block types first (e.g., variables)
    // then spawn the top level block for each window and it will cascade



    // called when spawning and deleting (/spawning empty block) block
    public static void spawnBlock(int blockVariant, Block toReplace, bool emptyOnly = true)
    {
        // get info from emptyBlock
        Block parent = toReplace.getParent();
        int subBlockIndex = parent.getSubBlockIndex(toReplace);



        bool replacingEmpty = toReplace.getBlockVariant().getBlockType().Equals(EMPTY);

        if (emptyOnly == true && !replacingEmpty) return;

        /*if (singleton.safeMode && replacingEmpty)
        {
            // if replacing an empty block, check it is correct block type for parent
            BlockVariant parentVariant = parent.getBlockVariant();
            int index = subBlockIndex;

            // splitters should take restrictions of parent
            if (parentVariant == getBlockVariant("Splitter"))
            {
                Block highestSplitter = getChildOfNonSplitter(parent);

                parentVariant = highestSplitter.getParent().getBlockVariant();
                index = highestSplitter.getParent().getSubBlockIndex(highestSplitter);
            }



            BlockVariant bV = getBlockVariant(blockVariant);
            string newBlockType = bV.getBlockType();
            string[] sBTs = parentVariant.getSubBlockTypes();
            string expectedType = sBTs[index];

            if (expectedType == newBlockType) ;
            else if (expectedType == CONSTRUCT_BODY)
            {
                if (newBlockType != FIELD && newBlockType != METHOD)
                {
                    Debug.Log("Only fields, methods and constructors may go in the construct body.");
                    return;
                }
            }
            else if (expectedType == BODY)
            {
                if (newBlockType != VARIABLE_DECLARATION)
                {
                    Debug.Log("That block can't go here.");
                    return;
                }
            }
            else if (expectedType == NEW_NAME && (newBlockType == NAME || newBlockType == VARIABLE_NAME)) ;
            else if (newBlockType == VARIABLE_NAME && expectedType == BOOLEAN_EXPRESSION) ;
            else if (newBlockType == NAME && expectedType == TYPE) ;
            else if (expectedType != ANY)
            {
                Debug.Log(expectedType + " block expected, not " + newBlockType + ".");
                return;
            }



            if (newBlockType == VARIABLE_NAME)
            {
                // find variable instance in window
                Variable variable = ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).getVariable(bV.getName());
                if (variable == null)
                {
                    Debug.Log("Err: variable not saved.");
                    // drop out of if and place anyways
                }
                // if not declared
                else if (variable.declarationBlock == null)
                {
                    variable.declarationBlock = parent.getParent(); // declare it!
                }
                // declared, check if in scope of declaration (walk up tree)
                else
                {
                    bool inScope = false;
                    for (Block p = parent; p != null; p = p.getParent())
                        if (p == variable.declarationBlock) { inScope = true; break; }

                    if (inScope == false)
                    {
                        Debug.Log("Must be placed in scope.");
                        return;
                    }
                    Debug.Log("This is in scope.");
                }
            }
        }*/

        // configure new block
        Block newBlock = Instantiate(blockFab, parent.transform).GetComponent<Block>();
        newBlock.transform.position = toReplace.transform.position;
        newBlock.initialise(blockVariant);

        // reconfigure parent
        parent.replaceSubBlock(newBlock, subBlockIndex);

        // draw the blocks
        lastMaster = parent.getMasterBlock();
        if (lastMaster != null) lastMaster.drawBlock();
    }

    public static void splitBlock(Block toSplit, bool originalOnTop = true)
    {
        // get info from toSplit
        Block parent = toSplit.getParent();
        if (parent == null)
        {
            Debug.Log("Can't split the master block.");
            return;
        }
        int subBlockIndex = parent.getSubBlockIndex(toSplit);

        // make splitter
        Block splitter = Instantiate(blockFab, parent.transform).GetComponent<Block>();
        splitter.transform.position = toSplit.transform.position;
        splitter.initialise(getBlockVariantIndex("Splitter"));

        // reconfigure parent
        parent.replaceSubBlock(splitter, subBlockIndex, false);

        // move toSplit into splitter
        toSplit.transform.parent = splitter.transform;
        toSplit.transform.localPosition += blockFab.transform.position;
        splitter.replaceSubBlock(toSplit, originalOnTop ? 0 : 1);

        // draw the blocks
        lastMaster = parent.getMasterBlock();
        if (lastMaster != null) lastMaster.drawBlock();
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



    private static Block getNonSplitterParent(Block b)
    {
        Block highestSplitter = b;
        for (int i = 0; highestSplitter != null; i++)
        {
            Block newParent = highestSplitter.getParent();
            if (newParent.getBlockVariant() != getBlockVariant("Splitter")) return highestSplitter;

            highestSplitter = newParent;
        }

        return null;
    }



    [SerializeField] private Color variableColor;
    public static BlockVariant createNameBlock(string name, bool variable)
    {
        string[] lines = new string[] { name };
        BlockVariant newVariable = new BlockVariant(name, (variable ? VARIABLE_NAME : NAME), singleton.variableColor, false, lines);
        singleton.blockVariants.Add(newVariable);

        return newVariable;
    }



    [SerializeField] private GameObject blockButtonFab;
    [SerializeField] private float fabWidth;
    private static Transform spawnedBlockButton;
    public static void moveBlockButton(Block focused)
    {
        if (spawnedBlockButton == null)
            spawnedBlockButton = Instantiate(singleton.blockButtonFab).transform;

        // position
        spawnedBlockButton.parent = focused.transform;
        spawnedBlockButton.localPosition = Vector3.zero;
        spawnedBlockButton.localRotation = Quaternion.identity;
        
        // scale
        int w = focused.getWidth();
        int h = focused.getHeight();
        Vector3 scale = FontManager.lettersAndLinesToVector(w, h);
        scale *= (1f / singleton.fabWidth);

        scale.z = spawnedBlockButton.localScale.z;

        spawnedBlockButton.localScale = scale;
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
