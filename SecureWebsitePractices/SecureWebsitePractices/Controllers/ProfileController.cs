﻿using Microsoft.ApplicationInsights.Extensibility.Implementation;
using SecureWebsitePractices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using UserContext = SecureWebsitePractices.Models.UserContext;

namespace SecureWebsitePractices.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index(string userName, ProfileModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new ApplicationException("User not authenticated");
            }

            //if (ModelState.IsValid)
            //{
            //    return View("Profile", model);
            //}

            userName = User.Identity.Name;
            ProfileModel profile = new ProfileModel();

            using (UserContext context = new UserContext())
            {
                profile = context.Profiles.SingleOrDefault(x => x.UserName == userName);
            }

            if (profile == null)
            {
                throw new ApplicationException("Profile does not exist");
            }

            //Json(new
            //{
            //    UserName = profile.UserName,
            //    Address = profile.Address,
            //    BirthDate = profile.BirthDate.ToString("dd MM yyyy"),
            //    NINumber = profile.NINumber
            //},
            //   JsonRequestBehavior.AllowGet);

            return View("Profile");
        }


        public ActionResult GetProfile(string userName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new ApplicationException("User not authenticated");
            }

            //if (ModelState.IsValid)
            //{
            //    return View("Profile", model);
            //}

            ProfileModel profile = new ProfileModel();

            using (UserContext context = new UserContext())
            {
                profile = context.Profiles.SingleOrDefault(x => x.UserName == userName);
            }

            if (profile == null)
            {
                throw new ApplicationException("Profile does not exist");
            }

            return Json(new
            {
                UserName = profile.UserName,
                Name = profile.Name,
                Email = profile.Email,
                Address = profile.Address,
                BirthDate = profile.BirthDate.ToString("dd MM yyyy"),
                NINumber = profile.NINumber
            },
               JsonRequestBehavior.AllowGet);
        }
    }
}
