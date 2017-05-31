using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BluetoothController : MonoBehaviour, IBtObserver {
    
    private Bluetooth bluetooth;

    [SerializeField]
    private BluetoothModel bluetoothModel;

    [SerializeField]
    private Dropdown deviceDropdown;

    [SerializeField]
    private Button searchButton;

    [SerializeField]
    private Button connectButton;

    [SerializeField]
    public Text bluetoothMessage;

    private void Awake() {
        this.bluetooth = Bluetooth.getInstance();
    }

    private void Start() {
        this.bluetoothModel.AddObserver(this);
        this.deviceDropdown.ClearOptions();

        this.searchButton.onClick.AddListener(
            () => {
                this.bluetooth.SearchDevice();
            });

        this.connectButton.onClick.AddListener(
             () => {
                 this.bluetooth.Connect(this.deviceDropdown.options[this.deviceDropdown.value].text);
             });
    }

    public void OnStateChanged(string _State) {
    }

    public void OnSendMessage(string _Message) {
    }

    public void OnGetMessage(string _Message) {
        this.bluetoothMessage.text = _Message;
        Debug.Log(_Message);
    }

    public void OnFoundNoDevice() {
    }

    public void OnScanFinish() {
    }

    public void OnFoundDevice() {
        // Clear and Get new List
        deviceDropdown.ClearOptions();
        deviceDropdown.AddOptions(this.bluetoothModel.macAddresses);
    }
}
