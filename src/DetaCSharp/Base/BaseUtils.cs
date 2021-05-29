using DetaCSharp.Types;

namespace DetaCSharp.Base
{
    public class BaseUtils
    {
        public static DetaAction Trim() => new DetaAction(ActionTypes.Trim);
        public static DetaAction Increment(float value = 1f) => new DetaAction(ActionTypes.Increment, value);
        public static DetaAction Append(object value) => new DetaAction(ActionTypes.Append, new[] { value });
        public static DetaAction Append(object[] value) => new DetaAction(ActionTypes.Append, value);
        public static DetaAction Prepend(object value) => new DetaAction(ActionTypes.Prepend, new[] { value });
        public static DetaAction Prepend(object[] value) => new DetaAction(ActionTypes.Prepend, value);
    }
}
