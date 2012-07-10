
namespace Client.Graphics
{
    public interface ICullable
    {
        bool CullTest(ICuller culler);
    }
}
