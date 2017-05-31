package com.hyeon.bluetoothPlugin;

import java.util.ArrayList;
import java.util.Set;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothAdapter;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.util.Log;
import android.widget.Toast;

import android.support.annotation.RequiresPermission;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class BluetoothPlugin extends UnityPlayerActivity {

    public static final int MESSAGE_STATE_CHANGE = 1;
    public static final int MESSAGE_READ = 2;
    public static final int MESSAGE_WRITE = 3;
    public static final int MESSAGE_DEVICE_NAME = 4;
    public static final int MESSAGE_TOAST = 5;

    private static final int REQUEST_CONNECT_DEVICE = 1;
    private static final int REQUEST_ENABLE_BT = 2;

    private static final String TAG = "BluetoothPlugin";
    private static final String TARGET = "BluetoothModel";

    private boolean IsScan = false;

    private String mConnectedDeviceName = null;

    private StringBuffer mOutStringBuffer;
    private BluetoothAdapter mBtAdapter = null;
    private BluetoothService mBtService = null;

    private ArrayList<String> singleAddress = new ArrayList();

    // Handler
    private final Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            switch(msg.what) {
                case MESSAGE_STATE_CHANGE:
                    UnityPlayer.UnitySendMessage(TARGET, "OnStateChanged", String.valueOf(msg.arg1));
                    break;
                case MESSAGE_READ:
                    byte[] readBuf = (byte[])msg.obj;
                    String readMessage = new String(readBuf, 0, msg.arg1);
                    UnityPlayer.UnitySendMessage(TARGET, "OnReadMessage", readMessage);
                    break;
                case MESSAGE_WRITE:
                    byte[] writeBuf = (byte[])msg.obj;
                    String writeMessage = new String(writeBuf);
                    UnityPlayer.UnitySendMessage(TARGET, "OnSendMessage", writeMessage);
                    break;
                case MESSAGE_DEVICE_NAME:
                    BluetoothPlugin.this.mConnectedDeviceName = msg.getData().getString("device_name");
                    Toast.makeText(BluetoothPlugin.this.getApplicationContext(), "Connected to " + BluetoothPlugin.this.mConnectedDeviceName, Toast.LENGTH_SHORT).show();
                    break;
                case MESSAGE_TOAST:
                    Toast.makeText(BluetoothPlugin.this.getApplicationContext(), msg.getData().getString("toast"), Toast.LENGTH_SHORT).show();
                    break;
            }

        }
    };

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @RequiresPermission("android.permission.BLUETOOTH")
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if("android.bluetooth.device.action.FOUND".equals(action)) {
                BluetoothDevice device = intent.getParcelableExtra("android.bluetooth.device.extra.DEVICE");
                BluetoothPlugin.this.singleAddress.add(device.getName() + "\n" + device.getAddress());
                UnityPlayer.UnitySendMessage(TARGET, "OnFoundDevice", device.getName() + ",\n" + device.getAddress());

            } else if("android.bluetooth.adapter.action.DISCOVERY_FINISHED".equals(action)) {
                if(BluetoothPlugin.this.IsScan) {
                    UnityPlayer.UnitySendMessage(TARGET, "OnScanFinish", "");
                }

                if(BluetoothPlugin.this.singleAddress.size() == 0) {
                    UnityPlayer.UnitySendMessage(TARGET, "OnFoundNoDevice", "");
                }
            }

        }
    };

    // 1. Starting Point in Unity Script
    @RequiresPermission("android.permission.BLUETOOTH")
    public void StartPlugin() {
        if(Looper.myLooper() == null) {
            Looper.prepare();
        }

        this.SetupPlugin();
    }


    // 2. Setup Plugin
    // Get Default Bluetooth Adapter and start Service
    @RequiresPermission("android.permission.BLUETOOTH")
    public String SetupPlugin() {
        // Bluetooth Adapter
        this.mBtAdapter = BluetoothAdapter.getDefaultAdapter();

        // if Bluettoth Adapter is avaibale, start Service
        if(this.mBtAdapter == null) {
            return "Bluetooth is not available";

        } else {
            if(this.mBtService == null) {
                this.startService();
            }

            return "SUCCESS";
        }
    }


    // 3. Setup and Start Bluetooth Service
    private void startService() {
        Log.d(TAG, "setupService()");
        this.mBtService = new BluetoothService(this, this.mHandler);
        this.mOutStringBuffer = new StringBuffer("");
    }

    public String DeviceName() {
        return this.mBtAdapter.getName();
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public String GetDeviceConnectedName() {
        return !this.mBtAdapter.isEnabled()?"You Must Enable The BlueTooth":(this.mBtService.getState() != 3?"Not Connected":this.mConnectedDeviceName);
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public boolean IsEnabled() {
        return this.mBtAdapter.isEnabled();
    }

    public boolean IsConnected() {
        return this.mBtService.getState() == 3;
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public void stopThread() {
        Log.d(TAG, "stop");
        if(this.mBtService != null) {
            this.mBtService.stop();
            this.mBtService = null;
        }

        if(this.mBtAdapter != null) {
            this.mBtAdapter = null;
        }

        this.SetupPlugin();
    }

    @RequiresPermission(
            allOf = {"android.permission.BLUETOOTH", "android.permission.BLUETOOTH_ADMIN"}
    )
    public void Connect(String TheAdrees) {
        if(this.mBtAdapter.isDiscovering()) {
            this.mBtAdapter.cancelDiscovery();
        }

        this.IsScan = false;
        String address = TheAdrees.substring(TheAdrees.length() - 17);
        this.mConnectedDeviceName = TheAdrees.split(",")[0];
        BluetoothDevice device = this.mBtAdapter.getRemoteDevice(address);

        this.mBtService.connect(device);
    }

    @RequiresPermission(
            allOf = {"android.permission.BLUETOOTH", "android.permission.BLUETOOTH_ADMIN"}
    )
    String ScanDevice() {
        Log.d(TAG, "Start - ScanDevice()");
        if(!this.mBtAdapter.isEnabled()) {
            return "You Must Enable The BlueTooth";
        } else {
            this.IsScan = true;
            this.singleAddress.clear();
            IntentFilter filter = new IntentFilter("android.bluetooth.device.action.FOUND");
            this.registerReceiver(this.mReceiver, filter);
            filter = new IntentFilter("android.bluetooth.adapter.action.DISCOVERY_FINISHED");
            this.registerReceiver(this.mReceiver, filter);
            this.mBtAdapter = BluetoothAdapter.getDefaultAdapter();
            Set pairedDevices = this.mBtAdapter.getBondedDevices();
            if(pairedDevices.size() > 0) {

            }

            this.doDiscovery();
            return "SUCCESS";
        }
    }

    @RequiresPermission(
            allOf = {"android.permission.BLUETOOTH", "android.permission.BLUETOOTH_ADMIN"}
    )
    private void doDiscovery() {
        Log.d(TAG, "doDiscovery()");
        if(this.mBtAdapter.isDiscovering()) {
            this.mBtAdapter.cancelDiscovery();
        }

        this.mBtAdapter.startDiscovery();
    }

    @RequiresPermission(
            allOf = {"android.permission.BLUETOOTH", "android.permission.BLUETOOTH_ADMIN"}
    )
    String BluetoothSetName(String name) {
        if(!this.mBtAdapter.isEnabled()) {
            return "You Must Enable The BlueTooth";
        } else if(this.mBtService.getState() != 3) {
            return "Not Connected";
        } else {
            this.mBtAdapter.setName(name);
            return "SUCCESS";
        }
    }

    @RequiresPermission(
            allOf = {"android.permission.BLUETOOTH", "android.permission.BLUETOOTH_ADMIN"}
    )
    String DisableBluetooth() {
        if(!this.mBtAdapter.isEnabled()) {
            return "You Must Enable The BlueTooth";
        } else {
            if(this.mBtAdapter != null) {
                this.mBtAdapter.cancelDiscovery();
            }

            if(this.mBtAdapter.isEnabled()) {
                this.mBtAdapter.disable();
            }

            return "SUCCESS";
        }
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public String BluetoothEnable() {
        try {
            if(!this.mBtAdapter.isEnabled()) {
                Intent e = new Intent("android.bluetooth.adapter.action.REQUEST_ENABLE");
                this.startActivityForResult(e, 2);
            }

            return "SUCCESS";

        } catch (Exception e) {
            return "Faild";
        }
    }

    public void showMessage(final String message) {
        this.runOnUiThread(new Runnable() {
            public void run() {
                Toast.makeText(BluetoothPlugin.this, message, Toast.LENGTH_SHORT).show();
            }
        });
    }

    // Android Life cycle method
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.e(TAG, "+++ ON CREATE +++");
    }

    public void onStart() {
        super.onStart();
    }

    public synchronized void onPause() {
        super.onPause();
        Log.e(TAG, "- ON PAUSE -");
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public synchronized void onResume() {
        super.onResume();
        Log.d(TAG, "+ ON RESUME +");
        if(this.mBtService != null && this.mBtService.getState() == 0) {
            this.mBtService.start();
        }

    }

    public void onStop() {
        super.onStop();
        Log.e(TAG, "-- ON STOP --");
    }

    public void onDestroy() {
        super.onDestroy();
        if(this.mBtService != null) {
            this.mBtService.stop();
        }

        Log.e(TAG, "--- ON DESTROY ---");
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public String ensureDiscoverable() {
        if(!this.mBtAdapter.isEnabled()) {
            return "You Must Enable The BlueTooth";
        } else {
            if(this.mBtAdapter.getScanMode() != 23) {
                Intent discoverableIntent = new Intent("android.bluetooth.adapter.action.REQUEST_DISCOVERABLE");
                discoverableIntent.putExtra("android.bluetooth.adapter.extra.DISCOVERABLE_DURATION", 300);
                this.startActivity(discoverableIntent);
            }

            return "SUCCESS";
        }
    }

    @RequiresPermission("android.permission.BLUETOOTH")
    public String sendMessage(String message) {
        if(!this.mBtAdapter.isEnabled()) {
            return "You Must Enable The BlueTooth";

        } else if(this.mBtService.getState() != 3) {
            return "Not Connected";

        } else {
            if(message.length() > 0) {
                byte[] send = message.getBytes();
                this.mBtService.write(send);
                this.mOutStringBuffer.setLength(0);
            }

            return "SUCCESS";
        }
    }


}