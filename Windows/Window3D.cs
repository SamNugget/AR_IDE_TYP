using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class Window3D : MonoBehaviour
{
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private string name = "";
    public void setName(string name)
    {
        this.name = name;
        setTitleTextMessage("");
    }

    public void setTitleTextMessage(string message)
    {
        if (message == null || message == "")
            titleText.GetComponent<TextMeshPro>().text = name;
        else
            titleText.GetComponent<TextMeshPro>().text = name + " - " + message;
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



    public virtual void close()
    {
        WindowManager.destroyWindow(this);
    }



    void Awake()
    {
        radialView = GetComponentInChildren<RadialView>();
        if (radialView == null) Debug.LogError("Err no radial view component");

        setTitleTextMessage("");
    }
}
