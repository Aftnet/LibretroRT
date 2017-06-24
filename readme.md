# LibretroRT & RetriX

LibretroRT is a framework to enable porting of Libretro cores to WinRT components.

This should enable creating native UWP emulator front ends using high the high quality open source Libretro cores available.

## Demo

[![Youtube link](https://img.youtube.com/vi/1mzS54HhcEM/0.jpg)](https://youtu.be/1mzS54HhcEM)

Click image to play video

## Help wanted

I am accepting donations on [gofundme](https://www.gofundme.com/retrix) to help with development costs.
At minimum, I would need:

- An Authenticode certificate to do binary releases
- An Xbox One to develop/test on

## Design goals

- Full compliance with UWP sandboxing
- Having cores render to SwapChain panels, allowing [XAML and DirectX interop](https://docs.microsoft.com/en-us/windows/uwp/gaming/directx-and-xaml-interop) and front ends to use native UWP controls for the UI
- Support Windows 10 on all platforms (Desktop, Mobile, Xbox, VR) and architectures (x86, x64, ARM)
- Allow packaging and distribution via NuGet for a great development experience

## Project aim

- Creating a UWP Libretro front end for a better experience (proper DPI scaling, native look and feel, fullscreen as borderless window, integration with Windows's modern audio pipeline)
- Increasing safety by having emulation code run sandboxed
- Create a meaningful use case for UWP sideloading on Windows, since Microsoft has decided to ban emulators from the store

## Current state

- Created a framework to speed up porting of software rendering based Libretro cores to WinRT components
- Ported GenesisPlusGX, Snes9x, FCEUMM, Nestopia (doesn't work well, using FCEUMM instead), VBAM, Ganbatte (unstable)
- Created audio player WinRT components to interop between Libretro's audio rendering interface and Windows 10's [AudioGraph](https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/audio-graphs) API
- Created input manager WinRT component to interop between Libretro's input polling interface and Windows 10's [Gamepad APIs](https://docs.microsoft.com/en-us/uwp/api/windows.gaming.input.gamepad)
- Created [Win2D](https://github.com/Microsoft/Win2D) based video renderer. Supports software based Libretro cores.
- Created RetriX, a native XAML based Libretro front end, with an UI optimized for mouse, touch as well as gamepad interaction: it scales from phones to tablets, traditional PC form factors as well as the Xbox One.
- Implemented a way to virtualize file system access from within cores: allows opening zipped games and those made up of multiple files
- Working on PlayStation support

## Roadmap

- Expand framework to allow porting of OpenGL based cores to WinRT using [Angle](https://github.com/Microsoft/angle) while still allowing front ends to be written in languages other than C++
- Port more Libretro cores
- Split LibretroRT (Libretro core ports to UWP) from RetriX, distribute the former via Nuget package, the latter via direct appx downlad in addition to source availability
