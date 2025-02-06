using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcaPOS.Reports.ViewModel
{
    public partial class ReportViewModel
    {
        public string GetLedgerCalculation(string Type, string LadgerType, int GST,string storetype)
        {

            try
            {
                var _Ledgers = new ParseJSON().ParseJson<List<Ledgers>>("Ledgers");
                if (_Ledgers != null && _Ledgers.Count > 0)
                {
                    var _gst = Convert.ToString(GST) == "Nil" ? "Nil" : Convert.ToString(GST) == "0" ? "Nil" : Convert.ToString(GST) + "%";
                    //purchase taxs

                    switch (LadgerType)
                    {
                        case AppConstants.PURCHASE_LEDGER:
                            var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().PurchaseLedger.Where(x => x.GSTRate == _gst && x.StoreTypes==storetype).FirstOrDefault();
                            if (_ledgerHead != null)
                                return _ledgerHead.LedgerHead;
                            break;
                        case AppConstants.PURCHASE_OTHER_STATE:
                            var _ledgerOther = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().PurchaseOtherState.Where(x => x.GSTRate == _gst && x.StoreTypes == storetype).FirstOrDefault();
                            if (_ledgerOther != null)
                                return _ledgerOther.LedgerHead;
                            break;
                        case AppConstants.LEDGER_CGST:
                            var _ledgerCGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LedgerCGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerCGST != null)
                                return _ledgerCGST.LedgerHead;
                            break;
                        case AppConstants.LEDGER_SGST:
                            var _ledgerSGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LedgerSGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerSGST != null)
                                return _ledgerSGST.LedgerHead;
                            break;
                        case AppConstants.LEDGER_IGST:
                            var _ledgerIGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LedgerIGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerIGST != null)
                                return _ledgerIGST.LedgerHead;
                            break;
                        case AppConstants.SALES_LEDGER:
                            var _ledgerSales = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().SalesLedger.Where(x => x.GSTRate == _gst && x.StoreTypes == storetype).FirstOrDefault();
                            if (_ledgerSales != null)
                                return _ledgerSales.LedgerHead;
                            break;
                        case AppConstants.SALES_OTHER_STATE:
                            var _ledgerSaleOther = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().SalesOtherState.Where(x => x.GSTRate == _gst && x.StoreTypes == storetype).FirstOrDefault();
                            if (_ledgerSaleOther != null)
                                return _ledgerSaleOther.LedgerHead;
                            break;
                        case AppConstants.LIABILITY_CGST:
                            var _ledgerLiabilityCGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LiabilityCGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerLiabilityCGST != null)
                                return _ledgerLiabilityCGST.LedgerHead;
                            break;
                        case AppConstants.LIABILITY_SGST:
                            var _ledgerLiabilitySGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LiabilitySGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerLiabilitySGST != null)
                                return _ledgerLiabilitySGST.LedgerHead;
                            break;
                        case AppConstants.LIABILITY_IGST:
                            var _ledgerLiabilityIGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LiabilityIGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerLiabilityIGST != null)
                                return _ledgerLiabilityIGST.LedgerHead;
                            break;
                        case AppConstants.SERVICES_LEDGER:
                            var _ledgerServices = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().ServicesLedger.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerServices != null)
                                return _ledgerServices.LedgerHead;
                            break;
                        case AppConstants.SERVICES_CGST:
                            var _ledgerServicesCGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().ServicesCGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerServicesCGST != null)
                                return _ledgerServicesCGST.LedgerHead;
                            break;
                        case AppConstants.SERVICES_SGST:
                            var _ledgerServicesSGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().ServicesSGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerServicesSGST != null)
                                return _ledgerServicesSGST.LedgerHead;
                            break;
                        case AppConstants.SERVICES_IGST:
                            var _ledgerServicesIGST = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().ServicesIGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerServicesIGST != null)
                                return _ledgerServicesIGST.LedgerHead;
                            break;
                        case AppConstants.SERVICES_OTHER_STATE:
                            var _ledgerServicesOther = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().ServicesOtherState.Where(x => x.GSTRate == _gst).FirstOrDefault();
                            if (_ledgerServicesOther != null)
                                return _ledgerServicesOther.LedgerHead;
                            break;
                        default:
                            return null;

                    }


                }
                return null;
            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in GST Ledger", _ex);
                return null;
            }
            finally
            {

            }


        }

    }

    public class Ledgers
    {
        public string Types { get; set; }
        public List<Ledger> PurchaseLedger { get; set; }
        public List<Ledger> PurchaseOtherState { get; set; }
        public List<Ledger> LedgerCGST { get; set; }
        public List<Ledger> LedgerSGST { get; set; }
        public List<Ledger> LedgerIGST { get; set; }
        public List<Ledger> SalesLedger { get; set; }
        public List<Ledger> LiabilityCGST { get; set; }
        public List<Ledger> LiabilitySGST { get; set; }
        public List<Ledger> LiabilityIGST { get; set; }
        public List<Ledger> SalesOtherState { get; set; }

        public List<Ledger> ServicesLedger { get; set; }
        public List<Ledger> ServicesCGST { get; set; }
        public List<Ledger> ServicesSGST { get; set; }
        public List<Ledger> ServicesIGST { get; set; }
        public List<Ledger> ServicesOtherState { get; set; }
    }
    public class Ledger
    {
        public string GSTRate { get; set; }
        public string LedgerHead { get; set; }
        public string Remarks { get; set; }

        public string StoreTypes { get; set; }
    }
}
