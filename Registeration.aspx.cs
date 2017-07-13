using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Registeration : System.Web.UI.Page
{


    private string salt;
    private string finalhash;
    string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
    private byte[] Key;
    private byte[] IV;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {

        // getting user input
        string email = TextBox1.Text.Trim();
        string password = TextBox2.Text.Trim();
        string name = TextBox4.Text.Trim();


        //gen salt 

        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] saltByte = new byte[8];
        //fill array of bytes
        rng.GetBytes(saltByte);
        salt = Convert.ToBase64String(saltByte);

        //hashing
        SHA512Managed hashing = new SHA512Managed();
        string pwdWithsalt = password + salt;
        byte[] hashwithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithsalt));
        finalhash = Convert.ToBase64String(hashwithSalt);


        //key generation
        RijndaelManaged cipher = new RijndaelManaged();
        Key = cipher.Key;
        IV = cipher.IV;


        // saving the data to the database

        try
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString)) // parameterized queries handles sql injections!!
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO fernando VALUES(@email,@name,@passwordhash,@passwordsalt,@date,@SecretKey,@Iv,@att)"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@name", encryptData(name));
                        cmd.Parameters.AddWithValue("@passwordhash", finalhash);
                        cmd.Parameters.AddWithValue("@passwordsalt", salt);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@SecretKey", Convert.ToBase64String(Key));
                        cmd.Parameters.AddWithValue("@Iv", Convert.ToBase64String(IV));
                        cmd.Parameters.AddWithValue("@att",0);

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
    protected string encryptData(string data)
    {
        string finalcipherText = null;

        try
        {
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.Key = Key;
            cipher.IV = IV;

            //transform
            ICryptoTransform enCryptoTransform = cipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(data);
            byte[] cipherText = enCryptoTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            finalcipherText = Convert.ToBase64String(cipherText);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        return finalcipherText;
    }
}