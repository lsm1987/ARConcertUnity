/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// The QCARBehaviour class handles tracking and triggers native video
/// background rendering. The class updates all Trackables in the scene.
/// </summary>
[RequireComponent(typeof(Camera))]
public class QCARBehaviour : QCARAbstractBehaviour
{
    QCARBehaviour()
    {
        mAndroidUnityPlayer = new AndroidUnityPlayer();
    }
}
