#pragma once

#include "../LibretroRT.CoreTools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_CoreTools;
using namespace Windows::Storage;

namespace Snes9XRT
{
	private ref class Snes9XCoreInternal sealed : public LibretroRT_CoreTools::CoreBase
	{
	protected private:
		Snes9XCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property Snes9XCoreInternal^ Instance { Snes9XCoreInternal^ get(); }
		virtual ~Snes9XCoreInternal();

		bool LoadGame(IStorageFile^ gameFile) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}


