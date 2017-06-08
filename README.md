UnityBluetoothPlugin
====================
Unity android bluetooth plugin for study

Introduction
------------
This project is based on the [BluetoothChat](https://github.com/googlesamples/android-BluetoothChat) example.
It is part of my [DIY VR Controller project]() and was created to study how to use Android modules in Unity & Unreal.

How to use
----------
1. Since the `Bluetooth` object is a singleton, it can be accessed and used immediately. You can access Java code through the `Bluetooth` object.
```csharp
private Bluetooth bluetooth = null;
            /* ... */
bluetooth = Bluetooth.getInstance();
bluetooth.SearchDevice();
            /* ... */
bluetooth.Connect(DEVICE_INFO);
```
2. Android events are updated in the `BluetoothModel`. BluetoothModel is `BtObservable`. If you want to observe updated data, you can subscribe by implementing the `IBtObserver` interface.
```csharp
public class BluetoothController : MonoBehaviour, IBtObserver {
  private Bluetooth bluetooth;
  private BluetoothModel bluetoothModel;
}
```
3. Now, you can refer to the event method list and design it in detail. :)

License
-------
Copyright 2017 The Android Open Source Project, Inc.

Licensed to the Apache Software Foundation (ASF) under one or more contributor
license agreements.  See the NOTICE file distributed with this work for
additional information regarding copyright ownership.  The ASF licenses this
file to you under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License.  You may obtain a copy of
the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the
License for the specific language governing permissions and limitations under
the License.
