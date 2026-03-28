using System;
using Cadlix_backend.BusinessLogic.Interfaces;

namespace Cadlix_backend.BusinessLogic;

public class BusinessLogic
{
    public ISession GetSessionBL()
    {
        return new SessionBL();
    }
}
