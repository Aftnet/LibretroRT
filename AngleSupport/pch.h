#pragma once

#include <memory>
#include <Windows.h>

#include <concrt.h>
#include <ppltasks.h>

#include <string>
#include <codecvt>

// Enable function definitions in the GL headers below
#define GL_GLEXT_PROTOTYPES

// OpenGL ES includes
#include <GLES2/gl2.h>
#include <GLES2/gl2ext.h>

// EGL includes
#include <EGL/egl.h>
#include <EGL/eglext.h>
#include <EGL/eglplatform.h>

#include <angle_windowsstore.h>