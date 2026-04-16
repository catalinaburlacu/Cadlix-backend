using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Structure;

namespace Cadlix_backend.BusinessLayer;

public class BusinessLogic
{
    public IMovieAction MovieAction()
    {
        return new MovieActionExecution();
    }
}
