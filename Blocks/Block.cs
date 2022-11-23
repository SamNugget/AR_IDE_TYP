using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System.Text;

public class Block : MonoBehaviour
{
    protected BlockManager.BlockVariant blockVariant;
    public BlockManager.BlockVariant getBlockVariant()
    {
        return blockVariant;
    }
    public List<Block> subBlocks = null; // TODO: make private

    [SerializeField] protected int width; // TODO: rm sf
    public int getWidth() { return width; }
    [SerializeField] protected int height; // TODO: rm sf
    public int getHeight() { return height; }

    [SerializeField] private TextMeshPro textBox;

    // this needs to go
    protected bool highlightable;
    public bool getHighlightable()
    {
        return highlightable;
    }



    public virtual void initialise(int blockVariant, int[] subBlockVariants = null)
    {
        this.blockVariant = BlockManager.singleton.getBlockVariant(blockVariant);
        this.subBlocks = new List<Block>();
        highlightable = true;



        if (subBlockVariants == null)
        {
            subBlockVariants = new int[this.blockVariant.getSubBlockCount()]; // assumes all values zero
        }
        else if (subBlockVariants.Length != this.blockVariant.getSubBlockCount())
        {
            Debug.Log("Block initialised with an incorrect subBlockVariants Array");
            subBlockVariants = new int[this.blockVariant.getSubBlockCount()]; // assumes all values zero
        }



        // if empty block, change layer. TODO: change this
        if (this.blockVariant.getBlockType().Equals(BlockManager.EMPTY))
        {
            transform.GetChild(0).GetChild(0).gameObject.layer = 8;
            transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = BlockManager.singleton.emptyMat;
        }



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
    public virtual void populateTextBox()
    {
        width = blockVariant.getWidth();
        height = blockVariant.getHeight();
        int extraHeight = 0;

        string[] lines = blockVariant.getLines();
        int[,] subBlockPositions = blockVariant.getSubBlockPositions();

        for (int i = 0; i < subBlockPositions.GetLength(0); i++)
        {
            int currentLine = subBlockPositions[i, 0];
            int posInLine = subBlockPositions[i, 1];

            // split line into two strings, one before @ and one after @
            string before = lines[currentLine].Substring(0, posInLine);
            string after = lines[currentLine].Substring(posInLine + 3);

            // create a blank area which subblocks[i] will be on top
            // if this is a multi-line block
            string newLine = before + new string(' ', subBlocks[i].getWidth()) + after;
            if (subBlocks[i].getHeight() > 1)
            {
                lines[currentLine] = new string('\n', subBlocks[i].getHeight() - 1) + newLine;
            }
            // if this is a single-line block
            else
            {
                // e.g.,             "    " + "\n\n        " + ";"
                lines[currentLine] = newLine;
            }



            // move subblock
            Vector3 sBP = FontManager.lettersAndLinesToVector(posInLine, -(currentLine + extraHeight));
            sBP.z = subBlocks[i].transform.localPosition.z;
            subBlocks[i].transform.localPosition = sBP;

            // update width and height
            if (lines[currentLine].IndexOf('\n') >= 0)
            {
                // increment height
                extraHeight += (subBlocks[i].getHeight() - 1);

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
        foreach (string line in lines)
        {
            text += line;
            if (line != lines[lines.Length - 1]) text += '\n';
        }
        textBox.text = text;
    }

    // resizes this block
    public virtual void resizeBlock()
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
        Block current = this;
        while (current.GetType() != typeof(Window2D))
            current = current.getParent();
        return (Window2D)current;
    }
}
