using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Windows.Forms;

namespace Dokk.Shared
{
    public class DBConnection : IDisposable
    {
        private MySqlConnectionStringBuilder dbbuild = new MySqlConnectionStringBuilder();
        private MySqlConnection dbconn = new MySqlConnection();
        private string query;
        private MySqlCommand dbcomm = new MySqlCommand();
        private MySqlDataReader dbread;

        public string sessionCode;

        public DBConnection()
        {
            dbbuild.Server = "remotemysql.com";
            dbbuild.UserID = Encoding.ASCII.GetString(new byte[] { 114, 78, 76, 68, 120, 81, 76, 102, 85, 67 });
            dbbuild.Password = Encoding.ASCII.GetString(new byte[] { 101, 107, 87, 113, 75, 80, 86, 111, 82, 65 });
            dbbuild.Database = Encoding.ASCII.GetString(new byte[] { 114, 78, 76, 68, 120, 81, 76, 102, 85, 67 });

            dbconn.ConnectionString = dbbuild.ToString();
            dbconn.Open();
        }

        public bool endHostSession()
        {
            try
            {
                Dispose();
                return true;
            }
            catch { return false; }
        }

        public bool createHostSession()
        {
            try
            {
                sessionCode = "";
                for (int i = 0; i < 6; i++)
                {
                    sessionCode += new Random().Next(i, 9).ToString();
                }

                uploadCode(sessionCode);

                return true;
            }
            catch { return false; }
        }

        private void uploadCode(string code)
        {
            try
            {
                dbcomm.Parameters.Clear();
                dbcomm = dbconn.CreateCommand();
                dbcomm.CommandText = "INSERT INTO `Dokk` (`Code`, `UsedBy`, `StatusCode`, `lastUsed`) VALUES (@c,@u,@s,@l)";
                dbcomm.Parameters.AddWithValue("@c", code);
                dbcomm.Parameters.AddWithValue("@u", "null");
                dbcomm.Parameters.AddWithValue("@s", 0);
                dbcomm.Parameters.AddWithValue("@l", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                dbcomm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbconn.Close();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }
}