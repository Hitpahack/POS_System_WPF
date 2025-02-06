using Telerik.Windows.Controls;

namespace FalcaPOS.Common
{
    /// <summary>
    /// This class is used to perform telerik grid related operations.
    /// </summary>
    public static class ResetTelerikGridFilters
    {
        /// <summary>
        /// This method clears all the filters that are applied on the telerik grid.
        /// </summary>
        /// <param name="obj">telerik grid object</param>
        public static void ClearTelerikGridViewFilters(object obj)
        {
            if (obj != null)
            {
                var radGridView = (RadGridView)obj;
                radGridView.FilterDescriptors?.Clear();
            }
        }
    }
}
