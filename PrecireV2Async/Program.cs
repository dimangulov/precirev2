using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Precire;

namespace PrecireV2Async
{
    class Program
    {
        private const string key = "XXX";
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var client = new redoc_v2_textsClient(httpClient);

            //Console.WriteLine("AnalyzeText");
            //var r1 = await AnalyzeText(client);
            Console.WriteLine("AnalyzeDoc");
            var r2 = await AnalyzeDoc(client);
        }

        private static async Task<Response2> AnalyzeText(redoc_v2_textsClient client)
        {
            var r1 = await client.V2TextsPostAsync(ContentLanguage.En, ContentType.Application_json, key, new Body
            {
                Text = new Text4 {Content = "Hello world!", Reference = Text4Reference.Default, Type = Text4Type.String},
                Results = new List<Results5>
                {
                    new Results5
                    {
                        Name = Results5Name.Formal
                    }
                }
            });

            return await WaitResults(client, r1);
        }

        private static async Task<Response2> AnalyzeDoc(redoc_v2_textsClient client)
        {
            var bytes = File.ReadAllBytes(@"testdoc.docx");
            var base64 = Convert.ToBase64String(bytes);

            var r1 = await client.V2TextsPostAsync(ContentLanguage.En, ContentType.Application_json, key, new Body
            {
                Text = new Text4 { Content = base64, Reference = Text4Reference.Default, Type = Text4Type.Docx },
                Results = new List<Results5>
                {
                    new Results5
                    {
                        Name = Results5Name.Formal
                    }
                }
            });

            return await WaitResults(client, r1);
        }

        private static async Task<Response2> WaitResults(redoc_v2_textsClient client, Response r1)
        {
            while (true)
            {
                Console.WriteLine("Wait a sec...");
                await Task.Delay(1);

                var state = await client.V2TextsIdGetAsync(r1.Text.Id, ContentType3.Application_json, key);

                Log(state);

                if (state.Text.State == Text6State.Finished)
                {
                    return state;
                    break;
                }
            }
        }

        private static void Log(Response2 r1)
        {
            Console.WriteLine($"({r1.Text.Id}) - State - {r1.Text.State}");
        }
    }
}
