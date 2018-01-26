#pragma once

using namespace Microsoft::Graphics::Canvas;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Graphics::DirectX::Direct3D11;

namespace Retrix
{
	namespace UWP
	{
		namespace Native
		{
#ifdef _WIN64
			typedef int64 UnsafeIntPtr;
#else
			typedef int32 UnsafeIntPtr;
#endif

			public ref class D3DSurfaceMap sealed
			{
			private:
				const UINT32 pitch;
				const UnsafeIntPtr data;

			public:
				property UINT32 Pitch { UINT32 get() { return pitch; } }
				property UnsafeIntPtr Data { UnsafeIntPtr get() { return data; } }

			internal:
				D3DSurfaceMap(D3D11_MAPPED_SUBRESOURCE input):
					pitch(input.RowPitch),
					data((UnsafeIntPtr)input.pData)
				{
				}
			};

			public ref class D3DSurfaceManager sealed
			{
			public:
				static IDirect3DSurface^ CreateWriteableD3DSurface(CanvasDevice^ device, unsigned int width, unsigned int height);
				static D3DSurfaceMap^ Map(CanvasDevice^ device, IDirect3DSurface^ surface);
				static void Unmap(CanvasDevice^ device, IDirect3DSurface^ surface);
			};
		}
	}
}
