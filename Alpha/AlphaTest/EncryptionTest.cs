using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using CSDeveloper;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AlphaTest
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]

        public void trythis()
        {
            LoginTest();
        }
        public async void LoginTest()
        {
            string _auth = string.Format("{0}:{1}", "myUser", "myPwd");

            string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
            string _cred = string.Format("{0} {1}", "Basic", _enc);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:3826/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _auth);

                try
                {



                    // New code:
                    HttpResponseMessage response = await client.GetAsync(@"api/values/Login?name=abc&password=abc");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("what");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        [TestMethod]
        public void TokenTest()
        {
            DateTime time = DateTime.Now; 
            string token = Encryption.CreateToken(time);

            Assert.IsTrue(Encryption.Validated(token));
            Assert.AreNotEqual(token, Encryption.CreateToken());
            Assert.AreNotEqual(Encryption.CreateToken(), Encryption.CreateToken());
            Assert.AreNotEqual(Encryption.CreateToken(), Encryption.CreateToken());
            Assert.AreNotEqual(Encryption.CreateToken(), Encryption.CreateToken());

            time = DateTime.Now;
            token = Encryption.CreateToken(time, 5);

            Thread.Sleep(60000);
            Assert.IsTrue(Encryption.Validated(token,1));
            Assert.AreEqual(token.Length, 16);

            time = DateTime.Now;
            token = Encryption.CreateToken(time, 80);
            Thread.Sleep(120000);
            Assert.IsFalse(Encryption.Validated(token,1));
            Assert.AreEqual(token.Length, 72);

        }
        [TestMethod]
        public void Password()
        {
            string pass11 = Encryption.CreateRandomPassword();
            int salt11 = Encryption.CreateRandomSalt();
            string pass22 = Encryption.CreateRandomPassword();
            int salt22 = Encryption.CreateRandomSalt();

            string hash11 = Encryption.HashPassword(pass11, salt11);
            string hash12 = Encryption.HashPassword(pass11, salt11);
            string hash21 = Encryption.HashPassword(pass22, salt22);
            string hash22 = Encryption.HashPassword(pass22, salt22);

            Assert.AreEqual(hash11, hash12);
            Assert.AreEqual(hash21, hash22);

            Assert.AreNotEqual(pass11, pass22);

        }

    }
}
