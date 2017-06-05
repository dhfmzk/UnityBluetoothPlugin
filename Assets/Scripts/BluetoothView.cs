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
    public abstract void GetDeviceList(List<string> _deviceList);
}

public class BluetoothView : UiObservable {
    
    [SerializeField]
    private Dropdown deviceDropdown;

    [SerializeField]
    private Button searchButton;

    [SerializeField]
    private Button connectButton;

    [SerializeField]
    public List<Text> infoTexts;

    [SerializeField]
    private GameObject cube;

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
	
	void Update () {
	
	}

    public void infoUpdate(Quaternion _rotation, Vector3 _position) {
        infoTexts[0].text = _rotation.x.ToString();
        infoTexts[1].text = _rotation.y.ToString();
        infoTexts[2].text = _rotation.z.ToString();

        infoTexts[3].text = _position.x.ToString();
        infoTexts[4].text = _position.y.ToString();
        infoTexts[5].text = _position.z.ToString();
        cube.transform.rotation = _rotation;
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

    public override void GetDeviceList(List<string> _deviceList) {
        this.deviceDropdown.ClearOptions();
        this.deviceDropdown.AddOptions(_deviceList);
    }
}
