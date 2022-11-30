using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EditButtonManager : ButtonManager2D
{
    [SerializeField] private EditWindow editWindow;
    [SerializeField] private Transform blockButtonsParent;

    // TODO: lists could be more dynamic, parent class could have add/remove button functions
    private List<Transform> blockButtons = new List<Transform>();

    [SerializeField] private int variantsToExclude;
    private int totalVariants;

    public override void distributeButtons()
    {
        deleteButtons(blockButtons);



        int noOfButtons = totalVariants - variantsToExclude;

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

        blockButtons = distributeVertically(buttonLabels, actions, data, blockButtonsParent);
    }

    void Start()
    {
        totalVariants = BlockManager.getNoOfBlockVariants();
        base.Start();
    }
}
