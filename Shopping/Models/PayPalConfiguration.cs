using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.Api;

namespace Shopping.Models
{
    //Lấy định dạng Paypal từ webconfig.
    public class PayPalConfiguration
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;

        static PayPalConfiguration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }

        //Tạo access token.
        public static string GetAccessToken()
        {
            string accessToekn = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToekn;
        }

        //Trả về apicontext object.
        public static APIContext GetAPIContext()
        {
            var apiConext = new APIContext(GetAccessToken());
            apiConext.Config = GetConfig();
            return apiConext;
        }
    }
}