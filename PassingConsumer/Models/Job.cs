namespace PassingConsumer.Models
{
    public class Job
    {
        public string name { get; set; }
        public int process { get; set; }
        public DateTime? earliestStart { get; set; }
        public DateTime latestStart { get; set; }
        public string arguments { get; set; }
    }
}
