using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.Models.MetaVideos;

namespace DynamicRoleBasedAuthorization.ViewModel
{
    public class MyBonusDetailsModelView
    {
        public Bouns bonus { get; set; }
        public string IframUri { get; set; }

        public List<MyVideos> Myownvids { get; set; }
        public List<MetaVids> Video { get; set; }
    }
}
