using SqlSugar;
using System;
using System.Diagnostics;
using System.Threading;

namespace MysqlDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var cnt = 0;
            for (var i = 0; i < 50; i++)
            {
                var thread = new Thread(async s =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    Random random = new Random();
                    while (sw.Elapsed < TimeSpan.FromSeconds(360))
                    {
                        try
                        {
                            #region SqlSugarClient
                            using (var db = GetDb())
                            {
                                var param = new { userid = random.Next(0, 10000000) };
                                var result = await db.Ado.SqlQuerySingleAsync<string>("SELECT nickname from users where userid=@userid;", param);
                                Console.WriteLine($"结果:{result}");
                            }
                            #endregion

                            #region MySqlConector OpenAsync
                            //var connString = "server=;port=;user=;password=;database=;charset=utf8mb4;Old Guids=true;SslMode=None;MAX Pool Size=20;MIN Pool Size=0;Connect Timeout = 180;Allow User Variables=True;";
                            //using (var conn = new MySqlConnection(connString))
                            //{
                            //    await conn.OpenAsync();
                            //    using (var cmd = new MySqlCommand("SELECT nickname from users where userid=@userid;", conn))
                            //    {
                            //        cmd.Parameters.AddWithValue("userid", random.Next(0, 10000000));
                            //        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                            //            while (await reader.ReadAsync())
                            //            {
                            //                for (var i = 0; i < reader.FieldCount; i++)
                            //                {
                            //                    var val = reader.GetValue(i);
                            //                    if (val == DBNull.Value)
                            //                        Console.WriteLine("结果：空");
                            //                    else
                            //                        Console.WriteLine($"结果:{val.ToString()}");
                            //                }
                            //            }
                            //    }
                            //}
                            #endregion

                            #region SqlSugarScope Singleton
                            //var param = new { userid = random.Next(0, 10000000) };
                            //var result = await singleDb.Ado.SqlQuerySingleAsync<string>("SELECT nickname from users where userid=@userid;", param);
                            //Console.WriteLine($"结果:{result}");
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"#########################异常：{ex.Message}#############################");
                        }
                        finally
                        {
                            Interlocked.Increment(ref cnt);
                        }
                    }

                    sw.Stop();
                    Console.WriteLine($"执行次数：{cnt}");
                });
                thread.Start(i);
            }

            Console.ReadLine();
        }

        private static SqlSugarClient GetDb()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.MySql,
                ConnectionString = "server=;port=;user=;password=;database=;charset=utf8mb4;Old Guids=true;SslMode=None;MAX Pool Size=20;MIN Pool Size=0;Connect Timeout = 180;Allow User Variables=True;",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                //AopEvents = new AopEvents
                //{
                //    //OnLogExecuting = (sql, p) =>
                //    //{
                //    //    Console.WriteLine(sql);
                //    //    Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                //    //}
                //}
            });
            return db;
        }

        static SqlSugarScope singleDb = new SqlSugarScope(
           new ConnectionConfig()
           {
               ConfigId = 1,
               DbType = SqlSugar.DbType.MySql,
               ConnectionString = "server=;port=;user=;password=;database=;charset=utf8mb4;Old Guids=true;SslMode=None;MAX Pool Size=20;MIN Pool Size=0;Connect Timeout = 180;Allow User Variables=True;",
               InitKeyType = InitKeyType.Attribute,
               IsAutoCloseConnection = true,
               //AopEvents = new AopEvents()
               //{
               //    OnLogExecuting = (sql, p) => { Console.WriteLine(sql); }
               //}
           });
    }
}
