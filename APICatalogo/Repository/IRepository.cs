using System.Linq.Expressions;

namespace APICatalogo.Repository;

public interface IRepository<T> 
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    T Create(T entity); // Não acessam banco, realizam operações na memoria, por isso não precisam ser Task
    T Update(T entity);// SaveChanges é onde realmente acessa o banco
    T Delete(T entity);
}
