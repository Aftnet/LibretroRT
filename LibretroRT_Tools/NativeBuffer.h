#pragma once

#include <wrl.h>
#include <wrl/implements.h>
#include <windows.storage.streams.h>
#include <robuffer.h>
#include <vector>

// todo: namespace

namespace LibretroRT_Tools
{
	class NativeBuffer :
		public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
		ABI::Windows::Storage::Streams::IBuffer,
		Windows::Storage::Streams::IBufferByteAccess>
	{
	public:
		virtual ~NativeBuffer()
		{
		}

		STDMETHODIMP RuntimeClassInitialize(byte *buffer, UINT32 totalSize)
		{
			m_length = totalSize;
			m_buffer = buffer;

			return S_OK;
		}

		STDMETHODIMP Buffer(byte **value)
		{
			*value = m_buffer;

			return S_OK;
		}

		STDMETHODIMP get_Capacity(UINT32  *value)
		{
			*value = m_length;

			return S_OK;
		}

		STDMETHODIMP get_Length(UINT32  *value)
		{
			*value = m_length;

			return S_OK;
		}

		STDMETHODIMP put_Length(UINT32  value)
		{
			m_length = value;

			return S_OK;
		}

	private:
		UINT32  m_length;
		byte *m_buffer;
	};

	Windows::Storage::Streams::IBuffer ^CreateNativeBuffer(void* lpBuffer, size_t nNumberOfBytes)
	{
		Microsoft::WRL::ComPtr<LibretroRT_Tools::NativeBuffer> nativeBuffer;
		Microsoft::WRL::Details::MakeAndInitialize<LibretroRT_Tools::NativeBuffer>(&nativeBuffer, (byte *)lpBuffer, nNumberOfBytes);
		auto iinspectable = (IInspectable *)reinterpret_cast<IInspectable *>(nativeBuffer.Get());
		Windows::Storage::Streams::IBuffer ^buffer = reinterpret_cast<Windows::Storage::Streams::IBuffer ^>(iinspectable);

		return buffer;
	}
}