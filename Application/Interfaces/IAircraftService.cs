using AircraftMRO.Application.DTOs.Aircraft;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;

namespace AircraftMRO.Services.Interfaces;

public interface IAircraftService
{
    Task<PagedResult<AircraftListDto>> GetAircraftAsync(AircraftFilter filter);

    Task<ServiceResult<AircraftDetailsDto>> GetAircraftDetailsAsync(int id);

    Task<ServiceResult<Aircraft>> CreateAircraftAsync(AircraftCreateDto dto);

    Task<ServiceResult<AircraftEditDto>> GetForEditAsync(int id);

    Task<ServiceResult<Aircraft>> UpdateAsync(AircraftEditDto dto);

    Task<ServiceResult<AircraftDeleteDto>> GetForDeleteAsync(int id);

    Task<ServiceResult<bool>> DeleteAsync(int id);
}