#pragma once

#ifndef SUPPORT_API
#ifdef CORETOOLSEXPORT
#define SUPPORT_API __declspec(dllexport)
#else
#define SUPPORT_API __declspec(dllimport)
#endif
#endif