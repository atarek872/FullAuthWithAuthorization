using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.Models.MetaVideos;

namespace DynamicRoleBasedAuthorization.ViewModel
{
    public class ListVideosModelView
    {
        //public List<MetaVids> Video { get; set; }
        public List<YouTubeVideoDetails> Details { get; set; }
    }
}
