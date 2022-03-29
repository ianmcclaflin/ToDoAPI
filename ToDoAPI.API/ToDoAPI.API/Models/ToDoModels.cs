using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace ToDoAPI.API.Models
{
    //Below are Data Transfer Objects, which are just C# classes that model what a resource and category should look like in 
    //this API. These models will be used to package up the data for transport to the requesting apps OR for receiving data
    //from requesting apps. 
    public class ToDoItemViewModel
    {
        internal string Description;

        public int ToDoId { get; set; }
        public string Action { get; set; }
        public bool Done { get; set; }
        public int CategoryId { get; set; }

        public virtual CategoryViewModel Category { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
}