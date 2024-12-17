
namespace Nsu.HackathonProblem.Contracts
{

    public class TeamBuildingStrategy : ITeamBuildingStrategy
    {
        public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, 
                                            IEnumerable<Employee> juniors,
                                            IEnumerable<Wishlist> teamLeadsWishlists, 
                                            IEnumerable<Wishlist> juniorsWishlists)
        {
            // Словари для быстрого доступа к желаниям
            var teamLeadPreferences = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
            var juniorPreferences = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

            // Словарь для отслеживания пар
            var teamLeadMatches = new Dictionary<int, int?>(); // TeamLeadId -> JuniorId
            var juniorMatches = new Dictionary<int, int?>();   // JuniorId -> TeamLeadId

            foreach (var lead in teamLeads)
                teamLeadMatches[lead.Id] = null;

            foreach (var junior in juniors)
                juniorMatches[junior.Id] = null;

            var freeTeamLeads = new Queue<int>(teamLeads.Select(tl => tl.Id));

            // Алгоритм стабильного брака
            while (freeTeamLeads.Count > 0)
            {
                int teamLeadId = freeTeamLeads.Dequeue(); 
                var preferredJuniors = teamLeadPreferences[teamLeadId];

                foreach (var juniorId in preferredJuniors)
                {
                    if (juniorPreferences.ContainsKey(juniorId)) 
                    {
                        if (juniorMatches[juniorId] == null)
                        {
                            teamLeadMatches[teamLeadId] = juniorId;
                            juniorMatches[juniorId] = teamLeadId;
                            break;
                        }
                        else
                        {
                            int currentTeamLead = juniorMatches[juniorId].Value;
                            var juniorPreferenceList = juniorPreferences[juniorId];

                            if (juniorPreferenceList.IndexOf(teamLeadId) < juniorPreferenceList.IndexOf(currentTeamLead))
                            {
                                teamLeadMatches[teamLeadId] = juniorId;
                                juniorMatches[juniorId] = teamLeadId;

                                teamLeadMatches[currentTeamLead] = null;
                                freeTeamLeads.Enqueue(currentTeamLead);
                                break;
                            }
                        }
                    }
                }
            }

            // Формирование результата
            var teams = new List<Team>();
            foreach (var (teamLeadId, juniorId) in teamLeadMatches)
            {
                if (juniorId.HasValue)
                {
                    var teamLead = teamLeads.First(tl => tl.Id == teamLeadId);
                    var junior = juniors.First(j => j.Id == juniorId.Value);
                    teams.Add(new Team(teamLead, junior));
                }
            }

            return teams;
        }
    }
}
