package es.agonper.androiduib

import no.nordicsemi.android.support.v18.scanner.ScanResult
import java.lang.Error
import kotlin.math.pow


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
data class Beacon(val uuid: String, val major: Int, val minor: Int, val distance: Double, val rssi: Int) {
    companion object {
        fun fromScanResult(result: ScanResult): Beacon {
            var startByte = 2
            var patternFound = false

            result.scanRecord?.bytes?.let {scanRecord ->
                while (startByte <= 5) {
                    if (scanRecord[startByte + 2].toInt() and 0xff == 0x02 && //Identifies an iBeacon
                        scanRecord[startByte + 3].toInt() and 0xff == 0x15
                    ) { //Identifies correct data length
                        patternFound = true
                        break
                    }
                    startByte++
                }

                if (patternFound) {
                    //Convert to hex String
                    val uuidBytes = ByteArray(16)
                    System.arraycopy(scanRecord, startByte + 4, uuidBytes, 0, 16)
                    val hexString = bytesToHex(uuidBytes)

                    //Here is your UUID
                    val uuid = hexString.substring(0, 8) + "-" +
                            hexString.substring(8, 12) + "-" +
                            hexString.substring(12, 16) + "-" +
                            hexString.substring(16, 20) + "-" +
                            hexString.substring(20, 32)

                    //Here is your Major value
                    val major =
                        (scanRecord[startByte + 20].toInt() and 0xff) * 0x100 + (scanRecord[startByte + 21].toInt() and 0xff)

                    //Here is your Minor value
                    val minor =
                        (scanRecord[startByte + 22].toInt() and 0xff) * 0x100 + (scanRecord[startByte + 23].toInt() and 0xff)

                    return Beacon(uuid, major, minor, calculateDistance(result), result.rssi)
                }
            }
            throw Error("Not a beacon!")
        }

        fun isBeacon(result: ScanResult): Boolean {
            var startByte = 2
            result.scanRecord?.bytes?.let {scanRecord ->
                while (startByte <= 5) {
                    if (scanRecord[startByte + 2].toInt() and 0xff == 0x02 && //Identifies an iBeacon
                        scanRecord[startByte + 3].toInt() and 0xff == 0x15
                    ) { //Identifies correct data length
                        return true
                    }
                    startByte++
                }
            }
            return false
        }

        private fun calculateDistance(result: ScanResult): Double {
            val txPower = result.txPower
            val rssi = result.rssi
            if (rssi == 0) {
                return -1.0 // if we cannot determine distance, return -1.
            }

            val ratio = rssi * 1.0 / txPower
            return if (ratio < 1.0) {
                ratio.pow(10.0)
            } else {
                0.89976 * ratio.pow(7.7095) + 0.111 // Nexus 4 coefficients
            } * 1000
        }
    }
}

private val hexArray = "0123456789ABCDEF".toCharArray()
private fun bytesToHex(bytes: ByteArray): String {
    val hexChars = CharArray(bytes.size * 2)
    for (j in bytes.indices) {
        val v = bytes[j].toInt() and 0xFF
        hexChars[j * 2] = hexArray[v.ushr(4)]
        hexChars[j * 2 + 1] = hexArray[v and 0x0F]
    }
    return String(hexChars)
}