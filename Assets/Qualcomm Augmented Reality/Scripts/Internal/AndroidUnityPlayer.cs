/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using System;
using UnityEngine;

/// <summary>
/// This class encapsulates functionality to detect various surface events
/// (size, orientation changed) and delegate this to native.
/// </summary>
class AndroidUnityPlayer : IAndroidUnityPlayer
{
#if UNITY_ANDROID
    // The Activity orientation is sometimes not correct when triggered immediately after the orientation change is
    // reported in Unity.
    // querying for the next 20 frames seems to yield the correct orientation eventually across all devices.
    private const int NUM_FRAMES_TO_QUERY_ORIENTATION = 20;

    private AndroidJavaObject mCurrentActivity;
    private AndroidJavaClass mJavaOrientationUtility;
    private ScreenOrientation mScreenOrientation = ScreenOrientation.Unknown;
    private int mFramesSinceLastOrientationReset;
    private int mScreenWidth = 0;
    private int mScreenHeight = 0;

    #region PUBLIC_METHODS

    /// <summary>
    /// - trigger onSurfaceCreated
    /// - intially set the screen orientation
    /// </summary>
    public void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            InitializeSurface();

    }

    /// <summary>
    /// check for screen orientation changes
    /// </summary>
    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (SurfaceUtilities.HasSurfaceBeenRecreated())
            {
                InitializeSurface();
            }
            else
            {
                // if Unity reports that the orientation has changed, reset the member variable
                // - this will trigger a check in Java for a few frames...
                if (Screen.orientation != mScreenOrientation)
                    ResetUnityScreenOrientation();

                CheckOrientation();

                if (mScreenWidth != Screen.width || mScreenHeight != Screen.height)
                {
                    mScreenWidth = Screen.width;
                    mScreenHeight = Screen.height;
                    SurfaceUtilities.OnSurfaceChanged(mScreenWidth, mScreenHeight);
                }
            }

            mFramesSinceLastOrientationReset++;
        }
    }


    // Java resources need to be explicitly disposed.
    public void Dispose()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            mCurrentActivity.Dispose();
            mCurrentActivity = null;

            mJavaOrientationUtility.Dispose();
            mJavaOrientationUtility = null;
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void InitializeSurface()
    {
        SurfaceUtilities.OnSurfaceCreated();

        AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mCurrentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (mCurrentActivity != null)
        {
            mJavaOrientationUtility = new AndroidJavaClass("com.qualcomm.QCARUnityPlayer.OrientationUtility");
        }

        ResetUnityScreenOrientation();
        CheckOrientation();

        mScreenWidth = Screen.width;
        mScreenHeight = Screen.height;
        SurfaceUtilities.OnSurfaceChanged(mScreenWidth, mScreenHeight);
    }

    private void ResetUnityScreenOrientation()
    {
        mScreenOrientation = Screen.orientation;
        mFramesSinceLastOrientationReset = 0;
    }

    private void CheckOrientation()
    {
        // check for the activity orientation for a few frames after it has changed in Unity
        if (mFramesSinceLastOrientationReset < NUM_FRAMES_TO_QUERY_ORIENTATION)
        {
            // mScreenOrientation remains at the value reported by Unity even when the acitivity reports a different one
            // otherwise the check for orientation changes will return true every frame.
            int correctScreenOrientation = (int)mScreenOrientation;

            if (mCurrentActivity != null)
            {
                // The orientation reported by Unity is not reliable on some devices (e.g. landscape right on the Nexus 10)
                // We query the correct orientation from the activity to make sure.
                int activityOrientation = mJavaOrientationUtility.CallStatic<int>("getSurfaceOrientation", mCurrentActivity);
                if (activityOrientation != 0)
                    correctScreenOrientation = activityOrientation;
            }

            SurfaceUtilities.SetSurfaceOrientation(correctScreenOrientation);
        }
    }

    #endregion // PRIVATE_METHODS

#else

    public void Start()
    {
    }

    public void Update()
    {
    }

    public void Dispose()
    {
    }

#endif
}
