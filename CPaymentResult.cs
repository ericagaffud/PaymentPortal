using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAcqSample
{
    internal class CPaymentResult
    {
        public string _transactionid;
        public string _trandatetime;
        public string _returncode;
        public string _errordescription;

        public CPaymentResult()
        {
            _transactionid = "";
            _trandatetime = "";
            _returncode = "";
            _errordescription = "";
        }

        public void ClearResult()
        {
            _transactionid = "";
            _trandatetime = "";
            _returncode = "";
            _errordescription = "";
        }

        public bool IsSuccessful 
        {
            get
            {
                    if (_returncode == "00")
                        return true;
                    return false;
            }
        }

    }
}