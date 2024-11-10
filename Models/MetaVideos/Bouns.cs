using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRoleBasedAuthorization.Models.MetaVideos
{
    public class Bouns
    {
        [Key]
        public int id { get; set; }
        public string UserId { get; set; }
        public int TotalVideosViewed { get; set; }
        public int TotalBouns { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
