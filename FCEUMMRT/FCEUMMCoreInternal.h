﻿#pragma once

#include "../LibretroRTSupport/CoreBase.h"

using namespace Platform;
using namespace LibretroRTSupport;
using namespace Windows::Storage;

namespace FCEUMMRT
{
	private ref class FCEUMMCoreInternal sealed : public LibretroRTSupport::CoreBase
	{
	protected private:
		FCEUMMCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property FCEUMMCoreInternal^ Instance { FCEUMMCoreInternal^ get(); }
		virtual ~FCEUMMCoreInternal();

		bool LoadGame(IStorageFile^ gameFile) override;
		void UnloadGame() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}