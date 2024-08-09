using AgileKnowledge.Service.Domain.Enum;

namespace AgileKnowledge.Service.Mappings.ChatApplications
{
	public class CreateChatDialogHistoryInputDto
	{
		public Guid Id { get; set; }
    
		/// <summary>
		/// 对话id
		/// </summary>
		public Guid ChatDialogId { get; set; }

		/// <summary>
		/// 对话内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 消耗token
		/// </summary>
		public int TokenConsumption { get; set; }

		/// <summary>
		/// 对话类型
		/// </summary>
		public ChatDialogHistoryType Type { get; set; }


	}
}
