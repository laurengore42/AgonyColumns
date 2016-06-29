﻿namespace AgonyColumns
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    
    using EransOAuth;

    internal class Program
    {
        static async Task<string> MakeTweetCall(string baseUrl, string authHeader, string status)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                    {
                       { "status", status }
                    };

                var content = new FormUrlEncodedContent(values);

                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("OAuth", authHeader);

                client.DefaultRequestHeaders.Authorization = authHeaders;

                var response = await client.PostAsync(baseUrl, content);

                var responseString = await response.Content.ReadAsStringAsync();

                return responseString;
            }
        }

        static string MakeTweet(string consumKey, string consumSecret, string userToken, string userSecret, string nonce, string timestamp, string status)
        {
            var siggie = new EransOAuth.OAuthBase();
            var baseUrl = "https://api.twitter.com/1.1/statuses/update.json";

            var method = "POST";
            var baseUrlUri = new Uri(baseUrl + "?" + "status=" + status);
                        
            string normedUrl;
            string normedParams;
            
            Console.WriteLine(siggie.GenerateSignatureBase(baseUrlUri,consumKey,userToken,userSecret,method,timestamp,nonce,"HMAC-SHA1",out normedUrl,out normedParams));

            var signature = siggie.GenerateSignature(baseUrlUri, consumKey, consumSecret, userToken, userSecret, method, timestamp, nonce, out normedUrl, out normedParams);

            var dst = string.Empty;

            dst += "oauth_consumer_key";
            dst += "=";
            dst += '"' + siggie.UrlEncode(consumKey) + '"';

            dst += ", ";

            dst += "oauth_nonce";
            dst += "=";
            dst += '"' + siggie.UrlEncode(nonce) + '"';

            dst += ", ";

            dst += "oauth_signature";
            dst += "=";
            dst += '"' + siggie.UrlEncode(signature) + '"';

            dst += ", ";

            dst += "oauth_signature_method";
            dst += "=";
            dst += '"' + siggie.UrlEncode("HMAC-SHA1") + '"';

            dst += ", ";

            dst += "oauth_timestamp";
            dst += "=";
            dst += '"' + siggie.UrlEncode(timestamp) + '"';

            dst += ", ";

            dst += "oauth_token";
            dst += "=";
            dst += '"' + siggie.UrlEncode(userToken) + '"';

            dst += ", ";

            dst += "oauth_version";
            dst += "=";
            dst += '"' + "1.0" + '"';

            Console.WriteLine();
            Console.WriteLine(dst);
            Console.WriteLine();

            var outStr = MakeTweetCall(baseUrl, dst, status);
            outStr.Wait();

            return outStr.Result;
        }

        static void Main(string[] args)
        {
            var consumKey = "dummy";
            var consumSecret = "dummy";

            var userToken = "dummy";
            var userSecret = "dummy";

            var nonce = (new EransOAuth.OAuthBase()).GenerateNonce();
            var timestamp = (new EransOAuth.OAuthBase()).GenerateTimeStamp();

            var status = "testTweetText";

            Console.WriteLine(MakeTweet(consumKey, consumSecret, userToken, userSecret, nonce, timestamp, status));
            
            Console.ReadLine();
        }
    }
}