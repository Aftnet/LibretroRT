#pragma once

#include "../LibretroRT.CoreTools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_CoreTools;
using namespace Windows::Storage;

namespace NestopiaRT
{
	private ref class NestopiaCoreInternal sealed : public LibretroRT_CoreTools::CoreBase
	{
	protected private:
		NestopiaCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property NestopiaCoreInternal^ Instance { NestopiaCoreInternal^ get(); }
		virtual ~NestopiaCoreInternal();

		bool LoadGame(IStorageFile^ gameFile) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}


