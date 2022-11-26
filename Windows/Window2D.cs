using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Window2D : MonoBehaviour
{
    [SerializeField] private string name = "i_am_a_title";

    [SerializeField] private int width = 20;
    public void setWidth(int w) { width = w; }
    [SerializeField] private int height = 10;
    public void setHeight(int h) { height = h; }

    protected void resizeWindow()
    {
        float topPlaneHeight = FontManager.lineHeight;
        Vector2 planeSize = FontManager.lettersAndLinesToVector(width, height);

        // title plane
        Transform top = transform.GetChild(0);
        top.localScale = new Vector3(planeSize.x, topPlaneHeight, 1f);
        top.localPosition = new Vector3(0f, topPlaneHeight, 0f);

        // title text
        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, planeSize.x);
        topText.localPosition = new Vector3(planeSize.x / 2f, topPlaneHeight / 2f, topText.localPosition.z);
        topText.GetComponent<TextMeshPro>().text = name;

        // body plane
        Transform body = transform.GetChild(2);
        body.localScale = new Vector3(planeSize.x, planeSize.y, 1f);
    }

    // to be handled by MRTK
    /*protected void rescaleWindow(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }*/

    void Start()
    {
        resizeWindow();
    }

    public void setTitleTextMessage(string message)
    {
        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.GetComponent<TextMeshPro>().text = name + " - " + message;
    }
}
