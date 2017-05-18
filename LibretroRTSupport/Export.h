#pragma once

#ifndef SUPPORT_API
#ifdef SUPPORT_EXPORT_SYMBOLS
#define SUPPORT_API __declspec(dllexport)
#else
#define SUPPORT_API __declspec(dllimport)
#endif
#endif