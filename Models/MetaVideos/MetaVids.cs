using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRoleBasedAuthorization.Models.MetaVideos
{
    public class MetaVids
    {
        [Key]
        public int id { get; set; }
        public string IframUri { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public int? watchedCount { get; set; }
        public int? status { get; set; }

    }
}
