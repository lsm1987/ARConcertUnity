/*============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Qualcomm Confidential and Proprietary
============================================================================*/


class QCARUnityPlayer
{
public:
    static QCARUnityPlayer& getInstance();
    static void destroy();
    
    void testLog();
    
    void QCARInit(const char* uiScreenOrientation);
    void QCARPause(bool pause);
    
    // loads the QCAR trackers
    void QCARLoadTracker();
    
    // notify QCAR that the rendering surface has been created
    // with given width and height
    void QCARNotifyCreated(int width, int height);

    void QCARSetOrientation(int orientation);

    
protected:
    QCARUnityPlayer();
    virtual ~QCARUnityPlayer();
    
    // deinitialises QCAR and stops logging
    void QCARCleanup();
    
    int getRotationFlag(const char* uiScreenOrientation);
    
    static QCARUnityPlayer* instance;
};

