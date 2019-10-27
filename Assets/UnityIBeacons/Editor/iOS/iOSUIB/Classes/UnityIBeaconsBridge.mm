//
//  UnityIBeaconsBridge.mm
//  iOSUIB
//
//  Created by Alberto Gonzalez Perez on 26/10/2019.
//

#import <Foundation/Foundation.h>
#import "iOSUIB-Swift.h"

static UnityIBeacons* unityIBeacons = [[UnityIBeacons alloc] init];

#pragma mark - C interface

extern "C" {
    void _uib_initialize(const char* appUUID) {
        [unityIBeacons initializeWithAppUUID: [NSString stringWithCString:appUUID encoding:NSUTF8StringEncoding]];
    }

    void _uib_startScan() {
        [unityIBeacons startScan];
    }

    void _uib_stopScan() {
        [unityIBeacons stopScan];
    }
}
