using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static char BLOCK_SELECT = 'B';
    public static char PLACE = 'P';
    public static char DELETE = 'D';

    // which block variant to place
    private static int blockToPlace = -1;


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


    public static void callAction(char action, object data)
    {
        try
        {
            if (action == BLOCK_SELECT)
            {
                int variantIndex = (int)data;
                blockToPlace = variantIndex;
            }
            else if (action == PLACE || action == DELETE)
            {
                Block toReplace = (Block)data;
                BlockManager.spawnBlock((action == PLACE ? blockToPlace : 0), toReplace);
            }
            else Debug.Log("Action " + action + " was not recognised.");
        }
        catch(Exception e)
        {
            Debug.Log("Err calling action " + action + " with data: " + data);
        }
    }
}
