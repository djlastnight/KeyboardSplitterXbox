namespace SplitterCore.Emulation
{
    using System;
    using SplitterCore.Input;
    using SplitterCore.Preset;

    public interface IEmulationSlot
    {
        event EventHandler ResetRequested;

        uint SlotNumber { get; }

        IVirtualGamepad Gamepad { get; }

        Keyboard Keyboard { get; }

        Mouse Mouse { get; }

        IPreset Preset { get; set; }

        SlotInvalidationReason InvalidateReason { get; set; }

        void Lock();

        void Unlock();
    }
}
