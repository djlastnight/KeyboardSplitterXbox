namespace Interceptor
{
    /// <summary>
    /// A list of scan codes.
    /// </summary>
    /// <remarks>Scancodes change according to keyboard layout...so this may be inaccurate.</remarks>
    public enum InterceptionKeys : ushort
    {
        None = 0,
        Escape = 1,
        One = 2,
        Two = 3,
        Three = 4,
        Four = 5,
        Five = 6,
        Six = 7,
        Seven = 8,
        Eight = 9,
        Nine = 10,
        Zero = 11,
        DashUnderscore = 12,
        PlusEquals = 13,
        Backspace = 14,
        Tab = 15,
        Q = 16,
        W = 17,
        E = 18,
        R = 19,
        T = 20,
        Y = 21,
        U = 22,
        I = 23,
        O = 24,
        P = 25,
        OpenBracketBrace = 26,
        CloseBracketBrace = 27,
        Enter = 28,
        LeftControl = 29,
        A = 30,
        S = 31,
        D = 32,
        F = 33,
        G = 34,
        H = 35,
        J = 36,
        K = 37,
        L = 38,
        SemicolonColon = 39,
        SingleDoubleQuote = 40,
        Tilde = 41,
        LeftShift = 42,
        BackslashPipe = 43,
        Z = 44,
        X = 45,
        C = 46,
        V = 47,
        B = 48,
        N = 49,
        M = 50,
        CommaLeftArrow = 51,
        PeriodRightArrow = 52,
        ForwardSlashQuestionMark = 53,
        RightShift = 54,
        NumpadAsterisk = 55,
        LeftAlt = 56,
        Space = 57,
        CapsLock = 58,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        NumLock = 69,
        ScrollLock = 70,
        Numpad7 = 71,
        Numpad8 = 72,
        Numpad9 = 73,
        NumpadMinus = 74,
        Numpad4 = 75,
        Numpad5 = 76,
        Numpad6 = 77,
        NumpadPlus = 78,
        Numpad1 = 79,
        Numpad2 = 80,
        Numpad3 = 81,
        Numpad0 = 82,
        NumpadDelete = 83,
        Oem16 = 84,

        // 85 missing
        LeftBackslashPipe = 86,
        F11 = 87,
        F12 = 88,

        // 89 missing

        // 90 missing
        LeftWindows = 91,
        RightWindows = 92,
        Menu = 93,

        Up = 5001,
        Down = 5002,
        Left = 5003,
        Right = 5004,

        Home = 10071,
        PageUp = 10073,
        End = 10079,
        PageDown = 10081,
        Insert = 10082,
        Delete = 10083,

        MediaPreviousTrack = 10001,
        MediaPlayPause = 10002,
        MediaNextTrack = 10003,
        VolumeUp = 10004,
        VolumeDown = 10005,
        VolumeMute = 10006,

        RightControl = 10008,
        RightAlt = 10009,

        Oem0 = 10020, // escape in my laptop
        Oem2 = 10010, // brighness -
        Oem3 = 10011, // brightness +
        Oem4 = 10015, // monitor select
        Oem5 = 10012, // sleep
        Oem6 = 10013, // lock
        Oem7 = 10014, // web
        Oem13 = 10007, // wifi

        NumpadDivide = 10040, // fn + 0 in my laptop

        ShiftModifier = 10016,
        PrintScreen = 10017,
        Break = 10018,
        Pause = 10019,
        NumpadEnter = 10021,

        // Mouse Gestures
        MouseLeftButton = 20001,
        MouseRightButton = 20002,
        MouseMiddleButton = 20003,
        MouseExtraLeft = 20004,
        MouseExtraRight = 20005,
        MouseWheelUp = 20006,
        MouseWheelDown = 20007,
        MouseWheelLeft = 20008,
        MouseWheelRight = 20009
    }
}
