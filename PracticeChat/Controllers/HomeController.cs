using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PracticeChat.Database;
using PracticeChat.Models;
using PracticeChat.ViewModels;
using SignalRChat.Hubs;

namespace PracticeChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly dataContext _context;
        private readonly IHubContext<ChatHub> chatHub;
        public string CurrentUid { get; set; }
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        [Obsolete]
        private readonly IHostingEnvironment hostingEnvironment;

        [Obsolete]
        public HomeController(dataContext context,
            IHostingEnvironment hostingEnvironment,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IHubContext<ChatHub> _chatHub)
        {
            _context = context;
            chatHub = _chatHub;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult GetUser(string uid)
        {
            var ul = _context.imageInfos.Where(q => q.UserId == uid).FirstOrDefault();
            return Json(ul.PhotoPath);
        }
        public IActionResult UserDashboard()
        {
            ViewBag.sdfsdfsdf = ConnectedUser.connectedUserInfos;
            //ViewBag.number = ConnectedUser.Ids;
            //ViewBag.Name = ConnectedUser.userName;
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(login.Email);
                if (user == null)
                {
                    ViewBag.wrong = "Your email is wrong.";
                    return View();
                }
                else
                {
                    var result = await signInManager.PasswordSignInAsync(user, login.Password, false, true);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserDashboard", "Home");
                    }
                    ViewBag.pass = "Your password is wrong.";
                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Json($"Email {email} already in use");
            }
            else
            {
                return Json(true);

            }
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> IsNumberInUse(string MobileNumber)
        {
            var foundPhoneNumber = await _context.Users.FirstOrDefaultAsync(a => a.PhoneNumber == MobileNumber);
            if (foundPhoneNumber != null)
            {
                return Json($"Number {MobileNumber} already in use");
            }
            else
            {
                return Json(true);

            }
        }
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> SignUp(SignUpVM signupVM)
        {
            var user = new IdentityUser
            {
                UserName = signupVM.UserName,
                Email = signupVM.Email,
                PhoneNumber = signupVM.MobileNumber
            };
            var result = await userManager.CreateAsync(user, signupVM.Password);
            if (result.Succeeded)
            {
                string uniqueFileName = UploadedFile(signupVM);
                ImageInfo imageInfo = new ImageInfo
                {
                    UserId = user.Id,
                    PhotoPath = uniqueFileName
                };
                await _context.imageInfos.AddAsync(imageInfo);
                await _context.SaveChangesAsync();
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [Obsolete]
        private string UploadedFile(SignUpVM model)
        {
            string uniqueFileName = null;
            if (model.PhotoPath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoPath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoPath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [HttpGet]
        public IActionResult SearchList(string number)
        {
            //UID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var friend = _context.Users.FirstOrDefault(a => a.PhoneNumber == number);
            if (friend != null)
            {
                var imagePath = _context.imageInfos.Where(b => b.UserId == friend.Id).FirstOrDefault();

                SearchList searchList = new SearchList();
                if (imagePath.PhotoPath == null)
                {
                    searchList.PhotoPath = "a";
                }
                else
                {
                    searchList.PhotoPath = imagePath.PhotoPath;
                }
                searchList.UserId = friend.Id;
                searchList.UserName = friend.UserName;


                return Json(new { userId = searchList.UserId, userName = searchList.UserName, photoPath = searchList.PhotoPath });
            }
            else
            {
                return Json(null);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> ViewProfile()
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var Userview = await _context.Users.FirstOrDefaultAsync(a => a.Id == CurrentUid);
            var imagePath = _context.imageInfos.Where(b => b.UserId == Userview.Id).FirstOrDefault();
            ViewPtofileVM v = new ViewPtofileVM
            {
                UserName = Userview.UserName,
                PhoneNumber = Userview.PhoneNumber,
                Email = Userview.Email,
                PhotoPath = imagePath.PhotoPath
            };
            return View(v);
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var Userinfo = _context.Users.FirstOrDefault(a => a.Id == CurrentUid);
            var imagePath = _context.imageInfos.Where(b => b.UserId == CurrentUid).FirstOrDefault();
            var editInfo = new SignUpVM();

            editInfo.UserName = Userinfo.UserName;
            editInfo.UpdateEmail = Userinfo.Email;
            editInfo.UpdateMobileNumber = Userinfo.PhoneNumber;
            editInfo.ExistingPhoto = imagePath.PhotoPath;

            return View(editInfo);
        }
        [HttpPost]
        [Obsolete]
        public IActionResult EditProfile(SignUpVM signUpVM)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var CurrentUserinfo = _context.Users.FirstOrDefault(a => a.Id == CurrentUid);
            var userInfo = _context.Users.AsNoTracking().Where(b => b.Id != CurrentUid).ToList();
            foreach (var item in userInfo)
            {
                if (signUpVM.UserName == item.UserName)
                {
                    ViewBag.Uname = "This user name is already used.";
                    return View();
                }
                if (signUpVM.UpdateMobileNumber == item.PhoneNumber)
                {
                    ViewBag.phone = "This number is already used.";
                    return View();
                }
                if (signUpVM.UpdateEmail == item.Email)
                {
                    ViewBag.phone = "This email is already used.";
                    return View();
                }
            }

            CurrentUserinfo.Id = CurrentUid;
            CurrentUserinfo.UserName = signUpVM.UserName;
            CurrentUserinfo.NormalizedUserName = signUpVM.UserName;
            CurrentUserinfo.PhoneNumber = signUpVM.UpdateMobileNumber;
            CurrentUserinfo.Email = signUpVM.UpdateEmail;
            CurrentUserinfo.NormalizedEmail = signUpVM.UpdateEmail;
            _context.Users.Update(CurrentUserinfo);
            _context.SaveChanges();

            var imgInfo = _context.imageInfos.AsNoTracking().Where(x => x.UserId == CurrentUid).FirstOrDefault();
            imgInfo.UserId = CurrentUid;
            if (signUpVM.PhotoPath != null)
            {
                if (signUpVM.ExistingPhoto != null)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                                      "img", signUpVM.ExistingPhoto);
                    System.IO.File.Delete(filePath);
                }
                imgInfo.PhotoPath = UploadedFile(signUpVM);
            }
            _context.imageInfos.Update(imgInfo);
            _context.SaveChanges();

            return RedirectToAction("ViewProfile", "Home");
        }
        public IActionResult AddFriend(string Id)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var fndlist = _context.friendLists.ToList();
            foreach (var item in fndlist)
            {
                if (item.UserId == CurrentUid && item.FriendId == Id || item.FriendId == CurrentUid && item.UserId == Id)
                {
                    //return Json(false);
                    return RedirectToAction("UserDashboard", "Home");
                }
            }
            FriendList fl = new FriendList();

            fl.UserId = CurrentUid;
            fl.FriendId = Id;

            _context.friendLists.Add(fl);
            _context.SaveChanges();
            return RedirectToAction("UserDashboard", "Home");
        }
        [HttpGet]
        public IActionResult FindFriends()
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var frndlist = _context.friendLists.AsNoTracking().ToList();
            ////var imagePathq = _context.imageInfos.Where(b => b.UserId == CurrentUid).FirstOrDefault();
            var FFL = new List<FindFriendVM>();

            foreach (var item in frndlist)
            {
                if (item.UserId == CurrentUid)
                {
                    var friend = _context.Users.FirstOrDefault(a => a.Id.Equals(item.FriendId));
                    var imagePath = _context.imageInfos.Where(b => b.UserId == item.FriendId).FirstOrDefault();

                    FindFriendVM findFriendVM = new FindFriendVM
                    {
                        UserId = item.FriendId,
                        UserName = friend.UserName,
                        PhotoPath = imagePath.PhotoPath
                    };
                    FFL.Add(findFriendVM);
                }
                else if (item.FriendId == CurrentUid)
                {
                    var friend = _context.Users.FirstOrDefault(a => a.Id == item.UserId);
                    var imagePath = _context.imageInfos.Where(b => b.UserId == item.UserId).FirstOrDefault();
                    FindFriendVM findFriendVM = new FindFriendVM
                    {
                        UserId = item.UserId,
                        UserName = friend.UserName,
                        PhotoPath = imagePath.PhotoPath
                    };
                    FFL.Add(findFriendVM);
                }
            }
            return Json(FFL);
        }
        [HttpGet]
        public IActionResult ChatUI(string FriendId)
        {
            var friend = _context.Users.FirstOrDefault(a => a.Id == FriendId);

            var Path = _context.imageInfos.Where(b => b.UserId == FriendId).FirstOrDefault();

            var sl = new OneToOneVM();
            sl.FriendId = FriendId;
            sl.FriendUserName = friend.UserName;
            sl.PhotoPath = Path.PhotoPath;

            return View(sl);
        }
        public IActionResult FriendProfile(string FriendId)
        {           
            var Userview = _context.Users.Where(a => a.Id == FriendId).FirstOrDefault();
            var imagePath = _context.imageInfos.Where(b => b.UserId == Userview.Id).FirstOrDefault();
            ViewPtofileVM v = new ViewPtofileVM
            {
                UserName = Userview.UserName,
                PhoneNumber = Userview.PhoneNumber,
                Email = Userview.Email,
                PhotoPath = imagePath.PhotoPath
            };
            return View(v);            
        }
        [HttpPost]
        public IActionResult OneToOneChat(OneToOneVM oneToOneVM)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            OneToOne chat = new OneToOne
            {
                SenderId = CurrentUid,
                ReceiverId = oneToOneVM.FriendId,
                Message = oneToOneVM.Message,
            };
            _context.oneToOnes.Add(chat);
            _context.SaveChanges();
            return Json(true);
        }
        [HttpGet]
        public JsonResult SendMessageConnetion(string FriendId)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var a = ConnectedUser.connectedUserInfos;
            var friendinfo = _context.Users.FirstOrDefault(a => a.Id == FriendId);
            var cid = a.Where(s => s.UserName == friendinfo.UserName).ToList();
            ///ViewBag.sfsdfsdf = cid;
            var x = new List<ConnectedUserVM>();
            foreach (var item in cid)
            {
                ConnectedUserVM connectedUserVM = new ConnectedUserVM()
                {
                    Id = CurrentUid,
                    FriendUserName = item.UserName,
                    FriendConnectionId = item.ConnectionId
                };
                x.Add(connectedUserVM);
            }
            if (x.Count > 0)
            {
                return Json(x);
            }
            return Json(null);
        }
        [HttpGet]
        public IActionResult GetPrivateMessage(string FriendId)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var SenderInfo = _context.Users.Where(a => a.Id == CurrentUid).FirstOrDefault();
            var ReceiverInfo = _context.Users.Where(a => a.Id == FriendId).FirstOrDefault();
            var MessageList = _context.oneToOnes.OrderByDescending(s => s.When).ToList();
            var getMsgList = new List<GetPrivateMessageVM>();
            foreach (var item in MessageList)
            {
                GetPrivateMessageVM getPrivateMessageVM = new GetPrivateMessageVM();
                if(item.SenderId== SenderInfo.Id && item.ReceiverId==ReceiverInfo.Id)
                {
                    getPrivateMessageVM.SenderId = item.SenderId;
                    getPrivateMessageVM.SenderName = SenderInfo.UserName;
                    getPrivateMessageVM.ReceiverId = item.ReceiverId;
                    getPrivateMessageVM.ReceiverName = ReceiverInfo.UserName;
                    getPrivateMessageVM.Message = item.Message;
                    getPrivateMessageVM.DateTime = item.When.ToString("dd-MM-yyyy HH:mm tt");
                }
                if (item.SenderId == ReceiverInfo.Id && item.ReceiverId == SenderInfo.Id)
                {
                    getPrivateMessageVM.SenderId = item.SenderId;
                    getPrivateMessageVM.SenderName = ReceiverInfo.UserName;
                    getPrivateMessageVM.ReceiverId = item.ReceiverId;
                    getPrivateMessageVM.ReceiverName = SenderInfo.UserName;
                    getPrivateMessageVM.Message = item.Message;
                    getPrivateMessageVM.DateTime = item.When.ToString("dd-MM-yyyy HH:mm tt");
                }
                getMsgList.Add(getPrivateMessageVM);
            }
            return Json(getMsgList);
        }
        [HttpPost]
        public JsonResult SaveConnectionId(string CID)
        {
            return Json(true);
        }
        [HttpGet]
        public IActionResult CreateGroup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateGroup(CreateGroupVM createGroupVM)
        {
            var valid = _context.groupInfos.Where(a => a.GroupName == createGroupVM.GroupName).FirstOrDefault();
            if (valid != null)
            {
                ViewBag.Validation = "This group is already exist";
                return View();
            }
            var groupInfo = new GroupInfo();
            groupInfo.GroupName = createGroupVM.GroupName;
            _context.groupInfos.Add(groupInfo);
            _context.SaveChanges();
            //var createGroup = new List<CreateGroup>();
            foreach (var item in createGroupVM.UserId)
            {
                CreateGroup createGroup1 = new CreateGroup();
                createGroup1.GroupId = groupInfo.GroupId;
                createGroup1.UserId = item;
                //createGroup.Add(createGroup1);
                _context.createGroups.Add(createGroup1);
                _context.SaveChanges();
            }

            return RedirectToAction("UserDashboard", "Home");
        }
        public IActionResult GroupChatUI(int GroupId)
        {
            var grpName = _context.groupInfos.Where(a => a.GroupId == GroupId).FirstOrDefault();
            var groupinfo = new GroupChatVM();
            groupinfo.Groupid = GroupId;
            groupinfo.GroupName = grpName.GroupName;
            return View(groupinfo);
        }
        public IActionResult MyGroup()
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var grplist = _context.createGroups.AsNoTracking().Where(x => x.UserId == CurrentUid).ToList();
            var grouplist = new List<MyGroupVM>();
            foreach (var item in grplist)
            {
                MyGroupVM myGroupVM = new MyGroupVM();
                myGroupVM.Groupid = item.GroupId;
                var gName = _context.groupInfos.Where(b => b.GroupId == item.GroupId).FirstOrDefault();
                myGroupVM.GroupName = gName.GroupName;
                grouplist.Add(myGroupVM);
            }
            return Json(grouplist);
        }
        [HttpPost]
        public IActionResult SendGroupMessage(GroupChatVM groupChatVM)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            GroupMeassage chat = new GroupMeassage
            {
                GroupId=groupChatVM.Groupid,
                SenderId=CurrentUid,
                Message=groupChatVM.Message
            };
            _context.groupMeassages.Add(chat);
            _context.SaveChanges();
            return Json(true);
        }
        public IActionResult GetGroupMessage(int GroupId)
        {
            var groupMessageList = _context.groupMeassages.Where(a => a.GroupId == GroupId).ToList();
            var MessageList = groupMessageList.OrderByDescending(s => s.When).ToList();
            var UserByMessage = new List<GetGroupMessageVM>();
            foreach (var item in MessageList)
            {
                var senderInfo = _context.Users.Where(x => x.Id == item.SenderId).FirstOrDefault();
                GetGroupMessageVM gl = new GetGroupMessageVM
                {
                    GroupId=item.GroupId,
                    UserId=item.SenderId,
                    UserName=senderInfo.UserName,
                    Message=item.Message,
                    DateTime= item.When.ToString("dd-MM-yyyy HH:mm tt")
                };
                UserByMessage.Add(gl);
            }
            return Json(UserByMessage);
        }
        [HttpGet]
        public IActionResult AddNewGroupMember(int Groupid)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var frndlist = _context.friendLists.AsNoTracking().ToList();
            var grpInfo = _context.groupInfos.Where(x => x.GroupId == Groupid).FirstOrDefault();
            var ANGM = new List<AddNewGroupMemberVM>();

            foreach (var item in frndlist)
            {
                if (item.UserId == CurrentUid)
                {
                    var friend = _context.Users.FirstOrDefault(a => a.Id.Equals(item.FriendId));
                    var imagePath = _context.imageInfos.Where(b => b.UserId == item.FriendId).FirstOrDefault();

                    AddNewGroupMemberVM findFriendVM = new AddNewGroupMemberVM
                    {
                        GroupId=grpInfo.GroupId,
                        GroupName=grpInfo.GroupName,
                        UserId = item.FriendId,
                        UserName = friend.UserName,
                        PhotoPath = imagePath.PhotoPath
                    };
                    ANGM.Add(findFriendVM);
                }
                else if (item.FriendId == CurrentUid)
                {
                    var friend = _context.Users.FirstOrDefault(a => a.Id == item.UserId);
                    var imagePath = _context.imageInfos.Where(b => b.UserId == item.UserId).FirstOrDefault();
                    AddNewGroupMemberVM findFriendVM = new AddNewGroupMemberVM
                    {
                        GroupId = grpInfo.GroupId,
                        GroupName = grpInfo.GroupName,
                        UserId = item.UserId,
                        UserName = friend.UserName,
                        PhotoPath = imagePath.PhotoPath
                    };
                    ANGM.Add(findFriendVM);
                }
                
            }
            return View(ANGM);
        }
        [HttpGet]
        public JsonResult AddNewMember(int GroupId, string UserId)
        {
            var grpInfo = _context.createGroups.Where(a => a.GroupId == GroupId).ToList();
            foreach (var item in grpInfo)
            {
                if(item.UserId== UserId)
                {                    
                    return Json(false);
                }
            }
            CreateGroup createGroup = new CreateGroup();
            createGroup.GroupId = GroupId;
            createGroup.UserId = UserId;
            _context.createGroups.Add(createGroup);
            _context.SaveChanges();
            return Json(true);
        }
        public IActionResult GroupMembers(int Groupid)
        {
            CurrentUid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var grpMember = _context.createGroups.Where(a => a.GroupId == Groupid).ToList();
            var a = new List<AddNewGroupMemberVM>();
            foreach (var item in grpMember)
            {
                var userName = _context.Users.Where(b => b.Id == item.UserId).FirstOrDefault();
                var groupName = _context.groupInfos.Where(b => b.GroupId == item.GroupId).FirstOrDefault();
                var imagePath = _context.imageInfos.Where(b => b.UserId == item.UserId).FirstOrDefault();
                if (item.UserId != CurrentUid)
                {
                    AddNewGroupMemberVM aa = new AddNewGroupMemberVM
                    {
                        GroupId = item.GroupId,
                        GroupName = groupName.GroupName,
                        UserId = item.UserId,
                        UserName = userName.UserName,
                        PhotoPath = imagePath.PhotoPath
                    };
                    a.Add(aa);
                }                
            }
            return View(a);
        }
        public IActionResult RemoveFriend(string Friendid,int Groupid)
        {
            var grp = _context.createGroups.Where(a => a.GroupId == Groupid).ToList();
            foreach (var item in grp)
            {
                if(item.UserId==Friendid)
                {
                    _context.createGroups.Remove(item);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("GroupMembers", new { Groupid = Groupid});
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
