using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SampleDataCollector.Controllers
{
    public interface IDumbThing
    {
        Task DoesDumbThing();
    }

    public class DumbThing : IDumbThing
    {
        private readonly byte[] _garbage = new byte[1024 * 1024];
        public async Task DoesDumbThing()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(80));
        }
    }

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDumbThing _dumbThing;
        public ValuesController(IDumbThing dumbThing)
        {
            _dumbThing = dumbThing;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            await _dumbThing.DoesDumbThing();
            return "value";
        }
    }
}
