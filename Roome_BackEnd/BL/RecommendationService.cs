using Roome_BackEnd.BL;
using System.Dynamic;
using Newtonsoft.Json;

public class RecommendationService
{
    private static double CalculateUserSimilarity(User u1, User u2)
    {
        // Weights for user attribute similarity components
        double weightJobStatus = 0.3;
        double weightOwnPet = 0.15;
        double weightSmoke = 0.15;
        double weightAge = 0.4;

        double score = 0;

        // Increase score if users have the same job status
        if (u1.JobStatus == u2.JobStatus)
            score += weightJobStatus;

        // Increase score if users both own (or don't own) pets
        if (u1.OwnPet == u2.OwnPet)
            score += weightOwnPet;

        // Increase score if users have the same smoking habits
        if (u1.Smoke == u2.Smoke)
            score += weightSmoke;

        // Increase score if age difference is 5 years or less
        int ageDiff = Math.Abs((u1.BirthDate - u2.BirthDate).Days / 365);
        if (ageDiff <= 5)
            score += weightAge;

        return score;
    }

    private static double CalculateCombinedSimilarity(User target, User other)
    {
        // Get sets of liked apartment IDs for both users
        var targetLikes = target.GetUserLikedApartments(target.ID)
            .Select(a => (int)((IDictionary<string, object>)a)["ApartmentID"]).ToHashSet();

        var otherLikes = other.GetUserLikedApartments(other.ID)
            .Select(a => (int)((IDictionary<string, object>)a)["ApartmentID"]).ToHashSet();

        // Calculate Jaccard similarity for liked apartments
        int intersection = targetLikes.Intersect(otherLikes).Count();
        int union = targetLikes.Union(otherLikes).Count();
        double likeSimilarity = union == 0 ? 0 : (double)intersection / union;

        // Calculate similarity based on user attributes
        double userAttributeSimilarity = CalculateUserSimilarity(target, other);

        // Combine similarities with given weights
        double weightUserAttributes = 0.6;
        double weightLikes = 0.4;

        return weightUserAttributes * userAttributeSimilarity + weightLikes * likeSimilarity;
    }

    private static List<(User user, double score)> GetSimilarUsers(int userId, List<User> allUsers)
    {
        var targetUser = allUsers.First(u => u.ID == userId);
        var similarUsers = new List<(User user, double score)>();

        // Compute similarity score for all other users
        foreach (var otherUser in allUsers)
        {
            if (otherUser.ID == userId) continue;

            double similarity = CalculateCombinedSimilarity(targetUser, otherUser);
            if (similarity > 0)
                similarUsers.Add((otherUser, similarity));
        }

        // Return top 5 similar users sorted by similarity score descending
        return similarUsers.OrderByDescending(s => s.score).Take(5).ToList();
    }

    private static double CalculateContentScore(dynamic likedApartment, dynamic candidate)
    {
        double score = 0;

        var likedDict = likedApartment as IDictionary<string, object>;
        var candidateDict = candidate as IDictionary<string, object>;

        if (likedDict != null && candidateDict != null)
        {
            if (likedDict.ContainsKey("Location") && candidateDict.ContainsKey("Location"))
            {
                if ((string)likedDict["Location"] == (string)candidateDict["Location"])
                    score += 1;
            }
            if (likedDict.ContainsKey("AllowPet") && candidateDict.ContainsKey("AllowPet"))
            {
                if ((bool)likedDict["AllowPet"] == (bool)candidateDict["AllowPet"])
                    score += 1;
            }
            if (likedDict.ContainsKey("AllowSmoking") && candidateDict.ContainsKey("AllowSmoking"))
            {
                if ((bool)likedDict["AllowSmoking"] == (bool)candidateDict["AllowSmoking"])
                    score += 1;
            }
        }

        return score;
    }

    // Returns all apartments the user hasn't liked, ordered by combined recommendation score
    public static List<dynamic> GetHybridRecommendations(int userId)
    {
        var allUsers = User.GetAllUser();
        List<Dictionary<string, object>> allApartments = ApartmentService.GetAllActiveApartments(userId);
        var user = allUsers.First(u => u.ID == userId);

        var likedRaw = user.GetUserLikedApartments(userId).ToList();

        Console.WriteLine($"Total apartments count: {allApartments.Count}");

        Console.WriteLine("Liked apartments IDs:");
        // Collect IDs of apartments the user liked
        var liked = likedRaw
            .Select(a =>
            {
                int id = (int)((IDictionary<string, object>)a)["ApartmentID"];
                Console.WriteLine(id);
                return id;
            })
            .ToHashSet();

        // Calculate content-based scores for apartments the user hasn't liked
        var contentScores = new Dictionary<int, double>();
        foreach (var apartment in allApartments)
        {
            int aptId = Convert.ToInt32(apartment["ApartmentID"]);
            if (liked.Contains(aptId)) continue;

            double totalScore = 0;
            foreach (var likedApt in likedRaw)
            {
                totalScore += CalculateContentScore(likedApt, apartment);
            }

            contentScores[aptId] = totalScore;
        }

        // Get similar users and prepare collaborative scores for apartments
        var similarUsers = GetSimilarUsers(userId, allUsers);
        var collaborativeScores = new Dictionary<int, double>();

        Console.WriteLine("Similar users and similarity scores:");
        foreach (var (similarUser, similarityScore) in similarUsers)
        {
            Console.WriteLine($"UserID: {similarUser.ID}, Similarity: {similarityScore}");
            foreach (var apt in similarUser.GetUserLikedApartments(similarUser.ID))
            {
                int aptId = (int)((IDictionary<string, object>)apt)["ApartmentID"];
                // Ignore apartments the target user already liked
                if (liked.Contains(aptId)) continue;

                if (!collaborativeScores.ContainsKey(aptId))
                    collaborativeScores[aptId] = 0;

                // Accumulate similarity scores for each apartment liked by similar users
                collaborativeScores[aptId] += similarityScore;
            }
        }

        // Calculate final combined scores for all apartments the user hasn't liked
        var finalScores = new Dictionary<int, double>();
        foreach (var apartment in allApartments)
        {
            int aptId = Convert.ToInt32(apartment["ApartmentID"]);
            if (liked.Contains(aptId)) continue;

            double collabScore = collaborativeScores.GetValueOrDefault(aptId, 0);
            double contentScore = contentScores.GetValueOrDefault(aptId, 0);

            // Weighted sum of collaborative and content scores
            double combined = 0.6 * collabScore + 0.4 * contentScore;
            finalScores[aptId] = combined;
        }

        Console.WriteLine("Final scores per apartment:");
        foreach (var kvp in finalScores)
        {
            Console.WriteLine($"ApartmentID: {kvp.Key}, Score: {kvp.Value}");
        }

        if (!finalScores.Any())
        {
            Console.WriteLine("No recommended apartments found.");
            return new List<dynamic>();
        }

        // Return all apartments not liked by the user ordered by combined score descending
        return finalScores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp =>
            {
                var dict = allApartments.First(a => Convert.ToInt32(a["ApartmentID"]) == kvp.Key);
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (var kv in dict)
                    expando[kv.Key] = kv.Value;
                return (dynamic)expando;
            })
            .ToList();
    }
}
