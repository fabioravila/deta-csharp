

namespace DetaCSharp.Types
{
    public class DetaAction
    {
        public DetaAction(ActionTypes action, object value = null)
        {
            Action = action;
            Value = value;
        }

        public ActionTypes Action { get; }
        public object Value { get; }
    }
}
 