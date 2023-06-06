using System;
using System.Collections.Generic;
using System.Net.Http;
using BooruSharp.Booru;
using BooruSharp.Others;

namespace rever{
    //класс, который содержит различные токены
    public class Tokens{
        public string PixivRefreshToken {get;set;}
        public string VkAccessToken {get;set;}
    }
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

        public bool AnimePictures {get;set;}
        public bool Rule34 {get;set;}
        public bool VkGroups {get;set;}
    }

    //фабрика создания обьектов boorusharp из ActiveSource обьекта
    public static class BoooruFactory{
        
        public static ISource BooruSource(this ABooru b)
        {
            // если мы передаём new Pixiv, то мы создаём иной объект для работы с апи
            if(b is Pixiv)
                return new PixivSource(b);
            return new BooruSource(b);
        }
        public static List<ISource> Build(this ActiveSources source, HttpClient client, Random r, Tokens tokens){
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
                q.LoginAsync(tokens.PixivRefreshToken).Wait();
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
            
            if(source.AnimePictures){
                var q = new AnimePictures();
                q.Client = client;
                q.Random = r;
                res.Add(q);
            }
            if(source.Rule34){
                var q = new Rule34Source();
                q.Client = client;
                q.Random = r;
                res.Add(q);
            }
            if(source.VkGroups){
                var q = new VKSource(tokens.VkAccessToken);
                q.Client = client;
                q.Random = r;
                res.Add(q);
            }

            return res;
        }
    }
}