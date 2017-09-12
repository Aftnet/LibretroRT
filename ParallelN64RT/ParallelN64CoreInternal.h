#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace ParallelN64RT
{
	private ref class ParallelN64CoreInternal sealed : public CoreBase
	{
	protected private:
		virtual void OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options) override;

		ParallelN64CoreInternal();

	public:
		static property ParallelN64CoreInternal^ Instance { ParallelN64CoreInternal^ get(); }
		virtual ~ParallelN64CoreInternal();
	};
}