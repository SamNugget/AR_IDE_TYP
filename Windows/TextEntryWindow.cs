using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEntryWindow : Window3D
{
    [SerializeField] private TMP_InputField inputField;
    private char action;
    public void setAction(char a) { action = a; }

    public void onTextEntered()
    {
        ActionManager.callAction(action, inputField.text);
        inputField.text = "";
    }
}
