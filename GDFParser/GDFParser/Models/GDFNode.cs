namespace GDFParser.Models
{
    public class GDFNode
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Label { get; set; }
        public string Category { get; set; }
        public float PostActivity { get; set; }
        public long FanCount { get; set; }
        public long TalkingAboutCount { get; set; }
        public bool UsersCanPost { get; set; }
        public string Link { get; set; }
    }
}
