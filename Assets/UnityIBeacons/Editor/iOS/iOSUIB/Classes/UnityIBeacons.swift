//
//  UnityIBeacons.swift
//  iOSUIB
//
//  Created by Alberto Gonzalez Perez on 26/10/2019.
//

import Foundation
import CoreLocation

public class UnityIBeacons: NSObject {
    
    private var appUUID: String = "";
    private let locationManager = CLLocationManager()
    private let unityCallback = UnityCallback()
    private let beaconScanner: BeaconScanner
    
    private var scanning = false
    
    override public init() {
        beaconScanner = BeaconScanner(unityCallback)
    }
    
    @objc public func initialize(appUUID: String) {
        self.appUUID = appUUID;
        print(appUUID)
    }
    
    @objc public func startScan() {
        if scanning { return }
        if !CLLocationManager.isMonitoringAvailable(for: CLBeaconRegion.self) || !CLLocationManager.isRangingAvailable() {
            unityCallback.onError("cannotMonitorBeacons")
            print("Device hardware insufficient")
            return
        }
        locationManager.requestWhenInUseAuthorization()
        locationManager.delegate = self.beaconScanner
        
        scanning = true;
        print("Scanning!")
        let region = getBeaconsRegion()
        locationManager.startMonitoring(for: region)
        locationManager.startRangingBeacons(in: region)
    }
    
    @objc public func stopScan() {
        if !scanning { return }
        scanning = false
        print("Scanning stopped")
        let region = getBeaconsRegion()
        locationManager.stopMonitoring(for: region)
        locationManager.stopRangingBeacons(in: region)
    }
    
    private func getBeaconsRegion() -> CLBeaconRegion {
        let proximityUUID = UUID(uuidString: appUUID)
        return CLBeaconRegion(proximityUUID: proximityUUID!, identifier: appUUID)
    }
}
