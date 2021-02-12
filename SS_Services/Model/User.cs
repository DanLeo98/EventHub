using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;


public class User
{
    int id, accountId;
    string name;
    string email;
    string password;

    #region PROPERTIES

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Email { get => email; set => email = value; }
    public string Password { get => password; set => password = value; }
    public int AccountId { get => accountId; set => accountId = value; }
    #endregion


    public User()
    {
    }

    public User(string name, string email, string password)
    {
    }

    #region FUNCTIONS
    public bool CheckCredentials()
    {
        return true;
    }

    public bool ValidateObject()
    {
        if (1 == 1) return true;
        //return false;
    }

    /// <summary>
    /// Validate user exists
    /// </summary>
    /// <param name="connString"> Connection string </param>
    /// <param name="id"> Id of user for output </param>
    /// <returns></returns>
    public int ValidateUser(string connString, out int id)
    {
        try
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Show users with given credentials
                string query = "SELECT * FROM \"user\" WHERE email = @email AND password = @pass;";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                NpgsqlParameter p_email = new NpgsqlParameter("@email", this.Email);
                cmd.Parameters.Add(p_email);

                NpgsqlParameter p_pass = new NpgsqlParameter("@pass", this.Password);
                cmd.Parameters.Add(p_pass);

                NpgsqlDataReader reader = cmd.ExecuteReader();
                int rowsAff = 0;
                reader.Read();
                id = reader.GetInt32(0);
                while (reader.IsOnRow)
                {
                    rowsAff++;
                    reader.Read();
                }

                conn.Close();

                if (rowsAff == 1) return 0;
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            id = 0;
            return 2;
        }
    }
    
    public int Register(string connString)
    {
        // Connects to DB
        using (NpgsqlConnection conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            // Creates account for user
            string query0 = "INSERT INTO account(bankId)" +
                "VALUES(NULL);";
            NpgsqlCommand cmd0 = new NpgsqlCommand(query0, conn);
            cmd0.ExecuteNonQuery();

            // Gets user account id
            string query1 = "SELECT id from account;";
            NpgsqlCommand cmd1 = new NpgsqlCommand(query1, conn);
            var reader = cmd1.ExecuteReader();

            reader.Read();
            int id = 0;
            while (reader.IsOnRow)
            {
                id = reader.GetInt32(0);
                reader.Read();
            }
            reader.Close();

            // Registers user
            string query = "INSERT INTO \"user\"(name,email,password,accountid)" +
                "VALUES(@name,@email,@pass,@acc);";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            NpgsqlParameter p_name = new NpgsqlParameter("@name", this.Name);
            cmd.Parameters.Add(p_name);

            NpgsqlParameter p_email = new NpgsqlParameter("@email", this.Email);
            cmd.Parameters.Add(p_email);

            NpgsqlParameter p_pass = new NpgsqlParameter("@pass", this.Password);
            cmd.Parameters.Add(p_pass);

            NpgsqlParameter p_acc = new NpgsqlParameter("@acc", id);
            cmd.Parameters.Add(p_acc);

            int rowsAff = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAff == 1) return 0;
            return 1;
        }
    }
    #endregion


}

