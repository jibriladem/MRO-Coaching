using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.Abstractions.Identity;
using MROCoatching.Abstractions.Utility;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using MROCoatching.DataObjects.ViewModel.Identity;
using MROCoatching.Utilities.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MROCoatching.Web.Controllers
{

    //[AATMSMethodFilter]
    //[Route("controller")]
    [Area("Account")]
    [DisplayName("Account Mangement")]
    public class AccountController : Controller
    {
        ApplicationDbContext _db;
        readonly IETRoleManager etRoleManager;
        readonly IETUserManager etUserManager;
        readonly IEmailSender emailSender;
        AccessLog accessLog;
        IHttpContextAccessor _accessor;
        private readonly IActionDescriptorCollectionProvider _provider;
        [TempData]
        public string ErrorMessage { get; set; }

        LogWriter logWriter = new LogWriter();

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db, IETRoleManager _roleManager, IETUserManager _userManager, IEmailSender _emailSender, IHttpContextAccessor accessor, IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
            _accessor = accessor;

            UserManager = userManager;
            SignInManager = signInManager;
            _db = db;
            etRoleManager = _roleManager;
            etUserManager = _userManager;
            emailSender = _emailSender;
            accessLog = new AccessLog(_db);
        }

        public SignInManager<ApplicationUser> SignInManager { get; private set; }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //public ActionResult Validate(Admins admin)
        //{
        //    var _admin = _db.Admins.Where(s => s.Email == admin.Email).FirstOrDefault();
        //    if (_admin != null)
        //    {
        //        if (_admin.Password == admin.Password)
        //        {
        //            HttpContext.Session.SetString("email", _admin.Email);
        //            HttpContext.Session.SetInt32("id", _admin.Id);
        //            HttpContext.Session.SetInt32("role_id", (int)_admin.RolesId);
        //            HttpContext.Session.SetString("name", _admin.FullName);

        //            string roleId = HttpContext.Session.GetString("role_id");
        //            List<Menus> menus = _db.LinkRolesMenus.Where(s => s.RolesId == roleId).Select(s => s.Menus).ToList();

        //            DataSet ds = new DataSet();
        //            ds = ToDataSet(menus);
        //            DataTable table = ds.Tables[0];
        //            DataRow[] parentMenus = table.Select("ParentId = 0");

        //            var sb = new StringBuilder();
        //            string menuString = GenerateUL(parentMenus, table, sb);
        //            HttpContext.Session.SetString("menuString", menuString);
        //            HttpContext.Session.SetString("menus", JsonConvert.SerializeObject(menus));

        //            return Json(new { status = true, message = "Login Successfull!" });
        //        }
        //        else
        //        {
        //            return Json(new { status = true, message = "Invalid Password!" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { status = false, message = "Invalid Email!" });
        //    }
        //}

        private string GenerateUL(DataRow[] menu, DataTable table, StringBuilder sb)
        {
            if (menu.Length > 0)
            {
                foreach (DataRow dr in menu)
                {
                    string url = dr["Url"].ToString();
                    string menuText = dr["Name"].ToString();
                    string icon = dr["Icon"].ToString();

                    if (url != "")
                    {
                        string line = String.Format(@"<li class=""nav-item""><a href=""{0}"" class=""nav-link""><i class=""{2}""></i> <span>{1}</span></a></li>", url, menuText, icon);
                        sb.Append(line);
                    }

                    string pid = dr["MenuId"].ToString();
                    string parentId = dr["ParentId"].ToString();

                    DataRow[] subMenu = table.Select(String.Format("ParentId = '{0}'", pid));
                    if (subMenu.Length > 0 && !pid.Equals(parentId))
                    {
                        string line = String.Format(@"<li class=""nav-item has-treeview""><a href=""#"" class=""nav-link active""><i class=""{0}""></i> <p>{1}<i class=""right fa fa-angle-left""></i></p></a><ul class=""nav nav-treeview"">", icon, menuText);
                        var subMenuBuilder = new StringBuilder();
                        sb.AppendLine(line);
                        sb.Append(GenerateUL(subMenu, table, subMenuBuilder));
                        sb.Append("</ul></li>");
                    }
                }
            }
            return sb.ToString();
        }
        public string MenuToBeConstracted(List<Menus> menus, DataRow[] menu, DataTable table, StringBuilder sb)
        {
            foreach (var dr in menus)
            {
                string url = dr.Url.ToString();
                string menuText = dr.Name.ToString();
                string icon = dr.Icon.ToString();

                string pid = dr.MenuId.ToString();
                string parentId = dr.ParentId.ToString();

                DataRow[] subMenu = table.Select(String.Format("ParentId = '{0}'", pid));
                if (subMenu.Length > 0 && !pid.Equals(parentId))
                {
                    string line = String.Format(@"<li class=""nav-item has-treeview""><a href=""#"" class=""nav-link active""><i class=""{0}""></i> <p>{1}<i class=""right fa fa-angle-left""></i></p></a><ul class=""nav nav-treeview"">", icon, menuText);
                    var subMenuBuilder = new StringBuilder();
                    sb.AppendLine(line);
                    sb.Append(GenerateUL(subMenu, table, subMenuBuilder));
                    sb.Append("</ul></li>");
                }
            }
            return sb.ToString();
        }
        public List<Menus> LoopMenus(List<Menus> menus, Menus menuWithPrivilage)
        {
            var menuConstracted = new List<Menus>();
            if (menuWithPrivilage != null)
            {
                menuConstracted.Add(menuWithPrivilage);
                foreach (var menu in menus)
                {
                    menuWithPrivilage = menus.Where(con => con.MenuId == menuWithPrivilage.ParentId).FirstOrDefault();
                    if (menuWithPrivilage.ParentId != null)
                    {
                        menuConstracted.Add(menuWithPrivilage);
                    }
                    else
                        return menuConstracted;
                }
            }
            return menuConstracted;
        }
        public string MenuBasedOnPrivilage(DataRow[] menu, DataTable table, StringBuilder sb, List<string> privilages, List<Menus> menus)
        {
            var menuStartPrivilage = new Menus();
            var orderMenu = new Menus();
            var getFirstMenu = new Menus();
            var afterLoopingMenu = new List<Menus>();
            var allMenuConstract = new List<Menus>();
            var rootMenu = menus.Where(con => con.ParentId == null && con.Name == "ROOT").FirstOrDefault();
            //menus = menus.Where(con => con.ParentId != null).ToList();
            if (menu.Length > 0)
            {
                foreach (var privilage in privilages)
                {
                    menuStartPrivilage = menus.Where(con => con.Privilages == privilage).FirstOrDefault();
                    if (menuStartPrivilage != null)
                    {
                        afterLoopingMenu = LoopMenus(menus, menuStartPrivilage);
                        if (afterLoopingMenu.Count > 0)
                            foreach (var menuIn in afterLoopingMenu)
                                allMenuConstract.Add(menuIn);
                    }
                }

                if (allMenuConstract.Count > 0)
                {
                    orderMenu = allMenuConstract.Where(con => con.ParentId == rootMenu.MenuId).FirstOrDefault();
                    allMenuConstract = allMenuConstract.OrderBy(con => con.ParentId == rootMenu.MenuId).Distinct().ToList();
                    getFirstMenu = allMenuConstract.Last();
                    DataSet ds = new DataSet();
                    ds = ToDataSet(allMenuConstract);
                    table = ds.Tables[0];
                    DataRow[] parentMenus = table.Select(String.Format("ParentId = '{0}'", getFirstMenu.ParentId));
                    //var sb = new StringBuilder();
                    //MenuBasedOnPrivilage(parentMenus, table, sb, privilages, menuList);
                    string menuString = GenerateUL(parentMenus, table, sb);
                    HttpContext.Session.SetString("menuString", menuString);
                }
            }
            return sb.ToString();
        }
        public DataSet ToDataSet<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable);
            return ds;
        }
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            ViewBag.ReturnUrl = "/Account/Home/LandingPage";
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]        
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //MenuGenerator menugenerator = new MenuGenerator(_provider);
            //menugenerator.RouteValue();
            //Append missed ZEROS
            //string userName = model.Username.Trim();
            //var routes = _provider.ActionDescriptors.Items.Select(x => new
            //{
            //    Action = x.RouteValues["Action"],
            //    Controller = x.RouteValues["Controller"],
            //    x.AttributeRouteInfo.Name,
            //    x.AttributeRouteInfo.MROCoatching
            //}).ToList();
            //string appendableDigit = "";
            //for (int i = 0; i < (8 - userName.Length); i++)
            //    appendableDigit += "0";

            //model.Username = appendableDigit + model.Username.Trim();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            //[Authorize(Roles = "Admin, Manager, HR, Coach")]
            if (result.Succeeded)
            {
                var user = _db.Users.Where(u => u.UserName == model.Username).ToList().FirstOrDefault();
                //Token generation
                // var userAutntic = _userService.Authenticate(model.Username, model.Password, user);
                var roles = await etUserManager.GetUserRolesAsync(user.Id);
                string name = _db.Roles.FirstOrDefault(x => x.Id == roles).Name; //Added by JB
                var users = _db.Users.Include(ur => ur.UserRoles).ToList();
                //int roleId = (int)HttpContext.Session.GetInt32("role_id");
                //List<Menus> menus = _db.LinkRolesMenus.Select(s => s.Menus).ToList();
                //List<Menus> menuList = _db.Menus.ToList();
                //    List<string> PermissionList = (from p in _db.ApplicationPrivileges
                //                                   join rp in _db.ApplicationRolePrivileges on
                //                                   p.Id
                //                                       equals
                //                                       rp.PrivilegeId
                //                                   join r in _db.Roles on rp.RoleId
                //                                   equals r.Id
                //                                   join ur in _db.UserRoles on r.Id
                //                                   equals ur.RoleId
                //                                   where ur.UserId == user.Id
                //                                   select p.Action).Distinct().ToList();

                //DataSet ds = new DataSet();
                //ds = ToDataSet(menus);
                //DataTable table = ds.Tables[0];
                //DataRow[] parentMenus = table.Select("ParentId is null");
                //var sb = new StringBuilder();
                //MenuBasedOnPrivilage(parentMenus, table, sb, PermissionList, menuList);
                //string menuString = GenerateUL(parentMenus, table, sb);
                //HttpContext.Session.SetString("menuString", menuString);
                //HttpContext.Session.SetString("menus", JsonConvert.SerializeObject(menus));
                //if (userAutntic != null)
                //{
                //    HttpContext.Session.SetString("JWToken", userAutntic.Token);
                //}
                if (user.FirstLogin)
                    return RedirectToAction("ChangePassword", "Account", new { area = "Account" });//return RedirectToAction("ChangePassword");
                else
                {
                    if (name == "HR")
                    {
                        returnUrl = "/Account/Home/LandingPage1";
                    }
                    else if (name == "Manager")
                    {
                        returnUrl = "/Account/Home/LandingPage2";
                    }
                    else if (name == "Coach")
                    {
                        returnUrl = "/Account/Home/LandingPage3";
                    }
                    else if (name == "User")
                    {
                        //returnUrl = "/Account/Home/LandingPage2";
                    }
                }
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
                return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe, area = "Account" });

            if (result.IsLockedOut)
                return View("Lockout");
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Logoff()
        //{
        //    HttpContext.Session.Clear();
        //    return Redirect("~/Account/Login");
        //}
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                logWriter.CreateLog($"User with ID {user.Id} logged in with 2fa.", "AccountController", "LoginWith2fa");
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                logWriter.CreateLog($"User with ID {user.Id} account locked out.", "AccountController", "LoginWith2fa");
                return RedirectToAction(nameof(Lockout), "Account", new { area = "Account" });
            }
            else
            {
                logWriter.CreateLog($"Invalid authenticator code entered for user with ID {user.Id}.", "AccountController", "LoginWith2fa");
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        public async Task<bool> IsUserValid(string userName, string password)
        {
            var result = await SignInManager.PasswordSignInAsync(userName, password, false, lockoutOnFailure: false);

            if (result.Succeeded)
                return true;
            else
                return false;
        }

        //
        // GET: /Account/Index
        [DisplayName("AccountMangement")]
        [HttpGet]
        public ActionResult Index(string role)
        {
            var accounts = new List<RegisterViewModel>();

            var users = _db.Users.Include(ur => ur.UserRoles).ToList();

            if (string.IsNullOrEmpty(role))
            {
                foreach (var user in users)
                {
                    var account = new RegisterViewModel
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName
                    };
                    accounts.Add(account);
                }
            }
            else
            {
                var applicationRole = _db.Roles.Where(r => r.Name == role).FirstOrDefault();

                if (applicationRole != null)
                {
                    var userRoles = _db.UserRoles.Where(ur => ur.RoleId == applicationRole.Id).ToList();

                    foreach (var user in users)
                    {
                        if (userRoles.Where(ur => ur.UserId == user.Id).ToList().Count > 0)
                        {
                            var account = new RegisterViewModel
                            {
                                Username = user.UserName,
                                Email = user.Email,
                                FullName = user.FullName
                            };

                            accounts.Add(account);
                        }
                    }

                    ViewBag.UsersUnderAGivenRole = role;// "Under " + role + " Role";
                }
            }

            if (TempData["SuccessAlertMessage"] != null)
            {
                ViewBag.SuccessAlertMessage = TempData["SuccessAlertMessage"];
                TempData["SuccessAlertMessage"] = null;
            }

            if (TempData["FailureAlertMessage"] != null)
            {
                ViewBag.FailureAlertMessage = TempData["FailureAlertMessage"];
                TempData["FailureAlertMessage"] = null;
            }

            return View(accounts);
        }

        //
        // GET: /Account/Unauthorized
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Unauthorized()
        {
            //Session.Abandon();
            HttpContext.Session.Clear();
            return View();
        }

        //
        // GET: /Account/Update
        [DisplayName("AccountMangement")]
        [HttpGet]
        [MROCoatchingAuthorizationFilter]
        public ActionResult Update(string userName)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
            {
                string selected = "";

                var userRole = _db.UserRoles.Where(ur => ur.UserId == user.Id).ToList();

                foreach (var item in userRole)
                    selected += item.RoleId + ",";

                ViewBag.Selected = selected;
                ViewBag.Roles = _db.Roles.ToList();

                RegisterViewModel model = new RegisterViewModel();
                model.Username = user.UserName;
                return View(model);
            }
            else
                return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [MROCoatchingAuthorizationFilter]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(RegisterViewModel model)
        {
            string role = Request.Form["role"];

            if (role != null)
            {
                ////Append missed ZEROS
                //string userName = model.Username.Trim();

                //string appendableDigit = "";
                //for (int i = 0; i < (8 - userName.Length); i++)
                //    appendableDigit += "0";

                //model.Username = appendableDigit + model.Username.Trim();
                ////
                var user = _db.Users.FirstOrDefault(x => x.UserName == model.Username);

                if (user != null)
                {
                    await etUserManager.ClearUserRoles(user.Id);

                    string[] roles = role.Split(',');
                    foreach (var item in roles)
                    {
                        string name = _db.Roles.FirstOrDefault(x => x.Id == item).Name;
                        await etUserManager.AddUserToRole(user.Id, name);
                    }

                    accessLog.Save(this.ControllerContext.RouteData.Values["action"].ToString(), User.Identity.Name, "Account has been updated for Username = " + user.UserName + " and role has changed to " + role, _accessor.HttpContext.Connection.LocalIpAddress.ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                    TempData["SuccessAlertMessage"] = "Account updated successfully.";
                    return RedirectToAction("Index", "Account", new { area = "Account" });
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // GET: /Account/Register
        [DisplayName("AccountMangement")]
        [HttpGet]
        [MROCoatchingAuthorizationFilter]
        public ActionResult Register()
        {
            ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [MROCoatchingAuthorizationFilter]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model/*, FormCollection formCollection*/)
        {
            var role = Request.Form["role"];

            model.Role = role;
            if (ModelState.IsValid)
            {
                //Append missed ZEROS               
                //string userName = model.Username.Trim();
                ////string xx = Request.Form["Role"];
                //string appendableDigit = "";
                //for (int i = 0; i < (8 - userName.Length); i++)
                //    appendableDigit += "0";
                //model.Username = appendableDigit + model.Username.Trim();
                //
                var verifyuser = _db.Employees.Where(c => c.EmployeeId == model.Username && c.Status == "Y").FirstOrDefault();//Added by JB
                if (verifyuser != null)
                {
                    var user = new ApplicationUser { UserName = model.Username, Email = model.Email, FullName = model.FullName };
                    var weakPasswords = _db.PasswordStore.Select(con => con.Password).ToList();
                    if (weakPasswords.Contains(model.Password))
                    {
                        // If we got this far, something failed, redisplay form
                        ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
                        ModelState.AddModelError("", "Password should be stronger");
                        return View(model);
                    }
                    else
                    {
                        var passwordStrength = PasswordCheck.GetPasswordStrength(model.Password);
                        if (passwordStrength == PasswordStrength.VeryStrong || passwordStrength == PasswordStrength.Strong)
                        {
                            var result = await UserManager.CreateAsync(user, model.Password);
                            if (result.Succeeded)
                            {
                                if (model.Role.Contains(","))
                                {
                                    var roleArray = model.Role.Split(',');
                                    foreach (var item in roleArray)
                                        await etUserManager.AddUserToRole(user.Id, item);
                                }
                                else
                                    await etUserManager.AddUserToRole(user.Id, model.Role);

                                accessLog.Save(this.ControllerContext.RouteData.Values["action"].ToString(), User.Identity.Name, "Account has been created with Username = " + user.UserName + " Email = " + user.Email, _accessor.HttpContext.Connection.LocalIpAddress.ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                                //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                                // Send an email with this link
                                //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                                TempData["SuccessAlertMessage"] = "User successfully registered.";
                                return RedirectToAction("Index", "Account", new { area = "Account" });
                            }
                            //AddErrors(result);
                            TempData["FailureAlertMessage"] = GetErrors(result);
                            return RedirectToAction("Index", "Account", new { area = "Account" });

                        }
                        else
                        {
                            // If we got this far, something failed, redisplay form
                            ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
                            ModelState.AddModelError("", "Password should be stronger");
                            return View(model);
                        }
                    }
                }
                else
                {
                    // If we got this far, something failed, redisplay form
                    ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
                    return View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        [DisplayName("AccountMangement")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var usertId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await UserManager.FindByIdAsync(usertId);
            var weakPasswords = _db.PasswordStore.Select(con => con.Password).ToList();
            if (weakPasswords.Contains(model.NewPassword))
            {
                ModelState.AddModelError("", "Password should be stronger");
                return View(model);
            }
            else
            {
                var passwordStrength = PasswordCheck.GetPasswordStrength(model.NewPassword);
                if (passwordStrength == PasswordStrength.VeryStrong || passwordStrength == PasswordStrength.Strong)
                {
                    if (user != null)
                    {
                        var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                        if (result.Succeeded)
                        {
                            user.FirstLogin = false;

                            await UserManager.UpdateAsync(user);
                            await SignInManager.SignInAsync(user, isPersistent: false);

                            TempData["SuccessAlertMessage"] = "Password has changed successfully.";
                            return RedirectToAction("Login", "Account", new { area = "Account" });
                        }

                        AddErrors(result);
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Invalid password change attempt.");
                }
                else
                {
                    ModelState.AddModelError("", "Password should be stronger");
                    return View(model);
                }
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [DisplayName("AccountMangement")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Append missed ZEROS
                string userName = model.Username.Trim();

                string appendableDigit = "";
                for (int i = 0; i < (8 - userName.Length); i++)
                    appendableDigit += "0";

                model.Username = appendableDigit + model.Username.Trim();
                //
                var user = await UserManager.FindByNameAsync(model.Username);

                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                await emailSender.SendEmailAsync("<p>Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></p>", new List<string> { user.Email }, "Reset Password");

                return RedirectToAction("ForgotPasswordConfirmation", "Account", new { area = "Account" });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [DisplayName("AccountMangement")]
        [HttpGet]
        [MROCoatchingAuthorizationFilter]
        public async Task<ActionResult> ResetPassword(string code)
        {
            if (code == null)
            {
                code = "_resetCode";
                //throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };

            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MROCoatchingAuthorizationFilter]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Append missed ZEROS
            string userName = model.Username.Trim();

            string appendableDigit = "";
            for (int i = 0; i < (8 - userName.Length); i++)
                appendableDigit += "0";

            model.Username = appendableDigit + model.Username.Trim();
            //
            var weakPasswords = _db.PasswordStore.Select(con => con.Password).ToList();
            if (weakPasswords.Contains(model.Password))
            {
                ModelState.AddModelError("", "Password should be stronger");
                return View(model);
            }
            else
            {
                var passwordStrength = PasswordCheck.GetPasswordStrength(model.Password);
                if (passwordStrength == PasswordStrength.VeryStrong || passwordStrength == PasswordStrength.Strong)
                {
                    var user = await UserManager.FindByNameAsync(model.Username);

                    if (user == null)
                    {
                        // Don't reveal that the user does not exist
                        return RedirectToAction("ResetPasswordConfirmation", "Account", new { area = "Account" });
                    }
                    model.Code = await UserManager.GeneratePasswordResetTokenAsync(user);

                    var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        accessLog.Save(this.ControllerContext.RouteData.Values["action"].ToString(), User.Identity.Name, "Password Reset for Username = " + user.UserName, _accessor.HttpContext.Connection.LocalIpAddress.ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                        return RedirectToAction("ResetPasswordConfirmation", "Account", new { area = "Account" });
                    }

                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("", "Password should be stronger");
                    return View(model);
                }

            }

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/LogOff
        [DisplayName("AccountMangement")]
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult> LogOff()
        {
            //await _tokenManager.DeactivateCurrentAsync();
            HttpContext.Session.Clear();
            await SignInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Account" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login), "Account", new { area = "Account" });

            }
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login), "Account", new { area = "Account" });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                logWriter.CreateLog($"User logged in with {info.LoginProvider} provider.", "AccountController", "ExternalLoginCallback");
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout), "Account", new { area = "Account" });
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);
                        logWriter.CreateLog($"User created an account using {info.LoginProvider} provider.", "AccountController", "ExternalLoginConfirmation");
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> CancelAccessToken()
        //{
        //    await _tokenManager.DeactivateCurrentAsync();

        //    return NoContent();
        //}

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //return RedirectToAction(nameof(HomeController.LandingPage), "Home");
                return RedirectToAction("LandingPage", "Home", new { area = "Account" });
            }
        }
        private string GetErrors(IdentityResult result)
        {
            string message = "";
            foreach (var error in result.Errors)
            {
                message = message + error + " ";
            }
            return message;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }

                //if (SignInManager != null)
                //{
                //    SignInManager.Dispose();
                //    SignInManager = null;
                //}
            }

            base.Dispose(disposing);
        }
    }
}
