namespace AgileKnowledge.Service.Options
{
	public class JwtOptions
	{
		public const string Name = "Jwt";

		public static string Secret { get; set; }

		public static int EffectiveHours { get; set; }
	}
}
