using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjFromStream : MonoBehaviour {
	void Start () {
        //make www
        //var www = new WWW("https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj");
        var www = new WWW("file:///F:/CAD/ar/test%20objects/head.obj");
        while (!www.isDone)
            System.Threading.Thread.Sleep(1);
        
        //create stream and load
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
        Debug.Log(www.text);
        var loadedObj = new OBJLoader().Load(textStream);
	}
}
