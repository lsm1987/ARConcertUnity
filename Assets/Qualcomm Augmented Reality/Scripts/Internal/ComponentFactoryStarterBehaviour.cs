/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

/// <summary>
/// Small utility behaviour to create an instance of the VuforiaBehaviourComponentFactory at runtime before anything is initialized.
/// </summary>
public class ComponentFactoryStarterBehaviour : MonoBehaviour
{
    void Awake()
    {
        BehaviourComponentFactory.Instance = new VuforiaBehaviourComponentFactory();
    }
}