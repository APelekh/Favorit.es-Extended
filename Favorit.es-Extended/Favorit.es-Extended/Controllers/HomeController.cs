using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Favorit.es_Extended.Models;
using Microsoft.AspNet.Identity;

namespace Favorit.es.Controllers
{
    public class HomeController : Controller
    {
        //data context, data access layer, connects to database
        private FavoritesEntities db = new FavoritesEntities();

        /// <summary>
        /// Gets the id of the currently logged in user
        /// </summary>
        public string UserID
        {
            get { return User.Identity.GetUserId().ToString(); }
        }

        
        private List<Favorite> _userFavorites;
        public List<Favorite> UserFavorites
        {
            get
            {
                if (_userFavorites == null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        //logged in, get from DB
                        _userFavorites = db.Favorites.Where(x => x.UserId == this.UserID).ToList();
                    }
                    else
                    {
                        //not logged in, return empty list
                        _userFavorites = new List<Favorite>();
                    }
                }
                return _userFavorites;
            }
        }

        private Flickr _flickr;
        public Flickr Flickr
        {
            get 
            {
                if (_flickr == null)
                {
                    _flickr = new Flickr("381fe72e6e5d44f22c6192a211e5fbf0", "95203e8b65291995");
                    Flickr.CacheDisabled = true; //for server deployment
                }
                return _flickr; 
            }
        }

        /// <summary>
        /// this function searches flickr for photos
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        private PhotoCollection SearchFlickr(string searchText)
        {
            PhotoSearchOptions options = new PhotoSearchOptions();
            options.Tags = searchText;
            options.PerPage = 25; // 100 is the default anyway
            options.Extras = PhotoSearchExtras.Tags; //include tag info
            return this.Flickr.PhotosSearch(options);
        }

        [HttpGet]
        public ActionResult Index(string search)
        {
            //if the search string is empty, just use kittens
            if (string.IsNullOrEmpty(search)) { search = "Kittens"; }
            SearchViewModel viewModel = new SearchViewModel(search, this.UserFavorites, SearchFlickr(search));
            return View(viewModel);
        }

        [Authorize()] //require user login
        public ActionResult Favorite(Favorite favorite)
        {
            favorite.UserId = this.UserID; //fill in userID
            if (!this.UserFavorites.Any(x => x.PhotoId == favorite.PhotoId))
            {
                //only add if it is not in the list already
                db.Favorites.Add(favorite); // add to database
                db.SaveChanges(); // save to database
            }
            return Content("OK");
        }

        [Authorize()] //require user login
        public ActionResult Unfavorite(Favorite favorite)
        {
            db.Favorites.Remove(db.Favorites.Find(favorite.FavoriteId)); //remove from database
            db.SaveChanges(); // save database
            return Content("OK");
        }

        [Authorize()] // require user login
        public ActionResult MyFavorites()
        {
            return View(this.UserFavorites);
        }

        [Authorize()]
        public ActionResult ToggleFavorite(Favorite favorite)
        {
            favorite.UserId = this.UserID; //fill in userID
            if (this.UserFavorites.Any(x => x.PhotoId == favorite.PhotoId))
            {
                Favorite toRemove = this.UserFavorites.First(x => x.PhotoId == favorite.PhotoId);
                //already in the list, unfavorite the item
                db.Favorites.Remove(toRemove); //remove from database
                db.SaveChanges(); // save database
                return Content("unfavorited");
            }
            else
            {
                //not in the list, favorite the item
                db.Favorites.Add(favorite); // add to database
                db.SaveChanges(); // save to database
                return Content("favorited");
            }
        }



    }


}