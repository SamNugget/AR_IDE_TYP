using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    private BlockManager.BlockType blockType;
    private List<Block> subBlocks;

    // the block has been spawned in the correct pos
    // now we need to scale, populate with text, and fill with sub-blocks
    public void initialise(BlockManager.BlockType blockType, List<BlockManager.BlockType> subBlockTypes = null)
    {
        this.blockType = blockType;
        this.subBlocks = new List<Block>();



        // populate with text
        List<string[]> splitLines = getSplitLines(blockType);
        string blockText = "";

        if (subBlockTypes == null)
        {
            BlockManager.singleton.getBlockType(0);
        }

        int extraLines = 0;
        for (int sL = 0; sL < splitLines.Count; sL++)
        {
            string newLine = "";

            if (splitLines[sL].Length == 1)
            {
                newLine += splitLines[sL][0] + "\n";
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
                        subBlockScript.initialise(subBlockType);
                        subBlocks.Add(subBlockScript);

                        Vector3 pos = FontManager.lettersAndLinesToVector(stringPos, sL + extraLines);
                        pos.y = -pos.y; pos.z = -0.01f;
                        subBlock.localPosition = pos;


                        for (int j = 0; j < subBlockScript.getLineCount() - 1; j++)
                        {
                            newLine += "\n"; extraLines++;
                        }
                        int noOfSpaces = subBlockScript.getMaxLettersPerLine();
                        if (subBlockScript.getLineCount() > 1) noOfSpaces += stringPos;
                        for (int j = 0; j < noOfSpaces; j++)
                            newLine += " ";
                    }
                }
                newLine += "\n";
            }

            blockText += newLine;
        }

        TextMeshPro bodyText = transform.GetChild(0).GetComponent<TextMeshPro>();
        bodyText.text = blockText;



        // scale
        //BlockManager.BlockType thisBlockType = BlockManager.singleton.getBlockType(blockTypeIndex);
        Vector3 scale = FontManager.lettersAndLinesToVector(getMaxLettersPerLine(), splitLines.Count + extraLines); // -1 removes \n
        scale.z = 1f;
        transform.GetChild(1).localScale = scale;
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

    public int getLineCount()
    {
        TextMeshPro bodyText = transform.GetChild(0).GetComponent<TextMeshPro>();
        int i = 0;
        int nPos = -1;
        do
        {
            nPos = bodyText.text.IndexOf('\n', nPos + 1);
            if (nPos >= 0) i++;
        }
        while (nPos >= 0);
        return i;
    }

    public int getMaxLettersPerLine()
    {
        TextMeshPro bodyText = transform.GetChild(0).GetComponent<TextMeshPro>();
        int max = 0;
        int nPos = -1;
        do
        {
            int tempNPos = bodyText.text.IndexOf('\n', nPos + 1);
            if (tempNPos - (nPos + 1) > max) max = tempNPos - (nPos + 1);
            nPos = tempNPos;
        }
        while (nPos >= 0);
        Debug.Log("this blocktype " + blockType.getName() + " has max letters per line: " + max);
        return max;
    }
}
