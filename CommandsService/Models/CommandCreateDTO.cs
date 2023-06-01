using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
	public class CommandCreateDTO
	{
		[Required]
		public string HowTo { get; set; } = string.Empty;
		[Required]
		public string CommandLine { get; set; } = string.Empty;
	}
}
