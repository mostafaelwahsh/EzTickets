using System.ComponentModel.DataAnnotations;

namespace Models;

public class ContactRequest
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    [Required, MaxLength(2000)]
    public string Message { get; set; }

    public DateTime SubmittedAt { get; set; }
    public ContactStatus Status { get; set; }
}

public enum ContactStatus
{
    New,
    InProgress,
    Resolved
}