using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CSDeveloper.WebPackage;
using System.Security.Authentication;
namespace ApiSecureApp.Controllers
{

    public class ValuesController : ApiController
    {
        //[AuthorizeApiAccess]
        //[NoResponseModifier]
        [AllowAnonymous]
        [HttpGet]
        //[RequireHttps]
        public bool Login(string name, string password)
        {
            return false;
        }

        // GET api/values
        [ResponseModifier]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "value3" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
