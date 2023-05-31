using System;

namespace rever
{
    //представляет собой результат вызова апи и используется как возвращаемый тип из ISource
    public class SourceResult
    {
        public string FileUrl{get;init;}
        public string PostUrl{get;init;}
        public string[] Tags{get;init;}
        public int Rating {get;init;}
    }
}