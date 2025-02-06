using ClosedXML.Excel;
using FalcaPOS.Common;
using FalcaPOS.Common.Attributes;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.ExtenstionMethods;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Zone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public class CommonService : ICommonService
    {
        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        public CommonService(Logger logger, INotificationService notificationService)
        {
            _logger = logger;

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public bool ExportToXL<T>(List<T> data, string fileName, bool skipFilename = false, string tablename = "Falca POS Reports", String FilePath = "")
        {
            try
            {
                if (data == null || data.Count() <= 0)
                {
                    _logger.LogError("Data was empty to generate report");

                    return false;
                }


                using (XLWorkbook _workbook = new XLWorkbook())
                {
                    IEnumerable<ExportHeaderType> headers = GetHeaders<T>();

                    DataTable _dt = headers.GetDataTableFromIEnumerableT(data);

                    _dt.TableName = tablename;

                    _ = _workbook.Worksheets.Add(_dt);

                    string path = (String.IsNullOrEmpty(FilePath) ? ApplicationSettings.REPORTS_PATH : FilePath);

                    DirectoryInfo _info = Directory.CreateDirectory(path);

                    // Sets Suffix to 1.
                    int suffix = 1;

                    do
                    {
                        if (File.Exists(fileName))
                        {
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({suffix})";
                            suffix++;
                        }
                        else
                        {
                            break; // Exit the loop if the file name is available
                        }
                    } while (true);


                    string incrementFileName = fileName;

                    string _fileName = (skipFilename ? incrementFileName : StringHelper.GetReportFileName(incrementFileName)) + ".xlsx";


                    string _filePathName = path + "\\" + _fileName;

                    _workbook.SaveAs(_filePathName);

                    _workbook.Dispose();

                    ProcessStartInfo _p = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _filePathName
                    };

                    _ = Process.Start(_p);

                    return true;
                }
            }

            catch (Exception _ex)
            {

            }

            return false;
        }

        private static IEnumerable<ExportHeaderType> GetHeaders<T>()
        {
            return typeof(T).GetProperties()
                .Where(FilterAttributes)
                .Select(x => new ExportHeaderType { Name = x.Name, DisplayName = GetDisplayName(x), PropType = x.PropertyType });
        }

        private static bool FilterAttributes(PropertyInfo arg)
        {
            return arg.GetCustomAttribute(typeof(IgnorePropertyAttribute), false) == null;
        }

        private static string GetDisplayName(PropertyInfo prop)
        {


            object[] attributes = prop.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var displayName = (DisplayNameAttribute)attributes[0];
                return displayName.DisplayName;
            }
            return prop.Name;
        }

        public async Task<IEnumerable<District>> GetAllDistricts(CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                    .GetAsync<IEnumerable<District>>(AppConstants.GET_ALl_DISTRICTS, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting all districts", _ex);
            }
            return Enumerable.Empty<District>();

        }

        public async Task<IEnumerable<District>> GetDistricts(int stateId, CancellationToken token)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<District>>($"{AppConstants.GET_STATE_DISTRICTS}/{stateId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting all districts", _ex);
            }

            return Enumerable.Empty<District>();
        }

        public async Task<IEnumerable<State>> GetStates(string querry = null, CancellationToken token = default)
        {
            try
            {
                //?isenabled = true
                return await HttpHelper.GetAsync<IEnumerable<State>>($"{AppConstants.GET_STATES}{querry}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting all districts", _ex);
            }

            return Enumerable.Empty<State>();
        }

        public bool ExportToXL<T1, T2>(IEnumerable<T1> T1Data, IEnumerable<T2> T2Data, string T1Title, string T2Title, string fileName = null, bool skipFilename = false, string FilePath = null)
        {
            try
            {

                if (T1Data != null && T2Data != null && T1Data.Count() > 0 && T2Data.Count() > 0)
                {
                    using (XLWorkbook _workbook = new XLWorkbook())
                    {

                        IEnumerable<ExportHeaderType> headersT1 = GetHeaders<T1>();

                        DataTable _dt1 = headersT1.GetDataTableFromIEnumerableT(T1Data);

                        _dt1.TableName = T1Title ?? "Title one";

                        _ = _workbook.Worksheets.Add(_dt1);


                        IEnumerable<ExportHeaderType> headersT2 = GetHeaders<T2>();

                        DataTable _dt2 = headersT2.GetDataTableFromIEnumerableT(T2Data);

                        _dt2.TableName = T2Title ?? "Title two";

                        _ = _workbook.Worksheets.Add(_dt2);


                        string path = FilePath == null ? ApplicationSettings.REPORTS_PATH : FilePath;

                        DirectoryInfo _info = Directory.CreateDirectory(path);

                        string _fileName = (skipFilename ? fileName : StringHelper.GetReportFileName(fileName)) + ".xlsx";

                        string _filePathName = path + "\\" + _fileName;

                        _workbook.SaveAs(_filePathName);

                        _workbook.Dispose();

                        ProcessStartInfo _p = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = _filePathName
                        };

                        _ = Process.Start(_p);

                        return true;
                    }


                }
                else
                {
                    _notificationService.ShowMessage("Invalid Data to export to XL", NotificationType.Error);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting ", _ex);
            }

            return false;
        }

        public async Task<Response<State>> CreateState(State state, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<State, Response<State>>
                    (AppConstants.CREATE_NEW_STATE, state, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in creating new state", _ex);
            }

            return new Response<State>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<State>> UpdateState(int stateId, State state, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<State, Response<State>>
                    ($"{AppConstants.UPDATE_STATE}/{stateId}", state, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating state", _ex);
            }

            return new Response<State>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<District>> CreateDistrict(District district, CancellationToken token = default)
        {
            try
            {

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<District, Response<District>>
                    (AppConstants.CREATE_DISTRICT, district, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in creating new district", _ex);
            }

            return new Response<District>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<District>> UpdateDistrict(int districtId, District district, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PutAsync<District, Response<District>>
                    ($"{AppConstants.UPDATE_DISTRICT}/{districtId}", district, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in update district", _ex);
            }

            return new Response<District>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

       
        public bool ExportObjectToXL<T>(List<T> data, List<string> columns,Dictionary<string,string> propdisplayNames , string fileName, bool skipFilename = false, string tablename = "Falca POS Reports", String FilePath = "")
        {
            try
            {
                if (data == null || data.Count() <= 0)
                {
                    _logger.LogError("Data was empty to generate report");

                    return false;
                }


                using (XLWorkbook _workbook = new XLWorkbook())
                {
                    IEnumerable<ExportHeaderType> headers = GetHeadersObject(columns);

                    DataTable _dt = headers.GetDataTableFromIEnumerableExpandoObject(data);
                    if (propdisplayNames != null && propdisplayNames.Keys.Count > 0)
                    {
                        foreach (var item in headers)
                        {
                            _dt.Columns[item.Name].ColumnName = propdisplayNames[item.Name];
                        }
                    }
                    _dt.TableName = tablename;

                    _ = _workbook.Worksheets.Add(_dt);

                    //map prop name as display name for Object

                    
                    string path = (String.IsNullOrEmpty(FilePath) ? ApplicationSettings.REPORTS_PATH : FilePath);

                    DirectoryInfo _info = Directory.CreateDirectory(path);

                    // Sets Suffix to 1.
                    int suffix = 1;

                    do
                    {
                        if (File.Exists(fileName))
                        {
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({suffix})";
                            suffix++;
                        }
                        else
                        {
                            break; // Exit the loop if the file name is available
                        }
                    } while (true);


                    string incrementFileName = fileName;

                    string _fileName = (skipFilename ? incrementFileName : StringHelper.GetReportFileName(incrementFileName)) + ".xlsx";


                    string _filePathName = path + "\\" + _fileName;

                    _workbook.SaveAs(_filePathName);

                    _workbook.Dispose();

                    ProcessStartInfo _p = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _filePathName
                    };

                    _ = Process.Start(_p);

                    return true;
                }
            }

            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return false;
        }

        private IEnumerable<ExportHeaderType> GetHeadersObject(List<string> columns)
        {
            return columns.Select(x => new ExportHeaderType { DisplayName = x, Name = x, PropType = typeof(String) });
        }
    }


}
