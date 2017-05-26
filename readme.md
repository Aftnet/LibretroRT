# LibretroRT

LibretroRT is a framework to enable porting of Libretro cores to WinRT components.

This should enable creating native UWP emulator front ends using high the high quality open source Libretro cores available.

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
- Ported GenesisPlusGX core to WinRT
- Created audio player WinRT components to interop between Libretro's audio rendering interface and Windows 10's [AudioGraph](https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/audio-graphs) API
- Created input manager WinRT component to interop between Libretro's input polling interface and Windows 10's [Gamepad APIs](https://docs.microsoft.com/en-us/uwp/api/windows.gaming.input.gamepad)
- Created minimal C# [Monogame](http://www.monogame.net/) based front end for testing

## Roadmap

- Expand framework to allow porting of OpenGL based cores to WinRT using [Angle](https://github.com/Microsoft/angle), ideally still allowing front ends to be written in langiuages other than C++
- Create a more full-featured front end, without using Monogame
- Port more Libretro cores