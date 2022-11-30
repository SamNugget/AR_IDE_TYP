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
    [SerializeField] private Transform variableButtonsParent;

    // TODO: lists could be more dynamic, parent class could have add/remove button functions
    private List<Transform> blockButtons = new List<Transform>();
    private List<Transform> variableButtons = new List<Transform>();

    [SerializeField] private int variantsToExclude;
    private int totalVariants;

    public override void distributeButtons()
    {
        deleteButtons(blockButtons);



        // get no. of declared/undeclared variables
        List<Variable> variables = editWindow.getVariables();

        List<Variable> undeclared = new List<Variable>();
        List<Variable> declared = new List<Variable>();

        foreach (Variable v in variables)
        {
            if (v.declarationBlock == null) undeclared.Add(v);
            else declared.Add(v);
        }



        int noOfBlockButtons = totalVariants - variantsToExclude;
        int noOfButtons = noOfBlockButtons + undeclared.Count + 1;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        int i = 0;
        for (; i < noOfBlockButtons; i++)
        {
            int variantIndex = i + variantsToExclude;

            BlockManager.BlockVariant bV = BlockManager.getBlockVariant(variantIndex);
            buttonLabels[i] = bV.getName();
            actions[i] = ActionManager.PLACE_SELECT;
            data[i] = variantIndex;
        }

        buttonLabels[i] = "New Var";
        actions[i] = ActionManager.CREATE_VARIABLE;
        data[i] = null;
        i++;

        for (int j = 0; i < noOfButtons; i++)
        {
            buttonLabels[i] = undeclared[j].name;
            actions[i] = ActionManager.PLACE_SELECT;
            int variantIndex = BlockManager.getBlockVariantIndex(undeclared[j].blockVariant);
            data[i] = variantIndex;

            j++;
        }

        blockButtons = distributeVertically(buttonLabels, actions, data, blockButtonsParent);



        updateVariableButtons(declared);
    }

    private void updateVariableButtons(List<Variable> declaredVariables)
    {
        deleteButtons(variableButtons);



        float maxWidth = 0f;
        for (int i = 0; i < declaredVariables.Count; i++)
        {
            Variable v = declaredVariables[i];
            int variantIndex = BlockManager.getBlockVariantIndex(v.blockVariant);
            float width = FontManager.lettersAndLinesToVector(v.name.Length + 1, 0).x;
            if (width > maxWidth) maxWidth = width;

            float x = alignRightEdge ? -(width / 2f) : (width / 2f);
            Block declarationBlock = v.declarationBlock;
            //        vv this should instead be method, which account for things being off screen
            float y = declarationBlock.transform.localPosition.y/* - ((FontManager.lineHeight + buttonSpacing) * i)*/;
            Vector2 position = new Vector2(x, y - (FontManager.lineHeight / 2f));
            // TODO: account for duplicates

            variableButtons.Add(spawnButton(v.name, ActionManager.PLACE_SELECT, variantIndex, position, variableButtonsParent));
        }



        if (maxWidth > 0f)
            blockButtonsParent.transform.position = variableButtonsParent.transform.position - new Vector3(maxWidth + buttonSpacing, 0f);
    }

    void Start()
    {
        totalVariants = BlockManager.getNoOfBlockVariants();
        base.Start();
    }
}
