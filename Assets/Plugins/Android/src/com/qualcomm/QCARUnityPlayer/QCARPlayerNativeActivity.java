/*============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
============================================================================*/

package com.qualcomm.QCARUnityPlayer;

import android.app.Dialog;
import android.app.NativeActivity;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import java.lang.reflect.Method;

import com.unity3d.player.UnityPlayer;

/** This custom NativeActivity shows how to initialize QCAR together with Unity from an activity.
* 	If you need to integrate another native library, you can modifiy this code and 
*	compile it to a JAR file to replace QCARUnityPlayer.jar in Assets/Plugins/Android
* */
public class QCARPlayerNativeActivity extends NativeActivity
{
    private QCARPlayerSharedActivity mQCARShared;
	protected UnityPlayer mUnityPlayer;		// don't change the name of this variable; referenced from native code

	// UnityPlayer.init() should be called before attaching the view to a layout - it will load the native code.
	// UnityPlayer.quit() should be the last thing called - it will unload the native code.
	protected void onCreate (Bundle savedInstanceState)
	{
		requestWindowFeature(Window.FEATURE_NO_TITLE);

        mUnityPlayer = new UnityPlayer(this);		
		
		super.onCreate(savedInstanceState);
		
		// initialize QCAR asynchronously
		mQCARShared = new QCARPlayerSharedActivity();
		int gles_mode = mUnityPlayer.getSettings().getInt("gles_mode", 1);
        mQCARShared.onCreate(this, gles_mode, new UnityInitializer());
		
		getWindow().takeSurface(null);
		setTheme(android.R.style.Theme_NoTitleBar_Fullscreen);
		getWindow().setFormat(PixelFormat.RGB_565);
		
		if (mUnityPlayer.getSettings ().getBoolean ("hide_status_bar", true))
			getWindow ().setFlags (WindowManager.LayoutParams.FLAG_FULLSCREEN,
			                       WindowManager.LayoutParams.FLAG_FULLSCREEN);
		
	}
	
	private class UnityInitializer implements QCARPlayerSharedActivity.IUnityInitializer
	{
		public void InitializeUnity()
		{
			// Test for init method to see if this is 4.5 or earlier
			try
			{
				Class<?> cls = Class.forName("com.unity3d.player.UnityPlayer");
				Method method = cls.getMethod("init", Integer.TYPE, Boolean.TYPE);
				
				int glesMode = mUnityPlayer.getSettings().getInt("gles_mode", 1);
				boolean trueColor8888 = false;
				method.invoke(mUnityPlayer, glesMode, trueColor8888);
			}
			catch (Exception e) { }

			View playerView = mUnityPlayer.getView();
			setContentView(playerView);
			playerView.requestFocus();
		}
	}
	
	protected void onDestroy ()
	{
        mQCARShared.onDestroy();
		mUnityPlayer.quit();
		super.onDestroy();
	}

	// onPause()/onResume() must be sent to UnityPlayer to enable pause and resource recreation on resume.
	protected void onPause()
	{
		super.onPause();
		mUnityPlayer.pause();
        mQCARShared.onPause();
	}
	protected void onResume()
	{
		super.onResume();
		mQCARShared.onResume();
		mUnityPlayer.resume();
	}
	public void onConfigurationChanged(Configuration newConfig)
	{
		super.onConfigurationChanged(newConfig);
		mUnityPlayer.configurationChanged(newConfig);
	}
	public void onWindowFocusChanged(boolean hasFocus)
	{
		super.onWindowFocusChanged(hasFocus);
		mUnityPlayer.windowFocusChanged(hasFocus);
	}
	public boolean dispatchKeyEvent(KeyEvent event)
	{
		if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
			return mUnityPlayer.onKeyMultiple(event.getKeyCode(), event.getRepeatCount(), event);
		return super.dispatchKeyEvent(event);
	}
}
