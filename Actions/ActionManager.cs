using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager singleton = null;


    //                  |placement  |deletion   |empty replace      |split  |no action
    //ANY               |  never  instantiated  |any                |yes    |
    //EMPTY             |yes        |           |                   |       |
    //USING             |           |yes        |UG                 |yes    |
    //==============================================================================================
    //ACCESS_MODIFIER   |           |           |                   |       |cycles through
    //TYPE              |           |yes        |TP                 |no     |if no type, create type
    //BOOLEAN_EXPRESSION|           |yes        |BE                 |(?)    |
    //NAMESPACE         |           |yes        |                   |       |keyboard or list
    //BODY              |           |yes        |BY                 |yes    |


    // placement
    // 1. select block to place
    // 2. place block
    // 3. selected block == null

    // delete
    // 1. select delete
    // 2. click any number of blocks
    // 3. next action picked will end this action

    // creating types? scope logic?


    void Awake()
    {
        if (singleton == null) singleton = this;
        else Debug.LogError("Two ActionManager singletons.");
    }
}
