using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRoleBasedAuthorization.Models.MetaVideos
{
    public class WatchedVideos
    {
        [Key]
        public int id { get; set; }
        public string UserId { get; set; }
        public int VideoId { get; set; }
        public DateTime WatchedAt { get; set; }
    }
}
