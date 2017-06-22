#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace NestopiaRT
{
	private ref class NestopiaCoreInternal sealed : public CoreBase
	{
	protected private:
		NestopiaCoreInternal();

	internal:
		virtual IVectorView<FileDependency^>^ GenerateFileDependencies() override;
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		static property NestopiaCoreInternal^ Instance { NestopiaCoreInternal^ get(); }
		virtual ~NestopiaCoreInternal();
	};
}