namespace HYDB.Services.Interfaces
{
    public interface IOperationFactory
    {
        IOperation GetOperation(string opKey);
    }
}
