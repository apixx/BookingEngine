namespace BookingEngine.BusinessLogic.Models
{
    public class ErrorResponse
    {
        public List<ErrorDetail> Errors { get; set; }
    }

    public class ErrorDetail
    {
        public int Status { get; set; }
        public int Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public Source Source { get; set; }
    }

    public class Source
    {
        public string Parameter { get; set; }
        public string Example { get; set; }
    }

}
