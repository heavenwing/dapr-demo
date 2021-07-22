using Microsoft.AspNetCore.Mvc;

namespace Portal
{
    public class PersonModel
    {
        [BindProperty]
        public string Name { get; set; }
    }
}