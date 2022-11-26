using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private static char currentAction = '\0';
    private static void setCurrentAction(char action)
    {
        currentAction = action;

        if (currentAction == PLACE_SELECT)
        {
            string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
            tools.setTitleTextMessage("Placing " + blockName + " block...");
        }
        if (currentAction == DELETE_SELECT) tools.setTitleTextMessage("Deleting...");
    }

    public static char PLACE_SELECT = 'p';
    public static char DELETE_SELECT = 'd';
    public static char BLOCK_CLICKED = 'B';


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


    // which block variant to place
    private static int blockToPlace = -1;

    // tools window for feedback
    private static Window2D tools;
    [SerializeField] private Window2D toolsWindow;

    void Awake()
    {
        tools = toolsWindow;
    }

    public static void callAction(char action, object data)
    {
        try
        {



            if (action == PLACE_SELECT)
            {
                int variantIndex = (int)data;
                blockToPlace = variantIndex;
                setCurrentAction(action);
            }
            else if (action == DELETE_SELECT)
            {
                setCurrentAction(action);
            }
            else if (action == BLOCK_CLICKED)
            {
                Block toReplace = (Block)data;



                if (currentAction == PLACE_SELECT)
                {
                    BlockManager.spawnBlock(blockToPlace, toReplace);
                }
                else if (currentAction == DELETE_SELECT)
                {
                    string type = toReplace.getBlockVariant().getBlockType();
                    if (type.Equals(BlockManager.EMPTY) || type.Equals(BlockManager.ACCESS_MODIFIER))
                    {
                        Debug.Log("Can't delete block of this type.");
                        return;
                    }
                    BlockManager.spawnBlock(0, toReplace);
                }



            }
            else Debug.Log("Action " + action + " was not recognised.");



        }
        catch(Exception e)
        {
            Debug.Log("Err calling action " + action + " with data: " + data);
        }
    }
}
