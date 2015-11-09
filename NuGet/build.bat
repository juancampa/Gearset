@echo off
rem msbuild.exe build.xml /p:OutDir=\bin\XXXRelease\;Configuration=Release;Platform=x86 /target:BuildProjects
rem msbuild.exe build.xml /p:Configuration=Debug;Platform=AnyCpu;TargetFrameworkVersion=v4.5;lib=45 /target:BuildMonoGame
@echo on

"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.xml /target:Clean
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:lib=40 /target:BuildXNA
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:TargetFrameworkVersion=v4.0;lib=40 /target:BuildMonoGame
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.xml /clp:NoSummary;NoItemAndPropertyList;ErrorsOnly /p:TargetFrameworkVersion=v4.5;lib=45 /target:BuildMonoGame
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.xml /target:Clean

nuget pack Gearset.MonoGame.WindowsDX\Gearset.MonoGame.WindowsDX.nuspec
nuget pack Gearset.MonoGame.WindowsGL\Gearset.MonoGame.WindowsGL.nuspec
nuget pack Gearset.XNA.Windows\Gearset.XNA.Windows.nuspec

move /Y .\Gearset.MonoGame.WindowsDX*.* .\Gearset.MonoGame.WindowsDX\
move /Y .\Gearset.MonoGame.WindowsGL*.* .\Gearset.MonoGame.WindowsGL\
move /Y .\Gearset.XNA.Windows*.* .\Gearset.XNA.Windows\
pause