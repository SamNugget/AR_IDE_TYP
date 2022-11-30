using System;

public class Variable
{
    public string name;
    //public Block scopeBlock;
    public Block declarationBlock;
    public BlockManager.BlockVariant blockVariant;

    public Variable(string name, BlockManager.BlockVariant blockVariant)
    {
        this.name = name;
        this.blockVariant = blockVariant;

        declarationBlock = null;
    }
}
