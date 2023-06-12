namespace rever
{
    public class VKAlbum
    {
        string owner_id = "";
        string album_id = "";
        public VKAlbum(string raw){
            var a = raw.Split();
            owner_id = a[0];
            album_id = a[1];
        }
        public override string ToString()
        {
            return $"&album_id={album_id}&owner_id={owner_id}";
        }
        public string GetAlbum{get{return owner_id;}}
    }
}