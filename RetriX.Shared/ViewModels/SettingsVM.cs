using GalaSoft.MvvmLight;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RetriX.Shared.ViewModels
{
    public class SettingsVM : ViewModelBase
    {
        private readonly IEmulationService EmulationService;

        public IEnumerable<FileImporterVM> FileDependencyImporters;

        public SettingsVM(IEmulationService emulationService)
        {
            EmulationService = emulationService;
        }
    }
}
