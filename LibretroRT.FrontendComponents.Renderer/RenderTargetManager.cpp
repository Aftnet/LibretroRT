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
	Canvas(canvas)
{
	__abi_ThrowIfFailed(GetDXGIInterface(canvas->Device, Device.GetAddressOf()));
}


RenderTargetManager::~RenderTargetManager()
{
}

void RenderTargetManager::UpdateFormat(GameGeometry^ geometry, PixelFormats pixelFormat)
{
	auto maxDimension = max(geometry->MaxWidth, geometry->MaxHeight);
	auto requestedFormat = LibretroToDXGITextureFormatsMapping.find(pixelFormat)->second;

	bool shouldUpdate = true;
	if (D3DRenderTarget)
	{
		D3D11_TEXTURE2D_DESC description;
		D3DRenderTarget->GetDesc(&description);
		shouldUpdate = (description.Format != requestedFormat || description.Width < maxDimension);
	}

	if (shouldUpdate)
	{
		auto dimension = max(maxDimension, RenderTargetMinSize);
		dimension = ClosestGreaterPowerTwo(dimension);
		CreateD3DTexture(Device, requestedFormat, dimension, dimension);
	}
}

void UpdateRenderTarget()
{

}

void RenderTargetManager::UpdateFromCoreOutput(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{
}

void RenderTargetManager::Render(CanvasDrawingSession^ drawingSession, Size canvasSize)
{
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


ComPtr<ID3D11Texture2D> RenderTargetManager::CreateD3DTexture(ComPtr<ID3D11Device> device, DXGI_FORMAT format, unsigned int width, unsigned int height)
{
	D3D11_TEXTURE2D_DESC texDesc = { 0 };
	texDesc.Width = width;
	texDesc.Height = height;
	texDesc.Format = format;
	texDesc.MipLevels = 1;
	texDesc.ArraySize = 1;
	texDesc.SampleDesc.Count = 1;
	texDesc.SampleDesc.Quality = 0;
	texDesc.Usage = D3D11_USAGE_DEFAULT;
	texDesc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
	texDesc.CPUAccessFlags = 0;
	texDesc.MiscFlags = D3D11_RESOURCE_MISC_SHARED;

	ComPtr<ID3D11Texture2D> output;
	__abi_ThrowIfFailed(device->CreateTexture2D(&texDesc, nullptr, output.GetAddressOf()));
	return output;
}