#include "pch.h"
#include "RenderTargetManager.h"

using namespace LibretroRT_FrontendComponents_Renderer;

using namespace Windows::Graphics::DirectX::Direct3D11;

const std::map<PixelFormats, DXGI_FORMAT> RenderTargetManager::LibretroToDXGITextureFormatsMapping
{
	{ PixelFormats::FormatRGB565, DXGI_FORMAT::DXGI_FORMAT_B5G6R5_UNORM },
	{ PixelFormats::FormatXRGB8888, DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM },
	{ PixelFormats::Format0RGB1555, DXGI_FORMAT::DXGI_FORMAT_UNKNOWN },
	{ PixelFormats::FormatUknown, DXGI_FORMAT::DXGI_FORMAT_UNKNOWN },
};

RenderTargetManager::RenderTargetManager(CanvasAnimatedControl^ canvas) :
	Canvas(canvas),
	OpenGLESManager(OpenGLES::GetInstance())
{
	__abi_ThrowIfFailed(GetDXGIInterface(canvas->Device, Device.GetAddressOf()));
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
	if (D3DTexture)
	{
		D3D11_TEXTURE2D_DESC description;
		D3DTexture->GetDesc(&description);
		shouldUpdate = (description.Width < geometry->MaxWidth || description.Height < geometry->MaxHeight);
	}

	if (shouldUpdate)
	{
		auto dimension = max(geometry->MaxWidth, geometry->MaxHeight);
		dimension = max(dimension, RenderTargetMinSize);
		dimension = ClosestGreaterPowerTwo(dimension);
		CreateRenderTargets(Device, dimension, dimension);
	}
}

void RenderTargetManager::UpdateFromCoreOutput(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{
	critical_section::scoped_lock lock(RenderTargetCriticalSection);

	ID3D11DeviceContext* context;
	Device->GetImmediateContext(&context);
	
}

void RenderTargetManager::Render(CanvasDrawingSession^ drawingSession, Size canvasSize)
{
	if (D3DTexture == nullptr || RenderTargetViewport.Width <= 0 || RenderTargetViewport.Height <= 0)
	{
		return;
	}

	critical_section::scoped_lock lock(RenderTargetCriticalSection);

	auto destinationRect = ComputeBestFittingSize(canvasSize, Geometry->AspectRatio);
	drawingSession->DrawImage(RenderTarget, destinationRect, RenderTargetViewport);
}

void RenderTargetManager::CreateRenderTargets(ComPtr<ID3D11Device> device, unsigned int width, unsigned int height)
{
	DestroyRenderTargets();

	D3D11_TEXTURE2D_DESC texDesc = { 0 };
	texDesc.Width = width;
	texDesc.Height = height;
	texDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	texDesc.MipLevels = 1;
	texDesc.ArraySize = 1;
	texDesc.SampleDesc.Count = 1;
	texDesc.SampleDesc.Quality = 0;
	texDesc.Usage = D3D11_USAGE_DEFAULT;
	texDesc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
	texDesc.CPUAccessFlags = 0;
	texDesc.MiscFlags = D3D11_RESOURCE_MISC_SHARED;

	ComPtr<ID3D11Texture2D> d3dTexture;
	__abi_ThrowIfFailed(device->CreateTexture2D(&texDesc, nullptr, d3dTexture.GetAddressOf()));
	D3DTexture = d3dTexture;

	OpenGLESSurface = OpenGLESManager->CreateSurface(D3DTexture);
	OpenGLESManager->CreateTextureFromSurface(OpenGLESSurface);
}

void RenderTargetManager::DestroyRenderTargets()
{
	if (OpenGLESSurface != EGL_NO_SURFACE)
	{
		OpenGLESManager->DestroySurface(OpenGLESSurface);
		OpenGLESSurface = EGL_NO_SURFACE;
	}

	D3DTexture.Reset();
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