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

    [SerializeField] private int variantsToExclude;

    protected override void distributeButtons()
    {
        int noOfButtons = BlockManager.getNoOfBlockVariants() - variantsToExclude;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        for (int i = 0; i < noOfButtons; i++)
        {
            int variantIndex = i + variantsToExclude;

            BlockManager.BlockVariant bV = BlockManager.getBlockVariant(variantIndex);
            buttonLabels[i] = bV.getName();
            actions[i] = ActionManager.PLACE_SELECT;
            data[i] = variantIndex;
        }

        blockButtons = distributeVertically(buttonLabels, actions, data);
    }
}
