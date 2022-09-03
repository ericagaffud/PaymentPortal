using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAcqSample
{
    internal sealed class CPaymentAdapter
    {
        CPaymentResult _result = null;
        string lCVV;
        string lCardNumber;
        string lCardholderName;
        string lExpiryMMYYYY;
        string lCommandStr;
        string lAmount;

        public CPaymentAdapter()
        {
            _result = new CPaymentResult();
            lCVV = "";
            lCardNumber = "";
            lAmount = "";
            lCardholderName = "";
            lExpiryMMYYYY = "";
            lCommandStr = "";
        }
     
        public void ClearFields()
        {
            lCVV = "";
            lCardNumber = "";
            lAmount = "";
            lCardholderName = "";
            lExpiryMMYYYY = "";
            lCommandStr = "";
            _result.ClearResult();
        }

        #region BASIC_CHECK_AND_PARAMETERS
        // PARAMETERS SHOULD BE IN THE FORM OF KEYWORD=VALUE EXAMPLE: CARDNUMBER=1234567890123456
        public string PARAM_CARD = "CARDNUMBER";
        public string PARAM_AMOUNT = "AMOUNT";
        public string PARAM_EXPIRY_MMYYYY = "EXPIRY";  // expiry should be mmyyyy (mm = month, yyyy = 4 digit year)
        public string PARAM_CVV = "CVV";
        public string PARAM_CHNAME = "CARDHOLDER";
      
        private string GetParameter(string[] pParameters, string pParameterName)
        {
            try
            {
                string lParamValue = "";
                foreach (string lStrParam in pParameters)
                {
                    string[] lStrElements = lStrParam.Split('=');
                    if (lStrElements[0].ToUpper() == pParameterName)
                    {
                        if (lStrElements.Length == 2)
                            lParamValue = lStrElements[1];
                    }
                }
                return lParamValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool PerformBasicCheck(string[] pParameters)
        {
            try
            {
                bool bIsValid = true;
                // check card is all digits, 10 digits length at least.
                // check expiry is future.
                // check all fields are present.
                #region CHECK_CARD_NUMBER
                lCardNumber = GetParameter(pParameters, PARAM_CARD);
                foreach (char c in lCardNumber)
                {
                    if (c < '0' || c > '9')
                    {
                        bIsValid = false;
                        _result._errordescription = "Card number contains invalid characters. Expected numeric only.";
                        _result._returncode = "99";
                        break;
                    }
                }

                if ((lCardNumber.Length > 19) || (lCardNumber.Length < 10))
                {
                    bIsValid = false;
                    _result._errordescription = "Card number is too short or long.";
                    _result._returncode = "99";
                }
                #endregion

                #region CHECK_EXPIRY
                if (bIsValid)
                {
                     lExpiryMMYYYY = GetParameter(pParameters, PARAM_EXPIRY_MMYYYY);
                    foreach (char c in lExpiryMMYYYY)
                        if (c < '0' || c > '9')
                        {
                            bIsValid = false;
                            _result._errordescription = "Expiry contains invalid characters. Expected numeric only.";
                            _result._returncode = "99";
                            break;
                        }

                    if (lExpiryMMYYYY.Length != 6)
                    {
                        bIsValid = false;
                        _result._errordescription = "Expiry is invalid format. Expected MMYYYY format.";
                        _result._returncode = "99";
                    }
                    else
                    {
                        if (bIsValid)
                        {
                            string lMonth = lExpiryMMYYYY.Substring(0, 2);
                            string lYear = lExpiryMMYYYY.Substring(2);
                            int iMonth = System.Convert.ToInt32(lMonth);
                            int iYear = System.Convert.ToInt32(lYear);
                            DateTime lExpiryDate = new DateTime(iYear, iMonth + 1, 1);
                            DateTime lNow = System.DateTime.Now;

                            if (DateTime.Compare(lNow, lExpiryDate) >= 0)
                            {
                                bIsValid = false;
                                _result._errordescription = "Expiry is invalid. Card is already expired.";
                                _result._returncode = "99";
                            }
                        }
                    }
                }
                #endregion

                #region CHECK_CHNAME
                lCardholderName = GetParameter(pParameters, PARAM_CHNAME);
                if (bIsValid)
                {
                    if (lCardholderName == "")
                    {
                        bIsValid = false;
                        _result._returncode = "99";
                        _result._errordescription = "Cardholder name is empty.";
                    }
                }
                #endregion

                #region CHECK_CVV2
                if (bIsValid)
                {
                    lCVV = GetParameter(pParameters, PARAM_CVV);
                    foreach (char c in lCVV)
                    {
                        if (c < '0' || c > '9')
                        {
                            bIsValid = false;
                            _result._errordescription = "CVV contains invalid characters. Expected numeric only.";
                            _result._returncode = "99";
                            break;
                        }
                    }

                    if (bIsValid)
                    {
                        if (lCVV.Length != 3)
                        {
                            bIsValid = false;
                            _result._errordescription = "CVV expected to be 3 digits.";
                            _result._returncode = "99";
                        }
                    }
                }
                #endregion

                #region CHECK_AMOUNT
                if (bIsValid)
                {
                    lAmount = GetParameter(pParameters, PARAM_AMOUNT);
                    decimal dcmTryParse = 0.0m;
                    if (decimal.TryParse(lAmount, out dcmTryParse))
                    {
                        lAmount = (dcmTryParse * 100).ToString();
                        lAmount = lAmount.Substring(0, lAmount.Length - 2);
                    }
                    else
                    {
                        bIsValid = false;
                        _result._errordescription = "Unable to parse transaction amount.";
                        _result._returncode = "99";
                    }
                }
                #endregion


                return bIsValid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region PAYMENT_PROCESSING
        private void FormCommand()
        {
            try
            {
                lCommandStr = "";
                int iNbrFields = 6;

                for (int i = 0; i < iNbrFields; i++)
                {
                    lCommandStr += "FIELD=";
                    switch (i)
                    {
                        case 0:
                            lCommandStr += "2&VALUE=";
                            lCommandStr += lCardNumber;
                            break;
                        case 1:
                            lCommandStr += "14&VALUE=";
                            lCommandStr += lExpiryMMYYYY.Substring(0, 2);
                            lCommandStr += lExpiryMMYYYY.Substring(4);
                            break;
                        case 3:
                            lCommandStr += "48&VALUE=";
                            lCommandStr += lCVV;
                            break;
                    }
                    lCommandStr += ";";
                }

                lCommandStr = lCommandStr.Substring(0, lCommandStr.Length - 1);
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
        }
 

        private void ProcessPayment(string[] pParameters)
        {
            try
            {
                // form command string
                FormCommand();

                // payment gateway
                #region PAYMENT_GATEWAY
                if (lCommandStr == "FIELD=2&VALUE=6350123412341234;FIELD=14&VALUE=1221;FIELD=48&VALUE=789;")
                {
                    _result._returncode = "00";
                    _result._transactionid = "A877648";
                    _result._trandatetime = System.DateTime.Now.ToString("MMddyyyyHHmmss"); ;
                }
                else if (lCommandStr == "FIELD=2&VALUE=3566002020360505;FIELD=14&VALUE=1022;FIELD=48&VALUE=456;")
                {
                    _result._returncode = "89";
                    _result._errordescription = "Transaction not approved by bank.";
                    _result._trandatetime = System.DateTime.Now.ToString("MMddyyyyHHmmss"); ;
                }
                else
                {
                    _result._returncode = "99";
                    _result._errordescription = "Connection Timeout.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

       

        public CPaymentResult ProcessCardPayment(string[] pParameters)
        {
            try
            {
                _result.ClearResult();
                if (PerformBasicCheck(pParameters))
                {
                            ProcessPayment(pParameters);
                }                
                return _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}