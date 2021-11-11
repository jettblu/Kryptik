using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Services
{
    public class ChatHandler
    {
        private readonly Data.CrypticPayContext _context;
        public ChatHandler(Data.CrypticPayContext context)
        {
            _context = context;
        }
        public class GroupAndMembers{
            public Data.Group Group { get; set; }
            public IEnumerable<string> UserIds { get; set; }
        }
        public GroupAndMembers CreateGroup(Areas.Identity.Data.CrypticPayUser creator, List<string> memberIds, bool isPrivate=true)
        {
            Data.Group newGroup = new Data.Group { Creator = creator.Id,  Public = isPrivate, CreationTime = DateTime.Now};
            _context.Groups.Add(newGroup);
            Data.GroupUser creatorUserGroup = new Data.GroupUser { CrypticPayUserId = creator.Id, GroupId = newGroup.Id };
            foreach (var memberId in memberIds)
            {
                _context.GroupUsers.Add(
                     new Data.GroupUser { CrypticPayUserId = memberId, GroupId = newGroup.Id }
                 );
            }
            // save changes once DB is modified
            _context.SaveChanges();
            var groupAndMembers = new GroupAndMembers()
            {
                Group = newGroup,
                UserIds = memberIds
            };
            return groupAndMembers;
        }
        
        // gets group with x of x members... empty if none
        public GroupAndMembers GroupWithMembers(List<string> members)
        {
            var groupUsers = _context.Groups.GroupJoin(_context.GroupUsers,
                         gr => gr.Id,
                         gu => gu.GroupId,
                         (gr, guCollection) =>
                             new GroupAndMembers
                             {
                                 Group = gr,
                                 UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                             });
            var result = groupUsers.First(grp => grp.UserIds == members);
            return result;
        }
        // all of the groups a user is a member of
        public IQueryable<GroupAndMembers> GroupsUserHas (Areas.Identity.Data.CrypticPayUser user)
        {
            var groupUsers= _context.GroupUsers.Where(gu => gu.CrypticPayUserId == user.Id);
            var result = _context.Groups.GroupJoin(groupUsers,
                        gr => gr.Id,
                        gu => gu.GroupId,
                        (gr, guCollection) =>
                             new GroupAndMembers
                             {
                                 Group = gr,
                                 UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                             });
            return result;
        }
    }
}
