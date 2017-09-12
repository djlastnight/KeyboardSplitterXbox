namespace SplitterCore
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using SplitterCore.Emulation;
    using SplitterCore.Input;

    public abstract class SplitterBase : DependencyObject, ISplitter
    {
        public static readonly DependencyProperty InputManagerProperty =
            DependencyProperty.Register(
            "InputManager",
            typeof(IInputManager),
            typeof(SplitterBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty EmulationManagerProperty =
            DependencyProperty.Register(
            "EmulationManager",
            typeof(IEmulationManager),
            typeof(SplitterBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ShouldBlockKeyboardsProperty =
            DependencyProperty.Register(
            "ShouldBlockKeyboards",
            typeof(bool),
            typeof(SplitterBase),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShouldBlockMiceProperty =
            DependencyProperty.Register(
            "ShouldBlockMice",
            typeof(bool),
            typeof(SplitterBase),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AssignedInputDevicesProperty =
            DependencyProperty.Register(
            "AssignedInputDevices",
            typeof(List<InputDevice>),
            typeof(SplitterBase),
            new PropertyMetadata(null));

        protected SplitterBase()
        {
            this.AssignedInputDevices = new List<InputDevice>();
        }

        protected SplitterBase(IInputManager inputManager, IEmulationManager emulationManager)
            : this()
        {
            if (inputManager == null)
            {
                throw new ArgumentNullException("inputManger");
            }

            if (emulationManager == null)
            {
                throw new ArgumentNullException("emulationManager");
            }

            this.InputManager = inputManager;
            this.EmulationManager = emulationManager;
        }

        public IInputManager InputManager
        {
            get { return (IInputManager)this.GetValue(InputManagerProperty); }
            set { this.SetValue(InputManagerProperty, value); }
        }

        public IEmulationManager EmulationManager
        {
            get { return (IEmulationManager)this.GetValue(EmulationManagerProperty); }
            set { this.SetValue(EmulationManagerProperty, value); }
        }

        public bool ShouldBlockKeyboards
        {
            get { return (bool)this.GetValue(ShouldBlockKeyboardsProperty); }
            set { this.SetValue(ShouldBlockKeyboardsProperty, value); }
        }

        public bool ShouldBlockMice
        {
            get { return (bool)this.GetValue(ShouldBlockMiceProperty); }
            set { this.SetValue(ShouldBlockMiceProperty, value); }
        }

        public List<InputDevice> AssignedInputDevices
        {
            get { return (List<InputDevice>)this.GetValue(AssignedInputDevicesProperty); }
            set { this.SetValue(AssignedInputDevicesProperty, value); }
        }

        public virtual void Destroy()
        {
            if (this.InputManager != null)
            {
                this.InputManager.Destroy();
            }

            if (this.EmulationManager != null)
            {
                this.EmulationManager.Destroy();
            }
        }
    }
}
