using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using TMPro;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Net;
using UnityEngine.Networking;

public class ARObjFromFile : MonoBehaviour
{

    //string objPath = "F:\\CAD\\ar\\test objects\\head.obj";

    string objPath;


    string error = string.Empty;
    GameObject loadedObject;
    MeshCollider meshCollider;
    Mesh mesh;
    [SerializeField] private GameObject workSpacePrefab;
    [SerializeField] private Vector3 workPlacePosition =  new Vector3(0, -0.3f, 1.3f);

    [SerializeField] TextMeshPro pathText;
    string consoleText = "None";
    string text;
#if ENABLE_WINMD_SUPPORT
    Windows.Storage.Streams.IRandomAccessStream randomAccessStream;
#endif
    Stream stream;
    private MemoryStream textStream;


    private void Start()
    {
        pathText.text = consoleText;
    }


    public void OpenBrowser()
    {
#if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            //Windows.Storage.OutputTextBlock.Text = "";

            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Objects3D;
            openPicker.FileTypeFilter.Add(".obj");
            //openPicker.FileTypeFilter.Add(".jpeg");
            //openPicker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // The StorageFile has read/write access to the picked file.
                // See the FileAccess sample for code that uses a StorageFile to read and write.

                /*using (randomAccessStream = await file.OpenReadAsync())
                {
                    using (stream = randomAccessStream.AsStreamForRead())
                    {
                        if (loadedObject != null) Destroy(loadedObject);
                        loadedObject = new OBJLoader().Load(stream);
                    }
                }*/

                randomAccessStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                ulong size = randomAccessStream.Size;

                stream = randomAccessStream.AsStream();

                using (var inputStream = randomAccessStream.GetInputStreamAt(0))
                {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                    {
                        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                        text = dataReader.ReadString(numBytesLoaded);
                        //textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                    }
                }

                consoleText = text;
                objPath = file.Path;




            }
            else
            {
                consoleText = "Operation cancelled.";


            }
        }, true);

        pathText.text = consoleText;




        //workSpacePrefab = Instantiate(workSpacePrefab, workPlacePosition, Quaternion.identity);

        //ConfigGameObject();


#endif
    }

    public void LoadOBJ()
    {
        try
        {
            if (!File.Exists(objPath))
            {
                pathText.text = "File doesn't exist.";
            }
            else
            {
                if (loadedObject != null)
                    Destroy(loadedObject);
                loadedObject = new OBJLoader().Load("C:\\Data\\User\\holo.ar%40outlook.de\\3D%20Objects\\head.obj");
                pathText.text = loadedObject.name+ " is loaded!1";
                error = string.Empty;
                loadedObject.transform.position= workPlacePosition;
            }
        }
        catch (Exception e)
        {
            pathText.text = e.ToString();
        }
    }

    public void LoadObjFromStream()
    {
        try
        {

            //create stream and load
            textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            Debug.Log(text);
            loadedObject = new OBJLoader().Load(textStream);
        }
        catch (Exception e)
        {
            pathText.text = e.ToString();
        }
    }

    void ConfigGameObject()
    {
        //loadedObject.transform.localScale = new Vector3(.1f, .1f, .1f);
        loadedObject.transform.parent = workSpacePrefab.transform;
        loadedObject.transform.position = workSpacePrefab.transform.position;
        SetupMeshForLoadedObject();

        AddARComponentsToLoadedObject();
    }

    private void SetupMeshForLoadedObject()
    {
        meshCollider = loadedObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        mesh = loadedObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        meshCollider.sharedMesh = mesh;
    }

    private void AddARComponentsToLoadedObject()
    {
        loadedObject.AddComponent<ConstraintManager>();
        loadedObject.AddComponent<ObjectManipulator>();
        loadedObject.AddComponent<NearInteractionGrabbable>();
    }

}
