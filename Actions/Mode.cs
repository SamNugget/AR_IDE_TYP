public abstract class Mode : Act
{
    private bool multiSelect;
    public bool getMultiSelect() { return multiSelect; }

    public Mode(char c, bool multiSelect) : base(c)
    {
        this.multiSelect = multiSelect;
    }

    public abstract void onSelect(object data);

    public abstract void onDeselect();

    public abstract string getToolsWindowMessage();
}
