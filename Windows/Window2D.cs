using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Window2D : Block
{
    [SerializeField] private TextMeshPro textBox;

    [SerializeField] private float width = 1f;
    [SerializeField] private float height = 1f;

    public override string initialise(BlockManager.BlockType blockType, int offsetX = 0, List<BlockManager.BlockType> subBlockTypes = null)
    {
        this.blockType = null;
        this.subBlocks = new List<Block>();

        // spawn subblock - currently only works for one
        Transform subBlock = Instantiate(BlockManager.blockFab, transform).transform;
        Block subBlockScript = subBlock.GetComponent<Block>();
        // TEMP
        List<BlockManager.BlockType> bTs = new List<BlockManager.BlockType>();
        bTs.Add(BlockManager.singleton.getBlockType(1));
        bTs.Add(BlockManager.singleton.getBlockType(2));
        string result = subBlockScript.initialise(subBlockTypes[0], 0, bTs);
        // TEMP END
        subBlocks.Add(subBlockScript);
        subBlock.localPosition = new Vector3(0f, 0f, -0.001f);

        textBox.text = result;

        return result;
    }



    // added functions

    void Update()
    {
        rescaleWindow(WindowManager.scale);

        resizeWindow(width, height);
    }

    public void rescaleWindow(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void resizeWindow(float w, float h)
    {
        width = w;
        height = h;

        float topPlaneHeight = FontManager.lettersAndLinesToVector(0, 1).y;

        // title plane
        Transform top = transform.GetChild(0);
        top.localScale =    new Vector3(w, topPlaneHeight, 1f);
        top.localPosition = new Vector3(0f, topPlaneHeight, 0f);

        // title text
        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        topText.localPosition = new Vector3(w / 2f, topPlaneHeight / 2f, topText.localPosition.z);

        // body plane
        Transform body = transform.GetChild(2);
        body.localScale = new Vector3(w, h, 1f);

        // body text
        RectTransform tB = (RectTransform)textBox.transform;
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        tB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        tB.localPosition = new Vector3(w / 2f, -h / 2f, topText.localPosition.z);
    }

    public string getCode()
    {
        return textBox.text;
    }
}
