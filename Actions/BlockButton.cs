using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockButton : MonoBehaviour
{
    public void callAction()
    {
        ActionManager.callAction(ActionManager.BLOCK_CLICKED, GetComponentInParent<Block>());
    }
}
