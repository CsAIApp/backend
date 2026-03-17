namespace TeamLink.API.DTO
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Budget { get; set; }
        public string? Category { get; set; }
        public List<string>? Technologies { get; set; }
        public List<string>? CandidateQuestions { get; set; }
        public string? ExternalLink { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Budget { get; set; }
        public string? Category { get; set; }
        public List<string>? Technologies { get; set; }
        public List<string>? CandidateQuestions { get; set; }
        public string? ExternalLink { get; set; }
    }
}
