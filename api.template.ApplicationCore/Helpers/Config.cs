using System.Collections.Generic;

namespace Api.Template.ApplicationCore.Helpers
{
	public class Config
	{
		public static string ConnectionString { get; set; }
		public static int DefaultPageSize { get; set; }
		public static string SomeExternalAPI { get; set; }
		public static string KeyVaultClientID { get; set; }
		public static string KeyVaultClientSecret { get; set; }
	}
}