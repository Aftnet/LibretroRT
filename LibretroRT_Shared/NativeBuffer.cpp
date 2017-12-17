#include "pch.h"
#include "NativeBuffer.h"

Windows::Storage::Streams::IBuffer^ LibretroRT_Shared::CreateNativeBuffer(void* lpBuffer, size_t nNumberOfBytes)
{
	Microsoft::WRL::ComPtr<LibretroRT_Shared::NativeBuffer> nativeBuffer;
	Microsoft::WRL::Details::MakeAndInitialize<LibretroRT_Shared::NativeBuffer>(&nativeBuffer, (byte *)lpBuffer, nNumberOfBytes);
	auto iinspectable = (IInspectable *)reinterpret_cast<IInspectable *>(nativeBuffer.Get());
	Windows::Storage::Streams::IBuffer ^buffer = reinterpret_cast<Windows::Storage::Streams::IBuffer ^>(iinspectable);

	return buffer;
}