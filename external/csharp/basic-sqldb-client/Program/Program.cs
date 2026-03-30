using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDatabase.Net.Server.Client;

namespace ServerClientExample {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("Running OpenCloseConnection Example ");
            OpenCloseConnection();

            Console.WriteLine("Running CreateDropDatabase Example ");
            CreateDropDatabase();

            Console.WriteLine("Running CreateDropUser Example ");
            CreateDropUser();

            Console.WriteLine("Running CreateTable Example ");
            CreateTable();

            Console.WriteLine("Running ORMClient Example ");
            ORMClient();

            Console.WriteLine("Running CacheServer Example ");
            CacheServer();

            Console.WriteLine("press enter key to exit.");
            Console.ReadLine();
        }


        static void OpenCloseConnection() {
            var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            Console.WriteLine(connection .State);
            connection.Close();
            connection.Dispose();
            Console.WriteLine("OpenCloseConnection() Completed");
        }

        static void CreateDropDatabase()
        {
            var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            if (connection.State == ConnectionState.Open) {
                var command = new SQLDatabaseCommand(connection);
                var utility = new SQLDatabaseUtility();
                utility.Command = command;
                utility.CreateDatabase("TestDatabase");
                utility.DropDatabase("TestDatabase");
            }
            connection.Close();
            connection.Dispose();
            Console.WriteLine("CreateDropDatabase() Completed");
        }


        static void CreateDropUser() {
            var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            if (connection.State == ConnectionState.Open) {
                var command = new SQLDatabaseCommand(connection);
                var utility = new SQLDatabaseUtility();
                utility.Command = command;
                utility.CreateUser("testuser", "testpass");
                // utility.Grant("TestDatabase", "testuser");                
                utility.DropUser("testuser");                
            }            
            connection.Close();
            connection.Dispose();
            Console.WriteLine("CreateDropUser() Completed");
        }

        static void CreateTable() {
           var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            if (connection.State == ConnectionState.Open) {
                SQLDatabaseResultSet[] recordset;
                var command = new SQLDatabaseCommand(connection);
                var utility = new SQLDatabaseUtility();
                utility.Command = command;
                utility.CreateDatabase("testdb");
                connection.DatabaseName = "testdb";
                connection.MultipleActiveResultSets = true;

                command.CommandText = "Create table if not exists testtable (id integer, textvalue text);";
                recordset = command.ExecuteNonQuery();

                command.CommandText = "Insert Into testtable VALUES (1, 'example 1');";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT * FROM testtable;";
                recordset = command.ExecuteReader();

                foreach (SQLDatabaseResultSet resultset in recordset) { 
                    if (resultset != null) {
                        if (!string.IsNullOrWhiteSpace(resultset.ErrorMessage)) {
                            Console.WriteLine(resultset.ErrorMessage);
                        } else {
                            for (int r = 0; r < resultset.RowCount; r++)  {
                				for (int c = 0; c < resultset.ColumnCount; c++) {
                                    Console.Write(resultset.Columns[c] + "(" + resultset.DataTypes[c] + ")");
                                    Console.Write("\t");
                                }

                                Console.WriteLine("");

                                for (int c = 0; c < resultset.ColumnCount; c++)
                                {
                                    Console.Write(resultset.Rows[r][c]);
                                    Console.Write("\t");
                                }

                            }

                            Console.WriteLine("");
                        }

                    }
                }
                command.CommandText = "DROP TABLE testtable;";
                command.ExecuteNonQuery();
                utility.DropDatabase("testdb");
            }
            connection.Close();
            connection.Dispose();
            Console.WriteLine("CreateTable() Completed");
        }

        static void ORMClient() {
            var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            if (connection.State == ConnectionState.Open) {
 
               var command = new SQLDatabaseCommand(connection);
                var utility = new SQLDatabaseUtility();
                utility.Command = command;
                utility.CreateDatabase("ormtestdb");
                connection.DatabaseName = "ormtestdb";

                var applicationUser = new ApplicationUser();
                var orm = new SQLDatabaseOrmClient<ApplicationUser>();
                orm.Connection = connection;
                orm.CreateTable(applicationUser);

                applicationUser.Id = 1;
                applicationUser.Name = "SQLUser";
                applicationUser.Job = "SQL Developer";

                orm.Add(applicationUser); // add

                applicationUser = orm.GetById(1); //get one by id
                
                Console.WriteLine("Id \t {0} ", applicationUser.Id);
                Console.WriteLine("Name \t {0} ", applicationUser.Name);
                Console.WriteLine("Job \t {0} ", applicationUser.Job);

                applicationUser.Job = "New Job";
                orm.Update(applicationUser);
                
                // Get all
                IList<ApplicationUser> userList = orm.GetAll();

                //Filter example;
                var filter = new SQLDatabaseOrmClient<ApplicationUser>.Filter<ApplicationUser>();
                filter.Add(x => x.Id, 1);//get user with id of 1

                //methods for order by and contains including limiting number of returned rows.
                //f.Add(x => x.Name, "SQLUser");
                //f.OrderBy(x => x.Name, "DESC");
                //f.Contains(x => x.Name, "u");
                //f.Limit(10, 10);
                

                //to find use following
                IList<ApplicationUser> foundUsers = orm.Find(filter).ToList();

                //to remove use following
                orm.Remove(filter);

                //remove or drop entire entity
                orm.DropTable(applicationUser);
                utility.DropDatabase("ormtestdb");
            }
            connection.Close();
            connection.Dispose();
            Console.WriteLine("ORMClient() Completed");
        }

        static void CacheServer() {
           var connection = new SQLDatabaseConnection();
            connection.Server = "192.168.0.10";
            connection.Port = 5000;
            connection.Username = "sysadm";
            connection.Password = "system";
            connection.Open();
            if (connection.State == ConnectionState.Open) {
                var cacheServer = new SQLDatabaseCacheServer();
                cacheServer.Connection = connection;

                // In Cache server collections are automatically created if one does not exist.
                //Add remove raw bytes with Cache Id of 101 and collection name System.String
                //if trying to exchange strings or data with other programing languages use raw
                cacheServer.AddRaw("System.String", Encoding.UTF8.GetBytes("Example Text for Cache Server"), "101");
                string c101 = Encoding.UTF8.GetString((byte[])cacheServer.Get("System.String", "101")).ToString();
                cacheServer.Remove("System.String", "101");

                cacheServer.Add<string>("Example Text for Cache Server", "101");
                c101 = cacheServer.Get<string>("101");
                cacheServer.Remove<string>("101");


                var user = new ApplicationUser();
                user.Id = 1;
                user.Name = "SQLUser";
                user.Job = "SQL Developer";

                string id = cacheServer.Add<ApplicationUser>(user);
                ApplicationUser applicationUser = cacheServer.Get<ApplicationUser>(id);
                Console.WriteLine("Id \t {0} ", applicationUser.Id);
                Console.WriteLine("Name \t {0} ", applicationUser.Name);
                Console.WriteLine("Job \t {0} ", applicationUser.Job);

                List<string> collectionList = cacheServer.CollectionList();
                foreach (string collectionName in collectionList)
                    Console.WriteLine("Collection : {0}",collectionName);

                
                cacheServer.DropCollection("System.String");
                cacheServer.DropCollection<ApplicationUser>();

            }

            connection.Close();
            connection.Dispose();
            Console.WriteLine("CacheServer() Completed");
        }
    }


    [Serializable()]
    public class ApplicationUser {
        [DBColumn(AutoIncrement = true, PrimaryKey = true)]
        public long Id { get; set; }
        [DBColumn]
        public string Name { get; set; }
        [DBColumn]
        public string Job { get; set; }
    }
}
