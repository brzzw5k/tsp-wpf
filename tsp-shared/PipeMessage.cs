using System;

namespace tsp_shared
{
    [Serializable]
    public class PipeMessage
    {
        public Cycle Cycle { get; set; }
        public PipeMessageType PipeMessageType { get; set; }

    }

    [Serializable]
    public enum PipeMessageType
    {
        START,
        FINISH,
        STOP
    }
}
