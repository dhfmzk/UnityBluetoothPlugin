using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class BluetoothController : MonoBehaviour, IBtObserver, IUiObserver {
    
    private Bluetooth bluetooth;

    [SerializeField] private BluetoothModel bluetoothModel;
    [SerializeField] private UiObservable bluetoothView;

    private Vector3 pTemp = new Vector3();

    private Queue<byte[]> messageQueue = null;

    float sTemp_x = 0.0f;
    float sTemp_y = 0.0f;

    private void Awake() {
        this.bluetooth = Bluetooth.getInstance();
        messageQueue = new Queue<byte[]>();
    }

    private void Start() {
        this.bluetoothModel.AddObserver(this);
        this.bluetoothView.AddObserver(this);
    }

    private void Update() {
        if(messageQueue.Count > 0) {
            byte[] temp = messageQueue.Dequeue();
            Debug.Log(temp[0] + " / " + temp[1] + " / " + temp[2] + " / " + temp[3]);

            pTemp = new Vector3(temp[1], temp[2], 0.0f);

            bluetoothView.UpdateInfo(pTemp);
        }
    }

    public void OnStateChanged(string _State) {
    }

    public void OnSendMessage(string _Message) {
    }

    public void OnReadPacket(byte[] _Packet) {
        messageQueue.Enqueue(_Packet);
    }

    public void OnFoundNoDevice() {
        // DO SOMETHING
    }

    public void OnScanFinish() {
        // DO SOMETHING
    }

    public void OnFoundDevice() {
        this.bluetoothView.UpdateDeviceList(this.bluetoothModel.macAddresses);
    }

    public void OnSearchDevice() {
        this.bluetooth.SearchDevice();
    }

    public void OnConnectDevice(string _Device) {
        this.bluetooth.Connect(_Device);
    }
}
