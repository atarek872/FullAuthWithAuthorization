using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRoleBasedAuthorization.Models.MetaVideos
{
    public class MyVideos
    {
        [Key]
        public int id { get; set; }
        public string UserId { get; set; }
        public int VideoId { get; set; }
        public int ViewsOnIt { get; set; }
        public int EarnBounsFromIt { get; set; }
        public string DurationTime { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
