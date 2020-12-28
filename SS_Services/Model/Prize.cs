using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
    public class Prize
    {
        int amount;
        Place position;

        #region PROPERTIES
        public int Value { get => amount; set=> amount = value; }
        public Place Position { get => position; set => position = value; }
        #endregion

        public Prize()
        {
            position = Place.na; // initialization of place as none, when created
        }
    }
}
