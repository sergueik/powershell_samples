using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Utils;

namespace Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        // [Ignore]
        public void PerformanceTest()
        {
            var client = new InfluxDBClient("192.168.0.29", 8086, "user", "password", "example");

            // Create a database for this test
            List<string> databaseList = client.GetDatabaseList();
            if (!databaseList.Contains("example"))
            {
                client.CreateDatabase("example");
            }

            client.Query("DROP SERIES foo");

            // Create 
            var serie = new Serie {Name = "foo", ColumnNames = new[] {"value", "value_str"}};
            var series = new List<Serie> {serie};

            const int N = 10000;
            for (int i = 0; i < N; i++)
            {
                serie.Points.Add(new object[] {i, "yoyo"});
            }

            // Measure insert
            Stopwatch chrono = Stopwatch.StartNew();
            client.Insert(series);
            chrono.Stop();
            Debug.Write("Insert Elapsed:" + chrono.Elapsed.TotalMilliseconds + " ms" + Environment.NewLine);

            // Ugly
            Thread.Sleep(1000); // Give some time to the database to process insert. There must be a better way to do this

            // Make sure write was succesful
            List<Serie> result = client.Query("select count(value) from foo");
            Assert.AreEqual(N, result[0].Points[0][1]);

            // Measure query
            chrono.Restart();
            result = client.Query("select * from foo");
            chrono.Stop();
            Assert.AreEqual(N, result[0].Points.Count);
            Debug.Write("Query Elapsed:" + chrono.Elapsed.TotalMilliseconds + " ms" + Environment.NewLine);

            // Clean up
            client.DeleteDatabase("example");
        }

        [Test]
        // [Ignore]
        public void SerieCreationPerformanceTest()
        {
            Stopwatch chrono = Stopwatch.StartNew();

            var serie = new Serie {Name = "foo", ColumnNames = new[] {"value", "value_str"}};
            const long N = (long) 1e6;
            for (long i = 0; i < N; i++)
            {
                serie.Points.Add(new object[] {i, "some text"});
            }
            var client = new InfluxDBClient("192.168.0.29", 8086, "root", "root", "example");
            var series = new List<Serie> {serie};
            client.Insert(series);
            chrono.Stop();
            Debug.Write("Create Elapsed:" + chrono.Elapsed.TotalMilliseconds + " ms" + Environment.NewLine);
        }
    }
}