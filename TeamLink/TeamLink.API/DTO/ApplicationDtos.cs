namespace TeamLink.API.DTOs
{
    public class CreateApplicationDto
    {
        public int ProjectId { get; set; }
        public string Message { get; set; }
    }
    public class ApplicationDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string ApplicantName { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }

    public class UpdateApplicationStatusDto
    {
        public bool IsAccepted { get; set; }
    }
}