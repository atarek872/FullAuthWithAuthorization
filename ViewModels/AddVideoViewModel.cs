using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.Models.MetaVideos;

namespace DynamicRoleBasedAuthorization.ViewModel
{
    public class AddVideoViewModel
    {
        [Required]
        public string IframUri { get; set; }
        public Bouns? bonus { get; set; }
        public List<MyVideos> Myownvids { get; set; }
        public List<MetaVids> Video { get; set; }
        public Double Dollar { get; set; }
    }
}
