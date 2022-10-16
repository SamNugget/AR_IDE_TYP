using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System.Text;

public class Block : MonoBehaviour
{
    private BlockManager.BlockType blockType;
    private List<Block> subBlocks;

    // the block has been spawned in the correct pos
    // now we need to scale, populate with text, and fill with sub-blocks
    public string initialise(BlockManager.BlockType blockType, int offsetX = 0, List<BlockManager.BlockType> subBlockTypes = null)
    {
        this.blockType = blockType;
        this.subBlocks = new List<Block>();



        // populate with text
        List<string[]> splitLines = getSplitLines(blockType);
        string blockText = "";
        int extraLines = 0;

        for (int sL = 0; sL < splitLines.Count; sL++)
        {
            //Debug.Log("line " + sL + " of " + blockType.getName() + " has " + splitLines[sL].Length + " splits");
            string newLine = "";

            if (splitLines[sL].Length == 1)
            {
                newLine += splitLines[sL][0];
            }
            else
            {
                int stringPos = 0;
                for (int i = 0; i < splitLines[sL].Length; i++)
                {
                    newLine += splitLines[sL][i];

                    if (i + 1 < splitLines[sL].Length)
                    {
                        stringPos += splitLines[sL][i].Length;


                        // spawn subblock
                        Transform subBlock = Instantiate(BlockManager.blockFab, transform).transform;

                        BlockManager.BlockType subBlockType;
                        if (subBlockTypes == null) subBlockType = BlockManager.singleton.getBlockType(0); // empty
                        else subBlockType = subBlockTypes[subBlocks.Count];

                        Block subBlockScript = subBlock.GetComponent<Block>();
                        string result = subBlockScript.initialise(subBlockType, offsetX + stringPos);
                        newLine += result;
                        subBlocks.Add(subBlockScript);

                        extraLines += getLineCount(result) - 1;

                        Vector3 pos = FontManager.lettersAndLinesToVector(stringPos, sL + extraLines);
                        pos.y = -pos.y; pos.z = -0.01f;
                        subBlock.localPosition = pos;
                    }
                }
            }

            if (sL + 1 < splitLines.Count) newLine += '\n' + new string(' ', offsetX);
            blockText += newLine;
        }

        TextMeshPro bodyText = transform.GetChild(0).GetComponent<TextMeshPro>();
        if (transform.parent == null || transform.parent.GetComponent<Block>() == null)
        {
            bodyText.text = blockText;
        }
        else
        {
            bodyText.text = "";
        }



        // scale
        Vector3 scale = FontManager.lettersAndLinesToVector(getMaxLettersPerLine(blockText), getLineCount(bodyText.text));
        Debug.Log(blockType.getName());
        byte[] bytes = Encoding.ASCII.GetBytes(blockText);
        foreach (byte b in bytes)
        {
            Debug.Log(b);
        }
        scale.z = 1f;
        transform.GetChild(1).localScale = scale;



        Debug.Log(blockText);
        return blockText;
    }

    public string ToString()
    {
        // check if this is an empty block and throw error if so

        List<string[]> splitLines = getSplitLines(blockType);
        return "";
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
                if (i >= 0)
                    subBlockPositions.Add(i);
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
        return text.Split('\n').Length;
    }

    private static int getMaxLettersPerLine(string text)
    {
        string[] split = text.Split('\n');
        if (split.Length <= 1)
            return text.Length;

        int max = 0;
        foreach (string s in split)
            if (s.Length > max) max = s.Length;
        return max;
    }
}
