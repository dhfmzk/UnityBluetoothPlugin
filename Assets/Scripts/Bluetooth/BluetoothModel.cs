using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public enum BluetoothState {
    UNABLE_TO_CONNECT = 1,
    STATE_CONNECTING = 2,
    STATE_CONNECTED = 3
}

public interface IBtObserver {
    void OnStateChanged(string _State);
    void OnSendMessage(string _Message);
    void OnReadPacket(byte[] _Packet);
    void OnFoundNoDevice();
    void OnScanFinish();
    void OnFoundDevice();
}

public abstract class BtObservable : MonoBehaviour {
    protected List<IBtObserver> observerList;
    public abstract void AddObserver(IBtObserver _BTObserver);
    public abstract void RemoveObserver(IBtObserver _BTObserver);
}

public class BluetoothModel : BtObservable {
    
    private Bluetooth bluetooth;

    [SerializeField] private bool isPacketedData = true;
    [SerializeField] private int packetSize = 34;
    [SerializeField] private char startChar = '$';
    [SerializeField] private char endChar = '#';

    private List<byte> byteBuffer = null;

    public List<string> macAddresses = null;

    private bool isSearchFinished = true;

    private void Awake() {
        this.bluetooth = Bluetooth.getInstance();

        this.observerList = new List<IBtObserver>();
        this.macAddresses = new List<string>();

        this.byteBuffer = new List<byte>();
    }

    private void CheckBuffer() {
        Debug.Log("-- start check -- ");
        if(isPacketedData) {

            // Check until buffer size less then packetSize
            while(byteBuffer.Count >= packetSize) {
                // is Packet?
                if(Convert.ToChar(byteBuffer[0]) == startChar && Convert.ToChar(byteBuffer[packetSize-1]) == endChar) {
                    for (int i = 0; i < this.observerList.Count; ++i) {
                        this.observerList[i].OnReadPacket(byteBuffer.GetRange(0, packetSize).ToArray());
                    }
                    byteBuffer.RemoveRange(0,packetSize);
                }
                else {
                    byteBuffer.RemoveAt(0);
                }
            }
        }
        else {
            // Check data contain escape sqence '\n'
            if(byteBuffer.Contains((byte)'\n')) {
                int sliceIndex = byteBuffer.IndexOf((byte)'\n');
                for (int i = 0; i < this.observerList.Count; ++i) {
                    this.observerList[i].OnReadPacket(byteBuffer.GetRange(0, sliceIndex + 1).ToArray());
                }
                byteBuffer.RemoveRange(0, sliceIndex + 1);
            }
            else {
                // Do nothing
            }
        }
    }
    
    public void ClearMacAddresses() {
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

    void OnStateChanged(string _state) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnStateChanged(_state);
        }

        Debug.Log("[BlueToothPlugin] - " +  _state);
    }

    void OnSendMessage(string _Message) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnSendMessage(_Message);
        }

        Debug.Log("[BlueToothPlugin] - On Send Message : " + _Message);
    }

    void OnReadMessage(string _Message) {
        byte[] temp = bluetooth.GetPacketData();

        this.byteBuffer.AddRange(temp);
        this.CheckBuffer();

        Debug.Log("[BlueToothPlugin] - On Read Message : " + _Message);
    }

    void OnFoundNoDevice(string _s) {
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnFoundNoDevice();
        }

        Debug.Log("[BlueToothPlugin] - On Found No Device");
    }

    void OnScanFinish(string _s) {

        this.isSearchFinished = true;

        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnScanFinish();
        }

        Debug.Log("[BlueToothPlugin] - On Scan Finish");
    }

    void OnFoundDevice(string _Device) {
        if (this.isSearchFinished) {
            this.macAddresses.Clear();
            this.isSearchFinished = false;
        }

        this.macAddresses.Add(_Device);
        
        for (int i = 0; i < this.observerList.Count; ++i) {
            this.observerList[i].OnFoundDevice();
        }
        
        Debug.Log("[BlueToothPlugin] - On Found Device");
    }
}
