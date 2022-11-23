using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Window2D : Block
{
    public override void initialise(int blockVariant, int[] subBlockVariants = null)
    {
        this.blockVariant = null;
        this.subBlocks = new List<Block>();
        highlightable = false;



        if (subBlockVariants.Length != 1)
        {
            Debug.Log("Block initialised with an incorrect subBlockVariants Array");
            subBlockVariants = new int[1]; // assumes all values zero
        }



        // spawn all sub blocks
        foreach (int s in subBlockVariants)
        {
            Transform subBlock = Instantiate(BlockManager.blockFab, transform).transform;
            Block subBlockScript = subBlock.GetComponent<Block>();
            subBlockScript.initialise(s);
            subBlocks.Add(subBlockScript);
        }



        resizeBlock();
    }

    public override void populateTextBox()
    {

    }

    public override void resizeBlock()
    {
        float topPlaneHeight = FontManager.lettersAndLinesToVector(0, 1).y;
        Vector2 planeSize = FontManager.lettersAndLinesToVector(width, height);

        // title plane
        Transform top = transform.GetChild(0);
        top.localScale = new Vector3(planeSize.x, topPlaneHeight, 1f);
        top.localPosition = new Vector3(0f, topPlaneHeight, 0f);

        // title text
        RectTransform topText = (RectTransform)transform.GetChild(1);
        topText.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, planeSize.x);
        topText.localPosition = new Vector3(planeSize.x / 2f, topPlaneHeight / 2f, topText.localPosition.z);

        // body plane
        Transform body = transform.GetChild(2);
        body.localScale = new Vector3(planeSize.x, planeSize.y, 1f);


        //checkSubBlockSize(new Vector2(transform.position.x + width, transform.position.y - height));
    }



    // rescale top level child blocks, and thus all nested blocks
    public void rescaleWindow(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public string getCode()
    {
        // TODO: update so recursive
        return "";
    }
}
