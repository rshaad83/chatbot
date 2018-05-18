using System.Collections.Generic;

namespace FDCBot
{
    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
  /*
   *public class EchoState
    {
        public int TurnCount { get; set; } = 0;
    }

    */
    /// <summary>
    /// Class for storing conversation state.
    /// This bot only stores the turn count in order to echo it to the user
    /// </summary>
    public class EchoState : Dictionary<string, object>
    {
        private const string TurnCountKey = "TurnCount";
        public EchoState()
        {
            this[TurnCountKey] = 0;
        }

        public int TurnCount
        {
            get { return (int)this[TurnCountKey]; }
            set { this[TurnCountKey] = value; }
        }
    }
}
