using System.ComponentModel.DataAnnotations;

namespace ChatClient.Models
{
    class MessageModel
    {
        [Required]
        public string Content { get; set; }
        
        [Required]
        public string Username { get; set; }
    }
}
