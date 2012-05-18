using System;
using System.Net;
using AOPTutorial.Aspects;
using Newtonsoft.Json;

namespace AOPTutorial
{
    class Program
    {
        private static bool running = true;
        private static TwitterSearcher searcher = null;
        static void Main(string[] args)
        {
            searcher = new TwitterSearcher();
            while (running)
            {
                Console.WriteLine("Enter a search term, or type q to quit");
                var read = Console.ReadLine();
                if (read == "q")
                {
                    running = false;
                }
                else
                {
                    var results = searcher.GetTweet(read);
                    if (results == null)
                    {
                        Console.WriteLine("No results found");
                        continue;
                    }
                    Console.WriteLine(string.Format("Found {0} results", results.Results.Length));
                    foreach (var result in results.Results)
                    {
                        Console.WriteLine(string.Format(" - {0} said: {1}", result.from_user, result.Text));
                    }
                }
            }
        }
    }

    static class Console
    {
        public static string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public static void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        public static void WriteColorLine(string line, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(line);
            System.Console.ForegroundColor = ConsoleColor.White;
        }
    }

    [Serializable]
    class TwitterSearcher
    {
        private DateTime _start;

        [TracingAspect, LogExceptionAspect, ParallelAspect, CacheAspect]
        public SearchResults GetTweetWithAspects(string search)
        {
            //notice how much cleaner our code is now - it has only business logic
            //no one-off tracing or exception handling code
            //and we even added parallel and caching functionality!
            SearchResults results = null;

            if (search == "error")
                throw new ArgumentException("Forced Exception!");

            var json = new WebClient().DownloadString("http://search.twitter.com/search.json?q=" + search);
            results = JsonConvert.DeserializeObject<SearchResults>(json);

            return results;
        }

        public SearchResults GetTweet(string search)
        {
            //the old way - standard tracing and exception logging are intertwined with business logic
            //in a non-reusable way
            Console.WriteColorLine(string.Format("Starting search for {0}", search), ConsoleColor.Blue);
            _start = DateTime.Now;
            SearchResults results = null;
            try
            {
                var client = new WebClient();
                //these are literally the only 4 lines of business logic in the entire method
                if (search == "error")
                    throw new ArgumentException("Forced Exception!");
                var json = client.DownloadString("http://search.twitter.com/search.json?q=" + search);
                results = JsonConvert.DeserializeObject<SearchResults>(json);
                Console.WriteColorLine(string.Format("Successfully searched twitter, got {0} results", results.Results.Length), ConsoleColor.Yellow);
            }
            catch (ArgumentException ex)
            {
                Console.WriteColorLine(string.Format("Exception! {0}", ex.Message), ConsoleColor.Red);
            }
            finally
            {
                Console.WriteColorLine(string.Format("Finished search for {0}", search), ConsoleColor.Blue);
                Console.WriteColorLine(string.Format("Search took {0}", DateTime.Now.Subtract(_start)), ConsoleColor.Green);
            }
            return results;
        }
    }

    class SearchResults
    {
        public Tweet[] Results { get; set; }
    }

    class Tweet
    {
        public string from_user { get; set; }
        public string Text { get; set; }
    }
}
