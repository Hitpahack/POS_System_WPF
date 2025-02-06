namespace FalcaPOS.Common.Helper
{
    public class Response<T> where T : class
    {

        public bool IsSuccess { get; set; }

        public string Error { get; set; }
        public T Data { get; set; }       
        public string Message { get; set; }      

        public int ErrorCode { get; set; }
     
    }
}
