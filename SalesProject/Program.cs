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
            //var context = new StoreModelContainer();
            //context.Database.CreateIfNotExists();

            Watcher watcher = new Watcher();
            watcher.SetCatalog("D:\\Test");
            watcher.Watch();
            Console.ReadKey();
        }
    }
}
