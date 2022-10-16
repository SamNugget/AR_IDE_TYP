using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A temporary class for testing the 2D stuff
public class ClickManager : MonoBehaviour
{
    [SerializeField] private List<Layer> layers;
    [System.Serializable]
    public class Layer
    {
        [SerializeField] private string name;
        public string getName()
        {
            return name;
        }

        [SerializeField] private int layerIndex;
        public int getLayerIndex()
        {
            return layerIndex;
        }
    }

    RaycastHit lastHit;
    void Update()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int layerHit = hit.transform.gameObject.layer;
            string layerName = getHitLayerName(layerHit);

            if (Input.GetMouseButtonDown(0))
            {
                switch (layerName)
                {
                    case "Empty2D":
                        Debug.Log("Spawn block");
                        break;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                switch (layerName)
                {
                    case "Window2D":
                        if (Vector3.Distance(Vector3.zero, lastHit.point) == 0f) break;
                        Vector3 change = hit.point - lastHit.point;
                        hit.transform.parent.parent.position += change;
                        break;
                }
            }
            else
            {
                switch (layerName)
                {
                    case "Block2D":
                        hit.transform.GetComponent<MeshRenderer>().enabled = true;
                        break;
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
        if (lastHit.transform != null && string.Equals(getHitLayerName(lastHit.transform.gameObject.layer), "Block2D"))
        {
            Block b = lastHit.transform.parent.parent.GetComponent<Block>();
            if (b != null && b.getHighlightable())
                lastHit.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private string getHitLayerName(int layer)
    {
        string layerName = "";
        foreach (Layer l in layers)
        {
            if (l.getLayerIndex() == layer)
            {
                layerName = l.getName();
                break;
            }
        }
        return layerName;
    }
}
