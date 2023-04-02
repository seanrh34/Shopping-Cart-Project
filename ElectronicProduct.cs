using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart
{
    class ElectronicProduct:Product
    {
        private bool warranty;
        public ElectronicProduct(string aProductId, string aProductName, int aStock, double aPrice, bool aWarranty) : base(aProductId, aProductName, aStock, aPrice)
        {
            warranty = aWarranty;
        }

        public bool Warranty
        {
            get { return warranty; }
            set {
                warranty = value;
            }
        }
        public override void printOut()
        {
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
            Console.WriteLine("Product Id: #{0}", ProductId);
            Console.WriteLine("Product Name: {0}", ProductName);
            Console.WriteLine("Stock: {0}", Stock);
            Console.WriteLine("Price: ${0}", String.Format("{0:0.00}", Price));
            Console.WriteLine("Warranty: {0}", Convert.ToString(Warranty));
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
        }
    }
}
