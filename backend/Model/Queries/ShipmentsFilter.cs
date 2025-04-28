using System.Reflection;

namespace CosmoCargo.Model.Queries
{
    public class ShipmentsFilter
    {
        public string? Search { get; set; }
        public ShipmentStatus? Status { get; set; }

        // public static ValueTask<ShipmentsFilter?> BindAsync(HttpContext context, ParameterInfo parameter)
        // {
        //     var query = context.Request.Query;
        //     var filter = new ShipmentsFilter
        //     {
        //         Search = query["search"],
        //         Status = Enum.TryParse<ShipmentStatus>(query["status"], out var status) ? status : (ShipmentStatus?)null
        //     };

        //     return ValueTask.FromResult<ShipmentsFilter?>(filter);
        // }
    }
}
