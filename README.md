# drone_vstool

## DroCo - Multi-Drone Control Vizualization Tool
[DroCo (VSTool)](https://www.fit.vut.cz/research/product/647/.en) is a tool for effective drone remote control using mixed reality, that also supports communication and cooperation on a mission with multiple drones. The proposed solution is developed by [Robo@FIT, Brno University of Technology](https://www.fit.vut.cz/research/group/robo/.en) research group, and is inspired by the high mental load of the pilot in the control of the drone, especially in the performance of more complex missions (multiple drones, remote target, proximity to infrastructure etc.). The system is based on the extension of the 3D virtual model with real data (augmented virtuality). It uses temporal and spatial registration of:
 1) off-line data (map data, elevation data, 3D building models)
 2) on-line data (video-stream, reconstructed 3D structures, location information, flight data)
 3) virtual control objects (navigation points and directions, spatial areas and geo-fences, position of other drones, distance to nearby objects, preview map, or view from other drones). 
 
The system thus allows you to pilot the drone in FPV (first-person-view), but at any time it can switch to TPV (third --person-view), so that one can look around freely in a situation with poor orientation, further directs the pilot to other mission objectives, points out close objects or other drones, etc. The system is currently being expanded with the functions of multiple drones, sharing more sensory information across the system, increasing the security of network communication. The development also aims to use a system for drone control training for pilots, increase the realism of drone behavior in simulated mode, more efficient mission management and visualization of the status for the operator of the whole event.

<img src=vstool_ui.png />

## Installation
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

## Publications
 - [HUBINÁK, Róbert. Application for Efficient Drone Control Using Augmented Virtuality. Brno, 2020. Bachelor's thesis. Brno University of Technology, Faculty of Information Technology. Supervised by Beran Vítězslav.](https://www.fit.vut.cz/study/thesis-file/22839/22839.pdf)
 - [SEDLMAJER Kamil, BAMBUŠEK Daniel a BERAN Vítězslav. Effective Remote Drone Control Using Augmented Virtuality. In: Proceedings of the 3rd International Conference on Computer-Human Interaction Research and Applications 2019. Vienna: SciTePress - Science and Technology Publications, 2019, s. 177-182. ISBN 978-989-758-376-6.](https://www.fit.vut.cz/research/publication/12006/.en)
 - [SEDLMAJER, Kamil. User interface for drone control using augmented virtuality. Brno, 2019. Master's Thesis. Brno University of Technology, Faculty of Information Technology. 2019-06-14. Supervised by Beran Vítězslav.](https://www.fit.vut.cz/study/thesis-file/16730/16730.pdf)
