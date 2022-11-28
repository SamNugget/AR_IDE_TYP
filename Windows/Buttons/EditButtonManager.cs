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
    private List<Transform> blockButtons;
    private List<Transform> variableButtons = new List<Transform>();

    [SerializeField] private int variantsToExclude;

    protected override void distributeButtons()
    {
        int noOfButtons = BlockManager.getNoOfBlockVariants() - variantsToExclude + 1;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        buttonLabels[0] = "New Variable";
        actions[0] = ActionManager.CREATE_VARIABLE;
        data[0] = null;

        for (int i = 1; i < noOfButtons; i++)
        {
            int variantIndex = i + variantsToExclude - 1;

            BlockManager.BlockVariant bV = BlockManager.getBlockVariant(variantIndex);
            buttonLabels[i] = bV.getName();
            actions[i] = ActionManager.PLACE_SELECT;
            data[i] = variantIndex;
        }

        blockButtons = distributeVertically(buttonLabels, actions, data, blockButtonsParent);



        updateVariableButtons();
    }

    public void updateVariableButtons()
    {
        deleteButtons(variableButtons);



        List<Variable> variables = editWindow.getVariables();

        float maxWidth = 0f;
        for (int i = 0; i < variables.Count; i++)
        {
            Variable v = variables[i];
            int variantIndex = BlockManager.getBlockVariantIndex(v.blockVariant);
            float width = FontManager.lettersAndLinesToVector(v.name.Length + 1, 0).x;
            if (width > maxWidth) maxWidth = width;

            float x = alignRightEdge ? -(width / 2f) : (width / 2f);
            Block declarationBlock = v.declarationBlock;
            if (declarationBlock == null)
            {
                // add to other block list
            }
            //        vv this should instead be method, which account for things being off screen
            float y = declarationBlock.transform.localPosition.y - ((FontManager.lineHeight + buttonSpacing) * i);
            Vector2 position = new Vector2(x, y - (FontManager.lineHeight / 2f));
            // TODO: account for duplicates

            variableButtons.Add(spawnButton(v.name, ActionManager.PLACE_SELECT, variantIndex, position, variableButtonsParent));
        }



        if (maxWidth > 0f)
            blockButtonsParent.transform.position = variableButtonsParent.transform.position - new Vector3(maxWidth + buttonSpacing, 0f);
    }
}
