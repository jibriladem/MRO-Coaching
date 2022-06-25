using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.Models.General
{
    public class Dashboard
    {
        public List<Wholedata> Wholedatas { get; set; }
        public List<Completedtask> Completedtask { get; set; }
        public List<Notcompleted> Notcompleted { get; set; }
        public List<Menus> Menuss { get; set; }
    }
    public class Wholedata
    {
        public string key { get; set; }
        public decimal value { get; set; }
    }
    public class Completedtask
    {
        public string key { get; set; }
        public decimal value { get; set; }
    }
    public class Notcompleted
    {
        public string key { get; set; }
        public decimal value { get; set; }
    }
}
