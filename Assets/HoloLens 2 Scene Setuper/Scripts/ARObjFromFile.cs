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
using UnityEngine.SceneManagement;

namespace HoloLens_2_Scene_Setuper
{
    public class ARObjFromFile : MonoBehaviour
    {
        [SerializeField] private AssemblyGroup _assemblyGroup;

        string objPath = "F:\\cad\\ar\\test objects\\another_test.obj";


        string error = string.Empty;
        GameObject loadedObject;
        MeshCollider meshCollider;
        Mesh mesh;
        
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
                    }
                }

            }
            else
            {
                consoleText = "Operation cancelled.";


            }
        }, true);
            
            LoadObjFromStream();


#endif
        }
#if UNITY_EDITOR
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
                    loadedObject = new OBJLoader().Load(objPath);
                    pathText.text = loadedObject.name + " is loaded!1";
                    error = string.Empty;
                    _assemblyGroup.LoadedObjectFile = loadedObject;
                }
            }
            catch (Exception e)
            {
                if (pathText.IsActive())
                {
                    pathText.text = e.ToString();
                }
                
            }
        }
#endif
        public void LoadObjFromStream()
        {
            try
            {

                //create stream and load
                textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                Debug.Log(text);
                loadedObject = new OBJLoader().Load(textStream);
                _assemblyGroup.LoadedObjectFile = loadedObject;
            }
            catch (Exception e)
            {
                if (pathText.IsActive())
                {
                    pathText.text = e.ToString();
                }
            }
        }

        public void ReloadScene()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
