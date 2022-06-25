using System.Collections.Generic;

namespace MROCoatching.DataObjects.ViewModel.Shared
{
    public class MenuItem
    {
        public List<MenuItemViewModel> MenuItems { get; set; }
        public MenuItem()
        {
            MenuItems = new List<MenuItemViewModel>();
        }
    }

    public class MenuItemViewModel
    {
        public string Area { get; set; }
        public string ActionMethod { get; set; }
        public string ControllerName { get; set; }
        public string DisplayText { get; set; }
        public string Uri { get; set; }
    }
}
