using System.Security.Claims;
using System.Text;
using Danek.DAL;
using Danek.DAL.Models.Users;
using Danek.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Danek.Web.Controllers
{
    //[Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<UsersController> _logger;
        private readonly AppDbContext _context;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ILogger<UsersController> logger,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        #region Вспомогательные методы

        private async Task<bool> IsSuperAdminAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            return currentUser != null && await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");
        }

        private async Task<bool> IsAdminAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null) return false;

            return await _userManager.IsInRoleAsync(currentUser, "SuperAdmin") ||
                   await _userManager.IsInRoleAsync(currentUser, "Admin");
        }

        private async Task<List<string>> GetViewableRolesAsync(ClaimsPrincipal user)
        {
            var roles = new List<string>();

            if (await IsSuperAdminAsync(user))
            {
                // Суперадмин видит все роли
                roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            }
            else if (await IsAdminAsync(user))
            {
                // Админ видит все роли кроме SuperAdmin
                roles = await _roleManager.Roles
                    .Where(r => r.Name != "SuperAdmin")
                    .Select(r => r.Name)
                    .ToListAsync();
            }

            return roles;
        }

        private async Task<UserVM> MapToViewModelAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TelegramUsername = user.TelegramUsername,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            };
        }

        #endregion

        #region CRUD операции

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] UserFilterRequest filter)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для просмотра списка пользователей";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var viewableRoles = await GetViewableRolesAsync(User);
                var isSuperAdmin = await IsSuperAdminAsync(User);

                var query = _userManager.Users.AsQueryable();

                // Фильтр по ролям
                if (!isSuperAdmin)
                {
                    // Админы не видят суперадминов
                    var superAdminIds = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                    query = query.Where(u => !superAdminIds.Select(sa => sa.Id).Contains(u.Id));
                }

                // Фильтр по поиску
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(u =>
                        u.Email.Contains(filter.SearchTerm) ||
                        u.UserName.Contains(filter.SearchTerm) ||
                        u.FirstName.Contains(filter.SearchTerm) ||
                        u.LastName.Contains(filter.SearchTerm) ||
                        u.TelegramUsername.Contains(filter.SearchTerm));
                }

                // Фильтр по активности
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);
                }

                // Фильтр по роли
                if (!string.IsNullOrEmpty(filter.Role) && viewableRoles.Contains(filter.Role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(filter.Role);
                    var userIds = usersInRole.Select(u => u.Id);
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                // Сортировка
                query = query.OrderByDescending(u => u.CreatedAt);

                // Пагинация
                var totalCount = await query.CountAsync();
                var users = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                // Маппинг к ViewModel
                var userViewModels = new List<UserVM>();
                foreach (var user in users)
                {
                    userViewModels.Add(await MapToViewModelAsync(user));
                }

                var response = new UserListResponse
                {
                    Users = userViewModels,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                ViewBag.ViewableRoles = viewableRoles;
                ViewBag.IsSuperAdmin = isSuperAdmin;
                ViewBag.Filter = filter;

                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей");
                TempData["Error"] = "Произошла ошибка при загрузке списка пользователей";
                return View(new UserListResponse());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для просмотра данных пользователя";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Пользователь не найден";
                    return RedirectToAction("Index");
                }

                // Проверка прав доступа
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!isSuperAdmin && userRoles.Contains("SuperAdmin"))
                {
                    TempData["Error"] = "У вас нет прав для просмотра этого пользователя";
                    return RedirectToAction("Index");
                }

                var viewModel = await MapToViewModelAsync(user);
                ViewBag.ViewableRoles = await GetViewableRolesAsync(User);
                ViewBag.IsSuperAdmin = isSuperAdmin;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных пользователя");
                TempData["Error"] = "Произошла ошибка при загрузке данных пользователя";
                return RedirectToAction("Index");
            }
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для создания пользователей";
                return RedirectToAction("Index");
            }

            ViewBag.ViewableRoles = await GetViewableRolesAsync(User);
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для создания пользователей";
                return RedirectToAction("Index");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Проверяем доступные роли
                    var viewableRoles = await GetViewableRolesAsync(User);
                    var invalidRoles = request.Roles.Except(viewableRoles).ToList();

                    if (invalidRoles.Any())
                    {
                        ModelState.AddModelError("Roles", $"Вы не можете назначать роли: {string.Join(", ", invalidRoles)}");
                        ViewBag.ViewableRoles = viewableRoles;
                        return View(request);
                    }

                    var user = new User
                    {
                        UserName = request.Email,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        TelegramUsername = request.TelegramUsername,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        // Назначаем роли
                        if (request.Roles.Any())
                        {
                            var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
                            if (!roleResult.Succeeded)
                            {
                                _logger.LogWarning("Не удалось назначить роли пользователю {UserId}: {Errors}",
                                    user.Id, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                            }
                        }

                        _logger.LogInformation("Создан новый пользователь: {Email}", user.Email);
                        TempData["Success"] = "Пользователь успешно создан";
                        return RedirectToAction("Details", new { id = user.Id });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                ViewBag.ViewableRoles = await GetViewableRolesAsync(User);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя");
                TempData["Error"] = "Произошла ошибка при создании пользователя";
                return View(request);
            }
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для редактирования пользователей";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Пользователь не найден";
                    return RedirectToAction("Index");
                }

                // Проверка прав доступа
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!isSuperAdmin && userRoles.Contains("SuperAdmin"))
                {
                    TempData["Error"] = "У вас нет прав для редактирования этого пользователя";
                    return RedirectToAction("Index");
                }

                var viewModel = await MapToViewModelAsync(user);
                ViewBag.ViewableRoles = await GetViewableRolesAsync(User);
                ViewBag.IsSuperAdmin = isSuperAdmin;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке формы редактирования пользователя");
                TempData["Error"] = "Произошла ошибка при загрузке формы";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateUserRequest request)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для редактирования пользователей";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Пользователь не найден";
                    return RedirectToAction("Index");
                }

                // Проверка прав доступа
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!isSuperAdmin && userRoles.Contains("SuperAdmin"))
                {
                    TempData["Error"] = "У вас нет прав для редактирования этого пользователя";
                    return RedirectToAction("Index");
                }

                // Обновление базовых данных
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;

                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;

                if (!string.IsNullOrEmpty(request.TelegramUsername))
                    user.TelegramUsername = request.TelegramUsername;

                if (request.IsActive.HasValue)
                    user.IsActive = request.IsActive.Value;

                user.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(await MapToViewModelAsync(user));
                }

                // Обновление ролей
                if (request.Roles != null)
                {
                    var viewableRoles = await GetViewableRolesAsync(User);
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var rolesToRemove = currentRoles.Except(request.Roles).ToList();
                    var rolesToAdd = request.Roles.Except(currentRoles).ToList();

                    // Фильтруем роли по правам доступа
                    rolesToAdd = rolesToAdd.Where(r => viewableRoles.Contains(r)).ToList();
                    rolesToRemove = rolesToRemove.Where(r => viewableRoles.Contains(r)).ToList();

                    if (rolesToRemove.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    }

                    if (rolesToAdd.Any())
                    {
                        await _userManager.AddToRolesAsync(user, rolesToAdd);
                    }
                }

                TempData["Success"] = "Данные пользователя успешно обновлены";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя");
                TempData["Error"] = "Произошла ошибка при обновлении пользователя";
                return RedirectToAction("Edit", new { id });
            }
        }

        [HttpPost("{id}/change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordRequest request)
        {
            if (!await IsAdminAsync(User))
            {
                return Json(new { success = false, message = "У вас нет прав для смены пароля" });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Пользователь не найден" });
                }

                // Проверка прав доступа
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!isSuperAdmin && userRoles.Contains("SuperAdmin"))
                {
                    return Json(new { success = false, message = "У вас нет прав для изменения пароля этого пользователя" });
                }

                // Сбрасываем старый пароль и устанавливаем новый
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("Пароль пользователя {UserId} изменен администратором", user.Id);
                    return Json(new { success = true, message = "Пароль успешно изменен" });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при смене пароля пользователя");
                return Json(new { success = false, message = "Произошла ошибка при смене пароля" });
            }
        }

        [HttpPost("{id}/toggle-active")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            if (!await IsAdminAsync(User))
            {
                return Json(new { success = false, message = "У вас нет прав для деактивации пользователей" });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Пользователь не найден" });
                }

                // Проверка прав доступа
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!isSuperAdmin && userRoles.Contains("SuperAdmin"))
                {
                    return Json(new { success = false, message = "У вас нет прав для деактивации этого пользователя" });
                }

                // Нельзя деактивировать себя
                var currentUser = await _userManager.GetUserAsync(User);
                if (user.Id == currentUser?.Id)
                {
                    return Json(new { success = false, message = "Вы не можете деактивировать свой собственный аккаунт" });
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var status = user.IsActive ? "активирован" : "деактивирован";
                    _logger.LogInformation("Пользователь {UserId} {Status}", user.Id, status);
                    return Json(new
                    {
                        success = true,
                        message = $"Пользователь успешно {status}",
                        isActive = user.IsActive
                    });
                }

                return Json(new
                {
                    success = false,
                    message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении статуса пользователя");
                return Json(new { success = false, message = "Произошла ошибка при изменении статуса" });
            }
        }

        [HttpPost("{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await IsSuperAdminAsync(User))
            {
                TempData["Error"] = "Только суперадмин может удалять пользователей";
                return RedirectToAction("Index");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Пользователь не найден";
                    return RedirectToAction("Index");
                }

                // Нельзя удалить себя
                var currentUser = await _userManager.GetUserAsync(User);
                if (user.Id == currentUser?.Id)
                {
                    TempData["Error"] = "Вы не можете удалить свой собственный аккаунт";
                    return RedirectToAction("Index");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь {UserId} удален", user.Id);
                    TempData["Success"] = "Пользователь успешно удален";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя");
                TempData["Error"] = "Произошла ошибка при удалении пользователя";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Дополнительные функции

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] UserFilterRequest filter)
        {
            if (!await IsAdminAsync(User))
            {
                TempData["Error"] = "У вас нет прав для экспорта данных";
                return RedirectToAction("Index");
            }

            try
            {
                var users = await _userManager.Users
                    .Where(u => u.IsActive == (filter.IsActive ?? true))
                    .OrderBy(u => u.CreatedAt)
                    .ToListAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Email,FirstName,LastName,Telegram,Active,CreatedAt,LastLogin,Roles");

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    csv.AppendLine($"\"{user.Email}\",\"{user.FirstName}\",\"{user.LastName}\",\"{user.TelegramUsername}\",{user.IsActive},{user.CreatedAt:yyyy-MM-dd},{user.LastLoginAt:yyyy-MM-dd HH:mm},\"{string.Join(",", roles)}\"");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"users_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте пользователей");
                TempData["Error"] = "Произошла ошибка при экспорте данных";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("bulk-activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkActivate([FromBody] List<string> userIds)
        {
            if (!await IsAdminAsync(User))
            {
                return Json(new { success = false, message = "У вас нет прав для массовой активации" });
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var isSuperAdmin = await IsSuperAdminAsync(User);
                var activatedCount = 0;

                foreach (var userId in userIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null && user.Id != currentUser?.Id)
                    {
                        // Проверка прав доступа
                        var userRoles = await _userManager.GetRolesAsync(user);
                        if (isSuperAdmin || !userRoles.Contains("SuperAdmin"))
                        {
                            user.IsActive = true;
                            user.UpdatedAt = DateTime.UtcNow;
                            await _userManager.UpdateAsync(user);
                            activatedCount++;
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    message = $"Активировано пользователей: {activatedCount}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при массовой активации пользователей");
                return Json(new { success = false, message = "Произошла ошибка" });
            }
        }

        #endregion
    }
}