public abstract class Mode : Act
{
    public Mode(char c) : base(c) { }

    public abstract void onSelect(object data);

    public abstract void onDeselect();

    public abstract string getToolsWindowMessage();
}
