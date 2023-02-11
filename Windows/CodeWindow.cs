using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FileManagement;

public class CodeWindow : Window
{
    [SerializeField] private TMP_InputField inputField;

    public void onEndEdit()
    {
        CompilationManager.ExecuteClasslessCode(inputField.text);
    }
}
