using FileManagement;

public class OpenFile : Act
{
    public OpenFile(char c) : base(c) { }

    public override void onCall(object data)
    {
        ReferenceTypeS rTS = FileManager.getSourceFile((string)data);
        Window3D spawned = WindowManager.spawnFileWindow();
        ((ReferenceTypeWindow)spawned).referenceTypeSave = rTS;

        // TODO: remove from list in files window
    }
}
