# Lottie Player
[![openupm](https://img.shields.io/npm/v/com.gilzoide.lottie-player?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gilzoide.lottie-player/)

Player for [Lottie](https://airbnb.io/lottie) animations, powered by [rlottie](https://github.com/Samsung/rlottie).


## Features
- Scripted importer that interprets JSON files as [LottieAnimationAsset](Runtime/LottieAnimationAsset.cs)s.
  This helps you separate animation files from regular JSON ones and makes sure the asset contains valid animations.
- [ImageLottiePlayer](Runtime/UI/ImageLottiePlayer.cs): a Unity UI component that plays Lottie animation assets, rendering to a texture of customizable size.
- [Job System](https://docs.unity3d.com/Manual/JobSystemOverview.html)-friendly: texture updates may run in background threads.
  Use `ILottieAnimation.CreateRenderJob(...)` extension method for scheduling an animation render job.
- Supported platforms: Windows, Linux, macOS, iOS, tvOS, visionOS, Android, WebGL


## How to install
Either:
- Use the [openupm registry](https://openupm.com/) and install this package using the [openupm-cli](https://github.com/openupm/openupm-cli):
  ```
  openupm add com.gilzoide.lottie-player
  ```
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-lottie-player.git#1.0.0-preview1
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.