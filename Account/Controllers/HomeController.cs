using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using MROCoatching.DataObjects.ViewModel.Identity;
using MROCoatching.Utilities.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MROCoatching.DataObjects.Models.BasicTransactions;

namespace MROCoatching.Web.Controllers
{
    [Area("Account")]
    [DisplayName("Master Data")]
    public class HomeController : Controller
    {
        private ApplicationDbContext dbContext;
        private IApiVersionDescriptionProvider provider;
        private readonly DataObjects.Models.BasicTransactions.Coachactionplans _actionplans;
        public IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        public Menus menus;
        public Menu menu;
        [NonAction]
        public void privilageCreate()
        {
            var controller = "";
            var action = "";
            string[] strList = { "Controller", "Controller" };
            string[] stringSpareted = { };
            var privilageBuilder = "";
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)));
            var model = new ApplicationPrivilege();
            var privilages = dbContext.ApplicationPrivileges.Select(con => con.Action).ToList();
            //The following will extract controllers, actions, attributes and return types:
            var controlleractionlistforAll = asm.GetTypes()
        .Where(type => typeof(Controller).IsAssignableFrom(type))
        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
        .Where(method => !method.IsDefined(typeof(NonActionAttribute)))
        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
        .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
        .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
            foreach (var controllerAndAction in controlleractionlistforAll)
            {
                controller = controllerAndAction.Controller;
                action = controllerAndAction.Action;
                stringSpareted = controller.Split(strList, 2, StringSplitOptions.RemoveEmptyEntries);
                privilageBuilder = stringSpareted[0].ToString() + "-" + action.ToString();
                if (!privilages.Contains(privilageBuilder))
                {
                    model.Id = Guid.NewGuid().ToString();
                    model.Action = privilageBuilder;
                    model.Description = privilageBuilder;
                    dbContext.ApplicationPrivileges.Add(model);
                    dbContext.SaveChanges();
                    privilages.Add(privilageBuilder);
                    privilages.ToList();
                }

            }
        }
        public HomeController(ApplicationDbContext _dbContext, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            dbContext = _dbContext;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            menus = new Menus(_dbContext);
        }
        [NonAction]
        public void OperationalBuilder(List<IGrouping<string, MenuConstractor>> masterDataConstract, List<Menus> menus, Menus menuToCreate)
        {
            foreach (var group in masterDataConstract)
            {
                foreach (var Action in group)
                {
                    if (Action.Category == "Operational")
                    {
                        if (menus.Count == 0)
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "ROOT";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = null;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                                menus.Add(menuToCreate);
                        }
                        else
                        {
                            var selectRoot = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                            if (!menus.Select(con => con.Name).Contains("Operational"))
                            {
                                menuToCreate = new Menus(dbContext);
                                //create the Root
                                menuToCreate.Name = "Operational";
                                menuToCreate.Icon = "";
                                menuToCreate.ParentId = selectRoot.MenuId;
                                menuToCreate.Url = $"";
                                var response = menuToCreate.Save() as DatabaseOperationResponse;
                                if (response.Status == OperationStatus.SUCCESS)
                                    menus.Add(menuToCreate);
                            }
                            else
                                menuToCreate = menus.Where(con => con.Name == "Operational").FirstOrDefault();

                        }
                        var selectRootFOr = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                        if (!menus.Select(con => con.Name).Contains("Operational"))
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "Operational";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = selectRootFOr.MenuId;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                                menus.Add(menuToCreate);
                        }
                        else
                            menuToCreate = menus.Where(con => con.Name == "Operational").FirstOrDefault();

                        if (Action.Action == "Index")
                        {
                            createMenuForControllerInAccountMangment(Action, menus, menuToCreate, menuToCreate.Name);
                        }

                    }
                }

            }
        }
        [NonAction]
        public void MasterDataBuilder(List<IGrouping<string, MenuConstractor>> masterDataConstract, List<Menus> menus, Menus menuToCreate)
        {
            foreach (var group in masterDataConstract)
            {
                foreach (var Action in group)
                {
                    if (Action.Category == "MasterData")
                    {
                        if (menus.Count == 0)
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "ROOT";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = null;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                            {
                                TempData["SuccessAlertMessage"] = response.Message;
                                menus.Add(menuToCreate);
                            }
                            else
                                TempData["FailureAlertMessage"] = response.Message;
                        }
                        else
                        {
                            var selectRoot = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                            if (!menus.Select(con => con.Name).Contains("Master Data"))
                            {
                                menuToCreate = new Menus(dbContext);
                                //create the Root
                                menuToCreate.Name = "Master Data";
                                menuToCreate.Icon = "";
                                menuToCreate.ParentId = selectRoot.MenuId;
                                menuToCreate.Url = $"";
                                var response = menuToCreate.Save() as DatabaseOperationResponse;
                                if (response.Status == OperationStatus.SUCCESS)
                                {
                                    TempData["SuccessAlertMessage"] = response.Message;
                                    menus.Add(menuToCreate);
                                }
                                else
                                    TempData["FailureAlertMessage"] = response.Message;
                            }
                            else
                                menuToCreate = menus.Where(con => con.Name == "Master Data").FirstOrDefault();

                        }
                        var selectRootFOr = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                        if (!menus.Select(con => con.Name).Contains("Master Data"))
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "Master Data";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = selectRootFOr.MenuId;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                                menus.Add(menuToCreate);
                        }
                        else
                            menuToCreate = menus.Where(con => con.Name == "Master Data").FirstOrDefault();

                        if (Action.Action == "Index")
                        {
                            createMenuForControllerInAccountMangment(Action, menus, menuToCreate, menuToCreate.Name);
                        }

                    }
                }

            }
        }
        [NonAction]
        public void AccountMangmentBuilder(List<IGrouping<string, MenuConstractor>> accountConstractor, List<Menus> menus, Menus menuToCreate)
        {
            foreach (var group in accountConstractor)
            {
                foreach (var Action in group)
                {
                    if (Action.Category == "AccountMangement")
                    {
                        if (menus.Count == 0)
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "ROOT";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = null;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                                menus.Add(menuToCreate);
                        }
                        else
                        {
                            var selectRoot = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                            if (!menus.Select(con => con.Name).Contains("Account Mangement"))
                            {
                                menuToCreate = new Menus(dbContext);
                                //create the Root
                                menuToCreate.Name = "Account Mangement";
                                menuToCreate.Icon = "";
                                menuToCreate.ParentId = selectRoot.MenuId;
                                menuToCreate.Url = $"";
                                var response = menuToCreate.Save() as DatabaseOperationResponse;
                                if (response.Status == OperationStatus.SUCCESS)
                                    menus.Add(menuToCreate);

                            }
                            else
                                menuToCreate = menus.Where(con => con.Name == "Account Mangement").FirstOrDefault();

                        }
                        var selectRootFOr = menus.Where(con => con.Name == "ROOT" && con.ParentId == null).FirstOrDefault();
                        if (!menus.Select(con => con.Name).Contains("Account Mangement"))
                        {
                            menuToCreate = new Menus(dbContext);
                            //create the Root
                            menuToCreate.Name = "Account Mangement";
                            menuToCreate.Icon = "";
                            menuToCreate.ParentId = selectRootFOr.MenuId;
                            menuToCreate.Url = $"";
                            var response = menuToCreate.Save() as DatabaseOperationResponse;
                            if (response.Status == OperationStatus.SUCCESS)
                                menus.Add(menuToCreate);
                        }
                        else
                            menuToCreate = menus.Where(con => con.Name == "Account Mangement").FirstOrDefault();

                        if (Action.Action == "Index" || Action.Action == "ChangePassword" || Action.Action == "ResetPassword")
                        {
                            createMenuForControllerInAccountMangment(Action, menus, menuToCreate, menuToCreate.Name);
                        }

                    }
                }

            }
        }
        [NonAction]
        public void createMenuForControllerInAccountMangment(MenuConstractor accountConstractor, List<Menus> menus, Menus menuToCreate, string name)
        {
            string[] strList = { "Controller", "Controller" };
            string[] stringSpareted = { };
            var controllerName = "";
            stringSpareted = accountConstractor.Controller.Split(strList, 2, StringSplitOptions.RemoveEmptyEntries);
            controllerName = stringSpareted[0].ToString();
            var selectParent = menus.Where(con => con.Name == name).FirstOrDefault();
            if (!menus.Select(con => con.Name).Contains(controllerName))
            {
                menuToCreate = new Menus(dbContext);
                //create the Root
                menuToCreate.Name = controllerName;
                menuToCreate.Icon = "";
                menuToCreate.ParentId = selectParent.MenuId;
                menuToCreate.Url = $"";
                var response = menuToCreate.Save() as DatabaseOperationResponse;
                if (response.Status == OperationStatus.SUCCESS)
                    menus.Add(menuToCreate);
            }
            else
                menuToCreate = menus.Where(con => con.Name == controllerName).FirstOrDefault();
            if (accountConstractor.Action == "ChangePassword")
                CreateActionIndexForController(accountConstractor, menus, menuToCreate, "Account");
            else if (accountConstractor.Action == "ResetPassword")
                CreateActionIndexForController(accountConstractor, menus, menuToCreate, "Account");
            else
                CreateActionIndexForController(accountConstractor, menus, menuToCreate, controllerName);
        }
        [NonAction]
        public void CreateActionIndexForController(MenuConstractor accountConstractor, List<Menus> menus, Menus menuToCreate, string name)
        {

            string[] strList = { "Controller", "Controller" };
            string[] stringSpareted = { };
            var privilageBuilder = "";
            var selectParent = menus.Where(con => con.Name == name).FirstOrDefault();
            var controllerName = "";
            stringSpareted = accountConstractor.Controller.Split(strList, 2, StringSplitOptions.RemoveEmptyEntries);
            controllerName = stringSpareted[0].ToString();
            privilageBuilder = stringSpareted[0].ToString() + "-" + accountConstractor.Action.ToString();
            if (!menus.Select(con => con.Name).Contains(controllerName + "s"))
            {
                menuToCreate = new Menus(dbContext);
                if (accountConstractor.Action == "Index")
                    menuToCreate.Name = controllerName + "s";
                else
                    menuToCreate.Name = accountConstractor.Action;
                //create the Root
                //menuToCreate.Name = controllerName + "s";
                menuToCreate.Icon = "fa fa-user";
                menuToCreate.ParentId = selectParent.MenuId;
                menuToCreate.Url = $"/Account/{controllerName}/{accountConstractor.Action}";
                menuToCreate.Privilages = privilageBuilder;
                var response = menuToCreate.Save() as DatabaseOperationResponse;
                if (response.Status == OperationStatus.SUCCESS)
                    menus.Add(menuToCreate);
            }

        }
        [DisplayName("Master Data")]
        //[Route("[Action]")]
        public async Task<IActionResult> Index()
        {
            //PrivilageBuillder privilageBuild = new PrivilageBuillder(dbContext);
            privilageCreate();
            var permissionChartRoleUsers = new List<PermissionChartRoleUserviewModel>();
            var userId = User.Identity.Name;
            var users = dbContext.UserRoles.ToList();
            var roles = dbContext.Roles.ToList();
            var previleges = dbContext.ApplicationPrivileges.ToList();

            foreach (var role in roles)
            {
                if (role != null)
                {
                    permissionChartRoleUsers.Add(new PermissionChartRoleUserviewModel
                    {
                        RoleName = role.Name,
                        Description = role.Description,
                        NumberOfUsers = users.Where(u => u.RoleId == role.Id).ToList().Count
                    });
                }
            }
            string[] strList = { "Controller", "Controller" };
            string[] stringSpareted = { };
            Assembly asm = Assembly.GetExecutingAssembly();
            //var ControllerDisplayName = string.Empty;
            //var ActionsDisplayName = string.Empty;
            //var ControllerAttributes = ControllerContext.ActionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            //var AreaAttribute = ControllerContext.ActionDescriptor.ControllerName;
            //if (ControllerAttributes.Length > 0)
            //  {
            //    ControllerDisplayName = ((DisplayNameAttribute)ControllerAttributes[0]).DisplayName;
            //  }
            var model = new ApplicationPrivilege();
            var privilages = dbContext.ApplicationPrivileges.Select(con => con.Action).ToList();
            string[] CustomeAtr = { };
            //The following will extract controllers, actions, attributes and return types:
            var controlleractionlistforAll = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(method => !method.IsDefined(typeof(NonActionAttribute))).Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                .Select(x => new { Areas = x.DeclaringType.Namespace.Split('.').Reverse().Skip(1).First(), Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName)) })
                .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            //var allActionOfMasterData = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            //     .Where(method => !method.IsDefined(typeof(NonActionAttribute))).Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
            //     .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, Attributes = String.Join(",", x.GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName)) }).ToList();

            //var testControllerGet = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).SelectMany(type => type.GetCustomAttributes())
            //    .Select(x => new { NameToDisplay = x.IsDefaultAttribute(), Typeid = x.TypeId }).ToList();

            //var listOfController = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).SelectMany(type => type.GetCustomAttributes<DisplayNameAttribute>())
            //    .Select(x => new { NameToDisplay = x.DisplayName, Typeid = x.TypeId  }).ToList();

            //var listOfControllerWithAreaAndDisplay = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).SelectMany(type => type.GetCustomAttributes<AreaAttribute>())
            //    .Select(x => new { NameToDisplay = x.RouteValue, Typeid = x.TypeId, otherValue = x.RouteKey }).ToList();

            //var listOfControllerArea = asm.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).ToList();
            var thisType = GetType();
            Type t;
            //saving Areas 
            var areaName = string.Empty;
            var areaFullName = string.Empty;
            string[] areaSplit = { };
            var areaList = new List<string>();
            var menuToCreate = new Menus(dbContext);
            var menu = await menus.GetList();

            var listOfControllerConstractor = new List<MenuConstractor>();
            var listOfOrderByCatagory = new List<MenuConstractor>();
            var listOfOrderByController = new List<MenuConstractor>();
            var menuConstractor = new MenuConstractor();
            foreach (var ActionInArea in controlleractionlistforAll)
            {
                menuConstractor = new MenuConstractor();
                menuConstractor.Area = ActionInArea.Areas;
                menuConstractor.Category = ActionInArea.Attributes;
                menuConstractor.Controller = ActionInArea.Controller;
                menuConstractor.Action = ActionInArea.Action;
                if (menuConstractor.Area != null)
                    listOfControllerConstractor.Add(menuConstractor);
            }
            var AreaGrouping = listOfControllerConstractor.GroupBy(con => con.Area).ToList();
            foreach (var group in AreaGrouping)
            {
                foreach (var catagory in group)
                {
                    menuConstractor = new MenuConstractor();
                    menuConstractor = catagory;
                    listOfOrderByCatagory.Add(menuConstractor);
                }
            }
            var CatagoryGrouping = listOfOrderByCatagory.GroupBy(con => con.Category).ToList();
            foreach (var group in CatagoryGrouping)
            {
                foreach (var controllerCatagory in group)
                {
                    menuConstractor = new MenuConstractor();
                    menuConstractor = controllerCatagory;
                    listOfOrderByController.Add(menuConstractor);
                }
            }
            var controllerGrouping = listOfOrderByController.GroupBy(con => con.Controller).ToList();
            //Save Action On Respective Area and Respective Root
            AccountMangmentBuilder(controllerGrouping, menu, menuToCreate);
            MasterDataBuilder(controllerGrouping, menu, menuToCreate);
            OperationalBuilder(controllerGrouping, menu, menuToCreate);
            menu = await menus.GetList();
            var parentMenuList = new List<Menus>();
            var menuStart = new Menus();
            var controllerAndActionName = new Dictionary<string, string>();


            ViewBag.PermissionChartRoleUsers = permissionChartRoleUsers;

            ViewBag.NumberOfUsers = users.Count;
            ViewBag.NumberOfRoles = roles.Count;
            ViewBag.NumberOfPrevileges = previleges.Count;
            //ViewBag.NumberOfBusinessModels = previleges.Where;

            return View();
        }

        //[Route("[action]")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult LandingPage()
        {
            //menu.MenuBuilder(User.Identity.Name);

            var user = dbContext.Users.Where(u => u.UserName == User.Identity.Name).ToList().FirstOrDefault();

            List<Menus> menus = dbContext.Menus.ToList();
            List<Menus> menuList = dbContext.Menus.ToList();

            List<string> PermissionList = (from p in dbContext.ApplicationPrivileges
                                           join rp in dbContext.ApplicationRolePrivileges on
                                           p.Id
                                               equals
                                               rp.PrivilegeId
                                           join r in dbContext.Roles on rp.RoleId
                                           equals r.Id
                                           join ur in dbContext.UserRoles on r.Id
                                           equals ur.RoleId
                                           where ur.UserId == user.Id
                                           select p.Action).Distinct().ToList();



            DataSet ds = new DataSet();
            // ds = menu.ToDataSet(menus);
            ds = ToDataSet(menus);
            DataTable table = ds.Tables[0];
            DataRow[] parentMenus = table.Select("ParentId is null");
            var sb = new StringBuilder();

            //return MenuBasedOnPrivilage(parentMenus, table, sb, PermissionList, menuList);
            //-------------------------------------
            var menuStartPrivilage = new Menus();
            var orderMenu = new Menus();
            var getFirstMenu = new Menus();
            var afterLoopingMenu = new List<Menus>();
            var allMenuConstract = new List<Menus>();
            var rootMenu = menus.Where(con => con.ParentId == null && con.Name == "ROOT").FirstOrDefault();
            //menus = menus.Where(con => con.ParentId != null).ToList();
            if (parentMenus.Length > 0)
            {
                //favorite Menus
                var favorite = dbContext.MenusFavorite.Where(s => s.ForWho == User.Identity.Name).ToList();
                foreach (var fav in favorite)
                {
                    menuStartPrivilage = new Menus();

                    var thisRow = dbContext.Menus.Where(s => s.MenuId == fav.MenuId).FirstOrDefault();
                    menuStartPrivilage.MenuId = thisRow.MenuId;
                    menuStartPrivilage.Name = thisRow.Name;
                    menuStartPrivilage.Icon = thisRow.Icon;
                    menuStartPrivilage.Url = thisRow.Url;
                    menuStartPrivilage.ParentId = thisRow.ParentId;
                    menuStartPrivilage.Privilages = thisRow.Privilages;
                    menuStartPrivilage.Description = thisRow.Description;
                    menuStartPrivilage.Favorite = true;
                    menuStartPrivilage.ERNAM = thisRow.ERNAM;
                    menuStartPrivilage.ERDAT = thisRow.ERDAT;
                    menuStartPrivilage.AENAM = thisRow.AENAM;
                    menuStartPrivilage.AEDAT = thisRow.AEDAT;
                    menuStartPrivilage.ACTIND = thisRow.ACTIND;

                    allMenuConstract.Add(menuStartPrivilage); //Add Favotite links first 
                    //menuStartPrivilage = null;
                }

                foreach (var privilage in PermissionList)
                {
                    menuStartPrivilage = menus.Where(con => con.Privilages == privilage).FirstOrDefault();
                    if (menuStartPrivilage != null)
                    {
                        //afterLoopingMenu = LoopMenus(menus, menuStartPrivilage);
                        //if (afterLoopingMenu.Count > 0)
                        //    foreach (var menuIn in afterLoopingMenu)
                        allMenuConstract.Add(menuStartPrivilage);
                    }
                }
                if (allMenuConstract.Count > 0)
                {
                    return View(allMenuConstract);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult LandingPage1()
        {
            //menu.MenuBuilder(User.Identity.Name);

            var user = dbContext.Users.Where(u => u.UserName == User.Identity.Name).ToList().FirstOrDefault();

            List<Menus> menus = dbContext.Menus.ToList();
            List<Menus> menuList = dbContext.Menus.ToList();

            List<string> PermissionList = (from p in dbContext.ApplicationPrivileges
                                           join rp in dbContext.ApplicationRolePrivileges on
                                           p.Id
                                               equals
                                               rp.PrivilegeId
                                           join r in dbContext.Roles on rp.RoleId
                                           equals r.Id
                                           join ur in dbContext.UserRoles on r.Id
                                           equals ur.RoleId
                                           where ur.UserId == user.Id
                                           select p.Action).Distinct().ToList();



            DataSet ds = new DataSet();
            // ds = menu.ToDataSet(menus);
            ds = ToDataSet(menus);
            DataTable table = ds.Tables[0];
            DataRow[] parentMenus = table.Select("ParentId is null");
            var sb = new StringBuilder();

            //return MenuBasedOnPrivilage(parentMenus, table, sb, PermissionList, menuList);
            //-------------------------------------
            var menuStartPrivilage = new Menus();
            var orderMenu = new Menus();
            var getFirstMenu = new Menus();
            var afterLoopingMenu = new List<Menus>();
            var allMenuConstract = new List<Menus>();
            var rootMenu = menus.Where(con => con.ParentId == null && con.Name == "ROOT").FirstOrDefault();
            //menus = menus.Where(con => con.ParentId != null).ToList();
            if (parentMenus.Length > 0)
            {
                //favorite Menus
                var favorite = dbContext.MenusFavorite.Where(s => s.ForWho == User.Identity.Name).ToList();
                foreach (var fav in favorite)
                {
                    menuStartPrivilage = new Menus();

                    var thisRow = dbContext.Menus.Where(s => s.MenuId == fav.MenuId).FirstOrDefault();
                    menuStartPrivilage.MenuId = thisRow.MenuId;
                    menuStartPrivilage.Name = thisRow.Name;
                    menuStartPrivilage.Icon = thisRow.Icon;
                    menuStartPrivilage.Url = thisRow.Url;
                    menuStartPrivilage.ParentId = thisRow.ParentId;
                    menuStartPrivilage.Privilages = thisRow.Privilages;
                    menuStartPrivilage.Description = thisRow.Description;
                    menuStartPrivilage.Favorite = true;
                    menuStartPrivilage.ERNAM = thisRow.ERNAM;
                    menuStartPrivilage.ERDAT = thisRow.ERDAT;
                    menuStartPrivilage.AENAM = thisRow.AENAM;
                    menuStartPrivilage.AEDAT = thisRow.AEDAT;
                    menuStartPrivilage.ACTIND = thisRow.ACTIND;

                    allMenuConstract.Add(menuStartPrivilage); //Add Favotite links first 
                    //menuStartPrivilage = null;
                }

                foreach (var privilage in PermissionList)
                {
                    menuStartPrivilage = menus.Where(con => con.Privilages == privilage).FirstOrDefault();
                    if (menuStartPrivilage != null)
                    {
                        //afterLoopingMenu = LoopMenus(menus, menuStartPrivilage);
                        //if (afterLoopingMenu.Count > 0)
                        //    foreach (var menuIn in afterLoopingMenu)
                        allMenuConstract.Add(menuStartPrivilage);
                    }
                }
                if (allMenuConstract.Count > 0)
                {
                    return View(allMenuConstract);
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult LandingPage2()
        {
            //menu.MenuBuilder(User.Identity.Name);

            var user = dbContext.Users.Where(u => u.UserName == User.Identity.Name).ToList().FirstOrDefault();

            List<Menus> menus = dbContext.Menus.ToList();
            List<Menus> menuList = dbContext.Menus.ToList();

            List<string> PermissionList = (from p in dbContext.ApplicationPrivileges
                                           join rp in dbContext.ApplicationRolePrivileges on
                                           p.Id
                                               equals
                                               rp.PrivilegeId
                                           join r in dbContext.Roles on rp.RoleId
                                           equals r.Id
                                           join ur in dbContext.UserRoles on r.Id
                                           equals ur.RoleId
                                           where ur.UserId == user.Id
                                           select p.Action).Distinct().ToList();



            DataSet ds = new DataSet();
            // ds = menu.ToDataSet(menus);
            ds = ToDataSet(menus);
            DataTable table = ds.Tables[0];
            DataRow[] parentMenus = table.Select("ParentId is null");
            var sb = new StringBuilder();

            //return MenuBasedOnPrivilage(parentMenus, table, sb, PermissionList, menuList);
            //-------------------------------------
            var menuStartPrivilage = new Menus();
            var orderMenu = new Menus();
            var getFirstMenu = new Menus();
            var afterLoopingMenu = new List<Menus>();
            var allMenuConstract = new List<Menus>();
            var rootMenu = menus.Where(con => con.ParentId == null && con.Name == "ROOT").FirstOrDefault();
            //menus = menus.Where(con => con.ParentId != null).ToList();
            if (parentMenus.Length > 0)
            {
                //favorite Menus
                var favorite = dbContext.MenusFavorite.Where(s => s.ForWho == User.Identity.Name).ToList();
                foreach (var fav in favorite)
                {
                    menuStartPrivilage = new Menus();

                    var thisRow = dbContext.Menus.Where(s => s.MenuId == fav.MenuId).FirstOrDefault();
                    menuStartPrivilage.MenuId = thisRow.MenuId;
                    menuStartPrivilage.Name = thisRow.Name;
                    menuStartPrivilage.Icon = thisRow.Icon;
                    menuStartPrivilage.Url = thisRow.Url;
                    menuStartPrivilage.ParentId = thisRow.ParentId;
                    menuStartPrivilage.Privilages = thisRow.Privilages;
                    menuStartPrivilage.Description = thisRow.Description;
                    menuStartPrivilage.Favorite = true;
                    menuStartPrivilage.ERNAM = thisRow.ERNAM;
                    menuStartPrivilage.ERDAT = thisRow.ERDAT;
                    menuStartPrivilage.AENAM = thisRow.AENAM;
                    menuStartPrivilage.AEDAT = thisRow.AEDAT;
                    menuStartPrivilage.ACTIND = thisRow.ACTIND;

                    allMenuConstract.Add(menuStartPrivilage); //Add Favotite links first 
                    //menuStartPrivilage = null;
                }

                foreach (var privilage in PermissionList)
                {
                    menuStartPrivilage = menus.Where(con => con.Privilages == privilage).FirstOrDefault();
                    if (menuStartPrivilage != null)
                    {
                        //afterLoopingMenu = LoopMenus(menus, menuStartPrivilage);
                        //if (afterLoopingMenu.Count > 0)
                        //    foreach (var menuIn in afterLoopingMenu)
                        allMenuConstract.Add(menuStartPrivilage);
                    }
                }
                if (allMenuConstract.Count > 0)
                {
                    return View(allMenuConstract);
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult LandingPage3()
        {
            //menu.MenuBuilder(User.Identity.Name);

            var user = dbContext.Users.Where(u => u.UserName == User.Identity.Name).ToList().FirstOrDefault();

            List<Menus> menus = dbContext.Menus.ToList();
            List<Menus> menuList = dbContext.Menus.ToList();

            List<string> PermissionList = (from p in dbContext.ApplicationPrivileges
                                           join rp in dbContext.ApplicationRolePrivileges on
                                           p.Id
                                               equals
                                               rp.PrivilegeId
                                           join r in dbContext.Roles on rp.RoleId
                                           equals r.Id
                                           join ur in dbContext.UserRoles on r.Id
                                           equals ur.RoleId
                                           where ur.UserId == user.Id
                                           select p.Action).Distinct().ToList();



            DataSet ds = new DataSet();
            // ds = menu.ToDataSet(menus);
            ds = ToDataSet(menus);
            DataTable table = ds.Tables[0];
            DataRow[] parentMenus = table.Select("ParentId is null");
            var sb = new StringBuilder();

            //return MenuBasedOnPrivilage(parentMenus, table, sb, PermissionList, menuList);
            //-------------------------------------
            var menuStartPrivilage = new Menus();
            var orderMenu = new Menus();
            var getFirstMenu = new Menus();
            var afterLoopingMenu = new List<Menus>();
            var allMenuConstract = new List<Menus>();
            var rootMenu = menus.Where(con => con.ParentId == null && con.Name == "ROOT").FirstOrDefault();
            //menus = menus.Where(con => con.ParentId != null).ToList();
            if (parentMenus.Length > 0)
            {
                //favorite Menus
                var favorite = dbContext.MenusFavorite.Where(s => s.ForWho == User.Identity.Name).ToList();
                foreach (var fav in favorite)
                {
                    menuStartPrivilage = new Menus();

                    var thisRow = dbContext.Menus.Where(s => s.MenuId == fav.MenuId).FirstOrDefault();
                    menuStartPrivilage.MenuId = thisRow.MenuId;
                    menuStartPrivilage.Name = thisRow.Name;
                    menuStartPrivilage.Icon = thisRow.Icon;
                    menuStartPrivilage.Url = thisRow.Url;
                    menuStartPrivilage.ParentId = thisRow.ParentId;
                    menuStartPrivilage.Privilages = thisRow.Privilages;
                    menuStartPrivilage.Description = thisRow.Description;
                    menuStartPrivilage.Favorite = true;
                    menuStartPrivilage.ERNAM = thisRow.ERNAM;
                    menuStartPrivilage.ERDAT = thisRow.ERDAT;
                    menuStartPrivilage.AENAM = thisRow.AENAM;
                    menuStartPrivilage.AEDAT = thisRow.AEDAT;
                    menuStartPrivilage.ACTIND = thisRow.ACTIND;

                    allMenuConstract.Add(menuStartPrivilage); //Add Favotite links first 
                    //menuStartPrivilage = null;
                }

                foreach (var privilage in PermissionList)
                {
                    menuStartPrivilage = menus.Where(con => con.Privilages == privilage).FirstOrDefault();
                    if (menuStartPrivilage != null)
                    {
                        //afterLoopingMenu = LoopMenus(menus, menuStartPrivilage);
                        //if (afterLoopingMenu.Count > 0)
                        //    foreach (var menuIn in afterLoopingMenu)
                        allMenuConstract.Add(menuStartPrivilage);
                    }
                }
                if (allMenuConstract.Count > 0)
                {
                    return View(allMenuConstract);
                }
            }

            return View();
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

        [Route("[action]")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //[Route("[action]")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("error/{code:int}")]
        public IActionResult Error(int code)
        {
            // handle different codes or just return the default error view
            return View();
        }
    }
}
