[System.Serializable]
public class FieldS
{
    public Block fieldBlock;

    public FieldS(Block fieldBlock)
    {
        this.fieldBlock = fieldBlock;
    }

    public string getCode()
    {
        if (fieldBlock == null)
            return null;

        return fieldBlock.getBlockText(true);
    }
}
