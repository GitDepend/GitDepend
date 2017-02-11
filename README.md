# GitDepend

[![Documentation Status](https://readthedocs.org/projects/gitdepend/badge/?version=latest)](http://gitdepend.readthedocs.io/en/latest/?badge=latest)

This is a tool designed to deal with a large project split into multiple git repositories using nuget packages to tie libraries together.

This is very much a work in progress.

```
script: make.bat
packages: artifacts/NuGet/Debug
dependencies:
  dependencyA:
    url: ssh://git@github.com:kjjuno/dependencyA.git
    dir: ../dependencyA
    branch: develop
```

Steps that happen during a call to `update`

```
For each dependency:  
    * Check to see if the directory exists
    * Check out the dependency repository if needed
    * ensure the correct branch has been checked out
    * look for a `GitDepend.yml` file
    * if one exists recursively call `update` on each dependency
    * update to the latest nuget packages

* make update commit (if dependencies changed)
* run the build script (must produce packages)
```

## v0.2.0
Minimum Viable Product implementation

* init flag assists with setting up `GitDepend.json` files
* clone flag recursively clones all dependencies
* update flag recursively executes the build script and updates all projects to use the artifacts produced by dependency build scripts.
* config flag shows computed configuration

## v0.1.0
Simple setup for the sole purpose of reservering names on github.com and nuget.org
