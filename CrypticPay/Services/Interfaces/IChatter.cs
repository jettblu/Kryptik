using CrypticPay.Areas.Identity.Data;
using System.Collections.Generic;
using System.Linq;

namespace CrypticPay.Services
{
    public interface IChatter<T>
    {
        T CreateGroup(CrypticPayUser creator, List<string> memberIds, bool isPrivate = true);
        List<T> GroupsUserHas(CrypticPayUser user);
        T PrivateGroupWithMembers(List<string> members);
        IEnumerable<Data.ChatData> GroupMessages(string groupId);
    }
}