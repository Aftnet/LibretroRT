using GalaSoft.MvvmLight;
using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class SettingsVM : ViewModelBase
    {
        private readonly IEmulationService EmulationService;

        public IReadOnlyList<FileImporterVM> FileDependencyImporters => EmulationService.FileDependencyImporters;

        public SettingsVM(IEmulationService emulationService)
        {
            EmulationService = emulationService;
            EmulationService.CoresInitialized += OnCoresInitialized;
        }

        private void OnCoresInitialized(IEmulationService sender)
        {
            RaisePropertyChanged(nameof(FileDependencyImporters));
        }
    }
}
