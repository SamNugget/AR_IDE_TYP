using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EditButtonManager : ButtonManager2D
{
    // TODO: lists could be more dynamic, parent class could have add/remove button functions
    private List<Transform> blockButtons = new List<Transform>();
    private List<Transform> variableButtons = new List<Transform>();

    protected override void distributeButtons()
    {
        int noOfButtons = BlockManager.singleton.getNoOfBlockVariants() - 2;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        for (int i = 0; i < noOfButtons; i++)
        {
            int variantIndex = i + 2;

            BlockManager.BlockVariant bV = BlockManager.singleton.getBlockVariant(variantIndex);
            buttonLabels[i] = bV.getName();
            actions[i] = ActionManager.BLOCK_SELECT;
            data[i] = variantIndex;
        }

        distributeVertically(buttonLabels, actions, data);
    }
}
