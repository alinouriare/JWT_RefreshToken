using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProduct.Mdoels
{
    public class Product
    {
        public int Id { get; set; }

        private string _code;

        public string Code { get => _code; set => _code=value?.Trim().ToUpper(); }


        private string _name;

        public string Name { get => _name; set => _name = value?.Trim(); }


        public double Price { get; set; }
    }
}
