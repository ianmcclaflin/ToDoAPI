using System.Web;
using System.Web.Mvc;

namespace ToDoAPI.DATA.EF
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
