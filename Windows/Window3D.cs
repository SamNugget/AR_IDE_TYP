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
        setTitleTextMessage("", false);
    }

    public void setTitleTextMessage(string message, bool dash = true)
    {
        if (dash) titleText.GetComponent<TextMeshPro>().text = name + " - " + message;
        else      titleText.GetComponent<TextMeshPro>().text = name + message;
    }

    public void setWidth(float width)
    {
        Transform bP = transform.GetChild(0).Find("Backplate");
        bP.localScale = new Vector3(width * WindowManager.blockScale, bP.localScale.y, bP.localScale.z);
    }

    public void setHeight(float height)
    {
        Transform bP = transform.GetChild(0).Find("Backplate");
        bP.localScale = new Vector3(bP.localScale.x, height * WindowManager.blockScale, bP.localScale.z);
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

        setTitleTextMessage("", false);
    }
}
