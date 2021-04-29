#if UNITY_ANDROID && !UNITY_EDITOR && GOOGLE_INSTALL_REFERRER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waker.InstallReferrer
{
    public class ResponseCode
    {
        public static readonly int OK;
        public static readonly int FEATURE_NOT_SUPPORTED;
        public static readonly int SERVICE_UNAVAILABLE;
        public static readonly int DEVELOPER_ERROR;
        public static readonly int SERVICE_DISCONNECTED;

        static ResponseCode()
        {
            using (AndroidJavaClass responseClass = 
                new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse"))
            {
                OK = responseClass.GetStatic<int>("OK");
                FEATURE_NOT_SUPPORTED = responseClass.GetStatic<int>("FEATURE_NOT_SUPPORTED");
                SERVICE_UNAVAILABLE = responseClass.GetStatic<int>("SERVICE_UNAVAILABLE");
                DEVELOPER_ERROR = responseClass.GetStatic<int>("DEVELOPER_ERROR");
                SERVICE_DISCONNECTED = responseClass.GetStatic<int>("SERVICE_DISCONNECTED");
            }
        }
    }

    public class InstallReferrerInitializer
    {
        private static InstallReferrerStateListener installReferrerStateProxy;
        private static AndroidJavaObject ajoInstallReferrerClient;

        [RuntimeInitializeOnLoadMethod()]
        public static void RuntimeInitializeOnLoad()
        {
            using (AndroidJavaObject ajoCurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaClass ajcInstallReferrerClient = new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient"))
            {
                ajoInstallReferrerClient = ajcInstallReferrerClient.CallStatic<AndroidJavaObject>("newBuilder", ajoCurrentActivity).Call<AndroidJavaObject>("build");

                if (ajoInstallReferrerClient == null)
                {
                    Debug.LogError("Unable to obtain InstallReferrerClient instance");
                    return;
                }

                installReferrerStateProxy = new InstallReferrerStateListener();
                ajoInstallReferrerClient.Call("startConnection", installReferrerStateProxy);
            }
        }

        private class InstallReferrerStateListener : AndroidJavaProxy
        {
            public InstallReferrerStateListener() : base("com.android.installreferrer.api.InstallReferrerStateListener")
            {
                
            }

            public void onInstallReferrerSetupFinished(int responseCode)
            {
                try
                {
                    if (responseCode != ResponseCode.OK)
                    {
                        OnError(responseCode);
                        return;
                    }

                    Debug.Log("InstallReferrerResponse.OK status code received");

                    AndroidJavaObject ajoReferrerDetails = ajoInstallReferrerClient.Call<AndroidJavaObject>("getInstallReferrer");

                    if (ajoReferrerDetails == null)
                    {
                        Debug.LogError("getInstallReferrer returned null AndroidJavaObject!");
                        return;
                    }

                    string referrerUrl = ajoReferrerDetails.Call<string>("getInstallReferrer");
                    TrackInstallReferrer(referrerUrl);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception: " + e);
                }
                finally
                {
                    ajoInstallReferrerClient.Call("endConnection");
                }
            }

            public void onInstallReferrerServiceDisconnected()
            {
                Debug.Log("onInstallReferrerServiceDisconnected invoked");
            }

            private void OnError(int responseCode)
            {
                if (responseCode == ResponseCode.FEATURE_NOT_SUPPORTED)
                {
                    Debug.LogError("InstallReferrerResponse.FEATURE_NOT_SUPPORTED status code received");
                }
                else if (responseCode == ResponseCode.SERVICE_UNAVAILABLE)
                {
                    Debug.LogError("InstallReferrerResponse.SERVICE_UNAVAILABLE status code received");
                }
                else if (responseCode == ResponseCode.DEVELOPER_ERROR)
                {
                    Debug.LogError("InstallReferrerResponse.DEVELOPER_ERROR status code received");
                }
                else if (responseCode == ResponseCode.SERVICE_DISCONNECTED)
                {
                    Debug.LogError("InstallReferrerResponse.SERVICE_DISCONNECTED status code received");
                }
                else
                {
                    Debug.LogError("Unexpected response code arrived!");
                    Debug.LogError("Response: " + responseCode);
                    Exception exception = new Exception("Unexpected response code arrived");
                }
            }
        }

        private static void TrackInstallReferrer(string referrerUrl)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
            using (AndroidJavaObject receiver = new AndroidJavaObject("com.google.android.gms.analytics.CampaignTrackingReceiver"))
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "com.android.vending.INSTALL_REFERRER"))
            {
                intent.Call<AndroidJavaObject>("putExtra", "referrer", referrerUrl);
                receiver.Call("onReceive", context, intent);
            }
        }
    }
}
#endif