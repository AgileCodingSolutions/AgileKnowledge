namespace AgileKnowledge.Service.Mappings.KnowledgeBases
{
    public class PublishingDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? chatApplicationId { get; set; }
        public int availableToken { get; set ; }
        public int availableQuantity { get; set; }
        public string? apiKey { get; set; }
        public int usedToken { get; set; }
    }
}
