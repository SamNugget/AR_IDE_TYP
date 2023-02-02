using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using ActionManagement;

public class TextEntryWindow : Window
{
    [SerializeField] private MRTKTMPInputField inputField;
    private char action;

    public void onEndEdit()
    {
        string input = "";
        foreach (char c in inputField.text)
        {
            if (c == '_') ;
            else if (c >= 48 && c <= 57) ; // numbers
            else if (c >= 65 && c <= 90) ; // uppercase
            else if (c >= 97 && c <= 122) ; // lowercase
            else continue;

            input += c;
        }

        if (input.Length != 0)
            ActionManager.callCurrentMode(input);
    }

    public override void close()
    {
        ActionManager.clearMode();
        base.close();
    }

    void Start()
    {
        action = ActionManager.CREATE_NAME;
    }
}
