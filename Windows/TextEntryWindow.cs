using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEntryWindow : Window2D
{
    [SerializeField] private TMP_InputField inputField;
    // TODO: more dynamic -> private char action;

    public void onTextEntered()
    {
        ActionManager.callAction(ActionManager.CREATE_VARIABLE, inputField.text);
        inputField.text = "";
    }
}
