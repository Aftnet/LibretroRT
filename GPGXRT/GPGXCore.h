#pragma once

#include "../LibretroRTSupport/CoreBase.h"

using namespace Platform;
using namespace LibretroRTSupport;

namespace GPGXRT
{
	private ref class GPGXCore sealed : public LibretroRTSupport::CoreBase
	{
	protected private:
		GPGXCore();

	public:
		static GPGXCore^ GetInstance();
		virtual ~GPGXCore();

		void LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;
	};
}


