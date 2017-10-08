using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IUiObserver {
    void OnSearchDevice();
    void OnConnectDevice(string _device);
}

public abstract class UiObservable : MonoBehaviour {
    protected List<IUiObserver> observerList;
    public abstract void AddObserver(IUiObserver _observer);
    public abstract void RemoveObserver(IUiObserver _observer);
    public abstract void UpdateDeviceList(List<string> _deviceList);

    public abstract void UpdateInfo(Vector3 _position);
}

public class BluetoothView : UiObservable {
    
    [SerializeField] private Dropdown deviceDropdown;
    [SerializeField] private Button searchButton;
    [SerializeField] private Button connectButton;

    [SerializeField] public List<Text> infoTexts;
    [SerializeField] private GameObject character;

    private void Awake() {
        this.observerList = new List<IUiObserver>();
    }

    // Use this for initialization
    private void Start () {
        this.deviceDropdown.ClearOptions();

        this.searchButton.onClick.AddListener(() => {
            for (int i = 0; i < this.observerList.Count; ++i) {
                this.observerList[i].OnSearchDevice();
            }
        });

        this.connectButton.onClick.AddListener(() => {
            for (int i = 0; i < this.observerList.Count; ++i) {
                this.observerList[i].OnConnectDevice(this.deviceDropdown.options[this.deviceDropdown.value].text);
            }
        });
    }
    
    // ========================================
    //             Pattern Method
    // ========================================

    public override void AddObserver(IUiObserver _observer) {
        observerList.Add(_observer);
    }

    public override void RemoveObserver(IUiObserver _observer) {
        if (observerList.Contains(_observer)) {
            this.observerList.Remove(_observer);
        }
    }

    public override void UpdateDeviceList(List<string> _deviceList) {
        this.deviceDropdown.ClearOptions();
        this.deviceDropdown.AddOptions(_deviceList);
    }

    public override void UpdateInfo(Vector3 _velocity) {
        infoTexts[0].text = _velocity.x.ToString();
        infoTexts[1].text = _velocity.y.ToString();

        character.GetComponent<Rigidbody>().velocity = _velocity;        
    }

}
