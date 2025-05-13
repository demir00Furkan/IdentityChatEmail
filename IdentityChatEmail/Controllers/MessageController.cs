using IdentityChatEmail.Context;
using IdentityChatEmail.Entities;
using IdentityChatEmail.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace IdentityChatEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _Context;
        private readonly UserManager<AppUser> _UserManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _Context = context;
            _UserManager = userManager;
        }
        [Authorize]
        public async Task<ActionResult> Inbox()
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);

            var messages = _Context.Messages
                .Where(x => x.ReciverEmail == user.Email)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            var model = messages.Select(m =>
            {
                var sender = _UserManager.Users.FirstOrDefault(u => u.Email.ToLower() == m.SenderEmail.ToLower());
                return new InboxMessageViewModel
                {
                    Message = m,
                    SenderName = sender?.Name ?? "Gizli Kullanıcı",
                    SenderSurname = sender?.Surname ?? ""
                };
            }).ToList();
            var deger = _Context.Messages.Where(x=>x.ReciverEmail == user.Email).OrderByDescending(x=>x.SendDate).ToList();
            ViewBag.InboxCountFalse = deger.Where(x=>x.IsRead == false).Count();
            ViewBag.ProfilePhoto = user.ProfileImageUrl;
            ViewBag.name = user.Name;
            ViewBag.surname = user.Surname;
            return View(model);
        }


        public async Task<IActionResult> SendBox()
        {
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            string emailValue = user.Email;
            var sendMessageList = _Context.Messages.Where(x => x.SenderEmail == emailValue).ToList();
            ViewBag.ProfilePhoto = user.ProfileImageUrl;
            ViewBag.name = user.Name;
            ViewBag.surname = user.Surname;
            var deger = _Context.Messages.Where(x => x.ReciverEmail == user.Email).OrderByDescending(x => x.SendDate).ToList();
            ViewBag.InboxCountFalse = deger.Where(x => x.IsRead == false).Count();
            return View(sendMessageList);

        }
        [HttpGet]
        public async Task<IActionResult> CreateMessageAsync()
        {

            var values = await _UserManager.FindByNameAsync(User.Identity.Name);
            var deger = _Context.Messages.Where(x => x.ReciverEmail == values.Email).OrderByDescending(x => x.SendDate).ToList();
            ViewBag.InboxCountFalse = deger.Where(x => x.IsRead == false).Count();
            ViewBag.ProfileImage = values?.ProfileImageUrl;
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.ProfilePhoto = values.ProfileImageUrl;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message, string action)
        {
            var values = await _UserManager.FindByNameAsync(User.Identity.Name);
            ViewBag.ProfileImage = values?.ProfileImageUrl;
            if (action == "cancel")
            {
                return RedirectToAction("ProfileDetail", "Profile");
            }

            message.SenderEmail = values.Email;
            message.IsRead = false;
            message.SendDate = DateTime.Now;
            _Context.Messages.Add(message);
            _Context.SaveChanges();
            return RedirectToAction("ProfileDetail", "Profile");
        }
        public async Task<IActionResult> MessageDetail(int id)
        {
            var message = _Context.Messages.FirstOrDefault(x => x.MessageId == id);
            if (message == null)
                return NotFound();

            var sender = _UserManager.Users.FirstOrDefault(x => x.Email.ToLower() == message.SenderEmail.ToLower());

            var model = new MessageDetailViewModel
            {
                Message = message,
                SenderName = sender?.Name ?? "Gizli",
                SenderSurname = sender?.Surname ?? ""
            };
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            ViewBag.ProfilePhoto = user.ProfileImageUrl;
            ViewBag.name = user.Name;
            ViewBag.surname = user.Surname;
            var deger = _Context.Messages.Where(x => x.ReciverEmail == user.Email).OrderByDescending(x => x.SendDate).ToList();
            ViewBag.InboxCountFalse = deger.Where(x => x.IsRead == false).Count();
            return View(model);
        }

        public IActionResult InboxIsReadToTrue(int id)
        {
            var values = _Context.Messages.Find(id);
            values.IsRead = true;
            _Context.SaveChanges();
            return RedirectToAction("Inbox", "Message");
        }
        public IActionResult InboxIsReadToFalse(int id)
        {
            var values = _Context.Messages.Find(id);
            values.IsRead = false;
            _Context.SaveChanges();
            return RedirectToAction("Inbox", "Message");
        }
        public IActionResult DeleteMessage(int id)
        {
            var values = _Context.Messages.Find(id);
            _Context.Messages.Remove(values);
            _Context.SaveChanges();
            return RedirectToAction("Inbox");
        }


    }
}

