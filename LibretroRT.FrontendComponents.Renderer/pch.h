#pragma once

#include <collection.h>
#include <ppltasks.h>
#include <concrt.h>

#include <chrono>
#include <thread>

// DirectX headers.
#include <d3d11.h>
#include <d2d1_3.h>
#include <d2d1_2.h>
#include <d2d1helper.h>
#include <DirectXMath.h>
#include <Windows.Graphics.DirectX.Direct3D11.interop.h>

// Win2D headers.
#include <Microsoft.Graphics.Canvas.native.h>

// Enable function definitions in the GL headers below
#define GL_GLEXT_PROTOTYPES
#define EGL_EGLEXT_PROTOTYPES

// OpenGL ES includes
#include <GLES2/gl2.h>
#include <GLES2/gl2ext.h>

// EGL includes
#include <EGL/egl.h>
#include <EGL/eglext.h>
#include <EGL/eglplatform.h>

#include <angle_windowsstore.h>