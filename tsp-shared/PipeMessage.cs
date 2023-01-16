using System;

namespace tsp_shared
{
    [Serializable]
    public class PipeMessage
    {
        public Cycle Cycle { get; set; }
        public int Value { get; set; }
        public int ID { get; set; }
        public PipeMessageType PipeMessageType { get; set; }

    }

    [Serializable]
    public enum PipeMessageType
    {
        START,
        PROGRESS,
        NEW_BEST,
        FINISH,
        STOP
    }
}
