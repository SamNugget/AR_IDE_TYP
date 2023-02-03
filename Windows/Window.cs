using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class Window : MonoBehaviour
{
    // name
    [SerializeField] private string name = "";
    public void setName(string name)
    {
        this.name = name;
        setTitleTextMessage("");
        setSimpleText();
    }

    public void setTitleTextMessage(string message, bool dash = false)
    {
        if (dash) topText.GetComponent<TextMeshPro>().text = name + " - " + message;
        else topText.GetComponent<TextMeshPro>().text = name + message;
    }

    protected virtual void setSimpleText()
    {
        simpleText.GetComponent<TextMeshPro>().text = name;
    }





    // width & height
    [Header("Width and Height")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [SerializeField] private float minWidth = 0.3f;
    [SerializeField] private float minHeight = 0.3f;

    public void scaleWindow(float width, float height)
    {
        if (topBarEnabled) height += topBarHeight;

        if (width < minWidth) width = minWidth;
        if (height < minHeight) height = minHeight;
        this.width = width;
        this.height = height;

        updateVisuals();
    }





    // visuals
    private static float linkerOffset = 0.015f;
    private static float topBarHeight = 0.05f;

    [Header("Visuals")]
    [SerializeField] private bool simpleView;
    public void setSimpleView(bool simple)
    {
        if (simpleView == simple) return;
        simpleView = simple;
        updateVisuals();
    }
    [SerializeField] private bool topBarEnabled;
    [SerializeField] public Material defaultMat;

    // simple
    private Transform simpleParent;
    protected RectTransform simpleText;
    // -buttons
    [SerializeField] private Transform detailButton;
    // detailed
    private Transform detailedParent;
    private RectTransform topText;
    protected Transform contentParent;
    // -buttons
    [SerializeField] private Transform backButton;
    [SerializeField] private Transform closeButton;
    [SerializeField] private Transform simpleButton;

    [Header("Buttons")]
    [SerializeField] private bool backButtonEnabled;
    [SerializeField] private bool closeButtonEnabled;
    [SerializeField] private bool simpleButtonEnabled;

    private void updateVisuals()
    {
        simpleParent.gameObject.SetActive(simpleView);
        detailedParent.gameObject.SetActive(!simpleView);

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (simpleView)
        {
            Transform simpleCube = simpleParent.Find("SimpleCube");
            Transform linkers = simpleParent.Find("Linkers");

            // boxCollider
            boxCollider.center = new Vector2(width/2f, -height/2f);
            boxCollider.size = new Vector3(width, height, 0.03f);

            // cube
            simpleCube.localScale = new Vector3(width, height, 1f);

            // text
            simpleText.sizeDelta = new Vector2(width, height);

            // linkers
            linkers.localPosition = new Vector2(width/2f, -height/2f);
            linkers.GetChild(0).localPosition = new Vector2(0f, height/2f + linkerOffset);
            linkers.GetChild(1).localPosition = new Vector2(0f, -(height/2f + linkerOffset));

            // button
            detailButton.localPosition = new Vector2(width/2f, 0f);
        }
        else
        {
            Transform topBar = detailedParent.Find("TopBar");
            Transform backplate = detailedParent.Find("Backplate");

            // top bar
            topBar.gameObject.SetActive(topBarEnabled);
            topText.gameObject.SetActive(topBarEnabled);
            boxCollider.enabled = topBarEnabled;
            if (topBarEnabled)
            {
                // boxCollider
                boxCollider.center = new Vector2(width / 2f, -topBarHeight / 2f);
                boxCollider.size = new Vector3(width, topBarHeight, 0.01f);

                contentParent.localPosition = new Vector3(0f, -topBarHeight, 0f);

                topBar.localScale = new Vector3(width, 1f, 1f);

                topText.sizeDelta = new Vector2(width, topBarHeight);

                // buttons
                closeButton.localPosition = new Vector2(width, 0f);
                simpleButton.localPosition = new Vector2(width, 0f);
            }
            else
                contentParent.localPosition = Vector3.zero;

            // handle backplate
            backplate.localScale = new Vector3(width, height, 1f);
        }
    }

    protected void updateMaterial(Material mat)
    {
        simpleParent.Find("SimpleCube").GetComponentInChildren<Renderer>().material = mat;
        detailedParent.Find("TopBar").GetComponentInChildren<Renderer>().material = mat;
    }





    public virtual void close()
    {
        WindowManager.destroyWindow(this);
    }





    void Awake()
    {
        simpleParent = transform.Find("Simple");
        simpleText = (RectTransform)simpleParent.Find("SimpleText");

        detailedParent = transform.Find("Detailed");
        topText = (RectTransform)detailedParent.Find("TopText");
        contentParent = detailedParent.Find("Content");

        backButton.gameObject.SetActive(backButtonEnabled);
        closeButton.gameObject.SetActive(closeButtonEnabled);
        simpleButton.gameObject.SetActive(simpleButtonEnabled);

        scaleWindow(width, height);
        if (defaultMat != null) updateMaterial(defaultMat);
        if (name != "") setName(name);
    }
}
