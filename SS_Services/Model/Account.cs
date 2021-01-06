using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Model
{
    public class Account
    {
        int acccountId;
        string address;
        string shippingAddress;
        string bankId;

        #region PROPERTIES
        public string Address { get => address; set => address = value; }
        public string ShippingAddress { get => shippingAddress; set => address = value; }
        public string BankId { get => bankId; set => bankId = value; }
        public int AccountId { get => AccountId; set => AccountId = value; }
        #endregion

        public Account()
        {

        }
    }
}
