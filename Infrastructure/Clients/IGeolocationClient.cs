using JobPortal.Models;
using System.Threading.Tasks;

namespace JobPortal.WebAPI.Infrastructure.Clients
{
    public interface IGeolocationClient
    {
        Task<GeolocationResponse> GetGeolocation(GeolocationRequestModel requestModel);
    }
}