#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace BeetlePSXRT
{
	private ref class BeetlePSXCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetlePSXCoreInternal();

	internal:
		virtual IVectorView<FileDependency^>^ GenerateFileDependencies() override;
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		static property BeetlePSXCoreInternal^ Instance { BeetlePSXCoreInternal^ get(); }
		virtual ~BeetlePSXCoreInternal();
	};
}