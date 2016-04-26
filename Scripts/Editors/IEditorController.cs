
public interface IEditorController
{
    MTBEditorModeType editorType();
    void setState(int state);
    void cancel();
    void enable();
    void disEnable();
}
