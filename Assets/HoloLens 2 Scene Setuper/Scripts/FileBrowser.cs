using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace ObjectPicker
{
    public class FileBrowser : MonoBehaviour
    {

        public string path;
        /*string sourceDirectory = @"F:\CAD\ar\test objects";
        string archiveDirectory = @"C:\archive";

        public void BrowseFiles()
        {
            var objFiles = Directory.EnumerateFiles(sourceDirectory, "*.obj");

            foreach (string currentFile in objFiles)
            {
                Debug.Log(currentFile);
            }
        }*/

        public void SelectFolder()
        {
#if ENABLE_WINMD_SUPPORT
    UnityEngine.WSA.Application.InvokeOnUIThread(async () =>  
    {  
        var folderPicker = new Windows.Storage.Pickers.FolderPicker();  
        folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;  
        folderPicker.FileTypeFilter.Add("*");  
  
        Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();  
        if (folder != null)  
        {  
            // Application now has read/write access to all contents in the picked folder  
            // (including other sub-folder contents)  
            Windows.Storage.AccessCache.StorageApplicationPermissions.  
            FutureAccessList.AddOrReplace("PickedFolderToken", folder);  
        }  
    }, false);  
#endif
        }

        public void SelectFile()
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
                    //Windows.Storage.OutputTextBlock.Text = "Picked photo: " + file.Name;

                    //return file.Path;


                }
                else
                {
                    //Windows.Storage.OutputTextBlock.Text = "Operation cancelled.";
                    
                }
            }, false);
#endif
            
            
        }
    }
}
