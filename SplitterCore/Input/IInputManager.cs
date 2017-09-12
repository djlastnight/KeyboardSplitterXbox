namespace SplitterCore.Input
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This interface manages the Windows input devices.
    /// It must support real time listing of all attached keyboards and mice.
    /// It must fire events, when any keyboard/mouse has some activity
    /// and also when any keyboard/mouse is connected/disconnected from the system.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Fired, when keyboard or mouse has some activity (key down, up, etc..)
        /// </summary>
        event EventHandler<InputEventArgs> InputActivity;

        /// <summary>
        /// Fired, when keyboard or mouse has been connected or disconnected
        /// from the system.
        /// </summary>
        event EventHandler<InputDeviceChangedEventArgs> InputDeviceChanged;

        /// <summary>
        /// Fired when user press Ctrl+Alt+Delete
        /// </summary>
        event EventHandler EmergencyStop;

        /// <summary>
        /// Fired when user press LeftCtrl 5 times in a row.
        /// </summary>
        event EventHandler EmergencyLeft;

        /// <summary>
        /// Firedm when user press RightCtrl 5 times in a row.
        /// </summary>
        event EventHandler EmergencyRight;

        /// <summary>
        /// Gets an up to date keyboards list.
        /// </summary>
        List<Keyboard> Keyboards { get; }

        /// <summary>
        /// Gets and up to date mice list.
        /// </summary>
        List<Mouse> Mice { get; }

        /// <summary>
        /// Gets the history of input events stored as text.
        /// </summary>
        string InputMonitorHistory { get; set; }

        /// <summary>
        /// Clears the input monitor history
        /// </summary>
        void ClearInputMonitorHistory();

        /// <summary>
        /// Determines if the desired key on desired device is currently held.
        /// </summary>
        bool IsKeyDown(InputDevice device, InputKey key);

        /// <summary>
        /// Destroys the IInputManager
        /// </summary>
        void Destroy();
    }
}