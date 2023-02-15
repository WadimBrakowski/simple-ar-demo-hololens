using Dummiesman;
using System.IO;
using UnityEngine;
using HoloLens_2_Scene_Setuper;

public class ObjFromFile : MonoBehaviour
{
    [SerializeField] private AssemblyGroup _assemblyGroup;
    //string objPath = "C:\\Users\\wadim\\Dropbox\\NX\\obj_vereinfacht\\BG\\assembly1_1.obj";
    string objPath = "F:\\cad\\ar\\test objects\\another_test.obj";
    //string objPath = "F:\\cad\\ar\\test objects\\cube_assembly.obj";
    string error = string.Empty;
    GameObject loadedObject;
    

    void OnGUI() {
        objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

        GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
        if(GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
        {
            //file path
            if (!File.Exists(objPath))
            {
                error = "File doesn't exist.";
            }else{
                if(loadedObject != null)            
                    Destroy(loadedObject);
                loadedObject = new OBJLoader().Load(objPath);
                error = string.Empty;
                _assemblyGroup.LoadedObjectFile = loadedObject;
            }
        }

        if(!string.IsNullOrWhiteSpace(error))
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
            GUI.color = Color.white;
        }
    }
}
