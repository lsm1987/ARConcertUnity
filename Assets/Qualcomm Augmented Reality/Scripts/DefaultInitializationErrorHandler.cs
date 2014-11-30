/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using UnityEngine;

/// <summary>
/// A custom handler that implements the IQCARErrorHandler interface.
/// </summary>
public class DefaultInitializationErrorHandler : MonoBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES

    private string mErrorText = "";
    private bool mErrorOccurred = false;

    private const string WINDOW_TITLE = "QCAR Initialization Error";

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        // Check for an initialization error on start.
        QCARUnity.InitError errorCode = QCARUnity.CheckInitializationError();
        if (errorCode != QCARUnity.InitError.INIT_SUCCESS)
        {
            SetErrorCode(errorCode);
            SetErrorOccurred(true);
        }
    }


    void OnGUI()
    {
        // On error, create a full screen window.
        if (mErrorOccurred)
            GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height),
                                   DrawWindowContent, WINDOW_TITLE);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    private void DrawWindowContent(int id)
    {
        // Create text area with a 10 pixel distance from other controls and
        // window border.
        GUI.Label(new Rect(10, 25, Screen.width - 20, Screen.height - 95),
                    mErrorText);

        // Create centered button with 50/50 size and 10 pixel distance from
        // other controls and window border.
        if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height - 60,
                                    150, 50), "Close"))
            Application.Quit();
    }

    // Implementation of the IQCARErrorHandler function which sets the
    // error message.
    private void SetErrorCode(QCARUnity.InitError errorCode)
    {
        switch (errorCode)
        {
            case QCARUnity.InitError.INIT_DEVICE_NOT_SUPPORTED:
                mErrorText =
                      "Failed to initialize QCAR because this device is not " +
                      "supported.";

                break;
            case QCARUnity.InitError.INIT_ERROR:
                mErrorText = "Failed to initialize QCAR.";
                break;
        }
    }


    // Implementation of the IQCARErrorHandler function which sets if an
    // error has been thrown.
    private void SetErrorOccurred(bool errorOccurred)
    {
        mErrorOccurred = errorOccurred;

        // We set the clear mode of the camera to solid. Otherwise the Window is
        // messed up.
        if (errorOccurred)
            this.camera.clearFlags = CameraClearFlags.SolidColor;
    }

    #endregion // PRIVATE_METHODS
}
