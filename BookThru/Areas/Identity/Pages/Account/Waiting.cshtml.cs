using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookThru.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class Waiting : PageModel
    {
        public void OnGet()
        {
        }
    }
}
