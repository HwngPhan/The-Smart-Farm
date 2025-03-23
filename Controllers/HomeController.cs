using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSF_mustidisProj.Models;  
using TSF_mustidisProj.Services;  

namespace TSF_mustidisProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly AdafruitService _adafruitService;
        public HomeController(AdafruitService adafruitService)
        {
            _adafruitService = adafruitService;
        }

        public async Task<IActionResult> Index()
        {
            var feeds = await _adafruitService.GetFeedsAsync();
            return View(feeds);
        }
    }
}
