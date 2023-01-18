using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

namespace ActionManagement
{
    public static class ActionManager
    {
        // BLOCK STUFF
        public readonly static char BLOCK_CLICKED = 'C';
        public readonly static char PLACE_SELECT = 'p'; // mode
        public readonly static char DELETE_SELECT = 'd'; // mode
        public readonly static char INSERT_LINE = 'i'; // mode
        public readonly static char CREATE_NAME = 'n'; // mode
        public readonly static char NAME_FIELD_OR_METHOD = 'N';
        public readonly static char NAME_VARIABLE = 'V';

        // TOOLS
        public readonly static char SAVE_CODE = 'S';

        // WINDOW STUFF
        public readonly static char OPEN_WORKSPACE = 'W';
        public readonly static char OPEN_FILE = 'F';
        public readonly static char BACK_TO_WORKSPACES = 'B';
        public readonly static char CYCLE_CONSTRUCT = 'Y';



        //public static List<string> BlocksEnabledForPlacing = new List<string>() { BlockManager.EMPTY, BlockManager.NAME });
        //public static List<string> BlocksDisabledDefault = new List<string>() { BlockManager.EMPTY });



        private static Mode currentMode = null;
        private static void setCurrentMode(Mode mode, object data)
        {
            string toolsWindowMessage = "";


            // if this is an already active mode
            if (mode == currentMode)
            {
                if (mode == null) return;


                try
                {
                    // if this mode can selected repeatedly
                    // (and is called outside of ActionManager)
                    if (mode.multiSelect)
                    {
                        mode.onSelect(data);
                        toolsWindowMessage = mode.getToolsWindowMessage();
                    }
                    else
                    {
                        mode.onCall(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Err calling mode.");
                    Debug.Log(e.StackTrace);

                    currentMode = null;
                    toolsWindowMessage = "ERR MODE CALL";
                }
            }


            // if this a new mode
            else
            {
                if (currentMode != null) currentMode.onDeselect(); // deactivate current mode

                currentMode = mode; // switch state

                try
                {
                    if (mode != null)
                    {
                        // activate mode (if exists)
                        mode.onSelect(data);
                        toolsWindowMessage = mode.getToolsWindowMessage();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Err selecting mode.");
                    Debug.Log(e.StackTrace);

                    currentMode = null;
                    toolsWindowMessage = "ERR MODE SELECT";
                }
            }


            // set message on toolsWindow
            Window3D toolsWindow = WindowManager.getWindowWithComponent<ToolsWindow>();
            if (toolsWindow != null)
                toolsWindow.setTitleTextMessage(toolsWindowMessage);
        }
        public static bool callCurrentMode(object data)
        {
            try
            {
                currentMode.onCall(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Err calling mode.");
                Debug.Log(e.StackTrace);
                return false;
            }
        }
        public static void clearMode()
        {
            setCurrentMode(null, null);
        }
        public static char getCurrentModeSymbol()
        {
            foreach (char c in actions.Keys)
                if (actions[c] == currentMode) return c;
            return '\0';
        }



        private static Dictionary<char, Act> actions = null;



        private static void initialiseActions()
        {
            actions = new Dictionary<char, Act>();

            actions.Add(BLOCK_CLICKED, new BlockClicked());
            actions.Add(PLACE_SELECT, new Place());
            actions.Add(DELETE_SELECT, new Delete());
            actions.Add(INSERT_LINE, new InsertLine());
            actions.Add(CREATE_NAME, new CreateName());
            actions.Add(NAME_FIELD_OR_METHOD, new NameFieldOrMethod());
            actions.Add(NAME_VARIABLE, new NameVariable());

            actions.Add(SAVE_CODE, new SaveCode());

            actions.Add(OPEN_WORKSPACE, new OpenWorkspace());
            actions.Add(OPEN_FILE, new OpenFile());
            actions.Add(BACK_TO_WORKSPACES, new BackToWorkspaces());
        }



        public static void callAction(char action, object data)
        {
            if (actions == null) initialiseActions();

            if (!actions.ContainsKey(action))
            {
                Debug.Log("Action " + action + " was not recognised.");
                return;
            }

            Act newAction = actions[action];





            // is this newAction a mode-selection
            if (newAction is Mode mode)
            {
                setCurrentMode(mode, data);
            }
            // or immediate action
            else
            {
                try { newAction.onCall(data); }
                catch (Exception e)
                {
                    Debug.Log("Err calling action " + action + " with data: " + data);
                    Debug.Log(e.StackTrace);
                }
            }
        }
    }



    public interface Act
    {
        public void onCall(object data);
    }



    public abstract class Mode : Act
    {
        public bool multiSelect = false;

        public abstract void onCall(object data);

        public abstract void onSelect(object data);

        public abstract void onDeselect();

        public abstract string getToolsWindowMessage();
    }



    public class BlockClicked : Act
    {
        public void onCall(object data)
        {
            char modeSymbol = ActionManager.getCurrentModeSymbol();

            Block clicked = (Block)data;
            BlockManager.BlockVariant variant = clicked.getBlockVariant();
            string type = variant.getBlockType();



            bool codeModified = true;
            // check for special types first
            if (BlockManager.isCycleable(type))
            {
                int nVIndex = BlockManager.cycleBlockVariantIndex(variant);
                BlockManager.spawnBlock(nVIndex, clicked, false);

                Block master = clicked.getMasterBlock();
                if (master != null) master.drawBlock();
            }
            else if (type == BlockManager.INSERT_LINE)
            {
                // this must be insert line mode, so call insert line
                ActionManager.callCurrentMode(data);
            }
            else if (type == BlockManager.PLACE_FIELD || type == BlockManager.PLACE_METHOD)
            {
                ActionManager.callAction(ActionManager.NAME_FIELD_OR_METHOD, clicked);
            }


            else if (modeSymbol == ActionManager.DELETE_SELECT)
            {
                // will delete or place
                ActionManager.callCurrentMode(data);
            }


            // check for lower-priority special types
            else if (type == BlockManager.NAME)
            {
                int variantIndex = BlockManager.getBlockVariantIndex(variant);
                ActionManager.callAction(ActionManager.PLACE_SELECT, variantIndex);
                codeModified = false; // just copying, no changes
            }


            else if (modeSymbol == ActionManager.PLACE_SELECT)
            {
                // will delete or place
                ActionManager.callCurrentMode(data);
            }
            else codeModified = false; // if dropped out, no changes



            if (codeModified)
                ;//    ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).setTitleTextMessage("*");
        }
    }



    public class Place : Mode
    {
        // which block variant to place
        private int blockToPlace = -1;

        public override void onCall(object data)
        {
            Block lastMaster = ((Block)data).getMasterBlock();
            lastMaster.setColliderEnabled(false);
            lastMaster.setColliderEnabled(true, new List<string>() { BlockManager.EMPTY, BlockManager.NAME });

            BlockManager.spawnBlock(blockToPlace, (Block)data);
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            int variantIndex = (int)data;
            blockToPlace = variantIndex;

            Block lastMaster = BlockManager.getLastMaster();
            lastMaster.setColliderEnabled(false);
            lastMaster.setColliderEnabled(true, new List<string>() { BlockManager.EMPTY, BlockManager.NAME });
        }

        public override void onDeselect()
        {
            BlockManager.getLastMaster().setColliderEnabled(true);
        }

        public override string getToolsWindowMessage()
        {
            string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
            return ("Placing " + blockName + " block...");
        }
    }



    public class Delete : Mode
    {
        public override void onCall(object data)
        {
            Block toReplace = (Block)data;
            Block parent = toReplace.getParent();
            if (parent == null)
            {
                Debug.Log("Can't delete the master block."); return;
            }

            // TODO: will deleting this affect variable declaration?

            /*string actualType = ((Block)data).getBlockVariant().getBlockType();
            if (actualType.Equals(BlockManager.EMPTY) || actualType.Equals(BlockManager.ACCESS_MODIFIER))
            {
                Debug.Log("Can't delete blocks of this type.");
                return;
            }*/

            string[] subBlockTypes = parent.getBlockVariant().getSubBlockTypes();
            int subBlockIndex = parent.getSubBlockIndex(toReplace);
            string supposedType = subBlockTypes[subBlockIndex]; // not the actual type, but what should be here
            if (supposedType.Equals(BlockManager.NEW_NAME))
            {
                Debug.Log("Can't delete a name.");
                return;
            }

            BlockManager.spawnBlock(0, toReplace, false);
            onSelect(null);
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            // hide all blocks that can't be deleted
            Block lastMaster = BlockManager.getLastMaster();
            lastMaster.setColliderEnabled(false, new List<string>() { BlockManager.EMPTY, BlockManager.CONSTRUCT, BlockManager.ACCESS_MODIFIER, BlockManager.TRUE_FALSE });
        }

        public override void onDeselect()
        {
            BlockManager.getLastMaster().setColliderEnabled(true);
        }

        public override string getToolsWindowMessage()
        {
            return ("Deleting...");
        }
    }



    public class InsertLine : Mode
    {
        public override void onCall(object data)
        {
            if (!(data is Block)) return;

            Block parent = ((Block)data).getParent();
            BlockManager.splitBlock(parent);

            Block lastMaster = parent.getMasterBlock();
            lastMaster.setColliderEnabled(false);
            lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), false);
            lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), true);
        }

        public override void onSelect(object data)
        {
            Block lastMaster = BlockManager.getLastMaster();
            lastMaster.setColliderEnabled(false);
            lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), true);
        }

        public override void onDeselect()
        {
            Block lastMaster = BlockManager.getLastMaster();
            lastMaster.setColliderEnabled(true);
            lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), false);
        }

        public override string getToolsWindowMessage()
        {
            return ("Inserting lines...");
        }
    }



    public class CreateName : Mode
    {
        private Window3D textEntryWindow;
        
        private NameCreator nameCreator;

        public override void onCall(object data)
        {
            nameCreator.onFinishedNaming(true, (string)data);

            WindowManager.destroyWindow(textEntryWindow);
            ActionManager.clearMode();
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            if (textEntryWindow != null) onDeselect();

            nameCreator = (NameCreator)data;

            textEntryWindow = WindowManager.spawnTextInputWindow();
            textEntryWindow.setName(nameCreator.getTextEntryWindowMessage());
        }

        public override void onDeselect()
        {
            if (textEntryWindow != null)
            {
                nameCreator.onFinishedNaming(false, null);

                WindowManager.destroyWindow(textEntryWindow);
            }
        }

        public override string getToolsWindowMessage()
        {
            return ("Naming...");
        }
    }



    public abstract class NameCreator
    {
        public abstract void onFinishedNaming(bool success, string name);
        public abstract string getTextEntryWindowMessage();
    }



    public class NameFieldOrMethod : NameCreator, Act
    {
        // BlockClicked ==> NameFieldOrMethod ==> CreateName ==> NameFieldOrMethod

        private Block clicked;
        private bool field;

        public void onCall(object data)
        {
            clicked = (Block)data;
            field = clicked.getBlockVariant().getBlockType() == BlockManager.PLACE_FIELD;

            ActionManager.callAction(ActionManager.CREATE_NAME, this);
        }

        public override void onFinishedNaming(bool success, string name)
        {
            if (!success) return;

            // split clicked, and put it on the bottom
            BlockManager.splitBlock(clicked, false);

            Block splitter = clicked.getParent();

            // get object reference to top (empty) block
            int clickedIndex = splitter.getSubBlockIndex(clicked);
            Block toReplace = splitter.getSubBlock(clickedIndex == 0 ? 1 : 0);

            // get variant index of to place
            int variantIndex = BlockManager.getBlockVariantIndex(field ? "Field" : "Method");

            // replace top block
            Block newBlock = BlockManager.spawnBlock(variantIndex, toReplace);

            // replace name block
            Block emptyNameBlock = newBlock.getSubBlock(2);
            int nameBlockIndex = BlockManager.createNameBlock(name);
            BlockManager.spawnBlock(nameBlockIndex, emptyNameBlock);
        }

        public override string getTextEntryWindowMessage()
        {
            if (field) return "Name Field:";
            else return "Name Method:";
        }
    }



    public class NameVariable : NameCreator, Act
    {
        // BlockClicked ==> NameVariable ==> CreateName ==> NameVariable

        private Block beingNamed; // variable declaration block
        private Block beingReplaced; // the empty block

        public void onCall(object data)
        {
            beingNamed = ((Block[])data)[0];
            beingReplaced = ((Block[])data)[1];

            ActionManager.callAction(ActionManager.CREATE_NAME, this);
        }

        public override void onFinishedNaming(bool success, string name)
        {
            if (success)
            {
                // create new block type for this name
                int nameBlockIndex = BlockManager.createNameBlock(name);

                // replace empty block with this new name block
                BlockManager.spawnBlock(nameBlockIndex, beingReplaced, false);
            }
            else
            {
                // delete the parent block which has a name as part of it
                BlockManager.spawnBlock(0, beingNamed, false);
            }
        }

        public override string getTextEntryWindowMessage()
        {
            return "Name Variable:";
        }
    }



    public class SaveCode : Act
    {
        public void onCall(object data)
        {
            //Window3D editWindow = BlockManager.getLastEditWindow();

            Window3D toolsWindow = (ToolsWindow)WindowManager.getWindowWithComponent<ToolsWindow>();

            if (FileManager.saveAllFiles())
                toolsWindow.setTitleTextMessage("Saved");
            else toolsWindow.setTitleTextMessage("Err saving");
        }
    }



    public class OpenWorkspace : Act
    {
        public void onCall(object data)
        {
            FileManager.loadWorkspace((string)data);
            Window3D filesWindow = WindowManager.spawnFilesWindow();
            filesWindow.setName((string)data);
        }
    }



    public class OpenFile : Act
    {
        public void onCall(object data)
        {
            ReferenceTypeS rTS = FileManager.getSourceFile((string)data);
            Window3D spawned = WindowManager.spawnFileWindow();
            ((ReferenceTypeWindow)spawned).referenceTypeSave = rTS;
            spawned.setName((string)data);

            //WindowManager.moveEditToolWindows();

            // TODO: remove from list in files window
        }
    }



    public class BackToWorkspaces : Act
    {
        public void onCall(object data)
        {
            WindowManager.spawnWorkspacesWindow();

            // TODO: make user choose whether to save
        }
    }
}
