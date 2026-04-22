using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Repositories;

namespace LibraryManagement.Api.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public IEnumerable<MemberResponse> GetMembers()
    {
        return _memberRepository.GetAll()
            .Select(m => new MemberResponse
            {
                Id = m.Id,
                FullName = m.FullName,
                Email = m.Email,
                MembershipDate = m.MembershipDate
            });
    }

    public MemberResponse? GetMemberById(Guid id)
    {
        var member = _memberRepository.GetById(id);
        if (member is null)
            return null;

        return new MemberResponse
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            MembershipDate = member.MembershipDate
        };
    }

    public MemberResponse CreateMember(CreateMemberRequest request)
    {
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            MembershipDate = DateTime.UtcNow
        };

        var created = _memberRepository.Add(member);

        return new MemberResponse
        {
            Id = created.Id,
            FullName = created.FullName,
            Email = created.Email,
            MembershipDate = created.MembershipDate
        };
    }

    public MemberResponse? UpdateMember(Guid id, UpdateMemberRequest request)
    {
        var member = _memberRepository.GetById(id);
        
        if (member is null)
            return null;

        member.FullName = request.FullName;
        member.Email = request.Email;

        _memberRepository.Update(member);

        return new MemberResponse
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            MembershipDate = member.MembershipDate
        };
    }

    public bool DeleteMember(Guid id)
    {
        var member = _memberRepository.GetById(id);
        
        if (member is null)
            return false;

        _memberRepository.Delete(member);
        
        return true;
    }
}