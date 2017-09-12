namespace SplitterCore
{
    using System.Collections.Generic;
    using SplitterCore.Emulation;
    using SplitterCore.Input;
    using SplitterCore.Preset;

    public interface ISplitter
    {
        IInputManager InputManager { get; set; }

        IEmulationManager EmulationManager { get; set; }

        bool ShouldBlockKeyboards { get; set; }

        bool ShouldBlockMice { get; set; }

        List<InputDevice> AssignedInputDevices { get; set; }

        void Destroy();
    }
}
