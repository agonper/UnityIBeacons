//
//  BeaconScanner.swift
//  iOSUIB
//
//  Created by Alberto Gonzalez Perez on 27/10/2019.
//

import Foundation
import CoreLocation

class BeaconScanner: NSObject {
    
    private let unityCallback: UnityCallback
    
    init(_ unityCallback: UnityCallback) {
        self.unityCallback = unityCallback
    }
}

extension BeaconScanner: CLLocationManagerDelegate {
    func locationManager(_ manager: CLLocationManager, monitoringDidFailFor region: CLRegion?, withError error: Error) {
        unityCallback.onError("failedMonitoringRegion: \(error.localizedDescription)")
        print("Failed monitoring region: \(error.localizedDescription)")
    }
    
    func locationManager(_ manager: CLLocationManager, didFailWithError error: Error) {
        unityCallback.onError("locationManagerFailure: \(error.localizedDescription)")
        print("Location manager failed: \(error.localizedDescription)")
    }
    
    func locationManager(_ manager: CLLocationManager, didRangeBeacons beacons: [CLBeacon], in region: CLBeaconRegion) {
        let visibleBeacons = beacons.filter { $0.accuracy >= 0 }
        if visibleBeacons.count > 0 {
            unityCallback.onScanResult(visibleBeacons)
        }
    }
}
