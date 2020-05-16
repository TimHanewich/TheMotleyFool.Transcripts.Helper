using TheMotleyFool.Transcripts;
using System;
using System.Collections.Generic;

namespace TheMotleyFool.Transcripts.Helper
{
    public static class TheMotleyFoolTrascriptsHelper
    {
        public static bool IsEmployee(this CallParticipant participant)
        {
            
            
            if (participant.Title == null) //If It is null, it is an external participant
            {
                return false;
            }
            
            if (participant.Title.ToLower() == "operator" || participant.Name.ToLower() == "operator") //If it is the operator
            {
                return false;
            }

            if (participant.Title.Contains("--"))
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
    
        public static string[] GetSentences(this Transcript trans, bool employee_only = false)
        {
            List<string> Splitter = new List<string>();
            Splitter.Add(". ");
            List<string> sens = new List<string>();
            foreach (Remark r in trans.Remarks)
            {
                
                bool should_include = true;
                if (employee_only == false)
                {
                    should_include = true;
                }
                else
                {
                    if (r.Speaker.IsEmployee())
                    {
                        should_include = true;
                    }
                    else
                    {
                        should_include = false;
                    }
                }

                if (should_include)
                {
                    foreach (string s in r.SpokenRemarks)
                    {
                        string[] sentencs = s.Split(Splitter.ToArray(), StringSplitOptions.None);
                        foreach (string sen in sentencs)
                        {
                            sens.Add(sen.Trim());
                        }
                    }
                }
                
            }
            return sens.ToArray();
        }
        
        public static SentenceValuePair[] RankSentences(this Transcript trans)
        {
            List<WordValuePair> WordVals = new List<WordValuePair>();
            WordVals.Add(new WordValuePair("revenue", 3));
            WordVals.Add(new WordValuePair("net income", 3));
            WordVals.Add(new WordValuePair("earnings per share", 4));
            WordVals.Add(new WordValuePair("record", 2));
            WordVals.Add(new WordValuePair("growth", 2));
            WordVals.Add(new WordValuePair("$", 4));
            WordVals.Add(new WordValuePair("%", 5));
            WordVals.Add(new WordValuePair("revenue grew", 8));
            WordVals.Add(new WordValuePair("revenue fell", 8));
            WordVals.Add(new WordValuePair("inncome grew", 8));
            WordVals.Add(new WordValuePair("income fell", 8));
            WordVals.Add(new WordValuePair("increase in volume", 2));
            WordVals.Add(new WordValuePair("decrease in volume", 2));
            WordVals.Add(new WordValuePair("brought down our cost", 4));
            WordVals.Add(new WordValuePair("revenue of", 3));
            WordVals.Add(new WordValuePair("income of", 3));
            WordVals.Add(new WordValuePair("cash flow", 5));

            List<string> Splitter = new List<string>();
            

            //Make the rankings
            string[] sentences = trans.GetSentences(true);
            List<SentenceValuePair> sentence_ratings =  new List<SentenceValuePair>();
            foreach (string s in sentences)
            {
                SentenceValuePair srp = new SentenceValuePair();
                srp.Sentence = s;
                
                //Get rating
                srp.Rating = 0f;
                foreach (WordValuePair wvp in WordVals)
                {
                    Splitter.Clear();
                    Splitter.Add(wvp.word_.ToLower());
                    string[] split = s.ToLower().Split(Splitter.ToArray(), StringSplitOptions.None);
                    srp.Rating = srp.Rating + ((split.Length-1) * wvp.value_);
                }
                sentence_ratings.Add(srp);
            }


            //Rank them
            List<SentenceValuePair> ranked = new List<SentenceValuePair>();
            do
            {
                SentenceValuePair winner = sentence_ratings[0];
                foreach (SentenceValuePair svp in sentence_ratings)
                {
                    if (svp.Rating > winner.Rating)
                    {
                        winner = svp;
                    }
                }
                ranked.Add(winner);
                sentence_ratings.Remove(winner);
            } while (sentence_ratings.Count > 0);


            return ranked.ToArray();

        }

        private class WordValuePair
        {
            public string word_;
            public float value_;
            
            public WordValuePair(string word, float value)
            {
                word_ = word;
                value_ = value;
            }
        }
    }
}
