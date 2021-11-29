using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace CrypticPay.Areas.Community.Pages
{
    public class IndexModel : PageModel
    {

        private readonly Data.CrypticPayContext _context;
        private readonly Data.CrypticPayFriendshipContext _contextFriends;
        private readonly UserManager<CrypticPayUser> _userManager;
        private ChatHandler _chatter;
        private WalletHandler _walletHandler;
        private Crypto _crypto;

        public IndexModel(Data.CrypticPayContext context, Data.CrypticPayFriendshipContext contextFriends, UserManager<CrypticPayUser> userManager, ChatHandler chatHandler, WalletHandler walletHandler, Crypto crypto)
        {
            _context = context;
            _contextFriends = contextFriends;
            _userManager = userManager;
            _chatter = chatHandler;
            _walletHandler = walletHandler;
            _crypto = crypto;
        }

        // initialize as empty list to avoid errors in view
        public IList<CrypticPayUser> Users = new List<CrypticPayUser>();


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperty]
        public static List<Services.DataTypes.GroupAndMembers> UserGroups { get; set; }
        [BindProperty]
        public static Services.DataTypes.ClientCryptoPack UserAndCrypto { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; }
            public string MemberString { get; set; }
        }
        public async Task OnGet()
        {
            var currUserId = _userManager.GetUserId(User);

            var user = _walletHandler.GetUserandWallet(currUserId, _context);
            // load groups user is a member of
            UserGroups = _chatter.GroupsUserHas(user);
        }
        public class MessageBox
        {
            public List<MessageForClient> Messages { get; set; }
            public string MetaTag { get; set; }
            public bool IsEncrypted { get; set; }
        }
        public class MessageForClient
        {
            public Data.MsgEncrypted.Root MsgEncrypted { get; set; }
            // indicate whether message is incoming or outgoing
            public bool IsIn { get; set; }
        }

        public async Task<JsonResult> OnPostSearchAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var query = Input.SearchString;


            // uncomment below to return page if search bypasses client side checks and is empty

            /*if (string.IsNullOrEmpty(query))
            {
                StatusMessage = "Please enter a valid query.";
                return Page();
            }*/



            // match friend name, username, or number based on query
            Users = Utils.SearchFriends(user, _context, _contextFriends, query);
            return new JsonResult(Users);
        }
        // returns existing group w/ members or new group
        public async Task<JsonResult> OnPostCreateGroupAsync()
        {
            var currUserId = _userManager.GetUserId(User);

            var user = _walletHandler.GetUserandWallet(currUserId, _context);
            
            if (string.IsNullOrEmpty(Input.MemberString))
            {
                StatusMessage = "Please enter valid group.";
            };
       
            List<string> members = Input.MemberString.Split(',').ToList();

            // convert list of unames to list of uids
            members = await Utils.UserNamesToIds(members, _userManager);

            // get group and populate w/ messages, if any
            Services.DataTypes.GroupAndMembers group = _chatter.CreateGroup(user, members, isPublic:false);
            group.Messages = _chatter.GroupMessages(group.Group.Id);
            // UPDATE TO SUPPORT MULTIPLE MEMBERS
            var reciever = _walletHandler.GetUserandWallet(members[0], _context);
            // set recipient key for group
            group.RecipientKey = _crypto.GetUserMsgKey(reciever);
            group.Xpub = reciever.WalletKryptik.Xpub;

            var resultMsgs = new List<MessageForClient>();
            foreach (var msg in group.Messages)
            {
                Data.MsgEncrypted.Root msgEncrypted;
                bool isIn;
                if (msg.SenderId == user.Id)
                {
                    msgEncrypted = JsonConvert.DeserializeObject<Data.MsgEncrypted.Root>(msg.MessageFrom);
                    isIn = false;
                    
                }
                else
                {
                    msgEncrypted = JsonConvert.DeserializeObject<Data.MsgEncrypted.Root>(msg.MessageTo);
                    isIn = true;
                }
                var thisMsg = new MessageForClient()
                {
                    MsgEncrypted = msgEncrypted,
                    IsIn = isIn
                };
                resultMsgs.Add(thisMsg);
            }

            var result = new MessageBox()
            {
                Messages = resultMsgs,
                MetaTag = MakeMetaTag(group),
                IsEncrypted = group.Group.IsEncrypted
            };
            return new JsonResult(result);
/*            return new PartialViewResult()
            {
                ViewName = "shared/_GroupMessagesPartial",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = group
                }
            };*/
        }

        public string MakeMetaTag(Services.DataTypes.GroupAndMembers group)
        {
            var memberString = string.Join(",", group.UserIds);
            var result = $@"<div hidden id=""msgMeta"" data-members=""{memberString}"" data-group=""{group.Group.Id}"" data-recipientkey=""{group.RecipientKey}"" data-isencrypted=""{group.Group.IsEncrypted}"" data-xpub=""{group.Xpub}""><p> POOP! </p></div>";
            return result;
        }
    }
}
