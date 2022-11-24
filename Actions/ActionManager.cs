using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static char BLOCK_SELECT = 'B';
    public static char PLACE = 'P';


    //                  |placement  |deletion   |empty replace      |split  |no action              |place in ANY
    //ANY               |  never  instantiated  |any                |yes    |                       |NA
    //EMPTY             |yes        |           |                   |       |                       |NA
    //USING             |           |yes        |UG                 |yes    |                       |must be explicit
    //===============================================================================================
    //ACCESS_MODIFIER   |           |           |                   |       |cycles through         |NA
    //TYPE              |           |yes        |TP                 |no     |if no type, create type|yes?
    //BOOLEAN_EXPRESSION|           |yes        |BE                 |(?)    |                       |must be explicit
    //NAMESPACE         |           |yes        |                   |       |keyboard or list       |must be explicit
    //BODY              |           |yes        |BY                 |yes    |                       |yes


    // placement
    // 1. select block to place
    // 2. place block
    // 3. selected block == null

    // delete
    // 1. select delete
    // 2. click any number of blocks
    // 3. next action picked will end this action

    // creating types? scope logic?


    public static void callAction(char action, string data)
    {
        switch(action)
        {
            case 'B':
                int variantIndex;
                int.TryParse(data, out variantIndex);
                if (variantIndex > 0)
                {
                    ClickManager.setCurrentBlockVariantIndex(variantIndex);
                    return;
                }
                break;
        }

        Debug.Log("Err calling action " + action + " with data: " + data);
    }
}
