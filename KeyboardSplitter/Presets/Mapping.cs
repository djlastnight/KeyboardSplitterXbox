namespace KeyboardSplitter.Presets
{
    using System.Collections.Generic;
    using SplitterCore.Input;

    public class Mapping
    {
        public Mapping(object function, List<InputKey> keys, object targetValue = null)
        {
            this.Function = function;
            this.Keys = keys;
            this.TargetValue = targetValue;
        }

        public object Function { get; private set; }

        public List<InputKey> Keys { get; private set; }

        public object TargetValue { get; private set; }
    }
}
