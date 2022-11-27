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
        else if (currentAction == DELETE_SELECT) tools.setTitleTextMessage("Deleting...");
        else if (currentAction == INSERT_LINE) tools.setTitleTextMessage("Inserting lines...");
    }

    public readonly static char PLACE_SELECT = 'p';
    public readonly static char DELETE_SELECT = 'd';
    public readonly static char BLOCK_CLICKED = 'B';
    public readonly static char INSERT_LINE = 'I';


    //                  |placement  |deletion   |empty replace      |split  |no action              |place in ANY
    //ANY               |  never  instantiated  |any                |yes    |                       |NA
    //EMPTY             |yes        |           |                   |       |                       |NA
    //USING             |           |yes        |UG                 |yes    |                       |must be explicit
    //===============================================================================================
    //ACCESS_MODIFIER   |           |           |                   |       |cycles through         |NA
    //TYPE              |           |yes        |TP  create if none |no     |                       |yes?
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
    // edit window
    private static EditWindow edit;
    [SerializeField] private EditWindow editWindow;

    void Awake()
    {
        tools = toolsWindow;
        edit = editWindow;
    }

    public static void callAction(char action, object data)
    {
        try
        {



            if (currentAction == INSERT_LINE)
            {
                if (action != BLOCK_CLICKED && action != INSERT_LINE)
                {
                    edit.setCollidersEnabled(true);
                    edit.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), false);
                }
            }

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
                Block clicked = (Block)data;
                BlockManager.BlockVariant variant = clicked.getBlockVariant();
                string type = variant.getBlockType();
                Debug.Log("Clicked " + type);



                // check for special types first
                if (type.Equals(BlockManager.ACCESS_MODIFIER))
                {
                    BlockManager.BlockVariant newVariant;
                    if (variant.getName().Equals("Public"))
                        newVariant = BlockManager.getBlockVariant("Private");
                    else
                        newVariant = BlockManager.getBlockVariant("Public");

                    int nVIndex = BlockManager.getBlockVariantIndex(newVariant);
                    BlockManager.spawnBlock(nVIndex, clicked);

                    Window2D window = clicked.getWindow2D();
                    if (window != null) ((EditWindow)window).drawBlocks();
                }
                else if (type.Equals(BlockManager.NAMESPACE))
                {
                    // keyboard or special list
                    // namespaces clutter block list
                }
                else if (type.Equals(BlockManager.INSERT_LINE))
                {
                    Block parent = clicked.getParent();
                    BlockManager.splitBlock(parent);
                    edit.setCollidersEnabled(false);
                    edit.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), false);
                    edit.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), true);
                }



                // apply current action if not special type
                else if (currentAction == PLACE_SELECT)
                {
                    BlockManager.spawnBlock(blockToPlace, clicked);
                }
                else if (currentAction == DELETE_SELECT)
                {
                    if (type.Equals(BlockManager.EMPTY) || type.Equals(BlockManager.ACCESS_MODIFIER))
                    {
                        Debug.Log("Can't delete block of this type.");
                        return;
                    }
                    BlockManager.spawnBlock(0, clicked);
                }



            }
            else if (action == INSERT_LINE)
            {
                edit.setCollidersEnabled(false);
                edit.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), true);
                setCurrentAction(action);
            }
            else Debug.Log("Action " + action + " was not recognised.");



        }
        catch(Exception e)
        {
            Debug.Log("Err calling action " + action + " with data: " + data);
        }
    }
}
