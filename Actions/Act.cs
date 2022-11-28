public abstract class Act
{
    private char symbol;
    public char getSymbol() { return symbol; }

    public Act(char symbol)
    {
        this.symbol = symbol;
    }

    public abstract void onCall(object data);
}
