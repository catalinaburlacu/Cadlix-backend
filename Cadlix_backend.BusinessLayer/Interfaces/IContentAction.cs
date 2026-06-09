using Cadlix_backend.Domain.DTOs.Content;
using Cadlix_backend.Domain.DTOs.Frontend;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IContentAction
{
    IEnumerable<ContentDTO> GetAllContent();
    IEnumerable<ContentDTO> GetContentByType(string type);
    ContentDTO? GetContentById(int id);
    ContentDTO CreateContent(CreateContentDTO dto);
    ContentDTO? UpdateContent(int id, UpdateContentDTO dto);
    bool DeleteContent(int id);
    IEnumerable<ContentDTO> SearchContent(string query);
}
