using mvcApp.Data;
using mvcApp.Models;

namespace mvcApp.Controllers
{
    public class SharedController
    {
        static DataContextDapper _dapper;

        public SharedController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public static int? getPostVotes(int? postId)
        {
            // Here I'm calculating votes by formula
            // Total Post votes = Upvotes - DownVotes

            if (postId == null) return 0;
            string sql = "select " +
                        " (select count(Id) from Votes " +
                        " where PostId= " + postId + " and VoteTypeId=(select Id from VoteTypes where Name='UpMod')) - " +
                        " (select count(Id) from Votes " +
                        " where PostId=" + postId + " and VoteTypeId=(select Id from VoteTypes where Name='DownMod')) as Votes;";
            int? val = _dapper.LoadDataSignal<int>(sql);

            if (val == null) return 0;
            return val;
        }

        public static string? getUserName(int? userId)
        {
            if (userId == null) return "";

            string sql = "select DisplayName from Users where Id=" + userId.ToString() + ";";
            List<string> name = _dapper.LoadData<string>(sql).ToList<string>();

            if (name.Count == 0) return "";
            return name[0];
        }

        public static int? getUserReputation(int? userId)
        {
            if (userId == null) return 0;

            string sql = "select Reputation from Users where Id = " + userId;
            List<int> rep = _dapper.LoadData<int>(sql).ToList<int>();

            if (rep.Count == 0) return 0;

            return rep[0];
        }

        public static string getUserBadges(int? userId)
        {
            // I have added all the badges in string and returning it to display it on UI,
            // for better UI experience if char length of all the badges are greater than 50 that I'm

            if (userId == null) return "";

            string sql = "select distinct name from Badges where UserId = " + userId;
            List<string> badgeList = _dapper.LoadData<string>(sql).ToList<string>();

            string badges = "";

            foreach (string bg in badgeList)
            {
                badges += bg + ", ";
            }

            if (badges.Length > 50)
            {
                badges = badges.Substring(0, 50);
                badges += "....";
            }

            return badges;
        }

        public static List<Post> getTop10Posts(string inputString, int pageNumber)
        {
            string[] words = inputString.Split(' ');
            string matchScoreStr = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] != "")
                {
                    matchScoreStr += "(CASE WHEN PATINDEX('%" + words[i] + "%', dbo.Posts.Title) > 0 THEN 1 ELSE 0 END) * 2 + " +
                    " (CASE WHEN PATINDEX('%" + words[i] + "%', dbo.Posts.Tags) > 0 THEN 1 ELSE 0 END) ";

                    if (i != words.Length - 1)
                        matchScoreStr += "+";
                }
            }

            string sql = @"SELECT *, " + matchScoreStr +
                            " AS MatchScore " +
                            " FROM dbo.Posts " +
                            " ORDER BY MatchScore DESC " +
                            " OFFSET " + 10 * pageNumber + " ROWS FETCH FIRST 10 ROWS ONLY;";

            List<Post> posts = _dapper.LoadData<Post>(sql).ToList();
            return posts;
        }

        public static List<Post> getAny10post()
        {
            Random rnd = new Random();
            int offset = rnd.Next(10000, 11000);
            string sql = "SELECT * from Posts where Title is not null Order By Id OFFSET " + offset + " ROWS FETCH FIRST 10 ROWS ONLY;";
            List<Post> posts = _dapper.LoadData<Post>(sql).ToList();
            return posts;
        }

        public static List<SearchPost> GetSearchPosts(string text, int pgNumber, bool getReq)
        {
            List<Post> posts = new List<Post>();

            if (getReq)
            {
                posts = getAny10post();
            }
            else
            {
                posts = getTop10Posts(text, pgNumber - 1);
            }

            List<SearchPost> searchPosts = new List<SearchPost>();

            foreach (Post post in posts)
            {
                SearchPost spost = new SearchPost();
                spost.Title = post.Title;
                spost.AuthorName = getUserName(post.OwnerUserId);
                spost.Badges = getUserBadges(post.OwnerUserId);
                spost.Reputation = getUserReputation(post.OwnerUserId);
                spost.votesCount = getPostVotes(post.Id);

                if (post.AnswerCount == null)
                    spost.AnswerCount = 0;
                else spost.AnswerCount = post.AnswerCount;

                spost.Body = post.Body;
                if (spost.Body != null && spost.Body.Length > 144)
                {
                    spost.Body = spost.Body.Substring(0, 140);
                    spost.Body += "...";
                }
                searchPosts.Add(spost);
            }
            return searchPosts;
        }
    }
}
