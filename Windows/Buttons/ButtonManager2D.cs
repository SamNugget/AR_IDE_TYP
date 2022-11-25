using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public abstract class ButtonManager2D : MonoBehaviour
{
    public Transform blockButtonsParent;
    public GameObject buttonFab;

    public float buttonSpacing;



    protected abstract void distributeButtons();

    protected List<Transform> distributeVertically(string[] buttonLabels, char[] actions, object[] data)
    {
        if (buttonLabels.Length != actions.Length || buttonLabels.Length != data.Length)
        {
            Debug.Log("All three input parameter arrays must be the same length.");
            return null;
        }


        float textHeight = FontManager.lineHeight;
        List<Transform> buttons = new List<Transform>();
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            float width = FontManager.lettersAndLinesToVector(buttonLabels[i].Length + 1, 0).x;

            // spawn, position and scale button
            RectTransform newButton = Instantiate(buttonFab, blockButtonsParent).GetComponent<RectTransform>();
            newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight * 100f);
            newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 100f);
            newButton.localPosition = new Vector2(-(width / 2f), -(i * (textHeight + buttonSpacing) + (textHeight / 2f)));

            // set text of button
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonLabels[i];

            // configure actionbuttonscript
            ActionButton actionButtonScript = newButton.GetComponent<ActionButton>();
            actionButtonScript.setAction(actions[i]);
            actionButtonScript.setData(data[i]);

            // add callback to actionbuttonscript on buttonscript
            Button buttonScript = newButton.GetComponent<Button>();
            UnityEvent uE = buttonScript.onClick;
            uE.AddListener(actionButtonScript.callAction);

            buttons.Add(newButton);
        }

        return buttons;
    }

    protected void removeButtonFromList(Transform toRemove, List<Transform> buttons)
    {
        int index = buttons.IndexOf(toRemove);
        if (index < 0) return;


        Transform toReplace = buttons[index + 1];
        if (toReplace != null)
        {
            Vector3 change = toRemove.position - toReplace.position;
            // shift all the buttons over
            for (int i = index + 1; i < buttons.Count; i++)
                buttons[i].position += change;
        }


        // delete button
        Destroy(toRemove.gameObject);
    }

    void Start()
    {
        distributeButtons();
    }
}
