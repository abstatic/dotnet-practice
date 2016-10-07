using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TeleprompterConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string name = Console.ReadLine();
            Console.WriteLine("Hello " + name);
            RunTeleprompter().Wait();
            var lines = ReadFrom("sampleQuotes.txt");

            foreach (var line in lines)
            {
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var pause = Task.Delay(200);
                    // Synchronously waiting on a task is an
                    // anti-pattern. This will get fixed in
                    // later steps.
                    pause.Wait();
                }
            }
        }
        
        private static async Task RunTeleprompter()
        {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);
            
            var speedTask = GetInput(config);
            await Task.WhenAny(displayTask, speedTask);
        }
        
        private static async Task GetInput(TelePrompterConfig config)
        {
            //var delay = 200;
            Action work = () =>
            {
                do 
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>')
                        config.UpdateDelay(-10);
                    else if (key.KeyChar == '<')
                        config.UpdateDelay(10);
                } while (!config.Done);
            };
            await Task.Run(work);
        }
        
        private static async Task ShowTeleprompter(TelePrompterConfig config)
        {
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var line in words)
            {
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await Task.Delay(config.DelayInMilliSeconds);
                }
            }
            config.SetDone();
        }
        
        static IEnumerable<string> ReadFrom(string file)
        {
            string line;
            using (var reader = File.OpenText(file))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(' ');
                    int lineLength = 0;
                    foreach (var word in words)
                    {
                        yield return word + " ";
                        lineLength += word.Length + 1;
                        if (lineLength > 70)
                        {
                            yield return Environment.NewLine;
                            lineLength = 0;
                        }
                    }
                    yield return Environment.NewLine;
                }
            }
        }
    }
}
