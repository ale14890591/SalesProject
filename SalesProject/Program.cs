using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace SalesProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //var context = new StroreModelContainer();
            //context.Database.CreateIfNotExists();

            Watcher watcher = new Watcher();
            watcher.SetCatalog("D:\\Test");
            watcher.Watch();
            Console.ReadKey();

            //Client alex = new Client() { Name = "Alex" };
            //Client bob = new Client() { Name = "Bob" };
            //Client john = new Client() { Name = "John" };

            //Manager ted = new Manager { Name = "Ted" };
            //Manager bill = new Manager { Name = "Bill" };

            //Product water = new Product { Name = "water", Price = 10 };
            //Product bread = new Product { Name = "bread", Price = 50 };

            //Order o1 = new Order { Client = alex, Date = new DateTime(2015, 1, 1), Manager = ted, Product = water };
            //Order o2 = new Order { Client = bob, Date = new DateTime(2015, 1, 2), Manager = ted, Product = water };
            //Order o3 = new Order { Client = john, Date = new DateTime(2015, 1, 2), Manager = bill, Product = bread };

            //context.ClientSet.Add(alex);
            //context.ClientSet.Add(bob);
            //context.ClientSet.Add(john);

            //context.ManagerSet.Add(ted);
            //context.ManagerSet.Add(bill);

            //context.ProductSet.Add(water);
            //context.ProductSet.Add(bread);

            //context.OrderSet.Add(o1);
            //context.OrderSet.Add(o2); 
            //context.OrderSet.Add(o3);

            //context.SaveChanges();
        }
    }
}
