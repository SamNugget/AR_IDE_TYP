using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : ActionButton
{
    [SerializeField] private string[] icons;
    [SerializeField] private string[] states;

    public void toggle()
    {
        int i;
        for (i = 0; i < icons.Length; i++)
            if ((string)data == states[i])
                break;

        i++;
        if (i >= icons.Length) i = 0;

        setData(states[i]);
        setIcon(icons[i]);
    }

    void Start()
    {
        setAction(ActionManager.CYCLE_CONSTRUCT);
        setData(states[0]);
        setIcon(icons[0]);
    }
}
