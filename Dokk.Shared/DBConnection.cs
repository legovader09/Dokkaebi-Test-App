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
        public int statusCode = 0;
        public bool isConnected = false;

        public DBConnection() => connectToDB();

        private void connectToDB()
        {
            if (isConnected)
                return;

            try
            {
                dbbuild.Server = "remotemysql.com";
                dbbuild.UserID = Encoding.ASCII.GetString(new byte[] { 114, 78, 76, 68, 120, 81, 76, 102, 85, 67 });
                dbbuild.Password = Encoding.ASCII.GetString(new byte[] { 101, 107, 87, 113, 75, 80, 86, 111, 82, 65 });
                dbbuild.Database = Encoding.ASCII.GetString(new byte[] { 114, 78, 76, 68, 120, 81, 76, 102, 85, 67 });

                dbconn.ConnectionString = dbbuild.ToString();
                dbconn.Open();
                isConnected = true;
            }
            catch
            {
                isConnected = false;
                connectToDB();
            }
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
                Random r = new Random();
                for (int i = 0; i < 6; i++)
                {
                    sessionCode += r.Next(10);
                }

                uploadCode(sessionCode);

                return true;
            }
            catch 
            {
                isConnected = false;
                connectToDB();
                uploadCode(sessionCode);
                return false; 
            }
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

        public void setRingingStatus(int status)
        {
            try
            {
                dbcomm = dbconn.CreateCommand();
                dbcomm.CommandText = String.Format($"UPDATE `Dokk` SET StatusCode='{status}' WHERE Code='{sessionCode}'");
                dbcomm.ExecuteNonQuery();
                statusCode = status;
            }
            catch
            {
                connectToDB();
                setRingingStatus(status);
            }
        }

        public int checkStatus()
        {
            try
            {
                connectToDB();

                query = String.Format("SELECT * FROM Dokk WHERE Code='{0}'", sessionCode);
                dbcomm = new MySqlCommand(query, dbconn);
                dbread = dbcomm.ExecuteReader();

                while (dbread.Read())
                {
                    statusCode = dbread.GetInt32("StatusCode");
                }

                dbread.Close();

                return statusCode;
            }
            catch
            {
                return 0;
            }
        }

        #region "Disposal"
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbcomm = dbconn.CreateCommand();
                    dbcomm.CommandText = $"DELETE FROM `Dokk` WHERE `Code`='{sessionCode}'";
                    var r = dbcomm.ExecuteNonQuery();
                    if (r != 0)
                        MessageBox.Show("Successfully Disconnected");
                    
                    dbconn.Close();
                    isConnected = false;
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
        #endregion
    }
}