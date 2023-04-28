using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArcHundred.Lock.SwitchBot
{
    public class LockDevice
    {
        public LockDevice() { }

        private const string DeviceID = "ED1D5B07FAC3"; // Check your own device id
        private const string Token = ""; // put your own token
        private const string Secret = ""; // put your own token

        public string LockState { get; set; }

        public async Task<(string lockState, string doorState, int battery, string error)> GetStatus()
        {
            string lockState = "unknown";
            string doorState = "unknown";
            int battery = 100;
            string error = "";

            try
            {
                var request = CreateMessage(HttpMethod.Get, $"https://api.switch-bot.com/v1.1/devices/{DeviceID}/status");
                HttpClient client = new HttpClient();
                var response = await client.SendAsync(request);
                var json = response.Content.ReadAsStringAsync().Result;
                var doc = JsonSerializer.Deserialize<JsonElement>(json);
                lockState = doc.GetProperty("body").GetProperty("lockState").GetString();
                doorState = doc.GetProperty("body").GetProperty("doorState").GetString();
                battery = doc.GetProperty("body").GetProperty("battery").GetInt32();
                error = doc.GetProperty("message").GetString();
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return (lockState: lockState, doorState: doorState, battery: battery, error: error);
        }

        public async Task<(string lockState, string doorState, int battery, string error)> LockUnlock()
        {
            string lockState = "unknown";
            string doorState = "unknown";
            int battery = 100;
            string error = "";
            try
            {
                var status = await this.GetStatus();

                var request = CreateMessage(HttpMethod.Post, $"https://api.switch-bot.com/v1.1/devices/{DeviceID}/commands");

                string command;
                if (status.lockState == "locked")
                {
                    command = "{\"commandType\":\"command\", \"command\":\"unlock\", \"parameter\":\"default\"}";

                }
                else if (status.lockState == "unlocked")
                {
                    command = "{\"commandType\":\"command\", \"command\":\"lock\", \"parameter\":\"default\"}";
                }
                else
                {
                    throw new Exception("LockState is unknown");
                }

                request.Content = new StringContent(command, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var response = await client.SendAsync(request);
                var json = response.Content.ReadAsStringAsync().Result;
                var doc = JsonSerializer.Deserialize<JsonElement>(json);
                lockState = doc.GetProperty("body").GetProperty("items")[0].GetProperty("status").GetProperty("lockState").GetString();
                doorState = doc.GetProperty("body").GetProperty("items")[0].GetProperty("status").GetProperty("doorState").GetString();
                battery = doc.GetProperty("body").GetProperty("items")[0].GetProperty("status").GetProperty("battery").GetInt32();
                error = doc.GetProperty("message").GetString();
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return (lockState: lockState, doorState: doorState, battery: battery, error: error);
        }

        public async Task<string> SetupWebHook(string callbackUrl)
        {
            string error = "";

            try
            {
                var status = await this.GetStatus();

                var request = CreateMessage(HttpMethod.Post, @"https://api.switch-bot.com/v1.1/webhook/setupWebhook");
                string command = $"{{\"action\":\"setupWebhook\", \"url\":\"{callbackUrl}\", \"deviceList\":\"ALL\"}}";
                request.Content = new StringContent(command, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var response = await client.SendAsync(request);
                var json = response.Content.ReadAsStringAsync().Result;
                var doc = JsonSerializer.Deserialize<JsonElement>(json);
                error = doc.GetProperty("message").GetString();
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return error;
        }

        private HttpRequestMessage CreateMessage(HttpMethod method, string uri)
        {
            string token = Token;
            string secret = Secret;
            long time = Convert.ToInt64(DateTime.Now.ToFileTime());
            string nonce = Guid.NewGuid().ToString();
            string data = token + time.ToString() + nonce;
            Encoding utf8 = Encoding.UTF8;
            HMACSHA256 hmac = new HMACSHA256(utf8.GetBytes(secret));
            string signature = Convert.ToBase64String(hmac.ComputeHash(utf8.GetBytes(data)));

            var request = new HttpRequestMessage(method, uri);
            request.Headers.TryAddWithoutValidation(@"Authorization", token);
            request.Headers.TryAddWithoutValidation(@"sign", signature);
            request.Headers.TryAddWithoutValidation(@"nonce", nonce);
            request.Headers.TryAddWithoutValidation(@"t", time.ToString());

            return request;
        }
    }
}
