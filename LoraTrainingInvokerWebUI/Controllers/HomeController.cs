using System.Diagnostics;
using LoraTrainingInvokerWebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoraTrainingInvokerWebUI.Controllers;

public class HomeController : Controller {
	private readonly IConfiguration _configuration;
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger, IConfiguration configuration) {
		_logger = logger;
		_configuration = configuration;
	}

	public IActionResult Index() {
		var model = new IndexViewModel();
		return View("Index", model);
	}

	public IActionResult Privacy() {
		var v = View("Privacy");
		return v;
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() {
		var v = View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		return v;
	}
}
