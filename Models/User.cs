

namespace TSF_mustidisProj.Models;
public class User
{
    public int Id {get;set;}
    public string Username {get;set;}
    public ICollection<Feed> Feeds {get;set;} = new List<Feed>();

}
