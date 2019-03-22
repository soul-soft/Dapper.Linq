using System;

namespace Dapper.Common.Example
{
    public class MemberService : IService
    {
        public ISession Session { get; set; }

        public virtual IActionResult MemberRegister(MemberModel req)
        {
            if (string.IsNullOrEmpty(req.NickName))
            {
                throw new ServiceArgumentNullException(nameof(req.NickName));
            }
            var entity = req.NewInstance();
            entity.CreateTime = DateTime.Now;
            var row = Session.From<Member>().Insert(entity);

            return new ActionResult()
            {
                Message = row > 0 ? "success" : "fail",
            };
        }

        public virtual IActionResult MemberLogin(MemberModel req)
        {
            if (string.IsNullOrEmpty(req.Account))
            {
                throw new ServiceArgumentNullException(nameof(req.Account));
            }
            if (string.IsNullOrEmpty(req.Password))
            {
                throw new ServiceArgumentNullException(nameof(req.Password));
            }
            var flag = Session.From<Member>().Where(a => a.Account == req.Account && a.Password == req.Password).Exists();
            return new ActionResult()
            {
                Message = flag ? "success" : "fail",
            };
        }

        public virtual IActionResult MemberOrderList(MemberOrderModel req)
        {
            if (req.MemberId == null)
            {
                throw new ServiceArgumentNullException(nameof(req.MemberId));
            }
            var member = Session.From<Member>()
                .Where(a => a.Id == req.MemberId)
                .Single();

            var orders = Session.From<MemberOrder>()
                .Skip(req.PageNo, 10, out int total)
                .Where(a => a.MemberId == req.MemberId)
                .Select();

            member.Orders = orders;

            return new ActionResult()
            {
                Total = total,
                Data = member,
            };
        }
    }
}
