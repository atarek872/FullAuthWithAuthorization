using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRoleBasedAuthorization.ViewModel
{
    public class YouTubeVideoDetails
    {
        public string VideoId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string ChannelTitle { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Uri { get; set; }
        public DateTime? PublicationDate { get; set; }
        public int MetaVIdsId { get; set; }
    }
}
