using LibraryManagement.Api.Data;
using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Member> GetAll()
    {
        return _context.Members.ToList();
    }

    public Member? GetById(Guid id)
    {
        return _context.Members.FirstOrDefault(m => m.Id == id);
    }

    public Member Add(Member member)
    {
        _context.Members.Add(member);
        _context.SaveChanges();
        return member;
    }

    public bool ExistsByEmail(string email)
    {
        return _context.Members.Any(m => m.Email == email);
    }
    public void Update(Member member)
    {

        _context.Members.Update(member); 
        _context.SaveChanges(); 
    }

    public void Delete(Member member)
    {

        _context.Members.Remove(member);
        _context.SaveChanges();
    }
}