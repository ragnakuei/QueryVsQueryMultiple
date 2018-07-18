using System;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dapper;
using QueryVsQueryMultiple.Model;

namespace QueryVsQueryMultiple
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 用來測試可取得資料
            //var test = new TestClass();
            //test.QueryMultiple();
            //test.MultipleQuery();

            BenchmarkRunner.Run<Benchmark>();
            Console.ReadLine();
        }
    }

    [MemoryDiagnoser]
    public class Benchmark
    {
        private readonly TestClass _benchClass = new TestClass();

        [Benchmark]
        public void MultipleQuery() => _benchClass.MultipleQuery();

        [Benchmark]
        public void QueryMultiple() => _benchClass.QueryMultiple();
    }

    internal class TestClass
    {
        private readonly string _connString =
            "Server=.\\mssql2017;Database=Northwind;Trusted_Connection=True;MultipleActiveResultSets=true";

        public TestClass()
        {
        }

        public void MultipleQuery()
        {
            Customer[] customers;
            using (var conn = new SqlConnection(_connString))
            {
                customers = conn.Query<Customer>(@"SELECT * FROM Customers").ToArray();
            }

            Category[] categories;
            using (var conn = new SqlConnection(_connString))
            {
                categories = conn.Query<Category>(@"SELECT * FROM Categories").ToArray();
            }

            Employee[] employees;
            using (var conn = new SqlConnection(_connString))
            {
                employees = conn.Query<Employee>(@"SELECT * FROM Employees").ToArray();
            }

            Order[] orders;
            using (var conn = new SqlConnection(_connString))
            {
                orders = conn.Query<Order>(@"SELECT * FROM Orders").ToArray();
            }

            OrderDetail[] orderDetails;
            using (var conn = new SqlConnection(_connString))
            {
                orderDetails = conn.Query<OrderDetail>(@"SELECT * FROM [Order Details]").ToArray();
            }
        }

        public void QueryMultiple()
        {
            var sqlScript = @"
SELECT * FROM Customers
SELECT * FROM Categories
SELECT * FROM Employees
SELECT * FROM Orders
SELECT * FROM [Order Details]
";
            Customer[]    customers;
            Category[]    categories;
            Employee[]    employees;
            Order[]       orders;
            OrderDetail[] orderDetails;

            using (var conn = new SqlConnection(_connString))
            {
                var queryResult = conn.QueryMultiple(sqlScript, null);
                customers    = queryResult.Read<Customer>().ToArray();
                categories   = queryResult.Read<Category>().ToArray();
                employees    = queryResult.Read<Employee>().ToArray();
                orders       = queryResult.Read<Order>().ToArray();
                orderDetails = queryResult.Read<OrderDetail>().ToArray();
            }
        }
    }
}