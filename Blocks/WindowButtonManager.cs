using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class WindowButtonManager : MonoBehaviour
{
    public Transform blockButtonsParent;
    public GameObject buttonFab;

    public float buttonSpacing;

    // temp
    public ClickManager clickManager;

    private List<Transform> blockButtons = new List<Transform>();
    private List<Transform> variableButtons = new List<Transform>();

    void Start()
    {
        float textHeight = FontManager.lettersAndLinesToVector(0, 1).y;

        for (int i = 0; ; i++)
        {
            int variantIndex = i + 2;
            BlockManager.BlockVariant bV = BlockManager.singleton.getBlockVariant(variantIndex);
            if (bV == null) break;

            string name = bV.getName();
            float width = FontManager.lettersAndLinesToVector(name.Length + 1, 0).x;

            RectTransform newButton = Instantiate(buttonFab, blockButtonsParent).GetComponent<RectTransform>();
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = name;
            newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight * 100f);
            newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 100f);
            newButton.localPosition = new Vector2(-(width / 2f), -(i * (textHeight + buttonSpacing) + (textHeight / 2f)));

            Button buttonScript = newButton.GetComponent<Button>();
            UnityEvent uE = buttonScript.onClick;
            uE.AddListener(setCurrentBlockVariantIndex);

            blockButtons.Add(newButton);
        }
    }

    public void setCurrentBlockVariantIndex()
    {
        clickManager.setCurrentBlockVariantIndex(4);
    }
}
