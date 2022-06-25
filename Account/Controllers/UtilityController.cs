//using MROCoatching.DataObjects.Data.Context;
//using MROCoatching.DataObjects.Models.UserManagment.Identity;
//using MROCoatching.DataObjects.ViewModel.Shared;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Controllers;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.ActionConstraints;

//namespace MROCoatching.Web.Areas.Account.Controllers
//{
//    [Area("Account")]
//    //[GroundServicePlanningAuthorizationFilter]
//    public class UtilityController : Controller
//    {
//        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
//        private ApplicationDbContext applicationDbContext;

//        public UtilityController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, ApplicationDbContext _applicationDbContext)
//        {
//            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
//            applicationDbContext = _applicationDbContext;
//        }

//        public async Task<MenuItem> GetMenuItem(string searchCriteria)
//        {
//            MenuItem menuItem = new MenuItem();

//            try
//            {
//                //Collect data
//                var actions = _actionDescriptorCollectionProvider
//                    .ActionDescriptors
//                    .Items
//                    .OfType<ControllerActionDescriptor>()
//                    .Select(a => new
//                    {
//                        a.DisplayName,
//                        a.ControllerName,
//                        a.ActionName,
//                        AttributeRouteOMPA = a.AttributeRouteInfo?.OMPA,
//                        HttpMethods = string.Join(", ", a.ActionConstraints?.OfType<HttpMethodActionConstraint>().SingleOrDefault()?.HttpMethods ?? new string[] { "any" }),
//                        Parameters = a.Parameters?.Select(p => new
//                        {
//                            Type = p.ParameterType.Name,
//                            p.Name
//                        }),
//                        ControllerClassName = a.ControllerTypeInfo.FullName,
//                        ActionMethodName = a.MethodInfo.Name,
//                        Filters = a.FilterDescriptors?.Select(f => new
//                        {
//                            ClassName = f.Filter.GetType().FullName,
//                            f.Scope //10 = Global, 20 = Controller, 30 = Action
//                        }),
//                        Constraints = a.ActionConstraints?.Select(c => new
//                        {
//                            Type = c.GetType().Name
//                        }),
//                        RouteValues = a.RouteValues.Select(r => new
//                        {
//                            r.Key,
//                            r.Value
//                        }),
//                    });

//                if (actions?.Count() > 0)
//                {
//                    string area = null;
//                    foreach (var item in actions)
//                    {
//                        area = null;
//                        if (item != null)
//                        {
//                            if (item.RouteValues?.Where(r => r.Key == "area" && r.Value != null).FirstOrDefault() != null)
//                                area = item.RouteValues?.Where(r => r.Key == "area" && r.Value != null).FirstOrDefault().Value;

//                            menuItem.MenuItems.Add(new MenuItemViewModel
//                            {
//                                ActionMethod = item.ActionName,
//                                ControllerName = item.ControllerName,
//                                Area = area,
//                                DisplayText = item.ActionName + " " + item.ControllerName,
//                                Uri = area != null ? "/" + area + "/" + item.ControllerName + "/" + item.ActionName : "/" + item.ControllerName + "/" + item.ActionName
//                            });
//                        }
//                    }
//                }

//                //make the search
//                if (menuItem.MenuItems.Count > 0)
//                    menuItem.MenuItems = menuItem.MenuItems.GroupBy(m => m.DisplayText).Select(grp => grp.FirstOrDefault()).ToList().Where(m => m.DisplayText.ToLower().Contains(searchCriteria.ToLower())).ToList();
//            }
//            catch (System.Exception)
//            {
//                return new MenuItem();
//            }

//            return menuItem;
//        }

//        public async Task<bool> SaveActionMethodAsPrivileges()
//        {
//            var menuItem = new MenuItem();

//            try
//            {
//                //Collect data
//                var actions = _actionDescriptorCollectionProvider
//                    .ActionDescriptors
//                    .Items
//                    .OfType<ControllerActionDescriptor>();
//                //.Select(a => new
//                //{
//                //    a.DisplayName,
//                //    a.ControllerName,
//                //    a.ActionName,
//                //    a.MethodInfo.DeclaringType.Get.GetCustomAttributesData(),
//                //    AttributeRouteOMPA = a.AttributeRouteInfo?.OMPA,
//                //    HttpMethods = string.Join(", ", a.ActionConstraints?.OfType<HttpMethodActionConstraint>().SingleOrDefault()?.HttpMethods ?? new string[] { "any" }),
//                //    Parameters = a.Parameters?.Select(p => new
//                //    {
//                //        Type = p.ParameterType.Name,
//                //        p.Name
//                //    }),
//                //    ControllerClassName = a.ControllerTypeInfo.FullName,
//                //    ActionMethodName = a.MethodInfo.Name,
//                //    Filters = a.FilterDescriptors?.Select(f => new
//                //    {
//                //        ClassName = f.Filter.GetType().FullName,
//                //        f.Scope //10 = Global, 20 = Controller, 30 = Action
//                //    }),
//                //    Constraints = a.ActionConstraints?.Select(c => new
//                //    {
//                //        Type = c.GetType().Name
//                //    }),
//                //    RouteValues = a.RouteValues.Select(r => new
//                //    {
//                //        r.Key,
//                //        r.Value
//                //    }),
//                //});

//                if (actions?.Count() > 0)
//                {
//                    var filteredActions = actions.Where(a =>
//                                a.MethodInfo.DeclaringType.GetCustomAttributesData().Count > 0 ? a.MethodInfo.DeclaringType.GetCustomAttributesData().Where(attr => attr.AttributeType.Name == nameof(GroundServicePlanningAuthorizationFilter)).ToList().Count() > 0 : false);

//                    string area = null;
//                    foreach (var item in filteredActions)
//                    {
//                        area = null;     

//                        if (item != null)
//                        {
//                            if (item.RouteValues?.Where(r => r.Key == "area" && r.Value != null).FirstOrDefault() != null)
//                                area = item.RouteValues?.Where(r => r.Key == "area" && r.Value != null).FirstOrDefault().Value;

//                            //var actionAttributes = item.att.MethodInfo.GetCustomAttributes(inherit: true);


//                            menuItem.MenuItems.Add(new MenuItemViewModel
//                            {
//                                ActionMethod = item.ActionName,
//                                ControllerName = item.ControllerName,
//                                Area = area,
//                                DisplayText = item.ActionName + " " + item.ControllerName,
//                                Uri = area != null ? "/" + area + "/" + item.ControllerName + "/" + item.ActionName : "/" + item.ControllerName + "/" + item.ActionName
//                            });
//                        }
//                    }
//                }

//                if (menuItem.MenuItems.Count > 0)
//                    menuItem.MenuItems = menuItem.MenuItems.GroupBy(m => m.DisplayText).Select(grp => grp.FirstOrDefault()).ToList();

//                var savedPrivileges = applicationDbContext.ApplicationPrivileges.ToList();

//                //Save to Database
//                List<ApplicationPrivilege> applicationPrivileges = new List<ApplicationPrivilege>();

//                foreach (var item in menuItem.MenuItems)
//                {
//                    if (savedPrivileges.Where(p => string.Equals(p.Action, $"{item.ControllerName}-{item.ActionMethod}", StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null)
//                    {
//                        applicationPrivileges.Add(new ApplicationPrivilege
//                        {
//                            Id = Guid.NewGuid().ToString(),
//                            Action = $"{item.ControllerName}-{item.ActionMethod}",
//                            Description = $"{item.ControllerName}-{item.ActionMethod}"
//                        });
//                    }
//                }

//                if (applicationPrivileges?.Count > 0)
//                    applicationDbContext.ApplicationPrivileges.AddRange(applicationPrivileges);

//                if (applicationDbContext.SaveChanges() > 0)
//                {
//                    //Success
//                };
//            }
//            catch (Exception ex)
//            {
//                return true;
//            }

//            return true;
//        }
//    }
//}