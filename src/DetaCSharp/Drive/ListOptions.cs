namespace DetaCSharp.Drive
{
    public class ListOptions
    {
        public string Prefix { get; set; }
        public int? Limit { get; set; }
        public string Last { get; set; }
        public static ListOptions Empty => new ListOptions();
    }
}
