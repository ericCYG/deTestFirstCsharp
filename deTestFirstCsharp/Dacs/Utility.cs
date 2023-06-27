using Microsoft.Data.SqlClient;
using System.Data;
using static System.Formats.Asn1.AsnWriter;

namespace deTestFirstCsharp.Dacs
{
    public class Utility
    {
        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\mssqllocaldb;Initial Catalog=GSSWEB;Integrated Security=True");

        }
        public DataTable SqlDataReader(string sql, Dictionary<string, object> keyValues)
        {
            DataTable schemaTable = new DataTable();

            using (SqlConnection sqlConnection = GetSqlConnection()) { 

                SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                sqlConnection.Open();
                SqlTransaction transactionMan = sqlConnection.BeginTransaction();
                cmd.Transaction = transactionMan;
                SqlDataReader sr = null;

                try
                {
                    if (keyValues.Count != 0)
                    {
                        foreach (var item in keyValues)
                        {
                            cmd.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }



                    sr = cmd.ExecuteReader();
                    schemaTable.Load(sr);
                    sr.Close();


                    transactionMan.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        sr.Close();
                        transactionMan.Rollback();
                    }
                    catch (Exception eex)
                    {
                       
                        Console.WriteLine(sql + "。(前面為sql) exeReader Rollback 失敗。(錯誤訊息) " + eex.Message);
                        return null;
                    }

                    Console.WriteLine(sql + "。(前面為sql) ExecuteReader 失敗。(錯誤訊息) " + ex.Message);
                    return null;
                }
                finally
                {
                    sr.Close();
                    sqlConnection.Close();
                }


            }

            return schemaTable;
        }
        public string SqlDataScalar(string sql, Dictionary<string, object> keyValues)
        {
        
            string ifSuccess = "-1";

            using (SqlConnection con = GetSqlConnection())
            {

                SqlCommand scomm = new SqlCommand(sql);
                scomm.Connection = con;
                con.Open();

                SqlTransaction transactionMan = con.BeginTransaction();
                scomm.Transaction = transactionMan;
                try
                {
                    if (keyValues.Count != 0)
                    {
                        foreach (var item in keyValues)
                        {
                            scomm.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    var ifSuccessVar = scomm.ExecuteScalar();

                    if (ifSuccessVar == null) { ifSuccess = "0"; } else { ifSuccess = ifSuccessVar.ToString(); }

                    transactionMan.Commit();
                }
                catch (Exception ex)
                {
                    transactionMan.Rollback();
                    
                    Console.WriteLine(sql + "。(前面為sql) exeScalar 失敗。(錯誤訊息) " + ex.Message);
                }
                finally
                {
                    con.Close();
                }

            }
            return ifSuccess;
        }
        public string SqlDataNonQuery(string sql, Dictionary<string, object> keyValues)
        {

            int ifSuccess = -1;
          

            using (SqlConnection con = GetSqlConnection())
            {

                SqlCommand scomm = new SqlCommand(sql,con);
             
                con.Open();
                SqlTransaction transactionMan = con.BeginTransaction();
                scomm.Transaction = transactionMan;

                try
                {
                    if (keyValues.Count > 0)
                    {
                        foreach (var item in keyValues)
                        {
                            scomm.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }


                    ifSuccess = scomm.ExecuteNonQuery();
                    transactionMan.Commit();
                }
                catch (Exception ex)
                {
                    transactionMan.Rollback();

                    
                    Console.WriteLine(sql + "。(前面為sql) exeNonQuery 失敗。(錯誤訊息) " + ex.Message);
                }
                finally
                {
                    con.Close();
                }

            }
            return ifSuccess.ToString();

           
        }

    }
}
