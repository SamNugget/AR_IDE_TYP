using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private TextMeshPro labelText;
    public void setLabel(string label)
    {
        if (labelText != null) labelText.text = label;
    }
    public string getLabel()
    {
        return labelText.text;
    }

    private char action;
    public void setAction(char a)
    {
        action = a;
    }
    private object data;
    public void setData(object d)
    {
        data = d;
    }

    public void callAction()
    {
        ActionManager.callAction(action, data);
    }
}
