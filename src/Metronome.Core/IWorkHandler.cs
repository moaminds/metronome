using System.Threading.Tasks;

namespace Metronome.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWorkHandler<in T>
        where T : WorkDefinition
    {
        Task HandleAsync(T work);
    }
}
