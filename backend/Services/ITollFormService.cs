using CosmoCargo.Models;

namespace CosmoCargo.Services
{
    public interface ITollFormService
    {
        Task<IEnumerable<TollForm>> GetAllTollFormsAsync();
        Task<TollForm?> GetTollFormByIdAsync(Guid id);
        Task<TollForm?> GetTollFormByShipmentIdAsync(Guid shipmentId);
        Task<TollForm> CreateTollFormAsync(TollForm tollForm);
        Task<TollForm?> ReviewTollFormAsync(Guid id, bool isApproved, string? reviewNotes, Guid reviewedById);
    }
} 