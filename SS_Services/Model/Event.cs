﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventHub.Model
{
    public enum EventStatus
    {
        open,
        closed,
        full,
        expired,
        cancelled
    }
    public abstract class Event
    {
        int id;
        string name;
        //Dictionary<Team, DateTime> teams;
        DateTime startDate;
        DateTime endDate;
        string local;
        string description;
        int slots;
        EventStatus status;
        int sportId;
        int teamMax;

        #region PROPERTIES
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; } // Check for only alpahbet characters
        public DateTime StartDate { get => startDate; set => startDate = value; } // check if date > present
        public DateTime EndDate { get => endDate; set => endDate = value; } // check if endDate > startDate + date > present
        public string Local { get => local; set => local = value; } 
        public string Description { get => description; set => description = value; }
        public int Slots { get => slots; set => slots = value; } // only values > 0
        public EventStatus Status { get => status; set => status = value; }
        public int SportId { get => sportId; set => sportId = value; }
        public int TeamMax { get => teamMax; set => teamMax = value; }
        #endregion

        protected Event()
        {
            
        }

        public bool ValidateObject()
        {
            if (1 == 1) return true;
            //return false;
        }


    }
}
