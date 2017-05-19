#pragma once

#include "../LibretroRTSupport/CoreBase.h"

using namespace Platform;
using namespace LibretroRTSupport;

namespace GPGXRT
{
	private ref class GPGXCoreInternal sealed : public LibretroRTSupport::CoreBase
	{
	protected private:
		GPGXCoreInternal();

	public:
		static property GPGXCoreInternal^ Instance { GPGXCoreInternal^ get(); }
		virtual ~GPGXCoreInternal();

		void LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;
	};
}


