RESOLVER_VERSION=1.2.130.0

# Build Android native library
cd ./NativeLibs/AndroidUIB
./gradlew :androiduib:clean :androiduib:build
cd ../..

# Copy generated output
cp ./NativeLibs/AndroidUIB/androiduib/build/outputs/aar/androiduib-debug.aar ./Assets/Plugins/Android/es.agonper.androiduib.aar

# Copy iOS sources
rm -rf ./Assets/UnityIBeacons/Editor/iOS*
mkdir ./Assets/UnityIBeacons/Editor/iOS
cp -r ./NativeLibs/iOSUIB/iOSUIB* ./Assets/UnityIBeacons/Editor/iOS

Unity -gvh_disable -batchmode -importPackage ./play-services-resolver-$RESOLVER_VERSION.unitypackage -projectPath . -exportPackage Assets UnityIBeacons.unitypackage -quit
