using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart
{
    class Product
    {
        private string productId;
        private string productName;
        private int stock;
        private double price;
        public Product(string aProductId, string aProductName, int aStock, double aPrice)
        {
            productId = aProductId;
            productName = aProductName;
            stock = aStock;
            price = aPrice;
        }
        
        public virtual void printOut()
        {
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
            Console.WriteLine("Product Id: #{0}", productId);
            Console.WriteLine("Product Name: {0}", productName);
            Console.WriteLine("Stock: {0}", stock);
            string formattedPrice = price.ToString("N2");
            Console.WriteLine("Price: ${0}", formattedPrice);
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
        }
        public string ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        public string ProductName
        {
            get { return productName; }
            set { productName = value; }

        }
        public int Stock
        { get { return stock; } set { stock = value; } }

        public double Price
        { get { return price; } set { price = value; } }
    }
}
