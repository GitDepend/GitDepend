SLN=GitDepend.sln
PACKAGES_DIR=packages
LOCAL_NUGET_DIRECTORY=C:\NuGet
NUGET_OUTPUT_DIRECTORY=artifacts\NuGet
CHOCOLATEY_OUTPUT_DIRECTORY=artifacts\Chocolatey
GITVERSION_VERSION=3.6.2
NUNIT_RUNNERS_VERSION=2.6.4
REPORT_UNIT_VERSION=1.2.1
DOTCOVER_VERSION=2016.2.20160913.100041

debug: package-debug reset
test: run-unit-tests generate-unit-test-report reset
cov: generate-code-coverage-report generate-unit-test-report reset
release: package-release reset
all: package-debug generate-code-coverage-report generate-unit-test-report package-release doc reset
teamcity: package-debug capture-code-coverage package-release documentation reset

doc: documentation reset
    @start docs\build\html\index.html

install-gitversion:
    @echo ##teamcity[blockOpened name='Install GitVersion']

    @nuget install GitVersion.CommandLine -Version $(GITVERSION_VERSION) -OutputDirectory $(PACKAGES_DIR)

    @echo ##teamcity[blockClosed name='Install GitVersion']

install-nunit-runners:
    @echo ##teamcity[blockOpened name='Install NUnit.Runners']

    @nuget install NUnit.Runners -Version $(NUNIT_RUNNERS_VERSION) -OutputDirectory $(PACKAGES_DIR)

    @echo ##teamcity[blockClosed name='Install NUnit.Runners']

install-dotcover:
    @echo ##teamcity[blockOpened name='Install JetBrains.dotCover.CommandLineTools']

    @nuget install JetBrains.dotCover.CommandLineTools -Version $(DOTCOVER_VERSION) -OutputDirectory $(PACKAGES_DIR)

    @echo ##teamcity[blockClosed name='Install JetBrains.dotCover.CommandLineTools']

install-reportunit:
    @echo ##teamcity[blockOpened name='Install ReportUnit']

    @nuget install ReportUnit -Version $(REPORT_UNIT_VERSION) -OutputDirectory $(PACKAGES_DIR)

    @echo ##teamcity[blockClosed name='Install ReportUnit']

version: install-gitversion
    @echo ##teamcity[blockOpened name='Set Version']

    @$(PACKAGES_DIR)\GitVersion.CommandLine.$(GITVERSION_VERSION)\tools\GitVersion.exe /updateassemblyinfo /output buildserver
    @$(PACKAGES_DIR)\GitVersion.CommandLine.$(GITVERSION_VERSION)\tools\GitVersion.exe /showvariable NuGetVersion > version.txt

    @call <<version.bat
@echo off
set /p VERSION= < version.txt
echo ##teamcity[buildNumber '%VERSION%']
<<

    @echo ##teamcity[blockClosed name='Set Version']

restore:
    @echo ##teamcity[blockOpened name='Install NuGet Packages']
    @call <<restore.bat
nuget restore $(SLN)
if ERRORLEVEL 1 exit /b 1
<<
    @echo ##teamcity[blockClosed name='Install NuGet Packages']

build-debug: version restore
    @echo ##teamcity[blockOpened name='Build Solution (Debug)']
    @echo ##teamcity[compilationStarted compiler='MSBuild']

    @msbuild $(SLN) /m /p:Configuration=Debug

    @echo ##teamcity[compilationFinished compiler='MSBuild']
    @echo ##teamcity[blockClosed name='Build Solution (Debug)']

run-unit-tests: build-debug install-nunit-runners
    @echo ##teamcity[blockOpened name='Run Unit Tests']

    @call <<tests.bat
@echo off

if exist Reports rmdir /Q /S Reports
mkdir Reports\UnitTests

set NUNIT_CONSOLE=$(PACKAGES_DIR)\NUnit.Runners.$(NUNIT_RUNNERS_VERSION)\tools\nunit-console.exe

echo %NUNIT_CONSOLE%
pushd .
cd bin\UnitTests\Debug
for /f "delims=" %%i in ('dir *.UnitTests.dll /s/b') do (
    ..\..\..\%NUNIT_CONSOLE% %%i /result=..\..\..\Reports\UnitTests\%%~nxi.xml
)
popd
<<

    @echo ##teamcity[blockClosed name='Run Unit Tests']

generate-unit-test-report: install-reportunit
    @echo ##teamcity[blockOpened name='Generate Unit Test Report']

    @$(PACKAGES_DIR)\ReportUnit.$(REPORT_UNIT_VERSION)\tools\ReportUnit.exe Reports\UnitTests
    @start Reports\UnitTests\index.html

    @echo ##teamcity[blockClosed name='Generate Unit Test Report']

capture-code-coverage: build-debug install-nunit-runners install-dotcover
    @echo ##teamcity[blockOpened name='Capture Code Coverage']

    @call <<coverage.bat
@echo off
if exist Reports rmdir /Q /S Reports
mkdir Reports\UnitTests
mkdir Reports\Coverage

set NUNIT_CONSOLE=$(PACKAGES_DIR)\NUnit.Runners.$(NUNIT_RUNNERS_VERSION)\tools\nunit-console.exe
set DOTCOVER=$(PACKAGES_DIR)\JetBrains.dotCover.CommandLineTools.$(DOTCOVER_VERSION)\tools\dotCover.exe

pushd .
cd bin\UnitTests\Debug
for /f "delims=" %%i in ('dir *.UnitTests.dll /s/b') do (
    ..\..\..\%DOTCOVER% c /TargetExecutable=..\..\..\%NUNIT_CONSOLE% /TargetArguments="%%i /result=..\..\..\Reports\UnitTests\%%~nxi.xml" /Output=..\..\..\Reports\Coverage\%%~nxi.dcvr /TargetWorkingDir=..\..\..
    echo ##teamcity[importData type='dotNetCoverage' tool='dotcover' path='Reports\Coverage\%%~nxi.dcvr']
)
popd

<<

    @echo ##teamcity[blockClosed name='Capture Code Coverage']

merge-code-coverage: capture-code-coverage
    @echo ##teamcity[blockOpened name='Merge dotCover Reports']

    @call <<merge.bat
@echo off

set DOTCOVER=$(PACKAGES_DIR)\JetBrains.dotCover.CommandLineTools.$(DOTCOVER_VERSION)\tools\dotCover.exe

pushd .

set COVERAGE_FILE=Reports\Coverage\Coverage.xml
if exist %COVERAGE_FILE% del %COVERAGE_FILE%

cd Reports\Coverage

echo ^<?xml version="1.0" encoding="utf-8"?^> > ..\..\%COVERAGE_FILE%
echo ^<MergeParams^> >> ..\..\%COVERAGE_FILE%

for /f "delims=" %%i in ('dir *.dcvr /s/b') do (
    echo   ^<Source^>%%i^</Source^> >> ..\..\%COVERAGE_FILE%
)

echo   ^<Output^>MergedSnapshots.dcvr^</Output^> >> ..\..\%COVERAGE_FILE%
echo ^</MergeParams^> >> ..\..\%COVERAGE_FILE%

popd

%DOTCOVER% m %COVERAGE_FILE%
<<

    @echo ##teamcity[blockClosed name='Merge dotCover Reports']

generate-code-coverage-report: merge-code-coverage
    @echo ##teamcity[blockOpened name='Generate Code Coverage Report']

    @call <<cov-report.bat
@echo off
set DOTCOVER=$(PACKAGES_DIR)\JetBrains.dotCover.CommandLineTools.$(DOTCOVER_VERSION)\tools\dotCover.exe
%DOTCOVER% r /Source=Reports\Coverage\MergedSnapshots.dcvr /Output=Reports\Coverage\CoverageReport.html /ReportType=html

del Reports\Coverage\Coverage.xml
del Reports\Coverage\*.dcvr

start Reports\Coverage\CoverageReport.html
<<

    @echo ##teamcity[blockClosed name='Generate Code Coverage Report']

build-release: version restore
    @echo ##teamcity[blockOpened name='Build Solution (Release)']
    @echo ##teamcity[compilationStarted compiler='MSBuild']

    @msbuild $(SLN) /m /p:Configuration=Release

    @echo ##teamcity[compilationFinished compiler='MSBuild']
    @echo ##teamcity[blockClosed name='Build Solution (Release)']

package-debug: build-debug
    @echo ##teamcity[blockOpened name='Generate NuGet Packages (Debug)']

    @if exist $(NUGET_OUTPUT_DIRECTORY)\Debug rd /S /Q $(NUGET_OUTPUT_DIRECTORY)\Debug
    @if exist $(CHOCOLATEY_OUTPUT_DIRECTORY)\Debug rd /S /Q $(CHOCOLATEY_OUTPUT_DIRECTORY)\Debug
    @md $(NUGET_OUTPUT_DIRECTORY)\Debug\Symbols
    @md $(CHOCOLATEY_OUTPUT_DIRECTORY)\Debug
    @if not exist $(LOCAL_NUGET_DIRECTORY) md $(LOCAL_NUGET_DIRECTORY)

    @call <<package.bat
@echo off
set /p VERSION= < version.txt
echo %VERSION%

for /f "delims=" %%i in ('dir *.nuspec /s/b') do (
    call :BUILD_NUGET_PACKAGE_DEBUG %%i
)

nuget pack GitDepend.CommandLine.nuspec -OutputDirectory $(NUGET_OUTPUT_DIRECTORY)\Debug -IncludeReferencedProjects -Properties Configuration=Debug -Symbols -Version %VERSION%
nuget pack GitDepend.Portable.nuspec -OutputDirectory $(CHOCOLATEY_OUTPUT_DIRECTORY)\Debug -Properties Configuration=Debug -Version %VERSION%

move /Y $(NUGET_OUTPUT_DIRECTORY)\Debug\*.symbols.nupkg $(NUGET_OUTPUT_DIRECTORY)\Debug\Symbols
copy $(NUGET_OUTPUT_DIRECTORY)\Debug\*.nupkg $(LOCAL_NUGET_DIRECTORY)

goto :eof

:BUILD_NUGET_PACKAGE_DEBUG
set file=%1
set file=%file:nuspec=csproj%
echo %file%

if not exist "%file%" goto :eof

nuget.exe pack %file% -OutputDirectory $(NUGET_OUTPUT_DIRECTORY)\Debug -IncludeReferencedProjects -Properties Configuration=Debug -Symbols -Version %VERSION%

goto :eof
<<

    @echo ##teamcity[blockClosed name='Generate NuGet Packages (Debug)']

install: package-debug
    @call <<install.bat
@echo off
set /p VERSION= < version.txt
choco install GitDepend.Portable -version %VERSION% -y -pre --force --allow-downgrade -source artifacts\Chocolatey\Debug
<<

build-release: version restore
    @echo ##teamcity[blockOpened name='Build Solution (Release)']
    @echo ##teamcity[compilationStarted compiler='MSBuild']

    @msbuild $(SLN) /m /nologo /p:Configuration=Release

    @echo ##teamcity[compilationFinished compiler='MSBuild']
    @echo ##teamcity[blockClosed name='Build Solution (Release)']

package-release: build-release
    @echo ##teamcity[blockOpened name='Generate NuGet Packages (Release)']

    @if exist $(NUGET_OUTPUT_DIRECTORY)\Release rd /S /Q $(NUGET_OUTPUT_DIRECTORY)\Release
    @if exist $(CHOCOLATEY_OUTPUT_DIRECTORY)\Release rd /S /Q $(CHOCOLATEY_OUTPUT_DIRECTORY)\Release
    @md $(NUGET_OUTPUT_DIRECTORY)\Release\Symbols
    @md $(CHOCOLATEY_OUTPUT_DIRECTORY)\Release
    @if not exist $(LOCAL_NUGET_DIRECTORY) md $(LOCAL_NUGET_DIRECTORY)

    @call <<package.bat
@echo off
set /p VERSION= < version.txt
echo %VERSION%

for /f "delims=" %%i in ('dir *.nuspec /s/b') do (
    call :BUILD_NUGET_PACKAGE_RELEASE %%i
)

nuget pack GitDepend.CommandLine.nuspec -OutputDirectory $(NUGET_OUTPUT_DIRECTORY)\Release -IncludeReferencedProjects -Properties Configuration=Release -Symbols -Version %VERSION%
nuget pack GitDepend.Portable.nuspec -OutputDirectory $(CHOCOLATEY_OUTPUT_DIRECTORY)\Release -Properties Configuration=Release -Version %VERSION%

move /Y $(NUGET_OUTPUT_DIRECTORY)\Release\*.symbols.nupkg $(NUGET_OUTPUT_DIRECTORY)\Release\Symbols

goto :eof

:BUILD_NUGET_PACKAGE_RELEASE
set file=%1
set file=%file:nuspec=csproj%
echo %file%

if not exist "%file%" goto :eof

nuget.exe pack %file% -OutputDirectory $(NUGET_OUTPUT_DIRECTORY)\Release -IncludeReferencedProjects -Properties Configuration=Release -Symbols -Version %VERSION%

goto :eof
<<

    @echo ##teamcity[blockClosed name='Generate NuGet Packages (Release)']

reset:
    @git checkout *\AssemblyInfo.cs
    @if exist version.txt del version.txt

documentation: version
    @echo ##teamcity[blockOpened name='Generate Documentation']

    @call <<docs.bat
@echo off
pushd .

cd docs
call make.bat html

popd
<<

    @echo ##teamcity[blockClosed name='Generate Documentation']

clean:
    @if exist docs\build rd /Q /S docs\build
    @if exist packages rd /Q /S packages
    @if exist artifacts rd /Q /S artifacts
    @if exist Reports rd /Q /S Reports
    @if exist version.txt del version.txt

    @msbuild $(SLN) /m /t:Clean /p:Configuration=Debug
    @msbuild $(SLN) /m /t:Clean /p:Configuration=Release