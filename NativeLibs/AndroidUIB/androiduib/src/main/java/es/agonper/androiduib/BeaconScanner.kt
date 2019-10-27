package es.agonper.androiduib

import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.content.Context
import android.util.Log
import no.nordicsemi.android.support.v18.scanner.*

/**
 * Copyright 2017 Alberto González Pérez
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
class BeaconScanner(private val context: Context, private val appUUID: String, private val unityCallback: UnityCallback) {
    private val tag = javaClass.simpleName

    private val bluetoothAdapter: BluetoothAdapter? by lazy(LazyThreadSafetyMode.NONE) {
        val bluetoothAdapter = context.getSystemService(Context.BLUETOOTH_SERVICE) as BluetoothManager
        bluetoothAdapter.adapter
    }

    private val BluetoothAdapter.isDisabled: Boolean
        get() = !isEnabled

    init {
        bluetoothAdapter?.takeIf { it.isDisabled }?.apply {
            enable()
        }
    }

    fun startScan() {
        bluetoothAdapter?.apply {
            val settings = ScanSettings.Builder()
                .setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY)
                .setReportDelay(500)
                .setUseHardwareBatchingIfSupported(false)
                .build()

            val scanner = BluetoothLeScannerCompat.getScanner()
            scanner.startScan(null, settings, scanCallback)
        }
    }

    fun stopScan() {
        bluetoothAdapter?.takeIf { it.isEnabled }?.apply {
            val scanner = BluetoothLeScannerCompat.getScanner()
            scanner.stopScan(scanCallback)
        }
    }

    private val scanCallback = object : ScanCallback() {
        override fun onScanResult(callbackType: Int, result: ScanResult) {
            notifyBeaconsFound(listOf(result))
        }

        override fun onBatchScanResults(results: MutableList<ScanResult>) {
            notifyBeaconsFound(results)
        }

        override fun onScanFailed(errorCode: Int) {
            unityCallback.onError("failedMonitoringRegion: Code: $errorCode")
        }
    }

    private fun notifyBeaconsFound(results: List<ScanResult>) {
        val allBeacons = scanResultsToBeacons(results)
        Log.d(tag, "$allBeacons")
        val filteredBeacons = filterBeacons(allBeacons)
        if (filteredBeacons.isNotEmpty()) {
            unityCallback.onScanResult(filteredBeacons)
        }
    }

    private fun scanResultsToBeacons(results: List<ScanResult>): List<Beacon> {
        val beacons = mutableListOf<Beacon>()
        for (result in results) {
            if (Beacon.isBeacon(result)) {
                beacons.add(Beacon.fromScanResult(result))
            }
        }
        return beacons
    }

    private fun filterBeacons(beacons: List<Beacon>): List<Beacon> {
        return beacons.filter { beacon -> beacon.uuid == appUUID }
    }
}