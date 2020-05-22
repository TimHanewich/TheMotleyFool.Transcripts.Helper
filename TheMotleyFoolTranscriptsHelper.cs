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
            
        public static CallParticipant WhoSaid(this Transcript trans, string quote)
        {
            foreach (Remark r in trans.Remarks)
            {
                foreach (string s in r.SpokenRemarks)
                {
                    if (s.ToLower().Contains(quote.ToLower()))
                    {
                        return r.Speaker;
                    }
                }
            }

            throw new Exception("Unable to find speaker of supplied quote '" + quote + "'.");
        }
    
        public static string GetTradingSymbol(this Transcript trans)
        {
            int loc1 = trans.Title.LastIndexOf("(");
            if (loc1 == -1)
            {
                throw new Exception("Unable to find symbol from title.");
            }
            int loc2 = trans.Title.IndexOf(")", loc1+1);
            string symbol = trans.Title.Substring(loc1 + 1, loc2 - loc1 - 1);
            return symbol;
        }
    }
}
