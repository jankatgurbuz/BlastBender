using Cysharp.Threading.Tasks;

namespace Global.Controller
{
    public interface IStartable
    {
        UniTask Start();
    }
}
