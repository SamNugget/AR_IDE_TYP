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

    private RaycastHit lastHit;
    private int currentBlockTypeIndex = 1;
    void Update()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int layerHit = hit.transform.gameObject.layer;
            string layerName = getHitLayerName(layerHit);
            Debug.Log(layerName);

            if (Input.GetMouseButtonDown(0))
            {
                switch (layerName)
                {
                    case "Empty2D":
                        Debug.Log("SPAWN");
                        Block hitBlock = hit.transform.parent.parent.GetComponent<Block>();
                        BlockManager.spawnBlock(currentBlockTypeIndex, hitBlock);
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
                    case "Empty2D":
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
        if (lastHit.transform != null)
        {
            string layerName = getHitLayerName(lastHit.transform.gameObject.layer);
            if (string.Equals(layerName, "Block2D") || string.Equals(layerName, "Empty2D"))
            {
                Block b = lastHit.transform.parent.parent.GetComponent<Block>();
                if (b != null && b.getHighlightable())
                    lastHit.transform.GetComponent<MeshRenderer>().enabled = false;
            }
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

    public void setCurrentBlockTypeIndex(int index)
    {
        currentBlockTypeIndex = index;
    }
}
