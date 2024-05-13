# TDF - Total Dialoge Framework

[Japanese/日本語](README_ja.md)

## Overview

TDF (Total Dialoge Framework) is a Conversation Engine asset for implementing conversation scenarios in Unity applications

Asynchronous processing including basic conversation functions and some animation can be written in a simple script or visual environment.

## Feature

Describes the main characteristics or benefits of the asset in list form.

- Modular structure

  TDF is characterized by modularizing all the conversation boxes, selections, and various UIs that tend to be used in conversation scenes.

  The scene creator is free to place only the Driver (the core part that provides the basic control) and the parts needed for the scene.

- Asynchronous processing

  In a conversation scene, it is necessary to animate the elements in the screen, display the conversation, and in some cases, move various gauges, images, 3D objects, etc. asynchronously.

  TDF makes it relatively easy to control these asynchronous operations.

- Rich binding environment

  As a standard, TDF can be controlled by a language called TDForth, which is based on the Forth language, and a language called TDFScript, which is more specialized for describing conversational scenes.

  In addition, BeXide's ScenarioBuilder Visual Control System supports visual control.

  Of course, the control mechanism can be controlled by a C #script.

  It is still in the planning stage, but we are also planning bindings for other languages (python/Miniscript etc.)

- Dictionary driven design

  TDF works on the basis of event-driven by a dictionary in ScriptableObject with an event-driven mechanism

  The Listen/Event structure in ScriptableObject is flexible, but the downside is that the ScriptableObject is scattered throughout the project.

  In TDF, to reduce this, each element has a Dictionary with a Listen/Fire structure, which provides integrated control.

## Required environment

- Unity 2022.3 LTS or later

  The base functionality should work in earlier versions, but the extensions use the packages available since 2022.
  Unit Tests require 2022.3 LTS

- [UniTask]()

  Get it in Asset Store / Github or install it in OpenUPM

- [DoTween]()

  Get it from the Asset Store

## How to install

Describes how to install the asset.

## Usage

Describe the basic use of the asset. If possible, include screenshots and code snippets to make it easier to understand.

## Live Demo

- [WebGL Demo]()
  This is a basic live demo.

## FAQ

Lists frequently asked questions and their answers.

## License

All Codes are MIT, but Some Assets in Demo have Own Open Licenses.
See LICENSE.TXT in Assets folders.
Includes [ScenarioBuilder]() 's code with some patches (under MIT License)(not yet)

## Contact

Provide contact information and feedback.
