namespace SplitterCore.Emulation
{
    using SplitterCore.Input;

    public interface IKeySlot
    {
        InputKey Key { get; set; }

        KeySlotType KeySlotType { get; }

        uint TargetFunction { get; }

        object TargetValue { get; }

        bool IsRemoveable { get; }
    }
}
