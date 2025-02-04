cmake_minimum_required(VERSION 3.17)
project(ROS2DDS CXX)
include(CopyDlls.cmake)
include(GNUInstallDirs)

find_package(asio REQUIRED)
find_package(OpenSSL REQUIRED)
find_package(tinyxml2 REQUIRED)
find_package(foonathan_memory REQUIRED)
find_package(fastcdr REQUIRED)
find_package(fastrtps REQUIRED)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})
set_source_files_properties(interface/Connector.i PROPERTIES CPLUSPLUS ON)
# SWIG command line option. Sets where to find native dll in runtime.
set_property(SOURCE interface/Connector.i PROPERTY COMPILE_OPTIONS -dllimport Plugins/Ros/libraries/ROS2DDS.dll)

if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()
set(DataTypes DataTypes/Ros.h DataTypes/Ros.cxx DataTypes/RosPubSubTypes.h DataTypes/RosPubSubTypes.cxx)

swig_add_library(ROS2DDS
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../Ros/Ros2/Online/SWIG_generated
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES interface/Connector.h interface/Connector.cpp interface/Connector.i interface/Handlers.h
        interface/Messages.h interface/Logger.h ${DataTypes})
target_link_libraries(ROS2DDS PRIVATE fastrtps fastcdr)
set_target_properties(ROS2DDS PROPERTIES CMAKE_CXX_STANDARD 17)

set(INSTALL_VR ${PROJECT_SOURCE_DIR}/../../build_vr/Plugins/Ros/libraries)
set(INSTALL_NON_VR ${PROJECT_SOURCE_DIR}/../../build/Plugins/Ros/libraries)

target_copy_libs(ROS2DDS foonathan_memory)
install(DIRECTORY ${CMAKE_BINARY_DIR}/ DESTINATION ${INSTALL_VR} FILES_MATCHING REGEX ".*\\.((lib)|(so)|(dll))")
install(DIRECTORY ${CMAKE_BINARY_DIR}/ DESTINATION ${INSTALL_NON_VR} FILES_MATCHING REGEX ".*\\.((lib)|(so)|(dll))")