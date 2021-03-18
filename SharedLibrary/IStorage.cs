using System.Collections.Generic;

namespace SharedLibrary
{
    public interface IStorage
    {
        void Store(string key, string value);
        string Load(string key);
        IEnumerable<string> GetKeys();
        bool IsKeyExist(string key);
    }
}