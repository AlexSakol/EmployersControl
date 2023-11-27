using Microsoft.AspNetCore.Mvc;
using CRUDMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using CRUDMVC.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace CRUDMVC.Controllers
{   
    public class AuthController : Controller
    {
        public DataContext context;
        public AuthController(DataContext dataContext)
        {
            this.context = dataContext;
        }

        public IActionResult Register()
        {
            if (User.IsInRole("Admin")) return View();
            else return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (User.IsInRole("Admin"))
            {
                string? password = Hasher.GetSha256Hash(user.Password);
                if (user != null)
                {
                    User? findUser = await context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                    if (findUser == null)
                    {
                        user.RoleId = 2;
                        user.Password = password;
                        if (ModelState.IsValid)
                        {
                            await context.Users.AddAsync(user);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            return RedirectToAction("UsersList");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            string? password = Hasher.GetSha256Hash(user.Password);
            var users = context.Users.ToList().Join(context.Roles.ToList(),
                    u => u.RoleId,
                    r => r.Id,
                    (u, r) => new { Login = u.Login, Password = u.Password, Role = r.Name }
                    );
            var findUser = users.FirstOrDefault(
                u => u.Login == user.Login && u.Password == password);
            if (findUser != null)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, findUser.Login),
                    new Claim(ClaimTypes.Role, findUser.Role)
                };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var handler = new JwtSecurityTokenHandler();
                string token = handler.WriteToken(jwt);
                Response.Cookies.Append("Token", token);
                return RedirectToAction("EmployerList", "Home");
            }
            else return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("Token");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> UsersList()
        {
            List<User> users = await context.Users.ToListAsync();
            List<Role> roles = await context.Roles.ToListAsync();
            var selectedUsers = users.Join(roles,
                u => u.RoleId,
                r => r.Id,
                (u, r) => new { Id = u.Id, Login = u.Login, Role = r.Name });
            if (User.IsInRole("Admin"))
            {
                ViewBag.Users = selectedUsers;
                return View();
            }
            else return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword() 
        {
            if (User.Identity.IsAuthenticated) return View();
            else return BadRequest("Вы не вошли в систему");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewPassword(User user)
        {
            string? password = Hasher.GetSha256Hash(user.Password);
            if (User.Identity.Name == user.Login)
            {
                User? foundUser = await context.Users.FirstOrDefaultAsync(
                    u => u.Login == user.Login && u.Password == password);
                if (foundUser != null)
                {
                    foundUser.Password = Hasher.GetSha256Hash(Request.Form["newPassword"].ToString());
                    context.Users.Update(foundUser);
                    await context.SaveChangesAsync();
                    Response.Cookies.Delete("Token");
                    return RedirectToAction("Login");
                }
                else return BadRequest("Вы не вошли в систему");
            }
            else return BadRequest("Попытка изменения не своего логина"); 
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetAdmin(int id)
        {
            User? user = await context.Users.FindAsync(id);
            if (user != null && user.RoleId != 1)
            {
                user.RoleId = 1;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> UnsetAdmin(int id)
        {
            User? user = await context.Users.FindAsync(id);
            if (user != null && user.RoleId == 1)
            {
                user.RoleId = 2;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("UsersList");
        }
        
    }
}
