using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace Application.Controllers
{
    public class HomeController : Controller
    {
        private Receiver _receiver;

        public HomeController()
        {
            _receiver = new Receiver("rabbitmq", "temperature");
        }

        public IActionResult Index()
        {
            List<String> messages = _receiver.GetMessages();
            return View("Messages", messages);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }

}