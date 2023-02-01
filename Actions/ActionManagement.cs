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
        public readonly static char CREATE_WORKSPACE = 'E';
        public readonly static char OPEN_FILE = 'F';
        public readonly static char CREATE_FILE = 'R';
        public readonly static char BACK_TO_WORKSPACES = 'B';



        public static List<string> blocksEnabledDefault = new List<string>() { BlockManager.NAME, BlockManager.PLACE, BlockManager.OPEN_METHOD }; // +all cyclable
        public static List<string> blocksEnabledForPlacing
        {
            get
            {
                List<string> placing = new List<string>(blocksEnabledDefault);
                placing.Add(BlockManager.EMPTY);
                return placing;
            }
        }



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
                toolsWindow.setTitleTextMessage(toolsWindowMessage, toolsWindowMessage != "");
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
            actions.Add(CREATE_WORKSPACE, new CreateWorkspace());
            actions.Add(OPEN_FILE, new OpenFile());
            actions.Add(CREATE_FILE, new CreateFile());
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
            Block master = clicked.getMasterBlock();
            BlockManager.BlockVariant variant = clicked.getBlockVariant();
            string type = variant.getBlockType();



            // check for special types first

            // cycleable blocks go to next state
            if (BlockManager.isCycleable(type))
            {
                int nVIndex = BlockManager.cycleBlockVariantIndex(variant);
                // change block variant to next in cycle and enable colliders
                Block spawned = BlockManager.spawnBlock(nVIndex, clicked, false);
                spawned.setColliderEnabled(true);

                master.drawBlock(true);
            }
            // lines are inserted
            else if (type == BlockManager.INSERT_LINE)
            {
                ActionManager.callCurrentMode(data);
            }
            // methods or fields are placed
            else if (type == BlockManager.PLACE)
            {
                BlockManager.lastMaster = master;
                ActionManager.callAction(ActionManager.NAME_FIELD_OR_METHOD, clicked);
                return; // change is in next action
            }
            else if (type == BlockManager.OPEN_METHOD)
            {
                FileWindow fileWindow = master.GetComponentInParent<FileWindow>();
                Block declarationBlock = clicked.getParent();

                WindowManager.moveMethodWindow(fileWindow, declarationBlock);

                return; // opening file
            }


            // delete the clicked block
            else if (modeSymbol == ActionManager.DELETE_SELECT)
            {
                ActionManager.callCurrentMode(data);
            }
            // select this block for placing
            else if (type == BlockManager.NAME)
            {
                int variantIndex = BlockManager.getBlockVariantIndex(variant);
                ActionManager.callAction(ActionManager.PLACE_SELECT, variantIndex);
                return; // just copying, no changes
            }
            // try and place a block here
            else if (modeSymbol == ActionManager.PLACE_SELECT)
            {
                ActionManager.callCurrentMode(data);
            }
            else return; // if dropped out, no changes



            BlockManager.lastFileWindow.setTitleTextMessage("*", false);
        }
    }



    public class Place : Mode
    {
        // which block variant to place
        private int blockToPlace = -1;

        public override void onCall(object data)
        {
            Block parent = ((Block)data).getParent();

            // place block and enable colliders where necessary
            BlockManager.spawnBlock(blockToPlace, (Block)data);
            parent.setColliderEnabled(true, ActionManager.blocksEnabledForPlacing);
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            int variantIndex = (int)data;
            blockToPlace = variantIndex;

            // enable only colliders necessary for placement
            WindowManager.updateEditWindowColliders(true, ActionManager.blocksEnabledForPlacing);
        }

        public override void onDeselect()
        {
            // return block collider states to default
            WindowManager.updateEditWindowColliders(true, ActionManager.blocksEnabledDefault);
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
            Block master = toReplace.getMasterBlock();
            if (toReplace == master)
            {
                Debug.Log("Hello, ActionManagement here, I'm afraid you cannot delete the master block."); return;
            }

            /*//TODO: will deleting this affect fields or methods
            string type = ((Block)data).getBlockVariant().getBlockType();
            if (type == BlockManager.FIELD || type == BlockManager.METHOD)
            {
                Debug.Log("Can't delete blocks of this type.");
                return;
            }

            //TODO: the effect of deleting variables
            string[] subBlockTypes = parent.getBlockVariant().getSubBlockTypes();
            int subBlockIndex = parent.getSubBlockIndex(toReplace);
            string supposedType = subBlockTypes[subBlockIndex]; // not the actual type, but what should be here
            if (supposedType.Equals(BlockManager.NEW_NAME))
            {
                Debug.Log("Can't delete a name.");
                return;
            }*/

            BlockManager.spawnBlock(0, toReplace, false);
            // enable only blocks that can be deleted
            master.enableLeafBlocks();
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            // enable only blocks that can be deleted
            WindowManager.enableEditWindowLeafBlocks();
        }

        public override void onDeselect()
        {
            // return block collider states to default
            WindowManager.updateEditWindowColliders(true, ActionManager.blocksEnabledDefault);
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

            Block block = (Block)data;
            Block toSplit = block.getParent();
            Block splitter = BlockManager.splitBlock(toSplit);

            // if splitting method or field, insert new method/field line
            string blockName = toSplit.getBlockVariant().getName();
            if (blockName == "Field" || blockName == "Method")
                BlockManager.spawnBlock(BlockManager.getBlockVariantIndex(blockName), splitter.getSubBlock(1));

            splitter.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), true);
        }

        public override void onSelect(object data)
        {
            insertLineBlocksEnabled(true);
        }

        public override void onDeselect()
        {
            insertLineBlocksEnabled(false);
        }

        private void insertLineBlocksEnabled(bool enabled)
        {
            if (enabled) WindowManager.updateEditWindowColliders(false);
            else         WindowManager.updateEditWindowColliders(true, ActionManager.blocksEnabledDefault);
            WindowManager.updateEditWindowSpecialBlocks(BlockManager.getBlockVariantIndex("Insert Line"), enabled);
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
            textEntryWindow = null;
            ActionManager.clearMode();
        }

        public override void onSelect(object data)
        {
            multiSelect = true;



            if (textEntryWindow != null) onDeselect();

            nameCreator = (NameCreator)data;

            textEntryWindow = WindowManager.spawnTextInputWindow(nameCreator.parent);
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
        public Window3D parent = null;

        public abstract void onFinishedNaming(bool success, string name);
        public abstract string getTextEntryWindowMessage();
    }



    public class NameFieldOrMethod : NameCreator, Act
    {
        // BlockClicked ==> NameFieldOrMethod ==> CreateName ==> NameFieldOrMethod

        private Block clicked;
        private string blockName;

        public void onCall(object data)
        {
            clicked = (Block)data;
            blockName = clicked.getBlockVariant().getName();

            ActionManager.callAction(ActionManager.CREATE_NAME, this);
        }

        public override void onFinishedNaming(bool success, string name)
        {
            if (!success) return;


            int emptyNameBlockIndex;
            Block b;
            if (blockName == "Place Variable")
            {
                // spawn a variable block (splittable with insert line)
                b = BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Variable"), clicked, false);
                emptyNameBlockIndex = 1;
            }
            else if (blockName == "Place Field")
            {
                b = BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Field"), clicked, false);
                BlockManager.lastFileWindow.referenceTypeSave.addField(b);
                emptyNameBlockIndex = 2;
            }
            else if (blockName == "Place Method")
            {
                b = BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Method"), clicked, false);
                BlockManager.lastFileWindow.referenceTypeSave.addMethod(b);
                emptyNameBlockIndex = 3;
            }
            else
            {
                Debug.Log("Hello, ActionManagement here, err, trying to name something not nameable."); return;
                return;
            }


            // replace name block
            Block emptyNameBlock = b.getSubBlock(emptyNameBlockIndex);
            int nameBlockVariantIndex = BlockManager.createNameBlock(name);
            BlockManager.spawnBlock(nameBlockVariantIndex, emptyNameBlock);


            BlockManager.lastMaster.setColliderEnabled(true, ActionManager.blocksEnabledForPlacing);
            BlockManager.lastFileWindow.setTitleTextMessage("*", false);
        }

        public override string getTextEntryWindowMessage()
        {
            return "Name " + blockName.Substring(6) + ':';
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

                BlockManager.lastMaster.setColliderEnabled(true, ActionManager.blocksEnabledForPlacing);
                BlockManager.lastFileWindow.setTitleTextMessage("*", false);
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
            BlockManager.lastFileWindow.saveFile();

            // save custom blocks
            FileManager.saveCustomBlockVariants();
        }
    }



    public class OpenWorkspace : Act
    {
        public void onCall(object data)
        {
            FileManager.loadWorkspace((string)data);

            Window3D filesWindow = WindowManager.spawnFilesWindow();
            filesWindow.setName((string)data);

            // load custom blocks
            BlockManager.loadCustomBlockVariants();
        }
    }



    public class CreateWorkspace : NameCreator, Act
    {
        public void onCall(object data)
        {
            parent = WindowManager.getWindowWithComponent<WorkspacesButtonManager>();
            ActionManager.callAction(ActionManager.CREATE_NAME, this);
        }

        public override void onFinishedNaming(bool success, string name)
        {
            if (!success) return;

            ActionManager.callAction(ActionManager.OPEN_WORKSPACE, name);
        }

        public override string getTextEntryWindowMessage()
        {
            return "Name Workspace:";
        }
    }



    public class OpenFile : Act
    {
        public void onCall(object data)
        {
            ReferenceTypeS rTS = FileManager.getSourceFile((string)data);
            Window3D spawned = WindowManager.spawnFileWindow();
            ((FileWindow)spawned).referenceTypeSave = rTS;

            // TODO: remove from list in files window
        }
    }



    public class CreateFile : NameCreator, Act
    {
        public void onCall(object data)
        {
            parent = WindowManager.getWindowWithComponent<FilesWindow>();
            ActionManager.callAction(ActionManager.CREATE_NAME, this);
        }

        public override void onFinishedNaming(bool success, string name)
        {
            if (!success) return;

            ReferenceTypeS rTS = FileManager.createSourceFile(name);
            if (rTS == null) return;

            Window3D spawned = WindowManager.spawnFileWindow();
            ((FileWindow)spawned).referenceTypeSave = rTS;

            // add to list in files window
            Window3D filesWindow = WindowManager.getWindowWithComponent<ButtonManager3D>();
            ButtonManager3D bM = filesWindow.GetComponent<ButtonManager3D>();
            bM.respawnButtons();
        }

        public override string getTextEntryWindowMessage()
        {
            return "Name File:";
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
