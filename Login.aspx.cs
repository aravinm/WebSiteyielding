using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{

    private byte[] Key;
    private byte[] IV;
    private string errorMsg = "";
    string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
    static string finalHash;
    static string salt;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected string decryptData(string data)
    {

        string finalPlainText = null;

        try
        {
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.Key = Key;
            cipher.IV = IV;

            //transform
            ICryptoTransform deCryptoTransform = cipher.CreateDecryptor();
            byte[] cipherText = Convert.FromBase64String(data);
            byte[] plainText = deCryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            finalPlainText = Encoding.UTF8.GetString(plainText);


        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        return finalPlainText;
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string pwd = TextBox2.Text.ToString().Trim();
        string userid = TextBox1.Text.ToString().Trim();
        SHA512Managed hashing = new SHA512Managed();
        string dbHash = getDBHash(userid);
        string dbSalt = getDBSalt(userid);
        lblError.Text = "";
        lblName.Text = "";

        int tries = getDBattempts(userid);




        if (tries < 3)
        {
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {

                        lblName.Text = " Welcome, " + getName(userid);

                    }
                    else
                    {
                        plusOneToAttempt(userid);
                        errorMsg = "Userid or password is not valid. Please try again.";
                        lblError.Text = errorMsg;

                    }
                }
                else
                {
                    plusOneToAttempt(userid);
                    errorMsg = "Userid or password is not valid. Please try again.";
                    lblError.Text = errorMsg;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        else
        {
            errorMsg = "You have excced 3 tries";
            lblError.Text = errorMsg;
        }


     
    }
    protected string getDBHash(string userid)
    {
        string h = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select PasswordHash FROM fernando WHERE Email=@USERID";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@USERID", userid);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["PasswordHash"] != null)
                    {
                        if (reader["PasswordHash"] != DBNull.Value)
                        {
                            h = reader["PasswordHash"].ToString();
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
    protected string getDBSalt(string userid)
    {
        string s = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select PASSWORDSALT FROM fernando WHERE Email=@USERID";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@USERID", userid);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["PASSWORDSALT"] != null)
                    {
                        if (reader["PASSWORDSALT"] != DBNull.Value)
                        {
                            s = reader["PASSWORDSALT"].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return s;
    }


    protected void plusOneToAttempt(string user)
    {

        //current attempot
        int curr = getDBattempts(user);
        int newVal = curr + 1;



        try
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString)) // parameterized queries handles sql injections!!
            {
                using (SqlCommand cmd = new SqlCommand("update fernando set attempts=@att where Email=@emm"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {

                        cmd.Parameters.AddWithValue("@emm", user);
                        cmd.Parameters.AddWithValue("@att", newVal);
                      

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }



        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }

    }
    protected string getName(string userid)
    {
        string h = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select Name FROM fernando WHERE Email=@USERID";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@USERID", userid);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["Name"] != null)
                    {
                        if (reader["Name"] != DBNull.Value)
                        {
                            h = reader["Name"].ToString();
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
    protected string getSecretKey(string name)
    {
        string h = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select SecretKey FROM fernando WHERE Email = @abc";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@abc", name);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["SecretKey"] != null)
                    {
                        if (reader["SecretKey"] != DBNull.Value)
                        {
                            h = reader["SecretKey"].ToString();
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
    protected string getIV(string name)
    {
        string h = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select Iv FROM fernando WHERE Email = @abc";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@abc", name);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["Iv"] != null)
                    {
                        if (reader["Iv"] != DBNull.Value)
                        {
                            h = reader["Iv"].ToString();
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
    protected string getDBSName(string name)
    {
        string h = null;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select Name FROM fernando WHERE Email = @email";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@email", name);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["Name"] != null)
                    {
                        if (reader["Name"] != DBNull.Value)
                        {
                            h = reader["Name"].ToString();
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
    protected int getDBattempts(string name)
    {
        int h = 0;
        SqlConnection connection = new SqlConnection(MYDBConnectionString);
        string sql = "select attempts FROM fernando WHERE Email = @email";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@email", name);
        try
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (reader["attempts"] != null)
                    {
                        if (reader["attempts"] != DBNull.Value)
                        {
                            h =int.Parse(reader["attempts"].ToString());
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally { connection.Close(); }
        return h;
    }
}