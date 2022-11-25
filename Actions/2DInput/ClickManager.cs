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
            Block b = getHitType<Block>(hit);
            if (b == null)
            {
                if (Input.GetMouseButton(0)) // 3D: while grabbing
                {
                    Window2D window = getHitType<Window2D>(hit);

                    // move window (3D: or block) (doesn't call actionmanager, done by MTRK)
                    if (window != null && Vector3.Distance(Vector3.zero, lastHit.point) != 0f)
                    {
                        Vector3 change = hit.point - lastHit.point;
                        window.transform.position += change;
                    }
                }
            }
            else
            {
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
                else if (Input.GetMouseButton(0))
                {

                }
                else // 3D: raycast out of hand
                {
                    // highlight (doesn't call actionmanager, simple action)
                    if (bV != null)
                    {
                        hit.transform.GetComponent<MeshRenderer>().enabled = true;
                    }
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
            Block b = getHitType<Block>(lastHit);
            if (b == null) return;
            
            if (b.getHighlightable()) // TODO: this is old, remove
                lastHit.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private T getHitType<T>(RaycastHit hit)
    {
        Transform parent = hit.transform.parent;
        for (int i = 0; i < 2 && parent != null; i++)
        {
            T h = parent.GetComponent<T>();
            if (h != null) return h;

            parent = parent.parent;
        }

        return default(T);
    }
}
