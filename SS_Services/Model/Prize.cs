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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Prize
{
    int id;
    int amount;
    Place position;

    #region PROPERTIES
    public int ID { get => id; set => id = value; }
    [JsonProperty("value")]
    public int Value { get => amount; set => amount = value; }
    public Place Position { get => position; set => position = value; }
    #endregion

    public Prize()
    {
        position = Place.na; // initialization of place as none, when created
    }
}

