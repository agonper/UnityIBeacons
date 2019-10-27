package es.agonper.androiduib

import com.unity3d.player.UnityPlayer
import java.lang.StringBuilder

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
class UnityCallback {

    fun onScanResult(beacons: List<Beacon>) {
        val encodedMessage = encodeBeacons(beacons)
        UnityPlayer.UnitySendMessage("IBeaconsEventSystem", "OnScanResult", encodedMessage)
    }

    fun onError(error: String) {
        UnityPlayer.UnitySendMessage("IBeaconsEventSystem", "OnProviderFailure", error)
    }

    private fun encodeBeacons(beacons: List<Beacon>): String {
        val encodedBeacons = StringBuilder()
        val lastIdx = beacons.size-1
        for ((index, beacon) in beacons.withIndex()) {
            if (index != lastIdx) {
                encodedBeacons.append(encodeBeacon(beacon) + ";")
            }
        }
        encodedBeacons.append(encodeBeacon(beacons[lastIdx]))
        return encodedBeacons.toString()
    }

    private fun encodeBeacon(beacon: Beacon): String {
        return "${beacon.major}#${beacon.minor}#${beacon.distance}#${beacon.rssi}"
    }
}