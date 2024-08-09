namespace AgileKnowledge.Service.Mappings.ChatApplications
{
	public class CreateChatDialogInputDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// 应用Id
		/// </summary>
		public Guid ApplicationId { get; set; }
		
	}
}
