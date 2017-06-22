#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace GPGXRT
{
	private ref class GPGXCoreInternal sealed : public CoreBase
	{
	protected private:
		GPGXCoreInternal();

	internal:
		IVectorView<FileDependency^>^ GenerateFileDependencies() override;
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		static property GPGXCoreInternal^ Instance { GPGXCoreInternal^ get(); }
		virtual ~GPGXCoreInternal();
	};
}