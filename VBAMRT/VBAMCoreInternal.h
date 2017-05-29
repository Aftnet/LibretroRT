#pragma once

#include "../LibretroRTSupport/CoreBase.h"

using namespace Platform;
using namespace LibretroRTSupport;
using namespace Windows::Storage;

namespace VBAMRT
{
	private ref class VBAMCoreInternal sealed : public LibretroRTSupport::CoreBase
	{
	protected private:
		VBAMCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property VBAMCoreInternal^ Instance { VBAMCoreInternal^ get(); }
		virtual ~VBAMCoreInternal();

		bool LoadGame(IStorageFile^ gameFile) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}


