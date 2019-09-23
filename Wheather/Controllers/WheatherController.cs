using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Wheather.Controllers
{
    public class WheatherController : Controller
    {
        private readonly PublisherRepo _publisherRepo;

        public WheatherController(PublisherRepo publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }

        public IActionResult Index(string cityName = "")
        {
            _publisherRepo.SendMessage(cityName);
            return Ok();
        }
    }
}
