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
        public static ISource BooruSource(this ABooru b)
        {
            return new BooruSource(b);
        }
        public static List<ISource> Build(this ActiveSources source, HttpClient client, string pixivrefresh){
            List<ISource> res = new();

            if(source.DanbooruDonmai){
                var q = new DanbooruDonmai();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.Yandere){
                var q = new Yandere();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.Gelbooru){
                var q = new Gelbooru();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.Pixiv){
                var q = new Pixiv();
                q.LoginAsync(pixivrefresh).Wait();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.SankakuComplex){
                var q = new SankakuComplex();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.Sakugabooru){
                var q = new Sakugabooru();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            if(source.Lolibooru){
                var q = new Lolibooru();
                q.HttpClient = client;
                res.Add(q.BooruSource());
            }
            

            return res;
        }
    }
}