using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class Window3D : MonoBehaviour
{
    public string name = "i_am_a_title";

    public void setTitleTextMessage(string message)
    {
        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.GetComponent<TextMeshPro>().text = name + " - " + message;
    }



    private RadialView radialView;

    public void stopFollowing()
    {
        Debug.Log("Stop following " + name);
        radialView.enabled = false;
    }

    public void followingToggled()
    {
        if (radialView.enabled)
            WindowManager.stopAllFollowing(this);
    }

    void Awake()
    {
        radialView = GetComponentInChildren<RadialView>();
        if (radialView == null) Debug.LogError("Err no radial view component");
    }
}
