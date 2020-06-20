using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProduct.Business;
using ApiProduct.Mdoels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProduct.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("Bearer")]
    public class ProductController : ControllerBase
    {
        private ProductService _service;
        

        public ProductController(ProductService service)
        {
            _service = service;
        }


        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _service.Products();
        }

        [HttpGet("{code}")]
        public ActionResult<Product> Get(string code)
        {
            var produto = _service.Get(code);
            if (produto != null)
                return produto;
            else
                return NotFound();
        }

        [HttpPost]
        public Result Post([FromBody] Product produto)
        {
            return _service.Insert(produto);
        }

        [HttpPut]
        public Result Put([FromBody] Product produto)
        {
            return _service.Update(produto);
        }

        [HttpDelete("{code}")]
        public Result Delete(string code)
        {
            return _service.Delete(code);
        }
    }
}
