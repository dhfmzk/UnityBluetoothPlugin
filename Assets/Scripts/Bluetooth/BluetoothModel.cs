using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public interface IBtObserver {
    void OnStateChanged(string _State);
    void OnSendMessage(string _Message);
    void OnGetMessage(string _Message);
    void OnFoundNoDevice();
    void OnScanFinish();
    void OnFoundDevice();
}

public abstract class BtObservale : MonoBehaviour {
    protected List<IBtObserver> observerList;
    public abstract void AddObserver(IBtObserver _btObserver);
    public abstract void RemoveObserver(IBtObserver _btObserver);
}

public class BluetoothModel : BtObservale {

    [SerializeField]
    private int bufferSize = 256;
    [SerializeField]
    private char startChar = '$';
    [SerializeField]
    private char endChar = '#';

    public List<string> macAddresses = null;
    private Queue<string> messageQueue = null;
    private StringBuilder rawMessage = null;

    void Awake() {
        this.observerList = new List<IBtObserver>();

        this.macAddresses = new List<string>();
        this.messageQueue = new Queue<string>();
        this.rawMessage = new StringBuilder(this.bufferSize);
    }

    public void clearMacAddresses() {
        macAddresses.Clear();
    }

    private void CheckMessageFormat() {
        int startPos = -1;
        int endPos = -1;
        for(int i = 0; i < rawMessage.Length; ++i) {
            if(startPos == -1 && rawMessage[i] == this.startChar) {
                startPos = i;
            }
            else if(endPos == -1 && rawMessage[i] == this.endChar) {
                endPos = i;
            }
        }

        if(startPos != -1 && endPos != -1) {
            messageQueue.Enqueue(rawMessage.ToString(startPos, endPos-startPos+1));
            rawMessage.Remove(startPos, endPos-startPos+1);
        }
        
        string tempMassege = messageQueue.Dequeue();

        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnGetMessage(tempMassege);
        }

        Debug.Log("Get Packet and Enqueue messageQueue");
        Debug.Log(rawMessage);
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
        this.rawMessage.Append(_Message);
        this.CheckMessageFormat();
        Debug.Log("On Read Message : " + _Message);
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
