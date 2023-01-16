using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusDetector : MonoBehaviour, IMixedRealityFocusHandler
{
    // https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/pointers?view=mrtkunity-2022-05#pointer-event-interfaces
    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.input.imixedrealitytouchhandler?preserve-view=true&view=mixed-reality-toolkit-unity-2020-dotnet-2.8.0

    public void OnFocusEnter(FocusEventData eventData)
    {
        BlockManager.moveBlockButton(GetComponentInParent<Block>());
    }

    public void OnFocusExit(FocusEventData eventData) { }
}
