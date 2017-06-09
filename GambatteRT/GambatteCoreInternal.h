#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace GambatteRT
{
	private ref class GambatteCoreInternal sealed : public CoreBase
	{
	protected private:
		GambatteCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		property unsigned int SerializationSize { unsigned int get() override; }

		static property GambatteCoreInternal^ Instance { GambatteCoreInternal^ get(); }
		virtual ~GambatteCoreInternal();

		bool LoadGameInternal(IStorageFile^ gameFile) override;
		void UnloadGameInternal() override;
		void RunFrame() override;
		void Reset() override;

		bool Serialize(WriteOnlyArray<uint8>^ stateData) override;
		bool Unserialize(const Array<uint8>^ stateData) override;
	};
}