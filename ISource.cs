using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rever
{
    //интерфейс который представляет собой источник изображений
    public interface ISource
    {
        //может ли источник искать по тегам?
        bool UseTags{get;}
        //есть ли у источника ограничение на максимальное количество тегов?
        bool NoMoreThanTwoTags {get;}
        //метод запроса к апи и получения результата
        Task<SourceResult> GetApiResult(params string[] tags);
        //домашняя страница сервиса
        string BaseUrl {get;}
        //передаёт httpclient (согласно документации лучшая практика работы с ним - один обьект на весь жизненный путь приложения)
        HttpClient Client {get;set;}
        // передаёт random (он тоже используется один для всего жиненного цикал)
        Random Random {get;set;}
    }
}