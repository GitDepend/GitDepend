@echo off

start http://localhost:8080
sphinx-autobuild source build\html -p 8080 -H 0.0.0.0