using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.Domain.DTOs.Frontend;

public class LeaderboardFiltersDto
{
    public List<UiOptionDto> Time { get; set; } = new();
    public List<UiOptionDto> Scope { get; set; } = new();
}

public class LeaderboardPayloadDto
{
    public LeaderboardFiltersDto Filters { get; set; } = new();
    public List<LeaderboardEntryDto> Users { get; set; } = new();
}
