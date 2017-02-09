# GitDepend

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
