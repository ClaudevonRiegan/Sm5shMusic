﻿namespace Sm5sh.Interfaces
{
    public interface IStateManager
    {
        bool Init();
        T LoadResource<T>(string gameRelativeResourcePath, bool optional = false) where T : IStateManagerDb, new();
        bool WriteChanges();
    }
}
