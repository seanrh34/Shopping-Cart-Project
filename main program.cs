using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Shopping_Cart
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool endSession = false;
            string admin_password = "p@ssw0rd";
            double discount = 0;

            List<Product> products = new List<Product>(); //products will be added via admin login to this list
            Dictionary<Product, int> shoppingCart = new Dictionary<Product, int>(); // <productID, quantity>

            while (endSession == false)
            {
                //initial prompt
                callMenu();
                var answer = Console.ReadLine();
                Thread.Sleep(500);
                Console.WriteLine("");

                if (answer == "quit")
                {
                    Console.WriteLine("Thank you for using this application!");
                    endSession= true;
                    break;
                }

                int num;
                var isNumeric = int.TryParse(answer, out num);

                // ensure answer given is an integer from 1-6 (valid menu options)
                while (isNumeric == false || Convert.ToInt32(answer) < 1 || Convert.ToInt32(answer) > 6)
                {
                    if (isNumeric == false)
                    {
                        Console.Write("Invalid, enter an option from 1-6: ");
                        answer = Console.ReadLine();
                    }
                    else
                    {
                        Console.Write("Invalid, enter an option from 1-6: ");
                        answer = Console.ReadLine();
                    }
                    Console.WriteLine("");
                    Thread.Sleep(500);
                }

                answerMenu(Convert.ToInt32(answer), products, shoppingCart, admin_password, discount);

            }
            
        }

        // cart functions
        public static void answerMenu(int answer, List<Product> products, Dictionary<Product, int> shoppingCart, string admin_password, double discount)
        {
            while (answer < 1 || answer > 6)
            {
                Console.WriteLine("Invalid input. Enter an option from 1-7!");
                answer = Convert.ToInt32(Console.ReadLine());   
                Console.WriteLine("");
                Thread.Sleep(500);
            }

            switch (answer)
            {
                case 1:
                    // print out all items on sale, add item to cart
                    int counter = 1;
                    foreach (Product product in products)
                    {
                        product.printOut();
                        counter++;
                    }
                    Console.Write("Enter the product's ID number to add to your cart: ");
                    string toAdd = Console.ReadLine();
                    Console.WriteLine("");
                    
                    Thread.Sleep(500);

                    bool product_exists = false;
                    int indexID = -1;
                    Product selectedProduct = new Product("-1", "test", 10, 2.5);
                    // to consider how to add on to existing products in shopping cart
                    foreach (Product product in products)
                    {
                        if(product.ProductId == toAdd)
                        {
                            indexID++;
                            product_exists = true;
                            selectedProduct = product;
                        }
                    }
                    int realID = Convert.ToInt32(toAdd);

                    if (product_exists)
                    {
                        Console.WriteLine("Product already exists in shopping cart.");
                    }
                    Console.Write("Enter the quantity of said product you wish to purchase: ");
                    var newQty = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("");

                    if (product_exists)
                    {
                        shoppingCart[products[realID - 1]] += newQty;
                        selectedProduct.Stock -= newQty;
                    }
                    else
                    {
                        shoppingCart[products[realID - 1]] = newQty;
                        Console.WriteLine("we have run out of that item.");
                        Console.WriteLine("Product has been added to your shopping cart.");
                    }   

                    

                    break;
                case 2:
                    // print out all items in cart, pick 1 to remove
                    counter = 1;
                    Console.WriteLine("Current items in your shopping cart:");
                    foreach (var item in shoppingCart)
                    {
                        Console.WriteLine("{0}. {1}", counter, item.Key.ProductName);
                        counter++;
                    }

                    Console.Write("Enter the product's ID number to add to your cart: ");
                    string toRemove = Console.ReadLine();
                    Console.WriteLine("");
                    Thread.Sleep(500);
                    bool item_removed = false;

                    foreach (Product product in products)
                    {
                        if (product.ProductId == toRemove)
                        {
                            shoppingCart.Remove(product);
                            item_removed = true;
                            break;
                        }
                    }
                    if (item_removed == false)
                    {
                        Console.WriteLine("Item not found.");
                    }

                    break;
                case 3: //done
                    // print out all items in cart, including name, quantity, price
                    printCart(shoppingCart);
                    break;
                case 4:
                    // proceed to checkout, calculate and display price breakdown after promo code (if have)
                    Console.Write("Enter a coupon code: ");
                    string couponCode = Console.ReadLine();
                    Console.WriteLine("");
                    Thread.Sleep(500);

                    discount = applyDiscount(couponCode);

                    if (discount == 0)
                    {
                        Console.WriteLine("Coupon code was Invalid!");
                    }

                    double current_total = calculateTotal(shoppingCart, discount);
                    Console.WriteLine("Thank you for shopping with us.");
                    break;
                case 5:
                    // prompt admin login for admin access
                    Console.Write("Enter the admin's password: ");
                    string inputPassword = Console.ReadLine();
                    Thread.Sleep(500);

                    while (inputPassword != admin_password && inputPassword != "0")
                    {
                        Console.WriteLine("Wrong password! Enter the correct password or enter '0' to exit to menu: ");
                        inputPassword = Console.ReadLine();
                        Console.WriteLine("Welcome admin.");
                        Thread.Sleep(500);
                    }

                    if (inputPassword == admin_password)
                    {
                        bool exit = false;
                        while (exit == false)
                        {
                            adminMenu();
                            var adminAnswer = Console.ReadLine();
                            (products, exit) = answerAdminMenu(products, Convert.ToInt32(adminAnswer));
                        }
                        Console.WriteLine("");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You have entered the wrong password. Returning to main menu..");
                        Console.WriteLine("");
                        callMenu();
                        break;
                    }

            }
        }

        public static (List<Product>, bool) answerAdminMenu(List<Product> products, int answer)
        {
            switch (answer)
            {
                case 1: //add new item
                    Console.Write("Is the product an electronic product (Y/N): ");
                    string ans = Console.ReadLine();

                    Console.Write("Enter the product's name: ");
                    string productName = Console.ReadLine();
                    Console.Write("Enter the number of stock available: ");
                    var productStock = Console.ReadLine();
                    Console.Write("Enter the price of the product: $");
                    var productPrice = Console.ReadLine();

                    int listLen = products.Count();
                    if (ans == "Y") //if the product is an electronic product
                    {
                        bool hasWarranty = false;
                        Console.Write("Does the product have warranty (Y/N): ");
                        string gotWarranty = Console.ReadLine();
                        if (gotWarranty == "Y")
                        {
                            hasWarranty=true;
                        }
                        ElectronicProduct newProduct = new ElectronicProduct(Convert.ToString(listLen+1), productName, Convert.ToInt32(productStock), Convert.ToDouble(productPrice), hasWarranty);
                        products.Add(newProduct);
                    }
                    else
                    {
                        Product newProduct = new Product(Convert.ToString(listLen + 1), productName, Convert.ToInt32(productStock), Convert.ToDouble(productPrice));
                        products.Add(newProduct);
                    }
                    Console.WriteLine("Product has been added to the list.");
                    return (products, false);
                    // product ID will be based on the length of the list of products + 1
                case 2: //remove item: print all, remove based on item ID
                    foreach (Product product in products)
                    {
                        product.printOut(); 
                    }

                    Console.Write("Enter the item ID of the procut you want to remove: ");
                    var toRemove = Console.ReadLine();
                    products.RemoveAt(Convert.ToInt32(toRemove)-1);

                    int counter = 0;
                    foreach (Product product in products)
                    {
                        counter++;
                        product.ProductId = Convert.ToString(counter);
                    }
                    Console.WriteLine("Product as been removed from the list.");
                    return (products, false);
                default: //go back to standard menu
                    return (products, true);
            }
        }
        static double calculateTotal(Dictionary<Product, int> shoppingCart, double discount)
        {
            double cart_total = 0.00f;
            Console.WriteLine("You get a discount of {0}%.", discount);
            foreach (var product in shoppingCart)
            {
                double unitPrice = product.Key.Price;
                int qty = product.Value;
                double curPrice = unitPrice * qty;
                cart_total += curPrice * (100-discount)/100;
            }
            Console.WriteLine("Current shopping cart total: ${0}", cart_total.ToString("N2"));
            return cart_total;
        }
        static void addToCart(Dictionary<Product, int> shoppingCart, List<Product> productList, int itemNo, int quantity)
        {
            shoppingCart.Add(productList[itemNo + 1], quantity);
        }

        static void removeFromCart(Dictionary<Product, int> shoppingCart, int itemNo)
        {
            shoppingCart.Remove(shoppingCart.ElementAt(itemNo).Key);
        }

        static void printCart(Dictionary<Product, int> shoppingCart)
        {
            int counter = 0;
            foreach(var item in shoppingCart)
            {
                double totalPrice = (item.Key.Price) * (item.Value);
                string formattedTotalPrice = totalPrice.ToString("N2");
                Console.WriteLine("{0}. {1}, Quantity: {2}, Price: ${3}, Total Price: ${4}", counter+1, item.Key.ProductName, item.Value, item.Key.Price.ToString("N2"), formattedTotalPrice);
                counter++;
            }
        }
        static double applyDiscount(string couponCode)
        {
            List<string> coupon_codes = new List<string> { "20OFF", "33SALES", "NEWUSER" };
            double discount = 0;

            if (coupon_codes.Contains(couponCode))
            {
                if (couponCode == "20OFF")
                {
                    discount = 20;
                }
                else if (couponCode == "33SALES")
                {
                    discount = 30;
                }
                else
                {
                    discount = 0;
                }
            }
            return discount;
        }
        // main functions
        static void callMenu()
        {
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Add item to cart.");
            Console.WriteLine("2. Remove item from cart.");
            Console.WriteLine("3. Print out all items in cart and price breakdown.");
            Console.WriteLine("4. Checkout.");
            Console.WriteLine("5. Admin login.");
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 50)));
            Console.WriteLine("");
            Console.Write("Pick an option from 1-5: ");
        }
        static void adminMenu()
        {
            Console.WriteLine("1. Add new item");
            Console.WriteLine("2. Remove item.");
            Console.WriteLine("3. Go to home.");
            Console.WriteLine("");
            Console.Write("Pick an option from 1-3: ");
        }
    }
}