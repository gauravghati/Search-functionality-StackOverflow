namespace mvcApp.Models
{
    public partial class SearchPost
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? AuthorName { get; set; }
        public string? Badges { get; set; }
        public int? AnswerCount { get; set; }
        public int? votesCount { get; set; }
        public int? Reputation { get; set; }
    }
}