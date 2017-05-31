using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Bluetooth {

    private AndroidJavaClass _plugin;
    private AndroidJavaObject _activityObject;
    private static Bluetooth instance;

    private Bluetooth() {}
    public static Bluetooth getInstance() {
        if(instance == null) {
            instance = new Bluetooth();
            instance.PluginStart();
        }
        return instance;
    }


    // ========================================
    //          Call Android Method
    // ========================================

    private void PluginStart() {
	    _plugin = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _activityObject = _plugin.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log(_activityObject);
        _activityObject.Call("StartPlugin");
    }

    public string Send(string message) {
        return _activityObject.Call<string>("sendMessage", message);
    }

    public string SearchDevice() {
       Debug.Log("unity -> android | SearchDevice");
       return _activityObject.Call<string>("ScanDevice");       
    }

    public string GetDeviceConnectedName() {
       return _activityObject.Call<string>("GetDeviceConnectedName");
    }

    public string Discoverable() {
        return _activityObject.Call<string>("ensureDiscoverable");
    }

    public void Connect(string Address) {
        _activityObject.Call("Connect", Address);
    }

    public string EnableBluetooth() {
        return _activityObject.Call<string>("BluetoothEnable");
    }

    public string DisableBluetooth() {
        return _activityObject.Call<string>("DisableBluetooth");
    }

    public string DeviceName() {
        return _activityObject.Call<string>("DeviceName");
    }

    public bool IsEnabled() {
        return _activityObject.Call<bool>("IsEnabled");
    }

    public bool IsConnected() {
        return _activityObject.Call<bool>("IsConnected");
    }

	public void Stop() {
		_activityObject.Call("stopThread");
	}

	public void showMessage(string mes) {
		_activityObject.Call("showMessage",mes);
	}
    
}