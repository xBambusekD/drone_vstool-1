# drone_vstool

### Installation
 - clone this repo
 - download multimedia files from LFS:
   ```bash
   git lfs install
   git lfs pull
   ```
 - Setup:
   - Download Modern UI Pack from https://assetstore.unity.com/packages/tools/gui/modern-ui-pack-150824
   - Install Mapbox SDK from https://www.mapbox.com/install/unity/
   - Download Unity Toggle https://github.com/Kalxoznik/Unity-Toggle-controller
   - Copy all assets into Assets/Submodules
 - RosSharp Setup:
   - Download ROSSharp v1.4 source code https://github.com/siemens/ros-sharp/releases/tag/v1.4
   - Extract and Copy **ros-sharp-1.4\Unity3D\Assets\RosSharp** file into Assets/Submodules
   - Replace **RosSharp\Scripts\RosBridgeClient\RosCommuncation\RosConnector.cs** with **VSTool\RosReplace\RosConnector.cs**