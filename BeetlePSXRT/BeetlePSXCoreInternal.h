#pragma once

#include "../LibretroRT_Shared/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace BeetlePSXRT
{
	private ref class BeetlePSXCoreInternal sealed : public CoreBase
	{
	protected private:
		virtual void OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options) override;

		BeetlePSXCoreInternal();

	public:
		static property BeetlePSXCoreInternal^ Instance { BeetlePSXCoreInternal^ get(); }
		virtual ~BeetlePSXCoreInternal();
	};
}