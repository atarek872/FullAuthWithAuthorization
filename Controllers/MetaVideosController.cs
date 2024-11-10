using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DynamicRoleBasedAuthorization.Models;
using DynamicRoleBasedAuthorization.Models.MetaVideos;
using DynamicRoleBasedAuthorization.ViewModel;

namespace DynamicRoleBasedAuthorization.Controllers
{
 
    public class MetaVideosController : Controller
    {
        private ILogger<MetaVideosController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private ApplicationDbContext _context;

        public MetaVideosController(ILogger<MetaVideosController> logger, ApplicationDbContext metavideosContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = metavideosContext;
            _userManager = userManager;

        }
        // GET: CreateNew Video in my list
        public ActionResult CreateNewVideos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var Dollar = (_context.Bouns.Where(t => t.UserId == userId).Select(t => t.TotalBouns).FirstOrDefault() / 50) * 0.00015;

            var Model = new AddVideoViewModel
            {
                bonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault(),
                Myownvids = _context.MyVideos.Where(e => e.UserId == userId).ToList(),
                Video = _context.MetaVids.Where(e => e.CreatedBy == userId).ToList(),
                Dollar = Dollar
            };
            return View(Model);
        }


        // GET: List Videos 
        public async Task<ActionResult> ListVids()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var MywatchedVids = _context.WatchedVideos.Where(t => t.UserId == userId).Select(m => m.VideoId).ToList();
            var Myvids = _context.MetaVids.Where(t => t.CreatedBy == userId).Select(m => m.id).ToList();
            var Video = _context.MetaVids.Where(t => !MywatchedVids.Contains(t.id) && t.status != 0 && !Myvids.Contains(t.id)).OrderBy(t => t.watchedCount).Take(5).ToList();
            //My total watched videos in last  (10 videos in 2 hours) 
            var WatchedVideosInLasttime = _context.WatchedVideos.Where(t => t.WatchedAt > DateTime.Now.AddHours(-2) && t.UserId == userId).Count();
            if (WatchedVideosInLasttime < 10)
            {
                List<YouTubeVideoDetails> ListVideDt = new List<YouTubeVideoDetails>();

                foreach (var item in Video)
                {
                    var getYoutubeDetails = await GetVideoDetails((item.IframUri).Split('/')[3]);
                    if (getYoutubeDetails != null)
                    {
                        YouTubeVideoDetails obj = new YouTubeVideoDetails();
                        obj.Title = getYoutubeDetails.Title;
                        obj.Uri = getYoutubeDetails.Uri;
                        obj.Url = getYoutubeDetails.Url;
                        obj.Image = getYoutubeDetails.Image;
                        obj.Description = getYoutubeDetails.Description;
                        obj.ChannelTitle = getYoutubeDetails.ChannelTitle;
                        obj.MetaVIdsId = item.id;
                        ListVideDt.Add(obj);
                    }

                }
                var ViewModel = new ListVideosModelView()
                {
                    Details = ListVideDt,
                };
                return View(ListVideDt);
            }
            else
            {
                return View();

            }

        }
        // GET: watch one video and earn money 

        [HttpGet("MetaVideos/EarnPoints/{VideoId}/{id}")]
        public ActionResult EarnPoints(string VideoId, int id)
        {
            var test1 = VideoId;
            var test2 = id;
            Console.WriteLine(test1+ " "+test2);
            return View();
        }
        // GET: MetaVideos/LoadVids
        [HttpGet]
        public JsonResult LoadVids()
        {
            var Model = new LoadAllVidsView
            {
                Video = _context.MetaVids.OrderByDescending(m =>m.CreateAt).ToList()
            };
            return Json(Model);
        }

        // GET: MetaVideos/MyBonus
        [HttpGet]
        public ActionResult MyBonus()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

            var Model = new MyBonusDetailsModelView
            {
                bonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault(),
                Myownvids = _context.MyVideos.Where(e => e.UserId == userId).ToList(),
                Video = _context.MetaVids.Where(e => e.CreatedBy == userId).ToList()
            };
            return View(Model);
        }

        // GET: MetaVideos/Create
        public ActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

            var Dollar = (_context.Bouns.Where(t => t.UserId == userId).Select(t => t.TotalBouns).FirstOrDefault() / 50) * 0.00015;  
            var Model = new AddVideoViewModel
            {
                bonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault(),
                Dollar = Dollar
            };
            return View(Model);
        }

        // POST: MetaVideos/CreateNewVideos/{metaVids}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewVideos(AddVideoViewModel metaVids)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
                //var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName

                //// For ASP.NET Core <= 3.1
                //IdentityUser identityuser = await _userManager.GetUserAsync(User);
                //string userEmail = identityuser?.Email; // will give the user's Email

                //bool isAuthenticated = User.Identity.IsAuthenticated;
                var MyBonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault();
                if (MyBonus.TotalBouns >= 100)
                {
                    if (ModelState.IsValid && _context.MetaVids.Where(e => e.IframUri == metaVids.IframUri).Count() == 0)
                    {
                        // Create at MetaVids New Vid

                        var newVideo = new MetaVids()
                        {
                            IframUri = metaVids.IframUri,
                            CreateAt = DateTime.Now,
                            CreatedBy = userId,
                            watchedCount = 0,
                            status = 0
                        };
                        
                        MyBonus.TotalBouns = (MyBonus.TotalBouns - 100);
                        MyBonus.Updated_at = DateTime.Now;
                        _context.MetaVids.Add(newVideo);
                        _context.Bouns.Update(MyBonus);
                        await _context.SaveChangesAsync();
                        using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            ApiKey = "AIzaSyClq67w_MkSZ_07JzifvnFjymuRfSwFaVc",
                        }))
                        {
                            var searchDuration = youtubeService.Videos.List("contentDetails");
                            searchDuration.Id = (metaVids.IframUri).Split('/')[3];
                            var searchDurationResponse = await searchDuration.ExecuteAsync();
                            var youTubeVideoDuration = searchDurationResponse.Items.FirstOrDefault();
                            string Duration = youTubeVideoDuration.ContentDetails.Duration;

                            var myVideo = new MyVideos()
                            {
                                UserId = userId,
                                DurationTime = Duration,
                                EarnBounsFromIt = 0,
                                UploadedAt = DateTime.Now,
                                VideoId = newVideo.id,
                                ViewsOnIt = 0,

                            };
                            _context.Add(myVideo);
                            await _context.SaveChangesAsync();
                        }
                        TempData["Message"] = "Your Video Sent to our Quality team and it will publish with in 24 hours.";
                        return RedirectToAction("CreateNewVideos", "MetaVideos");


                    }
                    ModelState.AddModelError("alreadyExist", "The below link already exist.!");
                    var Model = new AddVideoViewModel
                    {
                        bonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault(),
                        IframUri = metaVids.IframUri,
                        Myownvids = _context.MyVideos.Where(e => e.UserId == userId).ToList(),
                        Video = _context.MetaVids.Where(e => e.CreatedBy == userId).ToList()
                    };
                    return View(Model);
                }
                else
                {
                    ModelState.AddModelError("NoBonus", "You need 100 unit of points to create new video.");
                    var Modelobj = new AddVideoViewModel
                    {
                        bonus = _context.Bouns.Where(t => t.UserId == userId).FirstOrDefault(),
                        IframUri = metaVids.IframUri,
                        Myownvids = _context.MyVideos.Where(e => e.UserId == userId).ToList(),
                        Video = _context.MetaVids.Where(e => e.CreatedBy == userId).ToList()
                    };
                    return View(Modelobj);
                }
               
                
                //return View(metaVids);

             
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }

   

        // Test videos Informations
        public async Task<YouTubeVideoDetails> GetVideoDetails(string VideoId = "nIY6VxBKgaU")
        {
            YouTubeVideoDetails videoDetails = null;
            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyClq67w_MkSZ_07JzifvnFjymuRfSwFaVc",
                    
                }))
            {
                var searchRequest = youtubeService.Videos.List("snippet");
                searchRequest.Id = VideoId;
                try
                {
                    var searchResponse = await searchRequest.ExecuteAsync();
                    var youTubeVideo = searchResponse.Items.FirstOrDefault();

                    if (youTubeVideo != null)
                    {
                        videoDetails = new YouTubeVideoDetails()
                        {
                            VideoId = youTubeVideo.Id,
                            Description = youTubeVideo.Snippet.Description,
                            Title = youTubeVideo.Snippet.Title,
                            ChannelTitle = youTubeVideo.Snippet.ChannelTitle,
                            Image = youTubeVideo.Snippet.Thumbnails.High.Url,
                            Uri = VideoId,
                            Url = "https://youtu.be/" + VideoId

                        };
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
                

                
            }
        return videoDetails;
        }

        // GET: MetaVideos/LoadVids
        [HttpPost]
        public async Task<JsonResult> WatchAction(int VideoId,string Uri)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

            //Update Metavids
          
            try
            {
                var meta = await _context.MetaVids.FindAsync(VideoId);
                meta.watchedCount += 1;
                _context.Update(meta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

            //update watched Vids Table
            try
            {

                WatchedVideos New = new WatchedVideos();
                New.UserId = userId;
                New.VideoId = VideoId;
                New.WatchedAt = DateTime.Now;
                _context.WatchedVideos.Add(New);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

            //update MyVids Table

            try
            {
                var meta =  _context.MyVideos.FirstOrDefault(s => s.VideoId == VideoId);
                meta.ViewsOnIt += 1;
                meta.EarnBounsFromIt += 5;
                _context.MyVideos.Update(meta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

            // Update Bouns Table


            try
            {
                var meta = _context.Bouns.FirstOrDefault(s => s.UserId == userId);
                meta.TotalBouns += 10;
                meta.TotalVideosViewed += 1;
                meta.Updated_at = DateTime.Now;
                _context.Bouns.Update(meta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            
            // Update watched Bouns for video's uploader 

            try
            {
                var userIdUploader = _context.MetaVids.Where(t => t.id == VideoId).Select(t => t.CreatedBy).FirstOrDefault();
                var meta = _context.Bouns.FirstOrDefault(s => s.UserId == userIdUploader);
                meta.TotalBouns += 5;
                meta.Updated_at = DateTime.Now;
                _context.Bouns.Update(meta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


            return Json(userId);
        }
    }
}
