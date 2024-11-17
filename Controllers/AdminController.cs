using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using task4.Models;

namespace task4.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;

        public AdminController(UserManager<CustomUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortColumn, string sortOrder)
        {
            var users = _userManager.Users.AsQueryable();
            
            ViewData["SortOrder"] = sortOrder == "asc" ? "desc" : "asc";
            ViewData["SortColumn"] = sortColumn;

            users = sortColumn switch
            {
                "Name" => sortOrder == "asc" ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName),
                "Email" => sortOrder == "asc" ? users.OrderBy(u => u.Email) : users.OrderByDescending(u => u.Email),
                "Status" => sortOrder == "asc" ? users.OrderBy(u => u.Status) : users.OrderByDescending(u => u.Status),
                "LastLoginTime" => sortOrder == "asc" ? users.OrderBy(u => u.LastLoginTime) : users.OrderByDescending(u => u.LastLoginTime),
                _ => users.OrderBy(u => u.Id),
            };

            var list = await users.ToListAsync();

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> BulkAction(string[] selectedUserIds, string action)
        {
            if (selectedUserIds == null || selectedUserIds.Length == 0)
            {
                TempData["Error"] = "No users selected.";
                return RedirectToAction("Index");
            }

            foreach (var userId in selectedUserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;

                switch (action.ToLower())
                {
                    case "block":
                        user.Status = "Blocked";
                        await _userManager.UpdateAsync(user);
                        TempData["Success"] = $"Block completed for selected users";
                        break;

                    case "unblock":
                        user.Status = "Active";
                        await _userManager.UpdateAsync(user);
                        TempData["Success"] = $"Activate completed for selected users";
                        break;

                    case "delete":
                        await _userManager.DeleteAsync(user);
                        TempData["Success"] = $"Delete completed for selected users";
                        break;
                }

            }

            return RedirectToAction("Index");
        }
    }
}
