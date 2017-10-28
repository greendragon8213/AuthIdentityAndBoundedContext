namespace API.Models
{
    public class ExceptionResponse<T>
    {
        /// <summary>
        /// Can be displayed
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Info for dev
        /// </summary>
        public T ExceptionData { get; set; }
    }
}