Why do I need GitDepend?
========================

I work for a company that produces a lot of in-house nuget packages. Originally we tried maintaining the version of these packages
manually. If you've ever tried doing that on a large scale you realize that it quickly gets unmanageable. It's easy to make mistakes
with your versioning. It's easy to forget to bump the version. In short, versioning is something that should really be automated.

`GitVersion <https://github.com/GitTools/GitVersion>`_ to the rescue! With GitVersion we could just write code, and our packages
magically version themselves correctly. BUT, there was still a big problem. We didn't like lower level packages having to bump
their version just because another package had to change. GitVersion assumes the same version for the entire repository. So, if you
need a unique version so that your code can stabilize on a version you need to split it into another git repository.

We started doing this... a LOT. Before we knew it the setup process to get our full process building involved checking out multiple
repositories. Making sure that each one was on the appropriate branch for what we were doing, and writing a complicated build
script that lived in the upper most repository to build all other repositories. As we added new repositories that build script
was constantly changing, and getting more complicated. We needed a solution that made it easy to chain repositories together. Thus
GitDepend was born.
