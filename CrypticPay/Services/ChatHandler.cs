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

        public DataTypes.GroupAndMembers CreateGroup(Areas.Identity.Data.CrypticPayUser creator, List<string> memberIds, bool isPublic = false)
        {
            // add chat creator to members
            memberIds.Add(creator.Id);


            var existingGroup = PrivateGroupWithMembers(memberIds, creator);
            // if private group w/ these members already exists then return that group
            if (existingGroup != null) return existingGroup;

            Data.Group newGroup = new Data.Group { Creator = creator.Id, Public = isPublic, CreationTime = DateTime.Now };
            _context.Groups.Add(newGroup);
            // create new groupuser for each member
            foreach (var memberId in memberIds)
            {
                _context.GroupUsers.Add(
                     new Data.GroupUser { CrypticPayUserId = memberId, GroupId = newGroup.Id }
                 );
            }
            var reciever = _context.Users.Find(memberIds[0]);
            // indicate whether messaging is encrypted for new group
            // UPDATE to support >2 members
            if (reciever.WalletKryptikExists && creator.WalletKryptikExists) newGroup.IsEncrypted = true;
            else newGroup.IsEncrypted = false;
            // save changes once DB is modified
            _context.SaveChanges();
            var groupAndMembers = new DataTypes.GroupAndMembers()
            {
                Group = newGroup,
                UserIds = memberIds
            };
            return groupAndMembers;
        }

        public IEnumerable<Data.ChatData> GroupMessages(string groupId)
        {
            List<Data.ChatData> messages = _context.Chats.Where(ch => ch.GroupId == groupId).OrderBy(ch => ch.CreationTime).ToList();
            return messages;
        }

        // gets group with x of x members... empty if none
        public DataTypes.GroupAndMembers PrivateGroupWithMembers(List<string> members, Areas.Identity.Data.CrypticPayUser user)
        {
            // get private groups
            /*var privateGroups = _context.Groups.Where(gr => gr.Public == false);
            var groupUsers = privateGroups.GroupJoin(_context.GroupUsers,
                         gr => gr.Id,
                         gu => gu.GroupId,
                         (gr, guCollection) =>
                             new DataTypes.GroupAndMembers
                             {
                                 Group = gr,
                                 UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                             });
            var result = groupUsers.FirstOrDefault(grp => grp.UserIds == members);*/

           // linq query that failed with multiple group users
           /* var queryResult =
           (from gr in _context.Groups.Where(gr=>gr.Public==false)
            from gus in _context.GroupUsers.Where(gu => gu.GroupId == gr.Id).DefaultIfEmpty()
            select new DataTypes.GroupAndMembers
            {
                Group = gr,
                UserIds = _context.GroupUsers.Select(g1 => g1.CrypticPayUserId)
            }).ToList();*/

            var groupsPartOf = GroupsUserHas(user);

            var memberSet = new HashSet<string>(members);
            // find group with whose members exactly match input members
            foreach (var gCombo in groupsPartOf)
            {
                var gComboSet = new HashSet<string>(gCombo.UserIds);
                if (gComboSet.SetEquals(memberSet)) return gCombo;
            }
            // if no suchgcombo exists return NUll 
            return null;
        }
        // all of the groups a user is a member of
        public List<DataTypes.GroupAndMembers> GroupsUserHas(Areas.Identity.Data.CrypticPayUser user)
        {
            // initial implentation no longer valid with new ef core version
            /*            var result = _context.Groups.GroupJoin(_context.GroupUsers.Where(gu => gu.CrypticPayUserId == user.Id),
                                    gr => gr.Id,
                                    gu=>gu.CrypticPayUserId == user.Id,
                                    gu => gu.GroupId,
                                    (gr, guCollection) =>
                                         new DataTypes.GroupAndMembers
                                         {
                                             Group = gr,
                                             UserIds = guCollection.Select(gu => gu.CrypticPayUserId)
                                         });*/

            // linq implementation that fails w/ multiple groups
            /*var queryResult =
            (from gr in _context.Groups
             from gus in groupUsers
             select new DataTypes.GroupAndMembers
             {
                 Group = gr,
                 UserIds = groupUsers.Select(g1 => g1.CrypticPayUserId)
             }).ToList();*/


            // manual implementation... REPLACE with more efficient method
            var groupUsers = _context.GroupUsers.Where(gu => gu.CrypticPayUserId == user.Id);
            List<DataTypes.GroupAndMembers> result = new List<DataTypes.GroupAndMembers>();

            // populate result
            foreach (var gu in groupUsers)
            {
                string groupId = gu.GroupId;
                var group = _context.Groups.Find(groupId);
                var members = GroupMembers(groupId);
                // handle invalid group and member set
                if (!(group != null && members.Any())) return result;
                result.Add(new DataTypes.GroupAndMembers
                {
                    Group = group,
                    UserIds = members
                });
            };

            return result;
        }

        // returns all user ids that belong to group marked by id
        public IEnumerable<string> GroupMembers(string groupId)
        {
            return _context.GroupUsers.Where(gu => gu.GroupId == groupId).Select(g1 => g1.CrypticPayUserId).ToList();
        }
    }
}
