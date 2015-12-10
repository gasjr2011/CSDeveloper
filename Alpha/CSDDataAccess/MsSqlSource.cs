using System;
using System.Collections.Generic;
using CSDeveloper.DataAccess;
using System.Data;
using System.Data.SqlClient;

namespace CSDeveloper.DataAccess
{
    public class MsSqlSource : IDataSource
    {
        string conString = string.Empty;

        public MsSqlSource(string connectionString, bool testConnection = true)
        {
            conString = connectionString;
            if (testConnection)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();
                        if (con.State != ConnectionState.Open)
                            throw new Exception("Error connecting to specified server.");
                        con.Close();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    throw e;
                }
            }
        }

        public bool RunAsTransaction<T>(List<DataSourceCommand<T>> commands)
        {
            bool ret = false;
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        try
                        {
                            using (SqlTransaction trn = con.BeginTransaction())
                            {
                                using (SqlCommand com = con.CreateCommand())
                                {
                                    com.Transaction = trn;
                                    try
                                    {
                                        foreach (DataSourceCommand<T> cmd in commands)
                                        {
                                            com.CommandText = cmd.Command.Trim();
                                            com.Parameters.Clear();

                                            foreach (KeyValuePair<string, object> para in cmd.Parameters)
                                                com.Parameters.AddWithValue(para.Key, para.Value);

                                            switch (cmd.Type)
                                            {
                                                case DataSourceCommandType.Query:
                                                    if (cmd.Transformer == null)
                                                        cmd.Result = this.TranformFromReader(com.ExecuteReader());
                                                    else
                                                        cmd.Result = this.TranformFromReader<T>(com.ExecuteReader(), cmd.Transformer);
                                                    break;
                                                case DataSourceCommandType.Scalar:
                                                    cmd.Result = com.ExecuteScalar();
                                                    break;
                                                case DataSourceCommandType.Execute:
                                                    cmd.Result = com.ExecuteNonQuery();
                                                    break;
                                            }
                                        }
                                        trn.Commit();
                                    }
                                    catch
                                    {
                                        trn.Rollback();
                                        throw;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return ret;
        }

        private List<object> TranformFromReader(IDataReader reader)
        {
            List<object> ret = new List<object>();
            object value = default(object);
            List<object> rec = default(List<object>);

            try
            {
                while (reader.Read())
                {
                    rec = new List<object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        value = reader[i];
                        rec.Add(value);
                    }
                    ret.Add(rec);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
            }

            return ret;
        }

        private List<T> TranformFromReader<T>(IDataReader reader, TransformCurrentRecord<T> func = null)
        {
            List<T> ret = new List<T>();
            T value = default(T);

            try
            {
                while (reader.Read())
                {
                    value = func(reader);
                    ret.Add(value);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
            }

            return ret;
        }

        public IEnumerable<T> RunQuery<T>(DataSourceCommand<T> command) where T : new()
        {
            List<T> ret = new List<T>();

            if (command.Transformer == null) return ret;
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        try
                        {
                            using (SqlCommand com = con.CreateCommand())
                            {
                                com.CommandText = command.Command.Trim();
                                com.Parameters.Clear();

                                foreach (KeyValuePair<string, object> para in command.Parameters)
                                    com.Parameters.AddWithValue(para.Key, para.Value);

                                ret = this.TranformFromReader<T>(com.ExecuteReader(), command.Transformer);
                                command.Result = ret;
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            con.Close();
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw e;
            }
            return ret;
        }

        public T RunScalar<T>(DataSourceCommand<T> command)
        {
            T ret = default(T);

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        try
                        {
                            using (SqlCommand com = con.CreateCommand())
                            {
                                com.CommandText = command.Command.Trim();
                                com.Parameters.Clear();

                                foreach (KeyValuePair<string, object> para in command.Parameters)
                                    com.Parameters.AddWithValue(para.Key, para.Value);

                                ret = (T) com.ExecuteScalar();
                                command.Result = ret;
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            con.Close();
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw e;
            }
            return ret;
        }

        public long RunCommand<T>(DataSourceCommand<T> command)
        {
            long ret = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        try
                        {
                            using (SqlCommand com = con.CreateCommand())
                            {
                                com.CommandText = command.Command.Trim();
                                com.Parameters.Clear();

                                foreach (KeyValuePair<string, object> para in command.Parameters)
                                    com.Parameters.AddWithValue(para.Key, para.Value);

                                ret = com.ExecuteNonQuery();
                                command.Result = ret;
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw e;
            }
            return ret;
        }
    }
}
