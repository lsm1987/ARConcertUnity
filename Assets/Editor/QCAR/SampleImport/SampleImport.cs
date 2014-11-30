/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using System.IO;
using UnityEditor;
using UnityEngine;


public class SampleImport : AssetPostprocessor
{
    // This method is called by Unity whenever assets are updated (deleted,
    // moved or added)
    public static void OnPostprocessAllAssets(string[] importedAssets,
                                              string[] deletedAssets,
                                              string[] movedAssets,
                                              string[] movedFromAssetPaths)
    {
        // Set the Unity version for internal use
        string path = Path.Combine(Application.dataPath, "StreamingAssets/QCAR");
        QCARUnity.SetUnityVersion(path);
    }
}
