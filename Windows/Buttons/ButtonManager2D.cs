using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public abstract class ButtonManager2D : MonoBehaviour
{
    [SerializeField] private GameObject buttonFab;

    [SerializeField] protected float buttonSpacing;

    [SerializeField] protected bool alignRightEdge;



    protected abstract void distributeButtons();

    protected List<Transform> distributeVertically(string[] buttonLabels, char[] actions, object[] data, Transform parent = null)
    {
        return distribute(buttonLabels, actions, data, false, parent);
    }

    protected List<Transform> distributeHorizontally(string[] buttonLabels, char[] actions, object[] data, Transform parent = null)
    {
        return distribute(buttonLabels, actions, data, true, parent);
    }

    private List<Transform> distribute(string[] buttonLabels, char[] actions, object[] data, bool horizontal, Transform parent)
    {
        if (buttonLabels.Length != actions.Length || buttonLabels.Length != data.Length)
        {
            Debug.Log("All three input parameter arrays must be the same length.");
            return null;
        }


        if (parent == null) parent = transform;


        float textHeight = FontManager.lineHeight;
        List<Transform> buttons = new List<Transform>();
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            float width = FontManager.lettersAndLinesToVector(buttonLabels[i].Length + 1, 0).x;
            float x = alignRightEdge ? -(width / 2f) : (width / 2f);

            Vector2 position;
            if (horizontal)
            {
                if (buttons.Count > 0)
                {
                    float prevButtWidth = FontManager.lettersAndLinesToVector(buttonLabels[i - 1].Length + 1, 0).x;
                    float newX = (prevButtWidth / 2f) + buttonSpacing;
                    x += (alignRightEdge ? -newX : newX);

                    position = buttons[buttons.Count - 1].localPosition;
                    position.x += x;
                }
                else position = new Vector2(x, -(textHeight / 2f));
            }
            else
            {
                position = new Vector2(x, -(i * (textHeight + buttonSpacing) + (textHeight / 2f)));
            }

            Transform newButton = spawnButton(buttonLabels[i], actions[i], data[i], position, parent);
            buttons.Add(newButton);
        }

        return buttons;
    }

    protected Transform spawnButton(string buttonLabel, char action, object data, Vector2 position, Transform parent)
    {
        float textHeight = FontManager.lineHeight;
        float width = FontManager.lettersAndLinesToVector(buttonLabel.Length + 1, 0).x;

        // spawn, position and scale button
        RectTransform newButton = Instantiate(buttonFab, parent).GetComponent<RectTransform>();
        newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight * 100f);
        newButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 100f);
        newButton.localPosition = position;

        // set text of button
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonLabel;

        // configure actionbuttonscript
        ActionButton actionButtonScript = newButton.GetComponent<ActionButton>();
        actionButtonScript.setAction(action);
        actionButtonScript.setData(data);

        // add callback to actionbuttonscript on buttonscript
        Button buttonScript = newButton.GetComponent<Button>();
        UnityEvent uE = buttonScript.onClick;
        uE.AddListener(actionButtonScript.callAction);

        return newButton;
    }

    protected void removeButtonFromList(Transform toRemove, List<Transform> buttons)
    {
        int index = buttons.IndexOf(toRemove);
        if (index < 0) return;


        Transform toReplace = buttons[index + 1];
        if (toReplace != null)
        {
            Vector2 change = toRemove.position - toReplace.position;
            // shift all the buttons over
            for (int i = index + 1; i < buttons.Count; i++)
                buttons[i].position += (Vector3)change;
        }


        // delete button
        Destroy(toRemove.gameObject);
    }

    protected void deleteButtons(List<Transform> buttons)
    {
        for (int i = buttons.Count - 1; i >= 0; i--)
        {
            Destroy(buttons[i].gameObject);
            buttons.RemoveAt(i);
        }
    }

    void Start()
    {
        distributeButtons();
    }
}
