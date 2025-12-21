namespace TeamLink.API.DTO
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AddUserSkillDto
    {
        public int SkillId { get; set; }
    }
}
