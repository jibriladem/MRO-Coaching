using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.Models.ViewModels
{
    public class Dashboard
    {
        public List<Empvalgrp> Empvalgrps { get; set; }
        public List<Transgrp> Transgrps { get; set; }
        public List<Menus> Menuss { get; set; }
    }
    public class Empvalgrp
    {
        public string key { get; set; }
        public decimal value { get; set; }
    }
    public class Transgrp
    {
        public string key { get; set; }
        public decimal value { get; set; }
    }
}
