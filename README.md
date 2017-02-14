# GitDepend

[![Build status][appveyor-badge]][appveyor]

|                            |                Stable               |                 Pre-release               |
| -------------------------: | :---------------------------------: | :---------------------------------------: |
|                  **Docs**  |     [![Docs][docs-badge]][docs]     |    [![Docs][docs-pre-badge]][docs-pre]    |
|        **GitHub Release**  |                 -                   | [![GitHub release][gh-rel-badge]][gh-rel] |
|    **GitDepend.Portable**  | [![Chocolatey][choco-badge]][choco] |   [![Chocolatey][choco-pre-badge]][choco] |
| **GitVersion.CommandLine** |     [![NuGet][gdc-badge]][gdc]      |       [![NuGet][gdc-pre-badge]][gdc]      |

Solves the problem of working with multiple git repositories where lower level repositories produce nuget packages that are consumed by other repositories.

## Why do I need GitDepend
I work for a company that produces a lot of in-house nuget packages. Originally we tried maintaining the version of these packages
manually. If you've ever tried doing that on a large scale you realize that it quickly gets unmanageable. It's easy to make mistakes
with your versioning. It's easy to forget to bump the version. In short, versioning is something that should really be automated.

[GitVersion](https://github.com/GitTools/GitVersion) to the rescue! With GitVersion we could just write code, and our packages
magically version themselves correctly. BUT, there was still a big problem. We didn't like lower level packages having to bump
their version just because another package had to change. GitVersion assumes the same version for the entire repository. So, if you
need a unique version so that your code can stabalize on a version you need to split it into another git repository.

We started doing this... a LOT. Before we knew it the setup process to get our full process building involved checking out multiple
repositories. Making sure that each one was on the appropriate branch for what we were doing, and writing a complicated build
script that lived in the upper most repository to build all other repositories. As we added new repositories that build script
was constantly changing, and getting more complicated. We needed a solution that made it easy to chain repositories together. Thus
GitDepend was born.

## How does it work?

You should read the full [documentation](http://gitdepend.readthedocs.io/en/latest/) for a more comprehensive overview. In a nutshell GitDepend works by declaring
what it takes to build a repository, where nuget packages will be located after a successful build, and what the direct
git repository dependencies are.

In the root of your repository you will include a `GitDepend.json` file

```json
{
  "build": {
    "script": "make.bat"
  },
  "packages": {
    "dir": "artifacts/NuGet/Debug"
  },
  "dependencies": [
    {
      "name": "Lib1",
      "url": "git@github.com:kjjuno/Lib1.git",
      "dir": "../Lib1",
      "branch": "develop"
    }
  ]
}
```

Normally if you are working in an upper level repository you should just be able to run the build script and rely on nuget packages.
However, when you have changed code in a lower level repository you will need to have those changes cascade up the chain. This
is where GitDepend shines. Run the following command

```bash
GitDepend.exe update
```

This will follow the chain of `GitDepend.json` files. The following things will happen

1. Check out the dependency if it has not been checked out.
2. Ensure that the repository is on the correct branch.
3. update all dependencies (this is a recursive step)
4. consume the latest nuget packages produced by dependency repositories.

At this point the upper level repository should be all up to date, targetting the latest nuget packages and be ready to build.

## Try it out!

Take a look at some example projects and try it out for yourself.

* [Lib1](https://github.com/kjjuno/Lib1)
* [Lib2](https://github.com/kjjuno/Lib2)

Lib2 depends on Lib1

Clone Lib2

```cmd
git clone git@github.com:kjjuno/Lib2.git
```

from the root of Lib2 run

```cmd
make.bat update
```

This will clone and build all dependencies

build it with
```cmd
make.bat
```

Now, make a change in Lib1 and commit that change.

```cmd
make.bat update
```

This will build the new changes in Lib1 and pull them into Lib2. You can now use the new changes.

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
