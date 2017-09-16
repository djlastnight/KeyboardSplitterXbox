namespace SplitterCore.Emulation
{
    using System;
using System.Collections.ObjectModel;

    public interface IEmulationManager
    {
        event EventHandler EmulationStarted;

        event EventHandler EmulationStopped;

        bool IsEmulationStarted { get; set; }

        ObservableCollection<IEmulationSlot> Slots { get; set; }

        void Start(bool forced = false);

        void Stop();

        void ChangeSlotsCountBy(int amount);

        void Destroy();
    }
}