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



    public abstract void distributeButtons();

    protected List<Transform> distributeVertically(string[] buttonLabels, char[] actions, object[] data, Transform parent = null)
    {
        return distribute(buttonLabels, actions, data, parent);
    }

    protected List<Transform> distributeHorizontally(string[] buttonLabels, char[] actions, object[] data, Transform parent = null)
    {
        Debug.Log("Err, horizontal button placement was removed.");
        return /*distribute(buttonLabels, actions, data, parent)*/null;
    }

    private List<Transform> distribute(string[] buttonLabels, char[] actions, object[] data, Transform parent)
    {
        if (buttonLabels.Length != actions.Length || buttonLabels.Length != data.Length)
        {
            Debug.Log("All three input parameter arrays must be the same length.");
            return null;
        }


        if (parent == null) parent = transform;


        List<Transform> buttons = new List<Transform>();
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            Transform newButton = spawnButton(buttonLabels[i], actions[i], data[i], i, parent);
            buttons.Add(newButton);
        }

        return buttons;
    }

    protected Transform spawnButton(string buttonLabel, char action, object data, int row, Transform parent)
    {
        Vector2 planeSize = FontManager.lettersAndLinesToVector(buttonLabel.Length, 1);
        float width = planeSize.x;
        float height = planeSize.y;
        float s = WindowManager.blockScale;

        // spawn, position and scale button
        Transform newButton = Instantiate(buttonFab, parent).GetComponent<Transform>();
        newButton.localPosition = new Vector3(0f, (row * -height * s) + buttonSpacing, 0f);
        newButton.localScale = new Vector3(s, s, s);

        // scale body plane
        Transform body = newButton.GetChild(0);
        body.localScale = new Vector3(width, height, 1f);

        // scale body text
        RectTransform tB = (RectTransform)newButton.GetChild(1);
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        tB.localPosition = new Vector3(width / 2f, -height / 2f, tB.localPosition.z);

        // set text of button
        tB.GetComponent<TextMeshPro>().text = buttonLabel;

        // configure actionbuttonscript
        ActionButton actionButtonScript = newButton.GetComponent<ActionButton>();
        actionButtonScript.setAction(action);
        actionButtonScript.setData(data);

        return newButton;
    }

    protected void removeButtonFromList(Transform toRemove, List<Transform> buttons)
    {
        int index = buttons.IndexOf(toRemove);
        if (index < 0) return;


        Transform toReplace = (index + 1 < buttons.Count ? buttons[index + 1] : null);
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

    protected void deleteButtons(Transform parent = null)
    {
        if (parent == null) parent = transform;

        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    public void Start()
    {
        distributeButtons();
    }
}
