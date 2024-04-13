

namespace API.Helpers
{
    public class LikeParam :PaginationParam
    {
       public string Predicate {get; set; }
       public  int UserId {get; set; }


    }
}