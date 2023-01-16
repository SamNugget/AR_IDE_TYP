using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

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
                if (Input.GetMouseButtonDown(0)) // 3D: on finger intersect collider
                {
                    // block clicked
                    ActionManager.callAction(ActionManager.BLOCK_CLICKED, b);
                }
                else if (Input.GetMouseButton(0)) // 3D: on grab
                {

                }
                else // 3D: raycast out of hand
                {
                    // highlight (doesn't call actionmanager, simple action)
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
            Block b = getHitType<Block>(lastHit);
            if (b == null) return;
            
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
