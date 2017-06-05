using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public interface IBtObserver {
    void OnStateChanged(string _State);
    void OnSendMessage(string _Message);
    void OnGetMessage(byte[] _packet);
    void OnFoundNoDevice();
    void OnScanFinish();
    void OnFoundDevice();
}

public abstract class BtObservable : MonoBehaviour {
    protected List<IBtObserver> observerList;
    public abstract void AddObserver(IBtObserver _btObserver);
    public abstract void RemoveObserver(IBtObserver _btObserver);
}

public class BluetoothModel : BtObservable {
    
    private Bluetooth bluetooth;

    [SerializeField]
    private int packetSize = 34;
    [SerializeField]
    private char startChar = '$';
    [SerializeField]
    private char endChar = '#';

    private List<byte> buffer = null;
    private bool updateQueue = false;

    public List<string> macAddresses = null;
    private StringBuilder rawMessage = null;

    private void Awake() {
        this.bluetooth = Bluetooth.getInstance();

        this.observerList = new List<IBtObserver>();
        this.macAddresses = new List<string>();

        this.buffer = new List<byte>();
    }
    
    public void clearMacAddresses() {
        macAddresses.Clear();
    }

    // ========================================
    //             Pattern Method
    // ========================================

    public override void AddObserver(IBtObserver _btObserver) {
        this.observerList.Add(_btObserver);
    }

    public override void RemoveObserver(IBtObserver _btObserver) {
        if (observerList.Contains(_btObserver)) {
            this.observerList.Remove(_btObserver);
        }
    }

    // ========================================
    //    Receive Bluetooth Call Back Method
    // ========================================

    void OnStateChanged(string _State) {
        //"STATE_CONNECTED"
        //"STATE_CONNECTING"
        //"UNABLE TO CONNECT"
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnStateChanged(_State);
        }
        Debug.Log(_State);
    }

    void OnSendMessage(string _Message) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnSendMessage(_Message);
        }
        Debug.Log("On Send Message : " + _Message);
    }

    void OnReadMessage(string _Message) {
        byte[] temp = bluetooth.GetPacketData();
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnGetMessage(temp);
        }
    }

    void OnFoundNoDevice(string _s) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnFoundNoDevice();
        }
        Debug.Log("On Found No Device");
    }

    void OnScanFinish(string _s) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnScanFinish();
        }
        Debug.Log("On Scan Finish");
    }

    void OnFoundDevice(string _Device) {
        this.macAddresses.Add(_Device);
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnFoundDevice();
        }
        Debug.Log("On Found Device");
    }
}
