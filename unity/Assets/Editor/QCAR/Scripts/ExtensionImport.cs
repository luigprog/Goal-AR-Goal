/*==============================================================================
Copyright (c) 2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using UnityEngine;
using UnityEditor;

namespace Vuforia.EditorClasses
{
    [InitializeOnLoad]
    public static class ExtensionImport
    {
        private static readonly string VUFORIA_ANDROID_SETTINGS = "VUFORIA_ANDROID_SETTINGS";
        private static readonly string VUFORIA_IOS_SETTINGS = "VUFORIA_IOS_SETTINGS";
        
        static ExtensionImport() 
        {
            EditorApplication.update += UpdatePlayerSettings;
        }
        
        static void UpdatePlayerSettings()
        {
            // Unregister callback (executed only once)
            EditorApplication.update -= UpdatePlayerSettings;

            BuildTargetGroup androidBuildTarget = BuildTargetGroup.Android;

#if UNITY_5_1 || UNITY_5_0
            BuildTargetGroup iOSBuildTarget = BuildTargetGroup.iOS;
#else
            BuildTargetGroup iOSBuildTarget = BuildTargetGroup.iPhone;
#endif

            string androidSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(androidBuildTarget);
            androidSymbols = androidSymbols ?? "";
            if (!androidSymbols.Contains(VUFORIA_ANDROID_SETTINGS)) 
            {
                if (PlayerSettings.Android.targetDevice != AndroidTargetDevice.ARMv7)
                {
                    Debug.Log("Setting Android target device to ARMv7");
                    PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
                }

#if UNITY_5_1 || UNITY_5_0
                if (PlayerSettings.Android.androidTVCompatibility)
                {
                    // Disable Android TV compatibility, as this is not compatible with
                    // portrait, portrait-upside-down and landscape-right orientations.
                    Debug.Log("Disabling Android TV compatibility.");
                    PlayerSettings.Android.androidTVCompatibility = false;
                }
#endif
                // check if Graphics API is set to OpenGL ES 2.0
                if (PlayerSettings.targetGlesGraphics == TargetGlesGraphics.OpenGLES_3_0)
                {
                    Debug.Log("Setting Graphics API to OpenGL ES 2.0.");
                    PlayerSettings.targetGlesGraphics = TargetGlesGraphics.OpenGLES_2_0;
                    
                    if (PlayerSettings.targetGlesGraphics != TargetGlesGraphics.OpenGLES_2_0)
                    {
                        Debug.LogWarning("Failed to set Graphics API to OpenGL ES 2.0. Please make sure to set this manually in the " +
                                         "player settings, for compatibility with Vuforia.");
                    }
                }

                // Here we set the scripting define symbols for Android
                // so we can remember that the settings were set once.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                                                                 androidSymbols + ";" + VUFORIA_ANDROID_SETTINGS);
            }

            string iOSSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(iOSBuildTarget);
            iOSSymbols = iOSSymbols ?? "";
            if (!iOSSymbols.Contains(VUFORIA_IOS_SETTINGS))
            {
#if UNITY_5_1 || UNITY_5_0 || UNITY_4_9 || UNITY_4_8 || UNITY_4_7 || (UNITY_4_6 && !UNITY_4_6_1 && !UNITY_4_6_2)
                // check if Graphics API for iOS is set to Metal or Automatic
                if ((PlayerSettings.targetIOSGraphics == TargetIOSGraphics.Automatic) ||
                    (PlayerSettings.targetIOSGraphics == TargetIOSGraphics.Metal))
                {
                    Debug.Log("Setting iOS Graphics API to OpenGL ES 2.0, Vuforia does not support Metal yet.");
                    PlayerSettings.targetIOSGraphics = TargetIOSGraphics.OpenGLES_2_0;
                    
                    if (PlayerSettings.targetIOSGraphics != TargetIOSGraphics.OpenGLES_2_0)
                    {
                        Debug.LogWarning("Failed to set iOS Graphics API to OpenGL ES 2.0. Please make sure to set this manually in the "+
                                         "player settings, Vuforia does not support the Metal graphics API yet.");
                    }
                }
#endif

#if INCLUDE_IL2CPP
                int scriptingBackend = PlayerSettings.GetPropertyInt("ScriptingBackend", iOSBuildTarget);
                if (scriptingBackend != (int) ScriptingImplementation.IL2CPP)
                {
                    Debug.Log("Setting iOS scripting backend to IL2CPP to enable 64bit support.");
                    PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, iOSBuildTarget);
                }
#endif
                // Here we set the scripting define symbols for IOS
                // so we can remember that the settings were set once.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(iOSBuildTarget, 
                                                                 iOSSymbols + ";" + VUFORIA_IOS_SETTINGS);
            }
        }
    }
}
