using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Services
{
    public class ChatHandler : IChatter<DataTypes.GroupAndMembers>
    {
        private readonly Data.CrypticPayContext _context;
        public ChatHandler(Data.CrypticPayContext context)
        {
            _context = context;
        }

        public DataTypes.GroupAndMembers CreateGroup(Areas.Identity.Data.CrypticPayUser creator, List<string> memberIds, bool isPrivate = true)
        {
            // add creator to members
            memberIds.Add(creator.Id);
            // if private group w/ these members already exists then return that group
            var existingGroup = PrivateGroupWithMembers(memberIds);
            if (existingGroup != null) return existingGroup;

            Data.Group newGroup = new Data.Group { Creator = creator.Id, Public = isPrivate, CreationTime = DateTime.Now };
            _context.Groups.Add(newGroup);
            // create new groupuser for each member
            foreach (var memberId in memberIds)
            {
                _context.GroupUsers.Add(
                     new Data.GroupUser { CrypticPayUserId = memberId, GroupId = newGroup.Id }
                 );
            }
            // save changes once DB is modified
            _context.SaveChanges();
            var groupAndMembers = new DataTypes.GroupAndMembers()
            {
                Group = newGroup,
                UserIds = memberIds
            };
            return groupAndMembers;
        }

        // gets group with x of x members... empty if none
        public DataTypes.GroupAndMembers PrivateGroupWithMembers(List<string> members)
        {
            // get private groups
            var privateGroups = _context.Groups.Where(gr => gr.Public == false);
            var groupUsers = privateGroups.GroupJoin(_context.GroupUsers,
                         gr => gr.Id,
                         gu => gu.GroupId,
                         (gr, guCollection) =>
                             new DataTypes.GroupAndMembers
                             {
                                 Group = gr,
                                 UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                             });
            var result = groupUsers.FirstOrDefault(grp => grp.UserIds == members);
            return result;
        }
        // all of the groups a user is a member of
        public IQueryable<DataTypes.GroupAndMembers> GroupsUserHas(Areas.Identity.Data.CrypticPayUser user)
        {
            var groupUsers = _context.GroupUsers.Where(gu => gu.CrypticPayUserId == user.Id);
            var result = _context.Groups.GroupJoin(groupUsers,
                        gr => gr.Id,
                        gu => gu.GroupId,
                        (gr, guCollection) =>
                             new DataTypes.GroupAndMembers
                             {
                                 Group = gr,
                                 UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                             });
            return result;
        }
    }
}
