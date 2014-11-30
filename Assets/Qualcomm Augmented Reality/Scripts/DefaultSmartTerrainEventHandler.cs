/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/


using UnityEngine;

/// <summary>
/// A default event handler that implements the ISmartTerrainEventHandler interface.
/// It uses a single Prop template that is used for every newly created prop.
/// </summary>
public class DefaultSmartTerrainEventHandler : MonoBehaviour, ISmartTerrainEventHandler
{
    #region PUBLIC_MEMBERS

    public PropBehaviour PropTemplate;

    #endregion // PUBLIC_MEMBERS



    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        SmartTerrainBehaviour behaviour = GetComponent<SmartTerrainBehaviour>();
        if (behaviour)
        {
            behaviour.RegisterSmartTerrainEventHandler(this);
        }
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region ISmartTerrainEventHandler_IMPLEMENTATION

    /// <summary>
    /// Called when the smart terrain system has finished initializing
    /// </summary>
    public void OnInitialized(SmartTerrainInitializationInfo initializationInfo)
    {
    }

    /// <summary>
    /// Called when the geometry of a surface has been updated
    /// </summary>
    public void OnSurfaceUpdated(SurfaceAbstractBehaviour surfaceBehaviour)
    {
    }

    /// <summary>
    /// Called when a smart terrain prop has been created
    /// </summary>
    public void OnPropCreated(Prop prop)
    {
        var manager = TrackerManager.Instance.GetStateManager().GetSmartTerrainManager();
        manager.AssociateProp(PropTemplate, prop);
    }

    /// <summary>
    /// Called when the geometry of a smart terrain prop has been updated
    /// </summary>
    public void OnPropUpdated(Prop prop)
    {
    }

    /// <summary>
    /// Called when a smart terrain prop has been destroyed
    /// </summary>
    public void OnPropDeleted(Prop prop)
    {
    }

    #endregion // ISmartTerrainEventHandler_IMPLEMENTATION
}



