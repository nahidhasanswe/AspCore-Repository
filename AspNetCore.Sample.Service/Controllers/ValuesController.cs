using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AspNetCore.Repository;
using AspNetCore.Sample.Service.Model;
using AspNetCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Sample.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public ValuesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IGenericRepository<TestClass> GetUnitOfWork()
        {
            return _unitOfWork.Repository<TestClass>();
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try{

                string query = "Name == 'Nahid Hasan'";

                SqlParameter name = new SqlParameter("@name","Nahid Hasan");

                var result = await _unitOfWork.ExecFilterAsync <TestClass,Test>(query, p => new Test(){Name = p.Name});

                return new JsonResult(result);
            }catch(Exception ex)
            {
                return new JsonResult(ex.Message) {StatusCode = 400};
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Test
    {
        public string Name { get; set; }
    }
}
