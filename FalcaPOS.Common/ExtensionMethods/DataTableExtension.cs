using FalcaPOS.Entites.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FalcaPOS.Common.ExtenstionMethods
{
    public static class DataTableExtension
    {
        public static DataTable GetDataTableFromIEnumerableT<T>(this IEnumerable<ExportHeaderType> headers, IEnumerable<T> data)
        {
            DataTable _dataTable = new DataTable();

            _ = _dataTable.Columns.Add("SL No", typeof(int));

            foreach (ExportHeaderType _header in headers)
            {
                _dataTable.Columns.Add(_header.DisplayName, _header.PropType);

            }

            int index = 1;

            foreach (T item in data)
            {

                List<object> _list = new List<object> { index };

                foreach (ExportHeaderType hed in headers)
                {
                    PropertyInfo _pinfo = item.GetType().GetProperty(hed.Name);

                    object _value = _pinfo.GetValue(item, null);

                    _list.Add(_value);
                }
                _ = _dataTable.Rows.Add(_list.ToArray());

                index++;
            }
            return _dataTable;
        }

        public static DataTable GetDataTableFromIEnumerableExpandoObject<T>(this IEnumerable<ExportHeaderType> headers, IEnumerable<T> data)
        {
            DataTable _dataTable = new DataTable();

            _ = _dataTable.Columns.Add("SL No", typeof(int));

            foreach (T item in data)
            {
                foreach (ExportHeaderType _header in headers)
                {
                    
                    _dataTable.Columns.Add(_header.DisplayName, ((IDictionary<string, object>)item)[_header.Name]==null ? typeof(string) : ((IDictionary<string, object>)item)[_header.Name].GetType());

                }
                break;
            }

            int index = 1;

            foreach (T item in data)
            {
                List<object> _list = new List<object> { index };
                foreach (ExportHeaderType hed in headers)
                {
                    _list.Add(((IDictionary<string, object>)item)[hed.Name]);
                }
                _ = _dataTable.Rows.Add(_list.ToArray());

                index++;
            }
            return _dataTable;
        }

    }
}
