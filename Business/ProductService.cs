using ApiProduct.Data;
using ApiProduct.Mdoels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProduct.Business
{
    public class ProductService
    {
        private readonly ProductDbContext _context;

        public ProductService(ProductDbContext productDb)
        {
            _context = productDb;
        }

        public Product Get(string Code)
        {

            Code = Code?.Trim().ToUpper();
            if (!String.IsNullOrWhiteSpace(Code))
            {
                return _context.Products.Where(
                    p => p.Code == Code).FirstOrDefault();
            }
            else
                return null;
        }


        public IEnumerable<Product> Products()
        {
            return _context.Products
                .OrderBy(p => p.Name).ToList();
        }

        public Result Insert(Product dadosProduto)
        {
            Result resultado = DataValid(dadosProduto);
            resultado.Action = "Product Inclusion";

            if (resultado.Exception.Count == 0 &&
                _context.Products.Where(
                p => p.Code == dadosProduto.Code).Count() > 0)
            {
                resultado.Exception.Add(
                    "Barcode already registered");
            }

            if (resultado.Exception.Count == 0)
            {
                _context.Products.Add(dadosProduto);
                _context.SaveChanges();
            }

            return resultado;
        }

        public Result Update(Product dadosProduto)
        {
            Result resultado = DataValid(dadosProduto);
            resultado.Action = "Product Inclusion";

            if (resultado.Exception.Count == 0)
            {
                Product produto = _context.Products.Where(
                    p => p.Code == dadosProduto.Code).FirstOrDefault();

                if (produto == null)
                {
                    resultado.Exception.Add(
                        "Not Find");
                }
                else
                {
                    produto.Name = dadosProduto.Name;
                    produto.Price = dadosProduto.Price;
                    _context.SaveChanges();
                }
            }

            return resultado;
        }

        public Result Delete(string code)
        {
            Result resultado = new Result();
            resultado.Action = "Product Inclusion";

            Product produto = Get(code);
            if (produto == null)
            {
                resultado.Exception.Add(
                    "not Find");
            }
            else
            {
                _context.Products.Remove(produto);
                _context.SaveChanges();
            }

            return resultado;
        }









        private Result DataValid(Product product)
        {
            var result = new Result();
            if (product == null)
            {
                result.Exception.Add(
                    "Fill in the Product Data");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(product.Code))
                {
                    result.Exception.Add(
                        "Fill in the Barcode");
                }
                if (String.IsNullOrWhiteSpace(product.Name))
                {
                    result.Exception.Add(
                        "Fill in the Name");
                }
                if (product.Price <= 0)
                {
                    result.Exception.Add(
                        "Product Price must be greater than zero");
                }
            }

          return  result;
                }

    }
}
