public class RecommendationService
{
    private static double CalculateUserSimilarity(User u1, User u2)
    {
        double weightJobStatus = 0.3;
        double weightOwnPet = 0.15;
        double weightSmoke = 0.15;
        double weightAge = 0.4;

        double score = 0;

        if (u1.JobStatus == u2.JobStatus)
            score += weightJobStatus;

        if (u1.OwnPet == u2.OwnPet)
            score += weightOwnPet;

        if (u1.Smoke == u2.Smoke)
            score += weightSmoke;

        int ageDiff = Math.Abs((u1.BirthDate - u2.BirthDate).Days / 365);
        if (ageDiff <= 5)
            score += weightAge;

        return score;
    }

    private static double CalculateCombinedSimilarity(User target, User other)
    {
        var targetLikes = target.GetUserLikedApartments(target.ID).Select(a => (int)a.id).ToHashSet();
        var otherLikes = other.GetUserLikedApartments(other.ID).Select(a => (int)a.id).ToHashSet();

        int intersection = targetLikes.Intersect(otherLikes).Count();
        int union = targetLikes.Union(otherLikes).Count();
        double likeSimilarity = union == 0 ? 0 : (double)intersection / union;

        double userAttributeSimilarity = CalculateUserSimilarity(target, other);

        double weightUserAttributes = 0.6;
        double weightLikes = 0.4;

        return weightUserAttributes * userAttributeSimilarity + weightLikes * likeSimilarity;
    }

    private static List<(User user, double score)> GetSimilarUsers(int userId, List<User> allUsers)
    {
        var targetUser = allUsers.First(u => u.ID == userId);
        var similarUsers = new List<(User user, double score)>();

        foreach (var otherUser in allUsers)
        {
            if (otherUser.ID == userId) continue;

            double similarity = CalculateCombinedSimilarity(targetUser, otherUser);
            if (similarity > 0)
                similarUsers.Add((otherUser, similarity));
        }

        return similarUsers.OrderByDescending(s => s.score).Take(5).ToList();
    }

    private static double CalculateContentScore(dynamic likedApartment, dynamic candidate)
    {
        double score = 0;

        if (likedApartment.Location == candidate.Location) score += 1;
        if (likedApartment.AllowPet == candidate.AllowPet) score += 1;
        if (likedApartment.AllowSmoking == candidate.AllowSmoking) score += 1;
        if (likedApartment.PropertyTypeID == candidate.PropertyTypeID) score += 1;

        return score;
    }

    public static List<dynamic> GetHybridRecommendations(int userId)
    {
        var allUsers = User.GetAllUser();
        var allApartments = ApartmentService.GetAllApartments();
        var user = allUsers.First(u => u.ID == userId);
        var liked = user.GetUserLikedApartments(userId).Select(a => (int)a.id).ToHashSet();

        var contentScores = new Dictionary<int, double>();
        foreach (var apartment in allApartments)
        {
            if (liked.Contains((int)apartment.id)) continue;

            double totalScore = 0;
            foreach (var likedApt in user.GetUserLikedApartments(userId))
            {
                totalScore += CalculateContentScore(likedApt, apartment);
            }

            contentScores[(int)apartment.id] = totalScore;
        }

        var similarUsers = GetSimilarUsers(userId, allUsers);
        var collaborativeScores = new Dictionary<int, double>();

        foreach (var (similarUser, similarityScore) in similarUsers)
        {
            foreach (var apt in similarUser.GetUserLikedApartments(similarUser.ID))
            {
                int aptId = (int)apt.id;
                if (liked.Contains(aptId)) continue;

                if (!collaborativeScores.ContainsKey(aptId))
                    collaborativeScores[aptId] = 0;

                collaborativeScores[aptId] += similarityScore;
            }
        }

        var finalScores = new Dictionary<int, double>();
        foreach (var apartment in allApartments)
        {
            int aptId = (int)apartment.id;
            if (liked.Contains(aptId)) continue;

            double collabScore = collaborativeScores.GetValueOrDefault(aptId, 0);
            double contentScore = contentScores.GetValueOrDefault(aptId, 0);

            double combined = 0.6 * collabScore + 0.4 * contentScore;
            finalScores[aptId] = combined;
        }

        return finalScores
            .OrderByDescending(kvp => kvp.Value)
            .Take(10)
            .Select(kvp => allApartments.First(a => (int)a.id == kvp.Key))
            .ToList();
    }
}
