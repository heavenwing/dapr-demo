using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Message = Environment.GetEnvironmentVariable("PORTAL_MESSAGE") ?? "Run in local";
        }

        public string Hello { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {

        }

        public async Task OnPostSubmitAsync(PersonModel person, [FromServices] IHelloService helloService)
        {
            this.Hello = await helloService.HelloAsync(person.Name);
        }
    }
}
