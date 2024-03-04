# Lottie Player
Player for [Lottie](https://airbnb.io/lottie) animations, powered by [rlottie](https://github.com/Samsung/rlottie).


## Features
- Scripted importer that interprets JSON files as [LottieAnimationAsset](Runtime/LottieAnimationAsset.cs)s.
  This helps you separate animation files from regular JSON ones and makes sure the asset contains valid animations.
- [ImageLottiePlayer](Runtime/UI/ImageLottiePlayer.cs): a Unity UI component that plays Lottie animation assets, rendering to a texture of customizable size.
- [Job System](https://docs.unity3d.com/Manual/JobSystemOverview.html)-friendly: texture updates may run in background threads.
  Use `ILottieAnimation.CreateRenderJob(...)` extension method for scheduling an animation render job.
