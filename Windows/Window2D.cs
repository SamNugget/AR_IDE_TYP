using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Window2D : MonoBehaviour
{
    [SerializeField] private TextMeshPro textBox;
    private List<Block> blocks;

    private float width = 1f;
    private float height = 1f;

    private static float textWidthOffset = 0.1f;

    void Start()
    {
        rescaleWindow(WindowManager.scale);

        resizeWindow(width, height);
    }

    /*void Update()
    {
        resizeWindow(width, height);
    }*/

    public void rescaleWindow(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void resizeWindow(float w, float h)
    {
        width = w;
        height = h;

        float bodyW = 0.1f * width; // because it is a plane
        float bodyH = 0.1f * height;

        Transform top = transform.GetChild(0);
        top.localScale = new Vector3(bodyW, top.localScale.y, top.localScale.z);
        top.position = new Vector3(top.position.x, (height + 0.15f) / 2f, top.position.z);

        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width - textWidthOffset);
        topText.position = new Vector3(topText.position.x, (height + 0.15f) / 2f, topText.position.z);

        Transform body = transform.GetChild(2);
        body.localScale = new Vector3(bodyW, body.localScale.y, bodyH);

        RectTransform tB = (RectTransform)textBox.transform;
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width - textWidthOffset);
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
