# Build from source

Building of Elektronik is 3 stages process:
1. Building of native libraries
2. Building of Unity player
3. Building of plugins

### If you are using `git clone`, then you will need [git-lfs](https://git-lfs.github.com/).
After you have cloned repository go to root directory of repository and run this command:
```
git lfs fetch --all
```
This is necessary for correct downloading of large files. 

## Windows

### Building of native libraries

Some features could not be implemented in .NET environment so they was implemented in native code using C++.
These features are:
- Mesh reconstruction 
- Planes detection
- Working with ROS2 network

If you don't need these features you can skip this stage and move to building of Unity player. 

For building of natives you will need [cmake](https://cmake.org/), [swig](https://swig.org/),
[Visual Studio 2019](https://visualstudio.microsoft.com/) and some 3rd party libraries:
- OpenCV (Mesh reconstruction and planes detection),
- PCL (Mesh reconstruction)
- FastRTPS (ROS2)

We recommend to install them via packet manager [vcpkg](https://github.com/microsoft/vcpkg):
```
./vcpkg.exe install openssl opencv pcl fastrtps
```
For building of library that works with ROS2 move to directory `plugins/ROS2DDS` and generate cmake cache:
```
cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
```
If you are using vcpkg add key `CMAKE_TOOLCHAIN_FILE` with path to file `vcpkg.cmake` to command above, like here:
```
-DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake
```
After that build library:
```
cmake --build .
```
Install library to Elektronik:
```
cmake --install .
```
You can build other libraries same way. They are in directories `plugins\MeshBuilder` и `plugins\PlanesDetector`.

### Building of Unity player

For building Unity player you will need Unity editor (2020.3(LTS) or later) and Visual Studio 2019.
You can install Unity editor using official [launcher](https://unity3d.com/ru/get-unity/download)

If you didn't build library for mesh reconstruction or for planes detection, then find in file `./ProjectSettings/ProjectSettings.asset` lines
```unityyaml
  scriptingDefineSymbols:
    1: 
```
Add there `NO_MESH_BULDER`, if you didn't build library for mesh reconstruction,
and `NO_PLANES_DETECTION`, if you didn't build library for planes detection.
Example:
```unityyaml
  scriptingDefineSymbols:
    1: NO_MESH_BULDER;NO_PLANES_DETECTION
```

Now you can build player from command line or Unity editor GUI.
For command line build go to root directory of repository and run these commands (path to Unity will differ on your system):
```
"C:\Program Files\Unity\Hub\Editor\2020.3.11f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -logFile .\Logs\pre_build.log -executeMethod Elektronik.Editor.PlayerBuildScript.BuildAddressables -projectPath .\
"C:\Program Files\Unity\Hub\Editor\2020.3.11f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -logFile .\Logs\build.log -projectPath .\ -buildWindows64Player .\Build\Elektronik.exe 
```
After that in `./build` you will see Elektronik player.

Or you can build player through Unity editor GUI. 

Before building you need to compile translation files. For that in Unity editor select menu
`Window->Asset management->Addressables->Gropus` and in newly opened window select `Build->New build->Default Build Script`.
After compilation is done you can proceed to building player. Choose `./build` as output directory or you will need 
to change path to dependencies in plugins.

### Building of plugins

If you didn't build library for ROS2 then add then add in project file `./plugins/Ros/Ros.csproj` line
```xml
<DefineConstants>NO_ROS2DDS</DefineConstants>
```
into section `<PropertyGroup>`. Example:
```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <RootNamespace>Elektronik.RosPlugin</RootNamespace>
    <Deterministic>False</Deterministic>
    <AssemblyVersion>1.2.*</AssemblyVersion>
    <FileVersion>1.2.*</FileVersion>
    <DefineConstants>NO_ROS2DDS</DefineConstants>
</PropertyGroup>
```

The easiest way to build all plugins just run form root directory of repo script [build_plugins.bat](../.github/build_plugins.bat).
This script will build plugins and install them to Elektronik if it was built in `./build` dir.

If you what to build plugins your self then open file `./plugins/plugins.sln` in Visual Studio, or any other .NET IDE.
Make changes if you want to and build solution. Now you need to install plugins to Elektronik.
Publish plugins to folder `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/libraries/`.
After that copy files with translations `translations.csv` to `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/data`.

Congratulations! Elektronik is built and ready to work.

## Linux

*This instruction is for building on Ubuntu 21.01. In other distributives may be different names of packages
and any incomabilities.*

### Building of native libraries
Some features could not be implemented in .NET environment so they was implemented in native code using C++.
These features are:
- Mesh reconstruction
- Planes detection
- Working with ROS2 network

If you don't need these features you can skip this stage and move to building of Unity player.

For building of natives you will need [cmake](https://cmake.org/), [swig](https://swig.org/),
[Visual Studio 2019](https://visualstudio.microsoft.com/) and some 3rd party libraries:
- OpenCV (Planes detection),
- PCL (Mesh detection)
- FastRTPS (ROS2)

*Mesh reconstruction does not work in linux for now. See [issue](https://github.com/dioram/Elektronik-Tools-2.0/issues/51).*

We recommend to install this dependencies using [vcpkg](https://github.com/microsoft/vcpkg).
Also for them you will need to install some packages from apt:
```
sudo apt -y install make swig curl python
```
Now run vcpkg:
```
./vcpkg.exe install openssl opencv pcl fastrtps
```
For building of library that works with ROS2 move to directory `plugins/ROS2DDS` and generate cmake cache:
```
cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
```
If you are using vcpkg add key `CMAKE_TOOLCHAIN_FILE` with path to file `vcpkg.cmake` to command above, like here:
```
-DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake
```
After that build library:
```
cmake --build .
```
Install library to Elektronik:
```
cmake --install .
```
You can build other libraries same way. They are in directories `plugins\MeshBuilder` и `plugins\PlanesDetector`.

### Building of Unity player

For building Unity player you will need Unity editor (2020.3(LTS) or later) and .NET environment.
You can install Unity editor using official [launcher](https://unity3d.com/ru/get-unity/download).
Manual for .NET installation can be found on [MSDN](https://docs.microsoft.com/ru-ru/dotnet/core/install/linux).

If you didn't build library for planes detection, then find in file `./ProjectSettings/ProjectSettings.asset` lines
```unityyaml
  scriptingDefineSymbols:
    1: 
```
Add there `NO_PLANES_DETECTION`.
Example:
```unityyaml
  scriptingDefineSymbols:
    1: NO_PLANES_DETECTION
```

You can build player through Unity editor GUI.

Before building you need to compile translation files. For that in Unity editor select menu
`Window->Asset management->Addressables->Gropus` and in newly opened window select `Build->New build->Default Build Script`.
After compilation is done you can proceed to building player. Choose `./build` as output directory or you will need
to change path to dependencies in plugins.

### Building of plugins

If you didn't build library for ROS2 then add then add in project file `./plugins/Ros/Ros.csproj` line
```xml
<DefineConstants>NO_ROS2DDS</DefineConstants>
```
into section `<PropertyGroup>`. Example:
```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <RootNamespace>Elektronik.RosPlugin</RootNamespace>
    <Deterministic>False</Deterministic>
    <AssemblyVersion>1.2.*</AssemblyVersion>
    <FileVersion>1.2.*</FileVersion>
    <DefineConstants>NO_ROS2DDS</DefineConstants>
</PropertyGroup>
```

The easiest way to build all plugins just run form root directory of repo script [build_plugins.sh](../.github/build_plugins.sh).
This script will build plugins and install them to Elektronik if it was built in `./build` dir.

If you what to build plugins your self then open file `./plugins/plugins.sln` in any .NET IDE.
Make changes if you want to and build solution. Now you need to install plugins to Elektronik.
Publish plugins to folder `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/libraries/`.
After that copy files with translations `translations.csv` to `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/data`.

Congratulations! Elektronik is built and ready to work.

[<- Home page](Home-EN.md) | [Usage ->](Usage-EN.md)