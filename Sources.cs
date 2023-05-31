using System;
using System.Collections.Generic;
using System.Net.Http;
using BooruSharp.Booru;
using BooruSharp.Others;

namespace rever{
    //класс, который описывает использующиеся источники картинок
    public class ActiveSources{
        public bool Pixiv {get;set;}
        public bool Yandere {get;set;}
        public bool Gelbooru {get;set;}
        public bool DanbooruDonmai {get;set;}
        public bool SankakuComplex {get;set;}
        ///<summary>DON`T RECCOMEND TO USE</summary>
        public bool Sakugabooru {get;set;}
        public bool Lolibooru {get;set;}
    }

    //фабрика создания обьектов boorusharp из ActiveSource обьекта
    public static class BoooruFactory{
        public static List<ABooru> Build(this ActiveSources source, HttpClient client, string pixivrefresh){
            List<ABooru> res = new();

            if(source.DanbooruDonmai){
                res.Add(new DanbooruDonmai());
                res[^1].HttpClient = client;
            }
            if(source.Yandere){
                res.Add(new Yandere());
                res[^1].HttpClient = client;
            }
            if(source.Gelbooru){
                res.Add(new Gelbooru());
                res[^1].HttpClient = client;
            }
            if(source.Pixiv){
                res.Add(new Pixiv());
                ((Pixiv)res[^1]).LoginAsync(pixivrefresh).Wait();
                res[^1].HttpClient = client;
            }
            if(source.SankakuComplex){
                res.Add(new SankakuComplex());
                res[^1].HttpClient = client;
            }
            if(source.Sakugabooru){
                res.Add(new Sakugabooru());
                res[^1].HttpClient = client;
            }
            if(source.Lolibooru){
                res.Add(new Lolibooru());
                res[^1].HttpClient = client;
            }
            

            return res;
        }
    }
}