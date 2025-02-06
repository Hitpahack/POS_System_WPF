using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace FalcaPOS.Common.Helper
{
    public static class StringHelper
    {
        private static string _gstPattern = "^[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Z]{1}[0-9a-zA-Z]{1}$";//"^([0-9]{2}[a-zA-Z]{4}([a-zA-Z]{1}|[0-9]{1})[0-9]{4}[a-zA-Z]{1}([a-zA-Z]|[0-9]){3}){0,15}$";//"^[0 - 9]{ 2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0 - 9A - Z]{1}$";

        private static string _phoneNumberPattern = "[6-9][0-9]{9}";
        public static bool IsValidString(this string arg)
        {
            return !string.IsNullOrEmpty(arg) && !string.IsNullOrWhiteSpace(arg);

            //if(string.IsNullOrEmpty(arg) || string.IsNullOrWhiteSpace(arg))
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }
        public static bool IsDigitsOnly(this string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }

        public static bool isIfscCodeValid(this String IFSCCode)
        {
            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("^[A-Za-z]{4}0[A-Z0-9a-z]{6}$");
            return regx.Matches(IFSCCode).Count > 0 ? regx.Matches(IFSCCode)[0].Success : false;
        }
        public static bool IsValidGST(this string gst)
        {
            if (gst == null) return false;

            Match _match = Regex.Match(gst, _gstPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));

            return _match.Success;


        }

        public static bool IsValidPhone(this string phone)
        {
            if (phone == null) return false;
            Match _match = Regex.Match(phone, _phoneNumberPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(5000));

            return _match.Success;
        }

        public static Dictionary<int, string> AllMonth()
        {
            Dictionary<int, string> AllMonth = new Dictionary<int, string>();
            AllMonth.Add(1, "January");
            AllMonth.Add(2, "February");
            AllMonth.Add(3, "March");
            AllMonth.Add(4, "April");
            AllMonth.Add(5, "May");
            AllMonth.Add(6, "June");
            AllMonth.Add(7, "July");
            AllMonth.Add(8, "August");
            AllMonth.Add(9, "September");
            AllMonth.Add(10, "October");
            AllMonth.Add(11, "November");
            AllMonth.Add(12, "December");
            return AllMonth;
        }

        public static string GetReportFileName(string fileName)
        {
            return DateTime.Now.ToShortDateString().Replace(" ", "") + Guid.NewGuid().ToString().Substring(0, 3) + fileName ?? string.Empty;
        }


        public static bool IsValidEmail(this string email)
        {
            return email.IsValidString() && new EmailAddressAttribute().IsValid(email);
        }
    }
}
