using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusDetector : MonoBehaviour, IMixedRealityFocusHandler
{
    public void OnFocusEnter(FocusEventData eventData)
    {
        BlockManager.moveBlockButton(GetComponentInParent<Block>());
    }

    public void OnFocusExit(FocusEventData eventData) { }
}
