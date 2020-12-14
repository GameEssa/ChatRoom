
namespace Core.Net
{
	public interface IConnect
	{

		bool isConnect { get; }

		void Connect();

		void Close();
	}
}