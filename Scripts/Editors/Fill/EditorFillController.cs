using UnityEngine;
using MTB;

public class EditorFillController : MonoBehaviour, IEditorController
{
    private bool enableMark;

    public MTBEditorModeType editorType()
    {
        return MTBEditorModeType.FILL;
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            if (GUI.Button(new Rect(w - 100, h / 2, 100, 40), "填充手持物块"))
            {
                Item item = ItemManager.Instance.GetItem(HasActionObjectManager.Instance.playerManager.getMyPlayer().GetComponent<PlayerAttributes>().handMaterialId);
                if (item == null) fillBlock(BlockType.Air, 0);
                else
                    fillBlock((BlockType)item.sceneBlockType, item.sceneBlockExtendId);
            }
            if (GUI.Button(new Rect(w - 100, h / 2 - 50, 100, 40), "填充空气"))
            {
                fillBlock(BlockType.Air, 0);
            }
        }
    }

    private void fillBlock(BlockType type, byte exid)
    {
        EditorSelectArea.Instance.fill(type, exid);
    }

    public void enable()
    {
        if (!EditorSelectArea.Instance.hasData())
            return;
        enableMark = true;
    }
    public void disEnable()
    {
        enableMark = false;
    }
    public void cancel() { }

    public void setState(int state)
    {
    }
}
