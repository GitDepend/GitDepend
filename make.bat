@echo off

if /i "%1%" == "" (
    goto :NMAKE
) else if /i "%1%" == "debug" (
    goto :NMAKE
) else if /i "%1%" == "test" (
    goto :NMAKE
) else if /i "%1%" == "cov" (
    goto :NMAKE
) else if /i "%1%" == "prod" (
    goto :NMAKE
) else if /i "%1%" == "doc" (
    goto :NMAKE
) else if /i "%1%" == "all" (
    goto :NMAKE
) else if /i "%1%" == "teamcity" (
    goto :NMAKE
) else if /i "%1%" == "clean" (
    goto :NMAKE
)
) else (
    goto :SHOW_USAGE
)

:NMAKE

set OLD_PATH=%PATH%
call "%VS140COMNTOOLS%VsDevCmd.bat"

nmake %*

set PATH=%OLD_PATH%

goto :eof

:SHOW_USAGE

echo make.bat [target]
echo.
echo Valid modes:
echo   debug       : Only builds debug artifacts [Default]
echo   test        : Runs Unit Tests
echo   cov         : Runs Unit Tests and captures code coverage
echo   prod        : Only builds release artifacts
echo   doc         : Build the documentation
echo   all         : Builds both debug and release artifacts, runs unit tests,
echo                 and captures coverage information
echo   teamcity    : Runs the full build (same as all) but does not generate
echo                 html reports
echo   clean       : Cleans the build