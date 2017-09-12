namespace SplitterCore.Emulation
{
    using System;

    public interface IVirtualGamepad
    {
        event EventHandler Disconnected;

        uint UserIndex { get; }

        string FriendlyName { get; }

        bool Exsists { get; }

        bool IsOwned { get; }

        uint LedNumber { get; set; }

        bool PlugIn();

        bool Unplug(bool isForced);

        bool SetButtonState(uint button, bool value);

        bool SetTriggerState(uint trigger, byte value);

        bool SetDpadState(int value);

        bool SetAxisState(uint axis, short value);

        bool SetCustomFunctionState(uint function, bool value);

        bool GetButtonState(uint button);

        byte GetTriggerState(uint trigger);

        int GetDpadState();

        short GetAxisState(uint axis);

        bool GetCustomFunctionState(uint function);
    }
}
