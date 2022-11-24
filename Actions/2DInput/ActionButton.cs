using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private char action;
    public void setAction(char a)
    {
        action = a;
    }
    private string data;
    public void setData(string d)
    {
        data = d;
    }

    public void callAction()
    {
        ActionManager.callAction(action, data);
    }
}
