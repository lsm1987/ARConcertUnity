/*============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
============================================================================*/

package com.qualcomm.QCARUnityPlayer;

import java.io.File;
import java.io.FileReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.io.IOException;
import java.lang.reflect.Constructor;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.pm.ActivityInfo;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.BitmapFactory.Options;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;

import com.qualcomm.QCAR.QCAR;
import com.unity3d.player.UnityPlayer;

/** This class handles QCAR initialization before the UnityPlayer is initalized.
 * */
public class QCARPlayerSharedActivity
{
    // Application status constants:
    private static final int APPSTATUS_UNINITED         = -1;
    private static final int APPSTATUS_INIT_APP         = 0;
    private static final int APPSTATUS_INIT_QCAR        = 1;
    private static final int APPSTATUS_INIT_UNITY       = 2;
    private static final int APPSTATUS_INITED           = 3;
    
    // Name of the native dynamic libraries to load:
    private static final String NATIVE_LIB_UNITYPLAYER = "QCARUnityPlayer";
    private static final String NATIVE_LIB_QCARWRAPPER = "QCARWrapper";
    private static final String NATIVE_LIB_QCAR = "Vuforia";
    
    // The current application status
    private int mAppStatus = APPSTATUS_UNINITED;
    
    // The async tasks to initialize the QCAR SDK 
    private InitQCARTask mInitQCARTask;
    private InitUnityTask mInitUnityTask;
    
    // An object used for synchronizing QCAR initialization, dataset loading and
    // the Android onDestroy() life cycle event. If the application is destroyed
    // while a data set is still being loaded, then we wait for the loading
    // operation to finish before shutting down QCAR.
    private Object mShutdownLock = new Object();   
    
    // QCAR initialization flags
    private int mQCARFlags = 0;
    
    // Unity initializer object
    private IUnityInitializer mUnityInitializer;
    
    // Activity object
    private Activity mActivity;
    
    
    /** Load native libraries stored in "libs/armeabi*" */
    static
    {
        loadLibrary(NATIVE_LIB_QCAR);
        loadLibrary(NATIVE_LIB_QCARWRAPPER);
        loadLibrary(NATIVE_LIB_UNITYPLAYER);
    }
    
    // Set an error code if initialization was unsuccessful
    public native void setErrorCode(int errorCode);    
	
	public interface IUnityInitializer
	{
		void InitializeUnity();
	}
    
    /** An async task to initialize QCAR asynchronously. */
    private class InitQCARTask extends AsyncTask<Activity, Integer, Boolean>
    {   
        // Initialize with invalid value
        private int mProgressValue = -1;
        
        protected Boolean doInBackground(Activity... params)
        {
            // Prevent the onDestroy() method to overlap with initialization:
            synchronized (mShutdownLock)
            {
                if (QCAR.isInitialized())
                {
                    DebugLog.LOGD("InitQCARTask::doInBackground: forcing QCAR deinitialization");
                    QCAR.deinit();
                }
                
                QCAR.setInitParameters((params.length > 0 ? params[0] : null),
                                            mQCARFlags);
                
                do
                {
                    // QCAR.init() blocks until an initialization step is complete,
                    // then it proceeds to the next step and reports progress in
                    // percents (0 ... 100%)
                    // If QCAR.init() returns -1, it indicates an error.
                    // Initialization is done when progress has reached 100%.
                    mProgressValue = QCAR.init();
                    
                    // Publish the progress value:
                    publishProgress(mProgressValue);
                    
                    // We check whether the task has been canceled in the meantime
                    // (by calling AsyncTask.cancel(true))
                    // and bail out if it has, thus stopping this thread.
                    // This is necessary as the AsyncTask will run to completion
                    // regardless of the status of the component that started is.
                } while (!isCancelled() && mProgressValue >= 0 && mProgressValue < 100);
                
                return (mProgressValue > 0);
            }
        }
        
        
        protected void onProgressUpdate(Integer... values)
        {
            // Do something with the progress value "values[0]", e.g. update
            // splash screen, progress bar, etc.
        }
        
        
        protected void onPostExecute(Boolean result)
        {
            // Done initializing QCAR, proceed to next application
            // initialization status:
            if (result)
            {
                DebugLog.LOGI("QCAR initialization successful");
                updateApplicationStatus(APPSTATUS_INIT_UNITY);
            }
            else
            {
                DebugLog.LOGE("QCAR initialization failed");
                mUnityInitializer.InitializeUnity(); // We still initialize the Unity player
                setErrorCode(mProgressValue);
            }
        }
    }
    
    
    /** An async task to load the tracker data asynchronously. */
    private class InitUnityTask extends AsyncTask<Void, Integer, Boolean>
    {
        protected Boolean doInBackground(Void... params)
        {
            return true;
        }
        
        
        protected void onPostExecute(Boolean result)
        {
            DebugLog.LOGI("Initializing UnityPlayer");
            // Init Unity Player:
            mUnityInitializer.InitializeUnity();
            
            // Done loading the tracker, update application status: 
            updateApplicationStatus(APPSTATUS_INITED);
        }
    }
    
    
    /** Called when the activity first starts or the user navigates back
     * to an activity. */
    public void onCreate(Activity activity, int gles_mode, IUnityInitializer unityInitializer)
    {
		mActivity = activity;
		mUnityInitializer = unityInitializer;
        
        // Set the QCAR initialization flags:
        mQCARFlags = (gles_mode == 1) ? QCAR.GL_11 : QCAR.GL_20;
        
        // Update the application status to start initializing application
        // Note that QCAR may already be initialized (Unity apps call onCreate each
        // time the app is resumed from the main application list)
        if (QCAR.isInitialized())
        {
            // Let QCAR know that the Activity instance has changed.
            QCAR.setInitParameters(mActivity, mQCARFlags);
            
            updateApplicationStatus(APPSTATUS_INIT_UNITY);
        }
        else
        {
            updateApplicationStatus(APPSTATUS_INIT_APP);
        }
    }    
    
    
    /** Called when the activity will start interacting with the user.*/
    public void onResume()
    {
        // QCAR-specific resume operation
        QCAR.onResume();
    }
    
    
    /** Called when the system is about to start resuming a previous activity.*/
    public void onPause()
    {        
        // QCAR-specific pause operation
        QCAR.onPause();
    }
    
    
    /** The final call you receive before your activity is destroyed.*/
    public void onDestroy()
    {
        // Cancel potentially running tasks
        if (mInitQCARTask != null &&
            mInitQCARTask.getStatus() != InitQCARTask.Status.FINISHED)
        {
            mInitQCARTask.cancel(true);
            mInitQCARTask = null;
        }
        
        if (mInitUnityTask != null &&
            mInitUnityTask.getStatus() != InitUnityTask.Status.FINISHED)
        {
            mInitUnityTask.cancel(true);
            mInitUnityTask = null;
        }
        
        // Ensure that all asynchronous operations to initialize QCAR and loading
        // the tracker datasets do not overlap:
        synchronized (mShutdownLock)
        {
            // Deinitialize QCAR SDK
            QCAR.deinit();
        }
    }
    
    
    /** NOTE: this method is synchronized because of a potential concurrent
     * access by QCARSampleActivity::onResume() and InitQCARTask::onPostExecute(). */
    private synchronized void updateApplicationStatus(int appStatus)
    {
        // Exit if there is no change in status
        if (mAppStatus == appStatus)
            return;
        
        // Store new status value      
        mAppStatus = appStatus;
        
        // Execute application state-specific actions
        switch (mAppStatus)
        {
            case APPSTATUS_INIT_APP:
                // Initialize application elements that do not rely on QCAR
                // initialization
                initApplication();
                
                // Proceed to next application initialization status
                updateApplicationStatus(APPSTATUS_INIT_QCAR);
                break;
                
            case APPSTATUS_INIT_QCAR:
                // Initialize QCAR SDK asynchronously to avoid blocking the
                // main (UI) thread.
                // This task instance must be created and invoked on the UI
                // thread and it can be executed only once!
                try
                {
                    mInitQCARTask = new InitQCARTask();
                    mInitQCARTask.execute(mActivity);
                }
                catch (Exception e)
                {
                    DebugLog.LOGE("Initializing QCAR SDK failed");
                }
                break;
                
            case APPSTATUS_INIT_UNITY:
                // Initialize Unity
                //
                // This task instance must be created and invoked on the UI
                // thread and it can be executed only once!
                try
                {
                    mInitUnityTask = new InitUnityTask();
                    mInitUnityTask.execute();
                }
                    catch (Exception e)
                {
                    DebugLog.LOGE("Initializing Unity failed");
                }
                break;
                
            case APPSTATUS_INITED:
                // Hint to the virtual machine that it would be a good time to
                // run the garbage collector.
                //
                // NOTE: This is only a hint. There is no guarantee that the
                // garbage collector will actually be run.
                System.gc();
                
                break;
                
            default:
                throw new RuntimeException("Invalid application state");
        }
    }
    
    
    /** Native initialization prior to QCAR initialization */
    private native void initApplicationNative(int major, int minor, int change);
    
    
    /** Set the Unity version number. */
    private void initApplication()
    {        
        // Read in the Unity version either from the deployed asset file or from the cache and parse it.
        int major=0;
        int minor=0;
        int change=0;
        
        try
        {
        	String version = ReadUnityVersion();
        	String[] versionSplit = version.split("\\.");
        	major = Integer.parseInt(versionSplit[0]);
        	minor = Integer.parseInt(versionSplit[1]);
        	change = Integer.parseInt(versionSplit[2]);
        }
        catch (Exception e)
        {
            DebugLog.LOGW("Could not interpret unity version number: " + e.getMessage());
        }
        
        // Carry out native initialization:
        initApplicationNative(major, minor, change);
    }
    
    private String ReadUnityVersion()
    {
    	String unityVersion = "0.0.0";
    	// try to read from static asset file:
    	try
    	{
    		AssetManager assetManager = mActivity.getAssets();
    		InputStreamReader inputStreamReader = new InputStreamReader(assetManager.open("QCAR/unity.txt"));
        	unityVersion = ReadFileContents(inputStreamReader);
            DebugLog.LOGD("Found unity version file in activity assets.");
    	}
        catch (Exception e)
        {
            DebugLog.LOGD("Could not find unity version file in activity assets: " + e.getMessage());
        }
    	
    	// try to read from internal dynamic cache:
    	try
    	{
    		File filesDir = mActivity.getFilesDir();
    		FileReader fileReader = new FileReader(filesDir.getAbsolutePath() + "/unity.txt");
    		unityVersion = ReadFileContents(fileReader);
            DebugLog.LOGD("Found unity version file in internal file dir.");
    	}
    	catch (Exception e)
    	{
            DebugLog.LOGD("Could not find unity version file in internal file dir: " + e.getMessage());
    	}
    	
    	// try to read from internal dynamic cache:
    	try
    	{
    		File filesDir = mActivity.getExternalFilesDir("");
    		FileReader fileReader = new FileReader(filesDir.getAbsolutePath() + "/unity.txt");
    		unityVersion = ReadFileContents(fileReader);
            DebugLog.LOGD("Found unity version file in external file dir.");
    	}
    	catch (Exception e)
    	{
            DebugLog.LOGD("Could not find unity version file in external file dir: " + e.getMessage());
    	}
    	
    	if (unityVersion.equals("0.0.0"))
    	{
            DebugLog.LOGW("No unity version file found this time. This should only happen on first application start in some rare cases. A version file will be generated for the next application start");
    	}
    	
    	return unityVersion;
    }
    
    static private String ReadFileContents(InputStreamReader inputStreamReader) throws IOException
    {
		BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
		String ret = bufferedReader.readLine();
		bufferedReader.close();
		return ret;
    }    
    
    /** A helper for loading native libraries stored in "libs/armeabi*". */
    public static boolean loadLibrary(String nLibName)
    {
        try
        {
            System.loadLibrary(nLibName);
            //DebugLog.LOGI("Native library lib" + nLibName + ".so loaded");
            return true;
        }
        catch (UnsatisfiedLinkError ulee)
        {
            DebugLog.LOGE("The library lib" + nLibName +
                            ".so could not be loaded: " + ulee.toString());
        }
        catch (SecurityException se)
        {
            DebugLog.LOGE("The library lib" + nLibName +
                            ".so was not allowed to be loaded");
        }
        
        return false;
    }
}