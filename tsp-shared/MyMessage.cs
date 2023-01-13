namespace tsp_shared
{
    [Serializable]
    public class MyMessage
    {
        public Cycle Cycle { get; set; }
        public MessageType Type { get; set; }
        
        public double Progress { get; set; }

        public int Concurrency { get; set; }
    }

    [Serializable]
    public enum MessageType
    {
        START,
        PROGRESS,
        BEST_SOLUTION,
        CANCEL
    }
}