using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using ActionManagement;

public class EditButtonManager : ButtonManager2D
{
    [SerializeField] private Transform blockButtonsParent;

    [SerializeField] private int placeableVariants;
    private int totalVariants;

    public override void distributeButtons()
    {
        deleteButtons(blockButtonsParent);



        int noOfButtons = placeableVariants;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        for (int i = 0; i < noOfButtons; i++)
        {
            int variantIndex = i + (totalVariants - placeableVariants);

            BlockManager.BlockVariant bV = BlockManager.getBlockVariant(variantIndex);
            buttonLabels[i] = bV.getName();
            actions[i] = ActionManager.PLACE_SELECT;
            data[i] = variantIndex;
        }

        distributeFit(buttonLabels, actions, data, blockButtonsParent);
    }

    void Start()
    {
        totalVariants = BlockManager.getNoOfDefaultBlockVariants();
        base.Start();
    }
}
