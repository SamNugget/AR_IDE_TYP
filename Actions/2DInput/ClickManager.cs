using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A temporary class for testing the 2D stuff
public class ClickManager : MonoBehaviour
{
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

            if (Input.GetMouseButtonDown(0)) // 3D: on finger intersect collider
            {
                // place block
                if (bV != null && blockType.Equals(BlockManager.EMPTY))
                {
                    ActionManager.callAction(ActionManager.PLACE, b);
                }
            }
            else if (Input.GetMouseButton(0)) // 3D: while grabbing
            {
                // move window (3D: or block) (doesn't call actionmanager, done by MTRK)
                if (bV == null && Vector3.Distance(Vector3.zero, lastHit.point) != 0f)
                {
                    Vector3 change = hit.point - lastHit.point;
                    hit.transform.parent.parent.position += change;
                }
            }
            else // 3D: raycast out of hand
            {
                // highlight (doesn't call actionmanager, simple action)
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
