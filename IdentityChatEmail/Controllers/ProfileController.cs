using IdentityChatEmail.Context;
using IdentityChatEmail.Entities;
using IdentityChatEmail.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityChatEmail.Controllers
{
    public class ProfileController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _UserManager;

        public ProfileController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _UserManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfileDetail()
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);

            var messages = _context.Messages
                .Where(x => x.ReciverEmail == user.Email)
                .OrderByDescending(x => x.SendDate)
                .Take(3)
                .ToList();

            var inboxList = messages.Select(m =>
            {
                var sender = _UserManager.Users.FirstOrDefault(u => u.Email.ToLower() == m.SenderEmail.ToLower());
                return new InboxMessageViewModel
                {
                    Message = m,
                    SenderName = sender?.Name ?? "Gizli Kullanıcı",
                    SenderSurname = sender?.Surname ?? ""
                };
            }).ToList();

            var viewModel = new ProfileDetailViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Username = user.UserName,
                Email = user.Email,
                InboxMessages = inboxList,
                ProfileImageUrl = user.ProfileImageUrl
            };
            var deger = _context.Messages.Where(x => x.ReciverEmail == user.Email).OrderByDescending(x => x.SendDate).ToList();
            ViewBag.InboxCountFalse = deger.Where(x => x.IsRead == false).Count();
            ViewBag.ProfilePhoto= user.ProfileImageUrl;
            ViewBag.name = user.Name;
            ViewBag.surname = user.Surname;
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ProfileDetail(ProfileDetailViewModel model)
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                
                var messages = _context.Messages
                    .Where(x => x.ReciverEmail == user.Email)
                    .OrderByDescending(x => x.SendDate)
                    .Take(3)
                    .ToList();

                model.InboxMessages = messages.Select(m =>
                {
                    var sender = _UserManager.Users.FirstOrDefault(u => u.Email.ToLower() == m.SenderEmail.ToLower());
                    return new InboxMessageViewModel
                    {
                        Message = m,
                        SenderName = sender?.Name ?? "Gizli Kullanıcı",
                        SenderSurname = sender?.Surname ?? ""
                    };
                }).ToList();

                
                return View(model);
            }

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.UserName = model.Username;
            user.Email = model.Email;
           

            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHasher = new PasswordHasher<AppUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            }

            var result = await _UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profil güncellendi.";
                return RedirectToAction("ProfileDetail");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult ProfileIsReadToTrue(int id)
        {
            var values = _context.Messages.Find(id);
            values.IsRead = true;
            _context.SaveChanges();
            return RedirectToAction("ProfileDetail", "Profile");
        }
        public IActionResult ProfileIsReadToFalse(int id)
        {
            var values = _context.Messages.Find(id);
            values.IsRead = false;
            _context.SaveChanges();
            return RedirectToAction("ProfileDetail", "Profile");
        }
        public ActionResult ProfileDeleteMail(int id)
        {
            var values = _context.Messages.Find(id);
            _context.Messages.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("ProfileDetail", "Profile");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo(AppUser model)
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.ProfileImageUrl = model.ProfileImageUrl;

            if (!string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                user.PasswordHash = _UserManager.PasswordHasher.HashPassword(user, model.PasswordHash);
            }

            var result = await _UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ProfileDetail");
            }

          
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("ProfileDetail"); // Modeli tekrar doldurman gerekebilir
        }

    }
}
