using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Metods;

public class AddNewUser
{
    public string login { get; set; }
    public string password { get; set; }
    public int roleId { get; set; }
  
    public void AddNewUser2(string login, string password, int roleId, DbContext _context)
    {

        var newWorker = new Worker()
        {
            Login = login,
            Password = password,
            RoleId = roleId
        };
        _context.Add(newWorker);
        _context.SaveChanges(); 
    }
}