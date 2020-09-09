
using Cosmos;
namespace AscensionGateServer
{
    public class JWTManager : Module<JWTManager>
    {

        IJWTTokenHelper tokenHelper;
        public void SetHelper(IJWTTokenHelper helper)
        {
            this.tokenHelper = helper;
        }
        public string EncodeToken(string key, object value)
        {
            return tokenHelper.EncodeToken(key, value);
        }
        public string DecodeToken(string token)
        {
            return tokenHelper.DecodeToken(token);
        }
        public T DecodeToken<T>(string token)
        {
            return tokenHelper.DecodeToken<T>(token);
        }
    }
}
