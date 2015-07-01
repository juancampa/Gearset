@echo off
rem msbuild.exe build.xml /p:OutDir=\bin\XXXRelease\;Configuration=Release;Platform=x86 /target:BuildProjects
rem msbuild.exe build.xml /p:Configuration=Debug;Platform=AnyCpu;TargetFrameworkVersion=v4.5;lib=45 /target:BuildMonoGame
@echo on

msbuild.exe build.xml /target:Clean
msbuild.exe build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:lib=40 /target:BuildXNA
msbuild.exe build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:TargetFrameworkVersion=v4.0;lib=40 /target:BuildMonoGame
msbuild.exe build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:TargetFrameworkVersion=v4.5;lib=45 /target:BuildMonoGame
msbuild.exe build.xml /target:Clean

pause