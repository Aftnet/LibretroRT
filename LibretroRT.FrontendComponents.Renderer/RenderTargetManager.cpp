#include "pch.h"
#include "RenderTargetManager.h"

using namespace LibretroRT_FrontendComponents_Renderer;

using namespace Windows::Graphics::DirectX::Direct3D11;

RenderTargetManager::RenderTargetManager(CanvasAnimatedControl^ canvas) :
	Canvas(canvas),
	OpenGLESManager(OpenGLES::GetInstance())
{
}


RenderTargetManager::~RenderTargetManager()
{
	DestroyRenderTargets();
}

void RenderTargetManager::UpdateFormat()
{
	if (Geometry == nullptr)
	{
		return;
	}

	critical_section::scoped_lock lock(RenderTargetCriticalSection);

	bool shouldUpdate = true;
	if (Win2DTexture)
	{
		auto size = Win2DTexture->SizeInPixels;
		shouldUpdate = (size.Width < geometry->MaxWidth || size.Height < geometry->MaxHeight);
	}

	if (shouldUpdate)
	{
		auto dimension = max(geometry->MaxWidth, geometry->MaxHeight);
		dimension = max(dimension, RenderTargetMinSize);
		dimension = ClosestGreaterPowerTwo(dimension);
		CreateRenderTargets(Canvas, dimension, dimension);
	}
}

void RenderTargetManager::UpdateFromCoreOutput(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{
	RenderTargetViewport.Width = width;
	RenderTargetViewport.Height = height;

	critical_section::scoped_lock lock(RenderTargetCriticalSection);
	
	if (usingHardwareRendering)
	{
		OpenGLESManager->MakeCurrent(OpenGLESSurface);
		glClearColor(1.0f, 0.0f, 0.0f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT);
		glFlush();
	}
	else
	{
		ComPtr<ID3D11Device> d3dDevice;
		__abi_ThrowIfFailed(GetDXGIInterface(Canvas->Device, d3dDevice.GetAddressOf()));
		ComPtr<ID3D11DeviceContext> d3dContext;
		d3dDevice->GetImmediateContext(d3dContext.GetAddressOf());

		ComPtr<ID3D11Resource> d3dR3source;
		__abi_ThrowIfFailed(Direct3DTexture.As(&d3dR3source));
		D3D11_MAPPED_SUBRESOURCE mappedResource;
		__abi_ThrowIfFailed(d3dContext->Map(d3dR3source.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource));

		auto pSrc = frameBuffer->Data;
		auto pDest = (unsigned char*)mappedResource.pData;
		const size_t rgba8888bpp = 4;
		for (auto i = 0; i < height; i++)
		{
			switch (PixelFormat)
			{
			case PixelFormats::FormatXRGB8888:
			{
				memcpy(pDest, pSrc, width * rgba8888bpp);
				pSrc += pitch;
				pDest += mappedResource.RowPitch;
			}
			break;
			case PixelFormats::FormatRGB565:
			{

			}
			break;
			}
		}
		

		d3dContext->Unmap(d3dR3source.Get(), 0);
	}
}

void RenderTargetManager::Render(CanvasDrawingSession^ drawingSession, Size canvasSize)
{
	if (Win2DTexture == nullptr || RenderTargetViewport.Width <= 0 || RenderTargetViewport.Height <= 0)
	{
		return;
	}

	critical_section::scoped_lock lock(RenderTargetCriticalSection);

	auto destinationRect = ComputeBestFittingSize(canvasSize, Geometry->AspectRatio);
	drawingSession->DrawImage(Win2DTexture, destinationRect, RenderTargetViewport);
}

void RenderTargetManager::CreateRenderTargets(CanvasAnimatedControl^ canvas, unsigned int width, unsigned int height)
{
	DestroyRenderTargets();

	ComPtr<ID3D11Device> d3dDevice;
	__abi_ThrowIfFailed(GetDXGIInterface(canvas->Device, d3dDevice.GetAddressOf()));

	ComPtr<IDXGISurface> d3dSurface;

	if (usingHardwareRendering)
	{
		OpenGLESSurface = OpenGLESManager->CreateSurface(width, height, EGL_TEXTURE_RGBA);
		auto surfaceHandle = OpenGLESManager->GetSurfaceShareHandle(OpenGLESSurface);

		__abi_ThrowIfFailed(d3dDevice->OpenSharedResource(surfaceHandle, __uuidof(IDXGISurface), (void**)d3dSurface.GetAddressOf()));
	}
	else
	{
		D3D11_TEXTURE2D_DESC texDesc = { 0 };
		texDesc.Width = width;
		texDesc.Height = height;
		texDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
		texDesc.MipLevels = 1;
		texDesc.ArraySize = 1;
		texDesc.SampleDesc.Count = 1;
		texDesc.SampleDesc.Quality = 0;
		texDesc.Usage = D3D11_USAGE_DYNAMIC;
		texDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
		texDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
		texDesc.MiscFlags = 0;

		__abi_ThrowIfFailed(d3dDevice->CreateTexture2D(&texDesc, nullptr, Direct3DTexture.GetAddressOf()));
		__abi_ThrowIfFailed(Direct3DTexture.As(&d3dSurface));
	}

	auto winRTSurface = CreateDirect3DSurface(d3dSurface.Get());
	Win2DTexture = CanvasBitmap::CreateFromDirect3D11Surface(canvas->Device, winRTSurface);
}

void RenderTargetManager::DestroyRenderTargets()
{
	Win2DTexture = nullptr;
	Direct3DTexture.Reset();

	if (OpenGLESSurface != EGL_NO_SURFACE)
	{
		OpenGLESManager->DestroySurface(OpenGLESSurface);
		OpenGLESSurface = EGL_NO_SURFACE;
	}
}

Rect RenderTargetManager::ComputeBestFittingSize(Size viewportSize, float aspectRatio)
{
	auto candidateWidth = std::floor(viewportSize.Height * aspectRatio);
	if (viewportSize.Width >= candidateWidth)
	{
		Size size(candidateWidth, viewportSize.Height);
		Rect output(Point((viewportSize.Width - candidateWidth) / 2, 0), size);
		return output;
	}
	else
	{
		auto height = viewportSize.Width / aspectRatio;
		Size size(viewportSize.Width, height);
		Rect output(Point(0, (viewportSize.Height - height) / 2), size);
		return output;
	}
}

unsigned int RenderTargetManager::ClosestGreaterPowerTwo(unsigned int value)
{
	unsigned int output = 1;
	while (output < value)
	{
		output *= 2;
	}

	return output;
}