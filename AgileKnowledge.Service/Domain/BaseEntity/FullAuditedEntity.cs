namespace AgileKnowledge.Service.Domain.BaseEntity
{
	public class FullAuditedEntity
	{
		public Guid Id { get; set; }

		public DateTime CreationTime { get;  set; }
		public Guid? CreatorId { get;  set; }
		public DateTime? LastModificationTime { get; set; }
		public Guid? LastModifierId { get; set; }


		public bool IsDeleted { get; set; }
		public DateTime? DeletionTime { get; set; }
		public Guid? DeleterUserId { get; set; }
	}
}
