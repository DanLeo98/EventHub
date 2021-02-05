using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Account
{
    int accountId;
    string address;
    string shippingAddress;
    string bankId;

    #region PROPERTIES
    public string Address { get => address; set => address = value; }
    public string ShippingAddress { get => shippingAddress; set => shippingAddress = value; }
    public string BankId { get => bankId; set => bankId = value; }
    public int AccountId { get => accountId; set => accountId = value; }
    #endregion

    public Account()
    {

    }
}
