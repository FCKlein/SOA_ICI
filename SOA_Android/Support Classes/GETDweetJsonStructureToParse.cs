using System.Collections.Generic;

namespace SOA_Android.Support_Classes
{
    public class Content
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class With
    {
        public string Thing { get; set; }
        public string Created { get; set; }
        public Content Content { get; set; }
    }

    public class LatestDweet
    {
        public string This { get; set; }
        public string By { get; set; }
        public string The { get; set; }
        public List<With> With { get; set; }
    }
}