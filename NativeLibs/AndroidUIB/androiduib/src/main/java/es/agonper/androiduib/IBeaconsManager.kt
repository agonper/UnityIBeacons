package es.agonper.androiduib

import android.content.Context
import android.util.Log
import java.util.*

/**
 * Copyright 2019 Alberto González Pérez
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * <p>http://www.apache.org/licenses/LICENSE-2.0</p>
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
class IBeaconsManager(context: Context, appUUID: String) {

    private var scanning: Boolean = false
    private val unityCallback = UnityCallback()
    private val beaconScanner = BeaconScanner(context, appUUID.toUpperCase(Locale.ENGLISH), unityCallback)

    fun startScan() {
        if (scanning) return
        scanning = true
        Log.d(javaClass.simpleName, "Scan started")
        beaconScanner.startScan()
    }

    fun stopScan() {
        if (!scanning) return
        scanning = false
        Log.d(javaClass.simpleName, "Scan stopped")
        beaconScanner.stopScan()
    }
}