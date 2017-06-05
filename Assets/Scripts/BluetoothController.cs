using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class BluetoothController : MonoBehaviour, IBtObserver, IUiObserver {
    
    private Bluetooth bluetooth;

    [SerializeField]
    private BluetoothModel bluetoothModel;
    [SerializeField]
    private BluetoothView bluetoothView;

    private Quaternion qTemp = new Quaternion();
    private Vector3 pTemp = new Vector3();

    private Queue<byte[]> messageQueue = null;

    float qTemp_w = 0.0f;
    float qTemp_x = 0.0f;
    float qTemp_y = 0.0f;
    float qTemp_z = 0.0f;

    float sTemp_x = 0.0f;
    float sTemp_y = 0.0f;
    float sTemp_z = 0.0f;

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
            
            this.qTemp_w = BitConverter.ToSingle(temp,  1);
            this.qTemp_x = BitConverter.ToSingle(temp,  5);
            this.qTemp_y = BitConverter.ToSingle(temp,  9);
            this.qTemp_z = BitConverter.ToSingle(temp, 13);

            this.sTemp_x = BitConverter.ToSingle(temp, 17);
            this.sTemp_y = BitConverter.ToSingle(temp, 21);
            this.sTemp_z = BitConverter.ToSingle(temp, 25);

            qTemp.Set(this.qTemp_x, this.qTemp_y, this.qTemp_z, this.qTemp_w);
            pTemp.Set(this.sTemp_x, this.sTemp_y, this.sTemp_z);

            bluetoothView.infoUpdate(qTemp, pTemp);
            
        }
    }

    public void OnStateChanged(string _State) {
    }

    public void OnSendMessage(string _Message) {
    }

    public void OnGetMessage(byte[] _packet) {
        messageQueue.Enqueue(_packet);
    }

    public void OnFoundNoDevice() {
    }

    public void OnScanFinish() {
    }

    public void OnFoundDevice() {
        this.bluetoothView.GetDeviceList(this.bluetoothModel.macAddresses);
    }

    public void OnSearchDevice() {
        this.bluetooth.SearchDevice();
    }

    public void OnConnectDevice(string _device) {
        this.bluetooth.Connect(_device);
    }
}
