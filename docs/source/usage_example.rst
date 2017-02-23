Usage Example
=============

In the root of your repository you will include a ``GitDepend.json`` file

.. code-block:: json

    {
      "name": "Lib2",
      "build": {
        "script": "make.bat"
      },
      "packages": {
        "dir": "artifacts/NuGet/Debug"
      },
      "dependencies": [
        {
          "url": "git@github.com:kjjuno/Lib1.git",
          "dir": "../Lib1",
          "branch": "develop"
        }
      ]
    }

Normally if you are working in an upper level repository you should just be able to run the build script and rely on nuget packages.
However, when you have changed code in a lower level repository you will need to have those changes cascade up the chain. This
is where GitDepend shines. Run the following command

.. code-block:: bash

    GitDepend.exe update

This will follow the chain of ``GitDepend.json`` files. The following things will happen

1. Check out the dependency if it has not been checked out.
2. Ensure that the repository is on the correct branch.
3. update all dependencies (this is a recursive step)
4. consume the latest nuget packages produced by dependency repositories.

At this point the upper level repository should be all up to date, targetting the latest nuget packages and be ready to build.

Try it out!
-----------

Take a look at some example projects and try it out for yourself.

* `Lib1 <https://github.com/kjjuno/Lib1/>`_
* `Lib2 <https://github.com/kjjuno/Lib1/>`_

Lib2 depends on Lib1

Clone Lib2

.. code-block:: bash

    git clone git@github.com:kjjuno/Lib2.git

from the root of Lib2 run

.. code-block:: bash

    make.bat update

This will clone and build all dependencies

build it with

.. code-block:: bash

    make.bat

Now, make a change in Lib1 and commit that change.

.. code-block:: bash

    make.bat update