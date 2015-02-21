using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Diagnostics;

namespace SalesProject
{
    public class Watcher : IDisposable
    {
        private class FileContents
        {
            public string Manager { get; set; }
            public List<OrderRecord> Orders = new List<OrderRecord>();
        }

        private struct OrderRecord
        {
            public DateTime Date { get; set; }
            public string Client { get; set; }
            public string Product { get; set; }
            public int Sum { get; set; }
        }

        public string Catalog { get; set; }
        private FileSystemWatcher _catalogWatcher;
        private bool _isDisposed = true;

        private object _lockClients = new object();
        private object _lockManagers = new object();
        private object _lockProducts = new object();

        public void SetCatalog(string path)
        {
            Catalog = path;
        }

        public void Watch()
        {
            if (Catalog != null)
            {
                var files = System.IO.Directory.GetFiles(Catalog, "*_????????.csv", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    StartFileParsingTask(file);
                }

                _catalogWatcher = new FileSystemWatcher(Catalog, "*.csv");
                _catalogWatcher.Created += (sender, e) =>
                    {
                        if (e.FullPath != null)
                        {
                            StartFileParsingTask(e.FullPath);
                        }
                    };
                _catalogWatcher.EnableRaisingEvents = true;

                _isDisposed = false;
            }
        }

        private void StartFileParsingTask(string file)
        {
            Task fileParsingTask = Task.Factory.StartNew(ParseAndAddToBase, file);
            fileParsingTask.Wait();
        }


        private void ParseAndAddToBase(object file)
        {
            AddToBase(ParseFile(file as string));
            File.Move(file as string, Catalog + "\\Processed\\" + Regex.Match(file as string, "\\p{L}*_\\d{8}.csv").Value);
        }

        private FileContents ParseFile(string file)
        {
            FileContents contents = new FileContents();


            string manager = Regex.Match(file, "\\p{L}*_").Value.TrimEnd('_');
            contents.Manager = manager;

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(',');
                        string[] date = line[0].Split('.');

                        OrderRecord record = new OrderRecord
                        {
                            Date = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0])),
                            Client = line[1],
                            Product = line[2],
                            Sum = Convert.ToInt32(line[3])
                        };

                        contents.Orders.Add(record);
                    }
                }
            }

            return contents;
        }

        private void AddToBase(FileContents contents)
        {
            Manager manager;
            Client client;
            Product product;

            using (var context = new StoreModelContainer())
            {
                lock (_lockManagers)
                {
                    if (!context.ManagerSet.Any(x => x.Name == contents.Manager))
                    {
                        ExecuteTransaction(context, "ManagerSet (Name)", String.Format("'{0}'", contents.Manager));
                    }
                    manager = context.ManagerSet.First(x => x.Name == contents.Manager);
                }
                
                foreach(var order in contents.Orders)
                {
                    lock (_lockClients)
                    {
                        if (!context.ClientSet.Any(x => x.Name == order.Client))
                        {
                            ExecuteTransaction(context, "ClientSet (Name)", String.Format("'{0}'", order.Client));
                        }
                        client = context.ClientSet.First(x => x.Name == order.Client);
                    }

                    lock (_lockProducts)
                    {
                        if (!context.ProductSet.Any(x => x.Name == order.Product))
                        {
                            ExecuteTransaction(context, "ProductSet (Name)", String.Format("'{0}'", order.Product));
                        }
                        product = context.ProductSet.First(x => x.Name == order.Product);
                    }

                    ExecuteTransaction(
                        context,
                        "OrderSet (Date, Manager_Id, Client_Id, Product_Id, Sum)",
                        String.Format("'{0}',{1},{2},{3},{4}", order.Date, manager.Id, client.Id, product.Id, order.Sum)
                        );
                }
            }
        }

        private void ExecuteTransaction(DbContext context, string table, string record)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlCommand(
                        String.Format(
                                @"INSERT INTO {0}
                                VALUES ({1});",
                                table,
                                record
                                )
                        );
                    context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_catalogWatcher != null)
                {
                    _catalogWatcher.Dispose();
                    _isDisposed = true;
                }
            }
        }
    }
}
