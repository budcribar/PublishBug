namespace Interfaces
{
    public interface IHPSTInterface
    {
        void SaveNonVolatileStorage();
        bool NonVolatileStorageExists { get; }
        void RestoreNonVolatileStorage();
    }
}
