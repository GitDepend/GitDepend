# GitDepend

Solves the problem of working with multiple git repositories where lower level repositories produce nuget packages that are consumed by other repositories.

[![Join the chat at https://gitter.im/GitDepend/Lobby][gitter]][gitter-badge]
[![Build status][appveyor-badge]][appveyor]

|                            |                Stable               |                 Pre-release               |
| -------------------------: | :---------------------------------: | :---------------------------------------: |
|                  **Docs**  |     [![Docs][docs-badge]][docs]     |    [![Docs][docs-pre-badge]][docs-pre]    |
|        **GitHub Release**  |                 -                   | [![GitHub release][gh-rel-badge]][gh-rel] |
|    **GitDepend.Portable**  | [![Chocolatey][choco-badge]][choco] |   [![Chocolatey][choco-pre-badge]][choco] |
| **GitVersion.CommandLine** |     [![NuGet][gdc-badge]][gdc]      |       [![NuGet][gdc-pre-badge]][gdc]      |


## Quick Links

* [Why do I need GitDepend](http://gitdepend.readthedocs.io/en/latest/why.html)
* [How does it work?](http://gitdepend.readthedocs.io/en/latest/usage_example.html)

## v0.3.0
* NuGet packages are now restored before running the NuGet update.
* All branches and pull requests are now built automatically on appveyor
* Unit tests have been added
* Documentation has been updated to at least be somewhat helpful

## v0.2.1
Fixes an issue where installing GitDepend from chocolatey while using an administrative shell prevented GitDepend from
collection dependency nuget packages.

## v0.2.0
Minimum Viable Product implementation

* init flag assists with setting up `GitDepend.json` files
* clone flag recursively clones all dependencies
* update flag recursively executes the build script and updates all projects to use the artifacts produced by dependency build scripts.
* config flag shows computed configuration

## v0.1.0
Simple setup for the sole purpose of reservering names on github.com and nuget.org

[gitter]:          https://badges.gitter.im/GitDepend/Lobby.svg
[gitter-badge]:    https://gitter.im/GitDepend/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[appveyor]:        https://ci.appveyor.com/project/kjjuno/gitdepend/branch/develop
[appveyor-badge]:  https://ci.appveyor.com/api/projects/status/github/kjjuno/GitDepend?branch=develop&svg=true
[docs]:            http://gitdepend.readthedocs.org/en/stable/
[docs-badge]:      https://readthedocs.org/projects/gitdepend/badge/?version=stable
[docs-pre]:        http://gitdepend.readthedocs.org/en/latest/
[docs-pre-badge]:  https://readthedocs.org/projects/gitdepend/badge/?version=latest
[gh-rel]:          https://github.com/kjjuno/GitDepend/releases/latest
[gh-rel-badge]:    https://img.shields.io/github/release/kjjuno/gitdepend.svg
[choco]:           https://chocolatey.org/packages/GitDepend.Portable
[choco-badge]:     https://img.shields.io/chocolatey/v/gitepend.portable.svg
[choco-pre-badge]: https://img.shields.io/chocolatey/vpre/gitdepend.portable.svg
[gdc]:             https://www.nuget.org/packages/GitDepend.CommandLine
[gdc-badge]:       https://img.shields.io/nuget/v/GitDepend.CommandLine.svg
[gdc-pre-badge]:   https://img.shields.io/nuget/vpre/GitDepend.CommandLine.svg
