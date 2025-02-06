using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Common.Helper
{
    public class HttpHelper
    {
        public static async Task<T> GetAsync<T>(string resourceURL, string accessToken = "", CancellationToken Cancellationtoken = default(CancellationToken))
        {
            try
            {
                //if cache settings enabled

                Cancellationtoken.ThrowIfCancellationRequested();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    //Increase time out to 120 seconds
                    client.Timeout = TimeSpan.FromSeconds(120);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    using (var _response = await client.GetAsync(resourceURL, Cancellationtoken))
                    {
                        Cancellationtoken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        Cancellationtoken.ThrowIfCancellationRequested();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        Cancellationtoken.ThrowIfCancellationRequested();

                        return JsonConvert.DeserializeObject<T>(_responseBody);
                    }
                }
            }
            catch (Exception _ex)
            {
                throw;
            }
        }


        public static async Task<TOut> PostAsync<TIn, TOut>(string resource, TIn content, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var temp = JsonConvert.SerializeObject(content);


                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    cancellationToken.ThrowIfCancellationRequested();

                    using (var _response = await _client.PostAsync(resource, _serializedConetent))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_responseBody);
                    }
                }
            }
            catch (Exception _ex)
            {
                var temp = _ex.Message;
                throw;
            }
        }


        public static async Task<DataSet> PostAsyncDataSet<TIn, DataSet>(string resource, TIn content, string token = "")
        {
            try
            {

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    using (var _response = await _client.PostAsync(resource, _serializedConetent))
                    {
                        _response.EnsureSuccessStatusCode();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<DataSet>(_responseBody);
                    }
                }
            }
            catch (Exception _ex)
            {

                throw;
            }
        }

        public static async Task<bool> PostAsync<TIn>(string resource, TIn content, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    using (var _response = await _client.PostAsync(resource, _serializedConetent))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        return true;
                    }
                }
            }
            catch (HttpRequestException _ex)
            {
                throw;
            }
            catch (Exception _ex)
            {

                throw;
            }

        }

        public static async Task PostFormDataAsync(string resource, MultipartFormDataContent formDataContent, string token = "")
        {

            try
            {

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    using (var _response = await _client.PostAsync(resource, formDataContent))
                    {
                        _response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception _ex)
            {

                throw;
            }

        }


        public static async Task<bool> DeleteAsync(string resource, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    using (var _response = await _client.DeleteAsync(resource, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        return true;
                    }
                }
            }
            catch (HttpRequestException _ex)
            {
                throw;
            }
            catch (Exception _ex)
            {

                throw;
            }

        }


        public static async Task<TOut> DeleteAsync<TOut>(string resource, string TOKEN, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.IsNullOrEmpty(TOKEN)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);


                    using (var _response = await _client.DeleteAsync(resource, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        var _result = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_result);
                    }
                }
            }
            catch (HttpRequestException _ex)
            {
                throw;
            }
            catch (Exception _ex)
            {

                throw;
            }

        }

        public static async Task<TOut> PutAsync<TIn, TOut>(string resource, TIn content, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    cancellationToken.ThrowIfCancellationRequested();

                    using (var _response = await _client.PutAsync(resource, _serializedConetent, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        cancellationToken.ThrowIfCancellationRequested();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_responseBody);
                    }
                }
            }
            catch (Exception _ex)
            {

                throw;
            }
        }

        public static async Task<bool> PutAsync(string resource, string token = "")
        {
            try
            {

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    using (var _response = await _client.PutAsync(resource, null))
                    {
                        _response.EnsureSuccessStatusCode();

                        return true;
                    }
                }
            }
            catch (Exception _ex)
            {

                throw;
            }
        }

        public static async Task<TOut> PutAsync<TOut>(string resource, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    //var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    cancellationToken.ThrowIfCancellationRequested();

                    using (var _response = await _client.PutAsync(resource, null, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        _response.EnsureSuccessStatusCode();

                        cancellationToken.ThrowIfCancellationRequested();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_responseBody);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static async Task<TOut> PostFormDataAsync<TOut>(string uri, string access_token, MultipartFormDataContent multipartForm)
        {
            try
            {

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    if (!string.IsNullOrEmpty(access_token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                    using (var _response = await _client.PostAsync(uri, multipartForm))
                    {
                        _response.EnsureSuccessStatusCode();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_responseBody);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public static async Task<TOut> DenominationPostAsync<TIn, TOut>(string resource, TIn content, string token = "", CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    try
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        using (var _client = new HttpClient())
        //        {
        //            _client.BaseAddress = new Uri(ApplicationSettings.DenominationUrl);
        //            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



        //            if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //            var _serializedConetent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        //            cancellationToken.ThrowIfCancellationRequested();

        //            using (var _response = await _client.PostAsync(resource, _serializedConetent))
        //            {
        //                cancellationToken.ThrowIfCancellationRequested();

        //                _response.EnsureSuccessStatusCode();

        //                cancellationToken.ThrowIfCancellationRequested();

        //                string _responseBody = await _response.Content.ReadAsStringAsync();

        //                return JsonConvert.DeserializeObject<TOut>(_responseBody);
        //            }
        //        }
        //    }
        //    catch (Exception _ex)
        //    {

        //        throw;
        //    }
        //}


        //public static async Task<T> DenominationGetAsync<T>(string resourceURL, string accessToken = "", CancellationToken Cancellationtoken = default(CancellationToken))
        //{
        //    try
        //    {

        //        Cancellationtoken.ThrowIfCancellationRequested();

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ApplicationSettings.DenominationUrl);

        //            //Increase time out to 120 seconds
        //            client.Timeout = TimeSpan.FromSeconds(120);

        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            if (!string.IsNullOrEmpty(accessToken))
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //            }

        //            using (var _response = await client.GetAsync(resourceURL, Cancellationtoken))
        //            {
        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                _response.EnsureSuccessStatusCode();

        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                string _responseBody = await _response.Content.ReadAsStringAsync();

        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                return JsonConvert.DeserializeObject<T>(_responseBody);
        //            }
        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        throw;
        //    }
        //}




        //public static async Task<TOut> PostFormDataDenmiantionAsync<TOut>(string uri, string access_token, MultipartFormDataContent multipartForm)
        //{
        //    try
        //    {

        //        using (var _client = new HttpClient())
        //        {
        //            _client.BaseAddress = new Uri(ApplicationSettings.DenominationUrl);

        //            if (!string.IsNullOrEmpty(access_token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

        //            using (var _response = await _client.PostAsync(uri, multipartForm))
        //            {
        //                _response.EnsureSuccessStatusCode();

        //                string _responseBody = await _response.Content.ReadAsStringAsync();

        //                return JsonConvert.DeserializeObject<TOut>(_responseBody);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //public static async Task<T> GetAsyncDenomination<T>(string resourceURL, string filenameId, string accessToken = "", CancellationToken Cancellationtoken = default(CancellationToken))
        //{
        //    try
        //    {
        //        //if cache settings enabled

        //        Cancellationtoken.ThrowIfCancellationRequested();

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ApplicationSettings.DenominationUrl);

        //            //Increase time out to 120 seconds
        //            client.Timeout = TimeSpan.FromSeconds(120);

        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            if (!string.IsNullOrEmpty(accessToken))
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //            }

        //            var _serializedConetent = new StringContent(JsonConvert.SerializeObject(filenameId), Encoding.UTF8, "application/json");

        //            using (var _response = await client.PostAsync(resourceURL, _serializedConetent))
        //            {
        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                _response.EnsureSuccessStatusCode();

        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                string _responseBody = await _response.Content.ReadAsStringAsync();

        //                Cancellationtoken.ThrowIfCancellationRequested();

        //                return JsonConvert.DeserializeObject<T>(_responseBody);
        //            }
        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        throw;
        //    }
        //}


        public static async Task<TOut> PostFormDataWithAttachmentAsync<TOut>(string uri, string access_token, MultipartFormDataContent multipartForm)
        {
            try
            {

                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(ApplicationSettings.ServerUrl);

                    if (!string.IsNullOrEmpty(access_token)) _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                    using (var _response = await _client.PostAsync(uri, multipartForm))
                    {
                        _response.EnsureSuccessStatusCode();

                        string _responseBody = await _response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(_responseBody);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
