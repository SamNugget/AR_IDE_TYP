using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System.Text;

public class Block : MonoBehaviour
{
    protected BlockManager.BlockType blockType;
    protected List<Block> subBlocks;

    // the block has been spawned in the correct pos
    // now we need to scale, populate with text, and fill with sub-blocks
    public virtual string initialise(BlockManager.BlockType blockType, int offsetX = 0, List<BlockManager.BlockType> subBlockTypes = null)
    {
        this.blockType = blockType;
        this.subBlocks = new List<Block>();



        // populate with text
        List<string[]> splitLines = getSplitLines(blockType);
        string blockText = "";
        int extraLines = 0;

        for (int sL = 0; sL < splitLines.Count; sL++)
        {
            string newLine = "";

            if (splitLines[sL].Length == 1) // no subblocks on this line
            {
                newLine += splitLines[sL][0];
            }
            else // sub blocks on this line
            {
                int stringPos = offsetX;
                for (int i = 0; i < splitLines[sL].Length; i++)
                {
                    //TODO: assumes {} only comes in between strings
                    newLine += splitLines[sL][i];
                    stringPos += splitLines[sL][i].Length;

                    if (i + 1 < splitLines[sL].Length)
                    {
                        // spawn subblock
                        Transform subBlock = Instantiate(BlockManager.blockFab, transform).transform;

                        BlockManager.BlockType subBlockType;
                        if (subBlockTypes == null) subBlockType = BlockManager.singleton.getBlockType(0); // empty
                        else subBlockType = subBlockTypes[subBlocks.Count];

                        Block subBlockScript = subBlock.GetComponent<Block>();
                        string result = subBlockScript.initialise(subBlockType, stringPos);
                        newLine += result;
                        subBlocks.Add(subBlockScript);

                        Vector3 pos = FontManager.lettersAndLinesToVector(stringPos - offsetX, sL + extraLines);
                        pos.y = -pos.y; pos.z = -0.001f;
                        subBlock.localPosition = pos;

                        int lines = getLineCount(result) - 1;
                        extraLines += lines;
                        if (lines > 1) stringPos = 0;
                    }
                }
            }

            if (sL + 1 < splitLines.Count) newLine += '\n' + new string(' ', offsetX);
            blockText += newLine;
        }



        // scale
        Vector3 scale = FontManager.lettersAndLinesToVector(getMaxLettersPerLine(blockText, offsetX), getLineCount(blockText));
        scale.z = 1f;
        transform.GetChild(0).localScale = scale;



        return blockText;
    }

    private static List<string[]> getSplitLines(BlockManager.BlockType blockType)
    {
        string[] lines = blockType.getLines();
        List<string[]> splitLines = new List<string[]>();

        foreach (string line in lines)
        {
            List<int> subBlockPositions = new List<int>();
            int i = -2;
            do
            {
                i = line.IndexOf("{}", i + 2);
                if (i >= 0) subBlockPositions.Add(i);
            }
            while (i >= 0);


            if (subBlockPositions.Count == 0)
            {
                splitLines.Add(new string[] { line });
            }
            else
            {
                List<string> sL = new List<string>();

                int stringPos = 0;
                for (int j = 0; j < subBlockPositions.Count; j++)
                {
                    int len = subBlockPositions[j] - stringPos;
                    sL.Add(line.Substring(stringPos, len));

                    stringPos = subBlockPositions[j] + 2;
                }
                sL.Add(line.Substring(stringPos));

                splitLines.Add(sL.ToArray());
            }
        }

        return splitLines;
    }

    private static int getLineCount(string text)
    {
        text = string.Copy(text);

        return text.Split('\n').Length;
    }

    private static int getMaxLettersPerLine(string text, int offsetX)
    {
        text = string.Copy(text);

        string[] split = text.Split('\n');
        if (split.Length <= 1)
            return text.Length;

        int max = 0;
        for (int i = 0; i < split.Length; i++)
        {
            bool newMax = (i == 0 ? (split[i].Length > max) : (split[i].Length - offsetX > max));
            if (newMax) max = split[i].Length;
        }

        return max;
    }
}
