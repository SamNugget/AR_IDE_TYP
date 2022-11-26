using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    private BlockManager.BlockVariant blockVariant;
    public BlockManager.BlockVariant getBlockVariant()
    {
        return blockVariant;
    }
    private List<Block> subBlocks = null; // TODO: make private

    private int width;
    public int getWidth() { return width; }
    private int height;
    public int getHeight() { return height; }

    [SerializeField] private TextMeshPro textBox;



    public void initialise(int blockVariant, int[] subBlockVariants = null)
    {
        this.blockVariant = BlockManager.getBlockVariant(blockVariant);
        this.subBlocks = new List<Block>();



        if (subBlockVariants == null)
        {
            string[] subBlockTypes = this.blockVariant.getSubBlockTypes();
            subBlockVariants = new int[subBlockTypes.Length];
            for (int i = 0; i < subBlockTypes.Length; i++)
            {
                if (subBlockTypes[i].Equals(BlockManager.ACCESS_MODIFIER))
                {
                    BlockManager.BlockVariant bV = BlockManager.getBlockVariant("Public");
                    int bVI = BlockManager.getBlockVariantIndex(bV);
                    subBlockVariants[i] = bVI; // special AM block
                }
                else
                {
                    subBlockVariants[i] = 0; // empty block
                }
            }
        }
        else if (subBlockVariants.Length != this.blockVariant.getSubBlockCount())
        {
            Debug.Log("Block initialised with an incorrect subBlockVariants Array");
            subBlockVariants = new int[this.blockVariant.getSubBlockCount()]; // assumes all values zero
        }



        // TODO: none of this. the highlight should move and scale itself
        transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = this.blockVariant.getColor();



        // spawn all sub blocks
        foreach (int s in subBlockVariants)
        {
            Transform subBlock = Instantiate(BlockManager.blockFab, transform).transform;
            Block subBlockScript = subBlock.GetComponent<Block>();
            subBlockScript.initialise(s);
            subBlocks.Add(subBlockScript);
        }
    }

    public void drawBlock()
    {
        foreach (Block subBlock in subBlocks)
            subBlock.drawBlock();

        populateTextBox();
        resizeBlock();
    }

    // fills text box with text, updates width and height, and moves subblocks
    private void populateTextBox()
    {
        width = blockVariant.getWidth();
        height = blockVariant.getHeight();
        int extraHeight = 0;

        string[] lines = blockVariant.getLines();
        int[,] subBlockPositions = blockVariant.getSubBlockPositions();

        Debug.Log(blockVariant.getName() + " " + subBlockPositions.GetLength(0));
        for (int i = 0; i < subBlockPositions.GetLength(0); i++)
        {
            Block block = subBlocks[i];

            int currentLine = subBlockPositions[i, 0];
            int posInLine = subBlockPositions[i, 1];

            // split line into two strings, one before @ and one after @
            string before = lines[currentLine].Substring(0, posInLine + 1);
            string after = lines[currentLine].Substring(posInLine + 3);

            // create a blank area which subblocks[i] will be on top
            // if this is a multi-line block
            string newLine = before + new string(' ', block.getWidth() - 1) + after;
            if (block.getHeight() > 1)
            {
                lines[currentLine] = newLine + new string('\n', block.getHeight() - 1);
            }
            // if this is a single-line block
            else
            {
                // e.g.,             "    " + "\n\n        " + ";"
                lines[currentLine] = newLine;
            }



            // move subblock
            Vector3 sBP = FontManager.lettersAndLinesToVector(posInLine, -(currentLine + extraHeight));
            sBP.z = block.transform.localPosition.z;
            block.transform.localPosition = sBP;

            // update width and height
            if (lines[currentLine].IndexOf('\n') >= 0)
            {
                // increment height
                extraHeight += (block.getHeight() - 1);

                // compute width
                string[] split = lines[currentLine].Split('\n');
                foreach (string s in split)
                    if (s.Length > width) width = s.Length;
            }
            else
            {
                // compute width
                if (lines[currentLine].Length > width)
                    width = lines[currentLine].Length;
            }


            // if we need to insert another block on this line, recalculate block positions
            if (i + 1 < subBlockPositions.GetLength(0) && subBlockPositions[i + 1, 0] == currentLine)
                subBlockPositions = BlockManager.getSubBlockPositions(lines);
        }
        height += extraHeight;


        // flatten into a single string
        string text = "";
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace('@', ' ');
            text += lines[i];
            if (i != lines.Length - 1) text += '\n';
        }

        // get rid of all @s, we had to keep them for subBlockPositions
        textBox.text = text;
    }

    // resizes this block
    private void resizeBlock()
    {
        Vector2 planeSize = FontManager.lettersAndLinesToVector(width, height);

        // body plane
        Transform body = transform.GetChild(0);
        body.localScale = new Vector3(planeSize.x, planeSize.y, 1f);

        // body text
        RectTransform tB = (RectTransform)textBox.transform;
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, planeSize.x);
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, planeSize.y);
        tB.localPosition = new Vector3(planeSize.x / 2f, -planeSize.y / 2f, tB.localPosition.z);
    }



    public int getSubBlockIndex(Block b)
    {
        return subBlocks.IndexOf(b);
    }

    public void replaceSubBlock(Block b, int index)
    {
        subBlocks[index] = b;
    }

    public string getText()
    {
        return textBox.text;
    }

    public Block getParent()
    {
        return transform.parent.GetComponent<Block>();
    }

    public Window2D getWindow2D()
    {
        Window2D window = transform.parent.GetComponent<Window2D>();
        if (window == null)
        {
            Block parent = getParent();
            if (parent != null)
                window = parent.getWindow2D();
        }
        return window;
    }
}
