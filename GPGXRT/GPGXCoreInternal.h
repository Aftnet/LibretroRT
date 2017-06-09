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
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property GPGXCoreInternal^ Instance { GPGXCoreInternal^ get(); }
		virtual ~GPGXCoreInternal();

		bool LoadGame(IStorageFile^ gameFile) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}


