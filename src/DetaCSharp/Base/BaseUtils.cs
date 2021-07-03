using DetaCSharp.Types;
using System.Collections.Generic;

namespace DetaCSharp.Base
{
    public static class BaseUtils
    {
        public static DetaAction Trim() => new DetaAction(ActionTypes.Trim);
        public static DetaAction Increment(float value = 1f) => new DetaAction(ActionTypes.Increment, value);
        public static DetaAction Append(object value) => new DetaAction(ActionTypes.Append, new[] { value });
        public static DetaAction Append(IEnumerable<object> value) => new DetaAction(ActionTypes.Append, value);
        public static DetaAction Append(params object[] value) => new DetaAction(ActionTypes.Append, value);
        public static DetaAction Prepend(object value) => new DetaAction(ActionTypes.Prepend, new[] { value });
        public static DetaAction Prepend(IEnumerable<object> value) => new DetaAction(ActionTypes.Prepend, value);
        public static DetaAction Prepend(params object[] value) => new DetaAction(ActionTypes.Prepend, value);
    }
}
