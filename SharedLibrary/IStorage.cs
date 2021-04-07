namespace SharedLibrary
{
    public interface IStorage
    {
        void Store(string shard, string key, string value);
        void StoreShard(string key, string shard);
        string Load(string shard, string key);
        string LoadShard(string key);
        bool HasTextDuplicates(string text);
        bool IsKeyExist(string shard, string key);
    }
}