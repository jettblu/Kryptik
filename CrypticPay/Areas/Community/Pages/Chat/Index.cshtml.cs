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

namespace CrypticPay.Areas.Community.Pages
{
    public class IndexModel : PageModel
    {

        private readonly Data.CrypticPayContext _context;
        private readonly Data.CrypticPayFriendshipContext _contextFriends;
        private readonly UserManager<CrypticPayUser> _userManager;
        private ChatHandler _chatter;

        public IndexModel(Data.CrypticPayContext context, Data.CrypticPayFriendshipContext contextFriends, UserManager<CrypticPayUser> userManager, ChatHandler chatHandler)
        {
            _context = context;
            _contextFriends = contextFriends;
            _userManager = userManager;
            _chatter = chatHandler;
        }

        // initialize as empty list to avoid errors in view
        public IList<CrypticPayUser> Users = new List<CrypticPayUser>();


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperty]
        public static List<Services.DataTypes.GroupAndMembers> UserGroups { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; }
            public string MemberString { get; set; }
        }
        public async Task OnGet()
        {
            // load groups user is a member of
            var user = await _userManager.GetUserAsync(User);
            UserGroups = _chatter.GroupsUserHas(user);
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
        public async Task<IActionResult> OnPostCreateGroupAsync()
        {
            var user = await _userManager.GetUserAsync(User);
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

            return new PartialViewResult()
            {
                ViewName = "shared/_GroupMessagesPartial",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = group
                }
            };
        }
    }
}
