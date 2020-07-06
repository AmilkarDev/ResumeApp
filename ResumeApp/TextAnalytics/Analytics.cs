using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Azure;
using Azure.AI.TextAnalytics;
using ResumeApp.Models;
namespace ResumeApp.TextAnalytics
{
    public static class Analytics
    {
        static string endpoint = "https://resumeappalytics.cognitiveservices.azure.com/";
        static string apiKey = "58dfe5d121844f0780d00f5e0fa6b5b9";


        public static async Task<List<Entity>> ExtractEntities(List<string> strr)
        {        
            var client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            var documents = new List<TextDocumentInput>();
            var finalResult = new List<Entity>();
            int id = 0;
            foreach (var item in strr)
            {
                documents.Add(new TextDocumentInput(id.ToString(), item)
                {
                    Language = "fr",
                });
                id++;
            }
            RecognizeEntitiesResultCollection results = await client.RecognizeEntitiesBatchAsync(documents, new TextAnalyticsRequestOptions { IncludeStatistics = true });
            int i = 0;
            foreach (RecognizeEntitiesResult result in results)
            {
                TextDocumentInput document = documents[i++];

                //Console.WriteLine($"On document (Id={document.Id}, Language=\"{document.Language}\", Text=\"{document.Text}\"):");

                if (result.HasError)
                {
                    return finalResult;
                    //Console.WriteLine($"    Document error code: {result.Error.Code}.");
                    //Console.WriteLine($"    Message: {result.Error.Message}.");
                }
                else
                {
                   // Console.WriteLine($"    Recognized the following {result.Entities.Count()} entities:");

                    foreach (CategorizedEntity entity in result.Entities)
                    {
                        //Console.WriteLine($"        Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
                        Entity ent = new Entity { Text = entity.Text, Type = entity.Category.ToString(), Subtype = entity.SubCategory, Score = entity.ConfidenceScore , Offset = entity.GraphemeOffset, Length=entity.GraphemeLength };
                        finalResult.Add(ent);
                    }

                    //Console.WriteLine($"    Document statistics:");
                    //Console.WriteLine($"        Character count (in Unicode graphemes): {result.Statistics.CharacterCount}");
                    //Console.WriteLine($"        Transaction count: {result.Statistics.TransactionCount}");
                    //Console.WriteLine("");
                }
            }


                return finalResult;
        }
        public static async  Task<List<KeyPhrase>> ExtractKeyPhrases(List<string> strr)
        {

           
            var client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            var documents = new List<TextDocumentInput>();
            var finalResult = new List<KeyPhrase>();
            int id = 0;
            foreach (var item in strr)
            {
                documents.Add(new TextDocumentInput(id.ToString(), item)
                {
                    Language = "fr",
                });
                id++;
            }
            ExtractKeyPhrasesResultCollection results = await client.ExtractKeyPhrasesBatchAsync(documents, new TextAnalyticsRequestOptions { IncludeStatistics = true });

            int i = 0;
            foreach (ExtractKeyPhrasesResult result in results)
            {
                //TextDocumentInput document = documents[i++];

                //Console.WriteLine($"On document (Id={document.Id}, Language=\"{document.Language}\", Text=\"{document.Text}\"):");

                if (result.HasError)
                {
                    return finalResult;
                    //Console.WriteLine($"    Document error: {result.Error.ErrorCode}.");
                    //Console.WriteLine($"    Message: {result.Error.Message}.");
                }
                else
                {
                    Console.WriteLine($"    Extracted the following {result.KeyPhrases.Count()} key phrases:");

                    foreach (string keyPhrase in result.KeyPhrases)
                    {
                        finalResult.Add(new KeyPhrase { Text = keyPhrase });
                        //Console.WriteLine($"        {keyPhrase}");
                    }

                    //Console.WriteLine($"    Document statistics:");
                    //Console.WriteLine($"        Character count (in Unicode graphemes): {result.Statistics.CharacterCount}");
                    //Console.WriteLine($"        Transaction count: {result.Statistics.TransactionCount}");
                    //Console.WriteLine("");
                }
            }

            //Console.WriteLine($"Batch operation statistics:");
            //Console.WriteLine($"    Document count: {results.Statistics.DocumentCount}");
            //Console.WriteLine($"    Valid document count: {results.Statistics.ValidDocumentCount}");
            //Console.WriteLine($"    Invalid document count: {results.Statistics.InvalidDocumentCount}");
            //Console.WriteLine($"    Transaction count: {results.Statistics.TransactionCount}");
            //Console.WriteLine("");
            return finalResult;
        }
    }
}