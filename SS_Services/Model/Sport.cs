/*
 * Authors: João Rodrigues and Daniel Leonard
 * Project: Practical Work, implementing services
 * Current Solution: Client of services for sport events
 * 
 * 
 * Subject: Integration of Informatic Systems
 * Degree: Graduation on Engeneer of Informatic Systems
 * Lective Year: 2020/21
 */

using Npgsql;
using System.Collections.Generic;

public class Sport
{
    int id;
    string name;

    #region PROPERTIES
    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    #endregion

    public Sport()
    {

    }
    public static List<Sport> GetSports(string connString)
    {
        List<Sport> sports = new List<Sport>();
        using (NpgsqlConnection conn = new NpgsqlConnection((connString)))
        {
            conn.Open();

            string query = "SELECT * FROM sport;";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Sport sport = new Sport();
                sport.Id = reader.GetInt32(0);
                sport.Name = reader.GetString(1);
                sports.Add(sport);
            }
            conn.Close();
        }
        return sports;
    }
}

