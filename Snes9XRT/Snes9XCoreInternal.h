#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace Snes9XRT
{
	private ref class Snes9XCoreInternal sealed : public CoreBase
	{
	protected private:
		Snes9XCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property Snes9XCoreInternal^ Instance { Snes9XCoreInternal^ get(); }
		virtual ~Snes9XCoreInternal();

		bool LoadGameInternal(IStorageFile^ gameFile) override;
		void UnloadGameInternal() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}


