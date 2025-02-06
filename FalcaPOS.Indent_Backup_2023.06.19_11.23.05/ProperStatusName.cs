using FalcaPOS.Entites.Indent;
using System;

namespace FalcaPOS.Indent
{
    public class ProperStatus
    {
        public static string ProperStatusName(String status)
        {
            try
            {
                if (!String.IsNullOrEmpty(status))
                {
                    if (Enum.IsDefined(typeof(IndentStatus), status))
                    {
                        IndentStatus CurrentStatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), status);
                        switch (CurrentStatus)
                        {
                            case IndentStatus.created:
                                return "Planned";
                            case IndentStatus.review:
                                return "Review";
                            case IndentStatus.approve:
                                return "Approve";
                            case IndentStatus.addsupplier:
                                return "Add Supplier";
                            case IndentStatus.placed:
                                return "Placed";
                            case IndentStatus.intransit:
                                return "InTransit";
                            case IndentStatus.received:
                                return "Received";
                            case IndentStatus.closed:
                                return "Closed";
                            default:
                                return status;

                        }
                    }
                }
            }
            catch (Exception)
            {
                return String.Empty;
            }
            return String.Empty;
        }



    }
}
