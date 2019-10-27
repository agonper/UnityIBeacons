//
//  UnityCallback.swift
//  iOSUIB
//
//  Created by Alberto Gonzalez Perez on 27/10/2019.
//

import Foundation
import CoreLocation

class UnityCallback {
    
    public func onScanResult(_ beacons: [CLBeacon]) {
        let encodedBeacons = encodeBeacons(beacons)
        print(encodedBeacons)
        UnitySendMessage("IBeaconsEventSystem", "OnScanResult", encodedBeacons.cString(using: String.Encoding.utf8))
    }
    
    public func onError(_ message: String) {
        UnitySendMessage("IBeaconsEventSystem", "OnProviderFailure", message.cString(using: String.Encoding.utf8))
    }
    
    private func encodeBeacons(_ beacons: [CLBeacon]) -> String {
        var encodedBeacons = ""
        let lastBeacon = beacons.count-1
        for (index, beacon) in beacons.enumerated() {
            if (index != lastBeacon) {
                encodedBeacons += encodeBeacon(beacon) + ";"
            }
        }
        encodedBeacons += encodeBeacon(beacons[lastBeacon])
        return encodedBeacons
    }
    
    private func encodeBeacon(_ beacon: CLBeacon) -> String {
        return "\(beacon.major)#\(beacon.minor)#\(beacon.accuracy)#\(beacon.rssi)"
    }
}
