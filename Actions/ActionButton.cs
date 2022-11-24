using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    // float width?
    // float height?

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
