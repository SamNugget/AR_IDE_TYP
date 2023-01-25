using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class CIButton : MonoBehaviour
{
    private static string[] iconNames = {
        "IconClass",
        "IconInterface"
    };

    public void setButtonIcon(bool cl)
    {
        transform.GetComponent<ButtonConfigHelper>().SetQuadIconByName((cl ? iconNames[0] : iconNames[1]));
    }
}
