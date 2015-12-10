using System.Collections.Generic;

namespace CSDeveloper.DataAccess
{
    public interface IDataSource
    {
        IEnumerable<T> RunQuery<T>(DataSourceCommand<T> command) where T: new();
        T RunScalar<T>(DataSourceCommand<T> command);
        long RunCommand<T>(DataSourceCommand<T> command);
        bool RunAsTransaction<T>(List<DataSourceCommand<T>> commands);
    }
}
