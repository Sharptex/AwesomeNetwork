using AutoMapper;
using AwesomeNetwork.Models.Users;
using AwesomeNetwork.Repositories;
using AwesomeNetwork.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeNetwork.Extentions;
using AwesomeNetwork.Models;
using AwesomeNetwork.ViewModels;

namespace AwesomeNetwork.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var repo = _unitOfWork.GetRepository<Message>();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) 
                {
                    ModelState.AddModelError("Email", "Аккаунт с данной почтой не найден");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("UserProfile", "AccountManager");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }

            return View(model);
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Route("User")]
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var userVM = new UserViewModel(user);
            userVM.Friends = await GetAllFriends();

            return View("User", userVM);
        }

        [Authorize]
        [Route("UserEditor")]
        [HttpGet]
        public async Task<IActionResult> UserEditor()
        {
            var user = await _userManager.GetUserAsync(User);

            var userVM = _mapper.Map<UserEditViewModel>(user);

            return View("Edit", userVM);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserProfile", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserListAsync(string search)
        {
            var model = await CreateSearch(search);
            return View("UserList", model);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var repo = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            var user = await _userManager.GetUserAsync(User);
            var friend = await _userManager.FindByIdAsync(id);
            await repo.AddFriendAsync(user, friend);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction("UserProfile", "AccountManager");
        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var friend = await _userManager.FindByIdAsync(id);
            var repo = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            await repo.DeleteFriendAsync(user, friend);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction("UserProfile", "AccountManager");
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            List<User> list;

            if (string.IsNullOrWhiteSpace(search)) 
            {
                list = _userManager.Users.ToList();
            }
            else
            {
                list = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            }

            var data = new List<UserWithFriendExt>();
            var withfriend = new List<User>();
            if (result != null)
            {
                withfriend = await GetAllFriends();
                withfriend.Add(result);
            }
                
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        private async Task<List<User>> GetAllFriends()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return await repository.GetFriendsByUserAsync(result);
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessagesAsync(result, friend);

            var model = new ChatViewModel()
            {
                Sender = result,
                Recipient = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = await repository.GetMessagesAsync(result, friend);

            var model = new ChatViewModel()
            {
                Sender = result,
                Recipient = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            await repository.CreateAsync(item);
            await _unitOfWork.SaveChangesAsync();

            var mess = await repository.GetMessagesAsync(result, friend);

            var model = new ChatViewModel()
            {
                Sender = result,
                Recipient = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var usergen = new GenetateUsers();
            var userlist = usergen.Populate(35);

            foreach (var user in userlist)
            {
                var result = await _userManager.CreateAsync(user, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
