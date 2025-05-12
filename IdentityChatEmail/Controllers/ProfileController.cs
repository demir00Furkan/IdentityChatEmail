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

            var model = new ProfileDetailViewModel
            {
                InboxMessages = inboxList,
                User = new UserUpdateViewModel
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Username = user.UserName,
                    Email = user.Email
                }
            };

            ViewBag.InboxCountFalse = _context.Messages.Count(x => !x.IsRead);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileDetail(ProfileDetailViewModel model)
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                // Inbox'ı tekrar yükle (POST sonrası sayfa geri dönecekse)
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

                ViewBag.InboxCountFalse = _context.Messages.Count(x => !x.IsRead);
                return View(model);
            }

            user.Name = model.User.Name;
            user.Surname = model.User.Surname;
            user.UserName = model.User.Username;
            user.Email = model.User.Email;

            if (!string.IsNullOrEmpty(model.User.Password))
            {
                var passwordHasher = new PasswordHasher<AppUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.User.Password);
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

    }
}
