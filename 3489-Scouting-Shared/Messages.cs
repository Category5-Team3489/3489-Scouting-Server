using System;
using System.Collections.Generic;
using System.Text;

namespace ScoutingShared3489
{
    #region Client Bound
    public class PingCBMessage
    {
        public string Text { get; set; }
    }
    public class ProvideValueCBMessage
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    #endregion Client Bound

    #region Server Bound
    public class PingSBMessage
    {
        public string Text { get; set; }
    }
    public class RequestValueSBMessage
    {
        public string Name { get; set; }
    }
    public class UpdateValueSBMessage
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    #endregion Server Bound
}
