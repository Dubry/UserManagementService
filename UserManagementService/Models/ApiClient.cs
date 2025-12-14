namespace UserManagementService.Models
{
    public class ApiClient
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; } = default!;

        public string ApiKey { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
