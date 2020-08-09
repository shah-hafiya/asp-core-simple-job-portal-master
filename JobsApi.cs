using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobPortal
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsApi : ControllerBase
    {
        // GET: api/<JobsApi>
        [HttpGet]
        public IEnumerable<string> Get()
        {


            return new string[] { "value1", "value2" };
        }

        // GET api/<JobsApi>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<JobsApi>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<JobsApi>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<JobsApi>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
