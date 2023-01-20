using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class BlockButton : MonoBehaviour
{
    public void callAction()
    {
        ActionManager.callAction(ActionManager.BLOCK_CLICKED, GetComponentInParent<Block>());
    }

    void Update()
    {
        Collider coll = transform.parent.GetComponentInChildren<Collider>();
        if (coll == null || coll.enabled == false)
            Destroy(gameObject);
    }
}
