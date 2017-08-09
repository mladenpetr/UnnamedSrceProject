using System;
using System.Web;
using System.Web.Mvc;

namespace SrceApplicaton.Controllers
{
    public class MyAuthorization : AuthorizeAttribute
    {
        override
        protected bool AuthorizeCore(HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            return context.Session["user"] != null;
        }
    }
}