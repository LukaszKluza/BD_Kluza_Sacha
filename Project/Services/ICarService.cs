using System.Threading.Tasks;

public interface ICarService
{
    Task CreateCarAsync(Car car);
    Task<bool> DeleteCarAsync(int id);
}
