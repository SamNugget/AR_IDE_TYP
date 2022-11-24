using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A temporary class for testing the 2D stuff
public class ClickManager : MonoBehaviour
{
    // which block variant to place
    private static int currentBlockVariantIndex = 2;
    public static void setCurrentBlockVariantIndex(int index)
    {
        currentBlockVariantIndex = index;
    }

    private RaycastHit lastHit;
    void Update()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Block b = getHitBlock(hit);
            if (b == null) return;

            BlockManager.BlockVariant bV = b.getBlockVariant();

            string blockType = (bV != null ? bV.getBlockType() : "");

            if (Input.GetMouseButtonDown(0))
            {
                if (bV != null && blockType.Equals(BlockManager.EMPTY))
                {
                    Debug.Log("SPAWN");
                    Block hitBlock = hit.transform.parent.parent.GetComponent<Block>();
                    BlockManager.spawnBlock(currentBlockVariantIndex, hitBlock);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (bV == null && Vector3.Distance(Vector3.zero, lastHit.point) != 0f)
                {
                    Vector3 change = hit.point - lastHit.point;
                    hit.transform.parent.parent.position += change;
                }
            }
            else
            {
                if (bV != null)
                {
                    hit.transform.GetComponent<MeshRenderer>().enabled = true;
                }
            }



            if (lastHit.transform != hit.transform) hideLastHit();
            lastHit = hit;
            return;
        }



        hideLastHit();
    }

    private void hideLastHit()
    {
        if (lastHit.transform != null)
        {
            Block b = getHitBlock(lastHit);
            if (b == null) return;

            BlockManager.BlockVariant bV = b.getBlockVariant();
            if (bV == null) return;
            
            if (b.getHighlightable())
                lastHit.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private Block getHitBlock(RaycastHit hit)
    {
        Transform parent = hit.transform.parent;
        for (int i = 0; i < 2 && parent != null; i++)
        {
            Block b = parent.GetComponent<Block>();
            if (b != null) return b;

            parent = parent.parent;
        }

        return null;
    }
}
