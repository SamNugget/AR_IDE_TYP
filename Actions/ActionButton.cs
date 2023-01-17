using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using ActionManagement;

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
    protected object data;
    public virtual void setData(object d)
    {
        data = d;
    }

    public void setIcon(string iconName)
    {
        transform.GetComponent<ButtonConfigHelper>().SetQuadIconByName(iconName);
    }

    public void callAction()
    {
        ActionManager.callAction(action, data);
    }
}
