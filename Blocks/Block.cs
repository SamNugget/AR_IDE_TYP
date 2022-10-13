using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    private int blockTypeIndex;
    public int getBlockTypeIndex()
    {
        return blockTypeIndex;
    }
    private List<TMP_InputField> inputFields;

    public void initialise(int blockTypeIndex, List<TMP_InputField> inputFields)
    {
        this.blockTypeIndex = blockTypeIndex;
        this.inputFields = inputFields;
    }

    public string ToString()
    {
        string[] lines = BlockManager.singleton.getBlockType(blockTypeIndex).getLines();

        string output = "";
        int inputFieldIndex = 0;
        foreach (string line in lines)
        {
            List<int> inputFieldPositions = new List<int>();
            int i = -1;
            while (true)
            {
                i = line.IndexOf("{}", i + 1);
                if (i >= 0)
                    inputFieldPositions.Add(i);
                else
                    break;
            }

            int currentPos = 0;
            foreach (int iFPos in inputFieldPositions)
            {
                if (iFPos > currentPos)
                    output += line.Substring(currentPos, iFPos - currentPos);
                currentPos = iFPos + 2;
                inputFieldIndex++;
            }

            output += line.Substring(currentPos);
        }

        return output;
    }
}
