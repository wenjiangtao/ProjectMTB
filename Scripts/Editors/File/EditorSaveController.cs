using System.IO;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using MTB;

public class EditorSaveController
{
    private FileStream _fs;

    public void saveData()
    {
        if (!EditorSelectArea.Instance.hasData())
            return;
        SelectAreaParams saveParas = EditorSelectArea.Instance.save();

        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.dataPath + "Assets/Resources/Data/Preform";
        ofn.title = "载入文件";
        FileInfo fi;
        if (WindowDll.GetSaveFileName1(ofn))
            fi = new FileInfo(ofn.file);
        else
            return;

        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        _fs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _fs.Position = 0;
        byte[] lenght = new byte[4];
        Serialization.WriteIntToByteArr(lenght, saveParas.blockTypes.Length);
        _fs.Write(lenght, 0, lenght.Length);
        byte[] width = new byte[4];
        byte[] depth = new byte[4];
        byte[] height = new byte[4];
        Serialization.WriteIntToByteArr(width, saveParas.Width);
        Serialization.WriteIntToByteArr(depth, saveParas.Depth);
        Serialization.WriteIntToByteArr(height, saveParas.Height);
        _fs.Write(width, 0, width.Length);
        _fs.Write(depth, 0, depth.Length);
        _fs.Write(height, 0, height.Length);
        _fs.Write(saveParas.blockTypes, 0, saveParas.blockTypes.Length);
        _fs.Write(saveParas.extendIds, 0, saveParas.extendIds.Length);
        _fs.Close();
        _fs.Dispose();
        _fs = null;
    }

    public void saveBirthPos()
    {
        FileInfo fi = new FileInfo("Assets/Resources/Data/Preform/BirthPos");
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        _fs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _fs.Position = 0;

        byte[] width = new byte[4];
        byte[] depth = new byte[4];
        byte[] height = new byte[4];
        Vector3 pos = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
        Serialization.WriteIntToByteArr(width, (int)pos.x);
        Serialization.WriteIntToByteArr(depth, (int)pos.y);
        Serialization.WriteIntToByteArr(height, (int)pos.z);
        _fs.Write(width, 0, width.Length);
        _fs.Write(depth, 0, depth.Length);
        _fs.Write(height, 0, height.Length);
        _fs.Close();
        _fs.Dispose();
        _fs = null;
    }
}
