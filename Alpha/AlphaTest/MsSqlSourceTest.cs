using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSDeveloper.DataAccess;

namespace AlphaTest
{

    public class Alpha
    {
        public string Strings;
        public DateTime Datetime;
        public double Number;

        public static Alpha ReaderTransformer(IDataReader reader)
        {
            Alpha ret = new Alpha();

            ret.Strings = reader["strings"].ToString();
            ret.Datetime = Convert.ToDateTime(reader["datetime"]);
            ret.Number = Convert.ToDouble(reader["number"]);

            return ret;
        }
    }

    [TestClass]
    public class MsSqlSourceTest
    {
        const string CON_STRING = @"Data Source=CSDW10DADDY\SQLEXPRESS;Initial Catalog=CSDSandbox;Integrated Security=True";

        [TestMethod]
        public void RunQueryTest()
        {
            try
            {
                DataSourceCommand<Alpha> comm = new DataSourceCommand<Alpha>() { Command = "select * from alpha", Transformer = Alpha.ReaderTransformer };

                MsSqlSource con = new MsSqlSource(CON_STRING);
                con.RunQuery<Alpha>(comm);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void MultipleTestTransformer()
        {
            try
            {
                List<DataSourceCommand<Alpha>> states = new List<DataSourceCommand<Alpha>>();
                DataSourceCommand<Alpha> comm = new DataSourceCommand<Alpha>()
                {
                    Command = "insert alpha(strings, datetime, number) values (@strings, @datetime, @number);",
                    Type = DataSourceCommandType.Execute
                };
                comm.Parameters.Add("@strings", "abcde");
                comm.Parameters.Add("@datetime", DateTime.Now);
                comm.Parameters.Add("@number", 99);

                states.Add(comm);
                comm = new DataSourceCommand<Alpha>() { Command = "select * from alpha", Transformer = Alpha.ReaderTransformer };
                states.Add(comm);
                MsSqlSource con = new MsSqlSource(CON_STRING);
                con.RunAsTransaction<Alpha>(states);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

        }


        [TestMethod]
        public void MultipleTest()
        {
            try {
                List<DataSourceCommand<object>> states = new List<DataSourceCommand<object>>();
                DataSourceCommand<object> comm = new DataSourceCommand<object>()
                {
                    Command = "insert alpha(strings, datetime, number) values (@strings, @datetime, @number);",
                    Type = DataSourceCommandType.Execute
                };
                comm.Parameters.Add("@strings", "abcde");
                comm.Parameters.Add("@datetime", DateTime.Now);
                comm.Parameters.Add("@number", 99);

                states.Add(comm);
                states.Add(new DataSourceCommand<object>() { Command = "select * from alpha" });
                MsSqlSource con = new MsSqlSource(CON_STRING);

                con.RunAsTransaction<object>(states);
                Console.WriteLine(states);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void AutoTest()
        {
            try {
                MsSqlSource con = new MsSqlSource(CON_STRING);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Fail(); 
            }

            try
            {
                MsSqlSource con = new MsSqlSource(CON_STRING  + ";wtf=blah");
                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}
