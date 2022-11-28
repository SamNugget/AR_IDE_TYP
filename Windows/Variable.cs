using System;

public class Variable
{
    public string name;
    public Block declarationBlock;
    public BlockManager.BlockVariant blockVariant;

    public Variable(string name, Block declarationBlock, BlockManager.BlockVariant blockVariant)
    {
        this.name = name;
        this.declarationBlock = declarationBlock;
        this.blockVariant = blockVariant;
    }
}
