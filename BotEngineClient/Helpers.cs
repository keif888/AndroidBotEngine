using System;

namespace BotEngineClient
{
    public class Helpers
    {
        public static string CurrentMethodName([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }
    }
}
