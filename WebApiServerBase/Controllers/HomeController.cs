using System.Web.Http;

namespace WebApiServerBase.Controllers
{
    [RoutePrefix("")]
    public class HomeController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok();
        }
    }

    [RoutePrefix("api")]
    public class ApiHomeController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok();
        }
    }
}
