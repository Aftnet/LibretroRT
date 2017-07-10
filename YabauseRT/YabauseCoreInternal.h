#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace YabauseRT
{
	private ref class YabauseCoreInternal sealed : public CoreBase
	{
	protected private:
		virtual void OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options) override;

		YabauseCoreInternal();

	public:
		static property YabauseCoreInternal^ Instance { YabauseCoreInternal^ get(); }
		virtual ~YabauseCoreInternal();
	};
}